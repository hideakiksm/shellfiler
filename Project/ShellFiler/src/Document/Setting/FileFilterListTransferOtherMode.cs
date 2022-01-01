using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイルフィルター転送時、詳細設定でのその他のファイルの扱い
    //=========================================================================================
    public class FileFilterListTransferOtherMode {
        public static readonly FileFilterListTransferOtherMode SkipTransfer  = new FileFilterListTransferOtherMode("SkipTransfer");     // 転送をスキップする
        public static readonly FileFilterListTransferOtherMode UseSourceFile = new FileFilterListTransferOtherMode("UseSourceFile");    // 転送元をそのまま使用する

        // デバッグ用文字列
        private string m_debugString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]debugString  デバッグ用文字列
        // 戻り値：なし
        //=========================================================================================
        public FileFilterListTransferOtherMode(string debugString) {
            m_debugString = debugString;
        }

        //=========================================================================================
        // 機　能：FileFilterListTransferOtherModeを文字列に変換する
        // 引　数：[in]strValue      変換する値
        // 　　　　[in]defaultValue  変換できない場合のデフォルト値
        // 戻り値：変換した値
        //=========================================================================================
        public static FileFilterListTransferOtherMode StringToTransferMode(string strValue, FileFilterListTransferOtherMode defaultValue) {
            if (strValue == "SkipTransfer") {
                return FileFilterListTransferOtherMode.SkipTransfer;
            } else if (strValue == "UseSourceFile") {
                return FileFilterListTransferOtherMode.UseSourceFile;
            } else {
                return defaultValue;
            }
        }

        //=========================================================================================
        // 機　能：文字列をFileFilterListTransferOtherModeに変換する
        // 引　数：[in]value  変換する値
        // 戻り値：変換した値
        //=========================================================================================
        public static string TransferModeToString(FileFilterListTransferOtherMode value) {
            if (value == FileFilterListTransferOtherMode.SkipTransfer) {
                return "SkipTransfer";
            } else if (value == FileFilterListTransferOtherMode.UseSourceFile) {
                return "UseSourceFile";
            } else {
                Program.Abort("未定義のFileFilterListTransferOtherModeです。");
                return null;
            }
        }
    }
}
