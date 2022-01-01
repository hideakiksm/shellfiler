using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：仮想フォルダの実行準備エラーダイアログ
    //=========================================================================================
    public partial class VirtualPrepareErrorDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualPrepareErrorDialog() {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
        }
    }
}
