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
    // クラス：BASE64デコードを実行するフィルター
    //=========================================================================================
    public class FileFilterBase64Decode : IFileFilterComponent {
        // プロパティ
        private const string PROP_IGNORE_INVALID = "IgnoreInvalid";     // 不正文字を無視するときtrue

        // CR/LFのコード定義
        private const char BYTE_TAB = (char)0x09;                               // TAB
        private const char BYTE_CR = (char)0x0d;                                // CR
        private const char BYTE_LF = (char)0x0a;                                // LF

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterBase64Decode() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string dispInvalid;
            bool ignore = (bool)param[PROP_IGNORE_INVALID];
            if (ignore) {
                dispInvalid = Resources.FileFilter_B64DInvalidIgnore;
            } else {
                dispInvalid = Resources.FileFilter_B64DInvalidError;
            }
            return new string[] { dispInvalid };
        }
        
        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public FileFilterItem CreateFileFilterItem() {
            FileFilterItem item = new FileFilterItem();
            item.FileFilterClassPath = this.GetType().FullName;
            item.PropertyList.Add(PROP_IGNORE_INVALID, false);
            return item;
        }

        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Checkbox(Resources.FileFilter_B64DUI01Invalid, PROP_IGNORE_INVALID));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_B64DUI02Label, 1));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_IGNORE_INVALID) || !(param[PROP_IGNORE_INVALID] is bool)) {
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
            dest = null;

            // パラメータを取得
            bool ignoreMode = (bool)param[PROP_IGNORE_INVALID];

            MemoryStream outStream = new MemoryStream();
            int[] tempDigit = new int[4];
            int tempIndex = 0;
            int length = src.Length;
            bool last = false;
            for (int i = 0; i < length; i++) {
                // 0:A
                // 25:Z
                // 26:a
                // 51:z
                // 52:0
                // 61:9
                // 62:+
                // 63:/
                char by = (char)(src[i] & 0xff);
                int digit;
                if (by == BYTE_CR || by == BYTE_LF || by == BYTE_TAB || by == ' ') {
                    continue;
                } else if ('A' <= by && by <= 'Z') {
                    digit = by - 'A';
                } else if ('a' <= by && by <= 'z') {
                    digit = by - 'a' + 26;
                } else if ('0' <= by && by <= '9') {
                    digit = by - '0' + 52;
                } else if (by == '+') {
                    digit = 62;
                } else if (by == '/') {
                    digit = 63;
                } else if (by == '=') {
                    last = true;
                    continue;
                } else {
                    if (ignoreMode) {
                        continue;
                    } else {
                        return FileOperationStatus.FilterUnknownChar;
                    }
                }
                if (last) {
                    if (!ignoreMode) {
                        return FileOperationStatus.FilterUnknownChar;
                    }
                }
                tempDigit[tempIndex] = digit;
                if (tempIndex == 3) {
                    // 000000 111111 222222 333333
                    // 00000011 11112222 22333333
                    outStream.WriteByte((byte)((tempDigit[0] << 2) | (tempDigit[1] >> 4)));
                    outStream.WriteByte((byte)((tempDigit[1] << 4) | (tempDigit[2] >> 2)));
                    outStream.WriteByte((byte)((tempDigit[2] << 6) | (tempDigit[3])));
                    tempIndex = 0;
                } else {
                    tempIndex++;
                }
            }
            if (tempIndex >= 2) {
                outStream.WriteByte((byte)((tempDigit[0] << 2) | (tempDigit[1] >> 4)));
            }
            if (tempIndex >= 3) {
                outStream.WriteByte((byte)((tempDigit[1] << 4) | (tempDigit[2] >> 2)));
            }
            dest = outStream.ToArray(); 
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_B64Decode;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_B64DecodeExp;
            }
        }
    }
}
