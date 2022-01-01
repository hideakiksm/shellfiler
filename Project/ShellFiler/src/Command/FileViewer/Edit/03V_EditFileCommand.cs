using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Provider;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.Command.FileList;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.FileViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 現在の表示中の位置を中心としてテキストエディタでの編集を行います。
    //   書式 　 V_EditFile()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.1.0
    //=========================================================================================
    class V_EditFileCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_EditFileCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            IFileViewerDataSource file = TextFileViewer.FileViewerForm.CurrentFile;
            if (file.TargetFileSystem == FileSystemID.None) {
                InfoBox.Warning(TextFileViewer.FileViewerForm, Resources.Msg_FileViewerCanNotEditFile);
                return null;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(TextFileViewer.FileViewerForm, file.TargetFileSystem, true);
            if (!canStart) {
                return null;
            }
            if (!(TextFileViewer.TextViewerComponent is TextViewerComponent)) {
                InfoBox.Warning(TextFileViewer.FileViewerForm, Resources.Msg_FileViewerCanNotEditDump);
                return null;
            }

            // キーに対応するコマンドを取得して実行
            int topLine = TextFileViewer.TextViewerComponent.VisibleTopLine;
            int bottomLine = Math.Min(TextFileViewer.TextBufferLineInfo.LogicalLineCount - 1, topLine + TextFileViewer.TextViewerComponent.CompleteLineSize);
            int lineNumber = (TextFileViewer.TextBufferLineInfo.GetLineInfo(topLine).PhysicalLineNo + TextFileViewer.TextBufferLineInfo.GetLineInfo(bottomLine).PhysicalLineNo) / 2;
            FileListActionCommand command = new InternalLocalEditCommand();
            command.SetParameter(file.FilePath, file.TargetFileSystem, lineNumber);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_EditFileCommand;
            }
        }
    }
}
