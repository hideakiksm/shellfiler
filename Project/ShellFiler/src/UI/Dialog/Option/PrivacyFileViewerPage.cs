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
    // プライバシー＞ファイルビューア の設定ページ
    //=========================================================================================
    public partial class PrivacyFileViewerPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public PrivacyFileViewerPage(OptionSettingDialog parent) {
            m_parent = parent;
            InitializeComponent();

            this.numericViewerNum.Minimum = Configuration.MIN_VIEWER_SEARCH_HISTORY_MAX_COUNT;
            this.numericViewerNum.Maximum = Configuration.MAX_VIEWER_SEARCH_HISTORY_MAX_COUNT;

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
            // ビューア履歴
            this.numericViewerNum.Value = config.ViewerSearchHistoryMaxCountDefault;
            this.checkBoxViewerSave.Checked = config.ViewerSearchHistorySaveDisk;
        }
        
        //=========================================================================================
        // 機　能：ビューア検索履歴の削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonViewerDel_Click(object sender, EventArgs evt) {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(m_parent, Resources.Option_PrivacyViewerConfirm);
            if (result != DialogResult.Yes) {
                return;
            }
            DeleteHistoryProcedure.DeleteViewerHistory();
            InfoBox.Information(m_parent, Resources.Option_PrivacyViewerCompleted);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // ビューア履歴
            int viewerMax = (int)(this.numericViewerNum.Value);
            success = Configuration.CheckViewerSearchHistoryMaxCountDefault(ref viewerMax, m_parent);
            if (!success) {
                return false;
            }

            // Configに反映
            Configuration.Current.ViewerSearchHistoryMaxCountDefault = viewerMax;
            Configuration.Current.ViewerSearchHistorySaveDisk = this.checkBoxViewerSave.Checked;

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