namespace com.clusterrr.hakchi_gui
{
    partial class CustomBorderManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewBorders = new System.Windows.Forms.ListView();
            this.gameName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.pictureBoxArt = new System.Windows.Forms.PictureBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonSync = new System.Windows.Forms.Button();
            this.groupBoxDefaultBorders = new System.Windows.Forms.GroupBox();
            this.checkedListBoxDefaultBorders = new System.Windows.Forms.CheckedListBox();
            this.openFileDialogZipBorder = new System.Windows.Forms.OpenFileDialog();
            this.progressBarSync = new System.Windows.Forms.ProgressBar();
            this.labelSyncing = new System.Windows.Forms.Label();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.groupBoxOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            this.groupBoxDefaultBorders.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewBorders
            // 
            this.listViewBorders.CheckBoxes = true;
            this.listViewBorders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.gameName});
            this.listViewBorders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewBorders.HideSelection = false;
            this.listViewBorders.Location = new System.Drawing.Point(12, 25);
            this.listViewBorders.Name = "listViewBorders";
            this.listViewBorders.Size = new System.Drawing.Size(282, 269);
            this.listViewBorders.TabIndex = 8;
            this.listViewBorders.UseCompatibleStateImageBehavior = false;
            this.listViewBorders.View = System.Windows.Forms.View.Details;
            this.listViewBorders.SelectedIndexChanged += new System.EventHandler(this.listViewBorders_SelectedIndexChanged);
            this.listViewBorders.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewBorders_KeyDown);
            // 
            // gameName
            // 
            this.gameName.Text = "Game name";
            this.gameName.Width = 253;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Select borders:";
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.pictureBoxArt);
            this.groupBoxOptions.Controls.Add(this.textBoxName);
            this.groupBoxOptions.Controls.Add(this.labelName);
            this.groupBoxOptions.Location = new System.Drawing.Point(300, 9);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(293, 285);
            this.groupBoxOptions.TabIndex = 10;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Border options";
            // 
            // pictureBoxArt
            // 
            this.pictureBoxArt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxArt.Location = new System.Drawing.Point(6, 70);
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.Size = new System.Drawing.Size(281, 204);
            this.pictureBoxArt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxArt.TabIndex = 12;
            this.pictureBoxArt.TabStop = false;
            // 
            // textBoxName
            // 
            this.textBoxName.Enabled = false;
            this.textBoxName.Location = new System.Drawing.Point(59, 44);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(216, 20);
            this.textBoxName.TabIndex = 2;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelName.Location = new System.Drawing.Point(15, 47);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name:";
            // 
            // buttonDelete
            // 
            this.buttonDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonDelete.Location = new System.Drawing.Point(119, 321);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(125, 23);
            this.buttonDelete.TabIndex = 12;
            this.buttonDelete.Text = "Delete selected";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonAdd.Location = new System.Drawing.Point(12, 321);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(103, 23);
            this.buttonAdd.TabIndex = 11;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonSync
            // 
            this.buttonSync.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonSync.Location = new System.Drawing.Point(250, 321);
            this.buttonSync.Name = "buttonSync";
            this.buttonSync.Size = new System.Drawing.Size(103, 23);
            this.buttonSync.TabIndex = 13;
            this.buttonSync.Text = "Sync";
            this.buttonSync.UseVisualStyleBackColor = true;
            this.buttonSync.Click += new System.EventHandler(this.buttonSync_Click);
            // 
            // groupBoxDefaultBorders
            // 
            this.groupBoxDefaultBorders.Controls.Add(this.checkedListBoxDefaultBorders);
            this.groupBoxDefaultBorders.Location = new System.Drawing.Point(300, 9);
            this.groupBoxDefaultBorders.Name = "groupBoxDefaultBorders";
            this.groupBoxDefaultBorders.Size = new System.Drawing.Size(293, 285);
            this.groupBoxDefaultBorders.TabIndex = 15;
            this.groupBoxDefaultBorders.TabStop = false;
            this.groupBoxDefaultBorders.Text = "You can hide some default borders";
            this.groupBoxDefaultBorders.Visible = false;
            // 
            // checkedListBoxDefaultBorders
            // 
            this.checkedListBoxDefaultBorders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxDefaultBorders.FormattingEnabled = true;
            this.checkedListBoxDefaultBorders.Location = new System.Drawing.Point(5, 39);
            this.checkedListBoxDefaultBorders.Name = "checkedListBoxDefaultBorders";
            this.checkedListBoxDefaultBorders.Size = new System.Drawing.Size(282, 229);
            this.checkedListBoxDefaultBorders.TabIndex = 4;
            // 
            // openFileDialogZipBorder
            // 
            this.openFileDialogZipBorder.DefaultExt = "zip";
            this.openFileDialogZipBorder.Filter = "Fichiers zip|*.zip";
            this.openFileDialogZipBorder.Multiselect = true;
            // 
            // progressBarSync
            // 
            this.progressBarSync.Location = new System.Drawing.Point(359, 321);
            this.progressBarSync.Name = "progressBarSync";
            this.progressBarSync.Size = new System.Drawing.Size(234, 23);
            this.progressBarSync.TabIndex = 16;
            this.progressBarSync.Visible = false;
            // 
            // labelSyncing
            // 
            this.labelSyncing.AutoSize = true;
            this.labelSyncing.Location = new System.Drawing.Point(356, 300);
            this.labelSyncing.Name = "labelSyncing";
            this.labelSyncing.Size = new System.Drawing.Size(0, 13);
            this.labelSyncing.TabIndex = 17;
            this.labelSyncing.Visible = false;
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonUninstall.Location = new System.Drawing.Point(490, 295);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(103, 23);
            this.buttonUninstall.TabIndex = 18;
            this.buttonUninstall.Text = "Uninstall";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // CustomBorderManager
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 356);
            this.Controls.Add(this.buttonUninstall);
            this.Controls.Add(this.labelSyncing);
            this.Controls.Add(this.progressBarSync);
            this.Controls.Add(this.groupBoxDefaultBorders);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonSync);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listViewBorders);
            this.Name = "CustomBorderManager";
            this.Text = "CustomBorderManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomBorderManager_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CustomBorderManager_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CustomBorderManager_DragEnter);
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            this.groupBoxDefaultBorders.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ListView listViewBorders;
        private System.Windows.Forms.ColumnHeader gameName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.PictureBox pictureBoxArt;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonSync;
        private System.Windows.Forms.GroupBox groupBoxDefaultBorders;
        protected internal System.Windows.Forms.CheckedListBox checkedListBoxDefaultBorders;
        private System.Windows.Forms.OpenFileDialog openFileDialogZipBorder;
        private System.Windows.Forms.ProgressBar progressBarSync;
        private System.Windows.Forms.Label labelSyncing;
        private System.Windows.Forms.Button buttonUninstall;
    }
}