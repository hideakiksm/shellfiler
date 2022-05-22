using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ShellFiler.Locale;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog
{
    //=========================================================================================
    // クラス：バージョン情報ダイアログ
    //=========================================================================================
    public partial class AboutDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AboutDialog() {
            InitializeComponent();

            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string version = string.Format("{0}.{1}.{2}", ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart);
            AssemblyCopyrightAttribute asmcpy = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute));
            string copyright = string.Format(asmcpy.Copyright);

            this.labelVersion.Text = string.Format(labelVersion.Text, version);
            this.labelCopyright.Text = copyright;
            this.textBoxLibrary.Text = Resources.DlgAbout_ExternalSoftware;

            this.pictureBoxLogo.Image = UIIconManager.TitleLogo;
            this.pictureBoxIcon.Image = UIIconManager.MainIcon48;

            this.ActiveControl = this.buttonClose;
        }

        //=========================================================================================
        // 機　能：ライセンスについてのリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            Process.Start(KnownUrl.ShellFilerUrl);
        }

        //=========================================================================================
        // 機　能：閉じるボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            Close();
        }
    }
}
