using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキストファイルの行情報
    //=========================================================================================
    public class TextBufferLineInfoList {
        // 解析対象のファイル情報
        private IFileViewerDataSource m_accessibleFile;

        // ファイルビューアのパネル
        private TextFileViewer m_viewerPanel;

        // 折り返し設定
        private TextViewerLineBreakSetting m_lineBreakSetting;

        // 折り返し位置：１行分の画面のサイズ（ピクセル）
        private SizeF m_screenLinePixelSize;

        // 折り返し位置：画面の幅（文字数）
        private int m_screenCharWidth;
        
        // タブ幅設定
        private int m_tabWidth = 0;

        // テキストビューアで行番号を表示するときtrue
        private bool m_isDisplayLineNumber;

        // テキストビューアで表示する最大行数
        private int m_maxLineCount;

        // テキストの最大ピクセル幅
        private float m_maxTextPixelWidth = 0;

        // 半角１文字分の大きさの期待値
        private SizeF m_fontSize;

        // 画面幅
        private float m_screenWidth;

        // 行を解析するときのエンコーディング
        private EncodingType m_lineEncodingMode = null;

        // バイナリファイルと判定されたときtrue
        private bool m_isBinary;

        // ファイル解析が完了したときtrue
        private bool m_isParseEnd = false;

        // 行数オーバーのエラーメッセージを表示したときtrue
        private bool m_lineCountOverDisplayed = false;

        // 行バッファ
        private List<TextBufferLogicalLineInfo> m_lineList = new List<TextBufferLogicalLineInfo>();

        // 物理行（1,2,3…）から行バッファ内の論理行（0,1,2…）への対応
        private List<int> m_physicalLineToLineListIndex = new List<int>();

        // 次に解析を開始するAccessibleFileのインデックス位置
        private int m_nextParseStartPosition = 0;

        // 次の物理行番号
        private int m_nextPhysicalLineNo = 1;

        // 検索キーワードにヒットした件数
        private int m_searchHitCount = 0;

        // 自動検索キーワードにヒットした件数
        private int m_autoSearchHitCount = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]accessibleFile  解析対象のファイル
        // 　　　　[in]form            ファイルビューアのパネル
        // 　　　　[in]fontSize        半角１文字分の大きさの期待値
        // 　　　　[in]screenWidth     画面幅
        // 　　　　[in]lineBreak       折り返し設定
        // 　　　　[in]tabWidth        テキストの最大ピクセル幅
        // 　　　　[in]isDispLineNo    テキストビューアで行番号を表示するときtrue
        // 　　　　[in]maxLineCount    テキストビューアで表示する最大行数
        // 戻り値：なし
        //=========================================================================================
        public TextBufferLineInfoList(IFileViewerDataSource accessibleFile, TextFileViewer viewerPanel, SizeF fontSize, float screenWidth, TextViewerLineBreakSetting lineBreak, int tabWidth, bool isDispLineNo, int maxLineCount) {
            m_accessibleFile = accessibleFile;
            m_viewerPanel = viewerPanel;
            m_fontSize = fontSize;
            m_screenWidth = screenWidth;

            // 設定を保存
            m_lineBreakSetting = lineBreak;
            m_tabWidth = tabWidth;
            m_isDisplayLineNumber = isDispLineNo;
            m_maxLineCount = maxLineCount;
        }

        //=========================================================================================
        // 機　能：チャンク情報を解析する
        // 引　数：なし
        // 戻り値：初期化されたときtrue
        //=========================================================================================
        public bool ParseChunk() {
            bool initialized = false;
            lock (this) {
                // チャンクが小さすぎかどうか判定
                int readSize;
                byte[] readBuffer;
                m_accessibleFile.GetBuffer(out readBuffer, out readSize);
                if (m_accessibleFile.Status == RetrieveDataLoadStatus.Loading && readSize < m_accessibleFile.MinParseStartSize) {
                    return false;
                }
                
                TextViewerGraphics g = new TextViewerGraphics(m_viewerPanel, 0);
                try {
                    // 初回の実行で初期化
                    if (m_lineEncodingMode == null) {
                        // エンコーディングを初期化
                        initialized = true;
                        JudgeEncoding();

                        // 画面幅を初期化
                        InitializeWidthInfo(g);

                        m_physicalLineToLineListIndex.Clear();
                        m_physicalLineToLineListIndex.Add(0);
                    }

                    // 行単位に分解
                    if (!m_isParseEnd) {
                        SeparateLineInfo(g);
                    }
                } finally {
                    g.Dispose();
                }
            }
            return initialized;
        }

        //=========================================================================================
        // 機　能：再度解析を行う
        // 引　数：[in]画面幅も初期化するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void ResetParse(bool resetWidth) {
            lock (this) {
                m_isParseEnd = false;
                m_lineList.Clear();
                m_physicalLineToLineListIndex.Clear();
                m_physicalLineToLineListIndex.Add(0);
                m_nextParseStartPosition = 0;
                m_nextPhysicalLineNo = 1;
                m_lineCountOverDisplayed = false;

                TextViewerGraphics g = new TextViewerGraphics(m_viewerPanel, 0);
                try {
                    if (resetWidth) {
                        InitializeWidthInfo(g);
                    }
                    SeparateLineInfo(g);
                } finally {
                    g.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：エンコーディングを決める
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void JudgeEncoding() {
            EncodingType encoding = m_accessibleFile.DefaultEncoding;
            if (encoding == null) {
                byte[] readBuffer;
                int readSize;
                m_accessibleFile.GetBuffer(out readBuffer, out readSize);
                EncodingChecker encodingChecker = new EncodingChecker(readBuffer, readSize);
                encoding = encodingChecker.CheckEncodingType();
            }
            if (encoding == EncodingType.BINARY) {
                m_lineEncodingMode = EncodingType.SJIS;
                m_isBinary = true;
            } else {
                m_lineEncodingMode = encoding;
                m_isBinary = false;
            }
        }

        //=========================================================================================
        // 機　能：画面幅の情報を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializeWidthInfo(TextViewerGraphics g) {
            if (m_lineBreakSetting.LineBreakMode == TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar) {
                // 文字数
                if (m_lineBreakSetting.BreakCharCount <= 2) {
                    float width = GetScreenWidth(m_lineBreakSetting.BreakCharCount);
                    string testStr = "";
                    m_screenCharWidth = GraphicsUtils.GetCharWidth(g.Graphics, g.TextFont, width - 1, 3000);
                    testStr = StringUtils.Repeat("M", m_screenCharWidth);
                    m_maxTextPixelWidth = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, testStr);
                } else {
                    string testStr = StringUtils.Repeat("M", m_lineBreakSetting.BreakCharCount);
                    m_screenCharWidth = m_lineBreakSetting.BreakCharCount;
                    m_maxTextPixelWidth = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, testStr);
                }
            } else if (m_lineBreakSetting.LineBreakMode == TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByPixel) {
                // ピクセル数
                SizeF sizeTest = g.Graphics.MeasureString("1Aあ", g.TextFont);
                if (m_lineBreakSetting.BreakPixel <= 2) {
                    float width = GetScreenWidth(m_lineBreakSetting.BreakCharCount);
                    m_screenLinePixelSize = new SizeF(width, (int)(sizeTest.Height + 1));
                } else {
                    m_screenLinePixelSize = new SizeF(m_lineBreakSetting.BreakPixel, (int)(sizeTest.Height + 1));
                    m_maxTextPixelWidth = m_screenLinePixelSize.Width;
                }
            } else {
                // 折り返しなし
                m_maxTextPixelWidth = GetScreenWidth(0);
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウの幅を取得する
        // 引　数：[in]widthSetting   ウィンドウ幅の設定（0:ウィンドウ幅、1:画面幅、全画面幅:2）
        // 戻り値：なし
        //=========================================================================================
        private float GetScreenWidth(int widthSetting) {
            // 画面幅を初期化
            float width = 0.0f;
            if (widthSetting == 0) {
                width = m_viewerPanel.ClientRectangle.Width - TextViewerComponent.CX_DISPLAY_RIGHT_MARGIN;
            } else if (widthSetting == 1) {
                width = m_screenWidth - SystemInformation.VerticalScrollBarWidth - SystemInformation.SizingBorderWidth * 2 - TextViewerComponent.CX_DISPLAY_RIGHT_MARGIN;
            } else {
                width = FormUtils.GetAllScreenRectangle().Width
                            - SystemInformation.VerticalScrollBarWidth - SystemInformation.SizingBorderWidth * 2 - TextViewerComponent.CX_DISPLAY_RIGHT_MARGIN;
            }
            if (m_isDisplayLineNumber) {
                width -= m_viewerPanel.TextViewerLineNoAreaWidth + TextViewerComponent.CX_DISPLAY_LEFT_MARGIN;
            }
            return width;
        }

        //=========================================================================================
        // 機　能：バッファの内容を行単位に分解する
        // 引　数：[in]g  描画の際に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void SeparateLineInfo(TextViewerGraphics g) {
            byte[] buffer;
            int allLength;
            m_accessibleFile.GetBuffer(out buffer, out allLength);
            int index = m_nextParseStartPosition;
            int start = m_nextParseStartPosition;
            while (index < allLength) {
                if (buffer[index] == 0x0d) {                // CR
                    index++;
                    if (index + 1 < allLength && buffer[index + 1] == 0x00 && m_lineEncodingMode == EncodingType.UNICODE) {
                        index++;
                    }
                } else if (buffer[index] == 0x0a) {         // LF
                    LineBreakChar crlf = LineBreakChar.Cr;
                    int length = index - start;
                    if (m_lineEncodingMode == EncodingType.UNICODE) {
                        if (length > 1 && buffer[start + length - 1] == 0x00 && buffer[start + length - 2] == 0x0d) {
                            length -= 2;
                            crlf = LineBreakChar.CrLf;
                        }
                    } else {
                        if (length > 0 && buffer[start + length - 1] == 0x0d) {
                            length--;
                            crlf = LineBreakChar.CrLf;
                        }
                    }
                    CreateLogicalLine(g, buffer, start, length, crlf, m_nextPhysicalLineNo);
                    m_nextPhysicalLineNo++;
                    index++;
                    if (index < allLength && buffer[index] == 0x00 && m_lineEncodingMode == EncodingType.UNICODE) {
                        index++;
                    }
                    m_nextParseStartPosition = index;
                    start = index;
                } else {
                    index++;
                }
            }

            RetrieveDataLoadStatus status = m_accessibleFile.Status;
            if (status != RetrieveDataLoadStatus.Loading) {
                LineBreakChar eof = LineBreakChar.Eof;
                if (status == RetrieveDataLoadStatus.Failed) {
                    eof = LineBreakChar.None;
                } else if (status == RetrieveDataLoadStatus.CompletedPart) {
                    eof = LineBreakChar.EofContinue;
                }
                if (start < index) {
                    int length = index - start;
                    if (length > 0 && buffer[start + length - 1] == 0x0d) {
                        length--;
                    }
                    CreateLogicalLine(g, buffer, start, length, eof, m_nextPhysicalLineNo);
                    m_nextPhysicalLineNo++;
                } else if (status != RetrieveDataLoadStatus.Failed) {
                    CreateLogicalLine(g, buffer, start, 0, eof, m_nextPhysicalLineNo);
                    m_nextPhysicalLineNo++;
                }
                m_isParseEnd = true;
            }
        }

        //=========================================================================================
        // 機　能：物理行から行を登録する
        // 引　数：[in]g              描画の際に使用するグラフィックス
        // 　　　　[in]buffer         ファイル内のデータバッファ
        // 　　　　[in]start          データバッファで行として認識する開始位置
        // 　　　　[in]length         データバッファの行として認識するデータの長さ
        // 　　　　[in]crlf           作成する行の改行の扱い
        // 　　　　[in]physicalLineNo 物理行の行番号
        // 戻り値：なし
        //=========================================================================================
        private void CreateLogicalLine(TextViewerGraphics g, byte[] buffer, int start, int length, LineBreakChar crlf, int physicalLineNo) {
            m_physicalLineToLineListIndex.Add(m_lineList.Count);
            if (length == 0) {
                // 空の行
                TextBufferLogicalLineInfo lineInfo = new TextBufferLogicalLineInfo(start, "", crlf, m_nextPhysicalLineNo);
                AddLineInfo(lineInfo);
            } else if (m_lineBreakSetting.LineBreakMode == TextViewerLineBreakSetting.TextViewerLineBreakMode.NoBreak) {
                // 折り返しなし
                CreateLogicalLineNoBreak(g, buffer, start, length, crlf, physicalLineNo);
            } else if (m_lineBreakSetting.LineBreakMode == TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar) {
                // 文字数で折り返し
                CreateLogicalLineBreakByChar(g, buffer, start, length, crlf, physicalLineNo);
            } else {
                // 画面ピクセルで折り返し
                CreateLogicalLineBreakByPixel(g, buffer, start, length, crlf, physicalLineNo);
            }
        }

        //=========================================================================================
        // 機　能：物理行から折り返しなしの行を登録する
        // 引　数：[in]g              描画の際に使用するグラフィックス
        // 　　　　[in]buffer         ファイル内のデータバッファ
        // 　　　　[in]start          データバッファで行として認識する開始位置
        // 　　　　[in]length         データバッファの行として認識するデータの長さ
        // 　　　　[in]crlf           作成する行の改行の扱い
        // 　　　　[in]physicalLineNo 物理行の行番号
        // 戻り値：なし
        //=========================================================================================
        private void CreateLogicalLineNoBreak(TextViewerGraphics g, byte[] buffer, int start, int length, LineBreakChar crlf, int physicalLineNo) {
            string strLine = m_lineEncodingMode.Encoding.GetString(buffer, start, length);
            float width = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, strLine);
            TextBufferLogicalLineInfo lineInfo = new TextBufferLogicalLineInfo(start, strLine, crlf, m_nextPhysicalLineNo);
            AddLineInfo(lineInfo);
            m_maxTextPixelWidth = Math.Max(m_maxTextPixelWidth, width);
        }

        //=========================================================================================
        // 機　能：物理行から文字数で折り返して空を登録する
        // 引　数：[in]g              描画の際に使用するグラフィックス
        // 　　　　[in]buffer         ファイル内のデータバッファ
        // 　　　　[in]start          データバッファで行として認識する開始位置
        // 　　　　[in]length         データバッファの行として認識するデータの長さ
        // 　　　　[in]crlf           作成する行の改行の扱い
        // 　　　　[in]physicalLineNo 物理行の行番号
        // 戻り値：なし
        //=========================================================================================
        private void CreateLogicalLineBreakByChar(TextViewerGraphics g, byte[] buffer, int start, int length, LineBreakChar crlf, int physicalLineNo) {
            string strLineAll = m_lineEncodingMode.Encoding.GetString(buffer, start, length);
            string[] lineList = SplitByScreenWidth(g, strLineAll, m_screenCharWidth);
            for (int i = 0; i <lineList.Length; i++) {
                string strLine = lineList[i];
                LineBreakChar currentCrlf = crlf;
                if (i != lineList.Length - 1) {
                    currentCrlf = LineBreakChar.None;
                }
                TextBufferLogicalLineInfo lineInfo = new TextBufferLogicalLineInfo(start, strLine, currentCrlf, m_nextPhysicalLineNo);
                AddLineInfo(lineInfo);
            }
        }

        //=========================================================================================
        // 機　能：文字列を表示可能な文字だけ指定された文字数以内になるように分割する
        // 引　数：[in]g          描画の際に使用するグラフィックス
        // 　　　　[in]strAll     分割する文字列の全体
        // 　　　　[in]widthLimit 文字幅の制限値
        // 戻り値：分割した文字列の配列
        //=========================================================================================
        private string[] SplitByScreenWidth(TextViewerGraphics g, string strAll, int widthLimit) {
            if (strAll.Length == 0) {
                string[] result = new string[1];
                result[0] = "";
                return result;
            }
            CharWidth charWidth = new CharWidth();
            List<string> resultLineList = new List<string>();
            char[] chAll = strAll.ToCharArray();
            char[] lineBuffer = new char[widthLimit];
            int indexLineBuffer = 0;
            int indexAll = 0;
            int lineWidth = 0;
            int chLength = chAll.Length;
            while (indexAll < chLength) {
                // 全角／半角を判断
                char ch = chAll[indexAll];
                int chWidth = 0;            // この文字の幅（半角:1、全角:2、タブ:文字数）
                if (ch == 0x09) {
                    int targetWidth = Math.Min(widthLimit, (lineWidth + m_tabWidth) / m_tabWidth * m_tabWidth);
                    chWidth = targetWidth - lineWidth;
                    if (chWidth <= 0) {
                        chWidth = m_tabWidth;
                    }
                } else {
                    CharWidth.CharType type = charWidth.GetCharType(g.Graphics, chAll[indexAll]);
                    if (type == CharWidth.CharType.FullWidth) {
                        chWidth = 2;
                    } else if (type == CharWidth.CharType.HalfWidth) {
                        chWidth = 1;
                    } else {
                        chWidth = 1;
                        ch = '.';
                    }
                }

                // 文字列化
                if (lineWidth + chWidth > widthLimit) {
                    // 行幅を超えるとき
                    resultLineList.Add(new string(lineBuffer, 0, indexLineBuffer));
                    indexLineBuffer = 0;
                    lineWidth = 0;
                }
                lineWidth += chWidth;
                lineBuffer[indexLineBuffer] = ch;
                indexLineBuffer++;
                indexAll++;
            }
            if (lineWidth + 1 >= widthLimit) {
                // 改行を加えると行幅を超えるとき
                resultLineList.Add(new string(lineBuffer, 0, indexLineBuffer));
                resultLineList.Add("");
            } else {
                resultLineList.Add(new string(lineBuffer, 0, indexLineBuffer));
            }
            return resultLineList.ToArray();
        }

        //=========================================================================================
        // 機　能：物理行から文字数で折り返して空を登録する
        // 引　数：[in]g              描画の際に使用するグラフィックス
        // 　　　　[in]buffer         ファイル内のデータバッファ
        // 　　　　[in]start          データバッファで行として認識する開始位置
        // 　　　　[in]length         データバッファの行として認識するデータの長さ
        // 　　　　[in]crlf           作成する行の改行の扱い
        // 　　　　[in]physicalLineNo 物理行の行番号
        // 戻り値：なし
        //=========================================================================================
        private void CreateLogicalLineBreakByPixel(TextViewerGraphics g, byte[] buffer, int start, int length, LineBreakChar crlf, int physicalLineNo) {
            float cxCrChar = m_fontSize.Width;            // 改行文字の幅
            string strLineAll = m_lineEncodingMode.Encoding.GetString(buffer, start, length);
            while (strLineAll.Length > 0) {
                string strLineAllSp = strLineAll.Replace('\t', ' ');
                int fitted;
                int lines;
                SizeF sizeText = g.Graphics.MeasureString(strLineAllSp, g.TextFont, m_screenLinePixelSize, StringFormat.GenericDefault, out fitted, out lines);
                float width = sizeText.Width;
                string brokenLine = strLineAll.Substring(0, fitted);
                strLineAll = strLineAll.Substring(fitted);
                if (strLineAll.Length == 0 && width + cxCrChar < m_screenLinePixelSize.Width) {
                    // 残りなしで、改行を含めて幅に納まる場合
                    TextBufferLogicalLineInfo lineInfo = new TextBufferLogicalLineInfo(start, brokenLine, crlf, m_nextPhysicalLineNo);
                    AddLineInfo(lineInfo);
                    m_maxTextPixelWidth = Math.Max(m_maxTextPixelWidth, width);
                } else if (strLineAll.Length == 0) {
                    // 残りなしで、改行を新しい行に入れる場合
                    TextBufferLogicalLineInfo lineInfo1 = new TextBufferLogicalLineInfo(start, brokenLine, LineBreakChar.None, m_nextPhysicalLineNo);
                    AddLineInfo(lineInfo1);
                    m_maxTextPixelWidth = Math.Max(m_maxTextPixelWidth, width);
                    TextBufferLogicalLineInfo lineInfo2 = new TextBufferLogicalLineInfo(start, brokenLine, crlf, m_nextPhysicalLineNo);
                    AddLineInfo(lineInfo2);
                    m_maxTextPixelWidth = Math.Max(m_maxTextPixelWidth, cxCrChar);
                } else {
                    // 次の行に継続する場合
                    TextBufferLogicalLineInfo lineInfo = new TextBufferLogicalLineInfo(start, brokenLine, LineBreakChar.None, m_nextPhysicalLineNo);
                    AddLineInfo(lineInfo);
                    m_maxTextPixelWidth = Math.Max(m_maxTextPixelWidth, width);
                }
            }
        }

        //=========================================================================================
        // 機　能：新しい論理行を登録する
        // 引　数：[in]lineInfo   登録する論理行の情報
        // 戻り値：なし
        //=========================================================================================
        private void AddLineInfo(TextBufferLogicalLineInfo lineInfo) {
            if (m_lineList.Count < m_maxLineCount) {
                m_lineList.Add(lineInfo);
            } else {
                if (!m_lineCountOverDisplayed) {
                    m_lineCountOverDisplayed = true;
                    m_viewerPanel.ShowStatusbarMessage(Resources.FileViewer_LineCountAPart, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                }
            }
        }

        //=========================================================================================
        // 機　能：タブを展開する
        // 引　数：[in]g             フォントサイズの基準となるグラフィックス
        // 　　　　[in]strLine       論理行1行の文字列
        // 　　　　[out]strExtracted タブを展開した文字列
        // 　　　　[out]orgToTab     元の文字列からタブ展開済みの文字列への対応付けのインデックス（文字列長まで）
        // 　　　　[out]tabToOrg     タブ展開済み文字列から元の文字列への対応付けのインデックス（文字列長まで）
        // 戻り値：なし
        // メ　モ：TAB4でstrLine="a\tb"のとき、strExtracted="a   b"、orgToTab={0,1,4,5}、orgToTab={0,1,1,1,2,3}
        //=========================================================================================
        public void ExtractTab(TextViewerGraphics g, string strLine, out string strExtracted, out List<int> orgToTab, out List<int> tabToOrg) {
            orgToTab = new List<int>();
            tabToOrg = new List<int>();
            if (strLine.Length == 0) {
                tabToOrg.Add(0);
                orgToTab.Add(0);
                strExtracted = "";
                return;
            }
            CharWidth charWidth = new CharWidth();
            StringBuilder lineBuffer = new StringBuilder();
            int indexAll = 0;
            int lineWidth = 0;
            int strLineLength = strLine.Length;
            while (indexAll < strLineLength) {
                // 全角／半角を判断
                char ch = strLine[indexAll];
                int chWidth = 0;            // この文字の幅（半角:1、全角:2、タブ:文字数）
                int chRepeat = 1;           // 文字の繰り返し数（半角:1、全角:1、タブ:文字数）
                if (ch == 0x09) {
                    if (m_lineBreakSetting.LineBreakMode == TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar) {
                        ch = ' ';
                        int targetWidth = (lineWidth + m_tabWidth) / m_tabWidth * m_tabWidth;
                        chWidth = targetWidth - lineWidth;
                        chRepeat = chWidth;
                    } else {
                        ch = ' ';
                        chWidth = 1;
                        chRepeat = 1;
                    }
                } else {
                    CharWidth.CharType type = charWidth.GetCharType(g.Graphics, ch);
                    if (type == CharWidth.CharType.FullWidth) {
                        chWidth = 2;
                    } else if (type == CharWidth.CharType.HalfWidth) {
                        chWidth = 1;
                    } else {
                        chWidth = 1;
                        ch = '.';
                    }
                }

                // 文字列化
                orgToTab.Add(lineBuffer.Length);
                lineWidth += chWidth;
                for (int i = 0; i < chRepeat; i++) {        // TABの場合だけループ
                    lineBuffer.Append(ch);
                    tabToOrg.Add(indexAll);
                }
                indexAll++;
            }
            orgToTab.Add(lineBuffer.Length);
            tabToOrg.Add(indexAll);
            strExtracted = lineBuffer.ToString();
        }

        //=========================================================================================
        // 機　能：物理行が同一となる論理行の範囲を返す
        // 引　数：[in]line        基準となる論理行
        // 　　　　[out]startLine  開始の論理行を返す変数
        // 　　　　[out]endLine    終了の論理行を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetSameLineRange(int line, out int startLine, out int endLine) {
            TextBufferLogicalLineInfo targetLineInfo = m_lineList[line];
            int lineCount = m_lineList.Count;

            // 上方向
            int charCount = 0;
            startLine = line;
            for (int i = line - 1; i >= 0; i--) {
                TextBufferLogicalLineInfo lineInfo = m_lineList[i];
                if (lineInfo.PhysicalLineNo == targetLineInfo.PhysicalLineNo) {
                    charCount += lineInfo.StrLineOrg.Length;
                    if (charCount > TextSearchCondition.MAX_SEARCH_STRING_LENGTH) {
                        break;
                    }
                    startLine = i;
                } else {
                    break;
                }
            }

            // 下方向
            charCount = 0;
            endLine = line;
            for (int i = line + 1; i < lineCount; i++) {
                TextBufferLogicalLineInfo lineInfo = m_lineList[i];
                if (lineInfo.PhysicalLineNo == targetLineInfo.PhysicalLineNo) {
                    charCount += lineInfo.StrLineOrg.Length;
                    if (charCount > TextSearchCondition.MAX_SEARCH_STRING_LENGTH) {
                        break;
                    }
                    endLine = i;
                } else {
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：行情報を返す
        // 引　数：[in]dispLine   表示行
        // 戻り値：なし
        //=========================================================================================
        public TextBufferLogicalLineInfo GetLineInfo(int dispLine) {
            lock (this) {
                return m_lineList[dispLine];
            }
        }

        //=========================================================================================
        // 機　能：物理行から論理行を返す
        // 引　数：[in]physicalLine  物理行
        // 戻り値：論理行
        //=========================================================================================
        public int GetLineNumberFromPhysicalLineNumber(int physicalLine) {
            lock (this) {
                if (physicalLine >= m_physicalLineToLineListIndex.Count) {
                    return m_lineList.Count - 1;
                } else {
                    return m_physicalLineToLineListIndex[physicalLine];
                }
            }
        }

        //=========================================================================================
        // 機　能：アドレスから論理行を返す
        // 引　数：[in]address  アドレス
        // 戻り値：論理行
        //=========================================================================================
        public int GetLineNumberFromAddress(int address) {
            lock (this) {
                if (m_lineList.Count == 0) {
                    return 0;
                }
                for (int i = 0; i < m_lineList.Count; i++) {
                    if (m_lineList[i].BufferIndex > address) {
                        return i - 1;
                    }
                }
                return m_lineList.Count - 1;
            }
        }

        //=========================================================================================
        // 機　能：キーワードヒット行数の設定を行う
        // 引　数：[in]isSearch  キーワード検索のときtrue、自動検索のときfalse
        // 　　　　[in]value     設定値
        // 戻り値：なし
        //=========================================================================================
        public void SetSearchHitCount(bool isSearch, int value) {
            lock (this) {
                if (isSearch) {
                    m_searchHitCount = value;
                } else {
                    m_autoSearchHitCount = value;
                }
            }
        }

        //=========================================================================================
        // 機　能：キーワードヒット行数の増減を行う
        // 引　数：[in]isSearch  キーワード検索のときtrue、自動検索のときfalse
        // 　　　　[in]delta     変化量
        // 戻り値：なし
        //=========================================================================================
        public void AddSearchHitCount(bool isSearch, int delta) {
            lock (this) {
                if (isSearch) {
                    m_searchHitCount += delta;
                } else {
                    m_autoSearchHitCount += delta;
                }
            }
        }

        //=========================================================================================
        // プロパティ：対象ファイル
        //=========================================================================================
        public IFileViewerDataSource TargetFile {
            get {
                lock (this) {
                    return m_accessibleFile;
                }
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアで表示する最大行数（ビューアが開いたときの設定値）
        //=========================================================================================
        public int MaxLineCount {
            get {
                return m_maxLineCount;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアで行番号を表示するときtrue
        //=========================================================================================
        public bool IsDisplayLineNumber {
            get {
                return m_isDisplayLineNumber;
            }
        }

        //=========================================================================================
        // プロパティ：論理行の行数
        //=========================================================================================
        public int LogicalLineCount {
            get {
                lock (this) {
                    return m_lineList.Count;
                }
            }
        }

        //=========================================================================================
        // プロパティ：物理行の行数
        //=========================================================================================
        public int PhysicalLineCount {
            get {
                lock (this) {
                    return m_nextPhysicalLineNo - 1;
                }
            }
        }

        //=========================================================================================
        // プロパティ：テキストの最大ピクセル幅
        //=========================================================================================
        public float MaxTextPixelWidth {
            get {
                lock (this) {
                    return m_maxTextPixelWidth;
                }
            }
        }

        //=========================================================================================
        // プロパティ：次に解析するバッファ中の位置
        //=========================================================================================
        public int NextParseStartPosition {
            get {
                lock (this) {
                    return m_nextParseStartPosition;
                }
            }
        }

        //=========================================================================================
        // プロパティ：タブ幅（書き込み時はCompletedLoading==trueであること）
        //=========================================================================================
        public int TabWidth {
            get {
                lock (this) {
                    return m_tabWidth;
                }
            }
            set {
                lock (this) {
                    m_tabWidth = value;
                }
            }
        }

        //=========================================================================================
        // プロパティ：テキストモードの判定結果
        //=========================================================================================
        public bool IsBinary {
            get {
                lock (this) {
                    return m_isBinary;
                }
            }
        }

        //=========================================================================================
        // プロパティ：エンコーディング（書き込み時はCompletedLoading==trueであること）
        //=========================================================================================
        public EncodingType TextEncodingType {
            get {
                lock (this) {
                    return m_lineEncodingMode;
                }
            }
            set {
                lock (this) {
                    m_lineEncodingMode = value;
                }
            }
        }

        //=========================================================================================
        // プロパティ：折り返し設定
        //=========================================================================================
        public TextViewerLineBreakSetting LineBreakSetting {
            get {
                lock (this) {
                    return m_lineBreakSetting;
                }
            }
            set {
                lock (this) {
                    m_lineBreakSetting = value;
                }
            }
        }

        //=========================================================================================
        // プロパティ：読み込みが完了したときtrue
        //=========================================================================================
        public bool CompletedLoading {
            get {
                lock (this) {
                    return m_isParseEnd;
                }
            }
        }

        //=========================================================================================
        // プロパティ：検索キーワードにヒットした件数
        //=========================================================================================
        public int SearchHitCount {
            get {
                lock (this) {
                    return m_searchHitCount;
                }
            }
        }

        //=========================================================================================
        // プロパティ：自動検索キーワードにヒットした件数
        //=========================================================================================
        public int AutoSearchHitCount {
            get {
                lock (this) {
                    return m_autoSearchHitCount;
                }
            }
        }
    }
}
