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
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.listViewGames, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonOk, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxScrapers, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1047, 494);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listViewGames
            // 
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.gameName});
            this.listViewGames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewGames.HideSelection = false;
            this.listViewGames.Location = new System.Drawing.Point(6, 33);
            this.listViewGames.Margin = new System.Windows.Forms.Padding(6, 6, 0, 6);
            this.listViewGames.MinimumSize = new System.Drawing.Size(288, 4);
            this.listViewGames.MultiSelect = false;
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.Size = new System.Drawing.Size(288, 426);
            this.listViewGames.TabIndex = 2;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewGames_ItemSelectionChanged);
            // 
            // gameName
            // 
            this.gameName.Text = "Game name";
            this.gameName.Width = 284;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(300, 33);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(741, 426);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
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
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 16;
            this.tableLayoutPanel2.SetRowSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(370, 426);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(15, 291);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(355, 135);
            this.textBoxDescription.TabIndex = 39;
            this.textBoxDescription.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Description;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // maxPlayersComboBox
            // 
            this.maxPlayersComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.maxPlayersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maxPlayersComboBox.FormattingEnabled = true;
            this.maxPlayersComboBox.Location = new System.Drawing.Point(15, 251);
            this.maxPlayersComboBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.maxPlayersComboBox.Name = "maxPlayersComboBox";
            this.maxPlayersComboBox.Size = new System.Drawing.Size(355, 21);
            this.maxPlayersComboBox.TabIndex = 38;
            this.maxPlayersComboBox.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.PlayerCount;
            this.maxPlayersComboBox.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // comboBoxGenre
            // 
            this.comboBoxGenre.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxGenre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGenre.FormattingEnabled = true;
            this.comboBoxGenre.Location = new System.Drawing.Point(15, 211);
            this.comboBoxGenre.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.Size = new System.Drawing.Size(355, 21);
            this.comboBoxGenre.TabIndex = 37;
            this.comboBoxGenre.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Genre;
            this.comboBoxGenre.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // maskedTextBoxReleaseDate
            // 
            this.maskedTextBoxReleaseDate.BackColor = System.Drawing.SystemColors.Window;
            this.maskedTextBoxReleaseDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.maskedTextBoxReleaseDate.Location = new System.Drawing.Point(15, 133);
            this.maskedTextBoxReleaseDate.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.maskedTextBoxReleaseDate.Mask = "0000-00-00";
            this.maskedTextBoxReleaseDate.Name = "maskedTextBoxReleaseDate";
            this.maskedTextBoxReleaseDate.Size = new System.Drawing.Size(355, 20);
            this.maskedTextBoxReleaseDate.TabIndex = 14;
            this.maskedTextBoxReleaseDate.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.ReleaseDate;
            this.maskedTextBoxReleaseDate.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 39);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Publisher";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 78);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Developer";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 117);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Release Date";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 156);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Copyright";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 195);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Genre";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 235);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Player Count";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 275);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Description";
            // 
            // textBoxName
            // 
            this.textBoxName.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxName.Location = new System.Drawing.Point(15, 16);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(355, 20);
            this.textBoxName.TabIndex = 9;
            this.textBoxName.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Name;
            this.textBoxName.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // textBoxPublisher
            // 
            this.textBoxPublisher.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxPublisher.Location = new System.Drawing.Point(15, 55);
            this.textBoxPublisher.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.textBoxPublisher.Name = "textBoxPublisher";
            this.textBoxPublisher.Size = new System.Drawing.Size(355, 20);
            this.textBoxPublisher.TabIndex = 9;
            this.textBoxPublisher.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Publisher;
            this.textBoxPublisher.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // textBoxDeveloper
            // 
            this.textBoxDeveloper.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxDeveloper.Location = new System.Drawing.Point(15, 94);
            this.textBoxDeveloper.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.textBoxDeveloper.Name = "textBoxDeveloper";
            this.textBoxDeveloper.Size = new System.Drawing.Size(355, 20);
            this.textBoxDeveloper.TabIndex = 9;
            this.textBoxDeveloper.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Developer;
            this.textBoxDeveloper.Visible = false;
            this.textBoxDeveloper.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // textBoxCopyright
            // 
            this.textBoxCopyright.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxCopyright.Location = new System.Drawing.Point(15, 172);
            this.textBoxCopyright.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.textBoxCopyright.Name = "textBoxCopyright";
            this.textBoxCopyright.Size = new System.Drawing.Size(355, 20);
            this.textBoxCopyright.TabIndex = 9;
            this.textBoxCopyright.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Copyright;
            this.textBoxCopyright.TextChanged += new System.EventHandler(this.DataChanged);
            // 
            // checkBoxName
            // 
            this.checkBoxName.AutoSize = true;
            this.checkBoxName.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxName.Location = new System.Drawing.Point(0, 16);
            this.checkBoxName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxName.Name = "checkBoxName";
            this.checkBoxName.Size = new System.Drawing.Size(15, 14);
            this.checkBoxName.TabIndex = 40;
            this.checkBoxName.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Name;
            this.checkBoxName.UseVisualStyleBackColor = true;
            this.checkBoxName.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxPublisher
            // 
            this.checkBoxPublisher.AutoSize = true;
            this.checkBoxPublisher.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxPublisher.Location = new System.Drawing.Point(0, 55);
            this.checkBoxPublisher.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxPublisher.Name = "checkBoxPublisher";
            this.checkBoxPublisher.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPublisher.TabIndex = 41;
            this.checkBoxPublisher.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Publisher;
            this.checkBoxPublisher.UseVisualStyleBackColor = true;
            this.checkBoxPublisher.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxDeveloper
            // 
            this.checkBoxDeveloper.AutoSize = true;
            this.checkBoxDeveloper.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxDeveloper.Location = new System.Drawing.Point(0, 94);
            this.checkBoxDeveloper.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxDeveloper.Name = "checkBoxDeveloper";
            this.checkBoxDeveloper.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDeveloper.TabIndex = 41;
            this.checkBoxDeveloper.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Developer;
            this.checkBoxDeveloper.UseVisualStyleBackColor = true;
            this.checkBoxDeveloper.Visible = false;
            this.checkBoxDeveloper.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxReleaseDate
            // 
            this.checkBoxReleaseDate.AutoSize = true;
            this.checkBoxReleaseDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxReleaseDate.Location = new System.Drawing.Point(0, 133);
            this.checkBoxReleaseDate.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxReleaseDate.Name = "checkBoxReleaseDate";
            this.checkBoxReleaseDate.Size = new System.Drawing.Size(15, 14);
            this.checkBoxReleaseDate.TabIndex = 41;
            this.checkBoxReleaseDate.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.ReleaseDate;
            this.checkBoxReleaseDate.UseVisualStyleBackColor = true;
            this.checkBoxReleaseDate.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxCopyright
            // 
            this.checkBoxCopyright.AutoSize = true;
            this.checkBoxCopyright.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxCopyright.Location = new System.Drawing.Point(0, 172);
            this.checkBoxCopyright.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxCopyright.Name = "checkBoxCopyright";
            this.checkBoxCopyright.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCopyright.TabIndex = 41;
            this.checkBoxCopyright.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Copyright;
            this.checkBoxCopyright.UseVisualStyleBackColor = true;
            this.checkBoxCopyright.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxGenre
            // 
            this.checkBoxGenre.AutoSize = true;
            this.checkBoxGenre.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxGenre.Location = new System.Drawing.Point(0, 211);
            this.checkBoxGenre.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxGenre.Name = "checkBoxGenre";
            this.checkBoxGenre.Size = new System.Drawing.Size(15, 14);
            this.checkBoxGenre.TabIndex = 41;
            this.checkBoxGenre.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Genre;
            this.checkBoxGenre.UseVisualStyleBackColor = true;
            this.checkBoxGenre.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxPlayerCount
            // 
            this.checkBoxPlayerCount.AutoSize = true;
            this.checkBoxPlayerCount.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxPlayerCount.Location = new System.Drawing.Point(0, 251);
            this.checkBoxPlayerCount.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxPlayerCount.Name = "checkBoxPlayerCount";
            this.checkBoxPlayerCount.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPlayerCount.TabIndex = 41;
            this.checkBoxPlayerCount.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.PlayerCount;
            this.checkBoxPlayerCount.UseVisualStyleBackColor = true;
            this.checkBoxPlayerCount.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxDescription
            // 
            this.checkBoxDescription.AutoSize = true;
            this.checkBoxDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxDescription.Location = new System.Drawing.Point(0, 291);
            this.checkBoxDescription.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxDescription.Name = "checkBoxDescription";
            this.checkBoxDescription.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDescription.TabIndex = 41;
            this.checkBoxDescription.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.Description;
            this.checkBoxDescription.UseVisualStyleBackColor = true;
            this.checkBoxDescription.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panelArt);
            this.panel1.Controls.Add(this.checkBoxSpine);
            this.panel1.Controls.Add(this.checkBoxFront);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(373, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(368, 237);
            this.panel1.TabIndex = 2;
            // 
            // panelArt
            // 
            this.panelArt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelArt.Controls.Add(this.pictureBoxM2Spine);
            this.panelArt.Controls.Add(this.pictureBoxM2Front);
            this.panelArt.Controls.Add(this.comboBoxSpineTemplates);
            this.panelArt.Location = new System.Drawing.Point(94, 0);
            this.panelArt.Margin = new System.Windows.Forms.Padding(0);
            this.panelArt.Name = "panelArt";
            this.panelArt.Size = new System.Drawing.Size(181, 237);
            this.panelArt.TabIndex = 39;
            // 
            // pictureBoxM2Spine
            // 
            this.pictureBoxM2Spine.AllowDrop = true;
            this.pictureBoxM2Spine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxM2Spine.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxM2Spine.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxM2Spine.MaximumSize = new System.Drawing.Size(30, 216);
            this.pictureBoxM2Spine.MinimumSize = new System.Drawing.Size(30, 216);
            this.pictureBoxM2Spine.Name = "pictureBoxM2Spine";
            this.pictureBoxM2Spine.Size = new System.Drawing.Size(30, 216);
            this.pictureBoxM2Spine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxM2Spine.TabIndex = 2;
            this.pictureBoxM2Spine.TabStop = false;
            this.pictureBoxM2Spine.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.SpineArt;
            this.pictureBoxM2Spine.Click += new System.EventHandler(this.pictureBoxM2Spine_Click);
            // 
            // pictureBoxM2Front
            // 
            this.pictureBoxM2Front.AllowDrop = true;
            this.pictureBoxM2Front.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxM2Front.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxM2Front.Location = new System.Drawing.Point(29, 0);
            this.pictureBoxM2Front.MaximumSize = new System.Drawing.Size(152, 216);
            this.pictureBoxM2Front.MinimumSize = new System.Drawing.Size(152, 216);
            this.pictureBoxM2Front.Name = "pictureBoxM2Front";
            this.pictureBoxM2Front.Size = new System.Drawing.Size(152, 216);
            this.pictureBoxM2Front.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxM2Front.TabIndex = 3;
            this.pictureBoxM2Front.TabStop = false;
            this.pictureBoxM2Front.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.FrontArt;
            this.pictureBoxM2Front.Click += new System.EventHandler(this.pictureBoxM2Front_Click);
            // 
            // comboBoxSpineTemplates
            // 
            this.comboBoxSpineTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpineTemplates.FormattingEnabled = true;
            this.comboBoxSpineTemplates.Location = new System.Drawing.Point(0, 216);
            this.comboBoxSpineTemplates.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.comboBoxSpineTemplates.Name = "comboBoxSpineTemplates";
            this.comboBoxSpineTemplates.Size = new System.Drawing.Size(181, 21);
            this.comboBoxSpineTemplates.TabIndex = 38;
            this.comboBoxSpineTemplates.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.SpineTemplate;
            this.comboBoxSpineTemplates.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // checkBoxSpine
            // 
            this.checkBoxSpine.AutoSize = true;
            this.checkBoxSpine.Location = new System.Drawing.Point(79, 0);
            this.checkBoxSpine.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxSpine.Name = "checkBoxSpine";
            this.checkBoxSpine.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSpine.TabIndex = 41;
            this.checkBoxSpine.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.SpineArt;
            this.checkBoxSpine.UseVisualStyleBackColor = true;
            this.checkBoxSpine.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // checkBoxFront
            // 
            this.checkBoxFront.AutoSize = true;
            this.checkBoxFront.Location = new System.Drawing.Point(277, 0);
            this.checkBoxFront.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.checkBoxFront.Name = "checkBoxFront";
            this.checkBoxFront.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFront.TabIndex = 41;
            this.checkBoxFront.Tag = com.clusterrr.hakchi_gui.ScraperForm.DataType.FrontArt;
            this.checkBoxFront.UseVisualStyleBackColor = true;
            this.checkBoxFront.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.listViewScraperResults, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(373, 243);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(368, 183);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.textBoxSearchTerm, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.buttonSearch, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(368, 25);
            this.tableLayoutPanel6.TabIndex = 3;
            // 
            // textBoxSearchTerm
            // 
            this.textBoxSearchTerm.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxSearchTerm.Location = new System.Drawing.Point(4, 1);
            this.textBoxSearchTerm.Margin = new System.Windows.Forms.Padding(4, 1, 3, 3);
            this.textBoxSearchTerm.MinimumSize = new System.Drawing.Size(4, 21);
            this.textBoxSearchTerm.Name = "textBoxSearchTerm";
            this.textBoxSearchTerm.Size = new System.Drawing.Size(277, 21);
            this.textBoxSearchTerm.TabIndex = 2;
            this.textBoxSearchTerm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchTerm_KeyDown);
            // 
            // buttonSearch
            // 
            this.buttonSearch.AutoSize = true;
            this.buttonSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonSearch.Location = new System.Drawing.Point(287, 0);
            this.buttonSearch.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.buttonSearch.Size = new System.Drawing.Size(81, 23);
            this.buttonSearch.TabIndex = 3;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.buttonScraperPrevious, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.buttonScraperNext, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 160);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(365, 23);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // buttonScraperPrevious
            // 
            this.buttonScraperPrevious.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonScraperPrevious.Enabled = false;
            this.buttonScraperPrevious.Location = new System.Drawing.Point(0, 0);
            this.buttonScraperPrevious.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.buttonScraperPrevious.Name = "buttonScraperPrevious";
            this.buttonScraperPrevious.Size = new System.Drawing.Size(179, 23);
            this.buttonScraperPrevious.TabIndex = 0;
            this.buttonScraperPrevious.Tag = com.clusterrr.hakchi_gui.ScraperForm.PageDirection.Previous;
            this.buttonScraperPrevious.Text = "< Previous";
            this.buttonScraperPrevious.UseVisualStyleBackColor = true;
            this.buttonScraperPrevious.Click += new System.EventHandler(this.SwitchPage);
            // 
            // buttonScraperNext
            // 
            this.buttonScraperNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonScraperNext.Enabled = false;
            this.buttonScraperNext.Location = new System.Drawing.Point(185, 0);
            this.buttonScraperNext.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.buttonScraperNext.Name = "buttonScraperNext";
            this.buttonScraperNext.Size = new System.Drawing.Size(180, 23);
            this.buttonScraperNext.TabIndex = 0;
            this.buttonScraperNext.Tag = com.clusterrr.hakchi_gui.ScraperForm.PageDirection.Next;
            this.buttonScraperNext.Text = "Next >";
            this.buttonScraperNext.UseVisualStyleBackColor = true;
            this.buttonScraperNext.Click += new System.EventHandler(this.SwitchPage);
            // 
            // listViewScraperResults
            // 
            this.listViewScraperResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.scraperGameName});
            this.listViewScraperResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewScraperResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewScraperResults.HideSelection = false;
            this.listViewScraperResults.Location = new System.Drawing.Point(4, 28);
            this.listViewScraperResults.Margin = new System.Windows.Forms.Padding(4, 3, 1, 3);
            this.listViewScraperResults.MultiSelect = false;
            this.listViewScraperResults.Name = "listViewScraperResults";
            this.listViewScraperResults.Size = new System.Drawing.Size(363, 126);
            this.listViewScraperResults.TabIndex = 1;
            this.listViewScraperResults.UseCompatibleStateImageBehavior = false;
            this.listViewScraperResults.View = System.Windows.Forms.View.Details;
            this.listViewScraperResults.SelectedIndexChanged += new System.EventHandler(this.listViewScraperResults_SelectedIndexChanged);
            // 
            // scraperGameName
            // 
            this.scraperGameName.Width = 359;
            // 
            // buttonOk
            // 
            this.buttonOk.AutoSize = true;
            this.buttonOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonOk.Location = new System.Drawing.Point(966, 465);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(3, 0, 6, 6);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxScrapers
            // 
            this.comboBoxScrapers.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxScrapers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScrapers.FormattingEnabled = true;
            this.comboBoxScrapers.Location = new System.Drawing.Point(6, 6);
            this.comboBoxScrapers.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.comboBoxScrapers.Name = "comboBoxScrapers";
            this.comboBoxScrapers.Size = new System.Drawing.Size(288, 21);
            this.comboBoxScrapers.TabIndex = 38;
            this.comboBoxScrapers.SelectedIndexChanged += new System.EventHandler(this.DataChanged);
            // 
            // ScraperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1059, 506);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.Name = "ScraperForm";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.Text = "Information Scraper";
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