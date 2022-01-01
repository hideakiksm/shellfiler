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
    // プライバシー＞全般 の設定ページ
    //=========================================================================================
    public partial class PrivacyGeneralPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public PrivacyGeneralPage(OptionSettingDialog parent) {
            m_parent = parent;
            InitializeComponent();

            this.checkBoxFolder.Checked = true;
            this.checkBoxViewer.Checked = true;
            this.checkBoxCommand.Checked = true;
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(m_parent, Resources.Option_PrivacyGeneralDeleteConfirm);
            if (result != DialogResult.Yes) {
                return;
            }
            if (this.checkBoxFolder.Checked) {
                DeleteHistoryProcedure.DeleteFolderHistory();
            }
            if (this.checkBoxViewer.Checked) {
                DeleteHistoryProcedure.DeleteViewerHistory();
            }
            if (this.checkBoxCommand.Checked) {
                DeleteHistoryProcedure.DeleteCommandHistory();
            }
            InfoBox.Information(m_parent, Resources.Option_PrivacyGeneralDeleteCompleted);
        }

        //=========================================================================================
        // 機　能：選択のチェックボックスの状態が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CheckBox_CheckedChanged(object sender, EventArgs evt) {
            if (this.checkBoxFolder.Checked || this.checkBoxViewer.Checked || this.checkBoxCommand.Checked) {
                this.buttonDelete.Enabled = true;
            } else {
                this.buttonDelete.Enabled = false;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
        }
    }
}