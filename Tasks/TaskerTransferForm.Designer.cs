namespace com.clusterrr.hakchi_gui.Tasks
{
    partial class TaskerTransferForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskerTransferForm));
            this.statusPictureBox = new System.Windows.Forms.PictureBox();
            this.progressBarEx1 = new ProgressODoom.ProgressBarEx();
            this.fruityLoopsBackgroundPainter1 = new ProgressODoom.FruityLoopsBackgroundPainter();
            this.styledBorderPainter1 = new ProgressODoom.StyledBorderPainter();
            this.fruityLoopsProgressPainter1 = new ProgressODoom.FruityLoopsProgressPainter();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFileName = new System.Windows.Forms.Label();
            this.labelTimeLeft = new System.Windows.Forms.Label();
            this.labelTransferRate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // statusPictureBox
            // 
            resources.ApplyResources(this.statusPictureBox, "statusPictureBox");
            this.statusPictureBox.Name = "statusPictureBox";
            this.statusPictureBox.TabStop = false;
            // 
            // progressBarEx1
            // 
            this.progressBarEx1.BackgroundPainter = this.fruityLoopsBackgroundPainter1;
            this.progressBarEx1.BorderPainter = this.styledBorderPainter1;
            resources.ApplyResources(this.progressBarEx1, "progressBarEx1");
            this.progressBarEx1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.progressBarEx1.MarqueePercentage = 25;
            this.progressBarEx1.MarqueeSpeed = 30;
            this.progressBarEx1.MarqueeStep = 1;
            this.progressBarEx1.Maximum = 100;
            this.progressBarEx1.Minimum = 0;
            this.progressBarEx1.Name = "progressBarEx1";
            this.progressBarEx1.ProgressPadding = 0;
            this.progressBarEx1.ProgressPainter = this.fruityLoopsProgressPainter1;
            this.progressBarEx1.ProgressType = ProgressODoom.ProgressType.Smooth;
            this.progressBarEx1.ShowPercentage = true;
            this.progressBarEx1.Value = 0;
            // 
            // fruityLoopsBackgroundPainter1
            // 
            this.fruityLoopsBackgroundPainter1.FruityType = ProgressODoom.FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer;
            this.fruityLoopsBackgroundPainter1.GlossPainter = null;
            // 
            // styledBorderPainter1
            // 
            this.styledBorderPainter1.Border3D = System.Windows.Forms.Border3DStyle.Flat;
            // 
            // fruityLoopsProgressPainter1
            // 
            this.fruityLoopsProgressPainter1.FruityType = ProgressODoom.FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer;
            this.fruityLoopsProgressPainter1.GlossPainter = null;
            this.fruityLoopsProgressPainter1.ProgressBorderPainter = null;
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // labelFileName
            // 
            resources.ApplyResources(this.labelFileName, "labelFileName");
            this.labelFileName.Name = "labelFileName";
            // 
            // labelTimeLeft
            // 
            resources.ApplyResources(this.labelTimeLeft, "labelTimeLeft");
            this.labelTimeLeft.Name = "labelTimeLeft";
            // 
            // labelTransferRate
            // 
            resources.ApplyResources(this.labelTransferRate, "labelTransferRate");
            this.labelTransferRate.Name = "labelTransferRate";
            // 
            // TaskerTransferForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelTransferRate);
            this.Controls.Add(this.labelTimeLeft);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusPictureBox);
            this.Controls.Add(this.progressBarEx1);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskerTransferForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TaskerTransferForm_FormClosing);
            this.Load += new System.EventHandler(this.TaskerTransferForm_Load);
            this.Shown += new System.EventHandler(this.TaskerTransferForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox statusPictureBox;
        private ProgressODoom.ProgressBarEx progressBarEx1;
        private System.Windows.Forms.Label statusLabel;
        private ProgressODoom.FruityLoopsBackgroundPainter fruityLoopsBackgroundPainter1;
        private ProgressODoom.StyledBorderPainter styledBorderPainter1;
        private ProgressODoom.FruityLoopsProgressPainter fruityLoopsProgressPainter1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.Label labelTimeLeft;
        private System.Windows.Forms.Label labelTransferRate;
    }
}