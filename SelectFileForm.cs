using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectFileForm : Form
    {
        public SelectFileForm(string[] fileNames, string title = null, string secondButton = null)
        {
            InitializeComponent();
            listBoxFiles.Items.Clear();
            listBoxFiles.Items.AddRange(fileNames);
            if (!string.IsNullOrEmpty(title))
                this.Text = title;
            if (!string.IsNullOrEmpty(secondButton))
                this.buttonArchive.Text = secondButton;
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = listBoxFiles.SelectedItems.Count > 0;
        }

        private void listBoxFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
