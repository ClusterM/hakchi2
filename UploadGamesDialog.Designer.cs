namespace com.clusterrr.hakchi_gui
{
    partial class UploadGamesDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkLinked = new System.Windows.Forms.CheckBox();
            this.radioUSA = new System.Windows.Forms.RadioButton();
            this.radioEUR = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSelectedGamesCollection = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(3, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(139, 23);
            this.btnOk.TabIndex = 5;
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
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // checkLinked
            // 
            this.checkLinked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkLinked.AutoSize = true;
            this.checkLinked.Checked = true;
            this.checkLinked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLinked.Location = new System.Drawing.Point(211, 76);
            this.checkLinked.Name = "checkLinked";
            this.checkLinked.Size = new System.Drawing.Size(83, 17);
            this.checkLinked.TabIndex = 4;
            this.checkLinked.Text = "Linked sync";
            this.checkLinked.UseVisualStyleBackColor = true;
            // 
            // radioUSA
            // 
            this.radioUSA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioUSA.AutoSize = true;
            this.radioUSA.Enabled = false;
            this.radioUSA.Location = new System.Drawing.Point(247, 53);
            this.radioUSA.Name = "radioUSA";
            this.radioUSA.Size = new System.Drawing.Size(47, 17);
            this.radioUSA.TabIndex = 3;
            this.radioUSA.TabStop = true;
            this.radioUSA.Text = "USA";
            this.radioUSA.UseVisualStyleBackColor = true;
            // 
            // radioEUR
            // 
            this.radioEUR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioEUR.AutoSize = true;
            this.radioEUR.Enabled = false;
            this.radioEUR.Location = new System.Drawing.Point(193, 53);
            this.radioEUR.Name = "radioEUR";
            this.radioEUR.Size = new System.Drawing.Size(48, 17);
            this.radioEUR.TabIndex = 2;
            this.radioEUR.TabStop = true;
            this.radioEUR.Text = "EUR";
            this.radioEUR.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select region:";
            // 
            // labelSelectedGamesCollection
            // 
            this.labelSelectedGamesCollection.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelSelectedGamesCollection.Location = new System.Drawing.Point(8, 9);
            this.labelSelectedGamesCollection.Name = "labelSelectedGamesCollection";
            this.labelSelectedGamesCollection.Size = new System.Drawing.Size(290, 35);
            this.labelSelectedGamesCollection.TabIndex = 0;
            this.labelSelectedGamesCollection.Text = "Selected games collection: ";
            this.labelSelectedGamesCollection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.labelSelectedGamesCollection);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(306, 45);
            this.panel1.TabIndex = 11;
            // 
            // UploadGamesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 135);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.checkLinked);
            this.Controls.Add(this.radioUSA);
            this.Controls.Add(this.radioEUR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "UploadGamesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upload Games";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkLinked;
        private System.Windows.Forms.RadioButton radioUSA;
        private System.Windows.Forms.RadioButton radioEUR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSelectedGamesCollection;
        private System.Windows.Forms.Panel panel1;
    }
}