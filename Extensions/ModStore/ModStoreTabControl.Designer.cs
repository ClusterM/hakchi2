namespace com.clusterrr.hakchi_gui
{
    partial class ModStoreTabControl
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.moduleListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.moduleDownloadButton = new System.Windows.Forms.Button();
            this.moduleDownloadInstallButton = new System.Windows.Forms.Button();
            this.moduleDescriptionBrowser = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 243F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 283F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.webBrowser1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.moduleListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1003, 510);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowNavigation = false;
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Left;
            this.webBrowser1.Location = new System.Drawing.Point(530, 4);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(4);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(27, 25);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(454, 502);
            this.webBrowser1.TabIndex = 2;
            this.webBrowser1.Url = new System.Uri("https://www.hakchiresources.com/", System.UriKind.Absolute);
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // moduleListBox
            // 
            this.moduleListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moduleListBox.FormattingEnabled = true;
            this.moduleListBox.IntegralHeight = false;
            this.moduleListBox.ItemHeight = 16;
            this.moduleListBox.Location = new System.Drawing.Point(4, 4);
            this.moduleListBox.Margin = new System.Windows.Forms.Padding(4);
            this.moduleListBox.Name = "moduleListBox";
            this.moduleListBox.Size = new System.Drawing.Size(235, 502);
            this.moduleListBox.TabIndex = 1;
            this.moduleListBox.SelectedIndexChanged += new System.EventHandler(this.moduleListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.moduleDownloadButton);
            this.panel1.Controls.Add(this.moduleDownloadInstallButton);
            this.panel1.Controls.Add(this.moduleDescriptionBrowser);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(247, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(275, 502);
            this.panel1.TabIndex = 0;
            // 
            // moduleDownloadButton
            // 
            this.moduleDownloadButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.moduleDownloadButton.Enabled = false;
            this.moduleDownloadButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.moduleDownloadButton.Location = new System.Drawing.Point(0, 386);
            this.moduleDownloadButton.Margin = new System.Windows.Forms.Padding(4);
            this.moduleDownloadButton.Name = "moduleDownloadButton";
            this.moduleDownloadButton.Size = new System.Drawing.Size(273, 57);
            this.moduleDownloadButton.TabIndex = 9;
            this.moduleDownloadButton.Text = "Download Module";
            this.moduleDownloadButton.UseVisualStyleBackColor = true;
            this.moduleDownloadButton.Click += new System.EventHandler(this.moduleDownloadButton_Click);
            // 
            // moduleDownloadInstallButton
            // 
            this.moduleDownloadInstallButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.moduleDownloadInstallButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.moduleDownloadInstallButton.Location = new System.Drawing.Point(0, 443);
            this.moduleDownloadInstallButton.Margin = new System.Windows.Forms.Padding(4);
            this.moduleDownloadInstallButton.Name = "moduleDownloadInstallButton";
            this.moduleDownloadInstallButton.Size = new System.Drawing.Size(273, 57);
            this.moduleDownloadInstallButton.TabIndex = 8;
            this.moduleDownloadInstallButton.Text = "Download and Install Module";
            this.moduleDownloadInstallButton.UseVisualStyleBackColor = true;
            this.moduleDownloadInstallButton.Click += new System.EventHandler(this.moduleDownloadInstallButton_Click);
            // 
            // moduleDescriptionBrowser
            // 
            this.moduleDescriptionBrowser.AllowWebBrowserDrop = false;
            this.moduleDescriptionBrowser.Dock = System.Windows.Forms.DockStyle.Top;
            this.moduleDescriptionBrowser.IsWebBrowserContextMenuEnabled = false;
            this.moduleDescriptionBrowser.Location = new System.Drawing.Point(0, 0);
            this.moduleDescriptionBrowser.Margin = new System.Windows.Forms.Padding(4);
            this.moduleDescriptionBrowser.MinimumSize = new System.Drawing.Size(27, 25);
            this.moduleDescriptionBrowser.Name = "moduleDescriptionBrowser";
            this.moduleDescriptionBrowser.ScriptErrorsSuppressed = true;
            this.moduleDescriptionBrowser.ScrollBarsEnabled = false;
            this.moduleDescriptionBrowser.Size = new System.Drawing.Size(273, 386);
            this.moduleDescriptionBrowser.TabIndex = 7;
            this.moduleDescriptionBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // ModStoreTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ModStoreTabControl";
            this.Size = new System.Drawing.Size(1003, 510);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox moduleListBox;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.WebBrowser moduleDescriptionBrowser;
        private System.Windows.Forms.Button moduleDownloadButton;
        private System.Windows.Forms.Button moduleDownloadInstallButton;
    }
}
