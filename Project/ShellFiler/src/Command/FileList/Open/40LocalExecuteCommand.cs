using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.ArgumentConverter;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 指定されたコマンド{0}を、コマンド引数{1}によりローカルPCで実行します。SSHでは、対象パスのファイルの読み込みは{2}で、反対パスのファイルは{3}でダウンロードしてから実行します。
    //   書式 　 LocalExecute(string program, string argument, string targetuse, string oppositeuse)
    //   引数  　program:起動するプログラム名
    // 　　　　　program-default:notepad.exe
    // 　　　　　program-range:
    // 　　　　　argument:コマンド引数
    // 　　　　　argument-default:$P
    // 　　　　　argument-range:
    // 　　　　　targetuse:対象パスの必要ファイル
    // 　　　　　targetuse-default:cursormark
    // 　　　　　targetuse-range:cursor=カーソル位置,cursormark=カーソル位置とマーク,all=すべて
    // 　　　　　oppositeuse:反対パスの必要ファイル
    // 　　　　　oppositeuse-default:none
    // 　　　　　oppositeuse-range:none=不要,cursor=カーソル位置,cursormark=カーソル位置とマーク,all=すべて
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class LocalExecuteCommand : FileListActionCommand {
        // 起動するプログラム名
        private string m_program;

        // コマンド引数
        private string m_argument;

        // 対象パスの必要ファイル
        private LocalExecuteUseFile m_targetUseFile;

        // 反対パスの必要ファイル
        private LocalExecuteUseFile m_oppositeUseFile;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LocalExecuteCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_program = (string)param[0];
            m_argument = (string)param[1];
            string targetUse = (string)param[2];
            string oppositeUse = (string)param[3];
            m_targetUseFile = LocalExecuteUseFile.GetFromString(targetUse, false);
            m_oppositeUseFile = LocalExecuteUseFile.GetFromString(oppositeUse, true);
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return null;
            }

            // 仮想フォルダの場合は実行準備
            bool success = PrepareVirtualFolderCommand.ReadyForVirtualFolder(FileListViewTarget);
            if (!success) {
                return false;
            }

            // 表示名を作成
            string programFile = GenericFileStringUtils.GetFileName(StringUtils.RemoveStringQuote(m_program.Trim()));
            string dispName = string.Format(Resources.LocalExec_DisplayNameExecute, programFile);
            TemporarySpaceDisplayName nameInfo = new TemporarySpaceDisplayName(dispName, IconImageListID.FileList_ShellExecute);

            // コマンドラインを作成
            ShellCommandArgument commandArgument = new ShellCommandArgument(m_argument, FileListViewTarget, FileListViewOpposite, m_targetUseFile, m_oppositeUseFile);
            success = commandArgument.ParseArgument();
            if (!success) {
                InfoBox.Warning(Program.MainWindow, commandArgument.ErrorMessage);
                return null;
            }
            if (FileSystemID.IsVirtual(FileListViewOpposite.FileList.FileSystem.FileSystemId) && commandArgument.HasOpposite) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CanNotExecuteFromVirtualDirOpposite);
                return null;
            }
            if (commandArgument.IsInputKey) {
                ShellArgumentInputDialog dialog = new ShellArgumentInputDialog(m_program, commandArgument);
                DialogResult result = dialog.ShowDialog(Program.MainWindow);
                if (result != DialogResult.OK) {
                    return null;
                }
                commandArgument = dialog.InputResult;
            }

            LocalExecuteBackgroundTask.GetProgramArgumentDelegate argDelegate = delegate(List<string> srcPathList, List<string> destPathList) {
                string argument = commandArgument.GetCommandArgument(srcPathList, destPathList);
                return argument;
            };

            // バックグラウンドタスクで編集を実行
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = commandArgument.FileProviderSrc;
            srcProvider.FileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // virtualInfo:ライフサイクル管理
            IFileProviderDest destProvider = commandArgument.FileProviderDest;
            string targetDir = FileListViewTarget.FileList.DisplayDirectoryName;
            LocalExecuteBackgroundTask task = new LocalExecuteBackgroundTask(srcProvider, destProvider, uiTarget, m_program, argDelegate, nameInfo, targetDir, null);
            Program.Document.BackgroundTaskManager.StartFileTask(task, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.LocalExecuteCommand;
            }
        }
    }
}
