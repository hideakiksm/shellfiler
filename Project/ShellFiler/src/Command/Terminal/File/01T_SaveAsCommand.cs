using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Terminal;

namespace ShellFiler.Command.Terminal.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ターミナルの表示内容をファイルに保存します。
    //   書式 　 T_SaveAs()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.0.0
    //=========================================================================================
    class T_SaveAsCommand : TerminalActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public T_SaveAsCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            DialogResult result;
            if (TerminalPanel.IsMouseDrag) {
                return null;
            }

            // 保存範囲を確認
            bool all = true;
            if (TerminalPanel.SelectionRange != null) {
                TerminalSaveTypeDialog dialog = new TerminalSaveTypeDialog();
                result = dialog.ShowDialog(TerminalPanel.DialogParentForm);
                if (result != DialogResult.OK) {
                    return null;
                }
                all = dialog.SaveAll;
            }

            // テキストを取得
            string selectedText;
            bool createAll;
            TerminalPanel.GetSelectedText(all, "\n", out selectedText, out createAll);
            if (!createAll) {
                result = InfoBox.Question(TerminalPanel.DialogParentForm, MessageBoxButtons.YesNo, Resources.Msg_TerminalSavePart);
                if (result != DialogResult.Yes) {
                    return null;
                }
            }

            // ファイル名を入力
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "log" + DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now);
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sfd.Filter = Resources.Terminal_SaveAsFilter;
            sfd.FilterIndex = 3;
            sfd.RestoreDirectory = true;
            sfd.CheckFileExists = false;
            result = sfd.ShowDialog(TerminalPanel.DialogParentForm);
            if (result != DialogResult.OK) {
                return null;
            }
            string fileName = sfd.FileName;

            // 保存を実行
            try {
                byte[] data = TerminalPanel.ConsoleScreen.Encoding.GetBytes(selectedText);
                System.IO.File.WriteAllBytes(fileName, data);
            } catch (Exception e) {
                InfoBox.Warning(TerminalPanel.DialogParentForm, Resources.Terminal_SaveFailed, e.Message);
            }
           
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.T_SaveAsCommand;
            }
        }
    }
}
