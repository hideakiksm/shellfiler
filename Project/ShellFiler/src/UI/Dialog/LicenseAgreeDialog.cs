using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：使用許諾ダイアログ
    //=========================================================================================
    public partial class LicenseAgreeDialog : Form {
        // 現在のライセンスバージョン
        private const int CURRENT_LICENSE_VERSION = 1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]isUpdate   使用許諾が更新されたときtrue
        // 戻り値：なし
        //=========================================================================================
        public LicenseAgreeDialog(bool isUpdate) {
            InitializeComponent();

            string url = "res://" + Assembly.GetExecutingAssembly().Location + "/";//      + NativeResources.HtmlLicenseAgree;
            this.webBrowser.Navigate(url);
            if (isUpdate) {
                this.labelTitle.Text = Resources.DlgLicenseAgree_HeaderTitleUpdate;
            } else {
                this.labelTitle.Text = Resources.DlgLicenseAgree_HeaderTitleNew;
            }
        }

        //=========================================================================================
        // 機　能：必要によりライセンスダイアログを表示する
        // 引　数：[in]parentForm  ダイアログの親となるフォーム
        // 戻り値：初期化を継続してよいときtrue
        //=========================================================================================
        public static bool ShowLicenseDialog(Form parentForm) {
            // 使用許諾ファイルを確認
            int licenseVersion = -1;
            try {
                string[] lines = File.ReadAllLines(DirectoryManager.LicenseAgreeFile);
                if (lines.Length > 0) {
                    licenseVersion = int.Parse(lines[0]);
                }
            } catch (Exception) {
            }

            if (licenseVersion < CURRENT_LICENSE_VERSION) {
                // ダイアログを表示
                bool update = (licenseVersion != -1);
                LicenseAgreeDialog dialog = new LicenseAgreeDialog(update);
                DialogResult result = dialog.ShowDialog(parentForm);
                if (result != DialogResult.OK) {
                    return false;
                }

                // 使用許諾ファイルを保存
                DirectoryManager.CreateTemporary();
                string[] newLines = new string[1] { CURRENT_LICENSE_VERSION.ToString() };
                try {
                    File.WriteAllLines(DirectoryManager.LicenseAgreeFile, newLines);
                } catch (Exception e) {
                    InfoBox.Warning(parentForm, Resources.DlgLicenseAgree_SaveFile, DirectoryManager.LicenseAgreeFile, e.Message);
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LicenseAgreeDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            this.webBrowser.Dispose();
        }

        //=========================================================================================
        // 機　能：OKボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.radioButtonAgree.Checked) {
                DialogResult = DialogResult.OK;
            } else {
                DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgLicenseAgree_DonotAgree);
                if (yesNo == DialogResult.Yes) {
                    DialogResult = DialogResult.Cancel;
                } else {
                    return;
                }
            }
            Close();
        }
    }
}
