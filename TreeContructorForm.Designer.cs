namespace com.clusterrr.hakchi_gui
{
    partial class TreeContructorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeContructorForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.comboBoxPosition = new System.Windows.Forms.ComboBox();
            this.labelPosition = new System.Windows.Forms.Label();
            this.buttonNewFolder = new System.Windows.Forms.Button();
            this.listViewContent = new System.Windows.Forms.ListView();
            this.groupBoxSplitModes = new System.Windows.Forms.GroupBox();
            this.buttonNoFoldersOriginal = new System.Windows.Forms.Button();
            this.buttonNoFolders = new System.Windows.Forms.Button();
            this.buttonFoldersLetters = new System.Windows.Forms.Button();
            this.buttonFoldersEquallyOriginal = new System.Windows.Forms.Button();
            this.buttonFoldersLettersOriginal = new System.Windows.Forms.Button();
            this.buttonFoldersEqually = new System.Windows.Forms.Button();
            this.groupBoxArt = new System.Windows.Forms.GroupBox();
            this.pictureBoxArt = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSep = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelElementCount = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxSplitModes.SuspendLayout();
            this.groupBoxArt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.comboBoxPosition);
            this.splitContainer.Panel2.Controls.Add(this.labelPosition);
            this.splitContainer.Panel2.Controls.Add(this.buttonNewFolder);
            this.splitContainer.Panel2.Controls.Add(this.listViewContent);
            this.splitContainer.Panel2.Controls.Add(this.groupBoxSplitModes);
            this.splitContainer.Panel2.Controls.Add(this.groupBoxArt);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageList = this.imageList;
            this.treeView.LabelEdit = true;
            this.treeView.Name = "treeView";
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "folder_sm.png");
            this.imageList.Images.SetKeyName(1, "folder_sm_tr.png");
            this.imageList.Images.SetKeyName(2, "original_sm.png");
            this.imageList.Images.SetKeyName(3, "original_sm_tr.png");
            this.imageList.Images.SetKeyName(4, "app_sm.png");
            this.imageList.Images.SetKeyName(5, "app_sm_tr.png");
            this.imageList.Images.SetKeyName(6, "nes_sm.png");
            this.imageList.Images.SetKeyName(7, "nes_sm_tr.png");
            this.imageList.Images.SetKeyName(8, "famicom_sm.png");
            this.imageList.Images.SetKeyName(9, "famicom_sm_tr.png");
            this.imageList.Images.SetKeyName(10, "fds_sm.png");
            this.imageList.Images.SetKeyName(11, "fds_sm_tr.png");
            this.imageList.Images.SetKeyName(12, "snes-us_sm.png");
            this.imageList.Images.SetKeyName(13, "snes-us_sm_tr.png");
            this.imageList.Images.SetKeyName(14, "n64_sm.png");
            this.imageList.Images.SetKeyName(15, "n64_sm_tr.png");
            this.imageList.Images.SetKeyName(16, "sms_sm.png");
            this.imageList.Images.SetKeyName(17, "sms_sm_tr.png");
            this.imageList.Images.SetKeyName(18, "genesis_sm.png");
            this.imageList.Images.SetKeyName(19, "genesis_sm_tr.png");
            this.imageList.Images.SetKeyName(20, "pce_sm.png");
            this.imageList.Images.SetKeyName(21, "pce_sm_tr.png");
            this.imageList.Images.SetKeyName(22, "gb_sm.png");
            this.imageList.Images.SetKeyName(23, "gb_sm_tr.png");
            this.imageList.Images.SetKeyName(24, "gbc_sm.png");
            this.imageList.Images.SetKeyName(25, "gbc_sm_tr.png");
            this.imageList.Images.SetKeyName(26, "gba_sm.png");
            this.imageList.Images.SetKeyName(27, "gba_sm_tr.png");
            // 
            // comboBoxPosition
            // 
            this.comboBoxPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPosition.FormattingEnabled = true;
            this.comboBoxPosition.Items.AddRange(new object[] {
            resources.GetString("comboBoxPosition.Items"),
            resources.GetString("comboBoxPosition.Items1"),
            resources.GetString("comboBoxPosition.Items2"),
            resources.GetString("comboBoxPosition.Items3")});
            resources.ApplyResources(this.comboBoxPosition, "comboBoxPosition");
            this.comboBoxPosition.Name = "comboBoxPosition";
            this.comboBoxPosition.SelectionChangeCommitted += new System.EventHandler(this.comboBoxPosition_SelectionChangeCommitted);
            // 
            // labelPosition
            // 
            resources.ApplyResources(this.labelPosition, "labelPosition");
            this.labelPosition.Name = "labelPosition";
            // 
            // buttonNewFolder
            // 
            resources.ApplyResources(this.buttonNewFolder, "buttonNewFolder");
            this.buttonNewFolder.Name = "buttonNewFolder";
            this.buttonNewFolder.UseVisualStyleBackColor = true;
            this.buttonNewFolder.Click += new System.EventHandler(this.buttonNewFolder_Click);
            // 
            // listViewContent
            // 
            this.listViewContent.AllowDrop = true;
            resources.ApplyResources(this.listViewContent, "listViewContent");
            this.listViewContent.LabelEdit = true;
            this.listViewContent.LargeImageList = this.imageList;
            this.listViewContent.Name = "listViewContent";
            this.listViewContent.SmallImageList = this.imageList;
            this.listViewContent.UseCompatibleStateImageBehavior = false;
            this.listViewContent.View = System.Windows.Forms.View.List;
            this.listViewContent.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewContent_AfterLabelEdit);
            this.listViewContent.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewContent_BeforeLabelEdit);
            this.listViewContent.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewContent_ItemDrag);
            this.listViewContent.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.listViewContent.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.listViewContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewContent_KeyDown);
            this.listViewContent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listViewContent_KeyPress);
            this.listViewContent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewContent_MouseDoubleClick);
            this.listViewContent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
            // 
            // groupBoxSplitModes
            // 
            resources.ApplyResources(this.groupBoxSplitModes, "groupBoxSplitModes");
            this.groupBoxSplitModes.Controls.Add(this.button1);
            this.groupBoxSplitModes.Controls.Add(this.buttonNoFoldersOriginal);
            this.groupBoxSplitModes.Controls.Add(this.buttonNoFolders);
            this.groupBoxSplitModes.Controls.Add(this.buttonFoldersLetters);
            this.groupBoxSplitModes.Controls.Add(this.buttonFoldersEquallyOriginal);
            this.groupBoxSplitModes.Controls.Add(this.buttonFoldersLettersOriginal);
            this.groupBoxSplitModes.Controls.Add(this.buttonFoldersEqually);
            this.groupBoxSplitModes.Name = "groupBoxSplitModes";
            this.groupBoxSplitModes.TabStop = false;
            // 
            // buttonNoFoldersOriginal
            // 
            resources.ApplyResources(this.buttonNoFoldersOriginal, "buttonNoFoldersOriginal");
            this.buttonNoFoldersOriginal.Name = "buttonNoFoldersOriginal";
            this.buttonNoFoldersOriginal.UseVisualStyleBackColor = true;
            this.buttonNoFoldersOriginal.Click += new System.EventHandler(this.buttonNoFoldersOriginal_Click);
            // 
            // buttonNoFolders
            // 
            resources.ApplyResources(this.buttonNoFolders, "buttonNoFolders");
            this.buttonNoFolders.Name = "buttonNoFolders";
            this.buttonNoFolders.UseVisualStyleBackColor = true;
            this.buttonNoFolders.Click += new System.EventHandler(this.buttonNoFolders_Click);
            // 
            // buttonFoldersLetters
            // 
            resources.ApplyResources(this.buttonFoldersLetters, "buttonFoldersLetters");
            this.buttonFoldersLetters.Name = "buttonFoldersLetters";
            this.buttonFoldersLetters.UseVisualStyleBackColor = true;
            this.buttonFoldersLetters.Click += new System.EventHandler(this.buttonFoldersLetters_Click);
            // 
            // buttonFoldersEquallyOriginal
            // 
            resources.ApplyResources(this.buttonFoldersEquallyOriginal, "buttonFoldersEquallyOriginal");
            this.buttonFoldersEquallyOriginal.Name = "buttonFoldersEquallyOriginal";
            this.buttonFoldersEquallyOriginal.UseVisualStyleBackColor = true;
            this.buttonFoldersEquallyOriginal.Click += new System.EventHandler(this.buttonFoldersEquallyOriginal_Click);
            // 
            // buttonFoldersLettersOriginal
            // 
            resources.ApplyResources(this.buttonFoldersLettersOriginal, "buttonFoldersLettersOriginal");
            this.buttonFoldersLettersOriginal.Name = "buttonFoldersLettersOriginal";
            this.buttonFoldersLettersOriginal.UseVisualStyleBackColor = true;
            this.buttonFoldersLettersOriginal.Click += new System.EventHandler(this.buttonFoldersLettersOriginal_Click);
            // 
            // buttonFoldersEqually
            // 
            resources.ApplyResources(this.buttonFoldersEqually, "buttonFoldersEqually");
            this.buttonFoldersEqually.Name = "buttonFoldersEqually";
            this.buttonFoldersEqually.UseVisualStyleBackColor = true;
            this.buttonFoldersEqually.Click += new System.EventHandler(this.buttonFoldersEqually_Click);
            // 
            // groupBoxArt
            // 
            this.groupBoxArt.Controls.Add(this.pictureBoxArt);
            resources.ApplyResources(this.groupBoxArt, "groupBoxArt");
            this.groupBoxArt.Name = "groupBoxArt";
            this.groupBoxArt.TabStop = false;
            // 
            // pictureBoxArt
            // 
            this.pictureBoxArt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxArt.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pictureBoxArt, "pictureBoxArt");
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.TabStop = false;
            this.pictureBoxArt.Click += new System.EventHandler(this.pictureBoxArt_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripMenuItemSep,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            resources.ApplyResources(this.newFolderToolStripMenuItem, "newFolderToolStripMenuItem");
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripMenuItemSep
            // 
            this.toolStripMenuItemSep.Name = "toolStripMenuItemSep";
            resources.ApplyResources(this.toolStripMenuItemSep, "toolStripMenuItemSep");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // labelElementCount
            // 
            resources.ApplyResources(this.labelElementCount, "labelElementCount");
            this.labelElementCount.Name = "labelElementCount";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.buttonOk);
            this.panel2.Controls.Add(this.labelElementCount);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TreeContructorForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitContainer);
            this.Name = "TreeContructorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TreeContructorForm_FormClosing);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxSplitModes.ResumeLayout(false);
            this.groupBoxArt.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBoxArt;
        private System.Windows.Forms.PictureBox pictureBoxArt;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBoxSplitModes;
        private System.Windows.Forms.Button buttonNoFoldersOriginal;
        private System.Windows.Forms.Button buttonNoFolders;
        private System.Windows.Forms.Button buttonFoldersLetters;
        private System.Windows.Forms.Button buttonFoldersEquallyOriginal;
        private System.Windows.Forms.Button buttonFoldersLettersOriginal;
        private System.Windows.Forms.Button buttonFoldersEqually;
        private System.Windows.Forms.ListView listViewContent;
        private System.Windows.Forms.Label labelElementCount;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Button buttonNewFolder;
        private System.Windows.Forms.ComboBox comboBoxPosition;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItemSep;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}