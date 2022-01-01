using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル一覧＞ファイルコンペア の設定ページ
    //=========================================================================================
    public partial class FileListComparePage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListComparePage(OptionSettingDialog parent) {
            InitializeComponent();

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
            RadioButtonPrevFix_CheckedChanged(null, null);

            // イベントを接続
            this.radioButtonPrev.CheckedChanged += new EventHandler(RadioButtonPrevFix_CheckedChanged);
            this.radioButtonFix.CheckedChanged += new EventHandler(RadioButtonPrevFix_CheckedChanged);
            this.buttonExactly.Click += new EventHandler(buttonExactly_Click);
            this.buttonNameOnly.Click += new EventHandler(buttonNameOnly_Click);
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
            FileCompareSetting setting;
            if (config.FileCompareSettingDefault == null) {
                setting = new FileCompareSetting();
                this.radioButtonPrev.Checked = true;
            } else {
                setting = (FileCompareSetting)(config.FileCompareSettingDefault.Clone());
                this.radioButtonFix.Checked = true;
            }

            switch (setting.FileTimeMode) {
                case FileCompareSetting.FileTimeCompareMode.MarkExactly:
                    this.radioButtonTimeSame.Checked = true;
                    break;
                case FileCompareSetting.FileTimeCompareMode.MarkNewer:
                    this.radioButtonTimeNew.Checked = true;
                    break;
                case FileCompareSetting.FileTimeCompareMode.MarkOlder:
                    this.radioButtonTimeOld.Checked = true;
                    break;
                case FileCompareSetting.FileTimeCompareMode.Ignore:
                    this.radioButtonTimeIgnore.Checked = true;
                    break;
            }
            switch (setting.FileSizeMode) {
                case FileCompareSetting.FileSizeCompareMode.MarkExactly:
                    this.radioButtonSizeSame.Checked = true;
                    break;
                case FileCompareSetting.FileSizeCompareMode.MarkBigger:
                    this.radioButtonSizeBig.Checked = true;
                    break;
                case FileCompareSetting.FileSizeCompareMode.MarkSmaller:
                    this.radioButtonSizeSmall.Checked = true;
                    break;
                case FileCompareSetting.FileSizeCompareMode.Ignore:
                    this.radioButtonSizeIgnore.Checked = true;
                    break;
            }
            this.checkBoxExceptFolder.Checked = setting.ExceptFolder;
        }

        //=========================================================================================
        // 機　能：前回値/固定値のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            bool enabled = (this.radioButtonFix.Checked);
            this.radioButtonTimeSame.Enabled = enabled;
            this.radioButtonTimeNew.Enabled = enabled;
            this.radioButtonTimeOld.Enabled = enabled;
            this.radioButtonTimeIgnore.Enabled = enabled;
            this.radioButtonSizeSame.Enabled = enabled;
            this.radioButtonSizeBig.Enabled = enabled;
            this.radioButtonSizeSmall.Enabled = enabled;
            this.radioButtonSizeIgnore.Enabled = enabled;
            this.checkBoxExceptFolder.Enabled = enabled;
            this.buttonExactly.Enabled = enabled;
            this.buttonNameOnly.Enabled = enabled;
        }

        //=========================================================================================
        // 機　能：同一ファイルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void buttonExactly_Click(object sender, EventArgs evt) {
            this.radioButtonTimeSame.Checked = true;
            this.radioButtonSizeSame.Checked = true;
        }

        //=========================================================================================
        // 機　能：ファイル名のみボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void buttonNameOnly_Click(object sender, EventArgs evt) {
            this.radioButtonTimeIgnore.Checked = true;
            this.radioButtonSizeIgnore.Checked = true;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            FileCompareSetting setting;
            if (this.radioButtonPrev.Checked) {
                setting = null;
            } else {
                setting = new FileCompareSetting();
                if (this.radioButtonTimeSame.Checked) {
                    setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.MarkExactly;
                } else if (this.radioButtonTimeNew.Checked) {
                    setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.MarkNewer;
                } else if (this.radioButtonTimeOld.Checked) {
                    setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.MarkOlder;
                } else if (this.radioButtonTimeIgnore.Checked) {
                    setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.Ignore;
                }
                if (this.radioButtonSizeSame.Checked) {
                    setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.MarkExactly;
                } else if (this.radioButtonSizeBig.Checked) {
                    setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.MarkBigger;
                } else if (this.radioButtonSizeSmall.Checked) {
                    setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.MarkSmaller;
                } else if (this.radioButtonSizeIgnore.Checked) {
                    setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.Ignore;
                }
                setting.ExceptFolder = this.checkBoxExceptFolder.Checked;
            }
            Configuration.Current.FileCompareSettingDefault = setting;
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
