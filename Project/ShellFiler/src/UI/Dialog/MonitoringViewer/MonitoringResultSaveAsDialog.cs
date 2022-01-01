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
    // クラス：モニタリング結果の名前をつけて保存ダイアログ
    //=========================================================================================
    public partial class MonitoringResultSaveAsDialog : Form {
        // 保存する形式
        private MatrixFormatter.SaveFormat m_saveFormat;

        // 保存するファイル名
        private string m_saveFileName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MonitoringResultSaveAsDialog() {
            InitializeComponent();
            string file = "ssh" + DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + file;
            this.textBoxFileName.Text = path;
            this.radioFormatOrg.Checked = true;
        }

        //=========================================================================================
        // 機　能：参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFileNameRef_Click(object sender, EventArgs evt) {
            string file = this.textBoxFileName.Text;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = file;
            sfd.InitialDirectory = GenericFileStringUtils.GetDirectoryName(file);
            sfd.Filter = Resources.MonitorView_SaveAsFilter;
            sfd.FilterIndex = 3;
            sfd.RestoreDirectory = true;
            sfd.CheckFileExists = false;
            DialogResult result = sfd.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            this.textBoxFileName.Text = sfd.FileName;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // フォーマットを取得
            MatrixFormatter.SaveFormat format;
            if (this.radioFormatOrg.Checked) {
                format = MatrixFormatter.SaveFormat.Original;
            } else if (this.radioFormatTab.Checked) {
                format = MatrixFormatter.SaveFormat.Tsv;
            } else {
                format = MatrixFormatter.SaveFormat.Csv;
            }

            // ファイル名との比較
            bool warning = false;
            string path = this.textBoxFileName.Text;
            if (path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) {
                if (format != MatrixFormatter.SaveFormat.Csv) {
                    warning = true;
                }
            } else if (path.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase)) {
                if (format != MatrixFormatter.SaveFormat.Tsv) {
                    warning = true;
                }
            } else if (path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)) {
                if (format != MatrixFormatter.SaveFormat.Original) {
                    warning = true;
                }
            }

            // 拡張子を確認
            if (warning) {
                DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgMonitorSave_ExtFormatDifference);
                if (result != DialogResult.Yes) {
                    return;
                }
            }

            // ファイルを確認
            if (File.Exists(path)) {
                DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgMonitorSave_FileOverwrite);
                if (result != DialogResult.Yes) {
                    return;
                }
            }

            m_saveFileName = path;
            m_saveFormat = format;
            Close();
            DialogResult = DialogResult.OK;
        }

        //=========================================================================================
        // プロパティ：保存する形式
        //=========================================================================================
        public MatrixFormatter.SaveFormat SaveFormat {
            get {
                return m_saveFormat;
            }
        }

        //=========================================================================================
        // プロパティ：保存するファイル名
        //=========================================================================================
        public string SaveFileName {
            get {
                return m_saveFileName;
            }
        }
    }
}
