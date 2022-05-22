namespace ShellFiler.UI.Dialog.GraphicsViewer {
    partial class FilterPropertyPanel {
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
            this.pictureBoxSrc = new System.Windows.Forms.PictureBox();
            this.pictureBoxDest = new System.Windows.Forms.PictureBox();
            this.labelParam1 = new System.Windows.Forms.Label();
            this.trackBarParam1 = new System.Windows.Forms.TrackBar();
            this.labelFilterName = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.labelLevelRight1 = new System.Windows.Forms.Label();
            this.labelLevelRight2 = new System.Windows.Forms.Label();
            this.trackBarParam2 = new System.Windows.Forms.TrackBar();
            this.labelParam2 = new System.Windows.Forms.Label();
            this.labelParam3 = new System.Windows.Forms.Label();
            this.trackBarParam3 = new System.Windows.Forms.TrackBar();
            this.labelLevelRight3 = new System.Windows.Forms.Label();
            this.labelLevelLeft1 = new System.Windows.Forms.Label();
            this.labelLevelLeft2 = new System.Windows.Forms.Label();
            this.labelLevelLeft3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSrc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarParam1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarParam2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarParam3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxSrc
            // 
            this.pictureBoxSrc.Location = new System.Drawing.Point(9, 61);
            this.pictureBoxSrc.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pictureBoxSrc.Name = "pictureBoxSrc";
            this.pictureBoxSrc.Size = new System.Drawing.Size(192, 144);
            this.pictureBoxSrc.TabIndex = 0;
            this.pictureBoxSrc.TabStop = false;
            // 
            // pictureBoxDest
            // 
            this.pictureBoxDest.Location = new System.Drawing.Point(212, 61);
            this.pictureBoxDest.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pictureBoxDest.Name = "pictureBoxDest";
            this.pictureBoxDest.Size = new System.Drawing.Size(192, 144);
            this.pictureBoxDest.TabIndex = 0;
            this.pictureBoxDest.TabStop = false;
            // 
            // labelParam1
            // 
            this.labelParam1.AutoSize = true;
            this.labelParam1.Location = new System.Drawing.Point(424, 21);
            this.labelParam1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelParam1.Name = "labelParam1";
            this.labelParam1.Size = new System.Drawing.Size(103, 30);
            this.labelParam1.TabIndex = 2;
            this.labelParam1.Text = "コントラスト";
            // 
            // trackBarParam1
            // 
            this.trackBarParam1.Location = new System.Drawing.Point(623, 14);
            this.trackBarParam1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.trackBarParam1.Name = "trackBarParam1";
            this.trackBarParam1.Size = new System.Drawing.Size(152, 80);
            this.trackBarParam1.TabIndex = 3;
            this.trackBarParam1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarParam1.ValueChanged += new System.EventHandler(this.TrackBarParam_ValueChanged);
            // 
            // labelFilterName
            // 
            this.labelFilterName.AutoSize = true;
            this.labelFilterName.Location = new System.Drawing.Point(5, 7);
            this.labelFilterName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelFilterName.Name = "labelFilterName";
            this.labelFilterName.Size = new System.Drawing.Size(158, 30);
            this.labelFilterName.TabIndex = 0;
            this.labelFilterName.Text = "labelFilterName";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(273, 7);
            this.buttonReset.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(131, 40);
            this.buttonReset.TabIndex = 1;
            this.buttonReset.Text = "既定値(&X)";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.RuttonReset_Click);
            // 
            // labelLevelRight1
            // 
            this.labelLevelRight1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelLevelRight1.Location = new System.Drawing.Point(777, 21);
            this.labelLevelRight1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLevelRight1.Name = "labelLevelRight1";
            this.labelLevelRight1.Size = new System.Drawing.Size(70, 30);
            this.labelLevelRight1.TabIndex = 5;
            this.labelLevelRight1.Text = "強";
            // 
            // labelLevelRight2
            // 
            this.labelLevelRight2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelLevelRight2.Location = new System.Drawing.Point(777, 89);
            this.labelLevelRight2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLevelRight2.Name = "labelLevelRight2";
            this.labelLevelRight2.Size = new System.Drawing.Size(70, 30);
            this.labelLevelRight2.TabIndex = 9;
            this.labelLevelRight2.Text = "強";
            // 
            // trackBarParam2
            // 
            this.trackBarParam2.Location = new System.Drawing.Point(623, 82);
            this.trackBarParam2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.trackBarParam2.Name = "trackBarParam2";
            this.trackBarParam2.Size = new System.Drawing.Size(152, 80);
            this.trackBarParam2.TabIndex = 7;
            this.trackBarParam2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarParam2.ValueChanged += new System.EventHandler(this.TrackBarParam_ValueChanged);
            // 
            // labelParam2
            // 
            this.labelParam2.AutoSize = true;
            this.labelParam2.Location = new System.Drawing.Point(424, 89);
            this.labelParam2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelParam2.Name = "labelParam2";
            this.labelParam2.Size = new System.Drawing.Size(103, 30);
            this.labelParam2.TabIndex = 6;
            this.labelParam2.Text = "コントラスト";
            // 
            // labelParam3
            // 
            this.labelParam3.AutoSize = true;
            this.labelParam3.Location = new System.Drawing.Point(424, 158);
            this.labelParam3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelParam3.Name = "labelParam3";
            this.labelParam3.Size = new System.Drawing.Size(103, 30);
            this.labelParam3.TabIndex = 10;
            this.labelParam3.Text = "コントラスト";
            // 
            // trackBarParam3
            // 
            this.trackBarParam3.Location = new System.Drawing.Point(623, 150);
            this.trackBarParam3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.trackBarParam3.Name = "trackBarParam3";
            this.trackBarParam3.Size = new System.Drawing.Size(152, 80);
            this.trackBarParam3.TabIndex = 11;
            this.trackBarParam3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarParam3.ValueChanged += new System.EventHandler(this.TrackBarParam_ValueChanged);
            // 
            // labelLevelRight3
            // 
            this.labelLevelRight3.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelLevelRight3.Location = new System.Drawing.Point(777, 158);
            this.labelLevelRight3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLevelRight3.Name = "labelLevelRight3";
            this.labelLevelRight3.Size = new System.Drawing.Size(70, 30);
            this.labelLevelRight3.TabIndex = 13;
            this.labelLevelRight3.Text = "強";
            // 
            // labelLevelLeft1
            // 
            this.labelLevelLeft1.Location = new System.Drawing.Point(544, 21);
            this.labelLevelLeft1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLevelLeft1.Name = "labelLevelLeft1";
            this.labelLevelLeft1.Size = new System.Drawing.Size(72, 28);
            this.labelLevelLeft1.TabIndex = 4;
            this.labelLevelLeft1.Text = "弱";
            this.labelLevelLeft1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelLevelLeft2
            // 
            this.labelLevelLeft2.Location = new System.Drawing.Point(544, 89);
            this.labelLevelLeft2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLevelLeft2.Name = "labelLevelLeft2";
            this.labelLevelLeft2.Size = new System.Drawing.Size(72, 28);
            this.labelLevelLeft2.TabIndex = 8;
            this.labelLevelLeft2.Text = "弱";
            this.labelLevelLeft2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelLevelLeft3
            // 
            this.labelLevelLeft3.Location = new System.Drawing.Point(544, 158);
            this.labelLevelLeft3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLevelLeft3.Name = "labelLevelLeft3";
            this.labelLevelLeft3.Size = new System.Drawing.Size(72, 28);
            this.labelLevelLeft3.TabIndex = 12;
            this.labelLevelLeft3.Text = "弱";
            this.labelLevelLeft3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // FilterPropertyPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.labelLevelLeft3);
            this.Controls.Add(this.labelLevelRight3);
            this.Controls.Add(this.trackBarParam3);
            this.Controls.Add(this.labelLevelRight2);
            this.Controls.Add(this.labelLevelRight1);
            this.Controls.Add(this.labelLevelLeft2);
            this.Controls.Add(this.labelLevelLeft1);
            this.Controls.Add(this.labelParam3);
            this.Controls.Add(this.labelParam2);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.labelFilterName);
            this.Controls.Add(this.labelParam1);
            this.Controls.Add(this.pictureBoxDest);
            this.Controls.Add(this.pictureBoxSrc);
            this.Controls.Add(this.trackBarParam2);
            this.Controls.Add(this.trackBarParam1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "FilterPropertyPanel";
            this.Size = new System.Drawing.Size(852, 214);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSrc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarParam1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarParam2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarParam3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxSrc;
        private System.Windows.Forms.PictureBox pictureBoxDest;
        private System.Windows.Forms.Label labelParam1;
        private System.Windows.Forms.TrackBar trackBarParam1;
        private System.Windows.Forms.Label labelFilterName;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Label labelLevelRight1;
        private System.Windows.Forms.Label labelLevelRight2;
        private System.Windows.Forms.TrackBar trackBarParam2;
        private System.Windows.Forms.Label labelParam2;
        private System.Windows.Forms.Label labelParam3;
        private System.Windows.Forms.TrackBar trackBarParam3;
        private System.Windows.Forms.Label labelLevelRight3;
        private System.Windows.Forms.Label labelLevelLeft1;
        private System.Windows.Forms.Label labelLevelLeft2;
        private System.Windows.Forms.Label labelLevelLeft3;
    }
}
