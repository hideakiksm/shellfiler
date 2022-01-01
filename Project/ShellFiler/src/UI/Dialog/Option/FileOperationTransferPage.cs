using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileTask;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞全般 の設定ページ
    //=========================================================================================
    public partial class FileOperationTransferPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationTransferPage(OptionSettingDialog parent) {
            InitializeComponent();

            // 同名のファイルに対する操作
            string[] sameOperationItems = {
                Resources.OptionFileOprTransfer_SameForceOverwrite,
                Resources.OptionFileOprTransfer_SameOverwriteIfNewer,
                Resources.OptionFileOprTransfer_SameRenameNew,
                Resources.OptionFileOprTransfer_SameNotOverwrite,
                Resources.OptionFileOprTransfer_SameAutoRename,
                Resources.OptionFileOprTransfer_SameFullAutoTransfer,
            };
            this.comboBoxSameOpr.Items.AddRange(sameOperationItems);

            // 自動変更（Windows）
            string[] autoRenameWindowsItems = {
                Resources.DlgSameFile_AutoRenameUnderNum,
                Resources.DlgSameFile_AutoRenameUnder,
                Resources.DlgSameFile_AutoRenameParentheses,
                Resources.DlgSameFile_AutoRenameBracket,
            };
            this.comboBoxSameAutoWindows.Items.AddRange(autoRenameWindowsItems);

            // 自動変更（SSH）
            string[] autoRenameSSHItems = {
                Resources.DlgSameFile_AutoRenameUnderNum,
                Resources.DlgSameFile_AutoRenameUnder,
            };
            this.comboBoxSameAutoSSH.Items.AddRange(autoRenameSSHItems);

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
            // 同名ファイルの操作
            SameFileOption sameFile = config.SameFileOptionDefault;
            if (sameFile == null) {
                this.radioButtonSamePrev.Checked = true;
                this.comboBoxSameOpr.Enabled = false;
                this.comboBoxSameAutoWindows.Enabled = false;
                this.comboBoxSameAutoSSH.Enabled = false;
                SameFileOptionToUi(new SameFileOption());
            } else {
                this.radioButtonSameFix.Checked = true;
                this.comboBoxSameOpr.Enabled = true;
                this.comboBoxSameAutoWindows.Enabled = true;
                this.comboBoxSameAutoSSH.Enabled = true;
                SameFileOptionToUi(sameFile);
            }

            // ディレクトリ削除オプション
            DeleteFileOption delFile = config.DeleteFileOptionDefault;
            if (delFile == null) {
                this.radioButtonDeletePrev.Checked = true;
                this.checkBoxDirectory.Enabled = false;
                this.checkBoxAttr.Enabled = false;
                DeleteFileOptionToUi(new DeleteFileOption());
            } else {
                this.radioButtonDeleteFix.Checked = true;
                this.checkBoxDirectory.Enabled = true;
                this.checkBoxAttr.Enabled = true;
                DeleteFileOptionToUi(delFile);
            }
        }

        //=========================================================================================
        // 機　能：同名ファイル設定値をUIに設定する
        // 引　数：[in]sameOption   同名ファイル設定値
        // 戻り値：なし
        //=========================================================================================
        private void SameFileOptionToUi(SameFileOption sameOption) {
            // 同名のファイルに対する操作
            int indexOpr = 0;
            switch (sameOption.SameFileMode) {
                case SameFileOperation.SameFileTransferMode.ForceOverwrite:
                    indexOpr = 0;
                    break;
                case SameFileOperation.SameFileTransferMode.OverwriteIfNewer:
                    indexOpr = 1;
                    break;
                case SameFileOperation.SameFileTransferMode.RenameNew:
                    indexOpr = 2;
                    break;
                case SameFileOperation.SameFileTransferMode.NotOverwrite:
                    indexOpr = 3;
                    break;
                case SameFileOperation.SameFileTransferMode.AutoRename:
                    indexOpr = 4;
                    break;
                case SameFileOperation.SameFileTransferMode.FullAutoTransfer:
                    indexOpr = 5;
                    break;
            }

            // 自動変更（Windows）
            int indexAutoWindows = 0;
            switch (sameOption.AutoUpdateModeWindows) {
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber:
                    indexAutoWindows = 0;
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBar:
                    indexAutoWindows = 1;
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddParentheses:
                    indexAutoWindows = 2;
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddBracket:
                    indexAutoWindows = 3;
                    break;
            }

            // 自動変更（SSH）
            int indexAutoSSH = 0;
            switch (sameOption.AutoUpdateModeWindows) {
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber:
                    indexAutoSSH = 0;
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBar:
                    indexAutoSSH = 1;
                    break;
            }

            this.comboBoxSameOpr.SelectedIndex = indexOpr;
            this.comboBoxSameAutoWindows.SelectedIndex = indexAutoWindows;
            this.comboBoxSameAutoSSH.SelectedIndex = indexAutoSSH;
        }
        
        //=========================================================================================
        // 機　能：UIの同名ファイル設定値を取得する
        // 引　数：なし
        // 戻り値：同名ファイル設定値
        //=========================================================================================
        private SameFileOption UiToSameFileOption() {
            SameFileOption optionDefault = new SameFileOption();

            // 同名のファイルに対する操作
            SameFileOperation.SameFileTransferMode sameMode = optionDefault.SameFileMode;
            switch (this.comboBoxSameOpr.SelectedIndex) {
                case 0:
                    sameMode = SameFileOperation.SameFileTransferMode.ForceOverwrite;
                    break;
                case 1:
                    sameMode = SameFileOperation.SameFileTransferMode.OverwriteIfNewer;
                    break;
                case 2:
                    sameMode = SameFileOperation.SameFileTransferMode.RenameNew;
                    break;
                case 3:
                    sameMode = SameFileOperation.SameFileTransferMode.NotOverwrite;
                    break;
                case 4:
                    sameMode = SameFileOperation.SameFileTransferMode.AutoRename;
                    break;
                case 5:
                    sameMode = SameFileOperation.SameFileTransferMode.FullAutoTransfer;
                    break;
            }

            // 自動変更（Windows）
            SameFileOperation.SameFileAutoUpdateMode autoWindows = optionDefault.AutoUpdateModeWindows;
            switch (this.comboBoxSameAutoWindows.SelectedIndex) {
                case 0:
                    autoWindows = SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber;
                    break;
                case 1:
                    autoWindows = SameFileOperation.SameFileAutoUpdateMode.AddUnderBar;
                    break;
                case 2:
                    autoWindows = SameFileOperation.SameFileAutoUpdateMode.AddParentheses;
                    break;
                case 3:
                    autoWindows = SameFileOperation.SameFileAutoUpdateMode.AddBracket;
                    break;
            }

            // 自動変更（SSH）
            SameFileOperation.SameFileAutoUpdateMode autoSSH = optionDefault.AutoUpdateModeSSH;
            switch (this.comboBoxSameAutoSSH.SelectedIndex) {
                case 0:
                    autoSSH = SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber;
                    break;
                case 1:
                    autoSSH = SameFileOperation.SameFileAutoUpdateMode.AddUnderBar;
                    break;
            }

            SameFileOption sameOpt = new SameFileOption();
            sameOpt.SameFileMode = sameMode;
            sameOpt.AutoUpdateModeWindows = autoWindows;
            sameOpt.AutoUpdateModeSSH = autoSSH;
            return sameOpt;
        }

        //=========================================================================================
        // 機　能：UIに削除方法設定値を設定する
        // 引　数：[in]delOption  削除方法設定値
        // 戻り値：なし
        //=========================================================================================
        private void DeleteFileOptionToUi(DeleteFileOption delOption) {
            this.checkBoxDirectory.Checked = delOption.DeleteDirectoryAll;
            this.checkBoxAttr.Checked = delOption.DeleteSpecialAttrAll;
        }

        //=========================================================================================
        // 機　能：UIの削除方法設定値を取得する
        // 引　数：なし
        // 戻り値：削除方法設定値
        //=========================================================================================
        private DeleteFileOption UiToDeleteFileOption() {
            DeleteFileOption option = new DeleteFileOption();
            option.DeleteDirectoryAll = this.checkBoxDirectory.Checked;
            option.DeleteSpecialAttrAll = this.checkBoxAttr.Checked;
            return option;
        }
        
        //=========================================================================================
        // 機　能：同名ファイルの扱いのラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonSame_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonSamePrev.Checked) {
                this.comboBoxSameOpr.Enabled = false;
                this.comboBoxSameAutoWindows.Enabled = false;
                this.comboBoxSameAutoSSH.Enabled = false;
            } else {
                this.comboBoxSameOpr.Enabled = true;
                this.comboBoxSameAutoWindows.Enabled = true;
                this.comboBoxSameAutoSSH.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：削除方法のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonDelete_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonDeletePrev.Checked) {
                this.checkBoxDirectory.Enabled = false;
                this.checkBoxAttr.Enabled = false;
            } else {
                this.checkBoxDirectory.Enabled = true;
                this.checkBoxAttr.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            SameFileOption sameOption;
            if (this.radioButtonSamePrev.Checked) {
                sameOption = null;
            } else {
                sameOption = UiToSameFileOption();
            }
            
            DeleteFileOption deleteOption;
            if (this.radioButtonDeletePrev.Checked) {
                deleteOption = null;
            } else {
                deleteOption = UiToDeleteFileOption();
            }

            Configuration.Current.SameFileOptionDefault = sameOption;
            Configuration.Current.DeleteFileOptionDefault = deleteOption;

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
