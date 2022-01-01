using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：リクエスト電文でテキスト入力を行うときのコントロール
    //=========================================================================================
    public partial class HttpViewerTextControl : UserControl {
        // 親フォーム
        private HttpViewerRequestDialog m_parent;

        // サンプルのテキスト（無効にするときnull）
        private string m_sampleText = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親フォーム
        // 戻り値：なし
        //=========================================================================================
        public HttpViewerTextControl(HttpViewerRequestDialog parent) {
            InitializeComponent();
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]text          設定するテキスト（初期値を使用するときnull）
        // 　　　　[in]encoding      設定するエンコード（初期値を使用するときnull）
        // 　　　　[in]sample        サンプル電文（サンプルを無効にするときnull）
        // 　　　　[in]enableContent Content-Lengthを有効にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(string text, EncodingType encoding, string sample, bool enableContent) {
            m_sampleText = sample;
            
            // テキスト
            if (text != null) {
                this.textBoxRequestText.Text = text;
            } else {
                this.textBoxRequestText.Text = "";
            }

            // エンコーディング
            EncodingType[] encodingList = EncodingType.HttpRequestEncoding;
            string[] encodingNameList = new string[encodingList.Length];
            int encodingIndex = 0;
            for (int i = 0; i < encodingList.Length; i++) {
                encodingNameList[i] = encodingList[i].DisplayName;
                if (encoding == encodingList[i]) {
                    encodingIndex = i;
                }
            }
            this.comboBoxEncoding.Items.AddRange(encodingNameList);
            this.comboBoxEncoding.SelectedIndex = encodingIndex;

            // サンプル
            if (sample == null) {
                this.buttonSample.Visible = false;
            }

            // Content-Length
            this.buttonLength.Visible = enableContent;
        }
        
        //=========================================================================================
        // 機　能：サンプルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSample_Click(object sender, EventArgs evt) {
            this.textBoxRequestText.Text = m_sampleText;
        }
        
        //=========================================================================================
        // 機　能：Content-Lengthボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonContentLength_Click(object sender, EventArgs evt) {
            string text;
            EncodingType encoding;
            byte[] requestBody;
            GetResult(out text, out encoding, out requestBody);

            // bodyを識別
            const byte CR = 0x0d;
            const byte LF = 0x0a;
            byte[] crlfcrlfBytes = new byte[] { CR, LF, CR, LF };
            int idxBody = StringUtils.FindBytes(requestBody, crlfcrlfBytes, 0);
            if (idxBody == -1) {
                InfoBox.Warning(m_parent, Resources.DlgHttpConnect_ContentBody);
                return;
            }

            // Content-Lengthを作成
            idxBody += 4;
            int bodyLength = requestBody.Length - idxBody;
            byte[] lengthBytes = encoding.Encoding.GetBytes(bodyLength.ToString());

            // Content-Lengthを識別
            byte[] contentBytes = encoding.Encoding.GetBytes("\r\nContent-Length: ");
            int idxContent = StringUtils.FindBytes(requestBody, contentBytes, 0);
            if (idxContent == -1 || idxContent > idxBody) {
                InfoBox.Warning(m_parent, Resources.DlgHttpConnect_ContentLength);
                return;
            }

            // Content-Lengthの行を識別
            byte[] crlfBytes = new byte[] { CR, LF };
            int idxContentLine = StringUtils.FindBytes(requestBody, crlfBytes, idxContent + 2);

            // 作成
            MemoryStream stream = new MemoryStream();
            stream.Write(requestBody, 0, idxContent + contentBytes.Length);
            stream.Write(lengthBytes, 0, lengthBytes.Length);
            stream.Write(requestBody, idxContentLine, requestBody.Length - idxContentLine);
            stream.Close();
            byte[] resultBytes = stream.ToArray();
            text = encoding.Encoding.GetString(resultBytes);
            this.textBoxRequestText.Text = text;

            InfoBox.Information(m_parent, Resources.DlgHttpConnect_ContentSuccess, bodyLength);
        }

        //=========================================================================================
        // 機　能：入力結果を取得する
        // 引　数：[out]bodyFileName  ファイル名を返す変数
        // 　　　　[out]requestBody   リクエスト内容のバイナリデータを返す変数
        // 戻り値：入力を正しく取得できたときtrue
        //=========================================================================================
        public bool GetResult(out string text, out EncodingType encoding, out byte[] requestBody) {
            text = this.textBoxRequestText.Text;
            EncodingType[] encodingList = EncodingType.HttpRequestEncoding;
            int encodingIndex = this.comboBoxEncoding.SelectedIndex;
            if (encodingIndex >= 0 && encodingIndex < encodingList.Length) {
                encoding = encodingList[encodingIndex];
            } else {
                encoding = EncodingType.UTF8;
            }
            requestBody = encoding.Encoding.GetBytes(text);
            return true;
        }
    }
}
