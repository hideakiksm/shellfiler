namespace ShellFiler.UI.Dialog {
    partial class FileFilterSettingComponent {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.labelFilterName = new System.Windows.Forms.Label();
            this.textBoxExplain = new System.Windows.Forms.TextBox();
            this.panelUI = new System.Windows.Forms.Panel();
            this.labelNoFilter1 = new System.Windows.Forms.Label();
            this.labelNoFilter2 = new System.Windows.Forms.Label();
            this.panelName = new System.Windows.Forms.Panel();
            this.panelName.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelFilterName
            // 
            this.labelFilterName.AutoSize = true;
            this.labelFilterName.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelFilterName.Location = new System.Drawing.Point(3, 8);
            this.labelFilterName.Name = "labelFilterName";
            this.labelFilterName.Size = new System.Drawing.Size(100, 12);
            this.labelFilterName.TabIndex = 0;
            this.labelFilterName.Text = "labelFilterName";
            // 
            // textBoxExplain
            // 
            this.textBoxExplain.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBoxExplain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxExplain.Location = new System.Drawing.Point(3, 27);
            this.textBoxExplain.Multiline = true;
            this.textBoxExplain.Name = "textBoxExplain";
            this.textBoxExplain.ReadOnly = true;
            this.textBoxExplain.Size = new System.Drawing.Size(149, 113);
            this.textBoxExplain.TabIndex = 1;
            // 
            // panelUI
            // 
            this.panelUI.Location = new System.Drawing.Point(179, 4);
            this.panelUI.Name = "panelUI";
            this.panelUI.Size = new System.Drawing.Size(474, 145);
            this.panelUI.TabIndex = 2;
            // 
            // labelNoFilter1
            // 
            this.labelNoFilter1.AutoSize = true;
            this.labelNoFilter1.Location = new System.Drawing.Point(254, 40);
            this.labelNoFilter1.Name = "labelNoFilter1";
            this.labelNoFilter1.Size = new System.Drawing.Size(151, 15);
            this.labelNoFilter1.TabIndex = 3;
            this.labelNoFilter1.Text = "フィルターが選択されていません";
            // 
            // labelNoFilter2
            // 
            this.labelNoFilter2.AutoSize = true;
            this.labelNoFilter2.Location = new System.Drawing.Point(158, 84);
            this.labelNoFilter2.Name = "labelNoFilter2";
            this.labelNoFilter2.Size = new System.Drawing.Size(342, 15);
            this.labelNoFilter2.TabIndex = 3;
            this.labelNoFilter2.Text = "右側のフィルター一覧でフィルターを選択して、追加をクリックしてください。";
            // 
            // panelName
            // 
            this.panelName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelName.Controls.Add(this.textBoxExplain);
            this.panelName.Controls.Add(this.labelFilterName);
            this.panelName.Location = new System.Drawing.Point(6, 4);
            this.panelName.Name = "panelName";
            this.panelName.Size = new System.Drawing.Size(157, 145);
            this.panelName.TabIndex = 4;
            // 
            // FileFilterSettingComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panelName);
            this.Controls.Add(this.labelNoFilter1);
            this.Controls.Add(this.labelNoFilter2);
            this.Controls.Add(this.panelUI);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "FileFilterSettingComponent";
            this.Size = new System.Drawing.Size(656, 153);
            this.panelName.ResumeLayout(false);
            this.panelName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFilterName;
        private System.Windows.Forms.TextBox textBoxExplain;
        private System.Windows.Forms.Panel panelUI;
        private System.Windows.Forms.Label labelNoFilter1;
        private System.Windows.Forms.Label labelNoFilter2;
        private System.Windows.Forms.Panel panelName;
    }
}
