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
            this.tbReadme = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbReadme
            // 
            this.tbReadme.BackColor = System.Drawing.SystemColors.Window;
            this.tbReadme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbReadme.Location = new System.Drawing.Point(0, 0);
            this.tbReadme.MaxLength = 999999999;
            this.tbReadme.Multiline = true;
            this.tbReadme.Name = "tbReadme";
            this.tbReadme.ReadOnly = true;
            this.tbReadme.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbReadme.Size = new System.Drawing.Size(618, 465);
            this.tbReadme.TabIndex = 0;
            // 
            // TextReadmeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tbReadme);
            this.Name = "TextReadmeControl";
            this.Size = new System.Drawing.Size(618, 465);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbReadme;
    }
}
