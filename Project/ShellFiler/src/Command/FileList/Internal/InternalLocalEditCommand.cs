using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Command.FileList.Tools;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.Internal {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 指定ファイルをローカルPCで編集します。
    //   書式 　 InternalLocalEditCommand()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.1.0
    //=========================================================================================
    class InternalLocalEditCommand : FileListActionCommand {
        // 対象ファイルパス
        private string m_filePath;

        // 対象ファイルのファイルシステム
        private FileSystemID m_srcFileSystem;

        // 編集対象の行番号
        private int m_targetLineNo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public InternalLocalEditCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_filePath = (string)param[0];
            m_srcFileSystem = (FileSystemID)param[1];
            m_targetLineNo = (int)param[2];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 実行条件を確認
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, m_srcFileSystem, true);
            if (!canStart) {
                return null;
            }

            // 表示名を作成
            string fileName = GenericFileStringUtils.GetFileName(m_filePath);
            string dispName = string.Format(Resources.LocalExec_DisplayNameEdit1, fileName);
            TemporarySpaceDisplayName nameInfo = new TemporarySpaceDisplayName(dispName, IconImageListID.FileList_LocalEdit);

            // コマンドラインを作成
            // command="C:\…\Editor.exe -a -b {0}"
            // program="C:\…\Editor.exe"
            // argument="-a -b {0}";
            bool withLineNumber;
            string command = LocalEditCommand.GetEditorCommandLineWithLineNum(m_srcFileSystem, out withLineNumber);
            if (command == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_EditorCommandLineAssoc);
                return null;
            }
            string program, argument;
            GenericFileStringUtils.SplitCommandLine(command, out program, out argument);    
            LocalExecuteBackgroundTask.GetProgramArgumentDelegate argDelegate = delegate(List<string> srcPathList, List<string> destPathList) {
                string fileListCommand = GenericFileStringUtils.CreateCommandFiles(srcPathList, false);
                if (withLineNumber) {
                    return string.Format(argument, fileListCommand, m_targetLineNo);
                } else {
                    return string.Format(argument, fileListCommand);
                }
            };

            // バックグラウンドタスクで編集を実行
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(m_srcFileSystem);
            RefreshUITarget uiTarget = new RefreshUITarget(null, null, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            List<SimpleFileDirectoryPath> pathList = new List<SimpleFileDirectoryPath>();
            pathList.Add(new SimpleFileDirectoryPath(m_filePath, false, false));
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(pathList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestDummy();
            string targetDir = GenericFileStringUtils.GetDirectoryName(m_filePath);
            LocalExecuteBackgroundTask task = new LocalExecuteBackgroundTask(srcProvider, destProvider, uiTarget, program, argDelegate, nameInfo, targetDir, null);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.LocalEditCommand;
            }
        }
    }
}
