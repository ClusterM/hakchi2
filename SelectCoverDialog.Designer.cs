namespace com.clusterrr.hakchi_gui
{
    partial class SelectCoverDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectCoverDialog));
            this.listViewGames = new System.Windows.Forms.ListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filenameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.extColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.systemColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.coverColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.listViewImages = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonDiscard = new System.Windows.Forms.Button();
            this.buttonSetDefault = new System.Windows.Forms.Button();
            this.buttonImFeelingLucky = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewGames
            // 
            resources.ApplyResources(this.listViewGames, "listViewGames");
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.filenameColumnHeader,
            this.extColumnHeader,
            this.systemColumnHeader,
            this.coverColumnHeader});
            this.listViewGames.FullRowSelect = true;
            this.listViewGames.HideSelection = false;
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.ShowGroups = false;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewGames_ColumnClick);
            this.listViewGames.SelectedIndexChanged += new System.EventHandler(this.listViewGames_SelectedIndexChanged);
            // 
            // nameColumnHeader
            // 
            resources.ApplyResources(this.nameColumnHeader, "nameColumnHeader");
            // 
            // filenameColumnHeader
            // 
            resources.ApplyResources(this.filenameColumnHeader, "filenameColumnHeader");
            // 
            // extColumnHeader
            // 
            resources.ApplyResources(this.extColumnHeader, "extColumnHeader");
            // 
            // systemColumnHeader
            // 
            resources.ApplyResources(this.systemColumnHeader, "systemColumnHeader");
            // 
            // coverColumnHeader
            // 
            resources.ApplyResources(this.coverColumnHeader, "coverColumnHeader");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // listViewImages
            // 
            resources.ApplyResources(this.listViewImages, "listViewImages");
            this.listViewImages.HideSelection = false;
            this.listViewImages.LargeImageList = this.imageList;
            this.listViewImages.Name = "listViewImages";
            this.listViewImages.UseCompatibleStateImageBehavior = false;
            this.listViewImages.SelectedIndexChanged += new System.EventHandler(this.listViewImages_SelectedIndexChanged);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imageList, "imageList");
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonDiscard
            // 
            resources.ApplyResources(this.buttonDiscard, "buttonDiscard");
            this.buttonDiscard.Name = "buttonDiscard";
            this.buttonDiscard.UseVisualStyleBackColor = true;
            this.buttonDiscard.Click += new System.EventHandler(this.buttonDiscard_Click);
            // 
            // buttonSetDefault
            // 
            resources.ApplyResources(this.buttonSetDefault, "buttonSetDefault");
            this.buttonSetDefault.Name = "buttonSetDefault";
            this.buttonSetDefault.UseVisualStyleBackColor = true;
            this.buttonSetDefault.Click += new System.EventHandler(this.buttonSetDefault_Click);
            // 
            // buttonImFeelingLucky
            // 
            resources.ApplyResources(this.buttonImFeelingLucky, "buttonImFeelingLucky");
            this.buttonImFeelingLucky.Name = "buttonImFeelingLucky";
            this.buttonImFeelingLucky.UseVisualStyleBackColor = true;
            this.buttonImFeelingLucky.Click += new System.EventHandler(this.buttonImFeelingLucky_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonAccept
            // 
            resources.ApplyResources(this.buttonAccept, "buttonAccept");
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // SelectCoverDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonImFeelingLucky);
            this.Controls.Add(this.buttonSetDefault);
            this.Controls.Add(this.buttonDiscard);
            this.Controls.Add(this.listViewImages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewGames);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectCoverDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectCoverDialog_FormClosing);
            this.Shown += new System.EventHandler(this.SelectCoverDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.ColumnHeader filenameColumnHeader;
        private System.Windows.Forms.ColumnHeader extColumnHeader;
        private System.Windows.Forms.ColumnHeader systemColumnHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewImages;
        private System.Windows.Forms.Button buttonDiscard;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ColumnHeader coverColumnHeader;
        private System.Windows.Forms.Button buttonSetDefault;
        private System.Windows.Forms.Button buttonImFeelingLucky;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
    }
}