namespace com.clusterrr.hakchi_gui
{
    partial class SelectIconForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectIconForm));
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.pictureBoxArt = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox
            // 
            resources.ApplyResources(this.listBox, "listBox");
            this.listBox.FormattingEnabled = true;
            this.listBox.Name = "listBox";
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // pictureBoxArt
            // 
            resources.ApplyResources(this.pictureBoxArt, "pictureBoxArt");
            this.pictureBoxArt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.TabStop = false;
            // 
            // SelectIconForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxArt);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.listBox);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MinimizeBox = false;
            this.Name = "SelectIconForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        internal System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.PictureBox pictureBoxArt;
    }
}