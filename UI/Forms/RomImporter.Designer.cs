namespace com.clusterrr.hakchi_gui.UI.Forms
{
    partial class RomImporter
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.romTypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.extensionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.importDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.romCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnImport = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.romTypeBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.extensionDataGridViewTextBoxColumn,
            this.importDataGridViewCheckBoxColumn,
            this.romCountDataGridViewTextBoxColumn,
            this.sizeDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.romTypeBindingSource;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(2, 1);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(444, 301);
            this.dataGridView1.TabIndex = 0;
            // 
            // romTypeBindingSource
            // 
            this.romTypeBindingSource.DataSource = typeof(com.clusterrr.hakchi_gui.UI.Forms.RomImporter.RomType);
            this.romTypeBindingSource.CurrentChanged += new System.EventHandler(this.romTypeBindingSource_CurrentChanged);
            // 
            // extensionDataGridViewTextBoxColumn
            // 
            this.extensionDataGridViewTextBoxColumn.DataPropertyName = "extension";
            this.extensionDataGridViewTextBoxColumn.HeaderText = "extension";
            this.extensionDataGridViewTextBoxColumn.Name = "extensionDataGridViewTextBoxColumn";
            this.extensionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // importDataGridViewCheckBoxColumn
            // 
            this.importDataGridViewCheckBoxColumn.DataPropertyName = "import";
            this.importDataGridViewCheckBoxColumn.HeaderText = "import";
            this.importDataGridViewCheckBoxColumn.Name = "importDataGridViewCheckBoxColumn";
            // 
            // romCountDataGridViewTextBoxColumn
            // 
            this.romCountDataGridViewTextBoxColumn.DataPropertyName = "RomCount";
            this.romCountDataGridViewTextBoxColumn.HeaderText = "RomCount";
            this.romCountDataGridViewTextBoxColumn.Name = "romCountDataGridViewTextBoxColumn";
            this.romCountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // sizeDataGridViewTextBoxColumn
            // 
            this.sizeDataGridViewTextBoxColumn.DataPropertyName = "Size";
            this.sizeDataGridViewTextBoxColumn.HeaderText = "Size";
            this.sizeDataGridViewTextBoxColumn.Name = "sizeDataGridViewTextBoxColumn";
            this.sizeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(361, 308);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import!";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 308);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RomImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 339);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.dataGridView1);
            this.Name = "RomImporter";
            this.Text = "RomImporter";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.romTypeBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource romTypeBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn extensionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn importDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn romCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sizeDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button button1;
    }
}