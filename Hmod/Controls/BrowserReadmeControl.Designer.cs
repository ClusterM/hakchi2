namespace com.clusterrr.hakchi_gui.Hmod.Controls
{
    partial class BrowserReadmeControl
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
            this.wbReadme = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbReadme
            // 
            this.wbReadme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbReadme.Location = new System.Drawing.Point(0, 0);
            this.wbReadme.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbReadme.Name = "wbReadme";
            this.wbReadme.Size = new System.Drawing.Size(618, 465);
            this.wbReadme.TabIndex = 0;
            this.wbReadme.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbReadme_Navigating);
            // 
            // BrowserReadmeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.wbReadme);
            this.Name = "BrowserReadmeControl";
            this.Size = new System.Drawing.Size(618, 465);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbReadme;
    }
}
