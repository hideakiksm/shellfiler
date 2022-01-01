using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞全般 の設定ページ
    //=========================================================================================
    public partial class FileOperationGeneralPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

            this.numericBackgroundTask.Minimum = Configuration.MIN_MAX_BACKGROUND_TASK_WAITABLE_COUNT;
            this.numericBackgroundTask.Maximum = Configuration.MAX_MAX_BACKGROUND_TASK_WAITABLE_COUNT;

            this.numericEtcTask.Minimum = Configuration.MIN_MAX_BACKGROUND_TASK_LIMITED_COUNT;
            this.numericEtcTask.Maximum = Configuration.MAX_MAX_BACKGROUND_TASK_LIMITED_COUNT;

            this.numericBackgroundTask.Value = Configuration.Current.MaxBackgroundTaskWaitableCountDefault;
            this.numericEtcTask.Value = Configuration.Current.MaxBackgroundTaskLimitedCountDefault;
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // 生成できるバックグラウンドタスクの最大数
            int backgroundCount = (int)(this.numericBackgroundTask.Value);
            success = Configuration.CheckMaxBackgroundTaskWaitableCountDefault(ref backgroundCount, m_parent);
            if (!success) {
                return false;
            }

            // 生成できるバックグラウンドタスクの最大数
            int limitedCount = (int)(this.numericEtcTask.Value);
            success = Configuration.CheckMaxBackgroundTaskLimitedCountDefault(ref limitedCount, m_parent);
            if (!success) {
                return false;
            }

            // Configに反映
            Configuration.Current.MaxBackgroundTaskWaitableCountDefault = backgroundCount;
            Configuration.Current.MaxBackgroundTaskLimitedCountDefault = limitedCount;

            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            this.numericBackgroundTask.Value = org.MaxBackgroundTaskWaitableCountDefault;
            this.numericEtcTask.Value = org.MaxBackgroundTaskLimitedCountDefault;
        }
    }
}
