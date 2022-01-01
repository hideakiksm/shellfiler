using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：UIの地域情報の吸収クラス
    //=========================================================================================
    class UILocale {

        //=========================================================================================
        // プロパティ：Windowsのデフォルトフォント名
        //=========================================================================================
        public static string WindowsDefaultFontName {
            get {
                OperatingSystem os = Environment.OSVersion;
                if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6) {       // Vista以上
                    return "メイリオ";
                } else {
                    return "ＭＳ Ｐゴシック";
                }
            }
        }

        //=========================================================================================
        // プロパティ：WindowsのUI表示用フォント名
        //=========================================================================================
        public static string WindowsUIFontName {
            get {
                return "ＭＳ Ｐゴシック";
            }
        }
    }
}
