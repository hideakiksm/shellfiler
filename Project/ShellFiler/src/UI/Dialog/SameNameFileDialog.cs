using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：同名ファイルダイアログ
    //=========================================================================================
    public partial class SameNameFileDialog : Form {
        // キャッシュ情報
        private FileOperationRequestContext m_cacheContext;

        // 同名ダイアログの結果
        private SameFileOperation m_sameFileOperation;

        // 変更前のファイル名
        private string m_orgFileName;

        // 転送の種類（コピー、移動、ショートカット作成）
        private SameNameFileTransfer.TransferMode m_transferMode;

        // 入力が確定状態のときtrue
        private bool m_dialogSuccess = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SameNameFileDialog() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：ダイアログを初期化する
        // 引　数：[in]cacheContext   キャッシュ情報
        // 　　　　[in]fileDetail     同名ファイルの詳細情報
        // 　　　　[in]transferMode   転送モード
        // 戻り値：なし
        //=========================================================================================
        public void InitializeDialog(FileOperationRequestContext cacheContext, SameNameTargetFileDetail fileDetail, SameNameFileTransfer.TransferMode transferMode) {
            m_cacheContext = cacheContext;
            m_orgFileName = m_sameFileOperation.NewName;
            textBoxRename.Text = m_sameFileOperation.NewName;
            m_transferMode = transferMode;

            bool checkForce = false;
            bool checkIfNewer = false;
            bool checkRename = false;
            bool checkNotOverwrite = false;
            bool checkAuto = false;
            bool checkFull = false;
            switch (m_sameFileOperation.SameFileMode) {
                case SameFileOperation.SameFileTransferMode.ForceOverwrite:
                    checkForce = true;
                    this.ActiveControl = this.radioButtonForce;
                    break;
                case SameFileOperation.SameFileTransferMode.OverwriteIfNewer:
                    checkIfNewer = true;
                    this.ActiveControl = this.radioButtonIfNewer;
                    break;
                case SameFileOperation.SameFileTransferMode.RenameNew:
                    checkRename = true;
                    this.ActiveControl = this.textBoxRename;
                    break;
                case SameFileOperation.SameFileTransferMode.NotOverwrite:
                    checkNotOverwrite = true;
                    this.ActiveControl = this.radioButtonNotOverwrite;
                    break;
                case SameFileOperation.SameFileTransferMode.AutoRename:
                    checkAuto = true;
                    this.ActiveControl = this.radioButtonAuto;
                    break;
                case SameFileOperation.SameFileTransferMode.FullAutoTransfer:
                    checkFull = true;
                    this.ActiveControl = this.radioButtonFullAuto;
                    break;
                default:
                    Program.Abort("同名ダイアログで選択値の初期化ができません。");
                    break;
            }
            this.radioButtonForce.Checked = checkForce;
            this.radioButtonIfNewer.Checked = checkIfNewer;
            this.radioButtonRename.Checked = checkRename;
            this.radioButtonNotOverwrite.Checked = checkNotOverwrite;
            this.radioButtonAuto.Checked = checkAuto;
            this.radioButtonFullAuto.Checked = checkFull;
            
            checkBoxApplyAll.Checked = m_sameFileOperation.AllApply;

            if (FileSystemID.IsWindows(m_sameFileOperation.DestFileSystemId)) {
                string[] autoRenameItems = {
                    Resources.DlgSameFile_AutoRenameUnderNum,
                    Resources.DlgSameFile_AutoRenameUnder,
                    Resources.DlgSameFile_AutoRenameParentheses,
                    Resources.DlgSameFile_AutoRenameBracket,
                };
                comboBoxAutoRename.Items.AddRange(autoRenameItems);
            } else if (FileSystemID.IsSSH(m_sameFileOperation.DestFileSystemId)) {
                string[] autoRenameItems = {
                    Resources.DlgSameFile_AutoRenameUnderNum,
                    Resources.DlgSameFile_AutoRenameUnder,
                };
                comboBoxAutoRename.Items.AddRange(autoRenameItems);
            } else {
                FileSystemID.NotSupportError(m_sameFileOperation.DestFileSystemId);
            }
            int indexMax = comboBoxAutoRename.Items.Count - 1;
            switch (m_sameFileOperation.AutoUpdateMode) {
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber:
                    comboBoxAutoRename.SelectedIndex = 0;
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBar:
                    comboBoxAutoRename.SelectedIndex = 1;
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddParentheses:
                    comboBoxAutoRename.SelectedIndex = Math.Min(2, indexMax);
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddBracket:
                    comboBoxAutoRename.SelectedIndex = Math.Min(3, indexMax);
                    break;
                default:
                    Program.Abort("同名ダイアログで自動リネームの初期化ができません。");
                    break;
            }

            if (transferMode == SameNameFileTransfer.TransferMode.LinkSameFile) {
                this.radioButtonIfNewer.Enabled = false;
                this.radioButtonFullAuto.Enabled = false;
                int infoWidth = this.sameNameFileInfo.Width + 8;
                this.Width = this.Width - infoWidth;
                this.buttonCancel.Left -= infoWidth;
                this.buttonOk.Left -= infoWidth;
                this.sameNameFileInfo.Hide();
            } else {
                this.sameNameFileInfo.InitializeControl(m_cacheContext, fileDetail);
            }
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SameNameFileDialog_KeyDown(object sender, KeyEventArgs evt) {
            if (textBoxRename.Focused) {
                if (evt.KeyCode == Keys.Up) {
                    if (m_transferMode == SameNameFileTransfer.TransferMode.LinkSameFile) {
                        radioButtonForce.Select();
                        radioButtonForce.Focus();
                    } else {
                        radioButtonIfNewer.Select();
                        radioButtonIfNewer.Focus();
                    }
                } else if (evt.KeyCode == Keys.Down) {
                    radioButtonNotOverwrite.Select();
                    radioButtonNotOverwrite.Focus();
                }
            } else {
                if (evt.Control || evt.Alt) {
                    return;
                }
                if (m_transferMode == SameNameFileTransfer.TransferMode.LinkSameFile) {
                    switch (evt.KeyCode) {
                        case Keys.O:
                            this.radioButtonForce.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                        case Keys.R:
                            this.radioButtonRename.Checked = true;
                            this.textBoxRename.Enabled = true;
                            BeginInvoke(new SetFocusRenameBoxDelegate(SetFocusRenameBox));
                            break;
                        case Keys.N:
                            this.radioButtonNotOverwrite.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                        case Keys.A:
                            this.radioButtonAuto.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                    }
                } else {
                    switch (evt.KeyCode) {
                        case Keys.O:
                            this.radioButtonForce.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                        case Keys.U:
                            this.radioButtonIfNewer.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                        case Keys.R:
                            this.radioButtonRename.Checked = true;
                            this.textBoxRename.Enabled = true;
                            BeginInvoke(new SetFocusRenameBoxDelegate(SetFocusRenameBox));
                            break;
                        case Keys.N:
                            this.radioButtonNotOverwrite.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                        case Keys.A:
                            this.radioButtonAuto.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                        case Keys.F:
                            this.radioButtonFullAuto.Checked = true;
                            buttonOk_Click(this, null);
                            break;
                    }
                }
            }
        }

        private delegate void SetFocusRenameBoxDelegate();
        private void SetFocusRenameBox() {
            this.ActiveControl = this.textBoxRename;
            this.textBoxRename.Focus();
        }

        //=========================================================================================
        // 機　能：ラジオボタンの選択状態が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButton_CheckedChanged(object sender, EventArgs e) {
            if (this.radioButtonRename.Checked) {
                comboBoxAutoRename.Enabled = false;
                textBoxRename.Enabled = true;
                FormUtils.SetCursorPosExtension(textBoxRename);
                textBoxRename.Focus();
            } else if (this.radioButtonAuto.Checked || this.radioButtonFullAuto.Checked) {
                textBoxRename.Enabled = false;
                comboBoxAutoRename.Enabled = true;
            } else {
                textBoxRename.Enabled = false;
                comboBoxAutoRename.Enabled = false;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント（内部発生イベントのときtrue）
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // 入力値をチェック
            m_dialogSuccess = false;
            if (this.radioButtonRename.Checked) {
                if (textBoxRename.Text == "") {
                    InfoBox.Warning(this, Resources.DlgSameFile_MsgNoName);
                    return;
                }
                if (textBoxRename.Text == m_orgFileName) {
                    InfoBox.Warning(this, Resources.DlgSameFile_MsgNotChange);
                    return;
                }
            }

            // 値を取得
            m_sameFileOperation.NewName = this.textBoxRename.Text;
            m_sameFileOperation.AllApply = this.checkBoxApplyAll.Checked;
            if (this.radioButtonForce.Checked) {
                m_sameFileOperation.SameFileMode = SameFileOperation.SameFileTransferMode.ForceOverwrite;
            } else if (this.radioButtonIfNewer.Checked) {
                m_sameFileOperation.SameFileMode = SameFileOperation.SameFileTransferMode.OverwriteIfNewer;
            } else if (this.radioButtonRename.Checked) {
                m_sameFileOperation.SameFileMode = SameFileOperation.SameFileTransferMode.RenameNew;
                m_sameFileOperation.NewName = textBoxRename.Text;
            } else if (this.radioButtonNotOverwrite.Checked) {
                m_sameFileOperation.SameFileMode = SameFileOperation.SameFileTransferMode.NotOverwrite;
            } else if (this.radioButtonAuto.Checked) {
                m_sameFileOperation.SameFileMode = SameFileOperation.SameFileTransferMode.AutoRename;
            } else if (this.radioButtonFullAuto.Checked) {
                m_sameFileOperation.SameFileMode = SameFileOperation.SameFileTransferMode.FullAutoTransfer;
            } else {
                Program.Abort("同名ファイルダイアログでモード入力値の判断ができません。");
            }

            switch (this.comboBoxAutoRename.SelectedIndex) {
                case 0:
                    m_sameFileOperation.AutoUpdateMode = SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber;
                    break;
                case 1:
                    m_sameFileOperation.AutoUpdateMode = SameFileOperation.SameFileAutoUpdateMode.AddUnderBar;
                    break;
                case 2:
                    m_sameFileOperation.AutoUpdateMode = SameFileOperation.SameFileAutoUpdateMode.AddParentheses;
                    break;
                case 3:
                    m_sameFileOperation.AutoUpdateMode = SameFileOperation.SameFileAutoUpdateMode.AddBracket;
                    break;
                default:
                    Program.Abort("同名ファイルダイアログで自動リネーム入力値の判断ができません。");
                    break;
            }

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) {
                m_sameFileOperation.AllApply = true;
            }

            m_dialogSuccess = true;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SameNameFileDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && !m_dialogSuccess) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：属性が異なるとき転送のリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelFullAuto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            InfoBox.Information(this, Resources.DlgSameFile_MsgFullAutoHelp);
        }

        //=========================================================================================
        // プロパティ：同名ダイアログの結果（getがnullのときはキャンセル）
        //=========================================================================================
        public SameFileOperation Result {
            get {
                if (DialogResult == DialogResult.Cancel) {
                    return null;
                } else {
                    return m_sameFileOperation;
                }
            }
            set {
                m_sameFileOperation = value;
            }
        }
    }
}
