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
                for (int i = 1; i < mainForm.listViewGames.Items.Count; i++)
                    if ((mainForm.listViewGames.Items[i].Tag as NesApplication).Name.
                            ToLower().StartsWith(textBoxSearch.Text.ToLower()))
                    {
                        for (int j = 1; j < mainForm.listViewGames.Items.Count; j++)
                            mainForm.listViewGames.Items[j].Selected = i == j;
                        mainForm.listViewGames.EnsureVisible(i);
                        break;
                    }
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
