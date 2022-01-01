using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプビューアの実装
    //=========================================================================================
    public class DumpViewerComponent : IViewerComponent, ViewerScrollImpl.IScrollValueSource {
        // 画面の左側に作成する行番号とコンテンツの間の描画しない領域のマージン
        public const int CX_DISPLAY_LEFT_MARGIN = 2;
        
        // 画面の右側に作成する描画しない領域のマージン
        public const int CX_DISPLAY_RIGHT_MARGIN = 4;

        // ダンプ領域とテキスト領域の間のマージン
        public const int CX_DUMP_TEXT_MARGIN = 25;

        // アドレスに表示する桁数
        public const int ADDRESS_DIGHT_WIDTH = 8;

        // この実装の所有ビュー
        private TextFileViewer m_parent;

        // スクロール機能の実装
        private ViewerScrollImpl m_viewerScrollImpl;

        // １行に表示するバイト数
        private int m_dumpLineByteCount;

        // 水平スクロールバーの位置
        private int m_scrollXPosition;

        // 垂直スクロールバーの位置（先頭行に表示している表示上の行数）
        private int m_scrollYPosition = 0;

        // 画面上に完全に表示できる行数
        private int m_scrollYCompleteLineSize = 0;

        // 検索時のカーソル行（-1:表示しない）
        private int m_scrollSearchCursorLine = -1;

        // 行の高さ[ピクセル]
        private int m_cyLineHeight;

        // セパレータの表示Ｘ位置
        private float m_xPosSeparator;

        // ダンプの表示Ｘ位置
        private float m_xPosDump;

        // テキストの表示X位置
        private float m_xPosText;

        // 半角１文字分の大きさの期待値
        private SizeF m_fontSize;

        // 選択範囲（選択していないときnull）
        private DumpViewerSelectionRange m_selectionRange = null;

        // 検索条件（検索していないときnull）
        private DumpSearchCondition m_searchCondition = null;

        //=========================================================================================
        // プロパティ：１行に表示するバイト数
        //=========================================================================================
        public int DumpLineByteCount {
            get {
                return m_dumpLineByteCount;
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent   所有ビュー
        // 　　　　[in]fontSize 半角１文字分の大きさの期待値
        // 戻り値：なし
        //=========================================================================================
        public DumpViewerComponent(TextFileViewer parent, SizeF fontSize) {
            m_parent = parent;
            m_fontSize = fontSize;
            m_dumpLineByteCount = m_parent.TextBufferLineInfo.LineBreakSetting.DumpLineByteCount;

            ResetDisplayPosition();
        }

        //=========================================================================================
        // 機　能：表示位置をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ResetDisplayPosition() {
            TextViewerGraphics g = new TextViewerGraphics(m_parent, 0);
            try {
                byte[] testBuffer = new byte[DumpLineByteCount];
                string testDump;
                List<int> dumpPosition;
                DumpHexFormatter formatter = new DumpHexFormatter();
                formatter.CreateDumpHexStr(testBuffer, 0, testBuffer.Length, DumpLineByteCount, out testDump, out dumpPosition);
                m_cyLineHeight = g.TextFont.Height;
                m_xPosSeparator = (int)(Math.Ceiling(ADDRESS_DIGHT_WIDTH * m_fontSize.Width + CX_DISPLAY_LEFT_MARGIN));
                m_xPosDump = m_xPosSeparator + CX_DISPLAY_LEFT_MARGIN;
                m_xPosText = m_xPosDump + GraphicsUtils.MeasureString(g.Graphics, g.TextFont, testDump) + CX_DUMP_TEXT_MARGIN;
            } finally {
                g.Dispose();
            }

            m_viewerScrollImpl = new ViewerScrollImpl(m_parent, this, m_cyLineHeight);
        }

        //=========================================================================================
        // 機　能：該当の行番号が描画対象となるかどうかを返す
        // 引　数：[in]minLine  最小行番号
        // 　　　　[in]maxLine  最大行番号
        // 　　　　[in]minByte  最小ファイル位置
        // 　　　　[in]maxByte  最大ファイル位置
        // 戻り値：描画対象のときtrue
        //=========================================================================================
        public bool IsDisplay(int minLine, int maxLine, int minByte, int maxByte) {
            int extendLine = SystemInformation.HorizontalScrollBarHeight / m_cyLineHeight + 1;
            if (maxByte / DumpLineByteCount < ScrollYPosition || minByte / DumpLineByteCount > ScrollYPosition + ScrollYCompleteLineSize + extendLine) {
                return false;
            } else {
                return true;
            }
        }

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：[in]rcClient     クライアント領域
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChange(Rectangle rcClient) {
            m_viewerScrollImpl.OnSizeChange();
        }

        //=========================================================================================
        // 機　能：水平スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetHorizontalScrollbar() {
            m_viewerScrollImpl.SetHorizontalScrollbar();
        }
        
        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetVerticalScrollbar() {
            m_viewerScrollImpl.SetVerticalScrollbar();
        }

        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]startLine 表示上の開始行（-1で全体を描画）
        // 　　　　[in]endLine   表示上の終了行
        // 戻り値：なし
        //=========================================================================================
        public void OnPaint(TextViewerGraphics g, bool fillBack, int dispStartLine, int dispEndLine) {
            int extendLine = SystemInformation.HorizontalScrollBarHeight / m_cyLineHeight + 1;
            if (dispStartLine == -1) {
                dispStartLine = 0;
                dispEndLine = ScrollYCompleteLineSize + extendLine;
            }

            // 行番号背景
            float yPos1 = dispStartLine * m_cyLineHeight;
            float yPos2 = (dispEndLine + 1) * m_cyLineHeight;
            RectangleF rcLine = new RectangleF(-ScrollXPosition, yPos1, m_xPosSeparator, yPos2 - yPos1);
            g.Graphics.FillRectangle(g.TextViewerLineNoBackBrush, rcLine);
            g.Graphics.DrawLine(g.TextViewerLineNoSeparatorPen, rcLine.Right, yPos1, m_xPosSeparator - ScrollXPosition, rcLine.Bottom);

            // セパレータを描画
            float xPosSeparator = m_xPosText - CX_DUMP_TEXT_MARGIN / 2;
            g.Graphics.DrawLine(g.TextViewerLineNoSeparatorPen, xPosSeparator - ScrollXPosition, yPos1, xPosSeparator - ScrollXPosition, yPos2);
            
            // 行の内容表示
            int fileSize;
            byte[] fileBuffer;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out fileBuffer, out fileSize);
            DumpSearchHitPosition searchHit = new DumpSearchHitPosition(dispStartLine + ScrollYPosition, dispEndLine + ScrollYPosition, DumpLineByteCount, fileSize);
            for (int line = dispStartLine; line <= dispEndLine; line++) {
                if (line + ScrollYPosition >= ScrollCyNeed) {
                    continue;
                }

                DrawAddress(g, line);
                if (m_selectionRange != null && m_selectionRange.Selected) {
                    // 選択中
                    DrawLineSelected(g, fillBack, line);
                } else {
                    SearchHitState hitSearch, hitAuto;
                    m_parent.SearchEngine.DumpSearchHitStateList.GetHistState(line + ScrollYPosition, out hitSearch, out hitAuto);
                    if (hitSearch == SearchHitState.Hit || hitAuto == SearchHitState.Hit) {
                        // 検索ヒット
                        DrawLineSearchHit(g, fillBack, line, searchHit);
                    } else {
                        // 選択・検索なし
                        DrawLineNormal(g, fillBack, line);
                    }
                }
            }

            // カーソル
            if (m_scrollSearchCursorLine != -1) {
                float xPos = m_xPosDump - ScrollXPosition - 1;
                int yPos = (m_scrollSearchCursorLine - ScrollYPosition + 1) * m_cyLineHeight - 1;
                g.Graphics.DrawLine(g.SearchCursorPen, xPos, yPos, m_parent.Width - 1, yPos);
            }
        }

        //=========================================================================================
        // 機　能：行番号を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]dispLine  表示上の行数
        // 戻り値：なし
        //=========================================================================================
        private void DrawAddress(TextViewerGraphics g, int dispLine) {
            int yPos = dispLine * m_cyLineHeight;
            int address = (dispLine + ScrollYPosition) * DumpLineByteCount;
            string strLine = address.ToString("X" + ADDRESS_DIGHT_WIDTH);
            g.Graphics.DrawString(strLine, g.TextFont, g.TextViewerLineNoTextBrush, -ScrollXPosition, yPos);
        }

        //=========================================================================================
        // 機　能：検索／選択なし状態で行の内容を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineNormal(TextViewerGraphics g, bool fillBack, int dispLine) {
            FillLineBack(g, fillBack, dispLine);

            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            // ダンプ部分の描画
            int yPos = dispLine * m_cyLineHeight;
            int address = (dispLine + ScrollYPosition) * DumpLineByteCount;
            int length = Math.Min(DumpLineByteCount, readSize - address);
            IFileViewerDataSource file = m_parent.TextBufferLineInfo.TargetFile;
            string strDump;
            List<int> dumpPosition;
            DumpHexFormatter hexFormatter = new DumpHexFormatter();
            hexFormatter.CreateDumpHexStr(readBuffer, address, length, DumpLineByteCount, out strDump, out dumpPosition);
            g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerTextBrush, m_xPosDump - ScrollXPosition, yPos);

            // テキスト部分の描画
            EncodingType encoding = m_parent.TextBufferLineInfo.TextEncodingType;
            string strText;
            List<int> charToByte, byteToChar;
            DumpTextFormatter textFormatter = new DumpTextFormatter();
            textFormatter.Convert(encoding, readBuffer, address, length, out strText, out charToByte, out byteToChar);
            g.Graphics.DrawString(strText, g.TextFont, g.TextViewerTextBrush, m_xPosText - ScrollXPosition, yPos);

            // EOFの描画
            DrawEof(g, address, yPos, strDump);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行の内容を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 　　　　[in]searchHit 検索ヒットの情報
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineSearchHit(TextViewerGraphics g, bool fillBack, int dispLine, DumpSearchHitPosition searchHit) {
            int yPos = dispLine * m_cyLineHeight;
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            // 初回処理で検索ヒット情報を画面分作成
            if (!searchHit.Fixed) {
                searchHit.Initialize();
                DumpSearchCore searchCore = new DumpSearchCore();
                searchCore.SetDumpByteHitPosition(m_searchCondition, readBuffer, searchHit);
            }

            // ダンプ部分の情報を取得
            int startAddress = (dispLine + ScrollYPosition) * DumpLineByteCount;
            int length = Math.Min(DumpLineByteCount, searchHit.FileSize - startAddress);
            string strDump;
            List<int> dumpPosition;
            DumpHexFormatter hexFormatter = new DumpHexFormatter();
            hexFormatter.CreateDumpHexStr(readBuffer, startAddress, length, DumpLineByteCount, out strDump, out dumpPosition);

            // テキスト部分の情報を取得
            EncodingType encoding = m_parent.TextBufferLineInfo.TextEncodingType;
            string strText;
            List<int> charToByte, byteToChar;
            DumpTextFormatter converter = new DumpTextFormatter();
            converter.Convert(encoding, readBuffer, startAddress, length, out strText, out charToByte, out byteToChar);
            byteToChar.Add(strText.Length);

            // 背景色を描画
            List<HitDiffPoint> searchDiffPoint = searchHit.GetHitPointListSearch(dispLine + ScrollYPosition);
            if (searchDiffPoint.Count > 0) {     // 必ずhit=trueで始まり、hit=falseで終わる
                DrawSearchHitBackDump(g, fillBack, dispLine, searchDiffPoint, searchHit, strDump, dumpPosition);
                DrawSearchHitBackText(g, fillBack, dispLine, searchDiffPoint, searchHit, strText, charToByte, byteToChar);
            } else {
                FillLineBack(g, fillBack, dispLine);
            }

            // 文字列を描画
            List<HitDiffPoint> autoDiffPoint = searchHit.GetHitPointListAuto(dispLine + ScrollYPosition);
            if (autoDiffPoint.Count > 0) {     // 必ずいずれかのhit=trueで始まり、両方のhit=falseで終わる
                DrawSearchHitStringDump(g, dispLine, autoDiffPoint, searchHit, strDump, dumpPosition);
                DrawSearchHitStringText(g, dispLine, autoDiffPoint, searchHit, strText, charToByte, byteToChar);
            } else {
                g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerTextBrush, m_xPosDump - ScrollXPosition, yPos);
                g.Graphics.DrawString(strText, g.TextFont, g.TextViewerTextBrush, m_xPosText - ScrollXPosition, yPos);
            }

            // EOFの描画
            DrawEof(g, startAddress, yPos, strDump);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行のダンプ領域の背景部分を描画する
        // 引　数：[in]g              グラフィックス
        // 　　　　[in]fillBack       背景を塗りつぶすときtrue
        // 　　　　[in]dispLine       表示上の行数
        // 　　　　[in]hitDiffPoint   検索ヒットしたときの変化点の情報（HitAutoSearchは未使用、位置は通しアドレス）
        // 　　　　[in]searchHit      検索ヒットの情報
        // 　　　　[in]strDump        ダンプ領域の文字列
        // 　　　　[in]dumpPosition   ダンプの各バイト位置がとるダンプ文字列の位置（奇数:開始、偶数:終了）
        // 戻り値：なし
        //=========================================================================================
        private void DrawSearchHitBackDump(TextViewerGraphics g, bool fillBack, int dispLine, List<HitDiffPoint> hitDiffPoint, DumpSearchHitPosition searchHit, string strDump, List<int> dumpPosition) {
            // 座標を計算
            float xPosDump = m_xPosDump - ScrollXPosition;
            float xPosSep = m_xPosText - CX_DUMP_TEXT_MARGIN / 2 - ScrollXPosition;
            int yPos = dispLine * m_cyLineHeight;
            int line = dispLine + ScrollYPosition;
            int startAddress = line * DumpLineByteCount;
            int length = Math.Min(DumpLineByteCount, searchHit.FileSize - startAddress);

            // ダンプ：文字単位の表示座標を計算
            List<int> measure = new List<int>();
            foreach (HitDiffPoint hit in hitDiffPoint) {
                if (hit.HitSearch) {
                    measure.Add(dumpPosition[(hit.Position - startAddress) * 2]);
                } else {
                    measure.Add(dumpPosition[(hit.Position - startAddress - 1) * 2 + 1]);
                }
            }

            float[] border = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, measure.ToArray());
            List<RectangleF> rcList = new List<RectangleF>();
            for (int i = 0; i < border.Length - 1; i += 2) {
                rcList.Add(new RectangleF(xPosDump + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
            }

            // ダンプ：描画
            if (fillBack) {
                g.Graphics.SetClip(m_parent.ClientRectangle);
                foreach (RectangleF rc in rcList) {
                    g.Graphics.SetClip(rc, CombineMode.Xor);
                }
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPosDump, yPos, xPosSep - 1 - xPosDump, m_cyLineHeight));
            }
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.FillRectangle(g.TextViewerSearchHitBackBrush, new RectangleF(xPosDump, yPos, xPosSep - 1 - xPosDump, m_cyLineHeight));
            g.Graphics.SetClip(m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行のテキスト領域の背景部分を描画する
        // 引　数：[in]g              グラフィックス
        // 　　　　[in]fillBack       背景を塗りつぶすときtrue
        // 　　　　[in]dispLine       表示上の行数
        // 　　　　[in]hitDiffPoint   検索ヒットしたときの変化点の情報（HitAutoSearchは未使用、位置は通しアドレス）
        // 　　　　[in]searchHit      検索ヒットの情報
        // 　　　　[in]strText        テキスト領域の文字列
        // 　　　　[in]textCharToByte テキストの文字をバイトに変換するための位置
        // 　　　　[in]textByteToChar テキストのバイトを文字に変換するための位置
        // 戻り値：なし
        //=========================================================================================
        private void DrawSearchHitBackText(TextViewerGraphics g, bool fillBack, int dispLine, List<HitDiffPoint> hitDiffPoint, DumpSearchHitPosition searchHit, string strText, List<int> textCharToByte, List<int> textByteToChar) {
            // 座標を計算
            float xPosText = m_xPosText - ScrollXPosition;
            int yPos = dispLine * m_cyLineHeight;
            int startAddress = (dispLine + ScrollYPosition) * DumpLineByteCount;
            int length = Math.Min(DumpLineByteCount, searchHit.FileSize - startAddress);

            // テキスト：文字単位の表示座標を計算
            List<int> measure = new List<int>();
            foreach (HitDiffPoint hit in hitDiffPoint) {
                measure.Add(textByteToChar[hit.Position - startAddress]);
            }

            float[] border = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, measure.ToArray());
            List<RectangleF> rcList = new List<RectangleF>();
            for (int i = 0; i < border.Length - 1; i += 2) {
                rcList.Add(new RectangleF(xPosText + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
            }

            // テキスト：描画
            if (fillBack) {
                g.Graphics.SetClip(m_parent.ClientRectangle);
                foreach (RectangleF rc in rcList) {
                    g.Graphics.SetClip(rc, CombineMode.Xor);
                }
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPosText, yPos, m_parent.Width - xPosText, m_cyLineHeight));
            }
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.FillRectangle(g.TextViewerSearchHitBackBrush, new RectangleF(xPosText, yPos, m_parent.Width - xPosText, m_cyLineHeight));
            g.Graphics.SetClip(m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行の文字部分を描画する
        // 引　数：[in]g              グラフィックス
        // 　　　　[in]dispLine       表示上の行数
        // 　　　　[in]hitDiffPoint   検索ヒットしたときの変化点の情報（HitAutoSearchは未使用、位置は通しアドレス）
        // 　　　　[in]searchHit      検索ヒットの情報
        // 　　　　[in]strDump        ダンプ領域の文字列
        // 　　　　[in]dumpPosition   ダンプの各バイト位置がとるダンプ文字列の位置（奇数:開始、偶数:終了）
        // 戻り値：なし
        //=========================================================================================
        private void DrawSearchHitStringDump(TextViewerGraphics g, int dispLine, List<HitDiffPoint> hitDiffPoint, DumpSearchHitPosition searchHit, string strDump, List<int> dumpPosition) {
            // 座標を計算
            float xPosDump = m_xPosDump - ScrollXPosition;
            int yPos = dispLine * m_cyLineHeight;
            int startAddress = (dispLine + ScrollYPosition) * DumpLineByteCount;

            // 文字単位の表示座標を計算
            List<int> measure = new List<int>();
            foreach (HitDiffPoint hit in hitDiffPoint) {
                if (hit.HitSearch || hit.HitAutoSearch) {
                    measure.Add(dumpPosition[(hit.Position - startAddress) * 2]);
                } else {
                    measure.Add(dumpPosition[(hit.Position - startAddress - 1) * 2 + 1]);
                }
            }

            float[] border = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, measure.ToArray());
            List<RectangleF> rcSearchList = new List<RectangleF>();
            List<RectangleF> rcAutoList = new List<RectangleF>();
            for (int i = 0; i < border.Length - 1; i++) {
                if (hitDiffPoint[i].HitAutoSearch) {
                    rcAutoList.Add(new RectangleF(xPosDump + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
                } else if (hitDiffPoint[i].HitSearch) {
                    rcSearchList.Add(new RectangleF(xPosDump + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
                }
            }
            
            // 描画
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcSearchList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerSearchHitTextBrush, xPosDump, yPos);

            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcAutoList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerSearchAutoTextBrush, xPosDump, yPos);

            foreach (RectangleF rc in rcSearchList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Xor);
            g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerTextBrush, xPosDump, yPos);

            g.Graphics.SetClip(m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行の文字部分を描画する
        // 引　数：[in]g              グラフィックス
        // 　　　　[in]dispLine       表示上の行数
        // 　　　　[in]hitDiffPoint   検索ヒットしたときの変化点の情報（HitAutoSearchは未使用、位置は通しアドレス）
        // 　　　　[in]searchHit      検索ヒットの情報
        // 　　　　[in]strText        テキスト領域の文字列
        // 　　　　[in]textCharToByte テキストの文字をバイトに変換するための位置
        // 　　　　[in]textByteToChar テキストのバイトを文字に変換するための位置
        // 戻り値：なし
        //=========================================================================================
        private void DrawSearchHitStringText(TextViewerGraphics g, int dispLine, List<HitDiffPoint> hitDiffPoint, DumpSearchHitPosition searchHit, string strText, List<int> textCharToByte, List<int> textByteToChar) {
            // 座標を計算
            float xPosText = m_xPosText - ScrollXPosition;
            int yPos = dispLine * m_cyLineHeight;
            int startAddress = (dispLine + ScrollYPosition) * DumpLineByteCount;

            // 文字単位の表示座標を計算
            List<int> measure = new List<int>();
            foreach (HitDiffPoint hit in hitDiffPoint) {
                measure.Add(textByteToChar[hit.Position - startAddress]);
            }

            float[] border = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, measure.ToArray());
            List<RectangleF> rcSearchList = new List<RectangleF>();
            List<RectangleF> rcAutoList = new List<RectangleF>();
            for (int i = 0; i < border.Length - 1; i++) {
                if (hitDiffPoint[i].HitAutoSearch) {
                    rcAutoList.Add(new RectangleF(xPosText + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
                } else if (hitDiffPoint[i].HitSearch) {
                    rcSearchList.Add(new RectangleF(xPosText + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
                }
            }
            
            // 描画
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcSearchList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.DrawString(strText, g.TextFont, g.TextViewerSearchHitTextBrush, xPosText, yPos);

            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcAutoList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.DrawString(strText, g.TextFont, g.TextViewerSearchAutoTextBrush, xPosText, yPos);

            foreach (RectangleF rc in rcSearchList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Xor);
            g.Graphics.DrawString(strText, g.TextFont, g.TextViewerTextBrush, xPosText, yPos);

            g.Graphics.SetClip(m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：選択状態で行の内容を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineSelected(TextViewerGraphics g, bool fillBack, int dispLine) {
            FillLineBack(g, fillBack, dispLine);

            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);


            // 領域を計算
            float xPosDump = m_xPosDump - ScrollXPosition;
            float xPosText = m_xPosText - ScrollXPosition;
            int yPos = dispLine * m_cyLineHeight;
            int line = dispLine + ScrollYPosition;
            int address = line * DumpLineByteCount;
            int length = Math.Min(DumpLineByteCount, readSize - address);

            int selectStartLine = m_selectionRange.StartAddress / DumpLineByteCount;
            int selectEndLine = (m_selectionRange.EndAddress - 1) / DumpLineByteCount;
            int selectStartColumn = m_selectionRange.StartAddress % DumpLineByteCount;
            int selectEndColumn = (m_selectionRange.EndAddress - 1) % DumpLineByteCount;

            // バッファ作成
            string strDump;
            List<int> dumpPosition;
            DumpHexFormatter formatter = new DumpHexFormatter();
            formatter.CreateDumpHexStr(readBuffer, address, length, DumpLineByteCount, out strDump, out dumpPosition);

            EncodingType encoding = m_parent.TextBufferLineInfo.TextEncodingType;
            DumpTextFormatter converter = new DumpTextFormatter();
            string strText;
            List<int> charToByte, byteToChar;
            converter.Convert(encoding, readBuffer, address, length, out strText, out charToByte, out byteToChar);

            RectangleF rcSelectDump = new RectangleF(0, 0, 0, 0);
            RectangleF rcSelectText = new RectangleF(0, 0, 0, 0);
            if (selectStartLine == line && selectEndLine == line ) {
                // 同じ行で開始・終了
                float[] dumpRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, dumpPosition[selectStartColumn * 2], dumpPosition[selectEndColumn * 2 + 1]);
                float margin = (dumpRegion[0] == 0) ? 0 : 1;
                float dumpX2 = xPosDump + dumpRegion[0] - margin;
                float dumpX3 = xPosDump + dumpRegion[1];
                rcSelectDump = new RectangleF(dumpX2, yPos, dumpX3 - dumpX2, m_cyLineHeight);
                float[] textRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, byteToChar[selectStartColumn], byteToChar[selectEndColumn] + 1);
                float textX2 = xPosText + textRegion[0];
                float textX3 = xPosText + textRegion[1];
                rcSelectText = new RectangleF(textX2, yPos, textX3 - textX2, m_cyLineHeight);
            } else if (line < selectStartLine || selectEndLine < line) {
                // 選択なし
                ;
            } else if (selectStartLine == line) {
                // 行の途中から選択
                float[] dumpRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, dumpPosition[selectStartColumn * 2], strDump.Length);
                float margin = (dumpRegion[0] == 0) ? 0 : 1;
                float dumpX2 = xPosDump + dumpRegion[0] - margin;
                float dumpX3 = xPosDump + dumpRegion[1];
                rcSelectDump = new RectangleF(dumpX2, yPos, dumpX3 - dumpX2, m_cyLineHeight);
                float[] textRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, byteToChar[selectStartColumn], strText.Length);
                float textX2 = xPosText + textRegion[0];
                float textX3 = xPosText + textRegion[1];
                rcSelectText = new RectangleF(textX2, yPos, textX3 - textX2, m_cyLineHeight);
            } else if (selectEndLine == line) {
                // 行の途中まで選択
                float[] dumpRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, dumpPosition[selectEndColumn * 2 + 1]);
                float dumpX2 = xPosDump + dumpRegion[0];
                rcSelectDump = new RectangleF(xPosDump, yPos, dumpX2 - xPosDump, m_cyLineHeight);
                float[] textRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, byteToChar[selectEndColumn] + 1);
                float textX2 = xPosText + textRegion[0];
                rcSelectText = new RectangleF(xPosText, yPos, textX2 - xPosText, m_cyLineHeight);
            } else {
                // すべて選択
                float[] dumpRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, strDump.Length);
                float dumpX2 = xPosDump + dumpRegion[0];
                rcSelectDump = new RectangleF(xPosDump, yPos, dumpX2 - xPosDump, m_cyLineHeight);
                float[] textRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, strText.Length);
                float textX2 = xPosText + textRegion[0];
                rcSelectText = new RectangleF(xPosText, yPos, textX2 - xPosText, m_cyLineHeight);
            }

            // 通常状態で描画
            RectangleF rcAll = new RectangleF(m_xPosDump - ScrollXPosition, yPos, m_parent.Width - m_xPosDump, m_cyLineHeight);
            g.Graphics.SetClip(rcAll, CombineMode.Replace);
            g.Graphics.SetClip(rcSelectDump, CombineMode.Xor);
            g.Graphics.SetClip(rcSelectText, CombineMode.Xor);
            FillLineBack(g, fillBack, dispLine);
            g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerTextBrush, new PointF(m_xPosDump - ScrollXPosition, yPos));
            g.Graphics.DrawString(strText, g.TextFont, g.TextViewerTextBrush, new PointF(m_xPosText - ScrollXPosition, yPos));

            // 選択状態で描画
            if (rcSelectDump.Width > 0) {
                g.Graphics.SetClip(rcAll, CombineMode.Xor);
                if (m_selectionRange.SelectDump) {
                    g.Graphics.FillRectangle(g.TextViewerSelectBackBrush, rcSelectDump);
                    g.Graphics.FillRectangle(g.TextViewerSelectBackBrush2, rcSelectText);
                    g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerSelectTextBrush, new PointF(m_xPosDump - ScrollXPosition, yPos));
                    g.Graphics.DrawString(strText, g.TextFont, g.TextViewerSelectTextBrush2, new PointF(m_xPosText - ScrollXPosition, yPos));
                } else {
                    g.Graphics.FillRectangle(g.TextViewerSelectBackBrush2, rcSelectDump);
                    g.Graphics.FillRectangle(g.TextViewerSelectBackBrush, rcSelectText);
                    g.Graphics.DrawString(strDump, g.TextFont, g.TextViewerSelectTextBrush2, new PointF(m_xPosDump - ScrollXPosition, yPos));
                    g.Graphics.DrawString(strText, g.TextFont, g.TextViewerSelectTextBrush, new PointF(m_xPosText - ScrollXPosition, yPos));
                }
            }
            g.Graphics.SetClip(m_parent.ClientRectangle);

            // EOFの描画
            DrawEof(g, address, yPos, strDump);
        }

        //=========================================================================================
        // 機　能：EOFを描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]address   その行の１桁目のアドレス
        // 　　　　[in]yPos      表示Y座標
        // 　　　　[in]strDump   作成したダンプ文字列
        // 戻り値：なし
        //=========================================================================================
        private void DrawEof(TextViewerGraphics g, int address, float yPos, string strDump) {
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            if (address + DumpLineByteCount > readSize) {
                LineBreakChar breakChar = LineBreakChar.None;
                if (m_parent.TextBufferLineInfo.TargetFile.Status == RetrieveDataLoadStatus.CompletedAll) {
                    breakChar = LineBreakChar.Eof;
                } else if (m_parent.TextBufferLineInfo.TargetFile.Status == RetrieveDataLoadStatus.CompletedPart) {
                    breakChar = LineBreakChar.EofContinue;
                }
                if (breakChar != LineBreakChar.None) {
                    float cxDump = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, strDump);
                    float xPos = m_xPosDump + cxDump - ScrollXPosition;
                    m_parent.DrawEofMark(g, xPos, yPos, breakChar);
                }
            }
        }

        //=========================================================================================
        // 機　能：1行分の背景を塗りつぶす
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 戻り値：なし
        //=========================================================================================
        private void FillLineBack(TextViewerGraphics g, bool fillBack, int dispLine) {
            if (fillBack) {
                float xPos1 = m_xPosDump - ScrollXPosition;
                float xPos2 = m_xPosText - ScrollXPosition;
                float xPosSep = m_xPosText - CX_DUMP_TEXT_MARGIN / 2 - ScrollXPosition;
                float yPos = dispLine * m_cyLineHeight;
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPos1, yPos, xPosSep - 1 - xPos1, m_cyLineHeight));
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPos2, yPos, m_parent.Width - xPos2, m_cyLineHeight));
            }
        }
        
        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnHScroll(ScrollEventArgs evt) {
            m_viewerScrollImpl.OnHScroll(evt);
        }

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnVScroll(ScrollEventArgs evt) {
            m_viewerScrollImpl.OnVScroll(evt);
        }
        
        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseWheel(MouseEventArgs evt) {
            m_viewerScrollImpl.OnMouseWheel(evt);
        }

        //=========================================================================================
        // 機　能：表示位置を上下に移動する
        // 引　数：[in]lines    移動する行数（下方向が＋）
        // 戻り値：なし
        //=========================================================================================
        public void MoveDisplayPosition(int lines) {
            m_viewerScrollImpl.MoveDisplayPosition(lines);
        }
        
        //=========================================================================================
        // 機　能：マウスのボタンがダブルクリックされたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseDoubleClick(MouseEventArgs evt) {
            int address;
            bool isDump;
            OnCharType onChar;
            GetAddressPosition(evt.X, evt.Y, null, out address, out isDump, out onChar);
            if (onChar != OnCharType.Outside) {
                // 文字列が見つかった
                int startAddress;
                int endAddress;
                PickupKeyword(address, int.MaxValue, out startAddress, out endAddress);
                m_selectionRange = new DumpViewerSelectionRange();
                m_selectionRange.StartAddress = startAddress;
                m_selectionRange.EndAddress = endAddress;
                m_selectionRange.SelectDump = isDump;
                m_parent.Invalidate();
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, m_selectionRange);
            } else {
                // 周辺を調査
                int mouseX = (int)(evt.X - m_fontSize.Width);
                OnCharType onChar1;
                OnCharType onChar2;
                OnCharType onChar3;
                GetAddressPosition(mouseX, evt.Y - m_cyLineHeight, null, out address, out isDump, out onChar1);
                GetAddressPosition(mouseX, evt.Y,                  null, out address, out isDump, out onChar2);
                GetAddressPosition(mouseX, evt.Y + m_cyLineHeight, null, out address, out isDump, out onChar3);
                if (onChar1 != OnCharType.Outside || onChar2 != OnCharType.Outside || onChar3 != OnCharType.Outside) {
                    return;
                }

                // 終了コマンドを取得して実行
                FileViewerActionCommand command = Program.Document.CommandFactory.CreateFileViewerCommandFromMouseInput(evt, m_parent);
                if (command != null) {
                    command.Execute();
                }
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseDown(MouseEventArgs evt) {
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            int address;
            bool isDump;
            OnCharType onChar;
            GetAddressPosition(evt.X, evt.Y, null, out address, out isDump, out onChar);
            if (m_selectionRange != null && m_selectionRange.StartAddress <= address && address <= m_selectionRange.EndAddress && m_selectionRange.Selected) {
                // 選択状態の上を再度クリック：自動検索
                byte[] keyword = new byte[Math.Min(m_selectionRange.EndAddress - m_selectionRange.StartAddress, DumpSearchCondition.MAX_SEARCH_BYTES_LENGTH)];
                Array.Copy(readBuffer, m_selectionRange.StartAddress, keyword, 0, keyword.Length);
                if (m_searchCondition == null) {
                    m_searchCondition = new DumpSearchCondition();
                }
                bool trimmed;
                m_searchCondition.AutoSearchBytes = DumpSearchCondition.TrimBySearchLength(keyword, out trimmed);
                m_parent.SearchEngine.SearchDumpAutoNew(m_searchCondition, DumpLineByteCount);
            } else if (onChar != OnCharType.Outside) {
                // ダンプ上で文字列が見つかった
                byte[] keyword = new byte[1];
                keyword[0] = readBuffer[address];
                if (m_searchCondition == null) {
                    m_searchCondition = new DumpSearchCondition();
                }
                bool trimmed;
                m_searchCondition.AutoSearchBytes = DumpSearchCondition.TrimBySearchLength(keyword, out trimmed);
                m_parent.SearchEngine.SearchDumpAutoNew(m_searchCondition, DumpLineByteCount);
            } else {
                // 表示中の自動検索を削除
                if (m_searchCondition != null) {
                    m_searchCondition.AutoSearchBytes = null;
                    m_parent.SearchEngine.SearchDumpAutoNew(m_searchCondition, DumpLineByteCount);
                }
            }

            // 新しい選択状態
            m_selectionRange = new DumpViewerSelectionRange();
            m_selectionRange.StartAddress = address;
            m_selectionRange.EndAddress = address;
            m_selectionRange.FirstClickAddress = address;
            m_selectionRange.PrevAddress = address;
            m_selectionRange.SelectDump = isDump;
            m_parent.Invalidate();
        }
        
        //=========================================================================================
        // 機　能：画面からキーワードを取得する
        // 引　数：[in]address       取得するアドレス
        // 　　　　[in]maxLength     前後への最大バイト数
        // 　　　　[out]startAddress 開始アドレス
        // 　　　　[out]endAddress   終了アドレス（終了位置の次のバイト）
        // 戻り値：なし
        //=========================================================================================
        private void PickupKeyword(int address, int maxLength, out int startAddress, out int endAddress) {
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            ViewerCharType chTypeOrg = StringCategory.GetViewerByteType(readBuffer[address]);

            // 後方に検索
            startAddress = address;
            for (int i = address; i >= 0; i--) {
                ViewerCharType chType = StringCategory.GetViewerByteType(readBuffer[i]);
                if (chTypeOrg == chType) {
                    startAddress = i;
                } else {
                    break;
                }
            }

            // 前方に検索
            endAddress = address;
            int fileSize = readSize;
            for (int i = address; i < fileSize; i++) {
                ViewerCharType chType = StringCategory.GetViewerByteType(readBuffer[i]);
                if (chTypeOrg == chType) {
                    endAddress = i + 1;
                } else {
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：マウス座標から、選択されたアドレス位置を返す
        // 引　数：[in]mouseX    マウスカーソルのX位置
        // 　　　　[in]mouseY    マウスカーソルのY位置
        // 　　　　[in]oldSelect 既存の選択状態（まだ選択されていないときはnull）
        // 　　　　[out]address  選択されたアドレスを返す変数
        // 　　　　[out]isDump   ダンプ側が選択されたときtrueを返す変数
        // 　　　　[out]onChar   文字上で選択された文字の種類を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetAddressPosition(int mouseX, int mouseY, DumpViewerSelectionRange oldSelect, out int address, out bool isDump, out OnCharType onChar) {
            if (oldSelect != null) {
                isDump = oldSelect.SelectDump;
            } else if (mouseX < m_xPosText - CX_DUMP_TEXT_MARGIN / 2) {
                isDump = true;
            } else {
                isDump = false;
            }
            byte[] readBuffer;
            int fileSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out fileSize);

            int line = mouseY / m_cyLineHeight + ScrollYPosition;
            if (line < 0) {
                // 前方
                address = 0;
                onChar = OnCharType.Outside;
            } else if (line > (fileSize / DumpLineByteCount)) {
                // 後方
                address = fileSize;
                onChar = OnCharType.Outside;
            } else if (isDump) {
                // ダンプ領域
                GetDumpAddressPosition(mouseX, mouseY, out address, out onChar);
            } else {
                // テキスト領域
                GetTextAddressPosition(mouseX, mouseY, out address, out onChar);
            }
        }

        //=========================================================================================
        // 機　能：マウス座標から、ダンプ領域で選択されたアドレス位置を返す
        // 引　数：[in]mouseX    マウスカーソルのX位置
        // 　　　　[in]mouseY    マウスカーソルのY位置
        // 　　　　[out]address  選択されたアドレスを返す変数
        // 　　　　[out]onChar   文字上で選択された文字の種類を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetDumpAddressPosition(int mouseX, int mouseY, out int address, out OnCharType onChar) {
            // 開始位置より左は対象外
            int line = mouseY / m_cyLineHeight + ScrollYPosition;
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            if (mouseX < m_xPosDump - ScrollXPosition) {
                address = Math.Min(line * DumpLineByteCount, readSize);
                onChar = OnCharType.Outside;
                return;
            }

            // 文字上を判定
            TextViewerGraphics g = new TextViewerGraphics(m_parent, LineNoAreaWidth);
            try {
                string strDump;
                List<int> dumpPosition;
                int start = line * DumpLineByteCount;
                int length = Math.Min(DumpLineByteCount, readSize - start);
                DumpHexFormatter formatter = new DumpHexFormatter();
                formatter.CreateDumpHexStr(readBuffer, start, length, DumpLineByteCount, out strDump, out dumpPosition);
                float[] region = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strDump, dumpPosition.ToArray());
                for (int i = 0; i < length; i++) {
                    float startPos = mouseX - (m_xPosDump - ScrollXPosition);
                    if (region[i * 2] <= startPos && startPos <= region[i * 2 + 1]) {
                        address = start + i;
                        onChar = OnCharType.OnChar;
                        return;
                    } else if (startPos < region[i * 2]) {
                        address = start + i;
                        onChar = OnCharType.OnSeparator;
                        return;
                    }
                }
                address = start + length;
                onChar = OnCharType.Outside;
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：マウス座標から、テキスト領域で選択されたアドレス位置を返す
        // 引　数：[in]mouseX    マウスカーソルのX位置
        // 　　　　[in]mouseY    マウスカーソルのY位置
        // 　　　　[out]address  選択されたアドレスを返す変数
        // 　　　　[out]onChar   文字上で選択された文字の種類を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetTextAddressPosition(int mouseX, int mouseY, out int address, out OnCharType onChar) {
            // 開始位置より左は対象外
            int line = mouseY / m_cyLineHeight + ScrollYPosition;
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            if (mouseX < m_xPosText - ScrollXPosition) {
                address = Math.Min(line * DumpLineByteCount, readSize);
                onChar = OnCharType.Outside;
                return;
            }

            // 文字上を判定
            TextViewerGraphics g = new TextViewerGraphics(m_parent, LineNoAreaWidth);
            try {
                DumpTextFormatter converter = new DumpTextFormatter();
                EncodingType encoding = m_parent.TextBufferLineInfo.TextEncodingType;
                string strText;
                List<int> charToByte, byteToChar;
                int start = line * DumpLineByteCount;
                int length = Math.Min(DumpLineByteCount, readSize - start);
                converter.Convert(encoding, readBuffer, start, length, out strText, out charToByte, out byteToChar);
                int[] indexArray = new int[strText.Length];
                for (int i = 0; i < strText.Length; i++) {
                    indexArray[i] = i + 1;
                }
                float[] region = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, strText, indexArray);
                for (int i = 0; i < strText.Length; i++) {
                    float startPos = mouseX - (m_xPosText - ScrollXPosition);
                    if (region[i] >= startPos) {
                        address = start + charToByte[i];
                        onChar = OnCharType.OnChar;
                        return;
                    }
                }
                address = start + length;
                onChar = OnCharType.Outside;
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]mouseX  マウスカーソルのX位置
        // 　　　　[in]mouseY  マウスカーソルのY位置
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseMove(int mouseX, int mouseY) {
            if (m_selectionRange == null) {
                return;
            }
            int address;
            bool isDump;
            OnCharType onChar;
            GetAddressPosition(mouseX, mouseY, m_selectionRange, out address, out isDump, out onChar);

            // 場所の更新
            if (address < m_selectionRange.FirstClickAddress) {
                m_selectionRange.StartAddress = address;
                m_selectionRange.EndAddress = m_selectionRange.FirstClickAddress;
            } else {
                m_selectionRange.StartAddress = m_selectionRange.FirstClickAddress;
                m_selectionRange.EndAddress = address;
            }

            // 再描画
            int line = address / DumpLineByteCount;
            if (m_selectionRange.CheckFirstSelected()) {
                m_parent.Invalidate();
                m_parent.Update();
            } else {
                int origin = Math.Min(m_selectionRange.PrevAddress / DumpLineByteCount, line) - ScrollYPosition;
                int height = Math.Abs(m_selectionRange.PrevAddress / DumpLineByteCount - line);
                DoubleBuffer db = new DoubleBuffer(m_parent, m_parent.ClientRectangle.Width, (height + 1) * m_cyLineHeight);
                try {
                    TextViewerGraphics g = new TextViewerGraphics(db.DrawingGraphics, LineNoAreaWidth);
                    try {
                        g.Graphics.FillRectangle(SystemBrushes.Window, db.DrawingRectangle);
                        g.Graphics.TranslateTransform(0, -origin * m_cyLineHeight);
                        OnPaint(g, true, origin, origin + height);
                    } finally {
                        g.Dispose();
                    }
                } finally {
                    db.FlushScreen(0, origin * m_cyLineHeight);
                }
            }

            // スクロール
            m_viewerScrollImpl.MouseScroll(line);

            // ステータスバーを更新
            if (address != m_selectionRange.PrevAddress) {
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, m_selectionRange);
            }

            // 現在位置を記憶
            m_selectionRange.PrevAddress = address;
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseUp(MouseEventArgs evt) {
            if (m_selectionRange == null || m_parent.IsDisposed) {
                return;
            }
            int address;
            bool isDump;
            OnCharType onChar;
            GetAddressPosition(evt.X, evt.Y, m_selectionRange, out address, out isDump, out onChar);
            if (address == m_selectionRange.FirstClickAddress) {
                m_selectionRange = null;
                m_parent.Invalidate();
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, null);
            }
        }

        //=========================================================================================
        // 機　能：右クリックによりコンテキストメニューの表示指示が行われたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnContextMenu(MouseEventArgs evt) {
            // 項目を追加
            List<MenuItemSetting> menu = new List<MenuItemSetting>();
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpTopLineCommand)),       'T', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpBottomLineCommand)),    'B', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1),    'J', null));
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextCommand)),          'C', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextAsCommand)),        'F', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectAllCommand)),         'A', null));
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)),            'S', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchForwardNextCommand)), 'N', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchReverseNextCommand)), 'P', null));
            AddAutoSearchMenu(evt.X, evt.Y, menu);

            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, m_parent);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            m_parent.ContextMenuStrip = cms;
            m_parent.ContextMenuStrip.Show(m_parent, evt.Location);
            m_parent.ContextMenuStrip = null;
        }
        
        //=========================================================================================
        // 機　能：コンテキストメニューに自動検索の項目を追加する
        // 引　数：[in]mouseX  マウスのX位置
        // 　　　　[in]mouseY  マウスのY位置
        // 　　　　[in]menu    メニューを追加する変数
        // 戻り値：なし
        //=========================================================================================
        private void AddAutoSearchMenu(int mouseX, int mouseY, List<MenuItemSetting> menu) {
            int targetStartAddress = -1;
            int targetEndAddress = -1;
            if (m_selectionRange != null && m_selectionRange.Selected) {
                // 範囲選択されているとき
                targetStartAddress = m_selectionRange.StartAddress;
                targetEndAddress = m_selectionRange.EndAddress;
            } else {
                // 自動ピックアップ
                int address;
                bool isDump;
                OnCharType onChar;
                GetAddressPosition(mouseX, mouseY, m_selectionRange, out address, out isDump, out onChar);
                if (onChar != OnCharType.Outside) {
                    // 文字列が見つかった
                    PickupKeyword(address, DumpSearchCondition.MAX_SEARCH_BYTES_LENGTH, out targetStartAddress, out targetEndAddress);
                }
            }

            // 検索文字列を取得
            if (targetStartAddress == -1) {
                return;
            }

            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            byte[] keyword = new byte[Math.Min(targetEndAddress - targetStartAddress, DumpSearchCondition.MAX_SEARCH_BYTES_LENGTH)];
            Array.Copy(readBuffer, targetStartAddress, keyword, 0, keyword.Length);
            string strKeyword = DumpUtils.BytesToString(keyword);
            
            const int MAX_SEARCH_SAMPLE_LENGTH = 5;
            byte[] keywordSample = new byte[Math.Min(targetEndAddress - targetStartAddress, MAX_SEARCH_SAMPLE_LENGTH)];
            Array.Copy(readBuffer, targetStartAddress, keywordSample, 0, keywordSample.Length);
            string strKeywordSample = DumpUtils.BytesToString(keywordSample);
            if (keyword.Length != keywordSample.Length) {
                strKeywordSample += "...";
            }
            int targetStartLine = targetStartAddress / DumpLineByteCount;

            // メニュー項目を作成
            string itemNameForward = string.Format(Resources.MenuName_V_Context_SearchForward, strKeywordSample);
            string itemNameReverse = string.Format(Resources.MenuName_V_Context_SearchReverse, strKeywordSample);
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchDirectCommand), true, strKeyword, targetStartLine),  '*', itemNameForward));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchDirectCommand), false, strKeyword, targetStartLine), '*', itemNameReverse));
        }


        //=========================================================================================
        // 機　能：選択をキャンセルする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CancelSelect() {
            m_selectionRange = null;
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：指定されたアドレスを先頭に表示する
        // 引　数：[in]address  アドレス
        // 戻り値：なし
        //=========================================================================================
        public void MoveSpecifiedAddressOnTop(int address) {
            int targetLine = address / DumpLineByteCount;
            MoveDisplayPosition(targetLine - ScrollYPosition);
        }
        
        //=========================================================================================
        // 機　能：指定されたアドレスを画面上に表示する
        // 引　数：[in]address          論理行
        // 　　　　[in]invalidateAlways 常に再描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void MoveSpecifiedAddressOnScreen(int address, bool invalidateAlways) {
            int newPosition = ScrollYPosition;
            int line = address / DumpLineByteCount;
            if (line < ScrollYPosition ||  ScrollYPosition + ScrollYCompleteLineSize < line) {
                newPosition = line - ScrollYCompleteLineSize / 2;
            }
            if (newPosition != ScrollYPosition) {
                newPosition = Math.Max(0, Math.Min(newPosition, ScrollCyNeed - ScrollYCompleteLineSize));
                ScrollYPosition = newPosition;
                UpdateLinePosition();
                m_parent.Invalidate();
            } else {
                if (invalidateAlways) {
                    m_parent.Invalidate();
                }
            }
        }

        //=========================================================================================
        // 機　能：行位置を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void UpdateLinePosition() {
            SetVerticalScrollbar();
            m_parent.FileViewerStatusBar.RefreshLineNo();
        }

        //=========================================================================================
        // 機　能：すべて選択する
        // 引　数：なし
        // 戻り値：選択に成功したときtrue
        //=========================================================================================
        public bool SelectAll() {
            int lineCount = m_parent.TextBufferLineInfo.LogicalLineCount;
            if (lineCount == 0) {
                return false;
            }

            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            m_selectionRange = new DumpViewerSelectionRange();
            m_selectionRange.StartAddress = 0;
            m_selectionRange.EndAddress = readSize;
            m_selectionRange.FirstClickAddress = 0;
            m_selectionRange.PrevAddress = 0;
            m_selectionRange.SelectDump = true;
            m_parent.Invalidate();
            m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, m_selectionRange);
            return true;
        }

        //=========================================================================================
        // 機　能：選択位置をクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public bool CopyText() {
            if (m_selectionRange == null || !m_selectionRange.Selected) {
                return false;
            }

            // テキストを用意
            DumpClipboardSetting setting;
            if (m_selectionRange.SelectDump) {
                setting = DumpClipboardSetting.GetDumpDefaultSetting();
                setting.DumpLineWidth = m_parent.TextBufferLineInfo.LineBreakSetting.DumpLineByteCount;
            } else {
                setting = DumpClipboardSetting.GetTextDefaultSetting(m_parent.TextBufferLineInfo.TextEncodingType.Encoding);
            }
            CopyTextClipboard(setting);
            return true;
        }

        //=========================================================================================
        // 機　能：選択位置を形式を指定してクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public bool CopyTextAs() {
            if (m_selectionRange == null || !m_selectionRange.Selected) {
                return false;
            }
            DumpClipboardSetting setting;
            if (Configuration.Current.DumpClipboardSettingDefault == null) {
                setting = (DumpClipboardSetting)(Program.Document.UserGeneralSetting.DumpClipboardSetting.Clone());
            } else {
                setting = (DumpClipboardSetting)(Configuration.Current.DumpClipboardSettingDefault.Clone());
            }
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);

            EncodingType encoding = m_parent.TextBufferLineInfo.TextEncodingType;
            int width = m_parent.TextBufferLineInfo.LineBreakSetting.DumpLineByteCount;
            ViewerDumpCopyAsDialog dialog = new ViewerDumpCopyAsDialog(setting, readBuffer, m_selectionRange.StartAddress, m_selectionRange.EndAddress, width, encoding);
            DialogResult result = dialog.ShowDialog(m_parent);
            if (result != DialogResult.OK) {
                return false;
            }
            setting = dialog.Setting;
            Program.Document.UserGeneralSetting.DumpClipboardSetting = (DumpClipboardSetting)(setting.Clone());
            CopyTextClipboard(setting);

            return true;
        }

        //=========================================================================================
        // 機　能：選択位置を指定形式でクリップボードにコピーする
        // 引　数：[in]setting  フォーマット形式
        // 戻り値：なし
        //=========================================================================================
        private void CopyTextClipboard(DumpClipboardSetting setting) {
            DumpClipboardFormatter formatter = new DumpClipboardFormatter();
            int width = m_parent.TextBufferLineInfo.LineBreakSetting.DumpLineByteCount;
            EncodingType encoding = m_parent.TextBufferLineInfo.TextEncodingType;
            byte[] readBuffer;
            int readSize;
            m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            string text = formatter.Format(readBuffer, m_selectionRange.StartAddress, m_selectionRange.EndAddress, setting, width, encoding);

            // クリップボードにコピー
            Clipboard.SetDataObject(text, true);

            // 選択範囲を点滅
            DumpViewerSelectionRange tempSel = m_selectionRange;
            m_selectionRange = null;
            m_parent.Invalidate();
            m_parent.Update();
            Thread.Sleep(50);
            m_selectionRange = tempSel;
            m_parent.Invalidate();
            m_parent.Update();
        }

        //=========================================================================================
        // 機　能：テキストを検索する
        // 引　数：[in]condition  検索条件（前回の条件を維持するときnull）
        // 　　　　[in]forward    前方検索するときtrue、後方検索するときfalse
        // 　　　　[in]startLine  検索開始行（-1のとき画面端または前回の続きから）
        // 戻り値：なし
        //=========================================================================================
        public void SearchBytes(DumpSearchCondition condition, bool forward, int startLine) {
            if (condition != null) {
                m_searchCondition = condition;
            }
            if (m_searchCondition == null) {
                return;
            }
            if (m_searchCondition == null || m_searchCondition.SearchBytes == null || m_searchCondition.SearchBytes.Length == 0) {
                // 検索クリア
                m_parent.SearchEngine.ClearSearchResult();
                m_parent.NotifySearchEnd(false);
                m_parent.Invalidate();
            } else {
                // カーソル位置を決定
                if (startLine != -1) {
                    // 指定がある場合
                    ScrollSearchCursorLine = startLine;
                    if (forward) {
                        ScrollSearchCursorLine++;
                    } else {
                        ScrollSearchCursorLine--;
                    }
                    ScrollSearchCursorLine = Math.Min(Math.Max(0, ScrollSearchCursorLine), ScrollCyNeed);
                } else {
                    // 前回位置が未決定のときは画面の端から検索
                    if (ScrollSearchCursorLine == -1 || condition != null) {
                        if (forward) {
                            ScrollSearchCursorLine = ScrollYPosition;
                        } else {
                            ScrollSearchCursorLine = Math.Min(ScrollYPosition + ScrollYCompleteLineSize, ScrollCyNeed - 1);
                        }
                    }
                }

                // 検索を実行
                int hitLine;
                WaitCursor waitCursor = new WaitCursor();
                try {
                    if (condition != null) {
                        // 新規検索
                        hitLine = m_parent.SearchEngine.SearchDumpNew(condition, forward, ScrollSearchCursorLine, DumpLineByteCount);
                    } else {
                        // 前回検索の続き
                        hitLine = m_parent.SearchEngine.SearchDumpNext(forward, ScrollSearchCursorLine, DumpLineByteCount);
                    }
                } finally {
                    waitCursor.Dispose();
                }

                // 検索結果を反映
                if (hitLine == -1) {
                    if (condition != null) {
                        ScrollSearchCursorLine = -1;
                    }
                    m_parent.ShowStatusbarMessage(Resources.FileViewer_SearchNotFound + DumpUtils.BytesToString(m_searchCondition.SearchBytes), FileOperationStatus.LogLevel.Info, IconImageListID.FileViewer_SearchGeneric);
                } else {
                    ScrollSearchCursorLine = hitLine;
                    MoveSpecifiedAddressOnScreen(hitLine * DumpLineByteCount, true);
                }
            }
        }

        //=========================================================================================
        // 機　能：タブ幅を変更する
        // 引　数：[in]tab  新しいタブ幅
        // 戻り値：タブ幅の変更に成功したときtrue
        //=========================================================================================
        public bool SetTab(int tab) {
            if (!m_parent.TextBufferLineInfo.CompletedLoading) {
                m_parent.ShowStatusbarMessage(Resources.FileViewer_CannotRunCopyTextAs, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                return false;
            }
            m_parent.TextBufferLineInfo.TabWidth = tab;
            ResetLineInfo(false);
            return true;
        }

        //=========================================================================================
        // 機　能：エンコードを変更する
        // 引　数：[in]encoding  新しいエンコード
        // 戻り値：エンコードの変更に成功したときtrue
        //=========================================================================================
        public bool SetEncoding(EncodingType encoding) {
            if (!m_parent.TextBufferLineInfo.CompletedLoading) {
                m_parent.ShowStatusbarMessage(Resources.FileViewer_CannotRunCopyTextAs, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                return false;
            }
            m_parent.TextBufferLineInfo.TextEncodingType = encoding;
            ResetLineInfo(false);
            return true;
        }

        //=========================================================================================
        // 機　能：行情報をリセットする
        // 引　数：[in]resetWidth  画面幅も初期化するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void ResetLineInfo(bool resetWidth) {
            WaitCursor waitCursor = new WaitCursor();
            try {
                int address = ScrollYPosition * DumpLineByteCount;
                m_selectionRange = null;
                m_parent.TextBufferLineInfo.ResetParse(resetWidth);
                m_parent.FileViewerStatusBar.RefreshTextInfo();
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, null);

                MoveSpecifiedAddressOnTop(address);
                m_viewerScrollImpl.OnSizeChange();
                ResetDisplayPosition();
                m_parent.Invalidate();
            } finally {
                waitCursor.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：改行幅をセットする
        // 引　数：[in]setting  改行幅の設定
        // 戻り値：改行幅の変更に成功したときtrue
        //=========================================================================================
        public bool SetLineWidth(TextViewerLineBreakSetting setting) {
            if (!m_parent.TextBufferLineInfo.CompletedLoading) {
                m_parent.ShowStatusbarMessage(Resources.FileViewer_CannotRunCopyTextAs, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                return false;
            }
            m_dumpLineByteCount = setting.DumpLineByteCount;
            m_parent.TextBufferLineInfo.LineBreakSetting.DumpLineByteCount = setting.DumpLineByteCount;
            ResetLineInfo(true);
            return true;
        }

        //=========================================================================================
        // プロパティ：水平スクロールに必要な幅
        //=========================================================================================
        public int ScrollCxNeed {
            get {
                return (int)(m_xPosDump + m_fontSize.Width * (DumpLineByteCount * 3 + 2 + 3 + DumpLineByteCount));
            }
        }

        //=========================================================================================
        // プロパティ：水平スクロールバーの位置
        //=========================================================================================
        public int ScrollXPosition {
            get {
                return m_scrollXPosition;
            }
            set {
                m_scrollXPosition = value;
            }
        }

        //=========================================================================================
        // プロパティ：垂直スクロールに必要な高さ
        //=========================================================================================
        public int ScrollCyNeed {
            get {
                byte[] readBuffer;
                int readSize;
                m_parent.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
                return readSize / DumpLineByteCount + 1;
            }
        }

        //=========================================================================================
        // プロパティ：水平スクロールバーの位置
        //=========================================================================================
        public int ScrollYPosition {
            get {
                return m_scrollYPosition;
            }
            set {
                m_scrollYPosition = value;
            }
        }

        //=========================================================================================
        // プロパティ：画面上に完全に表示できる行数
        //=========================================================================================
        public int ScrollYCompleteLineSize {
            get {
                return m_scrollYCompleteLineSize;
            }
            set {
                m_scrollYCompleteLineSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：先頭のアドレス
        //=========================================================================================
        public int Address {
            get {
                return ScrollYPosition * DumpLineByteCount;
            }
        }

        //=========================================================================================
        // プロパティ：検索時のカーソル行（-1:表示しない）
        //=========================================================================================
        public int ScrollSearchCursorLine {
            get {
                return m_scrollSearchCursorLine;
            }
            set {
                m_scrollSearchCursorLine = value;
            }
        }

        //=========================================================================================
        // プロパティ：行番号表示領域の幅
        //=========================================================================================
        public float LineNoAreaWidth {
            get {
                return m_xPosSeparator;
            }
        }

        //=========================================================================================
        // プロパティ：画面上に表示可能な行数行番号表示領域の幅
        //=========================================================================================
        public int CompleteLineSize {
            get {
                return ScrollYCompleteLineSize;
            }
        }

        //=========================================================================================
        // プロパティ：先頭行に表示している表示上の行数
        //=========================================================================================
        public int VisibleTopLine {
            get {
                return ScrollYPosition * DumpLineByteCount;
            }
        }

        //=========================================================================================
        // プロパティ：検索条件（検索していないときnull）
        //=========================================================================================
        public DumpSearchCondition SearchCondition {
            get {
                return m_searchCondition;
            }
        }

        //=========================================================================================
        // 列挙子：マウス座標に対してヒットした画面上の位置
        //=========================================================================================
        private enum OnCharType {
            // 文字の上にヒット
            OnChar,
            // 文字間にヒット
            OnSeparator,
            // 文字の外にヒット
            Outside,
        }
    }
}
