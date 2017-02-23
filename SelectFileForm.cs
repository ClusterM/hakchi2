using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectFileForm : Form
    {
        public SelectFileForm(string[] fileNames)
        {
            InitializeComponent();
            listBoxFiles.Items.Clear();
            listBoxFiles.Items.AddRange(fileNames);
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
