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
    // ファイル操作＞各種操作 の設定ページ
    //=========================================================================================
    public partial class FileOperationEtcPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationEtcPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

            // インクリメンタルサーチ
            string[] incrementalItems = {
                Resources.OptionFileOprEtc_IncrementalPrev,         // 0:直前に指定された結果
                Resources.OptionFileOprEtc_IncrementalTop,          // 1:先頭から検索
                Resources.OptionFileOprEtc_IncrementalMiddle,       // 2:中間の文字にもヒット
            };
            this.comboBoxIncremental.Items.AddRange(incrementalItems);

            // フォルダ作成
            string[] mkdirItems = {
                Resources.OptionFileOprEtc_MkDirPrev,               // 直前に指定された結果
                Resources.OptionFileOprEtc_MkDirMove,	            // 作成したフォルダに移動
                Resources.OptionFileOprEtc_MkDirStay,	            // 作成フォルダに移動しない
            };
            this.comboBoxMkDirMove.Items.AddRange(mkdirItems);

            // コマンド実行
            string[] commandItems = {
                Resources.OptionFileOprEtc_ExecOutputPrev,          // 直前に指定された結果
                Resources.OptionFileOprEtc_ExecOutputLog,           // 標準出力をログに出力
                Resources.OptionFileOprEtc_ExecOutputViewer,        // 標準出力をファイルビューアに出力
                Resources.OptionFileOprEtc_ExecOutputNone,          // 標準出力を中継しない（Windowsのみ）
            };
            this.comboBoxWindowsExecOutput.Items.AddRange(commandItems);
            this.comboBoxSSHExecOutput.Items.AddRange(commandItems);

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
            // インクリメンタルサーチ
            if (config.IncrementalSearchFromHeadDefault == null) {
                this.comboBoxIncremental.SelectedIndex = 0;
            } else if (config.IncrementalSearchFromHeadDefault.Value) {
                this.comboBoxIncremental.SelectedIndex = 1;
            } else {
                this.comboBoxIncremental.SelectedIndex = 2;
            }

            // フォルダ作成
            this.textBoxMkDirWindows.Text = config.MakeDirectoryNewWindowsName;
            this.textBoxMkDirSSH.Text = config.MakeDirectoryNewSSHName;
            if (config.MakeDirectoryMoveCurrentDefault == null) {
                this.comboBoxMkDirMove.SelectedIndex = 0;
            } else if (config.MakeDirectoryMoveCurrentDefault.Value) {
                this.comboBoxMkDirMove.SelectedIndex = 1;
            } else {
                this.comboBoxMkDirMove.SelectedIndex = 2;
            }

            // コマンド実行
            if (config.ShellExecuteRelayModeWindowsDefault == null) {
                this.comboBoxWindowsExecOutput.SelectedIndex = 0;
            } else if (config.ShellExecuteRelayModeWindowsDefault == ShellExecuteRelayMode.RelayLogWindow) {
                this.comboBoxWindowsExecOutput.SelectedIndex = 1;
            } else if (config.ShellExecuteRelayModeWindowsDefault == ShellExecuteRelayMode.RelayFileViewer) {
                this.comboBoxWindowsExecOutput.SelectedIndex = 2;
            } else {
                this.comboBoxWindowsExecOutput.SelectedIndex = 3;
            }
            if (config.ShellExecuteRelayModeSSHDefault == null) {
                this.comboBoxSSHExecOutput.SelectedIndex = 0;
            } else if (config.ShellExecuteRelayModeSSHDefault == ShellExecuteRelayMode.RelayLogWindow) {
                this.comboBoxSSHExecOutput.SelectedIndex = 1;
            } else if (config.ShellExecuteRelayModeSSHDefault == ShellExecuteRelayMode.RelayFileViewer) {
                this.comboBoxSSHExecOutput.SelectedIndex = 2;
            } else {
                this.comboBoxSSHExecOutput.SelectedIndex = 3;
            }

            // ミラーコピーで除外するファイル
            this.textBoxMirrorExcept.Text = config.MirrorCopyExceptFiles;
        }

        //=========================================================================================
        // 機　能：ミラーコピーで除外ファイルの既定値のボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonResetMirror_Click(object sender, EventArgs evt) {
            Configuration configNew = new Configuration();
            this.textBoxMirrorExcept.Text = configNew.MirrorCopyExceptFiles;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // フォルダ作成の初期値を先にチェック
            string winDir = this.textBoxMkDirWindows.Text;
            success = Configuration.CheckMakeDirectoryNewWindowsName(ref winDir, m_parent);
            if (!success) {
                return false;
            }
            string sshDir = this.textBoxMkDirSSH.Text;
            success = Configuration.CheckMakeDirectoryNewSSHName(ref sshDir, m_parent);
            if (!success) {
                return false;
            }

            // インクリメンタルサーチ
            int indexIncre = this.comboBoxIncremental.SelectedIndex;
            if (indexIncre == 0) {
                Configuration.Current.IncrementalSearchFromHeadDefault = null;
            } else if (indexIncre == 1) {
                Configuration.Current.IncrementalSearchFromHeadDefault = new BooleanFlag(true);
            } else {
                Configuration.Current.IncrementalSearchFromHeadDefault = new BooleanFlag(false);
            }

            // フォルダ作成
            Configuration.Current.MakeDirectoryNewWindowsName = winDir;
            Configuration.Current.MakeDirectoryNewSSHName = sshDir;
            int indexMkDir = this.comboBoxMkDirMove.SelectedIndex;
            if (indexMkDir == 0) {
                Configuration.Current.MakeDirectoryMoveCurrentDefault = null;
            } else if (indexMkDir == 1) {
                Configuration.Current.MakeDirectoryMoveCurrentDefault = new BooleanFlag(true);
            } else {
                Configuration.Current.MakeDirectoryMoveCurrentDefault = new BooleanFlag(false);
            }

            // コマンド実行
            int indexExecWindows = this.comboBoxWindowsExecOutput.SelectedIndex;
            if (indexExecWindows == 0) {
                Configuration.Current.ShellExecuteRelayModeWindowsDefault = null;
            } else if (indexExecWindows == 1) {
                Configuration.Current.ShellExecuteRelayModeWindowsDefault = ShellExecuteRelayMode.RelayLogWindow;
            } else if (indexExecWindows == 2) {
                Configuration.Current.ShellExecuteRelayModeWindowsDefault = ShellExecuteRelayMode.RelayFileViewer;
            } else {
                Configuration.Current.ShellExecuteRelayModeWindowsDefault = ShellExecuteRelayMode.None;
            }
            int indexExecSSH = this.comboBoxSSHExecOutput.SelectedIndex;
            if (indexExecSSH == 0) {
                Configuration.Current.ShellExecuteRelayModeSSHDefault = null;
            } else if (indexExecSSH == 1) {
                Configuration.Current.ShellExecuteRelayModeSSHDefault = ShellExecuteRelayMode.RelayLogWindow;
            } else if (indexExecSSH == 2) {
                Configuration.Current.ShellExecuteRelayModeSSHDefault = ShellExecuteRelayMode.RelayFileViewer;
            } else {
                Configuration.Current.ShellExecuteRelayModeSSHDefault = ShellExecuteRelayMode.None;
            }

            // ミラーコピーで除外するファイル
            Configuration.Current.MirrorCopyExceptFiles = this.textBoxMirrorExcept.Text;

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