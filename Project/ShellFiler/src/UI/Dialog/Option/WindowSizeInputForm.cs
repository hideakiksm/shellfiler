using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：ウィンドウ初期サイズの設定フォーム
    //=========================================================================================
    public partial class WindowSizeInputForm : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public WindowSizeInputForm() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListInitialPageSizeForm_SizeChanged(object sender, EventArgs evt) {
            UpdateUI();
        }

        //=========================================================================================
        // 機　能：ウィンドウ位置が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListInitialPageSizeForm_LocationChanged(object sender, EventArgs evt) {
            UpdateUI();
        }

        //=========================================================================================
        // 機　能：UIの表示を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateUI() {
            string sizeDisp = string.Format(Resources.OptionWindowSizeForm, this.Left, this.Top, this.Right, this.Bottom);
            this.labelSize.Text = sizeDisp;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
