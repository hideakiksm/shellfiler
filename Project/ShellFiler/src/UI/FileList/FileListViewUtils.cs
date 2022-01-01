using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.UI.FileList.DefaultList;
using ShellFiler.UI.FileList.ThumbList;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：ファイル一覧ウィンドウでの共通処理
    //=========================================================================================
    public class FileListViewUtils {
        
        //=========================================================================================
        // 機　能：カーソルを左に移動する
        // 引　数：[in]fileListView  ファイル一覧のビュー
        // 戻り値：なし
        //=========================================================================================
        public static void CursorLeft(FileListView fileListView) {
            bool isLeft = fileListView.IsLeft;
            if (!isLeft) {
                fileListView.MainWindow.ToggleCursorLeftRight();
            } else {
                // すでに移動している場合
                if (Configuration.Current.ChdirParentOtherSideMove) {
                    ChdirParentCommand.ChangeDirectoryParent(fileListView);
                }
            }
        }

        //=========================================================================================
        // 機　能：カーソルを右に移動する
        // 引　数：[in]fileListView  ファイル一覧のビュー
        // 戻り値：なし
        //=========================================================================================
        public static void CursorRight(FileListView fileListView) {
            bool isLeft = fileListView.IsLeft;
            if (isLeft) {
                fileListView.MainWindow.ToggleCursorLeftRight();
            } else {
                // すでに移動している場合
                if (Configuration.Current.ChdirParentOtherSideMove) {
                    ChdirParentCommand.ChangeDirectoryParent(fileListView);
                }
            }
        }

        //=========================================================================================
        // 機　能：すべてのオブジェクトのマーク状態を変更する
        // 引　数：[in]markMode   マークの方法
        // 　　　　[in]updateUI   UIの更新も行うときtrue
        // 　　　　[in]condition  マークする条件（条件がないときnull） 
        // 戻り値：マーク状態を変更したオブジェクトの数
        //=========================================================================================
        public static int MarkAllFile(FileListView fileListView, MarkAllFileMode markMode, bool updateUI, CompareCondition condition) {
            int count = 0;
            MarkOperation markOperation = markMode.Operation;
            UIFileList fileList = fileListView.FileList;
            for (int i = 0; i < fileList.Files.Count; i++) {
                UIFile file = fileList.Files[i];

                // 条件に一致するファイルか確認
                if (condition != null) {
                    bool match = file.IsTargetFileWithCondition(condition, true);
                    if (!match) {
                        continue;
                    }
                }

                // マークを実行
                bool doMark = false;
                if (file.Attribute.IsDirectory && markMode.TargetDirectory) {
                    doMark = true;
                } else if (!file.Attribute.IsDirectory && markMode.TargetFile) {
                    doMark = true;
                }
                if (doMark) {
                    if (markOperation == MarkOperation.Mark) {
                        fileList.SetMarked(i, true);
                    } else if (markOperation == MarkOperation.Clear) {
                        fileList.SetMarked(i, false);
                    } else if (markOperation == MarkOperation.Revert) {
                        fileList.SetMarked(i, !file.Marked);
                    }
                    count++;
                }
            }

            // ファイルが隠されているとき警告
            if (fileListView.FileList.ExistFilteringSkippedFile &&
                    (markMode == MarkAllFileMode.SelectAll ||
                     markMode == MarkAllFileMode.RevertAll ||
                     markMode == MarkAllFileMode.SelectAllFile ||
                     markMode == MarkAllFileMode.RevertAllFile)) {
                fileListView.ParentPanel.ShowErrorMessage(Resources.StatusBarFileFilterHideFile, FileOperationStatus.LogLevel.Info, IconImageListID.FileList_FilterMini);
            }

            // 再描画
            fileListView.Invalidate();
            if (updateUI) {
                fileListView.ParentPanel.StatusBar.RefreshMarkInfo();
                Program.MainWindow.RefreshUIStatus();
            }

            return count;
        }

        //=========================================================================================
        // 機　能：ビューのデフォルト状態を返す
        // 引　数：[in]viewMode  ビューのモード
        // 戻り値：デフォルト状態
        //=========================================================================================
        public static IFileListViewState GetDefaultFileListViewState(FileListViewMode viewMode) {
            if (viewMode.ThumbnailModeSwitch) {
                return ThumbListViewComponent.GetDefaultViewState(viewMode);
            } else {
                return DefaultFileListViewComponent.GetDefaultViewState();
            }
        }
    }
}
