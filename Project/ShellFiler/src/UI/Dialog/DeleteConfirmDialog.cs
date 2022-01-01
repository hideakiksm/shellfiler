using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {
    
    //=========================================================================================
    // クラス：削除確認ダイアログ
    // 　　　　DialogResultによる戻り値は以下の通り
    // 　　　　Yes:はい  No:いいえ  Ignore:すべて  Cancel:キャンセル
    //=========================================================================================
    public partial class DeleteConfirmDialog : Form {
        // 対象ファイルの名前
        private string m_fileName;

        // 対象ファイルの属性
        private FileAttribute m_fileAttribute;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteConfirmDialog() {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
            this.ActiveControl = this.buttonYes;
        }

        //=========================================================================================
        // 機　能：[はい]ボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonYes_Click(object sender, EventArgs evt) {
            if (!CheckKey()) {
                DialogResult = DialogResult.Ignore;
            }
            Close();
        }

        //=========================================================================================
        // 機　能：[すべて]ボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAll_Click(object sender, EventArgs evt) {
            if (!CheckKey()) {
                DialogResult = DialogResult.Ignore;
            }
            Close();
        }

        //=========================================================================================
        // 機　能：[いいえ]ボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNo_Click(object sender, EventArgs evt) {
            if (!CheckKey()) {
                DialogResult = DialogResult.No;
            }
            Close();
        }
        
        //=========================================================================================
        // 機　能：[キャンセル]ボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            if (!CheckKey()) {
                DialogResult = DialogResult.Cancel;
            }
            Close();
        }

        //=========================================================================================
        // 機　能：ボタンクリック時のキー入力をチェックする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private bool CheckKey() {
            if (Win32API.Win32GetAsyncKeyState(Keys.Enter) && Win32API.Win32GetAsyncKeyState(Keys.ShiftKey)) {
                DialogResult = DialogResult.Ignore;
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：キー入力をチェックする
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DeleteConfirmDialog_KeyDown(object sender, KeyEventArgs evt) {
            if (evt.KeyCode == Keys.Enter && evt.Shift) {
                DialogResult = DialogResult.Ignore;
                Close();
            } else if (evt.KeyCode == Keys.Escape) {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        //=========================================================================================
        // プロパティ：対象ファイルの名前
        //=========================================================================================
        public string FileName {
            set {
                m_fileName = value;
                this.textBoxFileName.Text = m_fileName;
                this.textBoxFileName.Select(0, 0);
            }
        }
        
        //=========================================================================================
        // プロパティ：対象ファイルの属性
        //=========================================================================================
        public FileAttribute FileAttribute {
            set {
                m_fileAttribute = value;
                List<string> listAttr = new List<string>();
                if (m_fileAttribute.IsDirectory) {
                    listAttr.Add(Resources.DlgDeleteConfirm_AttrDir);
                } else {
                    if (m_fileAttribute.IsReadonly) {
                        listAttr.Add(Resources.DlgDeleteConfirm_AttrRd);
                    }
                    if (m_fileAttribute.IsSystem) {
                        listAttr.Add(Resources.DlgDeleteConfirm_AttrSys);
                    }
                }
                string strAttr = StringUtils.CombineStringArray(listAttr, Resources.DlgDeleteConfirm_AttrSeparator);
                string message = string.Format(this.labelMessage1.Text, strAttr);
                this.labelMessage1.Text = message;
            }
        }
    }
}
