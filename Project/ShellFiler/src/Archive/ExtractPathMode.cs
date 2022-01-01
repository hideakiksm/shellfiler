using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：展開先パスのモード
    //=========================================================================================
    public class ExtractPathMode {
        public static readonly ExtractPathMode None = new ExtractPathMode("None");                                          // 指定なし
        public static readonly ExtractPathMode Direct = new ExtractPathMode("Direct");                                      // 展開先そのまま
        public static readonly ExtractPathMode AlwaysNewDirectory = new ExtractPathMode("AlwaysNewDirectory");              // 常にディレクトリを作成する
        public static readonly ExtractPathMode NewDirectoryIsSameExist = new ExtractPathMode("NewDirectoryIsSameExist");    // 混ざる心配があるときだけディレクトリを作成する

        // デバッグ用文字列
        private string m_debugString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]debugString  デバッグ用文字列
        // 戻り値：なし
        //=========================================================================================
        private ExtractPathMode(string debugString) {
            m_debugString = debugString;
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した値
        //=========================================================================================
        public static ExtractPathMode FromSerializedData(string serialized) {
            if (serialized == "Direct") {
                return Direct;
            } else if (serialized == "AlwaysNewDirectory") {
                return AlwaysNewDirectory;
            } else if (serialized == "NewDirectoryIsSameExist") {
                return NewDirectoryIsSameExist;
            } else {
                return NewDirectoryIsSameExist;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(ExtractPathMode obj) {
            if (obj == Direct) {
                return "Direct";
            } else if (obj == AlwaysNewDirectory) {
                return "AlwaysNewDirectory";
            } else if (obj == NewDirectoryIsSameExist) {
                return "NewDirectoryIsSameExist";
            } else {
                return "NewDirectoryIsSameExist";
            }
        }
    }
}
