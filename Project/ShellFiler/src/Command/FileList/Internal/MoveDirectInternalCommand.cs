using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.Internal {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 内部処理用に指定されたファイルをコピーします。
    //   書式 　 CopyDirectInternal(List<SimpleFileDirectoryPath> fileList, FileSystemID srcFileSystem)
    //   引数  　fileList:対象ファイルのリスト
    // 　　　　　srcFileSystem:転送元のファイルシステム
    //   戻り値　bool:移動をバックグラウンドで開始したときtrue、移動を開始できなかったときfalseを返します。
    //=========================================================================================
    class MoveDirectInternalCommand : FileListActionCommand {
        // 対象ファイルのリスト
        private List<SimpleFileDirectoryPath> m_fileList;

        // 対象ファイルのファイルシステム
        private FileSystemID m_srcFileSystem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MoveDirectInternalCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_fileList = (List<SimpleFileDirectoryPath>)param[0];
            m_srcFileSystem = (FileSystemID)param[1];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(MoveBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(m_srcFileSystem);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(m_fileList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, null);
            MoveBackgroundTask moveTask = new MoveBackgroundTask(srcProvider, destProvider, uiTarget, null, null);
            Program.Document.BackgroundTaskManager.StartFileTask(moveTask, true);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MoveCommand;
            }
        }
    }
}
