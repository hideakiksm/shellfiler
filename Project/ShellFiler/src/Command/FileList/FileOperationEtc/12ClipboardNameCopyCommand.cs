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
    // マークされたファイルやフォルダのファイル名をクリップボードにコピーします。
    //   書式 　 ClipboardNameCopy()
    //   引数  　なし
    //   戻り値　bool:コピーに成功したときtrue、コピーできなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ClipboardNameCopyCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardNameCopyCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 開始条件の確認
            if (FileListViewTarget.FileList.MarkFiles.Count == 0) {
                return false;
            }

            // クリップボードにコピー
            List<UIFile> markFileList = FileListViewTarget.FileList.MarkFiles;
            List<string> fileNameList = new List<string>();
            foreach (UIFile file in markFileList) {
                fileNameList.Add(FileListViewTarget.FileList.DisplayDirectoryName + file.FileName);
            }
            string text = GenericFileStringUtils.CreateCommandFiles(fileNameList, false);
            Clipboard.SetDataObject(text, true);

            ClipboardCopyCommand.FlashMarkFiles(FileListViewTarget, markFileList);

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClipboardNameCopyCommand;
            }
        }
    }
}
