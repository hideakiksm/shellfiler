using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：選択範囲の比較対象を確認ダイアログ
    //=========================================================================================
    public partial class ViewerCompareDisplayDialog : Form {
        // 終了後、差分表示ツールを起動するときtrue
        private bool m_runDiffTool = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ViewerCompareDisplayDialog() {
            InitializeComponent();
            this.splitContainer.SplitterDistance = this.splitContainer.Width / 2;
            FileViewerSelectionCompareBuffer buffer = Program.Document.FileViewerSelectionCompareBuffer;
            if (buffer.LeftString != null) {
                this.textBoxLeft.Text = buffer.LeftString;
            } else {
                this.textBoxLeft.Text = Resources.DlgViewerCompare_NoText;
                this.buttonLeftClear.Enabled = false;
            }
            if (buffer.RightString != null) {
                this.textBoxRight.Text = buffer.RightString;
            } else {
                this.textBoxRight.Text = Resources.DlgViewerCompare_NoText;
                this.buttonRightClear.Enabled = false;
            }
            this.buttonDiffTool.Enabled = (buffer.LeftString != null && buffer.RightString != null);
        }

        //=========================================================================================
        // 機　能：左側テキストの消去ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonLeftClear_Click(object sender, EventArgs evt) {
            FileViewerSelectionCompareBuffer buffer = Program.Document.FileViewerSelectionCompareBuffer;
            buffer.LeftString = null;
            buffer.LeftStartLineNum = -1;
            this.textBoxLeft.Text = Resources.DlgViewerCompare_NoText;
            this.buttonLeftClear.Enabled = false;
        }

        //=========================================================================================
        // 機　能：右側テキストの消去ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRightClear_Click(object sender, EventArgs evt) {
            FileViewerSelectionCompareBuffer buffer = Program.Document.FileViewerSelectionCompareBuffer;
            buffer.RightString = null;
            buffer.RightStartLineNum = -1;
            this.textBoxRight.Text = Resources.DlgViewerCompare_NoText;
            this.buttonRightClear.Enabled = false;
        }

        //=========================================================================================
        // 機　能：差分表示ツールで比較ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDiffTool_Click(object sender, EventArgs evt) {
            m_runDiffTool = true;
            Close();
        }

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            m_runDiffTool = false;
            Close();
        }

        //=========================================================================================
        // プロパティ：終了後、差分表示ツールを起動するときtrue
        //=========================================================================================
        public bool RunDiffTool {
            get {
                return m_runDiffTool;
            }
        }
    }
}
