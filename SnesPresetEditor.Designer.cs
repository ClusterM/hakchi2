namespace com.clusterrr.hakchi_gui
{
    partial class SnesPresetEditor
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
            this.labelPresetID = new System.Windows.Forms.Label();
            this.textBoxPresetID = new System.Windows.Forms.TextBox();
            this.labelExtra = new System.Windows.Forms.Label();
            this.textBoxExtra = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelPresetID
            // 
            this.labelPresetID.AutoSize = true;
            this.labelPresetID.Location = new System.Drawing.Point(12, 22);
            this.labelPresetID.Name = "labelPresetID";
            this.labelPresetID.Size = new System.Drawing.Size(74, 13);
            this.labelPresetID.TabIndex = 0;
            this.labelPresetID.Text = "Preset ID:   0x";
            // 
            // textBoxPresetID
            // 
            this.textBoxPresetID.Location = new System.Drawing.Point(84, 19);
            this.textBoxPresetID.MaxLength = 4;
            this.textBoxPresetID.Name = "textBoxPresetID";
            this.textBoxPresetID.Size = new System.Drawing.Size(55, 20);
            this.textBoxPresetID.TabIndex = 1;
            // 
            // labelExtra
            // 
            this.labelExtra.AutoSize = true;
            this.labelExtra.Location = new System.Drawing.Point(191, 22);
            this.labelExtra.Name = "labelExtra";
            this.labelExtra.Size = new System.Drawing.Size(77, 13);
            this.labelExtra.TabIndex = 2;
            this.labelExtra.Text = "Extra byte:   0x";
            // 
            // textBoxExtra
            // 
            this.textBoxExtra.Location = new System.Drawing.Point(266, 19);
            this.textBoxExtra.MaxLength = 2;
            this.textBoxExtra.Name = "textBoxExtra";
            this.textBoxExtra.Size = new System.Drawing.Size(31, 20);
            this.textBoxExtra.TabIndex = 3;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(336, 19);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(53, 20);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // SnesPresetEditor
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 54);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxExtra);
            this.Controls.Add(this.labelExtra);
            this.Controls.Add(this.textBoxPresetID);
            this.Controls.Add(this.labelPresetID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "SnesPresetEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SNES Preset ID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SnesPresetEditor_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPresetID;
        private System.Windows.Forms.TextBox textBoxPresetID;
        private System.Windows.Forms.Label labelExtra;
        private System.Windows.Forms.TextBox textBoxExtra;
        private System.Windows.Forms.Button buttonOk;
    }
}