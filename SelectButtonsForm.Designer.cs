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
            this.checkBoxUp = new System.Windows.Forms.CheckBox();
            this.checkBoxDown = new System.Windows.Forms.CheckBox();
            this.checkBoxLeft = new System.Windows.Forms.CheckBox();
            this.checkBoxRight = new System.Windows.Forms.CheckBox();
            this.checkBoxSelect = new System.Windows.Forms.CheckBox();
            this.checkBoxStart = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxA = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxUp
            // 
            resources.ApplyResources(this.checkBoxUp, "checkBoxUp");
            this.checkBoxUp.Name = "checkBoxUp";
            this.checkBoxUp.UseVisualStyleBackColor = true;
            this.checkBoxUp.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxDown
            // 
            resources.ApplyResources(this.checkBoxDown, "checkBoxDown");
            this.checkBoxDown.Name = "checkBoxDown";
            this.checkBoxDown.UseVisualStyleBackColor = true;
            this.checkBoxDown.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxLeft
            // 
            resources.ApplyResources(this.checkBoxLeft, "checkBoxLeft");
            this.checkBoxLeft.Name = "checkBoxLeft";
            this.checkBoxLeft.UseVisualStyleBackColor = true;
            this.checkBoxLeft.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxRight
            // 
            resources.ApplyResources(this.checkBoxRight, "checkBoxRight");
            this.checkBoxRight.Name = "checkBoxRight";
            this.checkBoxRight.UseVisualStyleBackColor = true;
            this.checkBoxRight.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSelect
            // 
            resources.ApplyResources(this.checkBoxSelect, "checkBoxSelect");
            this.checkBoxSelect.Name = "checkBoxSelect";
            this.checkBoxSelect.UseVisualStyleBackColor = true;
            this.checkBoxSelect.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxStart
            // 
            resources.ApplyResources(this.checkBoxStart, "checkBoxStart");
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.UseVisualStyleBackColor = true;
            this.checkBoxStart.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxB
            // 
            resources.ApplyResources(this.checkBoxB, "checkBoxB");
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.UseVisualStyleBackColor = true;
            this.checkBoxB.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxA
            // 
            resources.ApplyResources(this.checkBoxA, "checkBoxA");
            this.checkBoxA.Name = "checkBoxA";
            this.checkBoxA.UseVisualStyleBackColor = true;
            this.checkBoxA.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.checkBoxUp);
            this.groupBox.Controls.Add(this.checkBoxDown);
            this.groupBox.Controls.Add(this.checkBoxA);
            this.groupBox.Controls.Add(this.checkBoxLeft);
            this.groupBox.Controls.Add(this.checkBoxB);
            this.groupBox.Controls.Add(this.checkBoxRight);
            this.groupBox.Controls.Add(this.checkBoxStart);
            this.groupBox.Controls.Add(this.checkBoxSelect);
            resources.ApplyResources(this.groupBox, "groupBox");
            this.groupBox.Name = "groupBox";
            this.groupBox.TabStop = false;
            // 
            // SelectButtonsForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SelectButtonsForm";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxUp;
        private System.Windows.Forms.CheckBox checkBoxDown;
        private System.Windows.Forms.CheckBox checkBoxLeft;
        private System.Windows.Forms.CheckBox checkBoxRight;
        private System.Windows.Forms.CheckBox checkBoxSelect;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxA;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox;
    }
}