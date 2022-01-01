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
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：HTTPヘッダとHTTPボディを分離するフィルター
    //=========================================================================================
    public class FileFilterHttpHeaderBody : IFileFilterComponent {
        // プロパティ
        private const string PROP_USING_PART = "UsingPart";         // 使用する部分
        private const string PROP_USING_PART_HEADER = "Header";     // ヘッダ部分を使用
        private const string PROP_USING_PART_BODY = "Body";         // ボディ部分を使用
        private const string PROP_ERROR_MODE = "ErrorMode";         // 想定外データに対する振る舞い
        private const string PROP_ERROR_MODE_ERROR = "Error";       // エラーにする
        private const string PROP_ERROR_MODE_SKIP = "Skip";         // このフィルターをスキップ

        // CR/LFのコード定義
        private const byte BYTE_CR = 0x0d;                          // CR
        private const byte BYTE_LF = 0x0a;                          // LF
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterHttpHeaderBody() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string dispUsingPart;
            string strUsingPart = (string)param[PROP_USING_PART];
            if (strUsingPart == PROP_USING_PART_HEADER) {
                dispUsingPart = Resources.FileFilter_HttpHeaderBodyUseHeader;
            } else {
                dispUsingPart = Resources.FileFilter_HttpHeaderBodyUseBody;
            }

            string dispError;
            string strError = (string)param[PROP_ERROR_MODE];
            if (strError == PROP_ERROR_MODE_ERROR) {
                dispError = Resources.FileFilter_HttpHeaderErrorSkip;
            } else {
                dispError = Resources.FileFilter_HttpHeaderErrorSkip;
            }

            if (single) {
                return new string[] { dispUsingPart };
            } else {
                return new string[] { dispUsingPart, dispError };
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
            item.PropertyList.Add(PROP_USING_PART, PROP_USING_PART_BODY);
            item.PropertyList.Add(PROP_ERROR_MODE, PROP_ERROR_MODE_ERROR);
            return item;
        }
        
        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_HttpHeaderBodyUI01Label, PROP_USING_PART,
                                                    new string[] { Resources.FileFilter_HttpHeaderBodyUI01ItemHeader, Resources.FileFilter_HttpHeaderBodyUI01ItemBody },
                                                    new string[] { PROP_USING_PART_HEADER, PROP_USING_PART_BODY }));
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_HttpHeaderBodyUI02Label, PROP_ERROR_MODE,
                                                    new string[] { Resources.FileFilter_HttpHeaderBodyUI02ItemError, Resources.FileFilter_HttpHeaderBodyUI02ItemSkip },
                                                    new string[] { PROP_ERROR_MODE_ERROR, PROP_ERROR_MODE_SKIP }));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_USING_PART) || !(param[PROP_USING_PART] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string strUsingPart = (string)(param[PROP_USING_PART]);
            if (strUsingPart != PROP_USING_PART_HEADER && strUsingPart != PROP_USING_PART_BODY) {
                return Resources.FileFilter_MsgSerializeError;
            }

            if (!param.ContainsKey(PROP_ERROR_MODE) || !(param[PROP_ERROR_MODE] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string strErrorMode = (string)(param[PROP_ERROR_MODE]);
            if (strErrorMode != PROP_ERROR_MODE_ERROR && strErrorMode != PROP_ERROR_MODE_SKIP) {
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

            // 実行条件を取得
            string strUsingPart = (string)(param[PROP_USING_PART]);
            string strErrorMode = (string)(param[PROP_ERROR_MODE]);

            // 分割位置を取得
            int separator = -1;
            for (int i = 0; i < src.Length - 4; i++) {
                if (src[i] == BYTE_CR && src[i + 1] == BYTE_LF && src[i + 2] == BYTE_CR && src[i + 3] == BYTE_LF) {
                    separator = i;
                }
            }

            // 想定外エラー
            if (separator == -1) {
                if (strErrorMode == PROP_ERROR_MODE_SKIP) {
                    dest = src;
                    return FileOperationStatus.Success;
                } else {
                    return FileOperationStatus.FilterUnexpected;
                }
            }

            if (strUsingPart == PROP_USING_PART_HEADER) {
                // ヘッダを残す
                dest = new byte[separator];
                Array.Copy(src, dest, dest.Length);
            } else {
                // ボディを残す
                dest = new byte[src.Length - separator - 4];
                Array.Copy(src, separator + 4, dest, 0, dest.Length);
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_HttpHeaderBodyName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_HttpHeaderBodyExp;
            }
        }
    }
}
