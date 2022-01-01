using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Web;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：HTTPリクエストパラメータの編集ダイアログ
    //=========================================================================================
    public partial class HttpViewerRequestParamDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]urlParam  「URL?パラメータ」の初期値
        // 戻り値：なし
        //=========================================================================================
        public HttpViewerRequestParamDialog(string urlParam) {
            InitializeComponent();
            this.textBoxEncoded.Text = urlParam;
        }

        //=========================================================================================
        // 機　能：ダイアログがロードされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void HttpViewerRequestParamDialog_Load(object sender, EventArgs evt) {
            // エンコーディング
            EncodingType[] encodingList = EncodingType.HttpRequestEncoding;
            string[] encodingNameList = new string[encodingList.Length];
            for (int i = 0; i < encodingList.Length; i++) {
                encodingNameList[i] = encodingList[i].DisplayName;
            }
            this.comboBoxEncoding.Items.AddRange(encodingNameList);
            this.comboBoxEncoding.SelectedIndex = 0;

            // 一覧
            TextToParamList();

            // UI状態を更新
            EnableUIItem();

            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            this.textBoxEncoded.TextChanged += new EventHandler(textBoxEncoded_TextChanged);
        }

        //=========================================================================================
        // 機　能：テキスト表現のパラメータをデータグリッドビューに反映する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void TextToParamList() {
            string[] encUrlParam = this.textBoxEncoded.Text.Split(new char[] {'?'}, 2);
            string encUrl = (encUrlParam.Length >= 2) ? encUrlParam[1] : "";
            EncodingType[] encodingList = EncodingType.HttpRequestEncoding;
            EncodingType encoding = encodingList[this.comboBoxEncoding.SelectedIndex];
            List<KeyValuePair<string, string>> paramList = StringUtils.SplitHtmlParameter(encUrl, encoding.Encoding);
            this.dataGridView.Rows.Clear();
            if (paramList == null) {
                InfoBox.Warning(this, Resources.DlgHttpViewerParam_CannotParse);
            } else {
                for (int i = 0; i < paramList.Count; i++) {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = paramList[i].Key;
                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = paramList[i].Value;
                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    this.dataGridView.Rows.Add(row);
                }
            }
        }

        //=========================================================================================
        // 機　能：データグリッドビューのデータをテキスト表現に反映する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ParamListToText() {
            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();
            int rowCount = this.dataGridView.Rows.Count;
            for (int i = 0; i < rowCount; i++) {
                string param = (string)(this.dataGridView.Rows[i].Cells[0].Value);
                string value = (string)(this.dataGridView.Rows[i].Cells[1].Value);
                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(param, value);
                paramList.Add(pair);
            }

            EncodingType[] encodingList = EncodingType.HttpRequestEncoding;
            EncodingType encoding = encodingList[this.comboBoxEncoding.SelectedIndex];
            string encodedParam = StringUtils.AppendHtmlParameter(paramList, encoding.Encoding);
            string url = this.textBoxEncoded.Text.Split('?')[0];
            if (encodedParam.Length > 0) {
                url += "?" + encodedParam;
            }
            this.textBoxEncoded.Text = url;
        }
        
        //=========================================================================================
        // 機　能：UIの項目の有効/無効状態を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            int currentRow = GetSelectedRow();
            if (currentRow == 0) {
                buttonUp.Enabled = false;
            } else {
                buttonUp.Enabled = true;
            }
            if (currentRow == this.dataGridView.Rows.Count - 1) {
                buttonDown.Enabled = false;
            } else {
                buttonDown.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：選択中の行を返す
        // 引　数：なし
        // 戻り値：選択中の行（-1:選択中ではない）
        //=========================================================================================
        private int GetSelectedRow() {
            if (this.dataGridView.SelectedCells.Count != 1) {
                return -1;
            } else {
                return this.dataGridView.SelectedCells[0].RowIndex;
            }
        }

        //=========================================================================================
        // 機　能：選択中のカラムを返す
        // 引　数：なし
        // 戻り値：選択中のカラム（-1:選択中ではない）
        //=========================================================================================
        private int GetSelectedColumn() {
            if (this.dataGridView.SelectedCells.Count != 1) {
                return -1;
            } else {
                return this.dataGridView.SelectedCells[0].ColumnIndex;
            }
        }

        //=========================================================================================
        // 機　能：パラメータの追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAdd_Click(object sender, EventArgs evt) {
            int currentColumn = Math.Max(0, GetSelectedColumn());
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = "Param";
            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = "Value";
            row.Cells.Add(cell1);
            row.Cells.Add(cell2);
            this.dataGridView.Rows.Add(row);
            ParamListToText();
            EnableUIItem();
            this.dataGridView.CurrentCell = this.dataGridView.Rows[this.dataGridView.Rows.Count - 1].Cells[currentColumn];
        }

        //=========================================================================================
        // 機　能：パラメータの削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            int row = GetSelectedRow();
            if (row == -1) {
                return;
            }
            this.dataGridView.Rows.RemoveAt(row);
            ParamListToText();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：パラメータの上へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            int currentRow = GetSelectedRow();
            int currentColumn = GetSelectedColumn();
            if (currentRow == -1 || currentColumn == -1) {
                return;
            }
            DataGridViewCell cursor = this.dataGridView.CurrentCell;
            DataGridViewRow rowUp = this.dataGridView.Rows[currentRow - 1];
            this.dataGridView.Rows.RemoveAt(currentRow - 1);
            this.dataGridView.Rows.Insert(currentRow, rowUp);
            ParamListToText();
            EnableUIItem();
            this.dataGridView.CurrentCell = this.dataGridView.Rows[currentRow - 1].Cells[currentColumn];
        }

        //=========================================================================================
        // 機　能：パラメータの下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            int currentRow = GetSelectedRow();
            int currentColumn = GetSelectedColumn();
            if (currentRow == -1 || currentColumn == -1) {
                return;
            }
            DataGridViewCell cursor = this.dataGridView.CurrentCell;
            DataGridViewRow rowDown = this.dataGridView.Rows[currentRow + 1];
            this.dataGridView.Rows.RemoveAt(currentRow + 1);
            this.dataGridView.Rows.Insert(currentRow, rowDown);
            ParamListToText();
            EnableUIItem();
            this.dataGridView.CurrentCell = this.dataGridView.Rows[currentRow + 1].Cells[currentColumn];
        }

        //=========================================================================================
        // 機　能：データグリッドビューのセルの値が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs evt) {
            ParamListToText();
        }

        //=========================================================================================
        // 機　能：データグリッドビューの選択状態が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void dataGridView_SelectionChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：テキストからパラメータへの変換ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonTextToParam_Click(object sender, EventArgs evt) {
            TextToParamList();
        }

        //=========================================================================================
        // 機　能：パラメータからテキストへの変換ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonParamToText_Click(object sender, EventArgs evt) {
            ParamListToText();
        }

        //=========================================================================================
        // 機　能：エンコード済みパラメータのテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxEncoded_TextChanged(object sender, EventArgs evt) {
            TextToParamList();
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

        //=========================================================================================
        // プロパティ：入力されたパラメータのテキスト表現
        //=========================================================================================
        public string ResultText {
            get {
                return this.textBoxEncoded.Text;
            }
        }
    }
}
