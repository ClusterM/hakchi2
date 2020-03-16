namespace com.clusterrr.hakchi_gui
{
    partial class LicenseInfo
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
            this.textBoxLicenses = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxLicenses
            // 
            this.textBoxLicenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLicenses.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLicenses.Location = new System.Drawing.Point(12, 12);
            this.textBoxLicenses.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxLicenses.Multiline = true;
            this.textBoxLicenses.Name = "textBoxLicenses";
            this.textBoxLicenses.ReadOnly = true;
            this.textBoxLicenses.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLicenses.Size = new System.Drawing.Size(585, 562);
            this.textBoxLicenses.TabIndex = 0;
            this.textBoxLicenses.Text = "--------------------------------------------------------------------------------";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxLicenses);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(12);
            this.panel1.Size = new System.Drawing.Size(609, 586);
            this.panel1.TabIndex = 1;
            // 
            // LicenseInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 586);
            this.Controls.Add(this.panel1);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MinimumSize = new System.Drawing.Size(625, 300);
            this.Name = "LicenseInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "License Information";
            this.Shown += new System.EventHandler(this.LicenseInfo_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLicenses;
        private System.Windows.Forms.Panel panel1;
    }
}