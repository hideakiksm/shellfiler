using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ShellFiler.FileViewer.HTTPResponseViewer;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：HTTPレスポンスビューアのリクエスト編集用ダイアログ
    //=========================================================================================
    public partial class HttpViewerRequestDialog : Form {
        // HTTPモードの実装ページ
        private HTTPPage m_httpPage = null;
        
        // TCPモードの実装ページ
        private TCPPage m_tcpPage = null;

        // ダイアログの入力結果（入力が未確定のときnull）
        private ResponseViewerRequest m_responseViewerRequest;

        // 対象パスのフォルダ名（Windows以外の場合は初期フォルダ）
        private string m_targetFilePath;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]defaultSetting  はじめに表示される設定
        // 　　　　[in]targetFilePath  対象パスのフォルダ名（Windows以外の場合は初期フォルダ）
        // 戻り値：なし
        //=========================================================================================
        public HttpViewerRequestDialog(ResponseViewerRequestSetting defaultSetting, string targetFilePath) {
            InitializeComponent();
            m_targetFilePath = targetFilePath;
            m_httpPage = new HTTPPage(this, defaultSetting.HttpModeRequest);
            m_tcpPage = new TCPPage(this, defaultSetting.TcpModeRequest);
            if (defaultSetting.SelectedMode == ResponseViewerMode.HttpMode) {
                this.tabControl.SelectedIndex = 0;
            } else {
                this.tabControl.SelectedIndex = 1;
            }
        }

        //=========================================================================================
        // 機　能：フォームがクローズされようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void HttpViewerRequestDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            if (DialogResult == DialogResult.OK && m_responseViewerRequest == null) {
                evt.Cancel = true;
                return;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.tabControl.SelectedIndex == 0) {
                m_responseViewerRequest = m_httpPage.GetResult();
            } else {
                m_responseViewerRequest = m_tcpPage.GetResult();
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        
        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        //=========================================================================================
        // プロパティ：ダイアログの入力結果（入力が未確定のときnull）
        //=========================================================================================
        public ResponseViewerRequest ResponseViewerRequest {
            get {
                return m_responseViewerRequest;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのフォルダ名（Windows以外の場合は初期フォルダ）
        //=========================================================================================
        public string TargetFilePath {
            get {
                return m_targetFilePath;
            }
        }

        //=========================================================================================
        // クラス：HTTPモードの実装
        //=========================================================================================
        private class HTTPPage {
            // 親ダイアログ
            private HttpViewerRequestDialog m_parent;

            // テキスト入力の詳細パネル
            private HttpViewerTextControl m_controlText;

            // ファイル参照入力の詳細パネル
            private HttpViewerFileRefControl m_controlFileRef;

            // GridViewのボールドフォント
            private Font m_boldFont = null;
            
            // listViewHTTPHeaderのTag
            // ・既定のヘッダのとき、HttpHeaderItem
            // ・独自ヘッダのとき、null

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent   親ダイアログ
            // 　　　　[in]setting  デフォルトに使用する設定
            // 戻り値：なし
            //=========================================================================================
            public HTTPPage(HttpViewerRequestDialog parent, ResponseViewerHttpRequest setting) {
                m_parent = parent;

                // リクエストURL
                m_parent.textBoxHTTPUrl.Text = setting.RequestUrl;

                // プロキシ
                if (setting.ProxyUrl == null) {
                    m_parent.checkBoxHTTPProxyUrl.Checked = false;
                    m_parent.textBoxHTTPProxyUrl.Enabled = false;
                    m_parent.textBoxHTTPProxyUrl.Text = "";
                } else {
                    m_parent.checkBoxHTTPProxyUrl.Checked = true;
                    m_parent.textBoxHTTPProxyUrl.Enabled = true;
                    m_parent.textBoxHTTPProxyUrl.Text = setting.ProxyUrl;
                }

                // メソッド
                string[] methodList = new string[] { "GET", "HEAD", "POST", "PUT", "DELETE", "CONNECT", "OPTIONS", "TRACE", "LINK", "UNLINK" }; 
                m_parent.comboBoxHTTPMethod.Items.AddRange(methodList);
                m_parent.comboBoxHTTPMethod.Text = setting.RequestMethod;
                m_parent.checkBoxAutoContent.Checked = true;

                // ヘッダ
                Dictionary<string, string> settingHeaderValue = new Dictionary<string, string>();
                for (int i = 0; i < setting.HttpHeader.Count; i++) {
                    settingHeaderValue.Add(setting.HttpHeader[i].Key, setting.HttpHeader[i].Value);
                }
                foreach (HTTPHeaderItem headerItem in HTTPHeaderItem.AllItems) {
                    bool useHeader = false;
                    bool allReadonly = false;
                    string cellName = headerItem.HTTPHeaderName;
                    string cellValue;
                    if (headerItem == HTTPHeaderItem.ContentLength) {
                        // Content-Lengthは自動設定
                        cellValue = Resources.DlgHttpView_ContentLength;
                        useHeader = true;
                        allReadonly = true;
                    } else if (settingHeaderValue.ContainsKey(headerItem.HTTPHeaderName)) {
                        cellValue = settingHeaderValue[headerItem.HTTPHeaderName];
                        useHeader = true;
                    } else {
                        cellValue = "";
                        useHeader = false;
                    }
                    DataGridViewRow gridRow = CreateGridRow(cellName, cellValue, useHeader, headerItem);
                    m_parent.dataGridView.Rows.Add(gridRow);
                    if (m_boldFont == null) {
                        m_boldFont = new Font(m_parent.Font, FontStyle.Bold);
                    }
                    if (allReadonly) {
                        gridRow.Cells[0].ReadOnly = true;
                        gridRow.Cells[1].ReadOnly = true;
                        gridRow.Cells[2].ReadOnly = true;
                        gridRow.Cells[1].Style.Font = m_boldFont;
                        gridRow.Cells[2].Style.Font = m_boldFont;
                    } else {
                        gridRow.Cells[1].ReadOnly = true;
                        gridRow.Cells[1].Style.Font = m_boldFont;
                    }
                }
                for (int i = 0; i < setting.HttpHeader.Count; i++) {
                    KeyValuePair<string, string> originalHeader = setting.HttpHeader[i];
                    if (HTTPHeaderItem.GetHeaderItemFromName(originalHeader.Key) == null) {
                        DataGridViewRow gridRow = CreateGridRow(originalHeader.Key, originalHeader.Value, true, null);
                        m_parent.dataGridView.Rows.Add(gridRow);
                    }
                }

                // リクエスト
                m_controlText = new HttpViewerTextControl(m_parent);
                m_controlFileRef = new HttpViewerFileRefControl(m_parent);
                if (setting.HttpBodyText != null) {
                    m_controlText.Initialize(setting.HttpBodyText, setting.HttpBodyEncodingType, null, false);
                    m_controlFileRef.Initialize(null);
                    m_parent.panelHTTPBody.Controls.Add(m_controlText);
                    m_parent.radioButtonHTTPBodyText.Checked = true;
                } else if (setting.HttpBodyFileName != null) {
                    m_controlText.Initialize(null, null, null, false);
                    m_controlFileRef.Initialize(setting.HttpBodyFileName);
                    m_parent.panelHTTPBody.Controls.Add(m_controlFileRef);
                    m_parent.radioButtonHTTPBodyFile.Checked = true;
                }

                // 最終調整
                EnableUIItem();
                comboBoxHTTPMethod_TextChanged(null, null);

                // イベントの接続
                m_parent.FormClosed += new FormClosedEventHandler(HttpViewerRequestDialog_FormClosed);
                m_parent.buttonHTTPParamEdit.Click += new EventHandler(buttonParamHTTPEdit_Click);
                m_parent.checkBoxHTTPProxyUrl.CheckedChanged += new EventHandler(checkBoxHTTPProxyUrl_CheckedChanged);
                m_parent.comboBoxHTTPMethod.TextChanged += new EventHandler(comboBoxHTTPMethod_TextChanged);
                m_parent.buttonHTTPHeaderAdd.Click += new EventHandler(buttonHTTPHeaderAdd_Click);
                m_parent.buttonHTTPHeaderDelete.Click += new EventHandler(buttonHTTPHeaderDelete_Click);
                m_parent.radioButtonHTTPBodyText.CheckedChanged += new EventHandler(radioButtonHTTPBody_CheckedChanged);
                m_parent.radioButtonHTTPBodyFile.CheckedChanged += new EventHandler(radioButtonHTTPBody_CheckedChanged);
            }

            //=========================================================================================
            // 機　能：HTTPヘッダのグリッドの行情報を作成する
            // 引　数：[in]cellName   HTTPヘッダの名前
            // 　　　　[in]cellValue  HTTPヘッダの値
            // 　　　　[in]useHeader  このヘッダを使用するとき（チェックボックスをONにするとき）true
            // 　　　　[in]headerItem HTTPヘッダの定義（独自ヘッダのときnull）
            // 戻り値：作成したグリッドの行オブジェクト
            //=========================================================================================
            private DataGridViewRow CreateGridRow(string cellName, string cellValue, bool useHeader, HTTPHeaderItem headerItem) {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell[] cells = new DataGridViewCell[3];
                cells[0] = new DataGridViewCheckBoxCell();
                cells[0].Value = useHeader;
                cells[1] = new DataGridViewTextBoxCell();
                cells[1].Value = cellName;
                cells[2] = new DataGridViewTextBoxCell();
                cells[2].Value = cellValue;
                gridRow.Cells.AddRange(cells);
                gridRow.Tag = headerItem;
                return gridRow;
            }

            //=========================================================================================
            // 機　能：フォームがクローズされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void HttpViewerRequestDialog_FormClosed(object sender, FormClosedEventArgs evt) {
                m_controlText.Dispose();
                m_controlText = null;
                m_controlFileRef.Dispose();
                m_controlFileRef = null;
                if (m_boldFont != null) {
                    m_boldFont.Dispose();
                    m_boldFont = null;
                }
            }

            //=========================================================================================
            // 機　能：UIの有効/無効状態を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                m_parent.textBoxHTTPProxyUrl.Enabled = m_parent.checkBoxHTTPProxyUrl.Checked;
            }

            //=========================================================================================
            // 機　能：HTTPパラメータの編集ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonParamHTTPEdit_Click(object sender, EventArgs evt) {
                HttpViewerRequestParamDialog dialog = new HttpViewerRequestParamDialog(m_parent.textBoxHTTPUrl.Text);
                DialogResult result = dialog.ShowDialog(m_parent);
                if (result != DialogResult.OK) {
                    return;
                }
                m_parent.textBoxHTTPUrl.Text = dialog.ResultText;
            }

            //=========================================================================================
            // 機　能：プロキシURLのチェック状態が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void checkBoxHTTPProxyUrl_CheckedChanged(object sender, EventArgs evt) {
                EnableUIItem();
            }
            
            //=========================================================================================
            // 機　能：メソッドのコンボボックスのテキストが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void comboBoxHTTPMethod_TextChanged(object sender, EventArgs evt) {
                string method = m_parent.comboBoxHTTPMethod.Text;
                bool bodyEnabled = true;
                if (method == "GET" || method == "HEAD") {
                    bodyEnabled = false;
                } else {
                    bodyEnabled = true;
                }
                m_parent.radioButtonHTTPBodyText.Enabled = bodyEnabled;
                m_parent.radioButtonHTTPBodyFile.Enabled = bodyEnabled;
                m_parent.panelHTTPBody.Enabled = bodyEnabled;

                // Contents-Typeの自動設定
                if (m_parent.checkBoxAutoContent.Checked) {
                    string cellValue = null;
                    if (method == "GET" || method == "HEAD") {
                        cellValue = ResponseViewerHttpRequest.CONTENTS_TYPE_GET;
                    } else if (method == "POST") {
                        cellValue = ResponseViewerHttpRequest.CONTENTS_TYPE_POST;
                    } else {
                        cellValue = null;
                    }
                    if (cellValue != null) {
                        for (int i = 0; i < m_parent.dataGridView.Rows.Count; i++) {
                            HTTPHeaderItem headerItem = (HTTPHeaderItem)(m_parent.dataGridView.Rows[i].Tag);
                            if (headerItem.HTTPHeaderName == "Content-Type") {
                                m_parent.dataGridView.Rows[i].Cells[2].Value = cellValue;
                                break;
                            }
                        }
                    }
                }
            }

            //=========================================================================================
            // 機　能：ヘッダの追加ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonHTTPHeaderAdd_Click(object sender, EventArgs evt) {
                DataGridViewRow gridRow = CreateGridRow("", "", true, null);
                m_parent.dataGridView.Rows.Add(gridRow);
                m_parent.dataGridView.CurrentCell = gridRow.Cells[1];
            }

            //=========================================================================================
            // 機　能：ヘッダの削除ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonHTTPHeaderDelete_Click(object sender, EventArgs evt) {
                int selectedIndex = m_parent.dataGridView.CurrentCell.RowIndex;
                if (m_parent.dataGridView.Rows[selectedIndex].Tag != null) {
                    InfoBox.Information(m_parent, Resources.DlgHttpView_KnownHeaderDelete);
                    return;
                }
                m_parent.dataGridView.Rows.RemoveAt(selectedIndex);
            }

            //=========================================================================================
            // 機　能：HTTPボディの選択用チェックボックスが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioButtonHTTPBody_CheckedChanged(object sender, EventArgs evt) {
                m_parent.panelHTTPBody.Controls.Clear();
                if (m_parent.radioButtonHTTPBodyText.Checked) {
                    m_parent.panelHTTPBody.Controls.Add(m_controlText);
                } else if (m_parent.radioButtonHTTPBodyFile.Checked) {
                    m_parent.panelHTTPBody.Controls.Add(m_controlFileRef);
                }
            }

            //=========================================================================================
            // 機　能：入力結果を返す
            // 引　数：なし
            // 戻り値：リクエストパラメータ（エラー等でキャンセルされたときnull）
            //=========================================================================================
            public ResponseViewerRequest GetResult() {
                bool success;

                // URL
                string url = m_parent.textBoxHTTPUrl.Text;
                if (url == "") {
                    InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckNoUrl);
                    return null;
                }

                // プロキシ
                string proxyUrl = null;
                if (m_parent.checkBoxHTTPProxyUrl.Checked) {
                    proxyUrl = m_parent.textBoxHTTPProxyUrl.Text;
                    try {
                        WebProxy proxyTest = new WebProxy(proxyUrl);
                    } catch (Exception) {
                        proxyUrl = "";
                    }
                    if (proxyUrl == "") {
                        InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckNoProxyUrl);
                        return null;
                    }
                }

                // HTTPヘッダ
                List<KeyValuePair<string, string>> headerList = new List<KeyValuePair<string, string>>();
                HashSet<string> headerDuplicateCheck = new HashSet<string>();
                int rowCount = m_parent.dataGridView.Rows.Count;
                for (int i = 0; i < rowCount; i++) {
                    DataGridViewRow row = m_parent.dataGridView.Rows[i];
                    if ((bool)(row.Cells[0].Value) != true) {
                        continue;
                    }
                    string name = (string)(row.Cells[1].Value);
                    string value = (string)(row.Cells[2].Value);
                    if (headerDuplicateCheck.Contains(name)) {
                        InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckHttpHeaderDuplicate, name);
                        return null;
                    }
                    if (row.Tag == HTTPHeaderItem.ContentLength) {
                        continue;
                    }
                    headerDuplicateCheck.Add(name);
                    headerList.Add(new KeyValuePair<string, string>(name, value));
                }

                // メソッド
                string method = m_parent.comboBoxHTTPMethod.Text;
                if (method == "") {
                    InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckNoMethod);
                    return null;
                }

                // HTTPボディ
                string httpBodyText = null;
                EncodingType httpBodyEncodingType = null;
                string httpBodyFileName = null;

                byte[] requestBody = null;
                if (m_parent.radioButtonHTTPBodyText.Checked) {
                    // テキスト
                    success = m_controlText.GetResult(out httpBodyText, out httpBodyEncodingType, out requestBody);
                    if (!success) {
                        return null;
                    }
                } else if (m_parent.radioButtonHTTPBodyFile.Checked) {
                    // ファイル
                    success = m_controlFileRef.GetResult(out httpBodyFileName, out requestBody);
                    if (!success) {
                        return null;
                    }
                }

                // 結果
                ResponseViewerHttpRequest httpRequest = new ResponseViewerHttpRequest();
                httpRequest.RequestUrl = url;
                httpRequest.ProxyUrl = proxyUrl;
                httpRequest.HttpHeader = headerList;
                httpRequest.RequestMethod = method;
                httpRequest.HttpBodyText = httpBodyText;
                httpRequest.HttpBodyEncodingType = httpBodyEncodingType;
                httpRequest.HttpBodyFileName = httpBodyFileName;

                ResponseViewerRequestSetting requestSetting = new ResponseViewerRequestSetting();
                requestSetting.SelectedMode = ResponseViewerMode.HttpMode;
                requestSetting.HttpModeRequest = httpRequest;
                requestSetting.TcpModeRequest = null;

                ResponseViewerRequest request = new ResponseViewerRequest();
                request.RequestSetting = requestSetting;
                request.RequestBody = requestBody;

                return request;
            }
        }

        //=========================================================================================
        // クラス：TCPモードの実装
        //=========================================================================================
        private class TCPPage {
            // TCPリクエストのサンプル
            private const string TCP_REQUEST_SAMPLE = "" +
                    "POST /myservice/function HTTP/1.0\r\n" +
                    "Accept: text/xml, */*\r\n" +
                    "Accept-Language: ja\r\n" +
                    "Accept-Encoding: text/xml\r\n" +
                    "Content-Length: 27\r\n" +
                    "Content-Type: application/x-www-form-urlencoded; charset=UTF-8\r\n" +
                    "Host: localhost\r\n" +
                    "User-Agent: ShellFiler HttpResponseViewer\r\n" +
                    "\r\n" +
                    "PARAM1=VALUE1&PARAM2=VALUE2";

            // 親ダイアログ
            private HttpViewerRequestDialog m_parent;

            // テキスト入力の詳細パネル
            private HttpViewerTextControl m_controlText;

            // ファイル参照入力の詳細パネル
            private HttpViewerFileRefControl m_controlFileRef;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent   親ダイアログ
            // 　　　　[in]setting  デフォルトに使用する設定
            // 戻り値：なし
            //=========================================================================================
            public TCPPage(HttpViewerRequestDialog parent, ResponseViewerTcpRequest setting) {
                m_parent = parent;
                m_parent.textBoxTCPServer.Text = setting.ServerName;
                m_parent.numericPortNum.Minimum = 1;
                m_parent.numericPortNum.Maximum = 65535;
                m_parent.numericPortNum.Value = setting.PortNo;
                
                // リクエスト
                m_controlText = new HttpViewerTextControl(m_parent);
                m_controlFileRef = new HttpViewerFileRefControl(m_parent);
                if (setting.RequestText != null) {
                    m_controlText.Initialize(setting.RequestText, setting.RequestTextEncodingType, TCP_REQUEST_SAMPLE, true);
                    m_controlFileRef.Initialize(null);
                    m_parent.panelTCPRequest.Controls.Add(m_controlText);
                    m_parent.radioButtonTCPReqText.Checked = true;
                } else if (setting.RequestFileName != null) {
                    m_controlText.Initialize(null, null, TCP_REQUEST_SAMPLE, true);
                    m_controlFileRef.Initialize(setting.RequestFileName);
                    m_parent.panelTCPRequest.Controls.Add(m_controlFileRef);
                    m_parent.radioButtonTCPReqFile.Checked = true;
                }

                // イベントの接続
                m_parent.FormClosed += new FormClosedEventHandler(m_parent_FormClosed);
                m_parent.radioButtonTCPReqText.CheckedChanged += new EventHandler(radioButtonTcpRequest_CheckedChanged);
                m_parent.radioButtonTCPReqFile.CheckedChanged += new EventHandler(radioButtonTcpRequest_CheckedChanged);
            }

            //=========================================================================================
            // 機　能：フォームがクローズされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void m_parent_FormClosed(object sender, FormClosedEventArgs evt) {
                m_controlText.Dispose();
                m_controlText = null;
                m_controlFileRef.Dispose();
                m_controlFileRef = null;
            }

            //=========================================================================================
            // 機　能：TCPリクエストの選択用チェックボックスが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioButtonTcpRequest_CheckedChanged(object sender, EventArgs evt) {
                m_parent.panelTCPRequest.Controls.Clear();
                if (m_parent.radioButtonTCPReqText.Checked) {
                    m_parent.panelTCPRequest.Controls.Add(m_controlText);
                } else if (m_parent.radioButtonTCPReqFile.Checked) {
                    m_parent.panelTCPRequest.Controls.Add(m_controlFileRef);
                }
            }

            //=========================================================================================
            // 機　能：入力結果を返す
            // 引　数：なし
            // 戻り値：リクエストパラメータ（エラー等でキャンセルされたときnull）
            //=========================================================================================
            public ResponseViewerRequest GetResult() {
                bool success;

                // URL
                string serverName = m_parent.textBoxTCPServer.Text;
                if (serverName == "") {
                    InfoBox.Warning(m_parent, Resources.DlgHttpView_CheckNoServer);
                    return null;
                }
                
                // ポート番号
                int portNo = (int)(m_parent.numericPortNum.Value);

                // リクエスト
                string tcpRequestText = null;
                EncodingType tcpRequestEncodingType = null;
                string tcpRequestFileName = null;

                byte[] requestBody = null;
                if (m_parent.radioButtonTCPReqText.Checked) {
                    // テキスト
                    success = m_controlText.GetResult(out tcpRequestText, out tcpRequestEncodingType, out requestBody);
                    if (!success) {
                        return null;
                    }
                } else if (m_parent.radioButtonTCPReqFile.Checked) {
                    // ファイル
                    success = m_controlFileRef.GetResult(out tcpRequestFileName, out requestBody);
                    if (!success) {
                        return null;
                    }
                }

                // 結果
                ResponseViewerTcpRequest tcpRequest = new ResponseViewerTcpRequest();
                tcpRequest.ServerName = serverName;
                tcpRequest.PortNo = portNo;
                tcpRequest.RequestText = tcpRequestText;
                tcpRequest.RequestTextEncodingType = tcpRequestEncodingType;
                tcpRequest.RequestFileName = tcpRequestFileName;

                ResponseViewerRequestSetting requestSetting = new ResponseViewerRequestSetting();
                requestSetting.SelectedMode = ResponseViewerMode.TcpMode;
                requestSetting.HttpModeRequest = null;
                requestSetting.TcpModeRequest = tcpRequest;

                ResponseViewerRequest request = new ResponseViewerRequest();
                request.RequestSetting = requestSetting;
                request.RequestBody = requestBody;

                return request;
            }
        }
    }
}
