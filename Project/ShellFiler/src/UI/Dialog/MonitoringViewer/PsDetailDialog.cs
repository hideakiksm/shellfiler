using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.MonitoringViewer;
using ShellFiler.MonitoringViewer.ProcessMonitor;

namespace ShellFiler.UI.Dialog.MonitoringViewer {

    //=========================================================================================
    // クラス：プロセス詳細表示ダイアログ
    //=========================================================================================
    public partial class PsDetailDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]matrixData   処理中のデータの全体
        // 　　　　[in]cursorLine   カーソル行
        // 戻り値：なし
        //=========================================================================================
        public PsDetailDialog(MatrixData matrixData, int cursorLine) {
            InitializeComponent();

            this.SuspendLayout();
            int tabIndex = 0;
            int yPos = 13;
            int detailIndex = -1;
            int columnCount = matrixData.LineList[cursorLine].ValueList.Count;
            for (int i = 0; i < columnCount; i++) {
                if (matrixData.Header[i].DisplayName == PsHeaderKind.COMMAND.DisplayName) {
                    detailIndex = i;
                    continue;
                }
                Label label = new Label();
                label.AutoSize = true;
                label.Location = new Point(13, yPos);
                label.Size = new Size(35, 12);
                label.TabIndex = tabIndex++;
                label.Text = matrixData.Header[i].DisplayName;

                TextBox textBox = new TextBox();
                textBox.Location = new Point(128, yPos - 3);
                textBox.Size = new Size(173, 19);
                textBox.ReadOnly = true;
                textBox.TabIndex = tabIndex++;
                textBox.Text = matrixData.LineList[cursorLine].ValueList[i];
                this.Controls.Add(label);
                this.Controls.Add(textBox);
                yPos += 19 + 4;
            }
            this.labelDetail.Location = new Point(this.labelDetail.Left, yPos);
            this.labelDetail.Text = matrixData.Header[detailIndex].DisplayName;
            this.labelDetail.TabIndex = tabIndex++;
            yPos += 12 + 4;
            this.textBoxDetail.Location = new Point(this.textBoxDetail.Left, yPos);
            this.textBoxDetail.TabIndex = tabIndex++;
            if (detailIndex != -1) {
                this.textBoxDetail.Text = matrixData.LineList[cursorLine].ValueList[detailIndex];
            }
            yPos += this.textBoxDetail.Height + 8;
            this.buttonClose.Location = new Point(this.buttonClose.Left, yPos);
            this.buttonClose.TabIndex = tabIndex++;
            yPos += this.buttonClose.Height + 4 + 30;
            this.Height = yPos;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
