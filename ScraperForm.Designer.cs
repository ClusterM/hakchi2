namespace com.clusterrr.hakchi_gui
{
    partial class ScraperForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScraperForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listViewGames = new System.Windows.Forms.ListView();
            this.gameName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.maxPlayersComboBox = new System.Windows.Forms.ComboBox();
            this.comboBoxGenre = new System.Windows.Forms.ComboBox();
            this.maskedTextBoxReleaseDate = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxPublisher = new System.Windows.Forms.TextBox();
            this.textBoxDeveloper = new System.Windows.Forms.TextBox();
            this.textBoxCopyright = new System.Windows.Forms.TextBox();
            this.checkBoxName = new System.Windows.Forms.CheckBox();
            this.checkBoxPublisher = new System.Windows.Forms.CheckBox();
            this.checkBoxDeveloper = new System.Windows.Forms.CheckBox();
            this.checkBoxReleaseDate = new System.Windows.Forms.CheckBox();
            this.checkBoxCopyright = new System.Windows.Forms.CheckBox();
            this.checkBoxGenre = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayerCount = new System.Windows.Forms.CheckBox();
            this.checkBoxDescription = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelArt = new System.Windows.Forms.Panel();
            this.pictureBoxM2Spine = new System.Windows.Forms.PictureBox();
            this.pictureBoxM2Front = new System.Windows.Forms.PictureBox();
            this.comboBoxSpineTemplates = new System.Windows.Forms.ComboBox();
            this.checkBoxSpine = new System.Windows.Forms.CheckBox();
            this.checkBoxFront = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxSearchTerm = new System.Windows.Forms.TextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonScraperPrevious = new System.Windows.Forms.Button();
            this.buttonScraperNext = new System.Windows.Forms.Button();
            this.listViewScraperResults = new System.Windows.Forms.ListView();
            this.scraperGameName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxScrapers = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelArt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Spine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Front)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.listViewGames, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonOk, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxScrapers, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // listViewGames
            // 
            resources.ApplyResources(this.listViewGames, "listViewGames");
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.gameName});
            this.listViewGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewGames.HideSelection = false;
            this.listViewGames.MultiSelect = false;
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewGames_ItemSelectionChanged);
            // 
            // gameName
            // 
            resources.ApplyResources(this.gameName, "gameName");
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 1, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.textBoxDescription, 1, 15);
            this.tableLayoutPanel3.Controls.Add(this.maxPlayersComboBox, 1, 13);
            this.tableLayoutPanel3.Controls.Add(this.comboBoxGenre, 1, 11);
            this.tableLayoutPanel3.Controls.Add(this.maskedTextBoxReleaseDate, 1, 7);
            this.tableLayoutPanel3.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label4, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.label5, 1, 6);
            this.tableLayoutPanel3.Controls.Add(this.label6, 1, 8);
            this.tableLayoutPanel3.Controls.Add(this.label7, 1, 10);
            this.tableLayoutPanel3.Controls.Add(this.label8, 1, 12);
            this.tableLayoutPanel3.Controls.Add(this.label9, 1, 14);
            this.tableLayoutPanel3.Controls.Add(this.textBoxName, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBoxPublisher, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.textBoxDeveloper, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.textBoxCopyright, 1, 9);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxName, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxPublisher, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxDeveloper, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxReleaseDate, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxCopyright, 0, 9);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxGenre, 0, 11);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxPlayerCount, 0, 13);
            this.tableLayoutPanel3.Controls.Add(this.checkBoxDescription, 0, 15);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel2.SetRowSpan(this.tableLayoutPanel3, 2);
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Description;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // maxPlayersComboBox
            // 
            resources.ApplyResources(this.maxPlayersComboBox, "maxPlayersComboBox");
            this.maxPlayersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maxPlayersComboBox.FormattingEnabled = true;
            this.maxPlayersComboBox.Name = "maxPlayersComboBox";
            this.maxPlayersComboBox.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.PlayerCount;
            this.maxPlayersComboBox.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // comboBoxGenre
            // 
            resources.ApplyResources(this.comboBoxGenre, "comboBoxGenre");
            this.comboBoxGenre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGenre.FormattingEnabled = true;
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Genre;
            this.comboBoxGenre.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // maskedTextBoxReleaseDate
            // 
            resources.ApplyResources(this.maskedTextBoxReleaseDate, "maskedTextBoxReleaseDate");
            this.maskedTextBoxReleaseDate.BackColor = System.Drawing.SystemColors.Window;
            this.maskedTextBoxReleaseDate.Name = "maskedTextBoxReleaseDate";
            this.maskedTextBoxReleaseDate.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.ReleaseDate;
            this.maskedTextBoxReleaseDate.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Name;
            this.textBoxName.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // textBoxPublisher
            // 
            resources.ApplyResources(this.textBoxPublisher, "textBoxPublisher");
            this.textBoxPublisher.Name = "textBoxPublisher";
            this.textBoxPublisher.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Publisher;
            this.textBoxPublisher.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // textBoxDeveloper
            // 
            resources.ApplyResources(this.textBoxDeveloper, "textBoxDeveloper");
            this.textBoxDeveloper.Name = "textBoxDeveloper";
            this.textBoxDeveloper.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Developer;
            this.textBoxDeveloper.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // textBoxCopyright
            // 
            resources.ApplyResources(this.textBoxCopyright, "textBoxCopyright");
            this.textBoxCopyright.Name = "textBoxCopyright";
            this.textBoxCopyright.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Copyright;
            this.textBoxCopyright.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // checkBoxName
            // 
            resources.ApplyResources(this.checkBoxName, "checkBoxName");
            this.checkBoxName.Name = "checkBoxName";
            this.checkBoxName.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Name;
            this.checkBoxName.UseVisualStyleBackColor = true;
            this.checkBoxName.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxPublisher
            // 
            resources.ApplyResources(this.checkBoxPublisher, "checkBoxPublisher");
            this.checkBoxPublisher.Name = "checkBoxPublisher";
            this.checkBoxPublisher.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Publisher;
            this.checkBoxPublisher.UseVisualStyleBackColor = true;
            this.checkBoxPublisher.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxDeveloper
            // 
            resources.ApplyResources(this.checkBoxDeveloper, "checkBoxDeveloper");
            this.checkBoxDeveloper.Name = "checkBoxDeveloper";
            this.checkBoxDeveloper.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Developer;
            this.checkBoxDeveloper.UseVisualStyleBackColor = true;
            this.checkBoxDeveloper.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxReleaseDate
            // 
            resources.ApplyResources(this.checkBoxReleaseDate, "checkBoxReleaseDate");
            this.checkBoxReleaseDate.Name = "checkBoxReleaseDate";
            this.checkBoxReleaseDate.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.ReleaseDate;
            this.checkBoxReleaseDate.UseVisualStyleBackColor = true;
            this.checkBoxReleaseDate.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxCopyright
            // 
            resources.ApplyResources(this.checkBoxCopyright, "checkBoxCopyright");
            this.checkBoxCopyright.Name = "checkBoxCopyright";
            this.checkBoxCopyright.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Copyright;
            this.checkBoxCopyright.UseVisualStyleBackColor = true;
            this.checkBoxCopyright.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxGenre
            // 
            resources.ApplyResources(this.checkBoxGenre, "checkBoxGenre");
            this.checkBoxGenre.Name = "checkBoxGenre";
            this.checkBoxGenre.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Genre;
            this.checkBoxGenre.UseVisualStyleBackColor = true;
            this.checkBoxGenre.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxPlayerCount
            // 
            resources.ApplyResources(this.checkBoxPlayerCount, "checkBoxPlayerCount");
            this.checkBoxPlayerCount.Name = "checkBoxPlayerCount";
            this.checkBoxPlayerCount.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.PlayerCount;
            this.checkBoxPlayerCount.UseVisualStyleBackColor = true;
            this.checkBoxPlayerCount.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxDescription
            // 
            resources.ApplyResources(this.checkBoxDescription, "checkBoxDescription");
            this.checkBoxDescription.Name = "checkBoxDescription";
            this.checkBoxDescription.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Description;
            this.checkBoxDescription.UseVisualStyleBackColor = true;
            this.checkBoxDescription.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.panelArt);
            this.panel1.Controls.Add(this.checkBoxSpine);
            this.panel1.Controls.Add(this.checkBoxFront);
            this.panel1.Name = "panel1";
            // 
            // panelArt
            // 
            resources.ApplyResources(this.panelArt, "panelArt");
            this.panelArt.Controls.Add(this.pictureBoxM2Spine);
            this.panelArt.Controls.Add(this.pictureBoxM2Front);
            this.panelArt.Controls.Add(this.comboBoxSpineTemplates);
            this.panelArt.Name = "panelArt";
            // 
            // pictureBoxM2Spine
            // 
            resources.ApplyResources(this.pictureBoxM2Spine, "pictureBoxM2Spine");
            this.pictureBoxM2Spine.AllowDrop = true;
            this.pictureBoxM2Spine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxM2Spine.Name = "pictureBoxM2Spine";
            this.pictureBoxM2Spine.TabStop = false;
            this.pictureBoxM2Spine.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.SpineArt;
            this.pictureBoxM2Spine.Click += new System.EventHandler(this.pictureBoxM2Spine_Click);
            // 
            // pictureBoxM2Front
            // 
            resources.ApplyResources(this.pictureBoxM2Front, "pictureBoxM2Front");
            this.pictureBoxM2Front.AllowDrop = true;
            this.pictureBoxM2Front.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxM2Front.Name = "pictureBoxM2Front";
            this.pictureBoxM2Front.TabStop = false;
            this.pictureBoxM2Front.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.FrontArt;
            this.pictureBoxM2Front.Click += new System.EventHandler(this.pictureBoxM2Front_Click);
            // 
            // comboBoxSpineTemplates
            // 
            resources.ApplyResources(this.comboBoxSpineTemplates, "comboBoxSpineTemplates");
            this.comboBoxSpineTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpineTemplates.FormattingEnabled = true;
            this.comboBoxSpineTemplates.Name = "comboBoxSpineTemplates";
            this.comboBoxSpineTemplates.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.SpineTemplate;
            this.comboBoxSpineTemplates.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // checkBoxSpine
            // 
            resources.ApplyResources(this.checkBoxSpine, "checkBoxSpine");
            this.checkBoxSpine.Name = "checkBoxSpine";
            this.checkBoxSpine.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.SpineArt;
            this.checkBoxSpine.UseVisualStyleBackColor = true;
            this.checkBoxSpine.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxFront
            // 
            resources.ApplyResources(this.checkBoxFront, "checkBoxFront");
            this.checkBoxFront.Name = "checkBoxFront";
            this.checkBoxFront.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.FrontArt;
            this.checkBoxFront.UseVisualStyleBackColor = true;
            this.checkBoxFront.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.listViewScraperResults, 0, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
            this.tableLayoutPanel6.Controls.Add(this.textBoxSearchTerm, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.buttonSearch, 1, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // textBoxSearchTerm
            // 
            resources.ApplyResources(this.textBoxSearchTerm, "textBoxSearchTerm");
            this.textBoxSearchTerm.Name = "textBoxSearchTerm";
            this.textBoxSearchTerm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchTerm_KeyDown);
            // 
            // buttonSearch
            // 
            resources.ApplyResources(this.buttonSearch, "buttonSearch");
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.buttonScraperPrevious, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.buttonScraperNext, 1, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // buttonScraperPrevious
            // 
            resources.ApplyResources(this.buttonScraperPrevious, "buttonScraperPrevious");
            this.buttonScraperPrevious.Name = "buttonScraperPrevious";
            this.buttonScraperPrevious.Tag = com.clusterrr.hakchi_gui.ScraperForm.PageDirection.Previous;
            this.buttonScraperPrevious.UseVisualStyleBackColor = true;
            this.buttonScraperPrevious.Click += new System.EventHandler(this.SwitchPage);
            // 
            // buttonScraperNext
            // 
            resources.ApplyResources(this.buttonScraperNext, "buttonScraperNext");
            this.buttonScraperNext.Name = "buttonScraperNext";
            this.buttonScraperNext.Tag = com.clusterrr.hakchi_gui.ScraperForm.PageDirection.Next;
            this.buttonScraperNext.UseVisualStyleBackColor = true;
            this.buttonScraperNext.Click += new System.EventHandler(this.SwitchPage);
            // 
            // listViewScraperResults
            // 
            resources.ApplyResources(this.listViewScraperResults, "listViewScraperResults");
            this.listViewScraperResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.scraperGameName});
            this.listViewScraperResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewScraperResults.HideSelection = false;
            this.listViewScraperResults.MultiSelect = false;
            this.listViewScraperResults.Name = "listViewScraperResults";
            this.listViewScraperResults.UseCompatibleStateImageBehavior = false;
            this.listViewScraperResults.View = System.Windows.Forms.View.Details;
            this.listViewScraperResults.SelectedIndexChanged += new System.EventHandler(this.listViewScraperResults_SelectedIndexChanged);
            // 
            // scraperGameName
            // 
            resources.ApplyResources(this.scraperGameName, "scraperGameName");
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxScrapers
            // 
            resources.ApplyResources(this.comboBoxScrapers, "comboBoxScrapers");
            this.comboBoxScrapers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScrapers.FormattingEnabled = true;
            this.comboBoxScrapers.Name = "comboBoxScrapers";
            this.comboBoxScrapers.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // ScraperForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "ScraperForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScraperForm_FormClosing);
            this.Load += new System.EventHandler(this.ScraperForm_Load);
            this.Shown += new System.EventHandler(this.ScraperForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelArt.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Spine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Front)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.ColumnHeader gameName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxPublisher;
        private System.Windows.Forms.TextBox textBoxDeveloper;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxReleaseDate;
        private System.Windows.Forms.TextBox textBoxCopyright;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private System.Windows.Forms.ComboBox maxPlayersComboBox;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBoxM2Spine;
        private System.Windows.Forms.PictureBox pictureBoxM2Front;
        private System.Windows.Forms.Panel panelArt;
        private System.Windows.Forms.ComboBox comboBoxSpineTemplates;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button buttonScraperPrevious;
        private System.Windows.Forms.Button buttonScraperNext;
        private System.Windows.Forms.ListView listViewScraperResults;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox comboBoxScrapers;
        private System.Windows.Forms.ColumnHeader scraperGameName;
        private System.Windows.Forms.CheckBox checkBoxName;
        private System.Windows.Forms.CheckBox checkBoxPublisher;
        private System.Windows.Forms.CheckBox checkBoxDeveloper;
        private System.Windows.Forms.CheckBox checkBoxReleaseDate;
        private System.Windows.Forms.CheckBox checkBoxCopyright;
        private System.Windows.Forms.CheckBox checkBoxGenre;
        private System.Windows.Forms.CheckBox checkBoxPlayerCount;
        private System.Windows.Forms.CheckBox checkBoxDescription;
        private System.Windows.Forms.CheckBox checkBoxSpine;
        private System.Windows.Forms.CheckBox checkBoxFront;
        private System.Windows.Forms.TextBox textBoxSearchTerm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button buttonSearch;
    }
}