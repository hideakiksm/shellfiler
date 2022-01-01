using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：タブとスペースを変換するフィルター
    //=========================================================================================
    public class FileFilterConvertTabSpace : IFileFilterComponent {
        // プロパティ
        private const string PROP_CONVERT_MODE = "ConvertMode";         // 変換モード
        private const string PROP_CONVERT_MODE_TO_TAB = "ToTab";        // Tabへ
        private const string PROP_CONVERT_MODE_TO_SPACE = "ToSpace";    // Spaceへ
        private const string PROP_TAB_WIDTH = "TabWidth";               // タブ幅
        private const string PROP_ENCODING = "Encoding";                // 文字コード
        private const string PROP_CODE_CHECK = "CodeCheck";             // エンコード不一致の場合
        private const string PROP_CODE_CHECK_SKIP = "Skip";             // このフィルターをスキップ
        private const string PROP_CODE_CHECK_ERROR = "Error";           // エラーにする
        private const string PROP_CODE_CHECK_IGNORE = "Ignore";         // 変換できない文字を無視

        private const int TAB_WIDTH_MIN = 2;                            // タブ幅の最小
        private const int TAB_WIDTH_MAX = 16;                           // タブ幅の最大
        
        // CR/LFのコード定義
        private const byte BYTE_00 = 0x00;                              // 00
        private const byte BYTE_CR = 0x0d;                              // CR
        private const byte BYTE_LF = 0x0a;                              // LF
        private const char CH_TAB = '\t';                               // TAB

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterConvertTabSpace() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string dispMode;
            string strMode = (string)param[PROP_CONVERT_MODE];
            if (strMode == PROP_CONVERT_MODE_TO_TAB) {
                dispMode = Resources.FileFilter_TabSpaceDispTab;
            } else if (strMode == PROP_CONVERT_MODE_TO_SPACE) {
                dispMode = Resources.FileFilter_TabSpaceDispSpace;
            } else {
                dispMode = Resources.FileFilter_TabSpaceDispTab;
            }
            
            int tabWidth = (int)param[PROP_TAB_WIDTH];
            string dispWidth = string.Format(Resources.FileFilter_TabSpaceDispWidth, tabWidth);

            string encoding = (string)param[PROP_ENCODING];
            EncodingType encodingType = EncodingType.FromString(encoding);
            string dispEncoding = encodingType.DisplayName;

            string dispCheck;
            string strCheck = (string)param[PROP_CODE_CHECK];
            if (strCheck == PROP_CODE_CHECK_ERROR) {
                dispCheck = Resources.FileFilter_CharsetError;
            } else if (strCheck == PROP_CODE_CHECK_SKIP) {
                dispCheck = Resources.FileFilter_CharsetSkip;
            } else {
                dispCheck = Resources.FileFilter_CharsetIgnore;
            }
            
            if (single) {
                return new string[] { dispMode };
            } else {
                return new string[] { dispMode, dispWidth, dispEncoding, dispCheck };
            }
        }

        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public FileFilterItem CreateFileFilterItem() {
            FileFilterItem item = new FileFilterItem();
            item.FileFilterClassPath = this.GetType().FullName;
            item.PropertyList.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_SPACE);
            item.PropertyList.Add(PROP_TAB_WIDTH, 4);
            item.PropertyList.Add(PROP_ENCODING, EncodingType.UTF8.DisplayName);
            item.PropertyList.Add(PROP_CODE_CHECK, PROP_CODE_CHECK_ERROR);
            return item;
        }

        //=========================================================================================
        // 機　能：フィルター設定用のプロパティ情報を作成する
        // 引　数：[in]mode  定義済みモード
        // 戻り値：フィルター設定用のプロパティ情報
        //=========================================================================================
        public static Dictionary<string, object> CreateProperty(FileFilterDefinedMode mode) {
            Dictionary<string, object> property = new Dictionary<string, object>();
            property.Add(PROP_CODE_CHECK, PROP_CODE_CHECK_ERROR);
            if (mode == FileFilterDefinedMode.ShiftJIS4TabSpace) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_SPACE);
                property.Add(PROP_TAB_WIDTH, 4);
                property.Add(PROP_ENCODING, EncodingType.SJIS.DisplayName);
            } else if (mode == FileFilterDefinedMode.ShiftJISSpace4Tab) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_TAB);
                property.Add(PROP_TAB_WIDTH, 4);
                property.Add(PROP_ENCODING, EncodingType.SJIS.DisplayName);
            } else if (mode == FileFilterDefinedMode.ShiftJIS8TabSpace) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_SPACE);
                property.Add(PROP_TAB_WIDTH, 8);
                property.Add(PROP_ENCODING, EncodingType.SJIS.DisplayName);
            } else if (mode == FileFilterDefinedMode.ShiftJISSpace8Tab) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_TAB);
                property.Add(PROP_TAB_WIDTH, 8);
                property.Add(PROP_ENCODING, EncodingType.SJIS.DisplayName);
            } else if (mode == FileFilterDefinedMode.UTF84TabSpace) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_SPACE);
                property.Add(PROP_TAB_WIDTH, 4);
                property.Add(PROP_ENCODING, EncodingType.UTF8.DisplayName);
            } else if (mode == FileFilterDefinedMode.UTF8Space4Tab) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_TAB);
                property.Add(PROP_TAB_WIDTH, 4);
                property.Add(PROP_ENCODING, EncodingType.UTF8.DisplayName);
            } else if (mode == FileFilterDefinedMode.UTF88TabSpace) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_SPACE);
                property.Add(PROP_TAB_WIDTH, 8);
                property.Add(PROP_ENCODING, EncodingType.UTF8.DisplayName);
            } else if (mode == FileFilterDefinedMode.UTF8Space8Tab) {
                property.Add(PROP_CONVERT_MODE, PROP_CONVERT_MODE_TO_TAB);
                property.Add(PROP_TAB_WIDTH, 8);
                property.Add(PROP_ENCODING, EncodingType.UTF8.DisplayName);
            } else {
                Program.Abort("サポートされていないパラメータが指定されました。");
            }
            return property;
        } 

        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            EncodingType[] encodingTypeList = EncodingType.AllTextValue;
            string[] encodingDispList = new string[encodingTypeList.Length];
            for (int i = 0; i < encodingTypeList.Length; i++) {
                encodingDispList[i] = encodingTypeList[i].DisplayName;
            }

            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_TabSpaceUI01Label, PROP_CONVERT_MODE,
                                                    new string[] { Resources.FileFilter_TabSpaceUI01ItemTab, Resources.FileFilter_TabSpaceUI01ItemSpace },
                                                    new string[] { PROP_CONVERT_MODE_TO_TAB, PROP_CONVERT_MODE_TO_SPACE }));
            itemList.Add(new SettingUIItem.Numeric(Resources.FileFilter_TabSpaceUI02Label, PROP_TAB_WIDTH, TAB_WIDTH_MIN, TAB_WIDTH_MAX));
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_TabSpaceUI03Label, PROP_ENCODING,
                                                    encodingDispList, encodingDispList));
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_TabSpaceUI04Label, PROP_CODE_CHECK,
                                                    new string[] { Resources.FileFilter_TabSpaceUI04ItemError, Resources.FileFilter_TabSpaceUI04ItemSkip, Resources.FileFilter_TabSpaceUI04ItemIgnore },
                                                    new string[] { PROP_CODE_CHECK_ERROR, PROP_CODE_CHECK_SKIP, PROP_CODE_CHECK_IGNORE }));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_TabSpaceUI05Label, 1));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_TabSpaceUI06Label, 1));

            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_CONVERT_MODE) || !(param[PROP_CONVERT_MODE] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string mode = (string)(param[PROP_CONVERT_MODE]);
            if (mode != PROP_CONVERT_MODE_TO_TAB && mode != PROP_CONVERT_MODE_TO_SPACE) {
                return Resources.FileFilter_MsgSerializeError;
            }

            if (!param.ContainsKey(PROP_TAB_WIDTH) || !(param[PROP_TAB_WIDTH] is int)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            int width = (int)(param[PROP_TAB_WIDTH]);
            if (width < TAB_WIDTH_MIN || TAB_WIDTH_MAX < width) {
                return Resources.FileFilter_MsgSerializeError;
            }

            if (!param.ContainsKey(PROP_ENCODING) || !(param[PROP_ENCODING] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string encoding = (string)(param[PROP_ENCODING]);
            EncodingType encodingType = EncodingType.FromString(encoding);
            if (encodingType == null) {
                return Resources.FileFilter_MsgSerializeError;
            }

            if (!param.ContainsKey(PROP_CODE_CHECK) || !(param[PROP_CODE_CHECK] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string check = (string)(param[PROP_CODE_CHECK]);
            if (check != PROP_CODE_CHECK_ERROR && check != PROP_CODE_CHECK_SKIP && check != PROP_CODE_CHECK_IGNORE) {
                return Resources.FileFilter_MsgSerializeError;
            }

            return null;
        }

        //=========================================================================================
        // 機　能：変換を実行する
        // 引　数：[in]orgFileName 元のファイルパス（クリップボードのときnull）
        // 　　　　[in]src         変換元のバイト列
        // 　　　　[out]dest       変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]param       変換に使用するパラメータ
        // 　　　　[in]cancelEvent キャンセル時にシグナル状態になるイベント
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Convert(string orgFileName, byte[] src, out byte[] dest, Dictionary<string, object> param, WaitHandle cancelEvent) {
            // パラメータを取得
            string strMode = (string)param[PROP_CONVERT_MODE];
            bool spaceToTab = (strMode == PROP_CONVERT_MODE_TO_TAB);
            int width = (int)param[PROP_TAB_WIDTH];
            CodeCheckMode checkMode = GetCodeCheckMode((string)(param[PROP_CODE_CHECK]));
            EncodingType encode = EncodingType.FromString((string)param[PROP_ENCODING]);

            // 処理を実行
            bool success;
            Graphics g = Program.MainWindow.CreateGraphics();
            try {
                Converter converter = new Converter(g, spaceToTab, width, checkMode);
                if (encode == EncodingType.UNICODE) {
                    success = converter.ConvertWithUnicode(src, out dest);
                } else {
                    success = converter.ConvertWithMultiByte(src, out dest, encode);
                }
            } finally {
                g.Dispose();
            }

            // エラーを確認
            if (!success) {
                if (checkMode == CodeCheckMode.Error) {
                    return FileOperationStatus.FilterUnknownChar;
                } else if (checkMode == CodeCheckMode.Skip) {
                    dest = src;
                    return FileOperationStatus.Success;
                } else {    // Ignoreはここにこない
                    Program.Abort("フィルターTabSpaceConvertで想定外のエラー状態です。");
                    return FileOperationStatus.Fail;
                }
            } else {
                return FileOperationStatus.Success;
            }
        }

        //=========================================================================================
        // 機　能：エラー発生時のモードの文字列をコードに変換する
        // 引　数：[in]strMode   エラー発生時のモードの文字列表現
        // 戻り値：エラー発生時のモード
        //=========================================================================================
        private CodeCheckMode GetCodeCheckMode(string strMode) {
            CodeCheckMode mode;
            if (strMode == PROP_CODE_CHECK_SKIP) {
                mode = CodeCheckMode.Skip;
            } else if (strMode == PROP_CODE_CHECK_IGNORE) {
                mode = CodeCheckMode.Ignore;
            } else {
                mode = CodeCheckMode.Error;
            }
            return mode;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_TabSpaceConvertName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_TabSpaceConvertExp;
            }
        }

        //=========================================================================================
        // クラス：変換処理を実行するクラス
        //=========================================================================================
        private class Converter {
            // フォントの大きさの判断に使用するグラフィックス
            private Graphics m_graphics;

            // 空白をタブに変換するときtrue
            private bool m_spaceToTab;

            // タブ幅
            private int m_tabWidth;

            // 文字コード不一致の場合の動作
            private CodeCheckMode m_checkMode;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]g           フォントの大きさの判断に使用するグラフィックス
            // 　　　　[in]spaceToTab  空白をタブに変換するときtrue
            // 　　　　[in]tabWidth    タブ幅
            // 　　　　[in]checkMode   文字コード不一致の場合の動作
            // 戻り値：なし
            //=========================================================================================
            public Converter(Graphics g, bool spaceToTab, int tabWidth, CodeCheckMode checkMode) {
                m_graphics = g;
                m_spaceToTab = spaceToTab;
                m_tabWidth = tabWidth;
                m_checkMode = checkMode;
            }

            //=========================================================================================
            // 機　能：UNICODEでの変換を行う
            // 引　数：[in]src     変換元のバイト列
            // 　　　　[out]dest   変換先のバイト列を返す変数（変換元と同一になる可能性あり）
            // 戻り値：変換に成功したときtrue
            //=========================================================================================
            public bool ConvertWithUnicode(byte[] src, out byte[] dest) {
                dest = null;
                MemoryStream destStream = new MemoryStream();
                int startIndex = 0;
                int length = src.Length;
                if (length % 2 == 1) {
                    return false;
                }
                for (int i = 0; i < length; i += 2) {
                    if (src[i] == BYTE_CR && src[i + 1] == BYTE_00 || src[i] == BYTE_LF && src[i + 1] == BYTE_00) {
                        if (startIndex != i) {
                            byte[] destPart = ConvertStringPart(src, startIndex, i, EncodingType.UNICODE);
                            if (destPart == null) {
                                return false;
                            }
                            destStream.Write(destPart, 0, destPart.Length);
                        }
                        destStream.WriteByte(src[i]);
                        destStream.WriteByte(src[i + 1]);
                        startIndex = i + 2;
                    }
                }
                if (startIndex != length) {
                    byte[] destPart = ConvertStringPart(src, startIndex, length, EncodingType.UNICODE);
                    if (destPart == null) {
                        return false;
                    }
                    destStream.Write(destPart, 0, destPart.Length);
                }
                dest = destStream.ToArray();
                return true;
            }

            //=========================================================================================
            // 機　能：マルチバイト系文字コードでの変換を行う
            // 引　数：[in]src     変換元のバイト列
            // 　　　　[out]dest   変換先のバイト列を返す変数（変換元と同一になる可能性あり）
            // 戻り値：変換に成功したときtrue
            //=========================================================================================
            public bool ConvertWithMultiByte(byte[] src, out byte[] dest, EncodingType encode) {
                dest = null;
                MemoryStream destStream = new MemoryStream();
                int startIndex = 0;
                int length = src.Length;
                for (int i = 0; i < length; i++) {
                    if (src[i] == BYTE_CR || src[i] == BYTE_LF) {
                        if (startIndex != i) {
                            byte[] destPart = ConvertStringPart(src, startIndex, i, encode);
                            if (destPart == null) {
                                return false;
                            }
                            destStream.Write(destPart, 0, destPart.Length);
                        }
                        destStream.WriteByte(src[i]);
                        startIndex = i + 1;
                    }
                }
                if (startIndex != length) {
                    byte[] destPart = ConvertStringPart(src, startIndex, length, encode);
                    if (destPart == null) {
                        return false;
                    }
                    destStream.Write(destPart, 0, destPart.Length);
                }
                dest = destStream.ToArray();
                return true;
            }

            //=========================================================================================
            // 機　能：切り出した文字列部分で１行分の変換を行う
            // 引　数：[in]src         変換元のバイト列
            // 　　　　[in]startIndex  開始位置
            // 　　　　[in]endIndex    終了位置の次のインデックス
            // 　　　　[in]encode      文字列のエンコード方式
            // 戻り値：変換結果（変換エラーのときnull）
            //=========================================================================================
            private byte[] ConvertStringPart(byte[] src, int startIndex, int endIndex, EncodingType encode) {
                bool codeCheck = (m_checkMode != CodeCheckMode.Ignore);

                // src→文字列化
                string str = encode.Encoding.GetString(src, startIndex, endIndex - startIndex);
                if (codeCheck) {
                    byte[] srcCheck = encode.Encoding.GetBytes(str);
                    bool success = ArrayUtils.CompareByteArray(src, startIndex, endIndex - startIndex, srcCheck, 0, srcCheck.Length);
                    if (!success) {
                        return null;
                    }
                }

                // tab/space変換
                string result;
                if (m_spaceToTab) {
                    result = ConvertSpaceToTab(str);
                } else {
                    result = ConvertTabToSpace(str);
                }

                // 文字列→dest
                byte[] resultBytes = encode.Encoding.GetBytes(result);
                return resultBytes;
            }

            //=========================================================================================
            // 機　能：文字列の空白をタブに変換する
            // 引　数：[in]str  変換元の文字列
            // 戻り値：変換後の文字列
            //=========================================================================================
            private string ConvertSpaceToTab(string str) {
                CharWidth charWidth = new CharWidth();
                StringBuilder lineBuffer = new StringBuilder();
                int lineWidth = 0;
                int indexSpaceStart = -1;       // 連続する空白の開始位置（空白を検出していないとき-1）
                int index = 0;
                while (index < str.Length) {
                    char ch = str[index];
                    if (ch == ' ') {
                        if (indexSpaceStart == -1) {
                            indexSpaceStart = lineWidth;
                        }
                        lineWidth++;
                    } else {
                        if (indexSpaceStart != -1) {
                            // 空白が蓄積されている場合
                            int spaceCount = lineWidth - indexSpaceStart;
                            AddSpaceTab(spaceCount, lineBuffer, ch, ref lineWidth);
                            indexSpaceStart = -1;
                        } else {
                            // 通常の文字のみ
                            lineBuffer.Append(ch);
                            if (ch == CH_TAB) {
                                lineWidth = (lineWidth + m_tabWidth) / m_tabWidth * m_tabWidth;
                            } else {
                                CharWidth.CharType type = charWidth.GetCharType(m_graphics, ch);
                                if (type == CharWidth.CharType.HalfWidth) {
                                    lineWidth++;
                                } else {
                                    lineWidth += 2;
                                }
                            }
                        }
                    }
                    index++;
                }
                if (indexSpaceStart != -1) {
                    int spaceCount = str.Length - indexSpaceStart;
                    AddSpaceTab(spaceCount, lineBuffer, -1, ref lineWidth);
                }
                return lineBuffer.ToString();
            }

            //=========================================================================================
            // 機　能：空白をタブに変換して指定されたバッファに追加する
            // 引　数：[in]spaceCount     蓄積されている空白の数
            // 　　　　[in]lineBuffer     結果を格納するバッファ
            // 　　　　[in]chCode         追加する文字（追加しない場合は-1）
            // 　　　　[in,out]lineWidth  現在の行の幅（新しい幅を返す）
            // 戻り値：なし
            //=========================================================================================
            private void AddSpaceTab(int spaceCount, StringBuilder lineBuffer, int chCode, ref int lineWidth) {
                char ch = (char)chCode;
                int spcCreate = Math.Min(lineWidth % m_tabWidth, spaceCount);
                int tabCreate = (spaceCount - spcCreate + m_tabWidth - 1) / m_tabWidth;
                if (ch == CH_TAB) {
                    // 連続する空白の後、タブを追加する
                    // tabWidth=6
                    //         1      2      3      4      5      6      7      8      9      10      11      12   
                    //     tab idx  tab idx  tab idx  tab idx  tab idx  tab idx  tab idx  tab idx  tab idx  tab idx  tab idx  tab idx
                    //  2   -   -    1   6    1   6    1   6    1   6    2  12    2  12    1  12    1  12    1  12    1  12    2  18
                    //  3   -   -    -   -    1   6    1   6    1   6    2  12    2  12    2  12    1  12    1  12    1  12    2  18
                    //  4   -   -    -   -    -   -    1   6    1   6    2  12    2  12    2  12    2  12    1  12    1  12    2  18
                    //  5   -   -    -   -    -   -    -   -    1   6    2  12    2  12    2  12    2  12    2  12    1  12    2  18
                    //  6   -   -    -   -    -   -    -   -    -   -    2  12    2  12    2  12    2  12    2  12    2  12    2  18
                    //  7   -   -    -   -    -   -    -   -    -   -    -   -    2  12    2  12    2  12    2  12    2  12    3  18
                    //  8   -   -    -   -    -   -    -   -    -   -    -   -    -   -    2  12    2  12    2  12    2  12    3  18
                    //  9   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    2  12    2  12    2  12    3  18
                    // 10   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    2  12    2  12    3  18
                    // 11   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    2  12    3  18
                    // 12   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    3  18
                    lineWidth = (lineWidth + m_tabWidth) / m_tabWidth * m_tabWidth;
                    tabCreate++;
                    lineBuffer.Append(CH_TAB, tabCreate);
                } else {
                    // 連続する空白の後、通常の文字を追加する
                    // tabWidth=6
                    //         1        2        3       4        5        6        7        8        9       10       11       12
                    //     tab spc  tab spc  tab spc  tab spc  tab spc  tab spc  tab spc  tab spc  tab spc  tab spc  tab spc  tab spc
                    //  2   -   -    0   2    0   2    0   2    0   2    1   0    1   1    0   2    0   2    0   2    0   2    1   0
                    //  3   -   -    -   -    0   3    0   3    0   3    1   0    1   1    1   2    0   3    0   3    0   3    1   0
                    //  4   -   -    -   -    -   -    0   4    0   4    1   0    1   1    1   2    0   3    0   4    0   4    1   0
                    //  5   -   -    -   -    -   -    -   -    0   5    1   0    1   1    1   2    1   3    1   4    0   5    1   0
                    //  6   -   -    -   -    -   -    -   -    -   -    1   0    1   1    1   2    1   3    1   4    1   5    1   0
                    //  7   -   -    -   -    -   -    -   -    -   -    -   -    1   1    1   2    1   3    1   4    1   5    2   0
                    //  8   -   -    -   -    -   -    -   -    -   -    -   -    -   -    1   2    1   3    1   4    1   5    2   0
                    //  9   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    1   3    1   4    1   5    2   0
                    // 10   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    1   4    1   5    2   0
                    // 11   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    1   5    2   0
                    // 12   -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    -   -    2   0
                    if (spaceCount >= 2) {
                        lineBuffer.Append(CH_TAB, tabCreate);
                        lineBuffer.Append(' ', spcCreate);
                    } else {
                        lineBuffer.Append(' ');
                    }
                    if (chCode != -1) {
                        lineBuffer.Append(ch);
                        CharWidth charWidth = new CharWidth();
                        CharWidth.CharType type = charWidth.GetCharType(m_graphics, ch);
                        if (type == CharWidth.CharType.HalfWidth) {
                            lineWidth++;
                        } else {
                            lineWidth += 2;
                        }
                    }
                }
            }

            //=========================================================================================
            // 機　能：文字列のタブを空白に変換する
            // 引　数：[in]str  変換元の文字列
            // 戻り値：変換後の文字列
            //=========================================================================================
            private string ConvertTabToSpace(string str) {
                CharWidth charWidth = new CharWidth();
                StringBuilder lineBuffer = new StringBuilder();
                int lineWidth = 0;
                for (int index = 0; index < str.Length; index++) {
                    char ch = str[index];
                    int chWidth = 0;            // この文字の幅（半角:1、全角:2、タブ:文字数）
                    int chRepeat = 1;           // 文字の繰り返し数（半角:1、全角:1、タブ:文字数）
                    if (ch == CH_TAB) {
                        ch = ' ';
                        int targetWidth = (lineWidth + m_tabWidth) / m_tabWidth * m_tabWidth;
                        chWidth = targetWidth - lineWidth;
                        chRepeat = chWidth;
                    } else {
                        CharWidth.CharType type = charWidth.GetCharType(m_graphics, ch);
                        if (type == CharWidth.CharType.HalfWidth) {
                            chWidth = 1;
                        } else {
                            chWidth = 2;
                        }
                    }

                    // 文字列化
                    lineWidth += chWidth;
                    for (int i = 0; i < chRepeat; i++) {        // TABの場合だけループ
                        lineBuffer.Append(ch);
                    }
                }
                return lineBuffer.ToString();
            }
        }

        //=========================================================================================
        // 列挙子：文字コード不正の振る舞い
        //=========================================================================================
        private enum CodeCheckMode {
            Error,                                      // 文字コード不正のとき、エラー停止する
            Skip,                                       // 文字コード不正のとき、このフィルターをスキップする
            Ignore,                                     // 文字コード不正のとき、エラーを無視する
        }
    }
}
