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
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：ShellFilerのダンプ形式で整形するフィルター
    //=========================================================================================
    public class FileFilterShellFilerDump : IFileFilterComponent {
        // プロパティ
        private const string PROP_DUMP_WIDTH = "DumpWidth";             // 1行に表示するバイト数
        private const string PROP_TEXT_IN_ENCODING = "InEncoding";      // テキストの入力エンコード方式
        private const string PROP_TEXT_OUT_ENCODING = "OutEncoding";    // ダンプの出力エンコード方式

        private const int DUMP_WIDTH_MIN = 8;                           // 1行に表示するバイト数の最小値
        private const int DUMP_WIDTH_MAX = 64;                          // 1行に表示するバイト数の最大値

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterShellFilerDump() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            int dumpWidth= (int)param[PROP_DUMP_WIDTH];
            string dispDumpWidth = string.Format(Resources.FileFilter_ShellDumpWidth, dumpWidth);

            string strInEncoding = (string)param[PROP_TEXT_IN_ENCODING];
            string dispInEncoding = string.Format(Resources.FileFilter_ShellDumpTextInEncoding, strInEncoding);

            string strOutEncoding = (string)param[PROP_TEXT_OUT_ENCODING];
            string dispOutEncoding = string.Format(Resources.FileFilter_ShellDumpTextOutEncoding, strOutEncoding);

            if (single) {
                return new string[] { dispDumpWidth };
            } else {
                return new string[] { dispDumpWidth, dispInEncoding, dispOutEncoding };
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
            item.PropertyList.Add(PROP_DUMP_WIDTH, 16);
            item.PropertyList.Add(PROP_TEXT_IN_ENCODING, EncodingType.UTF8.DisplayName);
            item.PropertyList.Add(PROP_TEXT_OUT_ENCODING, EncodingType.UTF8.DisplayName);
            return item;
        }

        //=========================================================================================
        // 機　能：フィルター設定用のプロパティ情報を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用のプロパティ情報
        //=========================================================================================
        public static Dictionary<string, object> CreateProperty() {
            Dictionary<string, object> property = new Dictionary<string, object>();
            property.Add(PROP_DUMP_WIDTH, 16);
            property.Add(PROP_TEXT_IN_ENCODING, EncodingType.UTF8.DisplayName);
            property.Add(PROP_TEXT_OUT_ENCODING, EncodingType.UTF8.DisplayName);
            return property;
        } 

        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            EncodingType[] encodingList = EncodingType.AllTextValue;
            string[] strEncodingList = new string[encodingList.Length];
            for (int i = 0; i < encodingList.Length; i++) {
                strEncodingList[i] = encodingList[i].DisplayName;
            }
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Numeric(Resources.FileFilter_ShellDumpUI01Label, PROP_DUMP_WIDTH, DUMP_WIDTH_MIN, DUMP_WIDTH_MAX));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_ShellDumpUI02Label, PROP_TEXT_IN_ENCODING, strEncodingList, strEncodingList));
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_ShellDumpUI03Label, PROP_TEXT_OUT_ENCODING, strEncodingList, strEncodingList));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_DUMP_WIDTH) || !(param[PROP_DUMP_WIDTH] is int)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            int dumpWidth = (int)(param[PROP_DUMP_WIDTH]);
            if (dumpWidth < DUMP_WIDTH_MIN || DUMP_WIDTH_MAX < dumpWidth) {
                return Resources.FileFilter_MsgSerializeError;
            }

            EncodingType[] allEncode = EncodingType.AllTextValue;
            List<string> strAllEncode = new List<string>();
            for (int i = 0; i < allEncode.Length; i++) {
                strAllEncode.Add(allEncode[i].DisplayName);
            }

            if (!param.ContainsKey(PROP_TEXT_IN_ENCODING) || !(param[PROP_TEXT_IN_ENCODING] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string strTextInEncoding = (string)(param[PROP_TEXT_IN_ENCODING]);
            string strTextOutEncoding = (string)(param[PROP_TEXT_OUT_ENCODING]);
            if (!strAllEncode.Contains(strTextInEncoding) || !strAllEncode.Contains(strTextOutEncoding)) {
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
            int dumpWidth = (int)(param[PROP_DUMP_WIDTH]);
            string strInEncoding = (string)(param[PROP_TEXT_IN_ENCODING]);
            string strOutEncoding = (string)(param[PROP_TEXT_OUT_ENCODING]);
            EncodingType inEncoding = EncodingType.FromString(strInEncoding);
            EncodingType outEncoding = EncodingType.FromString(strOutEncoding);

            MemoryStream result = new MemoryStream();
            StreamWriter writer = new StreamWriter(result, outEncoding.Encoding);
            DumpHexFormatter hexFormatter = new DumpHexFormatter();
            DumpTextFormatter textFormatter = new DumpTextFormatter();

            // ダンプ部分の長さを測定
            byte[] dummyData = new byte[dumpWidth];
            Array.Clear(dummyData, 0, dummyData.Length);
            string dummyMaxLen;
            List<int> dummyMaxLenPosition;
            hexFormatter.CreateDumpHexStr(dummyData, 0, dummyData.Length, dumpWidth, out dummyMaxLen, out dummyMaxLenPosition);

            for (int i = 0; i < src.Length; i += dumpWidth) {
                // ダンプ部分
                string strDump;
                List<int> dumpPosition;
                int length = Math.Min(dumpWidth, src.Length - i);
                hexFormatter.CreateDumpHexStr(src, i, length, dumpWidth, out strDump, out dumpPosition);

                // テキスト部分
                string strText;
                List<int> charToByte, byteToChar;
                textFormatter.Convert(inEncoding, src, i, length, out strText, out charToByte, out byteToChar);

                // 連結
                writer.Write(strDump);
                StringBuilder separator = new StringBuilder();
                separator.Append(' ', dummyMaxLen.Length + 2 - strDump.Length);
                separator.Append("| ");
                writer.Write(separator.ToString());
                writer.Write(strText);
                writer.Write("\r\n");
            }
            writer.Close();
            dest = result.ToArray();
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_ShellDumpName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_ShellDumpExp;
            }
        }
    }
}
