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
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.Internal {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 内部処理用に指定されたファイルをのショートカットを作成します。
    //   書式 　 CreateShortcutRetryInternal()
    //   引数  　なし
    //   戻り値　bool:ショートカットの作成をバックグラウンドで開始したときtrue、作成を開始できなかったときfalseを返します。
    //=========================================================================================
    class CreateShortcutRetryInternalCommand : FileListActionCommand {
        // 再試行情報
        private FileErrorRetryInfo m_retryInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CreateShortcutRetryInternalCommand() {
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
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(m_retryInfo.SrcFileSystem);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(m_retryInfo.DestFileSystem);
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }

            // ショートカットの種別を入力
            ShortcutType targetType;
            ShortcutType[] shortcutType = srcFileSystem.SupportedShortcutType;
            if (shortcutType.Length >= 2) {
                ShortcutType type = Configuration.Current.SSHShortcutTypeDefault;
                if (type == null) {
                    type = Program.Document.UserGeneralSetting.SSHShortcutType;
                }
                CreateShortcutDialog dialog = new CreateShortcutDialog(type);
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) {
                    return false;
                }
                targetType = dialog.ShortcutType;
                Program.Document.UserGeneralSetting.SSHShortcutType = targetType;
            } else {
                targetType = shortcutType[0];
            }

            // 処理開始条件をチェック
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(CreateShortcutBackgroundTask));
            if (!canStart) {
                return false;
            }

            // バックグラウンドタスクを開始
            RefreshUITarget uiTarget = new RefreshUITarget(null, null, RefreshUITarget.RefreshMode.NoRefresh, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(m_retryInfo.RetryFileList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestSimple(m_retryInfo.DestDiretoryRoot, destFileSystem, null);
            CreateShortcutBackgroundTask linkTask = new CreateShortcutBackgroundTask(srcProvider, destProvider, uiTarget, targetType, m_retryInfo.RetryApiList);
            Program.Document.BackgroundTaskManager.StartFileTask(linkTask, true);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CreateShortcutCommand;
            }
        }
    }
}
