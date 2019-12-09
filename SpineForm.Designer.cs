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
            this.imageGoogler1.AdditionalVariables = "";
            this.imageGoogler1.Location = new System.Drawing.Point(12, 12);
            this.imageGoogler1.Name = "imageGoogler1";
            this.imageGoogler1.Query = "";
            this.imageGoogler1.Size = new System.Drawing.Size(583, 454);
            this.imageGoogler1.TabIndex = 0;
            this.imageGoogler1.OnImageSelected += new com.clusterrr.hakchi_gui.Controls.ImageGoogler.ImageReceived(this.imageGoogler1_OnImageSelected);
            // 
            // pictureBoxSpine
            // 
            this.pictureBoxSpine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxSpine.Location = new System.Drawing.Point(679, 12);
            this.pictureBoxSpine.Name = "pictureBoxSpine";
            this.pictureBoxSpine.Size = new System.Drawing.Size(30, 216);
            this.pictureBoxSpine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxSpine.TabIndex = 1;
            this.pictureBoxSpine.TabStop = false;
            // 
            // buttonOk
            // 
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(601, 443);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(187, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // listBoxTemplates
            // 
            this.listBoxTemplates.FormattingEnabled = true;
            this.listBoxTemplates.Location = new System.Drawing.Point(601, 234);
            this.listBoxTemplates.Name = "listBoxTemplates";
            this.listBoxTemplates.Size = new System.Drawing.Size(187, 173);
            this.listBoxTemplates.TabIndex = 3;
            this.listBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.listBoxTemplates_SelectedIndexChanged);
            // 
            // buttonLoadLogo
            // 
            this.buttonLoadLogo.Enabled = false;
            this.buttonLoadLogo.Location = new System.Drawing.Point(601, 414);
            this.buttonLoadLogo.Name = "buttonLoadLogo";
            this.buttonLoadLogo.Size = new System.Drawing.Size(187, 23);
            this.buttonLoadLogo.TabIndex = 2;
            this.buttonLoadLogo.Text = "Open Clear Logo";
            this.buttonLoadLogo.UseVisualStyleBackColor = true;
            this.buttonLoadLogo.Click += new System.EventHandler(this.buttonLoadLogo_Click);
            // 
            // SpineForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 478);
            this.Controls.Add(this.listBoxTemplates);
            this.Controls.Add(this.buttonLoadLogo);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.pictureBoxSpine);
            this.Controls.Add(this.imageGoogler1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpineForm";
            this.Text = "SpineForm";
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