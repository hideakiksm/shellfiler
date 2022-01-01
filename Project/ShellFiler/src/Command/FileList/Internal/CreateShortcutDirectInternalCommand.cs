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
    //   書式 　 CreateShortcutDirectInternalCommand(List<SimpleFileDirectoryPath> fileList, FileSystemID srcFileSystem)
    //   引数  　fileList:対象ファイルのリスト
    // 　　　　　srcFileSystem:転送元のファイルシステム
    //   戻り値　bool:ショートカットの作成をバックグラウンドで開始したときtrue、作成を開始できなかったときfalseを返します。
    //=========================================================================================
    class CreateShortcutDirectInternalCommand : FileListActionCommand {
        // 対象ファイルのリスト
        private List<SimpleFileDirectoryPath> m_fileList;

        // 対象ファイルのファイルシステム
        private FileSystemID m_srcFileSystem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CreateShortcutDirectInternalCommand() {
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
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(m_srcFileSystem);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, srcFileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }
            string srcPath = m_fileList[0].FilePath;
            string destPath = FileListViewOpposite.FileList.DisplayDirectoryName;
            if (srcFileSystem.FileSystemId != destFileSystem.FileSystemId || !srcFileSystem.IsSameServerSpace(srcPath, destPath)) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CreateShortcutDifferentSystem);
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
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            IFileProviderSrc srcProvider = new FileProviderSrcSpecified(m_fileList, srcFileSystem, null);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, null);
            CreateShortcutBackgroundTask linkTask = new CreateShortcutBackgroundTask(srcProvider, destProvider, uiTarget, targetType, null);
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
