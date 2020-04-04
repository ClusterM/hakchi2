namespace com.clusterrr.hakchi_gui
{
    partial class SpineForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpineForm));
            this.imageGoogler1 = new com.clusterrr.hakchi_gui.Controls.ImageGoogler();
            this.pictureBoxSpine = new System.Windows.Forms.PictureBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.listBoxTemplates = new System.Windows.Forms.ListBox();
            this.buttonLoadLogo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpine)).BeginInit();
            this.SuspendLayout();
            // 
            // imageGoogler1
            // 
            resources.ApplyResources(this.imageGoogler1, "imageGoogler1");
            this.imageGoogler1.Name = "imageGoogler1";
            this.imageGoogler1.OnImageSelected += new com.clusterrr.hakchi_gui.Controls.ImageGoogler.ImageReceived(this.imageGoogler1_OnImageSelected);
            this.imageGoogler1.OnImageDeselected += new com.clusterrr.hakchi_gui.Controls.ImageGoogler.ImageDeselected(this.imageGoogler1_OnImageDeselected);
            // 
            // pictureBoxSpine
            // 
            resources.ApplyResources(this.pictureBoxSpine, "pictureBoxSpine");
            this.pictureBoxSpine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxSpine.Name = "pictureBoxSpine";
            this.pictureBoxSpine.TabStop = false;
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // listBoxTemplates
            // 
            resources.ApplyResources(this.listBoxTemplates, "listBoxTemplates");
            this.listBoxTemplates.FormattingEnabled = true;
            this.listBoxTemplates.Name = "listBoxTemplates";
            this.listBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.listBoxTemplates_SelectedIndexChanged);
            // 
            // buttonLoadLogo
            // 
            resources.ApplyResources(this.buttonLoadLogo, "buttonLoadLogo");
            this.buttonLoadLogo.Name = "buttonLoadLogo";
            this.buttonLoadLogo.UseVisualStyleBackColor = true;
            this.buttonLoadLogo.Click += new System.EventHandler(this.buttonLoadLogo_Click);
            // 
            // SpineForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBoxTemplates);
            this.Controls.Add(this.buttonLoadLogo);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.pictureBoxSpine);
            this.Controls.Add(this.imageGoogler1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpineForm";
            this.Load += new System.EventHandler(this.SpineForm_Load);
            this.Shown += new System.EventHandler(this.SpineForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpine)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ImageGoogler imageGoogler1;
        private System.Windows.Forms.PictureBox pictureBoxSpine;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ListBox listBoxTemplates;
        private System.Windows.Forms.Button buttonLoadLogo;
    }
}