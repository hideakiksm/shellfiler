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
    // クラス：改行コードを変換するフィルター
    //=========================================================================================
    public class FileFilterCharsetConvert : IFileFilterComponent {
        // プロパティ
        private const string PROP_BEFORE_CODE = "BeforeCode";       // 変換前のコード
        private const string PROP_AFTER_CODE = "AfterCode";         // 変換後のコード

        private const string PROP_CODE_CHECK = "CodeCheck";         // エンコード不一致の場合
        private const string PROP_CODE_CHECK_SKIP = "Skip";         // このフィルターをスキップ
        private const string PROP_CODE_CHECK_ERROR = "Error";       // エラーにする
        private const string PROP_CODE_CHECK_IGNORE = "Ignore";     // 変換できない文字を無視

        // CR/LFのコード定義
        private const byte BYTE_00 = 0x00;                          // 00
        private const byte BYTE_CR = 0x0d;                          // CR
        private const byte BYTE_LF = 0x0a;                          // LF
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterCharsetConvert() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string strBefore = string.Format(Resources.FileFilter_CharsetDispBefore, (string)param[PROP_BEFORE_CODE]);
            string strAfter = string.Format(Resources.FileFilter_CharsetDispAfter, (string)param[PROP_AFTER_CODE]);
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
                string singleParameter = string.Format(Resources.FileFilter_CharsetSingle, (string)param[PROP_BEFORE_CODE], (string)param[PROP_AFTER_CODE]);
                return new string[] { singleParameter };
            } else {
                return new string[] { strBefore, strAfter, dispCheck };
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
            item.PropertyList.Add(PROP_BEFORE_CODE, EncodingType.AllTextValue[0].DisplayName);
            item.PropertyList.Add(PROP_AFTER_CODE, EncodingType.AllTextValue[1].DisplayName);
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
            if (mode == FileFilterDefinedMode.ShiftJISToUTF8) {
                property.Add(PROP_BEFORE_CODE, EncodingType.SJIS.DisplayName);
                property.Add(PROP_AFTER_CODE, EncodingType.UTF8.DisplayName);
            } else if (mode == FileFilterDefinedMode.ShiftJISToEUC) {
                property.Add(PROP_BEFORE_CODE, EncodingType.SJIS.DisplayName);
                property.Add(PROP_AFTER_CODE, EncodingType.EUC.DisplayName);
            } else if (mode == FileFilterDefinedMode.UTF8ToShiftJIS) {
                property.Add(PROP_BEFORE_CODE, EncodingType.UTF8.DisplayName);
                property.Add(PROP_AFTER_CODE, EncodingType.SJIS.DisplayName);
            } else if (mode == FileFilterDefinedMode.EUCToShiftJIS) {
                property.Add(PROP_BEFORE_CODE, EncodingType.UTF8.DisplayName);
                property.Add(PROP_AFTER_CODE, EncodingType.EUC.DisplayName);
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
            EncodingType[] encodingList = EncodingType.AllTextValue;
            string[] strEncodingList = new string[encodingList.Length];
            for (int i = 0; i < encodingList.Length; i++) {
                strEncodingList[i] = encodingList[i].DisplayName;
            }
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CharsetUI01Label, PROP_BEFORE_CODE, strEncodingList, strEncodingList));
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CharsetUI02Label, PROP_AFTER_CODE, strEncodingList, strEncodingList));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CharsetUI03Label, PROP_CODE_CHECK,
                                                    new string[] { Resources.FileFilter_CharsetUI03ItemError, Resources.FileFilter_CharsetUI03ItemSkip, Resources.FileFilter_CharsetUI03ItemIgnore },
                                                    new string[] { PROP_CODE_CHECK_ERROR, PROP_CODE_CHECK_SKIP, PROP_CODE_CHECK_IGNORE }));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_CharsetUI04Label, 1));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_CharsetUI05Label, 1));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            EncodingType[] allEncode = EncodingType.AllTextValue;
            List<string> strAllEncode = new List<string>();
            for (int i = 0; i < allEncode.Length; i++) {
                strAllEncode.Add(allEncode[i].DisplayName);
            }

            if (!param.ContainsKey(PROP_BEFORE_CODE) || !(param[PROP_BEFORE_CODE] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            if (!param.ContainsKey(PROP_AFTER_CODE) || !(param[PROP_AFTER_CODE] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string strBefore = (string)(param[PROP_BEFORE_CODE]);
            string strAfter = (string)(param[PROP_AFTER_CODE]);
            if (!strAllEncode.Contains(strBefore) || !strAllEncode.Contains(strAfter)) {
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
            dest = null;

            // 実行条件を取得
            string beforeCode = (string)(param[PROP_BEFORE_CODE]);
            string afterCode = (string)(param[PROP_AFTER_CODE]);
            if (beforeCode == afterCode) {
                dest = src;
                return FileOperationStatus.Success;
            }
            EncodingType beforeType = null;
            EncodingType afterType = null;
            EncodingType[] allType = EncodingType.AllTextValue;
            for (int i = 0; i < allType.Length; i++) {
                if (beforeCode == allType[i].DisplayName) {
                    beforeType = allType[i];
                }
                if (afterCode == allType[i].DisplayName) {
                    afterType = allType[i];
                }
            }
            CodeCheckMode errorMode = GetCodeCheckMode((string)(param[PROP_CODE_CHECK]));

            // 変換を実行
            bool success;
            if (beforeType == EncodingType.UNICODE) {
                success = ConvertUnicodeToOther(src, out dest, beforeType, afterType, errorMode);
            } else {
                success = ConvertMultiByteToOther(src, out dest, beforeType, afterType, errorMode);
            }
            if (!success) {
                if (errorMode == CodeCheckMode.Error) {
                    return FileOperationStatus.FilterUnknownChar;
                } else if (errorMode == CodeCheckMode.Skip) {
                    dest = src;
                    return FileOperationStatus.Success;
                } else {    // Ignoreはここにこない
                    Program.Abort("フィルターCharsetConvertで想定外のエラー状態です。");
                    return FileOperationStatus.Fail;
                }
            } else {
                return FileOperationStatus.Success;
            }
        }

        //=========================================================================================
        // 機　能：UNICODEから他の文字コードへの変換を実行する
        // 引　数：[in]src         変換元のバイト列
        // 　　　　[out]dest       変換先のバイト列を返す変数
        // 　　　　[in]srcEncode   変換元のエンコード方式
        // 　　　　[in]destEncode  変換先のエンコード方式
        // 　　　　[in]errorMode   エラー発生時のモード
        // 戻り値：変換に成功したときtrue
        //=========================================================================================
        private bool ConvertUnicodeToOther(byte[] src, out byte[] dest, EncodingType srcEncode, EncodingType destEncode, CodeCheckMode errorMode) {
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
                        byte[] destPart = ConvertStringPart(src, startIndex, i, srcEncode, destEncode, errorMode);
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
                byte[] destPart = ConvertStringPart(src, startIndex, length, srcEncode, destEncode, errorMode);
                if (destPart == null) {
                    return false;
                }
                destStream.Write(destPart, 0, destPart.Length);
            }
            dest = destStream.ToArray();
            return true;
        }

        //=========================================================================================
        // 機　能：UNICODE以外から他の文字コードへの変換を実行する
        // 引　数：[in]src         変換元のバイト列
        // 　　　　[out]dest       変換先のバイト列を返す変数
        // 　　　　[in]srcEncode   変換元のエンコード方式
        // 　　　　[in]destEncode  変換先のエンコード方式
        // 　　　　[in]errorMode   エラー発生時のモード
        // 戻り値：変換に成功したときtrue
        //=========================================================================================
        private bool ConvertMultiByteToOther(byte[] src, out byte[] dest, EncodingType srcEncode, EncodingType destEncode, CodeCheckMode errorMode) {
            dest = null;
            MemoryStream destStream = new MemoryStream();
            int startIndex = 0;
            int length = src.Length;
            for (int i = 0; i < length; i++) {
                if (src[i] == BYTE_CR || src[i] == BYTE_LF) {
                    if (startIndex != i) {
                        byte[] destPart = ConvertStringPart(src, startIndex, i, srcEncode, destEncode, errorMode);
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
                byte[] destPart = ConvertStringPart(src, startIndex, length, srcEncode, destEncode, errorMode);
                if (destPart == null) {
                    return false;
                }
                destStream.Write(destPart, 0, destPart.Length);
            }
            dest = destStream.ToArray();
            return true;
        }

        //=========================================================================================
        // 機　能：部分文字列の文字コードを変換する
        // 引　数：[in]src         変換元のバイト列
        // 　　　　[in]startIndex  開始位置のインデックス
        // 　　　　[in]endInde     終了位置の次のインデックス
        // 　　　　[in]srcEncode   変換元のエンコード方式
        // 　　　　[in]destEncode  変換先のエンコード方式
        // 　　　　[in]errorMode   エラー発生時のモード
        // 戻り値：変換結果（変換エラーのときnull）
        //=========================================================================================
        private byte[] ConvertStringPart(byte[] src, int startIndex, int endIndex, EncodingType srcEncode, EncodingType destEncode, CodeCheckMode errorMode) {
            bool codeCheck = (errorMode != CodeCheckMode.Ignore);

            // src→文字列化
            string str = srcEncode.Encoding.GetString(src, startIndex, endIndex - startIndex);
            if (codeCheck) {
                byte[] srcCheck = srcEncode.Encoding.GetBytes(str);
                bool success = ArrayUtils.CompareByteArray(src, startIndex, endIndex - startIndex, srcCheck, 0, srcCheck.Length);
                if (!success) {
                    return null;
                }
            }

            // 文字列→dest
            byte[] destPart = destEncode.Encoding.GetBytes(str);
            if (codeCheck) {
                string destCheck = destEncode.Encoding.GetString(destPart);
                if (destCheck != str) {
                    return null;
                }
            }
            return destPart;
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
                return Resources.FileFilter_CharsetConvertName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_CharsetConvertExp;
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
