namespace com.clusterrr.hakchi_gui
{
    partial class SelectCoreDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectCoreDialog));
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewGames = new System.Windows.Forms.ListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.extColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.systemColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.coreColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listBoxSystem = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxCore = new System.Windows.Forms.ListBox();
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonApply = new System.Windows.Forms.Button();
            this.resetCheckBox = new System.Windows.Forms.CheckBox();
            this.showAllSystemsCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // listViewGames
            // 
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.extColumnHeader,
            this.systemColumnHeader,
            this.coreColumnHeader});
            this.listViewGames.FullRowSelect = true;
            this.listViewGames.HideSelection = false;
            resources.ApplyResources(this.listViewGames, "listViewGames");
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.ShowGroups = false;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewGames_ColumnClick);
            this.listViewGames.SelectedIndexChanged += new System.EventHandler(this.listViewGames_SelectedIndexChanged);
            // 
            // nameColumnHeader
            // 
            resources.ApplyResources(this.nameColumnHeader, "nameColumnHeader");
            // 
            // extColumnHeader
            // 
            resources.ApplyResources(this.extColumnHeader, "extColumnHeader");
            // 
            // systemColumnHeader
            // 
            resources.ApplyResources(this.systemColumnHeader, "systemColumnHeader");
            // 
            // coreColumnHeader
            // 
            resources.ApplyResources(this.coreColumnHeader, "coreColumnHeader");
            // 
            // listBoxSystem
            // 
            this.listBoxSystem.FormattingEnabled = true;
            resources.ApplyResources(this.listBoxSystem, "listBoxSystem");
            this.listBoxSystem.Name = "listBoxSystem";
            this.listBoxSystem.SelectedIndexChanged += new System.EventHandler(this.listBoxSystem_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // listBoxCore
            // 
            this.listBoxCore.FormattingEnabled = true;
            resources.ApplyResources(this.listBoxCore, "listBoxCore");
            this.listBoxCore.Name = "listBoxCore";
            this.listBoxCore.SelectedIndexChanged += new System.EventHandler(this.listBoxCore_SelectedIndexChanged);
            // 
            // commandTextBox
            // 
            resources.ApplyResources(this.commandTextBox, "commandTextBox");
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.Enter += new System.EventHandler(this.commandTextBox_Enter);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // buttonApply
            // 
            resources.ApplyResources(this.buttonApply, "buttonApply");
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // resetCheckBox
            // 
            resources.ApplyResources(this.resetCheckBox, "resetCheckBox");
            this.resetCheckBox.Checked = true;
            this.resetCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetCheckBox.Name = "resetCheckBox";
            this.resetCheckBox.UseVisualStyleBackColor = true;
            this.resetCheckBox.CheckedChanged += new System.EventHandler(this.resetCheckBox_CheckedChanged);
            // 
            // showAllSystemsCheckBox
            // 
            resources.ApplyResources(this.showAllSystemsCheckBox, "showAllSystemsCheckBox");
            this.showAllSystemsCheckBox.Name = "showAllSystemsCheckBox";
            this.showAllSystemsCheckBox.UseVisualStyleBackColor = true;
            this.showAllSystemsCheckBox.CheckedChanged += new System.EventHandler(this.showAllSystemsCheckBox_CheckedChanged);
            // 
            // SelectCoreDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.showAllSystemsCheckBox);
            this.Controls.Add(this.resetCheckBox);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.commandTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxCore);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxSystem);
            this.Controls.Add(this.listViewGames);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.Name = "SelectCoreDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectCoreDialog_FormClosing);
            this.Shown += new System.EventHandler(this.SelectCoreDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.ListBox listBoxSystem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBoxCore;
        private System.Windows.Forms.TextBox commandTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader extColumnHeader;
        private System.Windows.Forms.ColumnHeader systemColumnHeader;
        private System.Windows.Forms.ColumnHeader coreColumnHeader;
        private System.Windows.Forms.CheckBox resetCheckBox;
        private System.Windows.Forms.CheckBox showAllSystemsCheckBox;
    }
}