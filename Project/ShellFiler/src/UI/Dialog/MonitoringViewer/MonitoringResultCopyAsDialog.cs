using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.FileSystem;
using ShellFiler.MonitoringViewer;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.MonitoringViewer {

    //=========================================================================================
    // クラス：モニタリング結果の形式を指定してコピーダイアログ
    //=========================================================================================
    public partial class MonitoringResultCopyAsDialog : Form {
        // 使用するデータ
        private MatrixData m_matrixData;

        // SSHのエンコード方式
        private Encoding m_encoding;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]matrixData   使用するデータ
        // 　　　　[in]encoding     SSHのエンコード方式
        // 戻り値：なし
        //=========================================================================================
        public MonitoringResultCopyAsDialog(MatrixData matrixData, Encoding encoding) {
            InitializeComponent();
            m_matrixData = matrixData;
            m_encoding = encoding;
            this.radioFormatOrg.Checked = true;
        }

        //=========================================================================================
        // 機　能：フォーマットのラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioFormat_CheckedChanged(object sender, EventArgs evt) {
            string str = GetString();
            this.textBoxSample.Text = str;
        }

        //=========================================================================================
        // 機　能：UIで指定されたフォーマットに基づいて整形する
        // 引　数：なし
        // 戻り値：整形した文字列
        //=========================================================================================
        private string GetString() {
            string result;
            if (this.radioFormatOrg.Checked) {
                result = m_encoding.GetString(m_matrixData.OriginalData);
            } else if (this.radioFormatTab.Checked) {
                MatrixFormatter formatter = new MatrixFormatter(m_matrixData);
                result= formatter.Format(MatrixFormatter.SaveFormat.Tsv);
            } else {
                MatrixFormatter formatter = new MatrixFormatter(m_matrixData);
                result= formatter.Format(MatrixFormatter.SaveFormat.Csv);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            string data = GetString();
            Clipboard.SetDataObject(data, true);

            Close();
            DialogResult = DialogResult.OK;
        }
    }
}
