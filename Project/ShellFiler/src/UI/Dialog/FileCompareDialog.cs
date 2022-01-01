using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルコンペアダイアログ
    //=========================================================================================
    public partial class FileCompareDialog : Form {
        // ファイルコンペアの設定（コンストラクタとは別インスタンス）
        private FileCompareSetting m_setting;

        // 対象パスの一覧
        private FileListView m_targetFileList;

        // 反対パスの一覧
        private FileListView m_oppositeFileList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting   ファイルコンペアの設定
        // 　　　　[in]target    対象パスの一覧
        // 　　　　[in]opposite  反対パスの一覧
        // 戻り値：なし
        //=========================================================================================
        public FileCompareDialog(FileCompareSetting setting, FileListView target, FileListView opposite) {
            InitializeComponent();
            m_targetFileList = target;
            m_oppositeFileList = opposite;

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
                default:
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
                default:
                    this.radioButtonSizeIgnore.Checked = true;
                    break;
            }
            this.checkBoxExceptFolder.Checked = setting.ExceptFolder;
//            if (setting.CheckContents && FileSystemID.IsWindows(target.FileList.FileSystem.FileSystemId) && FileSystemID.IsWindows(opposite.FileList.FileSystem.FileSystemId)) {
//                this.checkBoxCheckContents.Checked = true;
//            } else {
//                this.checkBoxCheckContents.Checked = false;
//            }
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileCompareDialog_KeyDown(object sender, KeyEventArgs evt) {
            if (evt.KeyCode == Keys.A) {
                buttonExactly_Click(null, null);
            } else if (evt.KeyCode == Keys.N) {
                buttonNameOnly_Click(null, null);
            }
        }

        //=========================================================================================
        // 機　能：完全一致のボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonExactly_Click(object sender, EventArgs evt) {
            this.radioButtonTimeSame.Checked = true;
            this.radioButtonSizeSame.Checked = true;
        }

        //=========================================================================================
        // 機　能：名前のみのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNameOnly_Click(object sender, EventArgs evt) {
            this.radioButtonTimeIgnore.Checked = true;
            this.radioButtonSizeIgnore.Checked = true;
        }

        //=========================================================================================
        // 機　能：OKボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_setting = new FileCompareSetting();
            if (this.radioButtonTimeSame.Checked) {
                m_setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.MarkExactly;
            } else if (this.radioButtonTimeNew.Checked) {
                m_setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.MarkNewer;
            } else if (this.radioButtonTimeOld.Checked) {
                m_setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.MarkOlder;
            } else {
                m_setting.FileTimeMode = FileCompareSetting.FileTimeCompareMode.Ignore;
            }
            if (this.radioButtonSizeSame.Checked) {
                m_setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.MarkExactly;
            } else if (this.radioButtonSizeBig.Checked) {
                m_setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.MarkBigger;
            } else if (this.radioButtonSizeSmall.Checked) {
                m_setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.MarkSmaller;
            } else {
                m_setting.FileSizeMode = FileCompareSetting.FileSizeCompareMode.Ignore;
            }
            m_setting.ExceptFolder = this.checkBoxExceptFolder.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：ファイルコンペアの設定（コンストラクタとは別インスタンス）
        //=========================================================================================
        public FileCompareSetting FileCompareSetting {
            get {
                return m_setting;
            }
        }
    }
}
