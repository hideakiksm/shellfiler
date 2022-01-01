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
    public partial class SSHTerminalLogPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public SSHTerminalLogPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

            this.numericRotationSize.Minimum = Configuration.MIN_TERMINAL_LOG_FILE_COUNT;
            this.numericRotationSize.Maximum = Configuration.MAX_TERMINAL_LOG_FILE_COUNT;
            this.numericRotationSize.Minimum = Configuration.MIN_TERMINAL_LOG_MAX_SIZE;
            this.numericRotationSize.Maximum = Configuration.MAX_TERMINAL_LOG_MAX_SIZE;

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
            if (config.TerminalLogType == TerminalLogType.None) {
                this.radioFileNone.Checked = true;
            } else if (config.TerminalLogType == TerminalLogType.Integrate) {
                this.radioFileIntegrate.Checked = true;
            } else {
                this.radioFileEachSession.Checked = true;
            }
            this.numericRotationSize.Value = config.TerminalLogMaxSize;
            this.numericRotationCount.Value = config.TerminalLogFileCount;
            if (config.TerminalLogOutputFolder == null) {
                this.textBoxFolder.Text = "";
            } else {
                this.textBoxFolder.Text = config.TerminalLogOutputFolder;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;
            
            // 出力種別
            TerminalLogType logType;
            if (this.radioFileNone.Checked) {
                logType = TerminalLogType.None;
            } else if (this.radioFileIntegrate.Checked) {
                logType = TerminalLogType.Integrate;
            } else {
                logType = TerminalLogType.EachSession;
            }

            // 最大ファイルサイズ
            int fileSize = (int)(this.numericRotationSize.Value);
            success = Configuration.CheckTerminalLogMaxSize(ref fileSize, m_parent);
            if (!success) {
                return false;
            }

            // 最大ファイル数
            int fileCount = (int)(this.numericRotationCount.Value);
            success = Configuration.CheckTerminalLogFileCount(ref fileCount, m_parent);
            if (!success) {
                return false;
            }

            // 出力先
            string outputDir;
            if (this.textBoxFolder.Text.Trim() == "") {
                outputDir = null;
            } else {
                outputDir = this.textBoxFolder.Text.Trim();
                success = OptionSettingDialogUtils.CheckDirctory(outputDir, Resources.Option_OutputDirectoryCreate);
                if (!success) {
                    return false;
                }
            }

            // 設定
            Configuration.Current.TerminalLogType = logType;
            Configuration.Current.TerminalLogMaxSize = fileSize;
            Configuration.Current.TerminalLogFileCount = fileCount;
            Configuration.Current.TerminalLogOutputFolder = outputDir;

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