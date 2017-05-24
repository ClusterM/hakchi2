namespace com.clusterrr.hakchi_gui.UI.Forms
{
    partial class RomsList
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
            this.romBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.detectedNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Extension = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.romBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.detectedNameDataGridViewTextBoxColumn,
            this.Extension,
            this.SizeColumn,
            this.localPathDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.romBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(518, 388);
            this.dataGridView1.TabIndex = 0;
            // 
            // romBindingSource
            // 
            this.romBindingSource.DataSource = typeof(com.clusterrr.hakchi_gui.Manager.RomManager.Rom);
            // 
            // detectedNameDataGridViewTextBoxColumn
            // 
            this.detectedNameDataGridViewTextBoxColumn.DataPropertyName = "DetectedName";
            this.detectedNameDataGridViewTextBoxColumn.HeaderText = "DetectedName";
            this.detectedNameDataGridViewTextBoxColumn.Name = "detectedNameDataGridViewTextBoxColumn";
            this.detectedNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Extension
            // 
            this.Extension.DataPropertyName = "Extension";
            this.Extension.HeaderText = "Extension";
            this.Extension.Name = "Extension";
            this.Extension.ReadOnly = true;
            // 
            // Size
            // 
            this.SizeColumn.DataPropertyName = "Size";
            this.SizeColumn.HeaderText = "Size";
            this.SizeColumn.Name = "Size";
            this.SizeColumn.ReadOnly = true;
            // 
            // localPathDataGridViewTextBoxColumn
            // 
            this.localPathDataGridViewTextBoxColumn.DataPropertyName = "LocalPath";
            this.localPathDataGridViewTextBoxColumn.HeaderText = "LocalPath";
            this.localPathDataGridViewTextBoxColumn.Name = "localPathDataGridViewTextBoxColumn";
            this.localPathDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // RomsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 388);
            this.Controls.Add(this.dataGridView1);
            this.Name = "RomsList";
            this.Text = "RomsList";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.romBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource romBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn detectedNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Extension;
        private System.Windows.Forms.DataGridViewTextBoxColumn SizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn localPathDataGridViewTextBoxColumn;
    }
}