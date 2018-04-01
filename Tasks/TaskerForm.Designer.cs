namespace com.clusterrr.hakchi_gui.Tasks
{
    partial class TaskerForm
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
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBarEx1 = new ProgressODoom.ProgressBarEx();
            this.fruityLoopsBackgroundPainter1 = new ProgressODoom.FruityLoopsBackgroundPainter();
            this.styledBorderPainter1 = new ProgressODoom.StyledBorderPainter();
            this.fruityLoopsProgressPainter1 = new ProgressODoom.FruityLoopsProgressPainter();
            this.statusPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(50, 21);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(46, 13);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Status...";
            // 
            // progressBarEx1
            // 
            this.progressBarEx1.BackgroundPainter = this.fruityLoopsBackgroundPainter1;
            this.progressBarEx1.BorderPainter = this.styledBorderPainter1;
            this.progressBarEx1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressBarEx1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.progressBarEx1.Location = new System.Drawing.Point(12, 49);
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
            this.progressBarEx1.Size = new System.Drawing.Size(538, 46);
            this.progressBarEx1.TabIndex = 0;
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
            // statusPictureBox
            // 
            this.statusPictureBox.Location = new System.Drawing.Point(12, 11);
            this.statusPictureBox.Name = "statusPictureBox";
            this.statusPictureBox.Size = new System.Drawing.Size(32, 32);
            this.statusPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.statusPictureBox.TabIndex = 1;
            this.statusPictureBox.TabStop = false;
            // 
            // TaskerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 107);
            this.Controls.Add(this.statusPictureBox);
            this.Controls.Add(this.progressBarEx1);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskerForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tasker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TaskerForm_FormClosing);
            this.Load += new System.EventHandler(this.Tasker_Load);
            this.Shown += new System.EventHandler(this.TaskerForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label statusLabel;
        private ProgressODoom.ProgressBarEx progressBarEx1;
        private ProgressODoom.FruityLoopsBackgroundPainter fruityLoopsBackgroundPainter1;
        private ProgressODoom.FruityLoopsProgressPainter fruityLoopsProgressPainter1;
        private ProgressODoom.StyledBorderPainter styledBorderPainter1;
        private System.Windows.Forms.PictureBox statusPictureBox;
    }
}