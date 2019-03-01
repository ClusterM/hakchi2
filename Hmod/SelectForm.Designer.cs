namespace com.clusterrr.hakchi_gui.Hmod
{
    partial class SelectForm
    {

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectForm));
            this.buttonOk = new System.Windows.Forms.Button();
            this.listViewHmods = new System.Windows.Forms.ListView();
            this.hmodName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modListMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emulatedSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteModFromDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showModInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.readmeControl1 = new com.clusterrr.hakchi_gui.Hmod.Controls.ReadmeControl();
            this.modListMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // listViewHmods
            // 
            resources.ApplyResources(this.listViewHmods, "listViewHmods");
            this.listViewHmods.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewHmods.CheckBoxes = true;
            this.listViewHmods.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hmodName});
            this.listViewHmods.ContextMenuStrip = this.modListMenuStrip;
            this.listViewHmods.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewHmods.Name = "listViewHmods";
            this.listViewHmods.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewHmods.UseCompatibleStateImageBehavior = false;
            this.listViewHmods.View = System.Windows.Forms.View.Details;
            this.listViewHmods.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewHmods_ItemSelectionChanged);
            // 
            // hmodName
            // 
            resources.ApplyResources(this.hmodName, "hmodName");
            // 
            // modListMenuStrip
            // 
            this.modListMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupByToolStripMenuItem,
            this.deleteModFromDiskToolStripMenuItem,
            this.showModInExplorerToolStripMenuItem});
            this.modListMenuStrip.Name = "contextMenuStrip1";
            resources.ApplyResources(this.modListMenuStrip, "modListMenuStrip");
            this.modListMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.modListMenuStrip_Opening);
            // 
            // groupByToolStripMenuItem
            // 
            this.groupByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.categoryToolStripMenuItem,
            this.creatorToolStripMenuItem,
            this.emulatedSystemToolStripMenuItem});
            this.groupByToolStripMenuItem.Name = "groupByToolStripMenuItem";
            resources.ApplyResources(this.groupByToolStripMenuItem, "groupByToolStripMenuItem");
            // 
            // categoryToolStripMenuItem
            // 
            this.categoryToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.categoryToolStripMenuItem.Name = "categoryToolStripMenuItem";
            resources.ApplyResources(this.categoryToolStripMenuItem, "categoryToolStripMenuItem");
            this.categoryToolStripMenuItem.Click += new System.EventHandler(this.categoryToolStripMenuItem_Click);
            // 
            // creatorToolStripMenuItem
            // 
            this.creatorToolStripMenuItem.Name = "creatorToolStripMenuItem";
            resources.ApplyResources(this.creatorToolStripMenuItem, "creatorToolStripMenuItem");
            this.creatorToolStripMenuItem.Click += new System.EventHandler(this.creatorToolStripMenuItem_Click);
            // 
            // emulatedSystemToolStripMenuItem
            // 
            this.emulatedSystemToolStripMenuItem.Name = "emulatedSystemToolStripMenuItem";
            resources.ApplyResources(this.emulatedSystemToolStripMenuItem, "emulatedSystemToolStripMenuItem");
            this.emulatedSystemToolStripMenuItem.Click += new System.EventHandler(this.emulatedSystemToolStripMenuItem_Click);
            // 
            // deleteModFromDiskToolStripMenuItem
            // 
            this.deleteModFromDiskToolStripMenuItem.Name = "deleteModFromDiskToolStripMenuItem";
            resources.ApplyResources(this.deleteModFromDiskToolStripMenuItem, "deleteModFromDiskToolStripMenuItem");
            this.deleteModFromDiskToolStripMenuItem.Click += new System.EventHandler(this.deleteModFromDiskToolStripMenuItem_Click);
            // 
            // showModInExplorerToolStripMenuItem
            // 
            this.showModInExplorerToolStripMenuItem.Name = "showModInExplorerToolStripMenuItem";
            resources.ApplyResources(this.showModInExplorerToolStripMenuItem, "showModInExplorerToolStripMenuItem");
            this.showModInExplorerToolStripMenuItem.Click += new System.EventHandler(this.showModInExplorerToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewHmods);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.readmeControl1);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // readmeControl1
            // 
            resources.ApplyResources(this.readmeControl1, "readmeControl1");
            this.readmeControl1.BackColor = System.Drawing.SystemColors.Control;
            this.readmeControl1.Name = "readmeControl1";
            // 
            // SelectForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonOk);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "SelectForm";
            this.Shown += new System.EventHandler(this.SelectModsForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SelectModsForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SelectModsForm_DragEnter);
            this.modListMenuStrip.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.Button buttonOk;
        internal System.Windows.Forms.ListView listViewHmods;
        private System.Windows.Forms.ColumnHeader hmodName;
        private System.Windows.Forms.ContextMenuStrip modListMenuStrip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem groupByToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem categoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creatorToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem showModInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteModFromDiskToolStripMenuItem;
        private Controls.ReadmeControl readmeControl1;
        private System.Windows.Forms.ToolStripMenuItem emulatedSystemToolStripMenuItem;
    }
}