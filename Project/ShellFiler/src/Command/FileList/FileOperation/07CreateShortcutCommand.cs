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

namespace ShellFiler.Command.FileList.FileOperation {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルまたはフォルダのショートカットを反対パスに作成します。
    //   書式 　 CreateShortcut()
    //   引数  　なし
    //   戻り値　bool:ショートカットの作成をバックグラウンドで開始したときtrue、作成を開始できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class CreateShortcutCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CreateShortcutCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessShortcut);
            if (!markOk) {
                return false;
            }
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewTarget.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewOpposite.FileList.FileSystem.FileSystemId, false);
            if (!canStart) {
                return false;
            }

            // 処理開始条件をチェック
            bool sameSystem = BackgroundTaskCommandUtil.CheckSameFileSystem(FileListViewTarget, FileListViewOpposite);
            if (!sameSystem) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CreateShortcutDifferentSystem);
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(CreateShortcutBackgroundTask));
            if (!canStart) {
                return false;
            }

            // ショートカットの種別を入力
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);

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

            // バックグラウンドタスクを開始
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            IFileListContext srcFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // fileListContext:ライフサイクル管理
            IFileListContext destFileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // fileListContext:ライフサイクル管理
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, srcFileListContext);
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, destFileListContext);
            CreateShortcutBackgroundTask linkTask = new CreateShortcutBackgroundTask(srcProvider, destProvider, uiTarget, targetType, null);
            Program.Document.BackgroundTaskManager.StartFileTask(linkTask, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
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
