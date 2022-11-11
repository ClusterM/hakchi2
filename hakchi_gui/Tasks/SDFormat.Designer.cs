namespace com.clusterrr.hakchi_gui.Tasks
{
    partial class SDFormat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDFormat));
            this.chkStoreSaves = new System.Windows.Forms.CheckBox();
            this.chkCopySaves = new System.Windows.Forms.CheckBox();
            this.chkBootable = new System.Windows.Forms.CheckBox();
            this.lblBootable = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCopyNand = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkStoreSaves
            // 
            resources.ApplyResources(this.chkStoreSaves, "chkStoreSaves");
            this.chkStoreSaves.Checked = true;
            this.chkStoreSaves.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStoreSaves.Name = "chkStoreSaves";
            this.chkStoreSaves.UseVisualStyleBackColor = true;
            this.chkStoreSaves.CheckedChanged += new System.EventHandler(this.ChkStoreSaves_CheckedChanged);
            // 
            // chkCopySaves
            // 
            resources.ApplyResources(this.chkCopySaves, "chkCopySaves");
            this.chkCopySaves.Name = "chkCopySaves";
            this.chkCopySaves.UseVisualStyleBackColor = true;
            this.chkCopySaves.CheckedChanged += new System.EventHandler(this.ChkCopySaves_CheckedChanged);
            // 
            // chkBootable
            // 
            resources.ApplyResources(this.chkBootable, "chkBootable");
            this.chkBootable.Name = "chkBootable";
            this.chkBootable.UseVisualStyleBackColor = true;
            this.chkBootable.CheckedChanged += new System.EventHandler(this.ChkBootable_CheckedChanged);
            // 
            // lblBootable
            // 
            resources.ApplyResources(this.lblBootable, "lblBootable");
            this.lblBootable.Name = "lblBootable";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.chkBootable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblBootable, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkStoreSaves, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.chkCopySaves, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkCopyNand, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.btnOk, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // chkCopyNand
            // 
            resources.ApplyResources(this.chkCopyNand, "chkCopyNand");
            this.chkCopyNand.Name = "chkCopyNand";
            this.chkCopyNand.UseVisualStyleBackColor = true;
            this.chkCopyNand.CheckedChanged += new System.EventHandler(this.ChkCopyNand_CheckedChanged);
            // 
            // SDFormat
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SDFormat";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkStoreSaves;
        private System.Windows.Forms.CheckBox chkCopySaves;
        private System.Windows.Forms.CheckBox chkBootable;
        private System.Windows.Forms.Label lblBootable;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCopyNand;
    }
}