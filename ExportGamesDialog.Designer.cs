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
            resources.ApplyResources(this.comboDriveLetters, "comboDriveLetters");
            this.comboDriveLetters.DisplayMember = "title";
            this.comboDriveLetters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDriveLetters.FormattingEnabled = true;
            this.comboDriveLetters.Name = "comboDriveLetters";
            this.comboDriveLetters.SelectedIndexChanged += new System.EventHandler(this.comboDriveLetters_SelectedIndexChanged);
            // 
            // lblSelectDrive
            // 
            resources.ApplyResources(this.lblSelectDrive, "lblSelectDrive");
            this.lblSelectDrive.Name = "lblSelectDrive";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioEUR
            // 
            resources.ApplyResources(this.radioEUR, "radioEUR");
            this.radioEUR.Name = "radioEUR";
            this.radioEUR.TabStop = true;
            this.radioEUR.UseVisualStyleBackColor = true;
            this.radioEUR.CheckedChanged += new System.EventHandler(this.Region_CheckedChanged);
            // 
            // radioUSA
            // 
            resources.ApplyResources(this.radioUSA, "radioUSA");
            this.radioUSA.Name = "radioUSA";
            this.radioUSA.TabStop = true;
            this.radioUSA.UseVisualStyleBackColor = true;
            this.radioUSA.CheckedChanged += new System.EventHandler(this.Region_CheckedChanged);
            // 
            // checkLinked
            // 
            resources.ApplyResources(this.checkLinked, "checkLinked");
            this.checkLinked.Checked = true;
            this.checkLinked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLinked.Name = "checkLinked";
            this.checkLinked.UseVisualStyleBackColor = true;
            this.checkLinked.CheckedChanged += new System.EventHandler(this.checkLinked_CheckedChanged);
            // 
            // ExportGamesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkLinked);
            this.Controls.Add(this.radioUSA);
            this.Controls.Add(this.radioEUR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboDriveLetters);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblSelectDrive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportGamesDialog";
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