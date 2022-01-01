using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime;
using ShellFiler.Api;
using ShellFiler.Command.FileList.FileOperationEtc;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.FileViewer;

namespace ShellFiler.Command.FileViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 選択範囲の比較の対象となる文字列を確認します。
    //   書式 　 V_SelectionCompareDisplay()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class V_SelectionCompareDisplayCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SelectionCompareDisplayCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            ViewerCompareDisplayDialog dialog = new ViewerCompareDisplayDialog();
            dialog.ShowDialog(this.TextFileViewer.FileViewerForm);
            if (dialog.RunDiffTool) {
                V_SelectionCompareDisplayCommand.FileCompare fileCompare = new V_SelectionCompareDisplayCommand.FileCompare(this.TextFileViewer);
                fileCompare.Execute();
                fileCompare.Dispose();
            }

            return null;
        }

        //=========================================================================================
        // 機　能：テキストビューアの選択中のテキストを返す
        // 引　数：[in]textFileViewer  対象のファイルビューア
        // 戻り値：選択対象のテキスト（取得できないときnull）
        //=========================================================================================
        public static string GetSelectionText(TextFileViewer textFileViewer) {
            if (!(textFileViewer.TextViewerComponent is TextViewerComponent)) {
                InfoBox.Information(textFileViewer.FileViewerForm, Resources.Msg_ViewerSelectionCompareTextMode);
                return null;
            }
            TextViewerComponent textModeViewer = (TextViewerComponent)(textFileViewer.TextViewerComponent);
            TextViewerSelectionRange select = textModeViewer.SelectionRange;
            if (select == null) {
                InfoBox.Information(textFileViewer.FileViewerForm, Resources.Msg_ViewerSelectionCompareNoSelect);
                return null;
            }
            string text = textModeViewer.GetTextClipboardOriginalTab(TextClipboardSetting.CopyLineBreakMode.Original);
            return text;
        }

        //=========================================================================================
        // クラス：ファイルの比較処理の実装
        //=========================================================================================
        public class FileCompare {
            // 対象のファイルビューア
            private TextFileViewer m_textFileViewer;

            // 左側のテキストを保存したファイルのファイル名（未保存のときnull）
            private string m_leftTempFileName = null;

            // 右側のテキストを保存したファイルのファイル名（未保存のときnull）
            private string m_rightTempFileName = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]textFileViewer  対象のファイルビューア
            // 戻り値：なし
            //=========================================================================================
            public FileCompare(TextFileViewer textFileViewer) {
                m_textFileViewer = textFileViewer;
            }

            //=========================================================================================
            // 機　能：一時ファイルを破棄する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                if (m_leftTempFileName != null) {
                    try {
                        File.Delete(m_leftTempFileName);
                    } catch (Exception) {
                    }
                    m_leftTempFileName = null;
                }

                if (m_rightTempFileName != null) {
                    try {
                        File.Delete(m_rightTempFileName);
                    } catch (Exception) {
                    }
                    m_rightTempFileName = null;
                }
            }

            //=========================================================================================
            // 機　能：比較処理を実行する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Execute() {
                FileViewerSelectionCompareBuffer buffer = Program.Document.FileViewerSelectionCompareBuffer;
                string leftFileName = Program.Document.TemporaryManager.GetTemporaryFile();
                string rightFileName = Program.Document.TemporaryManager.GetTemporaryFile();
                bool success;

                // 左右をテンポラリに作成
                success = CreateTargetFile(leftFileName, buffer.LeftString, buffer.LeftStartLineNum);
                if (!success) {
                    return;
                }
                m_leftTempFileName = leftFileName;
                success = CreateTargetFile(rightFileName, buffer.RightString, buffer.RightStartLineNum);
                if (!success) {
                    return;
                }
                m_rightTempFileName = rightFileName;

                // 差分表示ツールを起動
                string command = DiffMarkCommand.GetDiffToolCommand(false);
                if (command == null) {
                    DiffToolDownloadDialog dialogError = new DiffToolDownloadDialog();
                    dialogError.ShowDialog(m_textFileViewer.FileViewerForm);
                    return;
                }
                string commandFull = string.Format(command, "\"" + m_leftTempFileName + "\" \"" + m_rightTempFileName + "\"");
                string program;
                string argument;
                GenericFileStringUtils.SplitCommandLine(commandFull, out program, out argument);    
                string programFile = GenericFileStringUtils.GetFileName(program);
                string dirName = GenericFileStringUtils.GetDirectoryName(program);

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = program;
                psi.WorkingDirectory = dirName;
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = false;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.Arguments = argument;

                // プロセスを起動
                try {
                    Process process = OSUtils.StartProcess(psi, dirName);
                    Program.LogWindow.RegistLogLineHelper(string.Format(Resources.ShellExec_ProcessStart2, programFile));
                    process.Dispose();
                } catch (Exception e) {
                    InfoBox.Warning(m_textFileViewer.FileViewerForm, Resources.Msg_ViewerSelectionCompareFailDiffTool, e.ToString());
                    return;
                }

                // 終了
                bool deleteBuffer;
                if (Configuration.Current.TextViewerClearCompareBufferDefault == null) {
                    deleteBuffer = Program.Document.UserGeneralSetting.TextViewerClearCompareBuffer;
                } else {
                    deleteBuffer = Configuration.Current.TextViewerClearCompareBufferDefault.Value;
                }
                ViewerCompareSuccessDialog dialog = new ViewerCompareSuccessDialog(deleteBuffer);
                dialog.ShowDialog(m_textFileViewer.FileViewerForm);
                deleteBuffer = dialog.DeleteBuffer;
                Program.Document.UserGeneralSetting.TextViewerClearCompareBuffer = deleteBuffer;

                if (deleteBuffer) {
                    buffer.LeftString = null;
                    buffer.LeftStartLineNum = -1;
                    buffer.RightString = null;
                    buffer.RightStartLineNum = -1;
                }
            }

            //=========================================================================================
            // 機　能：一時ファイルに対象ファイルを作成する
            // 引　数：[in]fileName   ファイル名
            // 　　　　[in]buffer     比較対象の文字列（改行込みで選択範囲の全文字数分）
            // 　　　　[in]startLine  選択範囲の開始行番号
            // 戻り値：なし
            //=========================================================================================
            private bool CreateTargetFile(string fileName, string buffer, int startLine) {
                try {
                    StringBuilder sbCrLf = new StringBuilder();
                    for (int i = 0; i < startLine - 1; i++) {
                        sbCrLf.Append("\r\n");
                    }

                    StreamWriter writer= new StreamWriter(fileName, false, Encoding.UTF8);
                    try {
                        writer.Write(sbCrLf.ToString());
                        writer.Write(buffer);
                    } finally {
                        writer.Close();
                    }
                    return true;
                } catch (Exception) {
                    try {
                        File.Delete(fileName);
                    } catch (Exception) {
                    }
                    return false;
                }
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SelectionCompareDisplayCommand;
            }
        }
    }
}