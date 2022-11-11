namespace com.clusterrr.hakchi_gui
{
    partial class ImageGooglerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageGooglerForm));
            this.imageGoogler = new com.clusterrr.hakchi_gui.Controls.ImageGoogler();
            this.SuspendLayout();
            // 
            // imageGoogler
            // 
            resources.ApplyResources(this.imageGoogler, "imageGoogler");
            this.imageGoogler.Name = "imageGoogler";
            this.imageGoogler.OnImageDoubleClicked += new com.clusterrr.hakchi_gui.Controls.ImageGoogler.ImageReceived(this.imageGoogler_OnImageDoubleClicked);
            // 
            // ImageGooglerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.imageGoogler);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MinimizeBox = false;
            this.Name = "ImageGooglerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageGooglerForm_FormClosing);
            this.Shown += new System.EventHandler(this.ImageGooglerForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ImageGoogler imageGoogler;
    }
}