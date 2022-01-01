using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：想定外エラー表示ダイアログ
    //=========================================================================================
    public partial class UnexpectedExceptionDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]detail     詳細情報
        // 　　　　[in]message    メッセージ
        // 　　　　[in]param      メッセージに付加するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public UnexpectedExceptionDialog(bool autoClose, string detail, string message, params object[] param) {
            InitializeComponent();

            if (autoClose) {
                this.labelManualClose.Visible = false;
            } else {
                this.labelAutoClose.Visible = false;
            }
            string mainMessage = string.Format(message, param);
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string version = "Version: " + ver.FileMajorPart + "." + ver.FileMinorPart + "." + ver.FileBuildPart;
            string detailText = mainMessage + "\r\n" + version +  "\r\n" + detail;

            this.labelMessage.Text = mainMessage;
            this.textBoxDetail.Text = detailText;
            try {
                this.pictureBoxIcon.Image = SystemIcons.Error.ToBitmap();
            } catch (TargetInvocationException) {
                // リソース不足
                GC.Collect();
            }
        }
    }
}
