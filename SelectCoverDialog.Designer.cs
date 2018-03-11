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
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.filenameColumnHeader,
            this.extColumnHeader,
            this.systemColumnHeader,
            this.coverColumnHeader});
            this.listViewGames.FullRowSelect = true;
            this.listViewGames.HideSelection = false;
            this.listViewGames.Location = new System.Drawing.Point(12, 31);
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.ShowGroups = false;
            this.listViewGames.Size = new System.Drawing.Size(710, 225);
            this.listViewGames.TabIndex = 1;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewGames_ColumnClick);
            this.listViewGames.SelectedIndexChanged += new System.EventHandler(this.listViewGames_SelectedIndexChanged);
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Name";
            this.nameColumnHeader.Width = 40;
            // 
            // filenameColumnHeader
            // 
            this.filenameColumnHeader.Text = "Filename";
            this.filenameColumnHeader.Width = 54;
            // 
            // extColumnHeader
            // 
            this.extColumnHeader.Text = "Ext";
            this.extColumnHeader.Width = 27;
            // 
            // systemColumnHeader
            // 
            this.systemColumnHeader.Text = "System";
            this.systemColumnHeader.Width = 46;
            // 
            // coverColumnHeader
            // 
            this.coverColumnHeader.Text = "Box art";
            this.coverColumnHeader.Width = 539;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select appropriate box art for the following games:";
            // 
            // listViewImages
            // 
            this.listViewImages.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listViewImages.HideSelection = false;
            this.listViewImages.LargeImageList = this.imageList;
            this.listViewImages.Location = new System.Drawing.Point(12, 262);
            this.listViewImages.Name = "listViewImages";
            this.listViewImages.Size = new System.Drawing.Size(710, 184);
            this.listViewImages.TabIndex = 2;
            this.listViewImages.UseCompatibleStateImageBehavior = false;
            this.listViewImages.SelectedIndexChanged += new System.EventHandler(this.listViewImages_SelectedIndexChanged);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(114, 102);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonDiscard
            // 
            this.buttonDiscard.Location = new System.Drawing.Point(647, 452);
            this.buttonDiscard.Name = "buttonDiscard";
            this.buttonDiscard.Size = new System.Drawing.Size(75, 23);
            this.buttonDiscard.TabIndex = 7;
            this.buttonDiscard.Text = "Discard";
            this.buttonDiscard.UseVisualStyleBackColor = true;
            this.buttonDiscard.Click += new System.EventHandler(this.buttonDiscard_Click);
            // 
            // buttonSetDefault
            // 
            this.buttonSetDefault.Location = new System.Drawing.Point(69, 452);
            this.buttonSetDefault.Name = "buttonSetDefault";
            this.buttonSetDefault.Size = new System.Drawing.Size(131, 23);
            this.buttonSetDefault.TabIndex = 4;
            this.buttonSetDefault.Text = "&Default / no change";
            this.buttonSetDefault.UseVisualStyleBackColor = true;
            this.buttonSetDefault.Click += new System.EventHandler(this.buttonSetDefault_Click);
            // 
            // buttonImFeelingLucky
            // 
            this.buttonImFeelingLucky.Location = new System.Drawing.Point(206, 452);
            this.buttonImFeelingLucky.Name = "buttonImFeelingLucky";
            this.buttonImFeelingLucky.Size = new System.Drawing.Size(131, 23);
            this.buttonImFeelingLucky.TabIndex = 5;
            this.buttonImFeelingLucky.Text = "I\'m feeling &lucky!";
            this.buttonImFeelingLucky.UseVisualStyleBackColor = true;
            this.buttonImFeelingLucky.Click += new System.EventHandler(this.buttonImFeelingLucky_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 457);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Set all to:";
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(566, 452);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 8;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // SelectCoverDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 487);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonImFeelingLucky);
            this.Controls.Add(this.buttonSetDefault);
            this.Controls.Add(this.buttonDiscard);
            this.Controls.Add(this.listViewImages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewGames);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(750, 605);
            this.MinimizeBox = false;
            this.Name = "SelectCoverDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Box Art";
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