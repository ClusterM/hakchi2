namespace com.clusterrr.hakchi_gui
{
    partial class ExportGamesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportGamesDialog));
            this.comboDriveLetters = new System.Windows.Forms.ComboBox();
            this.lblSelectDrive = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radioEUR = new System.Windows.Forms.RadioButton();
            this.radioUSA = new System.Windows.Forms.RadioButton();
            this.checkLinked = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboDriveLetters
            // 
            this.comboDriveLetters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDriveLetters.DisplayMember = "title";
            this.comboDriveLetters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDriveLetters.FormattingEnabled = true;
            this.comboDriveLetters.Location = new System.Drawing.Point(12, 25);
            this.comboDriveLetters.Name = "comboDriveLetters";
            this.comboDriveLetters.Size = new System.Drawing.Size(282, 21);
            this.comboDriveLetters.TabIndex = 0;
            this.comboDriveLetters.SelectedIndexChanged += new System.EventHandler(this.comboDriveLetters_SelectedIndexChanged);
            // 
            // lblSelectDrive
            // 
            this.lblSelectDrive.AutoSize = true;
            this.lblSelectDrive.Location = new System.Drawing.Point(9, 9);
            this.lblSelectDrive.Name = "lblSelectDrive";
            this.lblSelectDrive.Size = new System.Drawing.Size(131, 13);
            this.lblSelectDrive.TabIndex = 1;
            this.lblSelectDrive.Text = "Select a drive to export to:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 99);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(290, 29);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(3, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(139, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(148, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(139, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select region:";
            // 
            // radioEUR
            // 
            this.radioEUR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioEUR.AutoSize = true;
            this.radioEUR.Enabled = false;
            this.radioEUR.Location = new System.Drawing.Point(193, 53);
            this.radioEUR.Name = "radioEUR";
            this.radioEUR.Size = new System.Drawing.Size(48, 17);
            this.radioEUR.TabIndex = 4;
            this.radioEUR.TabStop = true;
            this.radioEUR.Text = "EUR";
            this.radioEUR.UseVisualStyleBackColor = true;
            this.radioEUR.CheckedChanged += new System.EventHandler(this.Region_CheckedChanged);
            // 
            // radioUSA
            // 
            this.radioUSA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioUSA.AutoSize = true;
            this.radioUSA.Enabled = false;
            this.radioUSA.Location = new System.Drawing.Point(247, 53);
            this.radioUSA.Name = "radioUSA";
            this.radioUSA.Size = new System.Drawing.Size(47, 17);
            this.radioUSA.TabIndex = 5;
            this.radioUSA.TabStop = true;
            this.radioUSA.Text = "USA";
            this.radioUSA.UseVisualStyleBackColor = true;
            this.radioUSA.CheckedChanged += new System.EventHandler(this.Region_CheckedChanged);
            // 
            // checkLinked
            // 
            this.checkLinked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkLinked.AutoSize = true;
            this.checkLinked.Checked = true;
            this.checkLinked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLinked.Enabled = false;
            this.checkLinked.Location = new System.Drawing.Point(204, 76);
            this.checkLinked.Name = "checkLinked";
            this.checkLinked.Size = new System.Drawing.Size(90, 17);
            this.checkLinked.TabIndex = 6;
            this.checkLinked.Text = "Linked export";
            this.checkLinked.UseVisualStyleBackColor = true;
            // 
            // ExportGamesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 135);
            this.Controls.Add(this.checkLinked);
            this.Controls.Add(this.radioUSA);
            this.Controls.Add(this.radioEUR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboDriveLetters);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblSelectDrive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ExportGamesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Games";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboDriveLetters;
        private System.Windows.Forms.Label lblSelectDrive;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioEUR;
        private System.Windows.Forms.RadioButton radioUSA;
        private System.Windows.Forms.CheckBox checkLinked;
    }
}