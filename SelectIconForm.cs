using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectIconForm : Form
    {
        public SelectIconForm(string selected = null)
        {
            InitializeComponent();
            listBox.Items.Clear();
            var files = Directory.GetFiles(NesMenuFolder.FolderImagesDirectory, "*.png", SearchOption.AllDirectories);
            listBox.Items.AddRange((from f in files select Path.GetFileNameWithoutExtension(f)).ToArray());

            if (selected != null)
                for (int i = 0; i < listBox.Items.Count; i++)
                    if (listBox.Items[i].ToString() == selected)
                    {
                        listBox.SelectedIndex = i;
                        break;
                    }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedItems.Count > 0)
            {
                buttonOk.Enabled = true;
                pictureBoxArt.Image = Image.FromFile(Path.Combine(NesMenuFolder.FolderImagesDirectory, listBox.SelectedItems[0] + ".png"));
            }
            else
            {
                buttonOk.Enabled = false;
                pictureBoxArt.Image = null;
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
