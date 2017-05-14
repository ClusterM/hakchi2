using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SearchForm : Form
    {
        MainForm mainForm;

        public SearchForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSearch.Text.Length > 0)
            {
                Manager.EventBus.getInstance().Search(textBoxSearch.Text);
               
            }
        }

        private void SearchForm_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void SearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
                Close();
        }
    }
}
