using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Command.FileList.Tools;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：インストール情報＞エディタ の設定ページ
    //=========================================================================================
    public partial class InstallEditorPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public InstallEditorPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            SetInitialValue(Configuration.Current);
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (this.radioButtonEditorAuto.Checked) {
                this.textBoxEditor.Enabled = false;
                this.buttonEditorRef.Enabled = false;
                this.textBoxEditorSSH.Enabled = false;
                this.buttonEditorRefSSH.Enabled = false;
                this.checkBoxEditorSSH.Enabled = false;
            } else {
                this.textBoxEditor.Enabled = true;
                this.buttonEditorRef.Enabled = true;
                this.checkBoxEditorSSH.Enabled = true;
                if (this.checkBoxEditorSSH.Checked) {
                    this.textBoxEditorSSH.Enabled = true;
                    this.buttonEditorRefSSH.Enabled = true;
                } else {
                    this.textBoxEditorSSH.Enabled = false;
                    this.buttonEditorRefSSH.Enabled = false;
                }
            }

            if (this.radioButtonLineNone.Checked) {
                this.textBoxEditorLineNum.Enabled = false;
                this.buttonEditorLineNumRef.Enabled = false;
                this.textBoxEditorLineNumSSH.Enabled = false;
                this.buttonEditorLineNumRefSSH.Enabled = false;
                this.checkBoxLineSSH.Enabled = false;
            } else {
                this.textBoxEditorLineNum.Enabled = true;
                this.buttonEditorLineNumRef.Enabled = true;
                this.checkBoxLineSSH.Enabled = true;
                if (this.checkBoxLineSSH.Checked) {
                    this.textBoxEditorLineNumSSH.Enabled = true;
                    this.buttonEditorLineNumRefSSH.Enabled = true;
                } else {
                    this.textBoxEditorLineNumSSH.Enabled = false;
                    this.buttonEditorLineNumRefSSH.Enabled = false;
                }
            }
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            // エディタ
            string configEditor = config.TextEditorCommandLine;
            string configEditorSSH = config.TextEditorCommandLineSSH;
            string editorDefault = LocalEditCommand.GetTextAssocCommandLine();
            if (configEditor == "") {
                this.radioButtonEditorAuto.Checked = true;
                this.textBoxEditorAuto.Text = editorDefault;
                this.textBoxEditor.Text = editorDefault;
                this.checkBoxEditorSSH.Checked = false;
                this.textBoxEditorSSH.Text = "";
            } else {
                this.radioButtonEditorFix.Checked = true;
                this.textBoxEditorAuto.Text = editorDefault;
                this.textBoxEditor.Text = configEditor;
                if (configEditorSSH == "") {
                    this.checkBoxEditorSSH.Checked = false;
                    this.textBoxEditorSSH.Text = "";
                } else {
                    this.checkBoxEditorSSH.Checked = true;
                    this.textBoxEditorSSH.Text = configEditorSSH;
                }
            }

            // ビューアでのエディタ
            string configViewerEditor = config.TextEditorCommandLineWithLineNumber;
            string configViewerEditorSSH = config.TextEditorCommandLineWithLineNumberSSH;
            if (configViewerEditor == "") {
                this.radioButtonLineNone.Checked = true;
                this.textBoxEditorLineNum.Text = this.textBoxEditor.Text.Replace("{0}", "/+{1} {0}");
                this.checkBoxLineSSH.Checked = false;
                this.textBoxEditorLineNumSSH.Text = "";
            } else {
                this.radioButtonLineSpecify.Checked = true;
                this.textBoxEditorLineNum.Text = configViewerEditor;
                if (configViewerEditorSSH == "") {
                    this.checkBoxLineSSH.Checked = false;
                    this.textBoxEditorLineNumSSH.Text = "";
                } else {
                    this.checkBoxLineSSH.Checked = true;
                    this.textBoxEditorLineNumSSH.Text = configViewerEditorSSH;
                }
            }

            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：エディタのラジオボタンが変化したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButtonEditor_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：SSH別設定のチェックボックスが変化したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxEditorSSH_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：エディタの参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonEditorRef_Click(object sender, EventArgs evt) {
            // 対象を決定
            TextBox textBox;
            bool viewerMode;
            if (sender == buttonEditorRef) {
                textBox = this.textBoxEditor;
                viewerMode = false;
            } else if (sender == buttonEditorRefSSH) {
                textBox = this.textBoxEditorSSH;
                viewerMode = false;
            } else if (sender == buttonEditorLineNumRef) {
                textBox = this.textBoxEditorLineNum;
                viewerMode = true;
            } else {
                textBox = this.textBoxEditorLineNumSSH;
                viewerMode = true;
            }

            // 入力
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Resources.OptionInstallEditorRefTitle;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            ofd.Filter = Resources.OptionInstallEditorRefFilter;
            ofd.RestoreDirectory = true;
            DialogResult dr = ofd.ShowDialog(this);
            if (dr != DialogResult.OK) {
                return;
            }

            // 反映
            string editorExe = ofd.FileName;
            string editorCommand;
            if (!viewerMode) {
                if (editorExe.IndexOf(' ') != -1) {
                    editorCommand = "\"" + editorExe + "\" {0}";
                } else {
                    editorCommand = editorExe + " {0}";
                }
            } else {
                if (editorExe.IndexOf(' ') != -1) {
                    editorCommand = "\"" + editorExe + "\" /+{1} {0}";
                } else {
                    editorCommand = editorExe + " /+{1} {0}";
                }
            }
            textBox.Text = editorCommand;
        }

        //=========================================================================================
        // 機　能：SSH用のエディタのリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelSSHHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            HelpMessageDialog dialog = new HelpMessageDialog(Resources.Option_SSHEditor, Resources.HtmlSSHEditor);
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // エディタのコマンドライン
            string editor;
            string editorSSH;
            if (this.radioButtonEditorAuto.Checked) {
                editor = "";
                editorSSH = "";
            } else {
                editor = this.textBoxEditor.Text;
                editorSSH = this.textBoxEditorSSH.Text;
                if (editor == "") {
                    InfoBox.Warning(m_parent, Resources.Option_TextEditorCommandLineNothing);
                    return false;
                }
                if (this.checkBoxEditorSSH.Checked) {
                    if (editorSSH == "") {
                        InfoBox.Warning(m_parent, Resources.Option_TextEditorCommandLineSSHNothing);
                    }
                } else {
                    editorSSH = "";
                }
                success = Configuration.CheckTextEditorCommandLine(ref editor, m_parent);
                if (!success) {
                    return false;
                }
                success = Configuration.CheckTextEditorCommandLine(ref editorSSH, m_parent);
                if (!success) {
                    return false;
                }
            }

            // ビューアでのエディタのコマンドライン
            string viewerEditor;
            string viewerEditorSSH;
            if (this.radioButtonLineNone.Checked) {
                viewerEditor = "";
                viewerEditorSSH = "";
            } else {
                viewerEditor = this.textBoxEditorLineNum.Text;
                viewerEditorSSH = this.textBoxEditorLineNumSSH.Text;
                if (viewerEditor == "") {
                    InfoBox.Warning(m_parent, Resources.Option_TextEditorCommandLineParamWithLineNumberNothing);
                    return false;
                }
                if (this.checkBoxLineSSH.Checked) {
                    if (viewerEditorSSH == "") {
                        InfoBox.Warning(m_parent, Resources.Option_TextEditorCommandLineParamWithLineNumberSSHNothing);
                    }
                } else {
                    viewerEditorSSH = "";
                }
                success = Configuration.CheckTextEditorCommandLineWithLineNumber(ref viewerEditor, m_parent);
                if (!success) {
                    return false;
                }
                success = Configuration.CheckTextEditorCommandLineWithLineNumber(ref viewerEditorSSH, m_parent);
                if (!success) {
                    return false;
                }
            }

            // Configに反映
            Configuration.Current.TextEditorCommandLine = editor;
            Configuration.Current.TextEditorCommandLineSSH = editorSSH;
            Configuration.Current.TextEditorCommandLineWithLineNumber = viewerEditor;
            Configuration.Current.TextEditorCommandLineWithLineNumberSSH = viewerEditorSSH;

            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            SetInitialValue(org);
        }
    }
}
