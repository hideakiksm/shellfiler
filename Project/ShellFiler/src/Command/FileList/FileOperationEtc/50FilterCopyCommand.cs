using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルをテキストフィルターで変換してから反対パスにコピーします。
    //   書式 　 FilterCopy()
    //   引数  　なし
    //   戻り値　bool:コピーをバックグラウンドで開始したときtrue、起動をキャンセルしたときやマークがないときfalseを返します。
    //   対応Ver 1.0.0
    //=========================================================================================
    class FilterCopyCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FilterCopyCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：起動に成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // マークを確認
            bool markOk = BackgroundTaskCommandUtil.CheckMarkState(FileListViewTarget, Configuration.Current.MarklessCopy);
            if (!markOk) {
                return false;
            }

            // 処理開始条件をチェック
            bool canStart = BackgroundTaskCommandUtil.CheckTaskStart(typeof(CopyBackgroundTask));
            if (!canStart) {
                return false;
            }
            canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, FileListViewOpposite.FileList.FileSystem.FileSystemId, false);
            if (!canStart) {
                return false;
            }

            // フィルターを入力
            RefreshUITarget uiTarget = new RefreshUITarget(FileListViewTarget, FileListViewOpposite, RefreshUITarget.RefreshMode.RefreshOpposite, RefreshUITarget.RefreshOption.None);
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(FileListViewTarget.FileList.FileSystem.FileSystemId);
            IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(FileListViewOpposite.FileList.FileSystem.FileSystemId);
            IFileProviderSrc srcProvider = new FileProviderSrcMarked(FileListViewTarget.FileList, FileListViewTarget.FileList.MarkFiles, srcFileSystem, null);      // 仮想フォルダ情報は後で設定
            IFileProviderDest destProvider = new FileProviderDestSimple(FileListViewOpposite.FileList.DisplayDirectoryName, destFileSystem, null);

            FileFilterSetting setting = Program.Document.UserGeneralSetting.FileFilterSetting;
            setting.LoadData();
            FileFilterMode filterMode = setting.TransferLastFilterMode;
            FileFilterTransferSetting detailSetting = (FileFilterTransferSetting)(setting.TransferDetailSetting.Clone());
            FileFilterTransferQuickSetting quickSetting = (FileFilterTransferQuickSetting)(setting.TransferQuickSetting.Clone());
            FileFilterTransferDefinedSetting definedSetting = (FileFilterTransferDefinedSetting)(setting.TransferDefinedSetting.Clone());
            FileFilterTransferDialog dialog = new FileFilterTransferDialog(filterMode, detailSetting, quickSetting, definedSetting, setting.ClipboardSetting.QuickSetting, srcProvider, destProvider);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }
            FileFilterTransferSetting transferSetting;          // 設定のClone
            setting.TransferLastFilterMode = dialog.FilterMode;
            if (dialog.FilterMode == FileFilterMode.DetailMode) {
                setting.TransferDetailSetting = dialog.DetailSetting;
                transferSetting = (FileFilterTransferSetting)(dialog.DetailSetting.Clone());
            } else if (dialog.FilterMode == FileFilterMode.QuickMode) {
                setting.TransferQuickSetting = dialog.QuickSetting;
                transferSetting = dialog.QuickSetting.CreateTransferSetting();
            } else {
                setting.TransferDefinedSetting = dialog.DefinedSetting;
                transferSetting = dialog.DefinedSetting.CreateTransferSetting(setting.ClipboardSetting.QuickSetting);
            }
            setting.SaveData();

            // タスクを登録
            srcProvider.FileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // FileListContext:ライフサイクル管理
            destProvider.FileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewOpposite);            // FileListContext:ライフサイクル管理
            AttributeSetMode attributeSetMode = (AttributeSetMode)(Configuration.Current.TransferAttributeSetMode.Clone());
            CopyMoveDeleteOption option = new CopyMoveDeleteOption(CopyMoveDeleteOption.CreateDefaultSameFileOperation(destFileSystem.FileSystemId), transferSetting, null, attributeSetMode, null, null, false);
            CopyBackgroundTask copyTask = new CopyBackgroundTask(srcProvider, destProvider, uiTarget, option, null);
            Program.Document.BackgroundTaskManager.StartFileTask(copyTask, true);
            FileListComponentTarget.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FilterCopyCommand;
            }
        }
    }
}
