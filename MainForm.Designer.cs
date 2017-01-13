namespace com.clusterrr.hakchi_gui
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMoreGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flashOriginalKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flashCustomKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useExtendedFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gitHubPageWithActualReleasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fAQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedListBoxGames = new System.Windows.Forms.CheckedListBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButtonTwoSim = new System.Windows.Forms.RadioButton();
            this.buttonGoogle = new System.Windows.Forms.Button();
            this.buttonBrowseImage = new System.Windows.Forms.Button();
            this.pictureBoxArt = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxArguments = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPublisher = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.maskedTextBoxReleaseDate = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonTwo = new System.Windows.Forms.RadioButton();
            this.radioButtonOne = new System.Windows.Forms.RadioButton();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonAddGames = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialogNes = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.buttonStart = new System.Windows.Forms.Button();
            this.groupBoxDefaultGames = new System.Windows.Forms.GroupBox();
            this.checkedListBoxDefaultGames = new System.Windows.Forms.CheckedListBox();
            this.timerCalculateGames = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.groupBoxDefaultGames.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.kernelToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMoreGamesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // addMoreGamesToolStripMenuItem
            // 
            this.addMoreGamesToolStripMenuItem.Name = "addMoreGamesToolStripMenuItem";
            resources.ApplyResources(this.addMoreGamesToolStripMenuItem, "addMoreGamesToolStripMenuItem");
            this.addMoreGamesToolStripMenuItem.Click += new System.EventHandler(this.buttonAddGames_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // kernelToolStripMenuItem
            // 
            this.kernelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpKernelToolStripMenuItem,
            this.flashOriginalKernelToolStripMenuItem,
            this.flashCustomKernelToolStripMenuItem,
            this.uninstallToolStripMenuItem});
            this.kernelToolStripMenuItem.Name = "kernelToolStripMenuItem";
            resources.ApplyResources(this.kernelToolStripMenuItem, "kernelToolStripMenuItem");
            // 
            // dumpKernelToolStripMenuItem
            // 
            this.dumpKernelToolStripMenuItem.Name = "dumpKernelToolStripMenuItem";
            resources.ApplyResources(this.dumpKernelToolStripMenuItem, "dumpKernelToolStripMenuItem");
            this.dumpKernelToolStripMenuItem.Click += new System.EventHandler(this.dumpKernelToolStripMenuItem_Click);
            // 
            // flashOriginalKernelToolStripMenuItem
            // 
            this.flashOriginalKernelToolStripMenuItem.Name = "flashOriginalKernelToolStripMenuItem";
            resources.ApplyResources(this.flashOriginalKernelToolStripMenuItem, "flashOriginalKernelToolStripMenuItem");
            this.flashOriginalKernelToolStripMenuItem.Click += new System.EventHandler(this.flashOriginalKernelToolStripMenuItem_Click);
            // 
            // flashCustomKernelToolStripMenuItem
            // 
            this.flashCustomKernelToolStripMenuItem.Name = "flashCustomKernelToolStripMenuItem";
            resources.ApplyResources(this.flashCustomKernelToolStripMenuItem, "flashCustomKernelToolStripMenuItem");
            this.flashCustomKernelToolStripMenuItem.Click += new System.EventHandler(this.flashCustomKernelToolStripMenuItem_Click);
            // 
            // uninstallToolStripMenuItem
            // 
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            resources.ApplyResources(this.uninstallToolStripMenuItem, "uninstallToolStripMenuItem");
            this.uninstallToolStripMenuItem.Click += new System.EventHandler(this.uninstallToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useExtendedFontToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            // 
            // useExtendedFontToolStripMenuItem
            // 
            this.useExtendedFontToolStripMenuItem.Checked = true;
            this.useExtendedFontToolStripMenuItem.CheckOnClick = true;
            this.useExtendedFontToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useExtendedFontToolStripMenuItem.Name = "useExtendedFontToolStripMenuItem";
            resources.ApplyResources(this.useExtendedFontToolStripMenuItem, "useExtendedFontToolStripMenuItem");
            this.useExtendedFontToolStripMenuItem.Click += new System.EventHandler(this.useExtendedFontToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gitHubPageWithActualReleasesToolStripMenuItem,
            this.fAQToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // gitHubPageWithActualReleasesToolStripMenuItem
            // 
            this.gitHubPageWithActualReleasesToolStripMenuItem.Name = "gitHubPageWithActualReleasesToolStripMenuItem";
            resources.ApplyResources(this.gitHubPageWithActualReleasesToolStripMenuItem, "gitHubPageWithActualReleasesToolStripMenuItem");
            this.gitHubPageWithActualReleasesToolStripMenuItem.Click += new System.EventHandler(this.gitHubPageWithActualReleasesToolStripMenuItem_Click);
            // 
            // fAQToolStripMenuItem
            // 
            this.fAQToolStripMenuItem.Name = "fAQToolStripMenuItem";
            resources.ApplyResources(this.fAQToolStripMenuItem, "fAQToolStripMenuItem");
            this.fAQToolStripMenuItem.Click += new System.EventHandler(this.fAQToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // checkedListBoxGames
            // 
            this.checkedListBoxGames.AllowDrop = true;
            resources.ApplyResources(this.checkedListBoxGames, "checkedListBoxGames");
            this.checkedListBoxGames.FormattingEnabled = true;
            this.checkedListBoxGames.Items.AddRange(new object[] {
            resources.GetString("checkedListBoxGames.Items")});
            this.checkedListBoxGames.Name = "checkedListBoxGames";
            this.checkedListBoxGames.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxGames_ItemCheck);
            this.checkedListBoxGames.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxGames_SelectedIndexChanged);
            this.checkedListBoxGames.DragDrop += new System.Windows.Forms.DragEventHandler(this.checkedListBoxGames_DragDrop);
            this.checkedListBoxGames.DragEnter += new System.Windows.Forms.DragEventHandler(this.checkedListBoxGames_DragEnter);
            this.checkedListBoxGames.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxGames_MouseDown);
            // 
            // groupBoxOptions
            // 
            resources.ApplyResources(this.groupBoxOptions, "groupBoxOptions");
            this.groupBoxOptions.Controls.Add(this.label6);
            this.groupBoxOptions.Controls.Add(this.radioButtonTwoSim);
            this.groupBoxOptions.Controls.Add(this.buttonGoogle);
            this.groupBoxOptions.Controls.Add(this.buttonBrowseImage);
            this.groupBoxOptions.Controls.Add(this.pictureBoxArt);
            this.groupBoxOptions.Controls.Add(this.label4);
            this.groupBoxOptions.Controls.Add(this.textBoxArguments);
            this.groupBoxOptions.Controls.Add(this.label3);
            this.groupBoxOptions.Controls.Add(this.textBoxPublisher);
            this.groupBoxOptions.Controls.Add(this.label2);
            this.groupBoxOptions.Controls.Add(this.maskedTextBoxReleaseDate);
            this.groupBoxOptions.Controls.Add(this.label1);
            this.groupBoxOptions.Controls.Add(this.radioButtonTwo);
            this.groupBoxOptions.Controls.Add(this.radioButtonOne);
            this.groupBoxOptions.Controls.Add(this.textBoxName);
            this.groupBoxOptions.Controls.Add(this.labelName);
            this.groupBoxOptions.Controls.Add(this.labelID);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // radioButtonTwoSim
            // 
            resources.ApplyResources(this.radioButtonTwoSim, "radioButtonTwoSim");
            this.radioButtonTwoSim.Name = "radioButtonTwoSim";
            this.radioButtonTwoSim.UseVisualStyleBackColor = true;
            this.radioButtonTwoSim.CheckedChanged += new System.EventHandler(this.radioButtonOne_CheckedChanged);
            // 
            // buttonGoogle
            // 
            resources.ApplyResources(this.buttonGoogle, "buttonGoogle");
            this.buttonGoogle.Name = "buttonGoogle";
            this.buttonGoogle.UseVisualStyleBackColor = true;
            this.buttonGoogle.Click += new System.EventHandler(this.buttonGoogle_Click);
            // 
            // buttonBrowseImage
            // 
            resources.ApplyResources(this.buttonBrowseImage, "buttonBrowseImage");
            this.buttonBrowseImage.Name = "buttonBrowseImage";
            this.buttonBrowseImage.UseVisualStyleBackColor = true;
            this.buttonBrowseImage.Click += new System.EventHandler(this.buttonBrowseImage_Click);
            // 
            // pictureBoxArt
            // 
            resources.ApplyResources(this.pictureBoxArt, "pictureBoxArt");
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxArguments
            // 
            resources.ApplyResources(this.textBoxArguments, "textBoxArguments");
            this.textBoxArguments.Name = "textBoxArguments";
            this.textBoxArguments.TextChanged += new System.EventHandler(this.textBoxArguments_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxPublisher
            // 
            resources.ApplyResources(this.textBoxPublisher, "textBoxPublisher");
            this.textBoxPublisher.Name = "textBoxPublisher";
            this.textBoxPublisher.TextChanged += new System.EventHandler(this.textBoxPublisher_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // maskedTextBoxReleaseDate
            // 
            resources.ApplyResources(this.maskedTextBoxReleaseDate, "maskedTextBoxReleaseDate");
            this.maskedTextBoxReleaseDate.Name = "maskedTextBoxReleaseDate";
            this.maskedTextBoxReleaseDate.TextChanged += new System.EventHandler(this.maskedTextBoxReleaseDate_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButtonTwo
            // 
            resources.ApplyResources(this.radioButtonTwo, "radioButtonTwo");
            this.radioButtonTwo.Name = "radioButtonTwo";
            this.radioButtonTwo.UseVisualStyleBackColor = true;
            this.radioButtonTwo.CheckedChanged += new System.EventHandler(this.radioButtonOne_CheckedChanged);
            // 
            // radioButtonOne
            // 
            resources.ApplyResources(this.radioButtonOne, "radioButtonOne");
            this.radioButtonOne.Checked = true;
            this.radioButtonOne.Name = "radioButtonOne";
            this.radioButtonOne.TabStop = true;
            this.radioButtonOne.UseVisualStyleBackColor = true;
            this.radioButtonOne.CheckedChanged += new System.EventHandler(this.radioButtonOne_CheckedChanged);
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // labelID
            // 
            resources.ApplyResources(this.labelID, "labelID");
            this.labelID.Name = "labelID";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // buttonAddGames
            // 
            resources.ApplyResources(this.buttonAddGames, "buttonAddGames");
            this.buttonAddGames.Name = "buttonAddGames";
            this.buttonAddGames.UseVisualStyleBackColor = true;
            this.buttonAddGames.Click += new System.EventHandler(this.buttonAddGames_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelSelected});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.SizingGrip = false;
            // 
            // toolStripStatusLabelSelected
            // 
            this.toolStripStatusLabelSelected.Name = "toolStripStatusLabelSelected";
            resources.ApplyResources(this.toolStripStatusLabelSelected, "toolStripStatusLabelSelected");
            // 
            // openFileDialogNes
            // 
            this.openFileDialogNes.DefaultExt = "nes";
            resources.ApplyResources(this.openFileDialogNes, "openFileDialogNes");
            this.openFileDialogNes.Multiselect = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.unselectAllToolStripMenuItem,
            this.deleteGameToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // unselectAllToolStripMenuItem
            // 
            this.unselectAllToolStripMenuItem.Name = "unselectAllToolStripMenuItem";
            resources.ApplyResources(this.unselectAllToolStripMenuItem, "unselectAllToolStripMenuItem");
            this.unselectAllToolStripMenuItem.Click += new System.EventHandler(this.unselectAllToolStripMenuItem_Click);
            // 
            // deleteGameToolStripMenuItem
            // 
            this.deleteGameToolStripMenuItem.Name = "deleteGameToolStripMenuItem";
            resources.ApplyResources(this.deleteGameToolStripMenuItem, "deleteGameToolStripMenuItem");
            this.deleteGameToolStripMenuItem.Click += new System.EventHandler(this.deleteGameToolStripMenuItem_Click);
            // 
            // openFileDialogImage
            // 
            resources.ApplyResources(this.openFileDialogImage, "openFileDialogImage");
            // 
            // buttonStart
            // 
            resources.ApplyResources(this.buttonStart, "buttonStart");
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // groupBoxDefaultGames
            // 
            this.groupBoxDefaultGames.Controls.Add(this.checkedListBoxDefaultGames);
            resources.ApplyResources(this.groupBoxDefaultGames, "groupBoxDefaultGames");
            this.groupBoxDefaultGames.Name = "groupBoxDefaultGames";
            this.groupBoxDefaultGames.TabStop = false;
            // 
            // checkedListBoxDefaultGames
            // 
            resources.ApplyResources(this.checkedListBoxDefaultGames, "checkedListBoxDefaultGames");
            this.checkedListBoxDefaultGames.FormattingEnabled = true;
            this.checkedListBoxDefaultGames.Name = "checkedListBoxDefaultGames";
            this.checkedListBoxDefaultGames.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxDefaultGames_MouseDown);
            // 
            // timerCalculateGames
            // 
            this.timerCalculateGames.Enabled = true;
            this.timerCalculateGames.Interval = 500;
            this.timerCalculateGames.Tick += new System.EventHandler(this.timerCalculateGames_Tick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.buttonAddGames);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkedListBoxGames);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBoxDefaultGames);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.groupBoxDefaultGames.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMoreGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBoxGames;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.RadioButton radioButtonTwo;
        private System.Windows.Forms.RadioButton radioButtonOne;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.TextBox textBoxPublisher;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxReleaseDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxArguments;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBoxArt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonGoogle;
        private System.Windows.Forms.Button buttonBrowseImage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonAddGames;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelected;
        private System.Windows.Forms.OpenFileDialog openFileDialogNes;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteGameToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ToolStripMenuItem kernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpKernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashCustomKernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashOriginalKernelToolStripMenuItem;
        private System.Windows.Forms.RadioButton radioButtonTwoSim;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useExtendedFontToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxDefaultGames;
        private System.Windows.Forms.CheckedListBox checkedListBoxDefaultGames;
        private System.Windows.Forms.Timer timerCalculateGames;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fAQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gitHubPageWithActualReleasesToolStripMenuItem;
    }
}

