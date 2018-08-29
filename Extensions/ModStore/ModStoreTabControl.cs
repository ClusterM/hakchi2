using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using com.clusterrr.hakchi_gui.module_library;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModStoreTabControl : UserControl
    {
        [Category("Data")]
        public string Category { get; set; }
        public ModStoreManager Manager { set { manager = value; loadModuleList(); } }

        private ModStoreItem currentItem { get; set; }
        private ModStoreManager manager;

        public ModStoreTabControl()
        {
            InitializeComponent();
        }

        #region GUI
        private void loadModuleDescription()
        {
            Cursor.Current = Cursors.WaitCursor;
            var installedModule = manager.GetInstalledModule(currentItem);
            webBrowser1.Navigate(new Uri(currentItem.Description, UriKind.Absolute));
            modInfo.SetInfo(currentItem.Name, currentItem.Author, currentItem.Version, (installedModule != null ? installedModule.Version : "N/A"));

            if (installedModule != null)
            {
                if (installedModule.Version != currentItem.Version)
                {
                    moduleDownloadButton.Enabled = true;
                    moduleDownloadButton.Text = "Update " + currentItem.Type;
                    moduleDownloadInstallButton.Text = "Update and Install" + currentItem.Type;
                }
                else
                {
                    moduleDownloadButton.Enabled = false;
                    moduleDownloadButton.Text = currentItem.Type + " Up-To-Date";
                    moduleDownloadInstallButton.Text = "Install " + currentItem.Type;
                }
            }
            else
            {
                moduleDownloadButton.Enabled = true;
                moduleDownloadButton.Text = "Download " + currentItem.Type;
                moduleDownloadInstallButton.Text = "Download and Install " + currentItem.Type;
            }
            moduleDownloadInstallButton.Enabled = true;
        }

        private void moduleListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (moduleListView.SelectedItems.Count != 0 && currentItem != moduleListView.SelectedItems[0].Tag)
            {
                currentItem = (ModStoreItem)moduleListView.SelectedItems[0].Tag;
                loadModuleDescription();
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();
            if (url == "about:blank" || url == currentItem.Description || url.StartsWith("res:"))
                return;

            // cancel the current event
            e.Cancel = true;

            // this opens the URL in the user's default browser
            System.Diagnostics.Process.Start(url);
        }
        #endregion

        #region Mod Manager Code
        private void loadModuleList()
        {
            currentItem = null;
            moduleListView.Groups.Clear();
            moduleListView.Items.Clear();
            moduleListView.Sorting = (manager.SortAlphabetically) ? SortOrder.Ascending : SortOrder.None;
            switch (Category)
            {
                case "game":
                    foreach (var item in manager.AvailableItems)
                    {
                        if (item is ModStoreGame)
                        {
                            var i = new ListViewItem(item.Name);
                            i.Tag = item;
                            moduleListView.Items.Add(i);
                        }
                    }
                    moduleDownloadInstallButton.Enabled = false;
                    moduleDownloadInstallButton.Visible = false;
                    break;
                case "retroarch_cores":
                    //Store System Groups
                    var systems = new Dictionary<string, ListViewGroup>();

                    //Create list item for each core and assign core to system group
                    foreach (var item in manager.AvailableItems)
                    {
                        if (item is RACoreModule)
                        {
                            var i = new ListViewItem(item.Name);
                            i.Tag = item;

                            //Create system group, if it doesn't exist
                            string System = ((RACoreModule)item).System;
                            if (!systems.ContainsKey(System))
                                systems[System] = new ListViewGroup(System);

                            i.Group = systems[System];
                        }
                    }

                    //Sort System Groups by name
                    var systemNames = new string[systems.Count];
                    systems.Keys.CopyTo(systemNames, 0);
                    Array.Sort(systemNames);

                    //Add each group and its items to the listview
                    foreach (var key in systemNames)
                    {
                        var group = systems[key];
                        if (manager.SortAlphabetically && group.Items.Count > 1)
                        {
                            //Sort items as array then add back to group
                            var items = new ListViewItem[group.Items.Count];
                            group.Items.CopyTo(items, 0);
                            group.Items.Clear();
                            Array.Sort(items, (ListViewItem a, ListViewItem b) => { return a.Text.CompareTo(b.Text); });
                            group.Items.AddRange(items);
                        }
                        moduleListView.Items.AddRange(group.Items);
                        moduleListView.Groups.Add(group);
                    }
                    break;
                default:
                    foreach (var item in manager.AvailableItems)
                    {
                        if (item is Module && (item as Module).Categories.Contains(Category))
                        {
                            var i = new ListViewItem(item.Name);
                            i.Tag = item;
                            moduleListView.Items.Add(i);
                        }
                    }
                    break;
            }

            if (moduleListView.Groups.Count != 0 && moduleListView.Items.Count != 0)
                moduleListView.Groups[0].Items[0].Selected = true;
            else if (moduleListView.Items.Count != 0)
                moduleListView.Items[0].Selected = true;
            moduleListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void moduleDownloadButton_Click(object sender, EventArgs e)
        {
            manager.DownloadItem(currentItem);
            loadModuleDescription();
        }

        private void moduleDownloadInstallButton_Click(object sender, EventArgs e)
        {
            InstalledModItem installedModule = manager.GetInstalledModule(currentItem);

            //Download or update module
            if (installedModule == null || installedModule.Version != currentItem.Version)
            {
                moduleDownloadButton_Click(this, new EventArgs());
                installedModule = manager.GetInstalledModule(currentItem);
            }

            if (installedModule != null)
            {
                List<string> mods = new List<string>();
                foreach (var file in installedModule.InstalledFiles)
                {
                    if (file.EndsWith(".hmod"))
                    {
                        mods.Add(file.Substring(0, file.Length - 5));
                    }
                    else if (file.EndsWith(".hmod\\"))
                    {
                        mods.Add(file.Substring(0, file.Length - 6));
                    }
                }
                MainForm mainForm = Application.OpenForms[0] as MainForm;
                mainForm.InstallMods(mods.ToArray());
            }
        }
        #endregion
    }
}
