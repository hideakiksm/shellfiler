using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Windows.Forms;
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
    // クラス：最終の空白とタブを除去するフィルター
    //=========================================================================================
    public class FileFilterTrimLastSpace : IFileFilterComponent {
        // プロパティ
        private const string PROP_CODE_UNICODE = "Unicode";         // Unicodeを扱うときtrue

        // CR/LFのコード定義
        private byte BYTE_00 = 0x00;                                // 00
        private byte BYTE_TAB = 0x09;                               // TAB
        private byte BYTE_CR = 0x0d;                                // CR
        private byte BYTE_LF = 0x0a;                                // LF

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterTrimLastSpace() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string dispUnicode;
            bool unicode = (bool)param[PROP_CODE_UNICODE];
            if (unicode) {
                dispUnicode = Resources.FileFilter_TrimLastUnicode;
            } else {
                dispUnicode = Resources.FileFilter_TrimLastMultibyte;
            }
            if (single) {
                return new string[] { "" };
            } else {
                return new string[] { dispUnicode };
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
            item.PropertyList.Add(PROP_CODE_UNICODE, false);
            return item;
        }

        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Checkbox(Resources.FileFilter_TrimLastUI01Check, PROP_CODE_UNICODE));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_TrimLastUI02Label, 1));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_TrimLastUI03Label, 1));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
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
            // 処理を実行
            bool unicode = (bool)param[PROP_CODE_UNICODE];
            if (!unicode) {
                return ConvertMultiByteString(src, out dest);
            } else {
                return ConvertUnicodeString(src, out dest);
            }
        }

        //=========================================================================================
        // 機　能：バイト列をマルチバイトと見なして変換する
        // 引　数：[in]src        変換元のバイト列
        // 　　　　[out]dest      変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ConvertMultiByteString(byte[] src, out byte[] dest) {
            MemoryStream outStream = new MemoryStream();
            int length = src.Length;
            int lineStart = 0;                      // 有効な文字列の先頭
            int lineEnd = 0;                        // 有効な文字列の末尾の次の文字
            for (int i = 0; i < length; i++) {
                if (src[i] == BYTE_CR || src[i] == BYTE_LF) {
                    if (lineEnd - lineStart > 0) {
                        outStream.Write(src, lineStart, lineEnd - lineStart);
                    }
                    lineStart = i;
                    lineEnd = i + 1;
                } else if (src[i] == ' ' || src[i] == BYTE_TAB) {
                    ;
                } else {
                    lineEnd = i + 1;
                }
            }
            if (lineEnd - lineStart > 0) {
                outStream.Write(src, lineStart, lineEnd - lineStart);
            }
            dest = outStream.ToArray(); 
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：バイト列をUNICODEと見なして変換する
        // 引　数：[in]src        変換元のバイト列
        // 　　　　[out]dest      変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ConvertUnicodeString(byte[] src, out byte[] dest) {
            MemoryStream outStream = new MemoryStream();
            int length = (src.Length & 0x7ffffffe);
            int lineStart = 0;                      // 有効な文字列の先頭
            int lineEnd = 0;                        // 有効な文字列の末尾の次の文字
            for (int i = 0; i < length; i += 2) {
                if (src[i] == BYTE_CR && src[i + 1] == BYTE_00 || src[i] == BYTE_LF && src[i + 1] == BYTE_00) {
                    if (lineEnd - lineStart > 0) {
                        outStream.Write(src, lineStart, lineEnd - lineStart);
                    }
                    lineStart = i;
                    lineEnd = i + 2;
                } else if (src[i] == ' ' && src[i + 1] == BYTE_00 || src[i] == BYTE_TAB && src[i + 1] == BYTE_00) {
                    ;
                } else {
                    lineEnd = i + 2;
                }
            }
            if (lineEnd - lineStart > 0) {
                outStream.Write(src, lineStart, lineEnd - lineStart);
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
                return Resources.FileFilter_TrimLastSpaceName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_TrimLastSpaceExp;
            }
        }
    }
}
