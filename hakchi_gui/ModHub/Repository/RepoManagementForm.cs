using com.clusterrr.hakchi_gui.ModHub.Repository;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.ModHub
{
    public partial class RepoManagementForm : Form
    {
        public RepoManagementForm()
        {
            InitializeComponent();
            foreach (RepositoryInfo repo in ConfigIni.Instance.repos)
            {
                ListViewItem item = new ListViewItem(repo.Name);
                item.SubItems.Add(repo.URL);
                item.Tag = repo;
                repoList.Items.Add(item);
            }
        }

        private void RepoManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (addRepoButton.Enabled && MessageBox.Show(Resources.UnsavedChangesQ, Resources.UnsavedChanges, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                btnAddRepository_Click(addRepoButton, null);
            }

            ConfigIni.Instance.repos = repoList.Items.Cast<ListViewItem>().Select(i => (RepositoryInfo)(i.Tag)).ToArray();
            ConfigIni.Save();
            MainForm.StaticRef.populateRepos();
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            MoveListViewItems(repoList, MoveDirection.Up);
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            MoveListViewItems(repoList, MoveDirection.Down);
        }
        
        private void editButton_Click(object sender, EventArgs e)
        {
            if (repoList.SelectedItems.Count > 0)
            {
                RepositoryInfo tag = (RepositoryInfo)(repoList.SelectedItems[0].Tag);
                repoName.Text = tag.Name;
                repoURL.Text = tag.URL;
                repoList.Items.Remove(repoList.SelectedItems[0]);
            } 
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var selected = repoList.SelectedItems;
            foreach (ListViewItem item in selected)
            {
                repoList.Items.Remove(item);
            }
        }

        private void repoList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            bool itemSelected = repoList.SelectedItems.Count > 0;

            moveUpButton.Enabled = itemSelected;
            moveDownButton.Enabled = itemSelected;
            deleteButton.Enabled = itemSelected;
            editButton.Enabled = itemSelected;
        }

        private void btnAddRepository_Click(object sender, EventArgs e)
        {
            var item = new ListViewItem(repoName.Text);
            item.SubItems.Add(repoURL.Text);
            item.Tag = new RepositoryInfo(repoName.Text, repoURL.Text);
            repoList.Items.Add(item);
            repoName.Text = null;
            repoURL.Text = null;
        }

        private enum MoveDirection { Up = -1, Down = 1 };

        private static void MoveListViewItems(ListView sender, MoveDirection direction)
        {
            int dir = (int)direction;
            int opp = dir * -1;

            bool valid = sender.SelectedItems.Count > 0 &&
                            ((direction == MoveDirection.Down && (sender.SelectedItems[sender.SelectedItems.Count - 1].Index < sender.Items.Count - 1))
                        || (direction == MoveDirection.Up && (sender.SelectedItems[0].Index > 0)));

            if (valid)
            {
                foreach (ListViewItem item in sender.SelectedItems)
                {
                    int index = item.Index + dir;
                    sender.Items.RemoveAt(item.Index);
                    sender.Items.Insert(index, item);
                }
            }
        }

        private void repoAddValidation(object sender, EventArgs e)
        {
            Uri result;
            addRepoButton.Enabled = (repoName.Text.Trim().Length > 0 && Uri.TryCreate(repoURL.Text, UriKind.Absolute, out result));
        }
    }
}
