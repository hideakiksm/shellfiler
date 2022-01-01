using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ソート方法の選択ダイアログを表示して、対象パスのソート方法を入力します。
    //   書式 　 SortMenu()
    //   引数  　なし
    //   戻り値　bool:ソート方法を変更したときtrue、キャンセルしたときはfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class SortMenuCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SortMenuCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return FileListOptionMenu(FileListOptionDialog.OptionPage.Sort, FileListViewTarget);
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：[in]initPage        はじめに表示するページ
        // 　　　　[in]fileListTarget  対象ウィンドウ
        // 戻り値：入力を行ったときtrue、キャンセルしたときfalse
        //=========================================================================================
        public static bool FileListOptionMenu(FileListOptionDialog.OptionPage initPage, FileListView fileListTarget) {
            // コンフィグが外部で更新されているかチェック
            bool success = OptionSettingCommand.CheckConfigurationUpdate(Program.MainWindow);
            if (!success) {
                return false;
            }
            Configuration prevConfig = (Configuration)(Configuration.Current.Clone());

            // 設定値を取得
            FileListSortMode sortMode;
            bool isLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            if (isLeft) {
                sortMode = Program.Document.CurrentTabPage.LeftFileList.SortMode;
            } else {
                sortMode = Program.Document.CurrentTabPage.RightFileList.SortMode;
            }
            FileListFilterMode filterMode = fileListTarget.FileList.FileListFilterMode;
            FileListViewMode viewMode = fileListTarget.FileList.FileListViewMode;
            FileSystemID fileSystem = fileListTarget.FileList.FileSystem.FileSystemId;

            // ダイアログで入力
            FileListOptionDialog dialog = new FileListOptionDialog();
            dialog.Initialize(initPage, sortMode, filterMode, viewMode, fileSystem);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }

            // UIを更新
            if (dialog.ActivePage == FileListOptionDialog.OptionPage.Sort) {
                if (isLeft) {
                    Program.Document.CurrentTabPage.LeftFileList.SortMode.SetSortMode(dialog.ResultSortMode);
                } else {
                    Program.Document.CurrentTabPage.RightFileList.SortMode.SetSortMode(dialog.ResultSortMode);
                }
                SortMenuCommand.RefreshUI(isLeft);
            } else if (dialog.ActivePage == FileListOptionDialog.OptionPage.Filter) {
                RefreshUITarget.ReloadDirectory(fileListTarget, true, dialog.ResultFilter);
            } else if (dialog.ActivePage == FileListOptionDialog.OptionPage.Color) {
                FileListOptionDialog.FileListColorSetting.SetConfiguration(dialog.ResultColorSetting, Configuration.Current);
                Program.MainWindow.LeftFileListView.Invalidate();
                Program.MainWindow.RightFileListView.Invalidate();
                if (!Configuration.EqualsConfig(prevConfig, Configuration.Current)) {
                    // コンフィグが更新されていれば保存
                    Configuration.Current.SaveSetting();
                }
            } else if (dialog.ActivePage == FileListOptionDialog.OptionPage.ViewMode) {
                if (isLeft) {
                    Program.MainWindow.LeftFileListView.RefreshViewMode(dialog.ResultViewMode);
                } else {
                    Program.MainWindow.RightFileListView.RefreshViewMode(dialog.ResultViewMode);
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：UIを更新する
        // 引　数：[in]isLeft  左ウィンドウを更新するときtrue
        // 戻り値：なし
        //=========================================================================================
        public static void RefreshUI(bool isLeft) {
            // 対象を決定
            FileListView view;
            if (isLeft) {
                view = Program.MainWindow.LeftFileListView;
            } else {
                view = Program.MainWindow.RightFileListView;
            }

            // カーソル位置を保存
            string cursorFile = null;
            int index = view.FileListViewComponent.CursorLineNo;
            if (index < view.FileList.Files.Count) {
                cursorFile = view.FileList.Files[index].FileName;
            }

            // ファイル一覧を更新
            view.FileList.SortFileList();
            view.FileListViewComponent.OnRefreshDirectory(new ChangeDirectoryParam.UiOnly(cursorFile));
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SortMenuCommand;
            }
        }
    }
}
