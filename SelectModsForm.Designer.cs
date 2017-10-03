namespace com.clusterrr.hakchi_gui
{
    partial class SelectModsForm
    {

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectModsForm));
            this.checkedListBoxMods = new System.Windows.Forms.CheckedListBox();
            this.textBoxReadme = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBoxMods
            // 
            this.checkedListBoxMods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxMods.FormattingEnabled = true;
            this.checkedListBoxMods.Location = new System.Drawing.Point(12, 12);
            this.checkedListBoxMods.Name = "checkedListBoxMods";
            this.checkedListBoxMods.Size = new System.Drawing.Size(258, 334);
            this.checkedListBoxMods.Sorted = true;
            this.checkedListBoxMods.TabIndex = 0;
            this.checkedListBoxMods.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxMods_SelectedIndexChanged);
            // 
            // textBoxReadme
            // 
            this.textBoxReadme.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReadme.Enabled = false;
            this.textBoxReadme.Location = new System.Drawing.Point(276, 12);
            this.textBoxReadme.Multiline = true;
            this.textBoxReadme.Name = "textBoxReadme";
            this.textBoxReadme.ReadOnly = true;
            this.textBoxReadme.Size = new System.Drawing.Size(271, 334);
            this.textBoxReadme.TabIndex = 1;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(12, 358);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(535, 33);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // SelectModsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 401);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxReadme);
            this.Controls.Add(this.checkedListBoxMods);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "SelectModsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ModsSelect";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SelectModsForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SelectModsForm_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.TextBox textBoxReadme;
        private System.Windows.Forms.Button buttonOk;
        internal System.Windows.Forms.CheckedListBox checkedListBoxMods;
    }
}