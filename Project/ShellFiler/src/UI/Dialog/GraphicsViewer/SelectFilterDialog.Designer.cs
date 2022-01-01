namespace ShellFiler.UI.Dialog.GraphicsViewer {
    partial class SelectFilterDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxUseFilter = new System.Windows.Forms.ListBox();
            this.listBoxNotUse = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.panelProperty = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOn = new System.Windows.Forms.Button();
            this.buttonOff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxUseFilter
            // 
            this.listBoxUseFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxUseFilter.FormattingEnabled = true;
            this.listBoxUseFilter.Location = new System.Drawing.Point(13, 13);
            this.listBoxUseFilter.Name = "listBoxUseFilter";
            this.listBoxUseFilter.Size = new System.Drawing.Size(200, 114);
            this.listBoxUseFilter.TabIndex = 0;
            this.listBoxUseFilter.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxUseFilter_DrawItem);
            this.listBoxUseFilter.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxUseFilter_MeasureItem);
            this.listBoxUseFilter.SelectedIndexChanged += new System.EventHandler(this.listBoxUseFilter_SelectedIndexChanged);
            // 
            // listBoxNotUse
            // 
            this.listBoxNotUse.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxNotUse.FormattingEnabled = true;
            this.listBoxNotUse.Location = new System.Drawing.Point(300, 13);
            this.listBoxNotUse.Name = "listBoxNotUse";
            this.listBoxNotUse.Size = new System.Drawing.Size(200, 114);
            this.listBoxNotUse.TabIndex = 5;
            this.listBoxNotUse.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxNotUse_DrawItem);
            this.listBoxNotUse.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxNotUse_MeasureItem);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(219, 13);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "<< 追加(&A)";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(219, 42);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "削除(&L) >>";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(219, 75);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(75, 23);
            this.buttonUp.TabIndex = 3;
            this.buttonUp.Text = "上へ(&U)";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(219, 104);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(75, 23);
            this.buttonDown.TabIndex = 4;
            this.buttonDown.Text = "下へ(&D)";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // panelProperty
            // 
            this.panelProperty.BackColor = System.Drawing.SystemColors.Window;
            this.panelProperty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelProperty.Location = new System.Drawing.Point(13, 134);
            this.panelProperty.Name = "panelProperty";
            this.panelProperty.Size = new System.Drawing.Size(487, 122);
            this.panelProperty.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "この画面の表示中にもビューアを操作できます";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(425, 262);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "元に戻す(&U)";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOn
            // 
            this.buttonOn.Location = new System.Drawing.Point(263, 262);
            this.buttonOn.Name = "buttonOn";
            this.buttonOn.Size = new System.Drawing.Size(75, 23);
            this.buttonOn.TabIndex = 8;
            this.buttonOn.Text = "閉じる(&C)";
            this.buttonOn.UseVisualStyleBackColor = true;
            this.buttonOn.Click += new System.EventHandler(this.buttonOn_Click);
            // 
            // buttonOff
            // 
            this.buttonOff.Location = new System.Drawing.Point(344, 262);
            this.buttonOff.Name = "buttonOff";
            this.buttonOff.Size = new System.Drawing.Size(75, 23);
            this.buttonOff.TabIndex = 9;
            this.buttonOff.Text = "設定OFF(&F)";
            this.buttonOff.UseVisualStyleBackColor = true;
            this.buttonOff.Click += new System.EventHandler(this.buttonOff_Click);
            // 
            // SelectFilterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(512, 297);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.panelProperty);
            this.Controls.Add(this.buttonOn);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonOff);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listBoxNotUse);
            this.Controls.Add(this.listBoxUseFilter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectFilterDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "フィルターの設定";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SelectFilterDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxUseFilter;
        private System.Windows.Forms.ListBox listBoxNotUse;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Panel panelProperty;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOn;
        private System.Windows.Forms.Button buttonOff;
    }
}