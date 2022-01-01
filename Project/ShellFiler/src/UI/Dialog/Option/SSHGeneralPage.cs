using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // SSH＞全般 の設定ページ
    //=========================================================================================
    public partial class SSHGeneralPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public SSHGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();

            // ショートカットの種類
            string[] shortcutItems = {
                Resources.OptionSSHGeneral_Shortcut,            // 0:直前に指定された結果
                Resources.OptionSSHGeneral_SymbolicLink,        // 1:シンボリックリンク
                Resources.OptionSSHGeneral_HardLink,            // 2:ハードリン直前に指定された結果
            };
            this.comboBoxShortcut.Items.AddRange(shortcutItems);

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
            // ショートカットの種類
            if (config.SSHShortcutTypeDefault == null) {
                this.comboBoxShortcut.SelectedIndex = 0;
            } else if (config.SSHShortcutTypeDefault == ShortcutType.SymbolicLink) {
                this.comboBoxShortcut.SelectedIndex = 1;
            } else {
                this.comboBoxShortcut.SelectedIndex = 2;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            // ショートカットの種類
            int indexShortcut = this.comboBoxShortcut.SelectedIndex;
            if (indexShortcut == 0) {
                Configuration.Current.SSHShortcutTypeDefault = null;
            } else if (indexShortcut == 1) {
                Configuration.Current.SSHShortcutTypeDefault = ShortcutType.SymbolicLink;
            } else {
                Configuration.Current.SSHShortcutTypeDefault = ShortcutType.HardLink;
            }

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