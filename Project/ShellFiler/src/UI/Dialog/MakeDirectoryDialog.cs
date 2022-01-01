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

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ディレクトリの作成ダイアログ
    //=========================================================================================
    public partial class MakeDirectoryDialog : Form {
        // 作成後、カレントを移動するときtrueのデフォルト
        public const bool MOVE_CURRENT_DEFAULT = true;

        // 新しく作成するディレクトリ
        private string m_newDirectoryName;

        // 作成後、カレントを移動するときtrue
        private bool m_isMoveCurrent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MakeDirectoryDialog() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]current    カレントディレクトリ
        // 　　　　[in]fileSystem ディレクトリ作成する先のファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(string current, IFileSystem fileSystem) {
            this.textBoxCurrent.Text = current;
            if (FileSystemID.IsWindows(fileSystem.FileSystemId)) {
                this.textBoxDirName.Text = Configuration.Current.MakeDirectoryNewWindowsName;
            } else if (FileSystemID.IsSSH(fileSystem.FileSystemId)) {
                this.textBoxDirName.Text = Configuration.Current.MakeDirectoryNewSSHName;
            } else {
                FileSystemID.NotSupportError(fileSystem.FileSystemId);
            }
            this.checkBoxMoveCurrent.Checked = GetMakeDirectoryMoveCurrentDefault();
        }

        //=========================================================================================
        // 機　能：ディレクトリ作成後にカレントを移動するかどうかのデフォルト設定を取得する
        // 引　数：なし
        // 戻り値：ディレクトリ作成後にカレントを移動するときtrue
        //=========================================================================================
        private bool GetMakeDirectoryMoveCurrentDefault() {
            bool result;
            BooleanFlag moveCurrent = Configuration.Current.MakeDirectoryMoveCurrentDefault;
            if (moveCurrent == null) {
                result = Program.Document.UserGeneralSetting.MakeDirectoryMoveCurrent;
            } else {
                result = moveCurrent.Value;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_newDirectoryName = this.textBoxDirName.Text;
            m_isMoveCurrent = this.checkBoxMoveCurrent.Checked;
            DialogResult = DialogResult.OK;
        }

        //=========================================================================================
        // プロパティ：新しく作成するディレクトリ名
        //=========================================================================================
        public string NewDirectoryName {
            get {
                return m_newDirectoryName;
            }
        }

        //=========================================================================================
        // プロパティ：作成後、カレントを移動するときtrue
        //=========================================================================================
        public bool IsMoveCurrent {
            get {
                return m_isMoveCurrent;
            }
        }
    }
}
