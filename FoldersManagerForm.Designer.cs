namespace com.clusterrr.hakchi_gui
{
    partial class FoldersManagerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoldersManagerForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonNewFolder = new System.Windows.Forms.Button();
            this.comboBoxPosition = new System.Windows.Forms.ComboBox();
            this.labelPosition1 = new System.Windows.Forms.Label();
            this.listViewContent = new System.Windows.Forms.ListView();
            this.groupBoxSplitModes = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelSplit = new System.Windows.Forms.TableLayoutPanel();
            this.buttonFoldersApp = new System.Windows.Forms.Button();
            this.buttonNoFoldersOriginal = new System.Windows.Forms.Button();
            this.buttonNoFolders = new System.Windows.Forms.Button();
            this.buttonFoldersLetters = new System.Windows.Forms.Button();
            this.buttonFoldersEqually = new System.Windows.Forms.Button();
            this.buttonFoldersGenre = new System.Windows.Forms.Button();
            this.groupBoxArt = new System.Windows.Forms.GroupBox();
            this.pictureBoxArt = new System.Windows.Forms.PictureBox();
            this.comboBoxBackPosition = new System.Windows.Forms.ComboBox();
            this.labelPosition2 = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSep = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelElementCount = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxAddHome = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxSplitModes.SuspendLayout();
            this.tableLayoutPanelSplit.SuspendLayout();
            this.groupBoxArt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.splitContainer.Panel2.Controls.Add(this.buttonNewFolder);
            this.splitContainer.Panel2.Controls.Add(this.comboBoxPosition);
            this.splitContainer.Panel2.Controls.Add(this.labelPosition1);
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
            this.imageList.Images.SetKeyName(0, "32x_sm.png");
            this.imageList.Images.SetKeyName(1, "32x_sm_tr.png");
            this.imageList.Images.SetKeyName(2, "2600_sm.png");
            this.imageList.Images.SetKeyName(3, "2600_sm_tr.png");
            this.imageList.Images.SetKeyName(4, "app_sm.png");
            this.imageList.Images.SetKeyName(5, "app_sm_tr.png");
            this.imageList.Images.SetKeyName(6, "arcade_sm.png");
            this.imageList.Images.SetKeyName(7, "arcade_sm_tr.png");
            this.imageList.Images.SetKeyName(8, "famicom_sm.png");
            this.imageList.Images.SetKeyName(9, "famicom_sm_tr.png");
            this.imageList.Images.SetKeyName(10, "fds_sm.png");
            this.imageList.Images.SetKeyName(11, "fds_sm_tr.png");
            this.imageList.Images.SetKeyName(12, "folder_sm.png");
            this.imageList.Images.SetKeyName(13, "folder_sm_tr.png");
            this.imageList.Images.SetKeyName(14, "gb_sm.png");
            this.imageList.Images.SetKeyName(15, "gb_sm_tr.png");
            this.imageList.Images.SetKeyName(16, "gba_sm.png");
            this.imageList.Images.SetKeyName(17, "gba_sm_tr.png");
            this.imageList.Images.SetKeyName(18, "gbc_sm.png");
            this.imageList.Images.SetKeyName(19, "gbc_sm_tr.png");
            this.imageList.Images.SetKeyName(20, "genesis_sm.png");
            this.imageList.Images.SetKeyName(21, "genesis_sm_tr.png");
            this.imageList.Images.SetKeyName(22, "gg_sm.png");
            this.imageList.Images.SetKeyName(23, "gg_sm_tr.png");
            this.imageList.Images.SetKeyName(24, "n64_sm.png");
            this.imageList.Images.SetKeyName(25, "n64_sm_tr.png");
            this.imageList.Images.SetKeyName(26, "neogeo_sm.png");
            this.imageList.Images.SetKeyName(27, "neogeo_sm_tr.png");
            this.imageList.Images.SetKeyName(28, "nes_sm.png");
            this.imageList.Images.SetKeyName(29, "nes_sm_tr.png");
            this.imageList.Images.SetKeyName(30, "original_sm.png");
            this.imageList.Images.SetKeyName(31, "original_sm_tr.png");
            this.imageList.Images.SetKeyName(32, "pce_sm.png");
            this.imageList.Images.SetKeyName(33, "pce_sm_tr.png");
            this.imageList.Images.SetKeyName(34, "sms_sm.png");
            this.imageList.Images.SetKeyName(35, "sms_sm_tr.png");
            this.imageList.Images.SetKeyName(36, "snes-us_sm.png");
            this.imageList.Images.SetKeyName(37, "snes-us_sm_tr.png");
            // 
            // buttonNewFolder
            // 
            resources.ApplyResources(this.buttonNewFolder, "buttonNewFolder");
            this.buttonNewFolder.Name = "buttonNewFolder";
            this.buttonNewFolder.UseVisualStyleBackColor = true;
            this.buttonNewFolder.Click += new System.EventHandler(this.buttonNewFolder_Click);
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
            // labelPosition1
            // 
            resources.ApplyResources(this.labelPosition1, "labelPosition1");
            this.labelPosition1.Name = "labelPosition1";
            // 
            // listViewContent
            // 
            this.listViewContent.AllowDrop = true;
            resources.ApplyResources(this.listViewContent, "listViewContent");
            this.listViewContent.HideSelection = false;
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
            this.groupBoxSplitModes.Controls.Add(this.tableLayoutPanelSplit);
            this.groupBoxSplitModes.Name = "groupBoxSplitModes";
            this.groupBoxSplitModes.TabStop = false;
            // 
            // tableLayoutPanelSplit
            // 
            resources.ApplyResources(this.tableLayoutPanelSplit, "tableLayoutPanelSplit");
            this.tableLayoutPanelSplit.Controls.Add(this.buttonFoldersApp, 0, 4);
            this.tableLayoutPanelSplit.Controls.Add(this.buttonNoFolders, 0, 0);
            this.tableLayoutPanelSplit.Controls.Add(this.buttonFoldersLetters, 0, 3);
            this.tableLayoutPanelSplit.Controls.Add(this.buttonNoFoldersOriginal, 1, 0);
            this.tableLayoutPanelSplit.Controls.Add(this.buttonFoldersEqually, 0, 2);
            this.tableLayoutPanelSplit.Controls.Add(this.buttonFoldersGenre, 0, 5);
            this.tableLayoutPanelSplit.Name = "tableLayoutPanelSplit";
            // 
            // buttonFoldersApp
            // 
            this.tableLayoutPanelSplit.SetColumnSpan(this.buttonFoldersApp, 2);
            resources.ApplyResources(this.buttonFoldersApp, "buttonFoldersApp");
            this.buttonFoldersApp.Name = "buttonFoldersApp";
            this.buttonFoldersApp.UseVisualStyleBackColor = true;
            this.buttonFoldersApp.Click += new System.EventHandler(this.buttonFoldersApp_Click);
            // 
            // buttonNoFoldersOriginal
            // 
            resources.ApplyResources(this.buttonNoFoldersOriginal, "buttonNoFoldersOriginal");
            this.buttonNoFoldersOriginal.Name = "buttonNoFoldersOriginal";
            this.tableLayoutPanelSplit.SetRowSpan(this.buttonNoFoldersOriginal, 2);
            this.buttonNoFoldersOriginal.UseVisualStyleBackColor = true;
            this.buttonNoFoldersOriginal.Click += new System.EventHandler(this.buttonNoFoldersOriginal_Click);
            // 
            // buttonNoFolders
            // 
            resources.ApplyResources(this.buttonNoFolders, "buttonNoFolders");
            this.buttonNoFolders.Name = "buttonNoFolders";
            this.tableLayoutPanelSplit.SetRowSpan(this.buttonNoFolders, 2);
            this.buttonNoFolders.UseVisualStyleBackColor = true;
            this.buttonNoFolders.Click += new System.EventHandler(this.buttonNoFolders_Click);
            // 
            // buttonFoldersLetters
            // 
            this.tableLayoutPanelSplit.SetColumnSpan(this.buttonFoldersLetters, 2);
            resources.ApplyResources(this.buttonFoldersLetters, "buttonFoldersLetters");
            this.buttonFoldersLetters.Name = "buttonFoldersLetters";
            this.buttonFoldersLetters.UseVisualStyleBackColor = true;
            this.buttonFoldersLetters.Click += new System.EventHandler(this.buttonFoldersLetters_Click);
            // 
            // buttonFoldersEqually
            // 
            this.tableLayoutPanelSplit.SetColumnSpan(this.buttonFoldersEqually, 2);
            resources.ApplyResources(this.buttonFoldersEqually, "buttonFoldersEqually");
            this.buttonFoldersEqually.Name = "buttonFoldersEqually";
            this.buttonFoldersEqually.UseVisualStyleBackColor = true;
            this.buttonFoldersEqually.Click += new System.EventHandler(this.buttonFoldersEqually_Click);
            // 
            // buttonFoldersGenre
            // 
            this.tableLayoutPanelSplit.SetColumnSpan(this.buttonFoldersGenre, 2);
            resources.ApplyResources(this.buttonFoldersGenre, "buttonFoldersGenre");
            this.buttonFoldersGenre.Name = "buttonFoldersGenre";
            this.buttonFoldersGenre.UseVisualStyleBackColor = true;
            this.buttonFoldersGenre.Click += new System.EventHandler(this.buttonFoldersGenre_Click);
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
            resources.ApplyResources(this.pictureBoxArt, "pictureBoxArt");
            this.pictureBoxArt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.TabStop = false;
            this.pictureBoxArt.Click += new System.EventHandler(this.pictureBoxArt_Click);
            // 
            // comboBoxBackPosition
            // 
            resources.ApplyResources(this.comboBoxBackPosition, "comboBoxBackPosition");
            this.comboBoxBackPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBackPosition.FormattingEnabled = true;
            this.comboBoxBackPosition.Items.AddRange(new object[] {
            resources.GetString("comboBoxBackPosition.Items"),
            resources.GetString("comboBoxBackPosition.Items1")});
            this.comboBoxBackPosition.Name = "comboBoxBackPosition";
            // 
            // labelPosition2
            // 
            resources.ApplyResources(this.labelPosition2, "labelPosition2");
            this.labelPosition2.Name = "labelPosition2";
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
            this.panel2.Controls.Add(this.checkBoxAddHome);
            this.panel2.Controls.Add(this.comboBoxBackPosition);
            this.panel2.Controls.Add(this.labelPosition2);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.buttonOk);
            this.panel2.Controls.Add(this.labelElementCount);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // checkBoxAddHome
            // 
            resources.ApplyResources(this.checkBoxAddHome, "checkBoxAddHome");
            this.checkBoxAddHome.Name = "checkBoxAddHome";
            this.checkBoxAddHome.UseVisualStyleBackColor = true;
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newToolStripMenuItem
            // 
            resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            resources.ApplyResources(this.toolStripSeparator, "toolStripSeparator");
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FoldersManagerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel2);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FoldersManagerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TreeContructorForm_FormClosing);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxSplitModes.ResumeLayout(false);
            this.tableLayoutPanelSplit.ResumeLayout(false);
            this.groupBoxArt.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button buttonFoldersEqually;
        private System.Windows.Forms.ListView listViewContent;
        private System.Windows.Forms.Label labelElementCount;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Button buttonNewFolder;
        private System.Windows.Forms.ComboBox comboBoxPosition;
        private System.Windows.Forms.Label labelPosition1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItemSep;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.Button buttonFoldersApp;
        private System.Windows.Forms.ComboBox comboBoxBackPosition;
        private System.Windows.Forms.Label labelPosition2;
        private System.Windows.Forms.CheckBox checkBoxAddHome;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSplit;
        private System.Windows.Forms.Button buttonFoldersGenre;
    }
}