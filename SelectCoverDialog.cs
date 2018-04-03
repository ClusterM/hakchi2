using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectCoverDialog : Form
    {
        public List<NesApplication> Games = new List<NesApplication>();
        private Dictionary<string, int> imageIndexes = new Dictionary<string, int>();

        private void fillGames()
        {
            listViewGames.Items.Clear();
            foreach (var game in Games)
            {
                var core = CoreCollection.GetCore(game.Metadata.Core);
                var filename = Path.GetFileName(game.GameFilePath) ?? string.Empty;
                if (game.IsOriginalGame || !string.IsNullOrEmpty(filename))
                {
                    if (filename.EndsWith(".7z"))
                        filename = filename.Substring(0, filename.Length - 3);
                    if (filename.EndsWith(".zip"))
                        filename = filename.Substring(0, filename.Length - 4);
                    var item = new ListViewItem(new string[] {
                        game.Name,
                        Path.GetFileNameWithoutExtension(filename),
                        Path.GetExtension(filename),
                        game.Metadata.System,
                        Resources.DefaultNoChange });
                    item.Tag = game;
                    listViewGames.Items.Add(item);
                }
            }
            coverColumnHeader.Width = -2;
        }

        public SelectCoverDialog()
        {
            InitializeComponent();
            Icon = Resources.icon;
            DialogResult = DialogResult.Abort;
        }

        private void SelectCoverDialog_Shown(object sender, EventArgs e)
        {
            fillGames();
            ShowSelected();
        }

        private void listViewGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }

        private void ShowSelected()
        {
            var items = listViewGames.SelectedItems;
            if (items.Count != 1)
            {
                listViewImages.Items.Clear();
                listViewImages.Enabled = false;
            }
            else
            {
                listViewImages.BeginUpdate();
                listViewImages.Items.Clear();

                NesApplication game = items[0].Tag as NesApplication;
                int i = imageList.Images.Count;

                if (imageIndexes.ContainsKey(game.Metadata.AppInfo.Name))
                {
                    var item = new ListViewItem(Resources.DefaultNoChange);
                    item.ImageIndex = imageIndexes[game.Metadata.AppInfo.Name];
                    listViewImages.Items.Add(item);
                }
                else
                {
                    var image = Shared.ResizeImage(game.Metadata.AppInfo.DefaultCover, null, listViewImages.BackColor, 114, 102, false, true, true, true);
                    imageList.Images.Add(image);

                    var item = new ListViewItem(Resources.DefaultNoChange);
                    imageIndexes.Add(game.Metadata.AppInfo.Name, item.ImageIndex = i++);
                    listViewImages.Items.Add(item);
                }

                foreach (var match in game.CoverArtMatches)
                {
                    var item = new ListViewItem(Path.GetFileName(match));
                    if (imageIndexes.ContainsKey(match))
                    {
                        item.ImageIndex = imageIndexes[match];
                    }
                    else
                    {
                        var image = Shared.ResizeImage(Image.FromFile(match), null, null, 114, 102, false, true, true, true);
                        imageList.Images.Add(image);
                        imageIndexes.Add(match, item.ImageIndex = i++);
                    }
                    listViewImages.Items.Add(item);
                }

                for (int j = 0; j < listViewImages.Items.Count; ++j)
                {
                    if (listViewImages.Items[j].Text == items[0].SubItems[4].Text)
                    {
                        listViewImages.Items[j].Selected = true;
                        break;
                    }
                }

                listViewImages.Enabled = true;
                listViewImages.EndUpdate();
            }
        }

        private void listViewImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var imageItem = listViewImages.SelectedItems.Count == 1 ? listViewImages.SelectedItems[0] : null;
            if (imageItem != null)
            {
                var gameItem = listViewGames.SelectedItems.Count == 1 ? listViewGames.SelectedItems[0] : null;
                if (gameItem != null)
                {
                    gameItem.SubItems[4].Text = imageItem.Text;
                }
            }
            coverColumnHeader.Width = -2;
        }

        private void buttonSetDefault_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewGames.Items.Count; ++i)
            {
                listViewGames.Items[i].SubItems[4].Text = Resources.DefaultNoChange;
            }
            coverColumnHeader.Width = -2;
            ShowSelected();
        }

        private void buttonImFeelingLucky_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewGames.Items.Count; ++i)
            {
                var gameItem = listViewGames.Items[i];
                var game = gameItem.Tag as NesApplication;
                var coverMatch = game.CoverArtMatches.First();
                gameItem.SubItems[4].Text = Path.GetFileName(coverMatch);
            }
            coverColumnHeader.Width = -2;
            ShowSelected();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar(tasker));
                tasker.AttachView(new Tasks.TaskerForm(tasker));
                var task = new Tasks.GameTask();
                for (int i = 0; i < listViewGames.Items.Count; ++i)
                {
                    var gameItem = listViewGames.Items[i];
                    if (gameItem.SubItems[4].Text != Resources.DefaultNoChange)
                    {
                        var game = gameItem.Tag as NesApplication;
                        foreach (var coverMatch in game.CoverArtMatches)
                        {
                            if(Path.GetFileName(coverMatch) == gameItem.SubItems[4].Text)
                            {
                                task.GamesChanged[game] = coverMatch;
                                break;
                            }
                        }
                    }
                }
                tasker.AddTask(task.SetCoverArtForMultipleGames);
                var conclusion = tasker.Start();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectCoverDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                bool change = false;
                for (int i = 0; i < listViewGames.Items.Count; ++i)
                    if (listViewGames.Items[i].SubItems[4].Text != Resources.DefaultNoChange)
                    {
                        change = true;
                        break;
                    }
                if (change &&
                    Tasks.MessageForm.Show(Resources.DiscardChanges, Resources.DiscardChangesQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button2) == Tasks.MessageForm.Button.No)
                    //MessageBox.Show(Resources.DiscardChangesQ, Resources.DiscardChanges, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    e.Cancel = true;
            }
        }
        
        private ColumnHeader SortingColumn = null;
        private void listViewGames_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = listViewGames.Columns[e.Column];
            SortOrder sort_order;
            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.EndsWith(" \u25BC"))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(0, SortingColumn.Text.Length - 2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = SortingColumn.Text + " \u25BC";
            }
            else
            {
                SortingColumn.Text = SortingColumn.Text + " \u25B2";
            }

            // Create a comparer.
            listViewGames.ListViewItemSorter =
                new ListViewItemComparer(e.Column, sort_order);

            // Sort.
            listViewGames.Sort();
        }

        // Compares two ListView items based on a selected column.
        public class ListViewItemComparer : IComparer
        {
            private int ColumnNumber;
            private SortOrder SortOrder;

            public ListViewItemComparer(int column_number, SortOrder sort_order)
            {
                ColumnNumber = column_number;
                SortOrder = sort_order;
            }

            // Compare two ListViewItems.
            public int Compare(object object_x, object object_y)
            {
                // Get the objects as ListViewItems.
                ListViewItem item_x = object_x as ListViewItem;
                ListViewItem item_y = object_y as ListViewItem;

                // Get the corresponding sub-item values.
                string string_x;
                if (item_x.SubItems.Count <= ColumnNumber)
                {
                    string_x = "";
                }
                else
                {
                    string_x = item_x.SubItems[ColumnNumber].Text;
                }

                string string_y;
                if (item_y.SubItems.Count <= ColumnNumber)
                {
                    string_y = "";
                }
                else
                {
                    string_y = item_y.SubItems[ColumnNumber].Text;
                }

                // Compare them.
                int result;
                result = string_x.CompareTo(string_y);

                // Return the correct result depending on whether
                // we're sorting ascending or descending.
                if (SortOrder == SortOrder.Ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }
        }
    }
}
