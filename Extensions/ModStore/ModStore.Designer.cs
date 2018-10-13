namespace com.clusterrr.hakchi_gui
{
    partial class ModStore
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModStore));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.refreshContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showExperimentalModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByAZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.submitYourOwnModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discordLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.PoweredByLinkS = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage0 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl1 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl4 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl5 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl3 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl2 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl6 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.modStoreTabControl7 = new com.clusterrr.hakchi_gui.ModStoreTabControl();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshContentToolStripMenuItem,
            this.showExperimentalModsToolStripMenuItem,
            this.sortToolStripMenuItem,
            this.submitYourOwnModToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // refreshContentToolStripMenuItem
            // 
            this.refreshContentToolStripMenuItem.Name = "refreshContentToolStripMenuItem";
            resources.ApplyResources(this.refreshContentToolStripMenuItem, "refreshContentToolStripMenuItem");
            this.refreshContentToolStripMenuItem.Click += new System.EventHandler(this.refreshContentToolStripMenuItem_Click);
            // 
            // showExperimentalModsToolStripMenuItem
            // 
            this.showExperimentalModsToolStripMenuItem.Name = "showExperimentalModsToolStripMenuItem";
            resources.ApplyResources(this.showExperimentalModsToolStripMenuItem, "showExperimentalModsToolStripMenuItem");
            this.showExperimentalModsToolStripMenuItem.Click += new System.EventHandler(this.showExperimentalModsToolStripMenuItem_Click);
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByAZToolStripMenuItem,
            this.sortByDateToolStripMenuItem});
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            resources.ApplyResources(this.sortToolStripMenuItem, "sortToolStripMenuItem");
            // 
            // sortByAZToolStripMenuItem
            // 
            this.sortByAZToolStripMenuItem.Name = "sortByAZToolStripMenuItem";
            resources.ApplyResources(this.sortByAZToolStripMenuItem, "sortByAZToolStripMenuItem");
            this.sortByAZToolStripMenuItem.Click += new System.EventHandler(this.sortMethodClick);
            // 
            // sortByDateToolStripMenuItem
            // 
            this.sortByDateToolStripMenuItem.Name = "sortByDateToolStripMenuItem";
            resources.ApplyResources(this.sortByDateToolStripMenuItem, "sortByDateToolStripMenuItem");
            this.sortByDateToolStripMenuItem.Click += new System.EventHandler(this.sortMethodClick);
            // 
            // submitYourOwnModToolStripMenuItem
            // 
            this.submitYourOwnModToolStripMenuItem.Name = "submitYourOwnModToolStripMenuItem";
            resources.ApplyResources(this.submitYourOwnModToolStripMenuItem, "submitYourOwnModToolStripMenuItem");
            this.submitYourOwnModToolStripMenuItem.Click += new System.EventHandler(this.submitYourOwnModToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.discordLinkToolStripMenuItem,
            this.visitWebsiteToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // discordLinkToolStripMenuItem
            // 
            this.discordLinkToolStripMenuItem.Name = "discordLinkToolStripMenuItem";
            resources.ApplyResources(this.discordLinkToolStripMenuItem, "discordLinkToolStripMenuItem");
            this.discordLinkToolStripMenuItem.Click += new System.EventHandler(this.discordLinkToolStripMenuItem_Click);
            // 
            // visitWebsiteToolStripMenuItem
            // 
            this.visitWebsiteToolStripMenuItem.Name = "visitWebsiteToolStripMenuItem";
            resources.ApplyResources(this.visitWebsiteToolStripMenuItem, "visitWebsiteToolStripMenuItem");
            this.visitWebsiteToolStripMenuItem.Click += new System.EventHandler(this.visitWebsiteToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PoweredByLinkS});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.SizingGrip = false;
            // 
            // PoweredByLinkS
            // 
            this.PoweredByLinkS.IsLink = true;
            this.PoweredByLinkS.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.PoweredByLinkS.Name = "PoweredByLinkS";
            resources.ApplyResources(this.PoweredByLinkS, "PoweredByLinkS");
            this.PoweredByLinkS.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.PoweredByLinkS.Click += new System.EventHandler(this.PoweredByLinkS_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage0);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage0
            // 
            this.tabPage0.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.tabPage0, "tabPage0");
            this.tabPage0.Name = "tabPage0";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.modStoreTabControl1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            // 
            // modStoreTabControl1
            // 
            this.modStoreTabControl1.Category = "additional_functionality";
            resources.ApplyResources(this.modStoreTabControl1, "modStoreTabControl1");
            this.modStoreTabControl1.Name = "modStoreTabControl1";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.modStoreTabControl4);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            // 
            // modStoreTabControl4
            // 
            this.modStoreTabControl4.Category = "retroarch";
            resources.ApplyResources(this.modStoreTabControl4, "modStoreTabControl4");
            this.modStoreTabControl4.Name = "modStoreTabControl4";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Controls.Add(this.modStoreTabControl5);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            // 
            // modStoreTabControl5
            // 
            this.modStoreTabControl5.Category = "retroarch_cores";
            resources.ApplyResources(this.modStoreTabControl5, "modStoreTabControl5");
            this.modStoreTabControl5.Name = "modStoreTabControl5";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.modStoreTabControl3);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            // 
            // modStoreTabControl3
            // 
            this.modStoreTabControl3.BackColor = System.Drawing.SystemColors.Control;
            this.modStoreTabControl3.Category = "usb-host";
            resources.ApplyResources(this.modStoreTabControl3, "modStoreTabControl3");
            this.modStoreTabControl3.Name = "modStoreTabControl3";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.modStoreTabControl2);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            // 
            // modStoreTabControl2
            // 
            this.modStoreTabControl2.BackColor = System.Drawing.SystemColors.Control;
            this.modStoreTabControl2.Category = "interface_mods";
            resources.ApplyResources(this.modStoreTabControl2, "modStoreTabControl2");
            this.modStoreTabControl2.Name = "modStoreTabControl2";
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage6.Controls.Add(this.modStoreTabControl6);
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Name = "tabPage6";
            // 
            // modStoreTabControl6
            // 
            this.modStoreTabControl6.BackColor = System.Drawing.SystemColors.Control;
            this.modStoreTabControl6.Category = "game";
            resources.ApplyResources(this.modStoreTabControl6, "modStoreTabControl6");
            this.modStoreTabControl6.Name = "modStoreTabControl6";
            // 
            // tabPage7
            // 
            this.tabPage7.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage7.Controls.Add(this.modStoreTabControl7);
            resources.ApplyResources(this.tabPage7, "tabPage7");
            this.tabPage7.Name = "tabPage7";
            // 
            // modStoreTabControl7
            // 
            this.modStoreTabControl7.Category = "experimental";
            resources.ApplyResources(this.modStoreTabControl7, "modStoreTabControl7");
            this.modStoreTabControl7.Name = "modStoreTabControl7";
            // 
            // ModStore
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModStore";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModStore_FormClosing);
            this.Load += new System.EventHandler(this.ModStore_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visitWebsiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshContentToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel PoweredByLinkS;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.FontDialog fontDialog1;
        private ModStoreTabControl modStoreTabControl2;
        private ModStoreTabControl modStoreTabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private ModStoreTabControl modStoreTabControl3;
        private System.Windows.Forms.TabPage tabPage4;
        private ModStoreTabControl modStoreTabControl4;
        private System.Windows.Forms.TabPage tabPage5;
        private ModStoreTabControl modStoreTabControl5;
        private System.Windows.Forms.ToolStripMenuItem submitYourOwnModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discordLinkToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage0;
        private System.Windows.Forms.TabPage tabPage6;
        private ModStoreTabControl modStoreTabControl6;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByAZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByDateToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage7;
        private ModStoreTabControl modStoreTabControl7;
        private System.Windows.Forms.ToolStripMenuItem showExperimentalModsToolStripMenuItem;
    }
}