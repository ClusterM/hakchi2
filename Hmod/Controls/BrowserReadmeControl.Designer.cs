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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserReadmeControl));
            this.wbReadme = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbReadme
            // 
            resources.ApplyResources(this.wbReadme, "wbReadme");
            this.wbReadme.Name = "wbReadme";
            this.wbReadme.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbReadme_Navigating);
            // 
            // BrowserReadmeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.wbReadme);
            this.Name = "BrowserReadmeControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbReadme;
    }
}
