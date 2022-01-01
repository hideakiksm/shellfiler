using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
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
    // マークされたファイルやフォルダをクリップボードへ切り取ります。
    //   書式 　 ClipboardCut()
    //   引数  　なし
    //   戻り値　bool:切り取りに成功したときtrue、切り取りできなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ClipboardCutCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardCutCommand() {
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

            // クリップボードに切り取り
            List<UIFile> markFileList = FileListViewTarget.FileList.MarkFiles;
            List<string> stringFiles = new List<string>();
            foreach (UIFile file in markFileList) {
                stringFiles.Add(FileListViewTarget.FileList.DisplayDirectoryName + file.FileName);
            }
            IDataObject data = new DataObject(DataFormats.FileDrop, stringFiles.ToArray());
            byte[] bs = new byte[] { (byte)DragDropEffects.Move, 0, 0, 0 };
            MemoryStream ms = new MemoryStream(bs);
            data.SetData("Preferred DropEffect", ms);
            Clipboard.SetDataObject(data);

            ClipboardCopyCommand.FlashMarkFiles(FileListViewTarget, markFileList);

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClipboardCutCommand;
            }
        }
    }
}
