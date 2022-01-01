using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：ファイルビューアの比較成功時の削除確認ダイアログ
    //=========================================================================================
    public partial class ViewerCompareSuccessDialog : Form {
        // 終了時にバッファを削除するときtrue
        private bool m_deleteBuffer;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]buffer   終了時にバッファを削除するときtrue
        // 戻り値：なし
        //=========================================================================================
        public ViewerCompareSuccessDialog(bool deleteBuffer) {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Information.ToBitmap();
            if (deleteBuffer) {
                this.radioButtonDispose.Checked = true;
            } else {
                this.radioButtonNextUse.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：[閉じる]ボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            if (this.radioButtonDispose.Checked) {
                m_deleteBuffer = true;
            } else {
                m_deleteBuffer = false;
            }
        }

        //=========================================================================================
        // プロパティ：終了時にバッファを削除するときtrue
        //=========================================================================================
        public bool DeleteBuffer {
            get {
                return m_deleteBuffer;
            }
        }
    }
}
