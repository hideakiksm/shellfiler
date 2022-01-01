using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイル一覧の右と左でファイル一覧を比較して、指定条件でマークします。
    //   書式 　 FileCompare()
    //   引数  　なし
    //   戻り値　bool:ファイル比較を行ったときはtrue、ダイアログをキャンセルしたときはfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class FileCompareCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileCompareCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 左右のフォルダを比較
            if (FileListViewTarget.FileList.DisplayDirectoryName == FileListViewOpposite.FileList.DisplayDirectoryName) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_FileCompareSameFolder);
                return false;
            }

            // ソート方法をダイアログで入力
            FileCompareSetting setting = InputCompareMethod();
            if (setting == null) {
                return false;
            }

            // 全マークをクリア
            FileListViewTarget.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);

            // 比較モード
            bool ignoreCase = true;
            if (FileSystemID.IsSSH(FileListViewTarget.FileList.FileSystem.FileSystemId) || FileSystemID.IsSSH(FileListViewOpposite.FileList.FileSystem.FileSystemId)) {
                ignoreCase = false;
            }

            // 反対パスの情報を整理
            Dictionary<string, UIFile> oppositeList = new Dictionary<string,UIFile>();
            foreach (UIFile oppositeFile in FileListViewOpposite.FileList.Files) {
                string oppositeFileName = oppositeFile.FileName;
                if (oppositeFileName == "..") {
                    continue;
                }
                if (setting.ExceptFolder && oppositeFile.Attribute.IsDirectory) {
                    continue;
                }
                if (ignoreCase) {
                    oppositeFileName = oppositeFileName.ToLower();
                }
                oppositeList.Add(oppositeFileName, oppositeFile);
            }

            // 対象パスで検索
            FileSystemID targetFileSystem = FileListViewTarget.FileList.FileSystem.FileSystemId;
            FileSystemID oppositeFileSystem = FileListViewOpposite.FileList.FileSystem.FileSystemId;
            int markCount = 0;
            for (int i = 0; i < FileListViewTarget.FileList.Files.Count; i++) {
                UIFile targetFile = FileListViewTarget.FileList.Files[i];
                string targetFileName = targetFile.FileName;
                if (targetFileName == "..") {
                    continue;
                }
                if (setting.ExceptFolder && targetFile.Attribute.IsDirectory) {
                    continue;
                }
                if (ignoreCase) {
                    targetFileName = targetFileName.ToLower();
                }
                if (!oppositeList.ContainsKey(targetFileName)) {
                    continue;
                }
                UIFile oppositeFile = oppositeList[targetFileName];
                if (targetFile.Attribute.IsDirectory != oppositeFile.Attribute.IsDirectory) {
                    continue;
                }
                if (!IsTargetFileTime(targetFileSystem, oppositeFileSystem, targetFile.ModifiedDate, oppositeFile.ModifiedDate, setting.FileTimeMode)) {
                    continue;
                }
                if (!targetFile.Attribute.IsDirectory) {
                    if (!IsTargetFileSize(targetFile.FileSize, oppositeFile.FileSize, setting.FileSizeMode)) {
                        continue;
                    }
                }
                markCount++;
                FileListViewTarget.FileList.SetMarked(i, true);
            }

            if (markCount == 0) {
                InfoBox.Information(Program.MainWindow, Resources.Msg_FileCompareNoFile);
            }

            FileListViewTarget.Invalidate();
            FileListViewTarget.ParentPanel.StatusBar.RefreshMarkInfo();
            Program.MainWindow.RefreshUIStatus();

            return true;
        }

        //=========================================================================================
        // 機　能：比較方法を入力する
        // 引　数：なし
        // 戻り値：比較方法（null:キャンセル）
        //=========================================================================================
        private FileCompareSetting InputCompareMethod() {
            FileCompareSetting setting;
            if (Configuration.Current.FileCompareSettingDefault == null) {
                setting = (FileCompareSetting)(Program.Document.UserGeneralSetting.FileCompareSetting.Clone());
            } else {
                setting = (FileCompareSetting)(Configuration.Current.FileCompareSettingDefault.Clone());
            }
            FileCompareDialog dialog = new FileCompareDialog(setting, FileListViewTarget, FileListViewOpposite);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }
            Program.Document.UserGeneralSetting.FileCompareSetting = (FileCompareSetting)(dialog.FileCompareSetting.Clone());
            return dialog.FileCompareSetting;
        }

        //=========================================================================================
        // 機　能：ファイルの更新時刻を比較する
        // 引　数：[in]targetSys    対象パスのファイルシステム
        // 　　　　[in]oppositeSys  反対パスのファイルシステム
        // 　　　　[in]target       対象パスのファイルの更新時刻
        // 　　　　[in]opposite     反対パスのファイルの更新時刻
        // 　　　　[in]mode         比較モード
        // 戻り値：同じと見なしてよい状態のときtrue
        //=========================================================================================
        private bool IsTargetFileTime(FileSystemID targetSys, FileSystemID oppoisteSys, DateTime target, DateTime opposite, FileCompareSetting.FileTimeCompareMode mode) {
            int compDate = BackgroundTaskCommandUtil.CompareFileDate(targetSys, oppoisteSys, target, opposite);
            switch (mode) {
                case FileCompareSetting.FileTimeCompareMode.Ignore:
                    return true;
                case FileCompareSetting.FileTimeCompareMode.MarkExactly:
                    return (compDate == 0);
                case FileCompareSetting.FileTimeCompareMode.MarkNewer:
                    return (compDate > 0);
                case FileCompareSetting.FileTimeCompareMode.MarkOlder:
                    return (compDate < 0);
                default:
                    return false;
            }
        }

        //=========================================================================================
        // 機　能：ファイルのサイズを比較する
        // 引　数：[in]target   対象パスのファイルのサイズ
        // 　　　　[in]opposite 反対パスのファイルのサイズ
        // 　　　　[in]mode     比較モード
        // 戻り値：同じと見なしてよい状態のときtrue
        //=========================================================================================
        private bool IsTargetFileSize(long target, long opposite, FileCompareSetting.FileSizeCompareMode mode) {
            switch (mode) {
                case FileCompareSetting.FileSizeCompareMode.Ignore:
                    return true;
                case FileCompareSetting.FileSizeCompareMode.MarkExactly:
                    return (target == opposite);
                case FileCompareSetting.FileSizeCompareMode.MarkBigger:
                    return (target > opposite);
                case FileCompareSetting.FileSizeCompareMode.MarkSmaller:
                    return (target < opposite);
                default:
                    return false;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileCompareCommand;
            }
        }
    }
}
