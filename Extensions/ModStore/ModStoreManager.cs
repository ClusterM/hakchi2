using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using com.clusterrr.hakchi_gui.Properties;
using SharpCompress.Archives;

namespace com.clusterrr.hakchi_gui.module_library
{
    public class ModStoreManager
    {
        public List<ModStoreItem> AvailableItems = new List<ModStoreItem>();
        public List<InstalledModItem> InstalledItems = new List<InstalledModItem>();
        public DateTime LastUpdate = new DateTime();
        public bool SortAlphabetically = false;
        public string ConfigPath { get { return Path.Combine(Program.BaseDirectoryExternal, "config\\ModStoreConfig.xml"); } }

        public void CheckForDeletedItems()
        {
            string userModDir = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
            for (int i = 0; i < InstalledItems.Count; ++i)
            {
                var item = InstalledItems[i];
                bool removeItem = false;
                if (item.InstalledFiles.Count == 1 && !item.InstalledFiles[0].EndsWith("\\"))
                {
                    removeItem = File.Exists(Path.Combine(userModDir, item.InstalledFiles[0])) == false;
                }
                else
                {
                    foreach (var file in item.InstalledFiles)
                    {
                        if (file.EndsWith("\\") && !Directory.Exists(Path.Combine(userModDir, file)))
                        {
                            removeItem = true;
                            break;
                        }
                    }
                }
                if (removeItem)
                {
                    InstalledItems.RemoveAt(i);
                    --i;
                }
            }
        }

        public void DownloadItem(ModStoreItem item)
        {
            switch(item.Type)
            {
                case "Module":
                    DownloadModule(item as Module);
                    break;
                case "Game":
                    DownloadGame(item as ModStoreGame);
                    break;
            }
        }

        public void DownloadGame(ModStoreGame game)
        {
            try
            {
                var installedGame = GetInstalledModule(game);
                //If game is installed remove it
                if (installedGame != null)
                    InstalledItems.Remove(installedGame);

                string tempFileName = Path.GetTempPath() + game.Path.Substring(game.Path.LastIndexOf("/") + 1);
                if (!ProgressBarForm.DownloadFile(game.Path, tempFileName))
                    throw new Exception("Cannot download file: " + game.Path);

                MainForm mainForm = Application.OpenForms[0] as MainForm;
                mainForm.AddGames(new string[] { tempFileName });
                File.Delete(tempFileName);

                //InstalledItems.Add(game.CreateInstalledItem());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SaveConfig();
        }

        public void DownloadModule(Module module)
        {
            try
            {
                string userModDir = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
                var installedModule = GetInstalledModule(module);
                //If module is installed remove it
                if (installedModule != null)
                {
                    foreach(var file in installedModule.InstalledFiles)
                    {
                        try
                        {
                            if (file.EndsWith("\\"))
                                Directory.Delete(Path.Combine(userModDir, file), true);
                            else
                                File.Delete(Path.Combine(userModDir, file));
                        }
                        catch { }
                    }

                    InstalledItems.Remove(installedModule);
                    installedModule = null;
                }
                switch (module.ModType)
                {
                    case ModuleType.hmod:
                        {
                            string fileLocation = Path.Combine(userModDir, module.Path.Substring(module.Path.LastIndexOf('/') + 1));

                            if (!ProgressBarForm.DownloadFile(module.Path, fileLocation))
                                throw new Exception("Cannot download file: " + module.Path);
                            installedModule = module.CreateInstalledItem();
                            installedModule.InstalledFiles.Add(module.Path.Substring(module.Path.LastIndexOf('/') + 1));
                            InstalledItems.Add(installedModule);
                        }
                        break;
                    case ModuleType.compressedFile:
                        var tempFileName = Path.GetTempFileName();
                        if(!ProgressBarForm.DownloadFile(module.Path, tempFileName))
                            throw new Exception("Cannot download file: " + module.Path);
                        using (var extractor = ArchiveFactory.Open(tempFileName))
                        {
                            installedModule = module.CreateInstalledItem();
                            foreach (var file in extractor.Entries)
                            {
                                int index = file.Key.IndexOf('/');
                                if (index != -1)
                                {
                                    var folder = file.Key.Substring(0, index + 1);
                                    if (!installedModule.InstalledFiles.Contains(folder))
                                    {
                                        installedModule.InstalledFiles.Add(folder);
                                        var localFolder = Path.Combine(userModDir, folder);
                                        if (Directory.Exists(localFolder))
                                            Directory.Delete(localFolder, true);
                                    }
                                }
                                else if(!file.IsDirectory)
                                    installedModule.InstalledFiles.Add(file.Key);
                            }
                            extractor.WriteToDirectory(userModDir);
                            InstalledItems.Add(installedModule);
                        }
                        File.Delete(tempFileName);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SaveConfig();
        }

        public InstalledModItem GetInstalledModule(ModStoreItem repoModule)
        {
            foreach (var module in InstalledItems)
            {
                if (module.Id == repoModule.Id)
                    return module;
            }
            return null;
        }

        public void SaveConfig()
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            XmlSerializer x = new XmlSerializer(typeof(ModStoreManager));
            using (var fs = new FileStream(ConfigPath, FileMode.Create))
            {
                x.Serialize(fs, this);
            }
        }
    }
}
