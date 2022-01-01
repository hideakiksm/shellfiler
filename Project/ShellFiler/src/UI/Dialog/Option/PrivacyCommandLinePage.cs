using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // プライバシー＞コマンド履歴 の設定ページ
    //=========================================================================================
    public partial class PrivacyCommandLinePage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public PrivacyCommandLinePage(OptionSettingDialog parent) {
            m_parent = parent;
            InitializeComponent();

            this.numericCommandNum.Minimum = Configuration.MIN_COMMAND_HISTORY_MAX_COUNT;
            this.numericCommandNum.Maximum = Configuration.MAX_COMMAND_HISTORY_MAX_COUNT;

            // コンフィグ値をUIに反映
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
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            // コマンド履歴
            this.numericCommandNum.Value = config.CommandHistoryMaxCountDefault;
            this.checkBoxCommandSave.Checked = config.CommandHistorySaveDisk;
        }

        //=========================================================================================
        // 機　能：コマンド履歴の削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCommandDel_Click(object sender, EventArgs evt) {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(m_parent, Resources.Option_PrivacyCommandConfirm);
            if (result != DialogResult.Yes) {
                return;
            }
            DeleteHistoryProcedure.DeleteCommandHistory();
            InfoBox.Information(m_parent, Resources.Option_PrivacyCommandCompleted);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // コマンド履歴
            int commandMax = (int)(this.numericCommandNum.Value);
            success = Configuration.CheckCommandHistoryMaxCountDefault(ref commandMax, m_parent);
            if (!success) {
                return false;
            }

            // Configに反映
            Configuration.Current.CommandHistoryMaxCountDefault = commandMax;
            Configuration.Current.CommandHistorySaveDisk = this.checkBoxCommandSave.Checked;

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