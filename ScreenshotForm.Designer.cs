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
            this.screenshotPictureBox = new System.Windows.Forms.PictureBox();
            this.screenshotContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInDefaultViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyImageToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.updateScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startStopLiveViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.takeUnattendedScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).BeginInit();
            this.screenshotContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // screenshotPictureBox
            // 
            this.screenshotPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenshotPictureBox.ContextMenuStrip = this.screenshotContextMenuStrip;
            this.screenshotPictureBox.Location = new System.Drawing.Point(0, 0);
            this.screenshotPictureBox.Name = "screenshotPictureBox";
            this.screenshotPictureBox.Size = new System.Drawing.Size(853, 480);
            this.screenshotPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.screenshotPictureBox.TabIndex = 0;
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
            this.takeUnattendedScreenshotToolStripMenuItem});
            this.screenshotContextMenuStrip.Name = "screenshotContextMenuStrip";
            this.screenshotContextMenuStrip.Size = new System.Drawing.Size(248, 148);
            // 
            // openInDefaultViewerToolStripMenuItem
            // 
            this.openInDefaultViewerToolStripMenuItem.Name = "openInDefaultViewerToolStripMenuItem";
            this.openInDefaultViewerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openInDefaultViewerToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.openInDefaultViewerToolStripMenuItem.Text = "&Open in default viewer";
            this.openInDefaultViewerToolStripMenuItem.Click += new System.EventHandler(this.OpenInDefaultViewer);
            // 
            // copyImageToClipboardToolStripMenuItem
            // 
            this.copyImageToClipboardToolStripMenuItem.Name = "copyImageToClipboardToolStripMenuItem";
            this.copyImageToClipboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyImageToClipboardToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.copyImageToClipboardToolStripMenuItem.Text = "&Copy image to clipboard";
            this.copyImageToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyImageToClipboard);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.saveImageToolStripMenuItem.Text = "&Save image to file";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToFile);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(244, 6);
            // 
            // updateScreenshotToolStripMenuItem
            // 
            this.updateScreenshotToolStripMenuItem.Name = "updateScreenshotToolStripMenuItem";
            this.updateScreenshotToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.updateScreenshotToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.updateScreenshotToolStripMenuItem.Text = "&Refresh screenshot";
            this.updateScreenshotToolStripMenuItem.Click += new System.EventHandler(this.updateScreenshot);
            // 
            // startStopLiveViewToolStripMenuItem
            // 
            this.startStopLiveViewToolStripMenuItem.Name = "startStopLiveViewToolStripMenuItem";
            this.startStopLiveViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.startStopLiveViewToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.startStopLiveViewToolStripMenuItem.Text = "Enable &auto refresh";
            this.startStopLiveViewToolStripMenuItem.Click += new System.EventHandler(this.LiveView);
            // 
            // saveImageFileDialog
            // 
            this.saveImageFileDialog.DefaultExt = "png";
            this.saveImageFileDialog.Filter = "PNG Images|*.png";
            // 
            // takeUnattendedScreenshotToolStripMenuItem
            // 
            this.takeUnattendedScreenshotToolStripMenuItem.Name = "takeUnattendedScreenshotToolStripMenuItem";
            this.takeUnattendedScreenshotToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.takeUnattendedScreenshotToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.takeUnattendedScreenshotToolStripMenuItem.Text = "Take unattended screenshot";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(244, 6);
            // 
            // ScreenshotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 480);
            this.Controls.Add(this.screenshotPictureBox);
            this.Name = "ScreenshotForm";
            this.Text = "Screenshot taken on {0} at {1}";
            this.Load += new System.EventHandler(this.ScreenshotForm_Load);
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
    }
}