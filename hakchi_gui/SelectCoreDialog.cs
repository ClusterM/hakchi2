using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectCoreDialog : Form
    {
        public List<NesApplication> Games = new List<NesApplication>();
        public bool Modified
        {
            get; private set;
        }

        private void fillGames()
        {
            listViewGames.Items.Clear();
            foreach(var game in Games)
            {
                var core = CoreCollection.GetCore(game.Metadata.Core);
                var filename = Path.GetFileName(game.GameFilePath);

                if (string.IsNullOrEmpty(filename) && game.Desktop.Args.Length > 0)
                {
                    filename = Path.GetFileName(game.Desktop.Args[0]);
                }

                if (!string.IsNullOrEmpty(filename))
                {
                    if (filename.EndsWith(".7z"))
                        filename = filename.Substring(0, filename.Length - 3);
                    if (filename.EndsWith(".zip"))
                        filename = filename.Substring(0, filename.Length - 4);
                    var item = new ListViewItem(new string[] { game.Name, Path.GetExtension(filename), game.Metadata.System, core == null ? string.Empty : core.Name });
                    item.Tag = game;
                    listViewGames.Items.Add(item);
                }
            }
        }

        private List<string> systemsCollection = null;
        private void fillSystems()
        {
            if (systemsCollection == null)
            {
                systemsCollection = CoreCollection.Systems.ToList();
                foreach(var appInfo in AppTypeCollection.Apps)
                {
                    if (!systemsCollection.Contains(appInfo.Name))
                        systemsCollection.Add(appInfo.Name);
                }
                systemsCollection.Sort();
            }

            listBoxSystem.BeginUpdate();
            listBoxSystem.Items.Clear();
            listBoxSystem.Items.Add(Resources.Unassigned);
            var collection = showAllSystemsCheckBox.Checked || firstSelected == null ? (IEnumerable<string>)systemsCollection : CoreCollection.GetSystemsFromExtension(firstSelected.SubItems[1].Text.ToLower()).ToArray();
            if (collection.Any())
            {
                foreach (var system in collection.OrderBy(s => s))
                    listBoxSystem.Items.Add(system);
            }
            else
            {
                var appInfo = AppTypeCollection.GetAppByExtension(firstSelected.SubItems[1].Text.ToLower());
                if (!appInfo.Unknown)
                {
                    listBoxSystem.Items.Add(appInfo.Name);
                }
            }
            listBoxSystem.Enabled = true;
            listBoxSystem.EndUpdate();
        }

        private void fillCores(string system)
        {
            listBoxCore.BeginUpdate();
            listBoxCore.Items.Clear();
            listBoxCore.Items.Add(Resources.Unassigned);
            var collection = string.IsNullOrEmpty(system) ? CoreCollection.Cores : CoreCollection.GetCoresFromSystem(system);
            if (collection != null)
            {
                foreach (var core in collection.OrderBy(c => c.Name))
                {
                    listBoxCore.Items.Add(core);
                }
            }
            listBoxCore.Enabled = true;
            listBoxCore.EndUpdate();
        }

        public SelectCoreDialog()
        {
            InitializeComponent();
            CoreCollection.HmodInfo = Hmod.Hmod.GetMods(false, new string[] { }).ToArray();
            CoreCollection.Load();
            Icon = Resources.icon;
            Modified = false;
            DialogResult = DialogResult.Abort;
        }

        private void SelectCoreDialog_Shown(object sender, EventArgs e)
        {
            fillGames();
            ShowSelected();
        }

        private void listBoxSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxCore.ClearSelected();
            listBoxCore.Items.Clear();

            var system = (string)listBoxSystem.SelectedItem;
            if (system != null)
            {
                fillCores(system == Resources.Unassigned ? string.Empty : system);

                if (firstSelected != null)
                {
                    var game = firstSelected.Tag as NesApplication;
                    var core = string.IsNullOrEmpty(game.Metadata.Core) ? AppTypeCollection.GetAppBySystem(system).DefaultCore : game.Metadata.Core;
                    for (int i = 0; i < listBoxCore.Items.Count; ++i)
                    {
                        if (!(listBoxCore.Items[i] is CoreCollection.CoreInfo))
                        {
                            listBoxCore.SetSelected(i, true);
                        }
                        else if ((listBoxCore.Items[i] as CoreCollection.CoreInfo).Bin == core)
                        {
                            listBoxCore.SetSelected(i, true);
                            break;
                        }
                    }
                }
            }
        }

        private void listBoxCore_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCommand();
            if (listBoxCore.SelectedItem == null)
            {
                buttonApply.Enabled = false;
                resetCheckBox.Enabled = false;
            }
            else
            {
                buttonApply.Enabled = true;
                resetCheckBox.Enabled = true;
            }
        }

        private ListViewItem firstSelected = null;
        private void ShowSelected()
        {
            var items = listViewGames.SelectedItems;
            if(items.Count == 0)
            {
                listBoxSystem.ClearSelected();
                commandTextBox.Text = string.Empty;
                listBoxSystem.Enabled = false;
                listBoxCore.Enabled = false;
                commandTextBox.Enabled = false;
                buttonApply.Enabled = false;
                resetCheckBox.Enabled = false;
                firstSelected = null;
            }
            else
            {
                if(items.Count == 1)
                {
                    var item = firstSelected = items[0];
                    var game = item.Tag as NesApplication;
                    fillSystems();
                    if (!string.IsNullOrEmpty(game.Metadata.System))
                    {
                        listBoxSystem.ClearSelected();
                        bool foundSystem = false;
                        for (int i = 0; i < listBoxSystem.Items.Count; ++i)
                        {
                            if (listBoxSystem.Items[i].ToString() == game.Metadata.System)
                            {
                                listBoxSystem.SetSelected(i, true);
                                foundSystem = true;
                                break;
                            }
                        }
                        if (!foundSystem)
                            listBoxSystem.SetSelected(0, true);
                    }
                    if (!string.IsNullOrEmpty(game.Metadata.Core))
                    {
                        for(int i = 0; i < listBoxCore.Items.Count; ++i)
                        {
                            if (listBoxCore.Items[i].ToString() == game.Metadata.Core)
                            {
                                listBoxCore.SetSelected(i, true);
                                break;
                            }
                        }
                    }
                    buttonApply.Enabled = false;

                }
                else
                {
                    ShowCommand();
                }
            }
        }

        private void ShowCommand()
        {
            var items = listViewGames.SelectedItems;
            if(items.Count == 0 || listBoxCore.SelectedItem == null)
            {
                commandTextBox.Text = string.Empty;
                commandTextBox.Enabled = false;
                return;
            }

            var core = listBoxCore.SelectedItem as CoreCollection.CoreInfo;
            if (core == null)
            {
                if (items.Count == 1)
                {
                    var item = items[0];
                    var game = item.Tag as NesApplication;

                    commandTextBox.Text = game.Desktop.Exec.Trim();
                    commandTextBox.Enabled = true;
                }
                else
                {
                    commandTextBox.Text = "";
                    commandTextBox.Enabled = false;
                }
            }
            else if (items.Count > 1)
            {
                string newCommand = core.QualifiedBin + " {rom}";
                if (resetCheckBox.Checked)
                {
                    newCommand += string.IsNullOrEmpty(core.DefaultArgs) ? string.Empty : (" " + core.DefaultArgs);
                }
                else
                {
                    newCommand += " {args}";
                }
                
                commandTextBox.Text = newCommand.Trim();
                commandTextBox.Enabled = true;
            }
            else if (items.Count == 1)
            {
                var item = items[0];
                var game = item.Tag as NesApplication;
                if (resetCheckBox.Checked)
                {
                    string newCommand = core.QualifiedBin;
                    if (game.Desktop.Args.Length > 0)
                        newCommand += " " + game.Desktop.Args[0];
                    newCommand += " " + core.DefaultArgs;
                    commandTextBox.Text = newCommand.Trim();
                }
                else
                {
                    DesktopFile desktop = (DesktopFile)game.Desktop.Clone();
                    desktop.Bin = core.QualifiedBin;
                    commandTextBox.Text = desktop.Exec.Trim();
                }
                commandTextBox.Enabled = true;
            }
            buttonApply.Enabled = true;
        }


        private void listViewGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            var core = listBoxCore.SelectedItem.GetType() == typeof(CoreCollection.CoreInfo) ? (CoreCollection.CoreInfo)listBoxCore.SelectedItem : null;
            string system = (string)listBoxSystem.SelectedItem;
            if (system == Resources.Unassigned)
                system = string.Empty;
            string newCommand = commandTextBox.Text;
            foreach(ListViewItem item in listViewGames.SelectedItems)
            {
                item.SubItems[2].Text = system;
                item.SubItems[3].Text = core == null ? string.Empty : core.Name;

                var game = item.Tag as NesApplication;
                game.Metadata.System = system;
                game.Metadata.Core = core == null ? string.Empty : core.Bin;
                game.Desktop.Exec = newCommand.Replace("{rom}", game.Desktop.Args[0]).Replace("{args}", string.Join(" ", game.Desktop.Args.Skip(1).ToArray())).Trim();
                game.Save();
                game.SaveMetadata();
            }
            buttonApply.Enabled = false;
            Modified = true;
        }

        private void resetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ShowCommand();
        }

        private void commandTextBox_Enter(object sender, EventArgs e)
        {
            buttonApply.Enabled = true;
        }

        private void showAllSystemsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count > 1)
            {
                try
                {
                    listViewGames.BeginUpdate();
                    listViewGames.SelectedItems.Cast<ListViewItem>().Skip(1).ToList().ForEach(i => i.Selected = false);
                }
                finally
                {
                    listViewGames.EndUpdate();
                }
            }
            ShowSelected();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectCoreDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (buttonApply.Enabled)
            {
                var result = Tasks.MessageForm.Show(this, Resources.ApplyChanges, Resources.ApplyChangesQ, Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No, Tasks.MessageForm.Button.Cancel }, Tasks.MessageForm.DefaultButton.Button1);
                switch (result)
                {
                    case Tasks.MessageForm.Button.Yes:
                        buttonApply_Click(sender, e);
                        break;
                    case Tasks.MessageForm.Button.No:
                        break;
                    case Tasks.MessageForm.Button.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            DialogResult = DialogResult.OK;
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
