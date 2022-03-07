namespace ShellFiler.UI.Dialog {
    partial class IntelliSensePopup {
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
            this.listViewWordList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listViewWordList
            // 
            this.listViewWordList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewWordList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewWordList.FullRowSelect = true;
            this.listViewWordList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewWordList.HideSelection = false;
            this.listViewWordList.HoverSelection = true;
            this.listViewWordList.Location = new System.Drawing.Point(0, 0);
            this.listViewWordList.MultiSelect = false;
            this.listViewWordList.Name = "listViewWordList";
            this.listViewWordList.OwnerDraw = true;
            this.listViewWordList.Size = new System.Drawing.Size(292, 273);
            this.listViewWordList.TabIndex = 0;
            this.listViewWordList.UseCompatibleStateImageBehavior = false;
            this.listViewWordList.View = System.Windows.Forms.View.Details;
            this.listViewWordList.VirtualMode = true;
            this.listViewWordList.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewWordList_DrawItem);
            this.listViewWordList.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewWordList_RetrieveVirtualItem);
            // 
            // IntelliSensePopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.ControlBox = false;
            this.Controls.Add(this.listViewWordList);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IntelliSensePopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "IntelliSensePopup";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewWordList;

    }
}