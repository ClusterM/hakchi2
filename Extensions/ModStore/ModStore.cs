using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Threading;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.module_library;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModStore : Form
    {
        private ModStoreManager config = new ModStoreManager();
        private Module currentModule { get; set; }
        private bool showExperimentalTab = false;

        #region Form Initialisation
        public ModStore()
        {
            config.parentForm = this;
            InitializeComponent();
            this.Icon = Resources.icon;
            PoweredByLinkS.Text = "Powered By HakchiResources.com";
        }

        private void ModStore_Load(object sender, EventArgs e)
        {
            ModStore_Initialise();
        }

        private void ModStore_Initialise(bool LoadXML = true)
        {
            if (LoadXML)
            {
                //Load Config
                XmlSerializer xs = new XmlSerializer(typeof(ModStoreManager));
                if (File.Exists(config.ConfigPath))
                {
                    using (var fs = File.Open(config.ConfigPath, FileMode.Open))
                    {
                        config = (ModStoreManager)xs.Deserialize(fs);
                        config.parentForm = this;
                    }
                }
            }

            //Set Sort Menu Check
            setSortMenuItemCheck();

            if (config.AvailableItems.Count == 0 || (DateTime.Now - config.LastUpdate).TotalDays >= 1.0)
            {
                //Ask user to update repository information
                updateModuleList();
            }

            //If no modules, update failed so close mod store
            if (config.AvailableItems.Count == 0)
            {
                Close();
                return;
            }

            //Check if user deleted module
            config.CheckForDeletedItems();

            //Add or Remove Experimental Tab
            if (showExperimentalTab == false)
                tabControl1.TabPages.Remove(tabPage7);
            else if (tabControl1.TabPages.Contains(tabPage7) == false)
                tabControl1.TabPages.Add(tabPage7);

            //Setup Tabs and Load Items
            loadTabItems();
        }
        #endregion

        #region Non Essential GUI code
        private void refreshContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateModuleList();
            ModStore_Initialise(false);
            Tasks.MessageForm.Show(this, this.Text, "Refreshed Mod Store");
        }

        private void PoweredByLinkS_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }

        private void submitYourOwnModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://hakchiresources.com/submit-mod/");
        }

        private void discordLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/ETe3ecx");
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }
		
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var modStoreAbout = new ModStoreAbout();
            modStoreAbout.ShowDialog();
        }

        private void sortMethodClick(object sender, EventArgs e)
        {
            //Check if method is active
            if (((ToolStripMenuItem)sender).Checked)
                return;

            //Set Sort Method
            config.SortAlphabetically = sender == sortByAZToolStripMenuItem;
            setSortMenuItemCheck();
            //Reload all tabs
            loadTabItems();
        }

        private void setSortMenuItemCheck()
        {
            sortByAZToolStripMenuItem.Checked = config.SortAlphabetically;
            sortByDateToolStripMenuItem.Checked = !config.SortAlphabetically;
        }

        private void showExperimentalModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.ShowExperimentalQuestion, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                showExperimentalTab = true;
                menuStrip.Items.Remove(showExperimentalModsToolStripMenuItem);
                ModStore_Initialise(false);
                tabControl1.SelectedTab = tabPage7;
            }
        }
        #endregion

        #region Main Mod Store Code
        private void loadTabItems()
        {
            //For each Mod Store Tab set ModStoreManager (forces tab item refresh)
            foreach (TabPage tabPage in tabControl1.Controls)
            {
                if (tabPage.Controls.Count == 0) continue;

                ModStoreTabControl modStoreTabControl = tabPage.Controls[0] as ModStoreTabControl;
                if (modStoreTabControl != null)
                    modStoreTabControl.Manager = config;
            }
        }

        private void updateModuleList()
        {
            try
            {
                JObject json;
                using (var wc = new WebClient())
                {
                    json = JObject.Parse(wc.DownloadString("https://hakchiresources.com/api/get_posts/?count=10000"));
                }
                config.AvailableItems.Clear();
                foreach (var post in json["posts"])
                {
                    string type = "Module";
                    foreach (var tag in post["tags"])
                    {
                        if (tag["slug"].ToString().Equals("non_hmod"))
                        {
                            type = "None";
                        }
                        else if (tag["slug"].ToString().Equals("game"))
                        {
                            type = "Game";
                            break;
                        }
                    }
                    if (type.Equals("None"))
                        continue;

                    try
                    {
                        //Grab Mod Store Item variables from JSON item
                        string Name = System.Web.HttpUtility.HtmlDecode(post["title"].ToString());
                        string Id = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()); //Temporary ID need to replace
                        string Author = post["custom_fields"]["user_submit_name"][0].ToString();
                        string Description = post["url"].ToString() + "?mode=mod_store";
                        string Content = post["content"].ToString();
                        string Version = post["custom_fields"]["usp_custom_field"][0].ToString();
                        string Path = post["custom_fields"]["user_submit_url"][0].ToString();
                        var Categories = new List<string>();
                        foreach (var category in post["categories"])
                        {
                            Categories.Add(category["slug"].ToString());
                        }

                        //Set module type for RA cores
                        if (Categories.Contains("retroarch_cores"))
                            type = "RACore";
                        
                        switch (type)
                        {
                            case "RACore":
                                string System = "unknown";
                                foreach (var tag in post["tags"])
                                {
                                    if (tag["slug"].ToString().StartsWith("ra_"))
                                        System = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tag["slug"].ToString().Substring(3).Replace('_', ' '));
                                }
                                var raModule = new RACoreModule { Name = Name, Id = Id, Author = Author, Content = Content, Description = Description, Version = Version, Path = Path, Categories = Categories, System = System };
                                if (raModule.SetModType())
                                    config.AvailableItems.Add(raModule);
                                break;

                            case "Module":
                                var module = new Module { Name = Name, Id = Id, Author = Author, Content = Content, Description = Description, Version = Version, Path = Path, Categories = Categories };
                                if (module.SetModType())
                                    config.AvailableItems.Add(module);
                                break;

                            case "Game":
                                config.AvailableItems.Add(new ModStoreGame { Name = Name, Id = Id, Author = Author, Content = Content, Description = Description, Version = Version, Path = Path });
                                break;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.LastUpdate = DateTime.Now;
            updateModStoreItems();
        }

        private void updateModStoreItems()
        {
            List<ModStoreItem> itemsToUpdate = new List<ModStoreItem>();
            //For each installed item find the matching repo entry
            foreach (var item in config.InstalledItems)
            {
                ModStoreItem storeItem = null;
                foreach (var rItem in config.AvailableItems)
                {
                    if (rItem.Id == item.Id)
                    {
                        storeItem = rItem;
                        break;
                    }
                }
                if (storeItem != null && storeItem.Version != item.Version)
                    itemsToUpdate.Add(storeItem);
            }
            if (itemsToUpdate.Count != 0)
            {
                var updateMsgBox = MessageBox.Show("Do you want to update all out of date mod store items?", "Update Items", MessageBoxButtons.YesNo);
                if (updateMsgBox == DialogResult.Yes)
                {
                    for (int i = 0; i < itemsToUpdate.Count; ++i)
                    {
                        config.DownloadItem(itemsToUpdate[i]);
                    }
                    MessageBox.Show(this, "Finished updating items.");
                }
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();
            if (url == "about:blank" || url == "https://hakchiresources.com/modstorewelcome/?mode=welcome" || url.StartsWith("res:"))
                return;

            // cancel the current event
                e.Cancel = true;

            // this opens the URL in the user's default browser
            Process.Start(url);
        }

        private void ModStore_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.SaveConfig();
        }

        #endregion
    }
}