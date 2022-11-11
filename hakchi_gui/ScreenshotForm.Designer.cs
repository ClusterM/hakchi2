using com.clusterrr.hakchi_gui.Properties;

namespace com.clusterrr.hakchi_gui
{
    partial class ScreenshotForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenshotForm));
            this.screenshotPictureBox = new System.Windows.Forms.PictureBox();
            this.screenshotContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInDefaultViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyImageToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.updateScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startStopLiveViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.takeUnattendedScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.takeScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).BeginInit();
            this.screenshotContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // screenshotPictureBox
            // 
            resources.ApplyResources(this.screenshotPictureBox, "screenshotPictureBox");
            this.screenshotPictureBox.BackColor = System.Drawing.Color.Black;
            this.screenshotPictureBox.ContextMenuStrip = this.screenshotContextMenuStrip;
            this.screenshotPictureBox.Name = "screenshotPictureBox";
            this.screenshotPictureBox.TabStop = false;
            this.screenshotPictureBox.DoubleClick += new System.EventHandler(this.screenshotPictureBox_DoubleClick);
            // 
            // screenshotContextMenuStrip
            // 
            this.screenshotContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInDefaultViewerToolStripMenuItem,
            this.copyImageToClipboardToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.toolStripSeparator1,
            this.updateScreenshotToolStripMenuItem,
            this.startStopLiveViewToolStripMenuItem,
            this.toolStripMenuItem1,
            this.takeUnattendedScreenshotToolStripMenuItem,
            this.takeScreenshotToolStripMenuItem});
            this.screenshotContextMenuStrip.Name = "screenshotContextMenuStrip";
            resources.ApplyResources(this.screenshotContextMenuStrip, "screenshotContextMenuStrip");
            // 
            // openInDefaultViewerToolStripMenuItem
            // 
            this.openInDefaultViewerToolStripMenuItem.Name = "openInDefaultViewerToolStripMenuItem";
            resources.ApplyResources(this.openInDefaultViewerToolStripMenuItem, "openInDefaultViewerToolStripMenuItem");
            this.openInDefaultViewerToolStripMenuItem.Click += new System.EventHandler(this.OpenInDefaultViewer);
            // 
            // copyImageToClipboardToolStripMenuItem
            // 
            this.copyImageToClipboardToolStripMenuItem.Name = "copyImageToClipboardToolStripMenuItem";
            resources.ApplyResources(this.copyImageToClipboardToolStripMenuItem, "copyImageToClipboardToolStripMenuItem");
            this.copyImageToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyImageToClipboard);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            resources.ApplyResources(this.saveImageToolStripMenuItem, "saveImageToolStripMenuItem");
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToFile);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // updateScreenshotToolStripMenuItem
            // 
            this.updateScreenshotToolStripMenuItem.Name = "updateScreenshotToolStripMenuItem";
            resources.ApplyResources(this.updateScreenshotToolStripMenuItem, "updateScreenshotToolStripMenuItem");
            this.updateScreenshotToolStripMenuItem.Click += new System.EventHandler(this.updateScreenshot);
            // 
            // startStopLiveViewToolStripMenuItem
            // 
            this.startStopLiveViewToolStripMenuItem.Name = "startStopLiveViewToolStripMenuItem";
            resources.ApplyResources(this.startStopLiveViewToolStripMenuItem, "startStopLiveViewToolStripMenuItem");
            this.startStopLiveViewToolStripMenuItem.Click += new System.EventHandler(this.LiveView);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // takeUnattendedScreenshotToolStripMenuItem
            // 
            this.takeUnattendedScreenshotToolStripMenuItem.Name = "takeUnattendedScreenshotToolStripMenuItem";
            resources.ApplyResources(this.takeUnattendedScreenshotToolStripMenuItem, "takeUnattendedScreenshotToolStripMenuItem");
            // 
            // saveImageFileDialog
            // 
            this.saveImageFileDialog.DefaultExt = "png";
            resources.ApplyResources(this.saveImageFileDialog, "saveImageFileDialog");
            // 
            // takeScreenshotToolStripMenuItem
            // 
            this.takeScreenshotToolStripMenuItem.Name = "takeScreenshotToolStripMenuItem";
            resources.ApplyResources(this.takeScreenshotToolStripMenuItem, "takeScreenshotToolStripMenuItem");
            this.takeScreenshotToolStripMenuItem.Click += new System.EventHandler(this.takeScreenshotToolStripMenuItem_Click);
            // 
            // ScreenshotForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.screenshotPictureBox);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "ScreenshotForm";
            this.Load += new System.EventHandler(this.ScreenshotForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenshotForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ScreenshotForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).EndInit();
            this.screenshotContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox screenshotPictureBox;
        private System.Windows.Forms.ContextMenuStrip screenshotContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openInDefaultViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveImageFileDialog;
        private System.Windows.Forms.ToolStripMenuItem startStopLiveViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem copyImageToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateScreenshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem takeUnattendedScreenshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem takeScreenshotToolStripMenuItem;
    }
}