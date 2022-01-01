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
    // クラス：インストール情報＞差分表示ツール の設定ページ
    //=========================================================================================
    public partial class InstallDiffPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public InstallDiffPage(OptionSettingDialog parent) {
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
            this.textBoxDiffCommand.Text = config.DiffCommandLine;
            this.textBoxDiffDirectoryCommand.Text = config.DiffDirectoryCommandLine;
        }

        //=========================================================================================
        // 機　能：参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDiffRef_Click(object sender, EventArgs evt) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Resources.OptionInstallDiffRefTitle;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            ofd.Filter = Resources.OptionInstallDiffRefFilter;
            ofd.RestoreDirectory = true;
            DialogResult dr = ofd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                string diffExe = ofd.FileName;
                if (diffExe.IndexOf(' ') != -1) {
                    this.textBoxDiffCommand.Text = "\"" + diffExe + "\" {0}";
                } else {
                    this.textBoxDiffCommand.Text = diffExe + " {0}";
                }
            }
        }
        
        //=========================================================================================
        // 機　能：ディレクトリで参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDiffDirectoryRef_Click(object sender, EventArgs evt) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Resources.OptionInstallDiffRefTitle;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            ofd.Filter = Resources.OptionInstallDiffRefFilter;
            ofd.RestoreDirectory = true;
            DialogResult dr = ofd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                string diffExe = ofd.FileName;
                if (diffExe.IndexOf(' ') != -1) {
                    this.textBoxDiffDirectoryCommand.Text = "\"" + diffExe + "\" {0}";
                } else {
                    this.textBoxDiffDirectoryCommand.Text = diffExe + " {0}";
                }
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // 差分ツールのコマンドライン
            string diff = this.textBoxDiffCommand.Text;
            success = Configuration.CheckDiffCommandLine(ref diff, m_parent);
            if (!success) {
                return false;
            }

            string diffDir = this.textBoxDiffDirectoryCommand.Text;
            success = Configuration.CheckDiffDirectoryCommandLine(ref diff, m_parent);
            if (!success) {
                return false;
            }

            // Configに反映
            Configuration.Current.DiffCommandLine = diff;
            Configuration.Current.DiffDirectoryCommandLine = diffDir;

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
