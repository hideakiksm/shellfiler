using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：設定ファイル内警告メッセージ表示ダイアログ
    //=========================================================================================
    public partial class SettingFileWarningDialog : Form {
        // 設定フォルダ
        private string m_settingFolder;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName     設定ファイル名
        // 　　　　[in]warningList  発生した警告情報のリスト
        // 戻り値：なし
        //=========================================================================================
        public SettingFileWarningDialog(List<string> fileNameList, List<SettingLoader.Warning> warningList) {
            InitializeComponent();

            this.pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
            m_settingFolder = GenericFileStringUtils.GetDirectoryName(fileNameList[0]);
            foreach (string fileName in fileNameList) {
                this.listBoxFileName.Items.Add(fileName);
            }

            // 警告メッセージを整形
            List<SettingLoader.WarningGroup> groupList = new List<SettingLoader.WarningGroup>();
            foreach (SettingLoader.Warning warning in warningList) {
                if (!groupList.Contains(warning.Group)) {
                    groupList.Add(warning.Group);
                }
            }
            StringBuilder sbMessage = new StringBuilder();
            foreach (SettingLoader.WarningGroup group in groupList) {
                if (sbMessage.Length > 0) {
                    sbMessage.AppendLine();
                }
                sbMessage.Append(Resources.DlgSettingWarn_EyeCatch);
                sbMessage.AppendLine(group.DisplayName);
                foreach (SettingLoader.Warning warning in warningList) {
                    if (group == warning.Group) {
                        sbMessage.AppendLine(warning.Message);
                    }
                }
            }
            this.textBoxInfo.Text = sbMessage.ToString();
        }

        //=========================================================================================
        // 機　能：開くボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOpen_Click(object sender, EventArgs evt) {
            try {
                Process process = OSUtils.ProcessStartCommandLine(m_settingFolder, m_settingFolder);
                if (process != null) {
                    process.Dispose();
                }
            } catch (Exception) {
                InfoBox.Warning(this, Resources.DlgSettingWarn_FailedFolderOpen);
            }
        }
    }
}
