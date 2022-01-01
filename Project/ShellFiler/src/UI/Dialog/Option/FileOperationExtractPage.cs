using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞展開 の設定ページ
    //=========================================================================================
    public partial class FileOperationExtractPage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationExtractPage(OptionSettingDialog parent) {
            InitializeComponent();

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
            if (config.ArchiveExtractPathMode == ExtractPathMode.Direct) {
                this.radioButtonDirect.Checked = true;
            } else if (config.ArchiveExtractPathMode == ExtractPathMode.AlwaysNewDirectory) {
                this.radioButtonAlwaysNew.Checked = true;
            } else {
                this.radioButtonSameExist.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            ExtractPathMode mode;
            if (this.radioButtonDirect.Checked) {
                mode = ExtractPathMode.Direct;
            } else if (this.radioButtonAlwaysNew.Checked) {
                mode = ExtractPathMode.AlwaysNewDirectory;
            } else {
                mode = ExtractPathMode.NewDirectoryIsSameExist;
            }

            Configuration.Current.ArchiveExtractPathMode = mode;
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
