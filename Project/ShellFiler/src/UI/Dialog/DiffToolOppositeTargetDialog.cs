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
    // クラス：差分表示ツール 同名/マークファイル選択ダイアログ
    //=========================================================================================
    public partial class DiffToolOppositeTargetDialog : Form {
        // 比較対象として同名ファイルが選択されたときtrue
        private bool m_selectName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]target   対象パスのファイル名
        // 　　　　[in]oppName  反対パスの同名ファイル名
        // 　　　　[in]oppMark  反対パスのマークファイル名
        // 戻り値：なし
        //=========================================================================================
        public DiffToolOppositeTargetDialog(string target, string oppName, string oppMark) {
            InitializeComponent();

            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
            this.textBoxTarget.Text = target;
            this.textBoxOppositeName.Text = oppName;
            this.textBoxOppositeMark.Text = oppMark;

            this.radioButtonName.Checked = true;
            this.ActiveControl = this.radioButtonName;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.radioButtonName.Checked) {
                m_selectName = true;
            } else {
                m_selectName = false;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：比較対象として同名ファイルが選択されたときtrue
        //=========================================================================================
        public bool SelectName {
            get {
                return m_selectName;
            }
        }
    }
}
