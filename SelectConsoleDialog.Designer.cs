namespace com.clusterrr.hakchi_gui
{
    partial class SelectConsoleDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectConsoleDialog));
            this.labelSelectConsole = new System.Windows.Forms.Label();
            this.buttonNes = new System.Windows.Forms.Button();
            this.buttonFamicom = new System.Windows.Forms.Button();
            this.buttonSnes = new System.Windows.Forms.Button();
            this.buttonSuperFamicom = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelSelectConsole
            // 
            resources.ApplyResources(this.labelSelectConsole, "labelSelectConsole");
            this.labelSelectConsole.Name = "labelSelectConsole";
            // 
            // buttonNes
            // 
            resources.ApplyResources(this.buttonNes, "buttonNes");
            this.buttonNes.Name = "buttonNes";
            this.buttonNes.UseVisualStyleBackColor = true;
            this.buttonNes.Click += new System.EventHandler(this.buttonNes_Click);
            // 
            // buttonFamicom
            // 
            resources.ApplyResources(this.buttonFamicom, "buttonFamicom");
            this.buttonFamicom.Name = "buttonFamicom";
            this.buttonFamicom.UseVisualStyleBackColor = true;
            this.buttonFamicom.Click += new System.EventHandler(this.buttonFamicom_Click);
            // 
            // buttonSnes
            // 
            resources.ApplyResources(this.buttonSnes, "buttonSnes");
            this.buttonSnes.Name = "buttonSnes";
            this.buttonSnes.UseVisualStyleBackColor = true;
            this.buttonSnes.Click += new System.EventHandler(this.buttonSnes_Click);
            // 
            // buttonSuperFamicom
            // 
            resources.ApplyResources(this.buttonSuperFamicom, "buttonSuperFamicom");
            this.buttonSuperFamicom.Name = "buttonSuperFamicom";
            this.buttonSuperFamicom.UseVisualStyleBackColor = true;
            this.buttonSuperFamicom.Click += new System.EventHandler(this.buttonSuperFamicom_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // SelectConsoleDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonSuperFamicom);
            this.Controls.Add(this.buttonSnes);
            this.Controls.Add(this.buttonFamicom);
            this.Controls.Add(this.buttonNes);
            this.Controls.Add(this.labelSelectConsole);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectConsoleDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SelectConsoleDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSelectConsole;
        private System.Windows.Forms.Button buttonNes;
        private System.Windows.Forms.Button buttonFamicom;
        private System.Windows.Forms.Button buttonSnes;
        private System.Windows.Forms.Button buttonSuperFamicom;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}