using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：ファイルビューアの指定行番号へジャンプダイアログ
    //=========================================================================================
    public partial class ViewerJumpLineDialog : Form {
        // ダイアログ位置の親フォームとの間のマージン
        public const int DIALOG_POSITION_MARGIN = 10;

        // ジャンプ先行番号（不正値が入力されたとき-1）
        private int m_lineNo = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]lineNo   初期状態の行番号
        // 　　　　[in]initial  入力状態にする場合、はじめに入力する数値（-1のときはlineNoで初期化）
        // 戻り値：なし
        //=========================================================================================
        public ViewerJumpLineDialog(int lineNo, int initial) {
            InitializeComponent();
            m_lineNo = lineNo;
            if (initial == -1) {
                this.textBoxLineNo.Text = "" + m_lineNo;
            } else {
                string line = "" + initial;
                this.textBoxLineNo.Text = line;
                this.textBoxLineNo.Select(line.Length, 0);
            }
        }

        //=========================================================================================
        // 機　能：OKボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            int num;
            bool success = int.TryParse(this.textBoxLineNo.Text, out num);
            if (!success || num <= 0) {
                InfoBox.Warning(this, Resources.DlgViewerJumpLine_NumberError);
                m_lineNo = -1;
                return;
            }
            m_lineNo = num;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ViewerJumpLineDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_lineNo == -1) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：入力した行番号
        //=========================================================================================
        public int LineNo {
            get {
                return m_lineNo;
            }
        }
    }
}
