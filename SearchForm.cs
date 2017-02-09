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
                for (int i = 1; i < mainForm.checkedListBoxGames.Items.Count; i++)
                    if ((mainForm.checkedListBoxGames.Items[i] as NesGame).Name.
                            ToLower().StartsWith(textBoxSearch.Text.ToLower()))
                    {
                        mainForm.checkedListBoxGames.SelectedIndex = i;
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
