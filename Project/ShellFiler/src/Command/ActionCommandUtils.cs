using System;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Document;

namespace ShellFiler.Command {
    
    //=========================================================================================
    // クラス：コマンド実行で使用する汎用のライブラリ
    //=========================================================================================
    public class ActionCommandUtils {

        //=========================================================================================
        // 機　能：表示中のカレントパスを使って保存先等のWindowsデフォルトフォルダを返す
        // 引　数：なし
        // 戻り値：Windowsのフォルダ
        // メ　モ：対象パスがWindowsのときは表示中のフォルダ、それ以外のときはデスクトップ
        //=========================================================================================
        public static string GetDefaultWindowsFolder() {
            string folder;
            FileSystemID targetFileSystemId;
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                targetFileSystemId = Program.Document.CurrentTabPage.LeftFileList.FileSystem.FileSystemId;
                folder = Program.Document.CurrentTabPage.LeftFileList.DisplayDirectoryName;
            } else {
                targetFileSystemId = Program.Document.CurrentTabPage.RightFileList.FileSystem.FileSystemId;
                folder = Program.Document.CurrentTabPage.RightFileList.DisplayDirectoryName;
            }
            if (!FileSystemID.IsWindows(targetFileSystemId)) {
                folder = UIFileList.InitialFolder;
            }
            return folder;
        }
    }
}
