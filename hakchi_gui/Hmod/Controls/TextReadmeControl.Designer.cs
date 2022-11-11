namespace com.clusterrr.hakchi_gui.Hmod.Controls
{
    partial class TextReadmeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextReadmeControl));
            this.tbReadme = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbReadme
            // 
            this.tbReadme.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tbReadme, "tbReadme");
            this.tbReadme.Name = "tbReadme";
            this.tbReadme.ReadOnly = true;
            // 
            // TextReadmeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tbReadme);
            this.Name = "TextReadmeControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbReadme;
    }
}
