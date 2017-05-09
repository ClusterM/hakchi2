namespace com.clusterrr.hakchi_gui.UI.Components
{
    partial class GameDetail
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.buttonShowGameGenieDatabase = new System.Windows.Forms.Button();
            this.maskedTextBoxReleaseDate = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxGameGenie = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
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
            this.radioButtonTwo = new System.Windows.Forms.RadioButton();
            this.radioButtonOne = new System.Windows.Forms.RadioButton();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.groupBoxOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.buttonShowGameGenieDatabase);
            this.groupBoxOptions.Controls.Add(this.maskedTextBoxReleaseDate);
            this.groupBoxOptions.Controls.Add(this.label1);
            this.groupBoxOptions.Controls.Add(this.textBoxGameGenie);
            this.groupBoxOptions.Controls.Add(this.label7);
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
            this.groupBoxOptions.Controls.Add(this.radioButtonTwo);
            this.groupBoxOptions.Controls.Add(this.radioButtonOne);
            this.groupBoxOptions.Controls.Add(this.textBoxName);
            this.groupBoxOptions.Controls.Add(this.labelName);
            this.groupBoxOptions.Controls.Add(this.labelID);
            this.groupBoxOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOptions.Location = new System.Drawing.Point(0, 0);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(309, 538);
            this.groupBoxOptions.TabIndex = 5;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Game options";
            // 
            // buttonShowGameGenieDatabase
            // 
            this.buttonShowGameGenieDatabase.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonShowGameGenieDatabase.Location = new System.Drawing.Point(252, 286);
            this.buttonShowGameGenieDatabase.Name = "buttonShowGameGenieDatabase";
            this.buttonShowGameGenieDatabase.Size = new System.Drawing.Size(25, 20);
            this.buttonShowGameGenieDatabase.TabIndex = 12;
            this.buttonShowGameGenieDatabase.Text = "+";
            this.buttonShowGameGenieDatabase.UseVisualStyleBackColor = true;
            this.buttonShowGameGenieDatabase.Click += new System.EventHandler(this.buttonShowGameGenieDatabase_Click);
            // 
            // maskedTextBoxReleaseDate
            // 
            this.maskedTextBoxReleaseDate.Location = new System.Drawing.Point(210, 154);
            this.maskedTextBoxReleaseDate.Mask = "0000-00-00";
            this.maskedTextBoxReleaseDate.Name = "maskedTextBoxReleaseDate";
            this.maskedTextBoxReleaseDate.Size = new System.Drawing.Size(65, 20);
            this.maskedTextBoxReleaseDate.TabIndex = 6;
            this.maskedTextBoxReleaseDate.TextChanged += new System.EventHandler(this.maskedTextBoxReleaseDate_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(15, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Release date (YYYY-MM-DD):";
            // 
            // textBoxGameGenie
            // 
            this.textBoxGameGenie.Location = new System.Drawing.Point(19, 286);
            this.textBoxGameGenie.Name = "textBoxGameGenie";
            this.textBoxGameGenie.Size = new System.Drawing.Size(227, 20);
            this.textBoxGameGenie.TabIndex = 11;
            this.textBoxGameGenie.TextChanged += new System.EventHandler(this.textBoxGameGenie_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(16, 269);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(194, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Game Genie codes (comma separated):";
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(15, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 63);
            this.label6.TabIndex = 16;
            this.label6.Text = "Max players:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButtonTwoSim
            // 
            this.radioButtonTwoSim.AutoSize = true;
            this.radioButtonTwoSim.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonTwoSim.Location = new System.Drawing.Point(103, 122);
            this.radioButtonTwoSim.Name = "radioButtonTwoSim";
            this.radioButtonTwoSim.Size = new System.Drawing.Size(156, 17);
            this.radioButtonTwoSim.TabIndex = 5;
            this.radioButtonTwoSim.Text = "Two players, simultaneously";
            this.radioButtonTwoSim.UseVisualStyleBackColor = true;
            // 
            // buttonGoogle
            // 
            this.buttonGoogle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonGoogle.Location = new System.Drawing.Point(219, 427);
            this.buttonGoogle.Name = "buttonGoogle";
            this.buttonGoogle.Size = new System.Drawing.Size(61, 23);
            this.buttonGoogle.TabIndex = 15;
            this.buttonGoogle.Text = "Google";
            this.buttonGoogle.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseImage
            // 
            this.buttonBrowseImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBrowseImage.Location = new System.Drawing.Point(219, 384);
            this.buttonBrowseImage.Name = "buttonBrowseImage";
            this.buttonBrowseImage.Size = new System.Drawing.Size(61, 23);
            this.buttonBrowseImage.TabIndex = 13;
            this.buttonBrowseImage.Text = "Browse";
            this.buttonBrowseImage.UseVisualStyleBackColor = true;
            // 
            // pictureBoxArt
            // 
            this.pictureBoxArt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxArt.Location = new System.Drawing.Point(71, 316);
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.Size = new System.Drawing.Size(140, 204);
            this.pictureBoxArt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxArt.TabIndex = 12;
            this.pictureBoxArt.TabStop = false;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(15, 316);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 204);
            this.label4.TabIndex = 11;
            this.label4.Text = "Box art:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxArguments
            // 
            this.textBoxArguments.Location = new System.Drawing.Point(18, 237);
            this.textBoxArguments.Name = "textBoxArguments";
            this.textBoxArguments.Size = new System.Drawing.Size(257, 20);
            this.textBoxArguments.TabIndex = 10;
            this.textBoxArguments.TextChanged += new System.EventHandler(this.textBoxArguments_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(15, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(253, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Command line arguments (for advanced users only!):";
            // 
            // textBoxPublisher
            // 
            this.textBoxPublisher.Location = new System.Drawing.Point(71, 186);
            this.textBoxPublisher.Name = "textBoxPublisher";
            this.textBoxPublisher.Size = new System.Drawing.Size(204, 20);
            this.textBoxPublisher.TabIndex = 8;
            this.textBoxPublisher.TextChanged += new System.EventHandler(this.textBoxPublisher_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(15, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Publisher:";
            // 
            // radioButtonTwo
            // 
            this.radioButtonTwo.AutoSize = true;
            this.radioButtonTwo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonTwo.Location = new System.Drawing.Point(103, 99);
            this.radioButtonTwo.Name = "radioButtonTwo";
            this.radioButtonTwo.Size = new System.Drawing.Size(174, 17);
            this.radioButtonTwo.TabIndex = 4;
            this.radioButtonTwo.Text = "Two players, not simultaneously";
            this.radioButtonTwo.UseVisualStyleBackColor = true;
            // 
            // radioButtonOne
            // 
            this.radioButtonOne.AutoSize = true;
            this.radioButtonOne.Checked = true;
            this.radioButtonOne.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonOne.Location = new System.Drawing.Point(103, 76);
            this.radioButtonOne.Name = "radioButtonOne";
            this.radioButtonOne.Size = new System.Drawing.Size(76, 17);
            this.radioButtonOne.TabIndex = 3;
            this.radioButtonOne.TabStop = true;
            this.radioButtonOne.Text = "One player";
            this.radioButtonOne.UseVisualStyleBackColor = true;
            this.radioButtonOne.CheckedChanged += new System.EventHandler(this.radioButtonOne_CheckedChanged);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(59, 46);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(216, 20);
            this.textBoxName.TabIndex = 2;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelName.Location = new System.Drawing.Point(15, 49);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name:";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelID.Location = new System.Drawing.Point(15, 21);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(21, 13);
            this.labelID.TabIndex = 0;
            this.labelID.Text = "ID:";
            // 
            // GameDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxOptions);
            this.Name = "GameDetail";
            this.Size = new System.Drawing.Size(309, 538);
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button buttonShowGameGenieDatabase;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxReleaseDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxGameGenie;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioButtonTwoSim;
        private System.Windows.Forms.Button buttonGoogle;
        private System.Windows.Forms.Button buttonBrowseImage;
        private System.Windows.Forms.PictureBox pictureBoxArt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxArguments;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPublisher;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonTwo;
        private System.Windows.Forms.RadioButton radioButtonOne;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelID;
    }
}
