using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：履歴削除の処理をまとめたクラス
    //=========================================================================================
    public class DeleteHistoryProcedure {

        //=========================================================================================
        // 機　能：履歴削除の確認メッセージを表示する
        // 引　数：[in]parentForm  親となるフォーム
        // 　　　　[in]message     元のメッセージ
        // 戻り値：なし
        //=========================================================================================
        public static DialogResult ConfirmDeleteHistory(Form parentForm, string message) {
            // 起動中のShellFilerのプロセスを調べる
            bool existOther = false;
            List<Process> allProcess = OSUtils.GetAllShellFilerProcess();
            Process current = Process.GetCurrentProcess();
            foreach (Process process in allProcess) {
                if (current.Id != process.Id) {
                    existOther = true;
                }
            }

            // 起動している場合はメッセージを変更
            if (existOther) {
                message += "\r\n" + Resources.Option_PrivacyOtherProcess;
            }

            // 確認
            DialogResult result = InfoBox.Question(parentForm, MessageBoxButtons.YesNo, message);
            return result;
        }

        //=========================================================================================
        // 機　能：フォルダ履歴を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void DeleteFolderHistory() {
            // 一覧ごとの履歴
            foreach (TabPageInfo tabPage in Program.Document.TabPageList.AllList) {
                UIFileList left = tabPage.LeftFileList;
                left.PathHistory.ClearAllHistory();
                left.PathHistory.AddItem(left.DisplayDirectoryName, "", left.FileSystem.FileSystemId);
                UIFileList right = tabPage.RightFileList;
                right.PathHistory.ClearAllHistory();
                right.PathHistory.AddItem(right.DisplayDirectoryName, "", right.FileSystem.FileSystemId);
            }
            Program.MainWindow.RefreshUIStatus();

            // 全体の履歴
            Program.Document.FolderHistoryWhole.ClearAllHistory();
            Program.Document.FolderHistoryWhole.DeleteSetting();
        }

        //=========================================================================================
        // 機　能：ファイルビューアの履歴を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void DeleteViewerHistory() {
            Program.Document.UserSetting.ViewerSearchHistory.ClearTextSearchWord();
            Program.Document.UserSetting.ViewerSearchHistory.ClearDumpSearchWord();
            Program.Document.UserSetting.ViewerSearchHistory.DeleteSetting();
        }

        //=========================================================================================
        // 機　能：コマンド履歴を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void DeleteCommandHistory() {
            Program.Document.UserSetting.CommandHistory.ClearAllHistory();
            Program.Document.UserSetting.CommandHistory.DeleteSetting();
        }
    }
}
