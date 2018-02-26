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
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonDiscard = new System.Windows.Forms.Button();
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
            this.SuspendLayout();
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(579, 334);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(72, 23);
            this.buttonAccept.TabIndex = 7;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonDiscard
            // 
            this.buttonDiscard.Location = new System.Drawing.Point(658, 334);
            this.buttonDiscard.Name = "buttonDiscard";
            this.buttonDiscard.Size = new System.Drawing.Size(65, 23);
            this.buttonDiscard.TabIndex = 8;
            this.buttonDiscard.Text = "Discard";
            this.buttonDiscard.UseVisualStyleBackColor = true;
            this.buttonDiscard.Click += new System.EventHandler(this.buttonDiscard_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select appropriate system and core for the following games:";
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
            this.listViewGames.Location = new System.Drawing.Point(12, 30);
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.ShowGroups = false;
            this.listViewGames.Size = new System.Drawing.Size(430, 259);
            this.listViewGames.TabIndex = 1;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewGames_ColumnClick);
            this.listViewGames.SelectedIndexChanged += new System.EventHandler(this.listViewGames_SelectedIndexChanged);
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Name";
            this.nameColumnHeader.Width = 180;
            // 
            // extColumnHeader
            // 
            this.extColumnHeader.Text = "Ext";
            this.extColumnHeader.Width = 45;
            // 
            // systemColumnHeader
            // 
            this.systemColumnHeader.Text = "System";
            this.systemColumnHeader.Width = 140;
            // 
            // coreColumnHeader
            // 
            this.coreColumnHeader.Text = "Core";
            // 
            // listBoxSystem
            // 
            this.listBoxSystem.FormattingEnabled = true;
            this.listBoxSystem.Location = new System.Drawing.Point(448, 30);
            this.listBoxSystem.Name = "listBoxSystem";
            this.listBoxSystem.Size = new System.Drawing.Size(274, 134);
            this.listBoxSystem.TabIndex = 2;
            this.listBoxSystem.SelectedIndexChanged += new System.EventHandler(this.listBoxSystem_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(445, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "System:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(445, 175);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Core:";
            // 
            // listBoxCore
            // 
            this.listBoxCore.FormattingEnabled = true;
            this.listBoxCore.Location = new System.Drawing.Point(448, 194);
            this.listBoxCore.Name = "listBoxCore";
            this.listBoxCore.Size = new System.Drawing.Size(275, 95);
            this.listBoxCore.TabIndex = 3;
            this.listBoxCore.SelectedIndexChanged += new System.EventHandler(this.listBoxCore_SelectedIndexChanged);
            // 
            // commandTextBox
            // 
            this.commandTextBox.Location = new System.Drawing.Point(12, 308);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.Size = new System.Drawing.Size(711, 20);
            this.commandTextBox.TabIndex = 4;
            this.commandTextBox.Enter += new System.EventHandler(this.commandTextBox_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 292);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(201, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Command line (for advanced users only!):";
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(12, 334);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 5;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // resetCheckBox
            // 
            this.resetCheckBox.AutoSize = true;
            this.resetCheckBox.Checked = true;
            this.resetCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetCheckBox.Location = new System.Drawing.Point(93, 338);
            this.resetCheckBox.Name = "resetCheckBox";
            this.resetCheckBox.Size = new System.Drawing.Size(174, 17);
            this.resetCheckBox.TabIndex = 6;
            this.resetCheckBox.Text = "Also reset to default parameters";
            this.resetCheckBox.UseVisualStyleBackColor = true;
            this.resetCheckBox.CheckedChanged += new System.EventHandler(this.resetCheckBox_CheckedChanged);
            // 
            // SelectCoreDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 366);
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
            this.Controls.Add(this.buttonDiscard);
            this.Controls.Add(this.buttonAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(750, 405);
            this.MinimumSize = new System.Drawing.Size(750, 405);
            this.Name = "SelectCoreDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Cores";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectCoreDialog_FormClosing);
            this.Shown += new System.EventHandler(this.SelectCoreDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonDiscard;
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
    }
}