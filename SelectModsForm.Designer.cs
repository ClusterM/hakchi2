namespace com.clusterrr.hakchi_gui
{
    partial class SelectModsForm
    {

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonOk = new System.Windows.Forms.Button();
            this.wbReadme = new System.Windows.Forms.WebBrowser();
            this.listViewHmods = new System.Windows.Forms.ListView();
            this.hmodName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modListMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modListMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(12, 403);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(816, 33);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // wbReadme
            // 
            this.wbReadme.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wbReadme.IsWebBrowserContextMenuEnabled = false;
            this.wbReadme.Location = new System.Drawing.Point(276, 12);
            this.wbReadme.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbReadme.Name = "wbReadme";
            this.wbReadme.ScriptErrorsSuppressed = true;
            this.wbReadme.ScrollBarsEnabled = false;
            this.wbReadme.Size = new System.Drawing.Size(552, 385);
            this.wbReadme.TabIndex = 3;
            this.wbReadme.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            this.wbReadme.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbReadme_Navigating);
            // 
            // listViewHmods
            // 
            this.listViewHmods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewHmods.CheckBoxes = true;
            this.listViewHmods.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hmodName});
            this.listViewHmods.ContextMenuStrip = this.modListMenuStrip;
            this.listViewHmods.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewHmods.Location = new System.Drawing.Point(12, 12);
            this.listViewHmods.Name = "listViewHmods";
            this.listViewHmods.Size = new System.Drawing.Size(248, 385);
            this.listViewHmods.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewHmods.TabIndex = 4;
            this.listViewHmods.UseCompatibleStateImageBehavior = false;
            this.listViewHmods.View = System.Windows.Forms.View.Details;
            this.listViewHmods.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewHmods_ItemChecked);
            this.listViewHmods.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewHmods_ItemSelectionChanged);
            this.listViewHmods.SelectedIndexChanged += new System.EventHandler(this.listViewHmods_SelectedIndexChanged);
            // 
            // hmodName
            // 
            this.hmodName.Text = "";
            this.hmodName.Width = 244;
            // 
            // modListMenuStrip
            // 
            this.modListMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupByToolStripMenuItem});
            this.modListMenuStrip.Name = "contextMenuStrip1";
            this.modListMenuStrip.Size = new System.Drawing.Size(124, 26);
            // 
            // groupByToolStripMenuItem
            // 
            this.groupByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.categoryToolStripMenuItem,
            this.creatorToolStripMenuItem});
            this.groupByToolStripMenuItem.Name = "groupByToolStripMenuItem";
            this.groupByToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.groupByToolStripMenuItem.Text = "Group by";
            // 
            // categoryToolStripMenuItem
            // 
            this.categoryToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.categoryToolStripMenuItem.Name = "categoryToolStripMenuItem";
            this.categoryToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.categoryToolStripMenuItem.Text = "Category";
            this.categoryToolStripMenuItem.Click += new System.EventHandler(this.categoryToolStripMenuItem_Click);
            // 
            // creatorToolStripMenuItem
            // 
            this.creatorToolStripMenuItem.Name = "creatorToolStripMenuItem";
            this.creatorToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.creatorToolStripMenuItem.Text = "Creator";
            this.creatorToolStripMenuItem.Click += new System.EventHandler(this.creatorToolStripMenuItem_Click);
            // 
            // SelectModsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 446);
            this.Controls.Add(this.listViewHmods);
            this.Controls.Add(this.wbReadme);
            this.Controls.Add(this.buttonOk);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "SelectModsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ModsSelect";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SelectModsForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SelectModsForm_DragEnter);
            this.modListMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.WebBrowser wbReadme;
        internal System.Windows.Forms.ListView listViewHmods;
        private System.Windows.Forms.ColumnHeader hmodName;
        private System.Windows.Forms.ContextMenuStrip modListMenuStrip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem groupByToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem categoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creatorToolStripMenuItem;
    }
}