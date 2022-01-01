using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：インストール情報＞作業フォルダ の設定ページ
    //=========================================================================================
    public partial class InstallWorkingDirPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public InstallWorkingDirPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
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
            // 作業ディレクトリ
            string configTempDir = config.TemporaryDirectoryDefault;
            string tempDirDefault = DirectoryManager.GetTemporaryDirectory("");
            string tempDir = DirectoryManager.GetTemporaryDirectory(configTempDir);
            if (configTempDir == "") {
                this.radioButtonTempAuto.Checked = true;
                this.textBoxTempAuto.Text = tempDirDefault;
                this.textBoxTempDir.Text = tempDirDefault;
            } else {
                this.radioButtonTempFix.Checked = true;
                this.textBoxTempAuto.Text = tempDirDefault;
                this.textBoxTempDir.Text = tempDir;
            }
            radioButtonTemp_CheckedChanged(null, null);
        }

        //=========================================================================================
        // 機　能：作業ディレクトリのラジオボタンが変化したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButtonTemp_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonTempAuto.Checked) {
                this.textBoxTempDir.Enabled = false;
                this.buttonTempRef.Enabled = false;
            } else {
                this.textBoxTempDir.Enabled = true;
                this.buttonTempRef.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：作業ディレクトリの参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTempRef_Click(object sender, EventArgs evt) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = this.textBoxTempDir.Text;
            DialogResult dr = fbd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                this.textBoxTempDir.Text = fbd.SelectedPath;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // 作業ディレクトリ
            string tempDir;
            if (this.radioButtonTempAuto.Checked) {
                tempDir = "";
            } else {
                tempDir = this.textBoxTempDir.Text;
                success = OptionSettingDialogUtils.CheckDirctory(tempDir, Resources.Option_TemporaryDirectoryCreate);
                if (!success) {
                    return false;
                }
                success = Configuration.CheckTemporaryDirectoryDefault(ref tempDir, m_parent);
                if (!success) {
                    return false;
                }
            }

            // Configに反映
            Configuration.Current.TemporaryDirectoryDefault = tempDir;

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
