namespace com.clusterrr.hakchi_gui
{
    partial class SelectButtonsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectButtonsForm));
            this.buttonOk = new System.Windows.Forms.Button();
            this.checkBoxUp = new System.Windows.Forms.CheckBox();
            this.checkBoxDown = new System.Windows.Forms.CheckBox();
            this.checkBoxA = new System.Windows.Forms.CheckBox();
            this.checkBoxLeft = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxRight = new System.Windows.Forms.CheckBox();
            this.checkBoxStart = new System.Windows.Forms.CheckBox();
            this.checkBoxSelect = new System.Windows.Forms.CheckBox();
            this.pictureBoxController = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxController)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // checkBoxUp
            // 
            this.checkBoxUp.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxUp, "checkBoxUp");
            this.checkBoxUp.Name = "checkBoxUp";
            this.checkBoxUp.UseVisualStyleBackColor = false;
            this.checkBoxUp.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxDown
            // 
            this.checkBoxDown.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxDown, "checkBoxDown");
            this.checkBoxDown.Name = "checkBoxDown";
            this.checkBoxDown.UseVisualStyleBackColor = false;
            this.checkBoxDown.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxA
            // 
            this.checkBoxA.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxA, "checkBoxA");
            this.checkBoxA.Name = "checkBoxA";
            this.checkBoxA.UseVisualStyleBackColor = false;
            this.checkBoxA.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxLeft
            // 
            this.checkBoxLeft.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxLeft, "checkBoxLeft");
            this.checkBoxLeft.Name = "checkBoxLeft";
            this.checkBoxLeft.UseVisualStyleBackColor = false;
            this.checkBoxLeft.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxB
            // 
            this.checkBoxB.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxB, "checkBoxB");
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.UseVisualStyleBackColor = false;
            this.checkBoxB.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxRight
            // 
            resources.ApplyResources(this.checkBoxRight, "checkBoxRight");
            this.checkBoxRight.Name = "checkBoxRight";
            this.checkBoxRight.UseVisualStyleBackColor = true;
            this.checkBoxRight.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxStart
            // 
            this.checkBoxStart.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxStart, "checkBoxStart");
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.UseVisualStyleBackColor = false;
            this.checkBoxStart.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSelect
            // 
            this.checkBoxSelect.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxSelect, "checkBoxSelect");
            this.checkBoxSelect.Name = "checkBoxSelect";
            this.checkBoxSelect.UseVisualStyleBackColor = false;
            this.checkBoxSelect.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // pictureBoxController
            // 
            this.pictureBoxController.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.gamepad;
            resources.ApplyResources(this.pictureBoxController, "pictureBoxController");
            this.pictureBoxController.Name = "pictureBoxController";
            this.pictureBoxController.TabStop = false;
            // 
            // SelectButtonsForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxUp);
            this.Controls.Add(this.checkBoxDown);
            this.Controls.Add(this.checkBoxA);
            this.Controls.Add(this.checkBoxLeft);
            this.Controls.Add(this.checkBoxB);
            this.Controls.Add(this.checkBoxRight);
            this.Controls.Add(this.checkBoxStart);
            this.Controls.Add(this.checkBoxSelect);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.pictureBoxController);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectButtonsForm";
            this.Load += new System.EventHandler(this.SelectButtonsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxController)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.CheckBox checkBoxUp;
        private System.Windows.Forms.CheckBox checkBoxDown;
        private System.Windows.Forms.CheckBox checkBoxA;
        private System.Windows.Forms.CheckBox checkBoxLeft;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxRight;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.CheckBox checkBoxSelect;
        private System.Windows.Forms.PictureBox pictureBoxController;
    }
}