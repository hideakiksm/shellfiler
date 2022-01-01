using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：リクエスト電文でファイルの参照を行うときのコントロール
    //=========================================================================================
    public partial class HttpViewerFileRefControl : UserControl {
        // 親フォーム
        private HttpViewerRequestDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親フォーム
        // 戻り値：なし
        //=========================================================================================
        public HttpViewerFileRefControl(HttpViewerRequestDialog parent) {
            InitializeComponent();
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]filePath   設定するファイルのパス（初期値を使用するときnull）
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(string filePath) {
            if (filePath == null) {
                this.textBoxFilePath.Text = "";
            } else {
                this.textBoxFilePath.Text = filePath;
            }
        }

        //=========================================================================================
        // 機　能：ファイルの参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRef_Click(object sender, EventArgs evt) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.InitialDirectory = m_parent.TargetFilePath;
            ofd.Title = Resources.DlgHttpView_OpenFileRequestBody;
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            DialogResult result = ofd.ShowDialog();
            if (result != DialogResult.OK) {
                return;
            }
            this.textBoxFilePath.Text = ofd.FileName;
        }

        //=========================================================================================
        // 機　能：入力結果を取得する
        // 引　数：[out]bodyFileName  ファイル名を返す変数
        // 　　　　[out]requestBody   リクエスト内容のバイナリデータを返す変数
        // 戻り値：入力を正しく取得できたときtrue
        //=========================================================================================
        public bool GetResult(out string bodyFileName, out byte[] requestBody) {
            bodyFileName = this.textBoxFilePath.Text;
            requestBody = null;
            if (bodyFileName == "") {
                InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckNoRefFile);
                return false;
            }
            try {
                requestBody = File.ReadAllBytes(bodyFileName);
            } catch (Exception) {
                InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckCannotAccessRefFile);
                return false;
            }
            return true;
        }
    }
}
