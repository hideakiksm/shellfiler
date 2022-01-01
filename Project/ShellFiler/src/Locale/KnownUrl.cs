using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：URL情報
    //=========================================================================================
    class KnownUrl {
        
        //=========================================================================================
        // プロパティ：ShellFilerのページ
        //=========================================================================================
        public static string ShellFilerUrl {
            get {
                return "https://github.com/hideakiksm/shellfiler";
            }
        }

        //=========================================================================================
        // プロパティ：差分表示ツールについてのページ
        //=========================================================================================
        public static string DiffToolUrl {
            get {
                return "https://github.com/hideakiksm/shellfiler/#連携ソフトウェア";
            }
        }

        //=========================================================================================
        // プロパティ：SharpSSHのページ
        //=========================================================================================
        public static string SharpSSHUrl {
            get {
                return "https://ja.osdn.net/projects/sfnet_sharpssh/";
            }
        }

        //=========================================================================================
        // プロパティ：7-Zipのページ
        //=========================================================================================
        public static string SevenZipUrl {
            get {
                return "https://sevenzip.osdn.jp/";
            }
        }

        //=========================================================================================
        // プロパティ：Windowsのダウンロードフォルダ
        //=========================================================================================
        public static string WindowsDownloadFolder {
            get {
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (myDocuments.EndsWith("\\")) {
                    myDocuments = myDocuments.Substring(0, myDocuments.Length - 1);
                }
                string myDocParentDownload = Path.Combine(Path.GetDirectoryName(myDocuments), "Downloads");
                return myDocParentDownload;
            }
        }
    }
}
