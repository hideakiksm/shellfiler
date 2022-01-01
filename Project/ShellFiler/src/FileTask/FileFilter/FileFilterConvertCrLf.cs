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
    // クラス：改行コードを変換するフィルター
    //=========================================================================================
    public class FileFilterConvertCrLf : IFileFilterComponent {
        // プロパティ
        private const string PROP_AFTER_CODE = "AfterCode";         // 変換後のコード
        private const string PROP_AFTER_CODE_CR = "CR";             // CRだけ
        private const string PROP_AFTER_CODE_LF = "LF";             // LFだけ
        private const string PROP_AFTER_CODE_CRLF = "CRLF";         // CR+LF

        private const string PROP_CODE_UNICODE = "Unicode";         // Unicodeを扱うときtrue

        // CR/LFのコード定義
        private byte BYTE_00 = 0x00;                                // 00
        private byte BYTE_CR = 0x0d;                                // CR
        private byte BYTE_LF = 0x0a;                                // LF

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterConvertCrLf() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string dispCode;
            string strCode = (string)param[PROP_AFTER_CODE];
            if (strCode == PROP_AFTER_CODE_CR) {
                dispCode = Resources.FileFilter_CrLfDispCr;
            } else if (strCode == PROP_AFTER_CODE_LF) {
                dispCode = Resources.FileFilter_CrLfDispLf;
            } else {
                dispCode = Resources.FileFilter_CrLfDispCrLf;
            }
            string dispUnicode;
            bool unicode = (bool)param[PROP_CODE_UNICODE];
            if (unicode) {
                dispUnicode = Resources.FileFilter_CrLfUnicode;
            } else {
                dispUnicode = Resources.FileFilter_CrLfMultibyte;
            }
            if (single) {
                return new string[] { dispCode };
            } else {
                return new string[] { dispCode, dispUnicode };
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
            item.PropertyList.Add(PROP_AFTER_CODE, PROP_AFTER_CODE_CRLF);
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
            if (mode == FileFilterDefinedMode.ReturnCRLF) {
                property.Add(PROP_AFTER_CODE, PROP_AFTER_CODE_CRLF);
                property.Add(PROP_CODE_UNICODE, false);
            } else if (mode == FileFilterDefinedMode.ReturnLF) {
                property.Add(PROP_AFTER_CODE, PROP_AFTER_CODE_LF);
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
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CrLfUI01Label, PROP_AFTER_CODE,
                                                    new string[] { Resources.FileFilter_CrLfUI01ItemCr, Resources.FileFilter_CrLfUI01ItemLf, Resources.FileFilter_CrLfUI01ItemCrLf },
                                                    new string[] { PROP_AFTER_CODE_CR, PROP_AFTER_CODE_LF, PROP_AFTER_CODE_CRLF }));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Checkbox(Resources.FileFilter_CrLfUI02Check, PROP_CODE_UNICODE));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_CrLfUI03Label, 1));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_CrLfUI04Label, 1));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_AFTER_CODE) || !(param[PROP_AFTER_CODE] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string afterCode = (string)(param[PROP_AFTER_CODE]);
            if (afterCode != PROP_AFTER_CODE_CR && afterCode != PROP_AFTER_CODE_LF && afterCode != PROP_AFTER_CODE_CRLF) {
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
            CrLfMode afterCode;
            string strMode = (string)param[PROP_AFTER_CODE];
            if (strMode == PROP_AFTER_CODE_CR) {
                afterCode = CrLfMode.CR;
            } else if (strMode == PROP_AFTER_CODE_LF) {
                afterCode = CrLfMode.LF;
            } else {
                afterCode = CrLfMode.CRLF;
            }

            // 処理を実行
            bool unicode = (bool)param[PROP_CODE_UNICODE];
            if (!unicode) {
                return ConvertMultiByteString(src, out dest, afterCode);
            } else {
                return ConvertUnicodeString(src, out dest, afterCode);
            }
        }

        //=========================================================================================
        // 機　能：バイト列をマルチバイトと見なして変換する
        // 引　数：[in]src        変換元のバイト列
        // 　　　　[out]dest      変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]afterCode  変換する改行コード
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ConvertMultiByteString(byte[] src, out byte[] dest, CrLfMode afterCode) {
            MemoryStream outStream = new MemoryStream();
            int length = src.Length;
            for (int i = 0; i < length; i++) {
                bool addRet = false;
                if (src[i] == BYTE_CR) {
                    addRet = true;
                    if (i + 1 < length && src[i + 1] == BYTE_LF) {
                        i++;
                    }
                } else if (src[i] == BYTE_LF) {
                    addRet = true;
                }
                if (addRet) {
                    if (afterCode == CrLfMode.CR) {
                        outStream.WriteByte(BYTE_CR);
                    } else if (afterCode == CrLfMode.LF) {
                        outStream.WriteByte(BYTE_LF);
                    } else {
                        outStream.WriteByte(BYTE_CR);
                        outStream.WriteByte(BYTE_LF);
                    }
                } else {
                    outStream.WriteByte(src[i]);
                }
            }
            dest = outStream.ToArray(); 
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：バイト列をUNICODEと見なして変換する
        // 引　数：[in]src        変換元のバイト列
        // 　　　　[out]dest      変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]afterCode  変換する改行コード
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ConvertUnicodeString(byte[] src, out byte[] dest, CrLfMode afterCode) {
            MemoryStream outStream = new MemoryStream();
            int length = (src.Length & 0x7ffffffe);
            for (int i = 0; i < length; i += 2) {
                bool addRet = false;
                if (src[i] == BYTE_CR && src[i + 1] == BYTE_00) {
                    addRet = true;
                    if (i + 3 < length && src[i + 2] == BYTE_LF && src[i + 3] == BYTE_00) {
                        i += 2;
                    }
                } else if (src[i] == BYTE_LF && src[i + 1] == BYTE_00) {
                    addRet = true;
                }
                if (addRet) {
                    if (afterCode == CrLfMode.CR) {
                        outStream.WriteByte(BYTE_CR);
                        outStream.WriteByte(BYTE_00);
                    } else if (afterCode == CrLfMode.LF) {
                        outStream.WriteByte(BYTE_LF);
                        outStream.WriteByte(BYTE_00);
                    } else {
                        outStream.WriteByte(BYTE_CR);
                        outStream.WriteByte(BYTE_00);
                        outStream.WriteByte(BYTE_LF);
                        outStream.WriteByte(BYTE_00);
                    }
                } else {
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
                return Resources.FileFilter_CrLfConvertName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_CrLfConvertExp;
            }
        }

        //=========================================================================================
        // 列挙子：変換モード
        //=========================================================================================
        private enum CrLfMode {
            CR,             // CRのみ
            LF,             // LFのみ
            CRLF,           // CR+LF
        }
    }
}
