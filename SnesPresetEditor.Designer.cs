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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnesPresetEditor));
            this.labelPresetID = new System.Windows.Forms.Label();
            this.textBoxPresetID = new System.Windows.Forms.TextBox();
            this.labelExtra = new System.Windows.Forms.Label();
            this.textBoxExtra = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelPresetID
            // 
            resources.ApplyResources(this.labelPresetID, "labelPresetID");
            this.labelPresetID.Name = "labelPresetID";
            // 
            // textBoxPresetID
            // 
            resources.ApplyResources(this.textBoxPresetID, "textBoxPresetID");
            this.textBoxPresetID.Name = "textBoxPresetID";
            // 
            // labelExtra
            // 
            resources.ApplyResources(this.labelExtra, "labelExtra");
            this.labelExtra.Name = "labelExtra";
            // 
            // textBoxExtra
            // 
            resources.ApplyResources(this.textBoxExtra, "textBoxExtra");
            this.textBoxExtra.Name = "textBoxExtra";
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // SnesPresetEditor
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxExtra);
            this.Controls.Add(this.labelExtra);
            this.Controls.Add(this.textBoxPresetID);
            this.Controls.Add(this.labelPresetID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "SnesPresetEditor";
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