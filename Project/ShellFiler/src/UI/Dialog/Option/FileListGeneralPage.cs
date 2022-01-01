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
    // ファイル一覧＞全般 の設定ページ
    //=========================================================================================
    public partial class FileListGeneralPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

            this.numericAutoUpdate.Minimum = Configuration.MIN_CHECK_AUTO_DIRECTORY_UPDATE_WAIT;
            this.numericAutoUpdate.Maximum = Configuration.MAX_CHECK_AUTO_DIRECTORY_UPDATE_WAIT;
            this.numericAutoUpdate.Value = Configuration.Current.AutoDirectoryUpdateWait;
            this.checkBoxRefreshTabChange.Checked = Configuration.Current.RefreshFileListTabChange;
            this.checkBoxRefreshSSH.Checked = Configuration.Current.RefreshFileListTabChangeSSH;
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

            // 自動更新までの時間
            int autoUpdate = (int)(this.numericAutoUpdate.Value);
            success = Configuration.CheckAutoDirectoryUpdateWait(ref autoUpdate, m_parent);
            if (!success) {
                return false;
            }

            // Configに反映
            Configuration.Current.AutoDirectoryUpdateWait = autoUpdate;
            Configuration.Current.RefreshFileListTabChange = this.checkBoxRefreshTabChange.Checked;
            Configuration.Current.RefreshFileListTabChangeSSH = this.checkBoxRefreshSSH.Checked;

            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            this.numericAutoUpdate.Value = org.AutoDirectoryUpdateWait;
        }
    }
}
