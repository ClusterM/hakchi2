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
        private List<ModStoreItem> itemList = new List<ModStoreItem>();

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
            moduleDescriptionBrowser.DocumentText = String.Format("<html style='background-color:#d20014;color:#ffffff;'>" +
                                                                    "<body background='https://hakchiresources.com/wp-content/uploads/2018/04/bg-1.png' style='width:273px;'>" +
                                                                         "<span style='font-family: Arial, Helvetica, sans-serif;'>" +
                                                                              "<b>Module Name:</b><br /><span style='font-size:75%;'>{0}</span><br />" +
                                                                              "<b>Author:</b><br /><span style='font-size:75%;'>{1}</span><br />" +
                                                                              "<b>Latest Version:</b><br /><span style='font-size:75%;'>{2}</span><br />" +
                                                                              "<b>Installed Version:</b><br /><span style='font-size:75%;'>{3}</span>" +
                                                                          "</span>" +
                                                                    "</body>" +
                                                                  "</html>",
                                                                  currentItem.Name,
                                                                  currentItem.Author,
                                                                  currentItem.Version,
                                                                  (installedModule != null) ? installedModule.Version : "N/A");

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
        }

        private void moduleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = moduleListBox.SelectedIndex;
            if (index != -1)
            {
                if (currentItem != itemList[index])
                {
                    currentItem = itemList[index];
                    loadModuleDescription();
                }
            }
            else
                currentItem = null;
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
            moduleListBox.Items.Clear();
            itemList.Clear();
            switch (Category)
            {
                case "game":
                    foreach (var item in manager.AvailableItems)
                    {
                        if (item is ModStoreGame)
                        {
                            itemList.Add(item);
                        }
                    }
                    moduleDownloadInstallButton.Enabled = false;
                    moduleDownloadInstallButton.Visible = false;
                    moduleDescriptionBrowser.Size = new System.Drawing.Size(moduleDescriptionBrowser.Size.Width, moduleDescriptionBrowser.Size.Height + 14);
                    break;
                default:
                    foreach (var item in manager.AvailableItems)
                    {
                        if (item is Module && (item as Module).Categories.Contains(Category))
                        {
                            itemList.Add(item);
                        }
                    }
                    break;
            }
            if (manager.SortAlphabetically)
                itemList.Sort((ModStoreItem a, ModStoreItem b) => { return a.Name.CompareTo(b.Name); });
            foreach (var item in itemList)
                moduleListBox.Items.Add(item.Name);
            moduleListBox.SelectedIndex = 0;
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
