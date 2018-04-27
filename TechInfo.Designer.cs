namespace com.clusterrr.hakchi_gui
{
    partial class TechInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TechInfo));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.logContentsRichTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnValue});
            this.listView1.FullRowSelect = true;
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups1"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups2"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups3"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups4"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups5"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups6")))});
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            resources.ApplyResources(this.columnName, "columnName");
            // 
            // columnValue
            // 
            resources.ApplyResources(this.columnValue, "columnValue");
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonSave
            // 
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "txt";
            this.saveFileDialog1.FileName = "Technical Information.txt";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // logContentsRichTextBox
            // 
            resources.ApplyResources(this.logContentsRichTextBox, "logContentsRichTextBox");
            this.logContentsRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.logContentsRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logContentsRichTextBox.Name = "logContentsRichTextBox";
            this.logContentsRichTextBox.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TechInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logContentsRichTextBox);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.listView1);
            this.Name = "TechInfo";
            this.Load += new System.EventHandler(this.TechInfo_Load);
            this.Shown += new System.EventHandler(this.TechInfo_Shown);
            this.Resize += new System.EventHandler(this.TechInfo_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnValue;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RichTextBox logContentsRichTextBox;
        private System.Windows.Forms.Label label1;
    }
}