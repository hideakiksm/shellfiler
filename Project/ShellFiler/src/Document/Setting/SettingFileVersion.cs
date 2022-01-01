using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Locale;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定ファイルのバージョン
    //=========================================================================================
    public class SettingFileVersion {

        //=========================================================================================
        // 機　能：現在のバージョンを返す
        // 引　数：なし
        // 戻り値：現在のバージョン
        //=========================================================================================
        public static int GetCurrentVersion() {
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            int version = ver.FileMajorPart * 1000000 + ver.FileMinorPart * 1000 + ver.FilePrivatePart;
            return version;
        }
    }
}
