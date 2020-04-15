using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ImagesForm : Form
    {
        public ImagesForm()
        {
            InitializeComponent();
            Left = Cursor.Position.X + 5;
            Top = Cursor.Position.Y + 5;
        }

        public void ShowImages(IEnumerable<Image> images)
        {
            int i = 0;
            Control[] pboxes;
            do
            {
                i++;
                pboxes = this.Controls.Find("pictureBox" + i, true);
                if (pboxes.Length > 0)
                {
                    (pboxes[0] as PictureBox).Image = null;
                    (pboxes[0] as PictureBox).Visible = false;
                }
            } while (pboxes.Length > 0);
            i = 0;
            foreach(var image in images)
            {
                i++;
                pboxes = this.Controls.Find("pictureBox"+i, true);
                if (pboxes.Length > 0)
                {
                    (pboxes[0] as PictureBox).Image = image;
                    (pboxes[0] as PictureBox).Visible = true;
                }
            }
        }

        private void ImagesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
