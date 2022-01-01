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
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：連続する改行コードを削除するフィルター
    //=========================================================================================
    public class FileFilterDeleteMultiCrLf : IFileFilterComponent {
        // プロパティ
        private const string PROP_MAX_EMPTY_LINES = "MaxEmptyLines";    // 空行の最大数
        private const string PROP_CODE_UNICODE = "Unicode";             // Unicodeを扱うときtrue

        private const int EMPTY_LINE_MIN = 0;                           // 空行の最小値
        private const int EMPTY_LINE_MAX = 16;                          // 空行の最大値

        // CR/LFのコード定義
        private byte BYTE_00 = 0x00;                                // 00
        private byte BYTE_CR = 0x0d;                                // CR
        private byte BYTE_LF = 0x0a;                                // LF

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterDeleteMultiCrLf() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            int maxEmptyLine= (int)param[PROP_MAX_EMPTY_LINES];
            string dispMaxEmptyLine = string.Format(Resources.FileFilter_MultiCrLfMaxEmpty, maxEmptyLine);

            string dispUnicode;
            bool unicode = (bool)param[PROP_CODE_UNICODE];
            if (unicode) {
                dispUnicode = Resources.FileFilter_MultiCrLfUnicode;
            } else {
                dispUnicode = Resources.FileFilter_MultiCrLfMultibyte;
            }
            if (single) {
                return new string[] { dispMaxEmptyLine };
            } else {
                return new string[] { dispMaxEmptyLine, dispUnicode };
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
            item.PropertyList.Add(PROP_MAX_EMPTY_LINES, 0);
            item.PropertyList.Add(PROP_CODE_UNICODE, false);
            return item;
        }

        //=========================================================================================
        // 機　能：フィルター設定用のプロパティ情報を作成する
        // 引　数：[in]mode  定義済みモード
        // 戻り値：フィルター設定用のプロパティ情報
        //=========================================================================================
        public static Dictionary<string, object> CreateProperty(FileFilterDefinedMode mode) {
            Dictionary<string, object> property = new Dictionary<string, object>();
            if (mode == FileFilterDefinedMode.DeleteEmptyLine) {
                property.Add(PROP_MAX_EMPTY_LINES, 0);
                property.Add(PROP_CODE_UNICODE, false);
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
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Numeric(Resources.FileFilter_MultiCrLfUI01Label, PROP_MAX_EMPTY_LINES, EMPTY_LINE_MIN, EMPTY_LINE_MAX));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Checkbox(Resources.FileFilter_MultiCrLfUI02Check, PROP_CODE_UNICODE));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_MultiCrLfUI03Label, 1));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_MultiCrLfUI04Label, 1));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_MAX_EMPTY_LINES) || !(param[PROP_MAX_EMPTY_LINES] is int)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            int maxEmptyLine = (int)(param[PROP_MAX_EMPTY_LINES]);
            if (maxEmptyLine < EMPTY_LINE_MIN || EMPTY_LINE_MAX < maxEmptyLine) {
                return Resources.FileFilter_MsgSerializeError;
            }

            if (!param.ContainsKey(PROP_CODE_UNICODE) || !(param[PROP_CODE_UNICODE] is bool)) {
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
            int maxEmptyLine = (int)(param[PROP_MAX_EMPTY_LINES]);

            // 処理を実行
            bool unicode = (bool)param[PROP_CODE_UNICODE];
            if (!unicode) {
                return ConvertMultiByteString(src, out dest, maxEmptyLine);
            } else {
                return ConvertUnicodeString(src, out dest, maxEmptyLine);
            }
        }

        //=========================================================================================
        // 機　能：バイト列をマルチバイトと見なして変換する
        // 引　数：[in]src           変換元のバイト列
        // 　　　　[out]dest         変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]maxEmptyLine  空行の最大行数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ConvertMultiByteString(byte[] src, out byte[] dest, int maxEmptyLine) {
            MemoryStream outStream = new MemoryStream();
            int retCodeCount = 0;
            int length = src.Length;
            for (int i = 0; i < length; i++) {
                int start = i;
                if (src[i] == BYTE_CR) {
                    if (i + 1 < length && src[i + 1] == BYTE_LF) {
                        i++;
                    }
                    retCodeCount++;
                } else if (src[i] == BYTE_LF) {
                    retCodeCount++;
                } else {
                    retCodeCount = 0;
                }
                if (retCodeCount > 0) {
                    // 今回のループで改行発見
                    if (retCodeCount - 1 <= maxEmptyLine) {
                        outStream.Write(src, start, i - start + 1);
                    }
                } else {
                    // 今回のループは通常の文字
                    outStream.WriteByte(src[i]);
                }
            }
            dest = outStream.ToArray(); 
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：バイト列をUNICODEと見なして変換する
        // 引　数：[in]src           変換元のバイト列
        // 　　　　[out]dest         変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]maxEmptyLine  空行の最大行数
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ConvertUnicodeString(byte[] src, out byte[] dest, int maxEmptyLine) {
            MemoryStream outStream = new MemoryStream();
            int retCodeCount = 0;
            int length = (src.Length & 0x7ffffffe);
            for (int i = 0; i < length; i += 2) {
                int start = i;
                if (src[i] == BYTE_CR && src[i + 1] == BYTE_00) {
                    if (i + 3 < length && src[i + 2] == BYTE_LF && src[i + 3] == BYTE_00) {
                        i += 2;
                    }
                    retCodeCount++;
                } else if (src[i] == BYTE_LF && src[i + 1] == BYTE_00) {
                    retCodeCount++;
                } else {
                    retCodeCount = 0;
                }
                if (retCodeCount > 0) {
                    // 今回のループで改行発見
                    if (retCodeCount - 1 <= maxEmptyLine) {
                        outStream.Write(src, start, i - start + 2);
                    }
                } else {
                    // 今回のループは通常の文字
                    outStream.WriteByte(src[i]);
                    outStream.WriteByte(src[i + 1]);
                }
            }
            if (src.Length % 2 == 1) {
                outStream.WriteByte(src[src.Length - 1]);
            }
            dest = outStream.ToArray(); 
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_MultiCrLfName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_MultiCrLfExp;
            }
        }
    }
}
