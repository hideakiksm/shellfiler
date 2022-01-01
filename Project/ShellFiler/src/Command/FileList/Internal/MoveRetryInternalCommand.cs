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
    // 内部処理用に指定された再試行情報を使って移動します。
    //   書式 　 MoveRetryInternal(FileErrorRetryInfo retryInfo)
    //   引数  　retryInfo:再試行情報
    //   戻り値　bool:移動をバックグラウンドで開始したときtrue、移動を開始できなかったときfalseを返します。
    //=========================================================================================
    class MoveRetryInternalCommand : FileListActionCommand {
        // 再試行情報
        private FileErrorRetryInfo m_retryInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MoveRetryInternalCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_retryInfo = (FileErrorRetryInfo)param[0];
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
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, m_retryInfo.SrcFileSystem, true);
            if (!canStart) {
                return false;
            }

            // タスクを登録
            RefreshUITarget uiTarget = new RefreshUITarget(null, null, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(m_retryInfo.SrcFileSystem);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(m_retryInfo.DestFileSystem);
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(m_retryInfo.RetryFileList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestSimple(m_retryInfo.DestDiretoryRoot, destFileSystem, null);
            MoveBackgroundTask moveTask = new MoveBackgroundTask(srcProvider, destProvider, uiTarget, m_retryInfo.Option, m_retryInfo.RetryApiList);
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
