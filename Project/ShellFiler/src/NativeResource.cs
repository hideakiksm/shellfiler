using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileViewer;
using ShellFiler.Virtual;

namespace ShellFiler {

    //=========================================================================================
    // クラス：ネイティブリソースの識別子
    //=========================================================================================
    public class NativeResources {
        // 外部ソフトウェアについてのHTML
        public static string HtmlAboutExternalSoftware = "101";
        // マウスの使用方法
        public static string HtmlHowToUseMouse = "102";
        // スライドショーマーク機能の使用方法
        public static string HtmlSlideShowMark = "103";
        // コマンド引数のパラメータ
        public static string HtmlCommandArgument = "106";
        // 転送条件の対象
        public static string HtmlTransferConditionTarget = "108";
        // 転送条件のファイル名
        public static string HtmlTransferConditionFileName = "109";
        // 鍵の生成とテスト
        public static string HtmlSSHPrivateKey = "110";
        // SSH用のエディタ
        public static string HtmlSSHEditor = "111";
        // SSHプロトコル
        public static string HtmlSSHProtocol = "112";
        // SSHセッション
        public static string HtmlSSHSession = "113";

        // 外部へのリンクイメージ
        public static string PngHtmlLink = "201";
        // マウスの使用方法のドラッグ＆ドロップイメージ
        public static string PngMouseDragDropImage = "202";
        // スライドショーマーク機能の使用方法
        public static string PngSlideShowMarkImage = "203";
        // 転送条件のファイルアイコン
        public static string PngFile = "204";
        // 転送条件のフォルダアイコン
        public static string PngFolder = "205";
        // 転送条件のツリーの例
        public static string PngTransferConditionTreeExample = "206";
    }
}
