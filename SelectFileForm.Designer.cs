namespace com.clusterrr.hakchi_gui
{
    partial class SelectFileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectFileForm));
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonArchive = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxFiles
            // 
            resources.ApplyResources(this.listBoxFiles, "listBoxFiles");
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxFiles_SelectedIndexChanged);
            this.listBoxFiles.DoubleClick += new System.EventHandler(this.listBoxFiles_DoubleClick);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.listBoxFiles_DoubleClick);
            // 
            // buttonArchive
            // 
            resources.ApplyResources(this.buttonArchive, "buttonArchive");
            this.buttonArchive.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.buttonArchive.Name = "buttonArchive";
            this.buttonArchive.UseVisualStyleBackColor = true;
            // 
            // SelectFileForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonArchive);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.listBoxFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MinimizeBox = false;
            this.Name = "SelectFileForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        internal System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonArchive;
    }
}