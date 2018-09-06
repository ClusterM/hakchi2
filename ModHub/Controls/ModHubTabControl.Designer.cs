namespace com.clusterrr.hakchi_gui.ModHub.Controls
{
    partial class ModHubTabControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.modList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modReadme = new com.clusterrr.hakchi_gui.Hmod.Controls.ReadmeControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.modDownloadButton = new System.Windows.Forms.Button();
            this.modDownloadInstallButton = new System.Windows.Forms.Button();
            this.modInfo = new com.clusterrr.hakchi_gui.Extensions.ModStore.ModInfoControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 244F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 212F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.modList, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.modReadme, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(752, 414);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // modList
            // 
            this.modList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.modList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.modList.Location = new System.Drawing.Point(3, 3);
            this.modList.Name = "modList";
            this.modList.Size = new System.Drawing.Size(238, 408);
            this.modList.TabIndex = 1;
            this.modList.UseCompatibleStateImageBehavior = false;
            this.modList.View = System.Windows.Forms.View.Details;
            this.modList.SelectedIndexChanged += new System.EventHandler(this.modList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 234;
            // 
            // modReadme
            // 
            this.modReadme.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modReadme.BackColor = System.Drawing.SystemColors.Control;
            this.modReadme.Location = new System.Drawing.Point(459, 3);
            this.modReadme.Name = "modReadme";
            this.modReadme.Size = new System.Drawing.Size(290, 408);
            this.modReadme.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.modDownloadButton);
            this.panel1.Controls.Add(this.modDownloadInstallButton);
            this.panel1.Controls.Add(this.modInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(247, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 408);
            this.panel1.TabIndex = 3;
            // 
            // modDownloadButton
            // 
            this.modDownloadButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.modDownloadButton.Enabled = false;
            this.modDownloadButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.modDownloadButton.Location = new System.Drawing.Point(0, 316);
            this.modDownloadButton.Name = "modDownloadButton";
            this.modDownloadButton.Size = new System.Drawing.Size(206, 46);
            this.modDownloadButton.TabIndex = 11;
            this.modDownloadButton.Text = "modDownloadButton";
            this.modDownloadButton.UseVisualStyleBackColor = true;
            this.modDownloadButton.Click += new System.EventHandler(this.modDownloadButton_Click);
            // 
            // modDownloadInstallButton
            // 
            this.modDownloadInstallButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.modDownloadInstallButton.Enabled = false;
            this.modDownloadInstallButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.modDownloadInstallButton.Location = new System.Drawing.Point(0, 362);
            this.modDownloadInstallButton.Name = "modDownloadInstallButton";
            this.modDownloadInstallButton.Size = new System.Drawing.Size(206, 46);
            this.modDownloadInstallButton.TabIndex = 10;
            this.modDownloadInstallButton.Text = "modDownloadInstallButton";
            this.modDownloadInstallButton.UseVisualStyleBackColor = true;
            this.modDownloadInstallButton.Visible = false;
            this.modDownloadInstallButton.Click += new System.EventHandler(this.modDownloadInstallButton_Click);
            // 
            // modInfo
            // 
            this.modInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modInfo.Author = null;
            this.modInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.modInfo.InstalledVersion = null;
            this.modInfo.LatestVersion = null;
            this.modInfo.Location = new System.Drawing.Point(0, 0);
            this.modInfo.ModuleName = null;
            this.modInfo.Name = "modInfo";
            this.modInfo.Size = new System.Drawing.Size(206, 408);
            this.modInfo.TabIndex = 1;
            // 
            // ModHubTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ModHubTabControl";
            this.Size = new System.Drawing.Size(752, 414);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView modList;
        private Hmod.Controls.ReadmeControl modReadme;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel panel1;
        private Extensions.ModStore.ModInfoControl modInfo;
        private System.Windows.Forms.Button modDownloadButton;
        private System.Windows.Forms.Button modDownloadInstallButton;
    }
}
