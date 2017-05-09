namespace com.clusterrr.hakchi_gui.UI.Components
{
    partial class GameSelecter
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
            this.tvGameSelecter = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvGameSelecter
            // 
            this.tvGameSelecter.CheckBoxes = true;
            this.tvGameSelecter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvGameSelecter.Location = new System.Drawing.Point(0, 0);
            this.tvGameSelecter.Name = "tvGameSelecter";
            this.tvGameSelecter.Size = new System.Drawing.Size(495, 607);
            this.tvGameSelecter.TabIndex = 0;
            // 
            // GameSelecter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvGameSelecter);
            this.Name = "GameSelecter";
            this.Size = new System.Drawing.Size(495, 607);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvGameSelecter;
    }
}
