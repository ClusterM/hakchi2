namespace com.clusterrr.hakchi_gui
{
    partial class GameImporterForm
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.listViewGames = new System.Windows.Forms.ListView();
            this.columnHeaderTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.buttonImport = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.listViewGames, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelButtons, 0, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // listViewGames
            // 
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderLocation,
            this.columnHeaderSize});
            this.listViewGames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewGames.HideSelection = false;
            this.listViewGames.Location = new System.Drawing.Point(3, 3);
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.Size = new System.Drawing.Size(794, 416);
            this.listViewGames.TabIndex = 0;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewGames_ItemSelectionChanged);
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Name";
            this.columnHeaderTitle.Width = 253;
            // 
            // columnHeaderLocation
            // 
            this.columnHeaderLocation.Text = "Location";
            this.columnHeaderLocation.Width = 458;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            // 
            // tableLayoutPanelButtons
            // 
            this.tableLayoutPanelButtons.AutoSize = true;
            this.tableLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelButtons.ColumnCount = 2;
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelButtons.Controls.Add(this.buttonImport, 1, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.labelStatus, 0, 0);
            this.tableLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelButtons.Location = new System.Drawing.Point(0, 422);
            this.tableLayoutPanelButtons.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            this.tableLayoutPanelButtons.RowCount = 1;
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelButtons.Size = new System.Drawing.Size(800, 28);
            this.tableLayoutPanelButtons.TabIndex = 1;
            this.tableLayoutPanelButtons.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanelButtons_Paint);
            // 
            // buttonImport
            // 
            this.buttonImport.AutoSize = true;
            this.buttonImport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonImport.Enabled = false;
            this.buttonImport.Location = new System.Drawing.Point(675, 3);
            this.buttonImport.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.buttonImport.MinimumSize = new System.Drawing.Size(0, 23);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(123, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "Import selected games";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelStatus.Location = new System.Drawing.Point(3, 3);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(3);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(667, 22);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GameImporterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "GameImporterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Games from Mini";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderLocation;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.Label labelStatus;
    }
}