using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：インクリメンタルサーチダイアログ
    //=========================================================================================
    public partial class FileIncrementalSearchDialog : Form {
        // 自動的に先頭から比較のデフォルト
        public const bool SEARCH_FROM_HEAD_DEFAULT = true;

        // 操作対象の対象パスがあるファイル一覧
        private FileListView m_fileListView;

        // エラー文字列（エラーがないときnull）
        private string m_errorString = null;

        // 自動的に先頭から比較をOFFにしたときtrue
        private bool m_autoCompareHeadOff = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileIncrementalSearchDialog() {
            InitializeComponent();
            this.ActiveControl = this.textBoxFileName;
            this.checkBoxCompareHead.Checked = GetConfigSearchHeadDefault();
        }

        //=========================================================================================
        // 機　能：先頭から比較するかどうかのデフォルト設定を取得する
        // 引　数：なし
        // 戻り値：先頭から比較する設定にするときtrue
        //=========================================================================================
        private bool GetConfigSearchHeadDefault() {
            bool result;
            BooleanFlag fromHead = Configuration.Current.IncrementalSearchFromHeadDefault;
            if (fromHead == null) {
                result = Program.Document.UserGeneralSetting.IncrementalSearchFromHead;
            } else {
                result = fromHead.Value;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：情報パネルの描画イベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelInformation_Paint(object sender, PaintEventArgs evt) {
            if (m_errorString != null) {
                Graphics g = evt.Graphics;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                Brush backBrush = new SolidBrush(Configuration.Current.DialogErrorBackColor);
                Brush textBrush = new SolidBrush(Configuration.Current.DialogErrorTextColor);
                try {
                    g.FillRectangle(backBrush, this.panelInformation.ClientRectangle);
                    g.DrawString(m_errorString, SystemFonts.DefaultFont, textBrush, this.panelInformation.ClientRectangle, sf);
                } finally {
                    backBrush.Dispose();
                    textBrush.Dispose();
                    sf.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：検索文字列が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxFileName_TextChanged(object sender, EventArgs evt) {
            // 「.」から始まる文字列を検索するときは自動的に先頭から比較をOFF
            string searchStr = this.textBoxFileName.Text;
            if (searchStr.StartsWith(".")) {
                this.checkBoxCompareHead.Checked = false;
                m_autoCompareHeadOff = true;
            } else if (searchStr == "" && m_autoCompareHeadOff) {
                this.checkBoxCompareHead.Checked = GetConfigSearchHeadDefault();
                m_autoCompareHeadOff = false;
            }

            // 検索を実行
            bool success = m_fileListView.FileListViewComponent.IncrementalSearch(searchStr, this.checkBoxCompareHead.Checked, IncrementalSearchOperation.FromTop);
            if (!success) {
                m_errorString = Resources.DlgISearch_NotFound;
            } else {
                m_errorString = null;
            }
            this.panelInformation.Invalidate();
        }

        //=========================================================================================
        // 機　能：各コントロールにフォーカスが設定されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SubControl_Enter(object sender, EventArgs evt) {
            // 常に検索文字列にフォーカスを設定
            this.ActiveControl = this.textBoxFileName;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxFileName_KeyDown(object sender, KeyEventArgs evt) {
            if (evt.Alt && evt.KeyCode == Keys.T) {
                this.checkBoxCompareHead.Checked = !this.checkBoxCompareHead.Checked;
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Up) {
                buttonUp_Click(this, null);
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Down) {
                buttonDown_Click(this, null);
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Space) {
                buttonMark_Click(this, null);
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.ControlKey) {
                this.checkBoxCompareHead.Checked = !this.checkBoxCompareHead.Checked;
            }
        }

        //=========================================================================================
        // 機　能：カーソル上移動時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            bool success = m_fileListView.FileListViewComponent.IncrementalSearch(this.textBoxFileName.Text, this.checkBoxCompareHead.Checked, IncrementalSearchOperation.MoveUp);
            if (!success) {
                m_errorString = Resources.DlgISearch_CanNotMove;
            } else {
                m_errorString = null;
            }
            this.panelInformation.Invalidate();
        }

        //=========================================================================================
        // 機　能：カーソル下移動時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            bool success = m_fileListView.FileListViewComponent.IncrementalSearch(this.textBoxFileName.Text, this.checkBoxCompareHead.Checked, IncrementalSearchOperation.MoveDown);
            if (!success) {
                m_errorString = Resources.DlgISearch_CanNotMove;
            } else {
                m_errorString = null;
            }
            this.panelInformation.Invalidate();
        }

        //=========================================================================================
        // 機　能：マーク時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonMark_Click(object sender, EventArgs evt) {
            if (m_errorString != null) {
                return;
            }
            bool success = m_fileListView.FileListViewComponent.IncrementalSearch(this.textBoxFileName.Text, this.checkBoxCompareHead.Checked, IncrementalSearchOperation.Mark);
            if (!success) {
                m_errorString = Resources.DlgISearch_CanNotMove;
            } else {
                m_errorString = null;
            }
            this.panelInformation.Invalidate();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            Program.Document.UserGeneralSetting.IncrementalSearchFromHead = this.checkBoxCompareHead.Checked;
        }

        //=========================================================================================
        // プロパティ：処理対象となるファイル一覧
        //=========================================================================================
        public FileListView FileListView {
            set {
                m_fileListView = value;
            }
        }
    }
}
