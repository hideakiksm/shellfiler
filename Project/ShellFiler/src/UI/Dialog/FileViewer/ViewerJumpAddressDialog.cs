using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：ファイルビューアの指定アドレスへジャンプダイアログ
    //=========================================================================================
    public partial class ViewerJumpAddressDialog : Form {
        // ダイアログ位置の親フォームとの間のマージン
        public const int DIALOG_POSITION_MARGIN = 10;

        // ジャンプ先アドレス（不正値が入力されたとき-1）
        private int m_address;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]address   初期状態の行番号
        // 　　　　[in]initial  入力状態にする場合、はじめに入力する数値（-1のときはaddressで初期化）
        // 戻り値：なし
        //=========================================================================================
        public ViewerJumpAddressDialog(int address, int initial) {
            InitializeComponent();
            if (initial == -1) {
                this.textBoxAddress.Text = m_address.ToString("X");
            } else {
                string line = initial.ToString("X");
                this.textBoxAddress.Text = line;
                this.textBoxAddress.Select(line.Length, 0);
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
            try {
                num = int.Parse(this.textBoxAddress.Text, NumberStyles.HexNumber);
            } catch (Exception) {
                InfoBox.Warning(this, Resources.DlgViewerJumpAddress_NumberError);
                m_address = -1;
                return;
            }
            m_address = num;
            DialogResult = DialogResult.OK;
            Close();
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ViewerJumpAddressDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_address == -1) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：入力したアドレス
        //=========================================================================================
        public int Address {
            get {
                return m_address;
            }
        }
    }
}
