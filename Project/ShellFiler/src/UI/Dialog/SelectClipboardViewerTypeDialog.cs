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
    // クラス：クリップボードビューア形式選択ダイアログ
    //=========================================================================================
    public partial class SelectClipboardViewerTypeDialog : Form {
        // 画像を読み込むときtrue
        private bool m_isImage;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SelectClipboardViewerTypeDialog() {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
        }

        //=========================================================================================
        // 機　能：画像ボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonImage_Click(object sender, EventArgs evt) {
            m_isImage = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：テキストボタンクリック時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonText_Click(object sender, EventArgs evt) {
            m_isImage = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：画像を読み込むときtrue
        //=========================================================================================
        public bool IsImage {
            get {
                return m_isImage;
            }
        }
    }
}
