using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞マークなし操作 の設定ページ
    //=========================================================================================
    public partial class FileOperationMarklessPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationMarklessPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

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
            this.checkBoxCopy.Checked = Configuration.Current.MarklessCopy;
            this.checkBoxMove.Checked = Configuration.Current.MarklessMove;
            this.checkBoxDelete.Checked = Configuration.Current.MarklessDelete;
            this.checkBoxShortcut.Checked = Configuration.Current.MarklessShortcut;
            this.checkBoxAttribute.Checked = Configuration.Current.MarklessAttribute;
            this.checkBoxPack.Checked = Configuration.Current.MarklessPack;
            this.checkBoxUnpack.Checked = Configuration.Current.MarklessUnpack;
            this.checkBoxEdit.Checked = Configuration.Current.MarklessEdit;
            this.checkBoxFolderSize.Checked = Configuration.Current.MarklessFodlerSize;
        }

        //=========================================================================================
        // 機　能：チェックボックスのチェックが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CheckBox_CheckedChanged(object sender, EventArgs evt) {
            if (sender == this.checkBoxCopy) {
                this.labelCopy.Visible = this.checkBoxCopy.Checked;
            } else if (sender == this.checkBoxMove) {
                this.labelMove.Visible = this.checkBoxMove.Checked;
            } else if (sender == this.checkBoxDelete) {
                this.labelDelete.Visible = this.checkBoxDelete.Checked;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            Configuration.Current.MarklessCopy = this.checkBoxCopy.Checked;
            Configuration.Current.MarklessMove = this.checkBoxMove.Checked;
            Configuration.Current.MarklessDelete = this.checkBoxDelete.Checked;
            Configuration.Current.MarklessShortcut = this.checkBoxShortcut.Checked;
            Configuration.Current.MarklessAttribute = this.checkBoxAttribute.Checked;
            Configuration.Current.MarklessPack = this.checkBoxPack.Checked;
            Configuration.Current.MarklessUnpack = this.checkBoxUnpack.Checked;
            Configuration.Current.MarklessEdit = this.checkBoxEdit.Checked;
            Configuration.Current.MarklessFodlerSize = this.checkBoxFolderSize.Checked;

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