namespace ShellFiler.UI.Dialog.Option {
    partial class GraphicsViewerViewPage {
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
            this.components = new System.ComponentModel.Container();
            this.radioOriginal = new System.Windows.Forms.RadioButton();
            this.radioAutoZoom = new System.Windows.Forms.RadioButton();
            this.radioAutoZoomClick = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxShrinkOnly = new System.Windows.Forms.CheckBox();
            this.panelSample = new System.Windows.Forms.Panel();
            this.radioAutoWideClick = new System.Windows.Forms.RadioButton();
            this.radioAutoWide = new System.Windows.Forms.RadioButton();
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioOriginal
            // 
            this.radioOriginal.AutoSize = true;
            this.radioOriginal.Location = new System.Drawing.Point(7, 19);
            this.radioOriginal.Name = "radioOriginal";
            this.radioOriginal.Size = new System.Drawing.Size(95, 19);
            this.radioOriginal.TabIndex = 0;
            this.radioOriginal.TabStop = true;
            this.radioOriginal.Text = "元のサイズ(&O)";
            this.radioOriginal.UseVisualStyleBackColor = true;
            this.radioOriginal.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioAutoZoom
            // 
            this.radioAutoZoom.AutoSize = true;
            this.radioAutoZoom.Location = new System.Drawing.Point(6, 41);
            this.radioAutoZoom.Name = "radioAutoZoom";
            this.radioAutoZoom.Size = new System.Drawing.Size(187, 19);
            this.radioAutoZoom.TabIndex = 1;
            this.radioAutoZoom.TabStop = true;
            this.radioAutoZoom.Text = "画面サイズで画像全体を表示(&A)";
            this.radioAutoZoom.UseVisualStyleBackColor = true;
            this.radioAutoZoom.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioAutoZoomClick
            // 
            this.radioAutoZoomClick.AutoSize = true;
            this.radioAutoZoomClick.Location = new System.Drawing.Point(6, 63);
            this.radioAutoZoomClick.Name = "radioAutoZoomClick";
            this.radioAutoZoomClick.Size = new System.Drawing.Size(286, 19);
            this.radioAutoZoomClick.TabIndex = 2;
            this.radioAutoZoomClick.TabStop = true;
            this.radioAutoZoomClick.Text = "画面サイズで画像全体を表示、クリックで元のサイズ(&C)";
            this.radioAutoZoomClick.UseVisualStyleBackColor = true;
            this.radioAutoZoomClick.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxShrinkOnly);
            this.groupBox1.Controls.Add(this.panelSample);
            this.groupBox1.Controls.Add(this.radioAutoWideClick);
            this.groupBox1.Controls.Add(this.radioAutoWide);
            this.groupBox1.Controls.Add(this.radioAutoZoomClick);
            this.groupBox1.Controls.Add(this.radioAutoZoom);
            this.groupBox1.Controls.Add(this.radioOriginal);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 169);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "画像の拡大方法";
            // 
            // checkBoxShrinkOnly
            // 
            this.checkBoxShrinkOnly.AutoSize = true;
            this.checkBoxShrinkOnly.Location = new System.Drawing.Point(6, 147);
            this.checkBoxShrinkOnly.Name = "checkBoxShrinkOnly";
            this.checkBoxShrinkOnly.Size = new System.Drawing.Size(278, 19);
            this.checkBoxShrinkOnly.TabIndex = 4;
            this.checkBoxShrinkOnly.Text = "画面より画像が小さいときも画面サイズに合わせる(&Z)";
            this.checkBoxShrinkOnly.UseVisualStyleBackColor = true;
            // 
            // panelSample
            // 
            this.panelSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSample.Location = new System.Drawing.Point(374, 19);
            this.panelSample.Name = "panelSample";
            this.panelSample.Size = new System.Drawing.Size(112, 52);
            this.panelSample.TabIndex = 3;
            this.panelSample.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSample_Paint);
            // 
            // radioAutoWideClick
            // 
            this.radioAutoWideClick.AutoSize = true;
            this.radioAutoWideClick.Location = new System.Drawing.Point(6, 107);
            this.radioAutoWideClick.Name = "radioAutoWideClick";
            this.radioAutoWideClick.Size = new System.Drawing.Size(246, 19);
            this.radioAutoWideClick.TabIndex = 2;
            this.radioAutoWideClick.TabStop = true;
            this.radioAutoWideClick.Text = "画面全体に拡大表示、クリックで元のサイズ(&L)";
            this.radioAutoWideClick.UseVisualStyleBackColor = true;
            this.radioAutoWideClick.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioAutoWide
            // 
            this.radioAutoWide.AutoSize = true;
            this.radioAutoWide.Location = new System.Drawing.Point(6, 85);
            this.radioAutoWide.Name = "radioAutoWide";
            this.radioAutoWide.Size = new System.Drawing.Size(152, 19);
            this.radioAutoWide.TabIndex = 1;
            this.radioAutoWide.TabStop = true;
            this.radioAutoWide.Text = "画面全体に拡大表示(&W)";
            this.radioAutoWide.UseVisualStyleBackColor = true;
            this.radioAutoWide.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // timerAnimation
            // 
            this.timerAnimation.Interval = 1000;
            this.timerAnimation.Tick += new System.EventHandler(this.TimerAnimation_Tick);
            // 
            // GraphicsViewerViewPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "GraphicsViewerViewPage";
            this.Size = new System.Drawing.Size(520, 370);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioOriginal;
        private System.Windows.Forms.RadioButton radioAutoZoom;
        private System.Windows.Forms.RadioButton radioAutoZoomClick;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioAutoWideClick;
        private System.Windows.Forms.RadioButton radioAutoWide;
        private System.Windows.Forms.Panel panelSample;
        private System.Windows.Forms.Timer timerAnimation;
        private System.Windows.Forms.CheckBox checkBoxShrinkOnly;



    }
}
