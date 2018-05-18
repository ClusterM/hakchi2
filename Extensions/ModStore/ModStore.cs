using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Net;
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

        #region Form Initialisation
        public ModStore()
        {
            InitializeComponent();
            this.Icon = Resources.icon;
            PoweredByLinkS.Text = "Powered By HakchiResources.com";
        }

        private void ModStore_Load(object sender, EventArgs e)
        {
            ModStore_Initialise();
        }

        private void ModStore_Initialise()
        {
            //Load Config
            XmlSerializer xs = new XmlSerializer(typeof(ModStoreManager));
            if (File.Exists(config.ConfigPath))
            {
                using (var fs = File.Open(config.ConfigPath, FileMode.Open))
                {
                    config = (ModStoreManager)xs.Deserialize(fs);
                }
            }

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

            //Set Control Config
            foreach (TabPage tabPage in tabControl1.Controls)
            {
                ModStoreTabControl modStoreTabControl = tabPage.Controls[0] as ModStoreTabControl;
                if (modStoreTabControl != null)
                    modStoreTabControl.Manager = config;
            }
        }
        #endregion

        #region Non Essential GUI code

        private void refreshContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(config.ConfigPath);
            }
            catch { }
            ModStore_Initialise();
            Tasks.MessageForm.Show(this, this.Text, "Refreshed Mod store");
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
        #endregion

        #region Main Mod Store Code
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
                        switch(type)
                        {
                            case "Module":
                                Module module = new Module
                                {
                                    Name = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()),
                                    Id = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()), //Temporary ID need to replace
                                    Author = post["custom_fields"]["user_submit_name"][0].ToString(),
                                    Description = post["url"].ToString() + "?mode=mod_store",
                                    Version = post["custom_fields"]["usp_custom_field"][0].ToString(),
                                    Path = post["custom_fields"]["user_submit_url"][0].ToString()
                                };

                                //Set Module Type
                                var extention = module.Path.Substring(module.Path.LastIndexOf('.') + 1).ToLower();
                                if (extention.Equals("hmod"))
                                    module.ModType = ModuleType.hmod;
                                else if (extention.Equals("zip") || extention.Equals("7z") || extention.Equals("rar"))
                                    module.ModType = ModuleType.compressedFile;
                                else
                                    continue; //Unknown File Type

                                //Set Categories
                                foreach (var category in post["categories"])
                                {
                                    module.Categories.Add(category["slug"].ToString());
                                }

                                config.AvailableItems.Add(module);
                                break;
                            
                            case "Game":
                                config.AvailableItems.Add(new ModStoreGame
                                {
                                    Name = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()),
                                    Id = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()), //Temporary ID need to replace
                                    Author = post["custom_fields"]["user_submit_name"][0].ToString(),
                                    Description = post["url"].ToString() + "?mode=mod_store",
                                    Version = post["custom_fields"]["usp_custom_field"][0].ToString(),
                                    Path = post["custom_fields"]["user_submit_url"][0].ToString()
                                });
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
            if (e.Url.ToString() == "about:blank") return;

            // cancel the current event
            e.Cancel = true;

            // this opens the URL in the user's default browser
            Process.Start(e.Url.ToString());
        }

        private void ModStore_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.SaveConfig();
        }

        #endregion
    }
}