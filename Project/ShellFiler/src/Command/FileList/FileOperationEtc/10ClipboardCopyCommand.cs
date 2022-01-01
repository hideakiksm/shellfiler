using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルやフォルダをクリップボードにコピーします。
    //   書式 　 ClipboardCopy()
    //   引数  　なし
    //   戻り値　bool:コピーに成功したときtrue、コピーできなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ClipboardCopyCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardCopyCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 開始条件の確認
            FileSystemID id = FileListViewTarget.FileList.FileSystem.FileSystemId;
            if (!FileSystemID.IsWindows(id)) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_ClipboardWindowsOnly);
                return false;
            }
            if (FileListViewTarget.FileList.MarkFiles.Count == 0) {
                return false;
            }

            // クリップボードにコピー
            List<UIFile> markFileList = FileListViewTarget.FileList.MarkFiles;
            StringCollection stringFiles = new StringCollection();
            foreach (UIFile file in markFileList) {
                stringFiles.Add(FileListViewTarget.FileList.DisplayDirectoryName + file.FileName);
            }
            Clipboard.SetFileDropList(stringFiles);

            FlashMarkFiles(FileListViewTarget, markFileList);

            return true;
        }

        //=========================================================================================
        // 機　能：マークファイルを点滅させる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void FlashMarkFiles(FileListView fileListView, List<UIFile> markFileList) {
            // マークをすべて消す
            List<int> markOrderBackup = new List<int>();
            StringCollection stringFiles = new StringCollection();
            for (int i = 0; i < markFileList.Count; i++) {
                UIFile file = markFileList[i];
                markOrderBackup.Add(file.MarkOrder);
                file.MarkOrder = 0;
            }

            // 再描画
            fileListView.Invalidate();
            fileListView.Update();
            Thread.Sleep(50);
            
            // マークを戻す
            for (int i = 0; i < markFileList.Count; i++) {
                UIFile file = markFileList[i];
                file.MarkOrder = markOrderBackup[i];
            }

            // 再描画
            fileListView.Invalidate();
            fileListView.Update();
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClipboardCopyCommand;
            }
        }
    }
}
