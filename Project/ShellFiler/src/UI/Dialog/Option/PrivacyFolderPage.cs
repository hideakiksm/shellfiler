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
    // プライバシー＞フォルダ履歴 の設定ページ
    //=========================================================================================
    public partial class PrivacyFolderPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public PrivacyFolderPage(OptionSettingDialog parent) {
            m_parent = parent;
            InitializeComponent();

            this.numericFolderNum.Minimum = Configuration.MIN_PATH_HISTORY_MAX_COUNT;
            this.numericFolderNum.Maximum = Configuration.MAX_PATH_HISTORY_MAX_COUNT;
            this.numericWholeFolder.Minimum = Configuration.MIN_PATH_HISTORY_MAX_COUNT;
            this.numericWholeFolder.Maximum = Configuration.MAX_PATH_HISTORY_MAX_COUNT;

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
            // フォルダ履歴
            this.numericFolderNum.Value = config.PathHistoryMaxCountDefault;
            this.numericWholeFolder.Value = config.PathHistoryWholeMaxCountDefault;
            this.checkBoxFolderSave.Checked = config.PathHistoryWholeSaveDisk;
        }

        //=========================================================================================
        // 機　能：フォルダ履歴の削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderDel_Click(object sender, EventArgs evt) {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(m_parent, Resources.Option_PrivacyFolderConfirm);
            if (result != DialogResult.Yes) {
                return;
            }
            DeleteHistoryProcedure.DeleteFolderHistory();
            InfoBox.Information(m_parent, Resources.Option_PrivacyFolderCompleted);
        }
        
        //=========================================================================================
        // 機　能：起動フォルダの設定へのリンクボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelInitFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            OptionStructureItem item = m_parent.OptionStructure.GetPageItemFromType(typeof(FileListInitialPage));
            m_parent.ChangePage(item, true);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // フォルダ履歴
            int pathMax = (int)(this.numericFolderNum.Value);
            success = Configuration.CheckPathHistoryMaxCountDefault(ref pathMax, m_parent);
            if (!success) {
                return false;
            }
            int pathMaxWhole = (int)(this.numericWholeFolder.Value);
            success = Configuration.CheckPathHistoryMaxCountDefault(ref pathMaxWhole, m_parent);
            if (!success) {
                return false;
            }

            // Configに反映
            Configuration.Current.PathHistoryMaxCountDefault = pathMax;
            Configuration.Current.PathHistoryWholeMaxCountDefault = pathMax;
            Configuration.Current.PathHistoryWholeSaveDisk = this.checkBoxFolderSave.Checked;

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