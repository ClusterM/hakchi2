using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.Tasks;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Hmod
{
    public struct Hmod
    {
        public readonly string Name;
        public readonly string HmodPath;
        public readonly bool isFile;
        public readonly HmodReadme Readme;
        public readonly string RawName;
        public readonly string Category;
        public readonly string Creator;
        public readonly string Version;
        public readonly string EmulatedSystem;
        public readonly Dictionary<string, string> LibretroInfo;
        public readonly DateTime LastModified;
        public readonly bool isInstalled;
        public static string UserModsDirectory
        {
            get
            {
                return Path.Combine(Program.BaseDirectoryExternal, "user_mods");
            }
        }

        public Hmod(string mod, string[] installedHmods = null)
        {
            isInstalled = false;
            if (installedHmods != null)
            {
                isInstalled = installedHmods.Contains(mod);
            }
            RawName = mod;
            this.HmodPath = null;
            this.isFile = false;
            
            string usermodsDirectory = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
            string cacheDir = Shared.PathCombine(Program.BaseDirectoryExternal, "cache", "readme_cache");
            string cacheFile = Path.Combine(cacheDir, $"{mod}.xml");


            Dictionary<string, string> readmeData = new Dictionary<string, string>();
            Dictionary<string, string> libretroInfo = new Dictionary<string, string>();

            LastModified = DateTime.UtcNow;
            this.LibretroInfo = new Dictionary<string, string>();

            try
            {
                var dir = Path.Combine(usermodsDirectory, mod + ".hmod");
                if (Directory.Exists(dir))
                {
                    var files = (from f in (new DirectoryInfo(dir)).GetFiles("*", SearchOption.AllDirectories)
                                 orderby f.LastWriteTimeUtc descending
                                 select f.LastWriteTimeUtc);

                    if (files.Count() > 0)
                        LastModified = files.First();

                    isFile = false;
                    HmodPath = dir;
                    foreach (var f in HmodReadme.readmeFiles)
                    {
                        var fn = Path.Combine(dir, f);
                        if (File.Exists(fn))
                        {
                            readmeData.Add(f.ToLower(), File.ReadAllText(fn));
                        }
                    }

                    foreach (string file in Directory.EnumerateFiles(dir, "*_libretro.info"))
                    {
                        libretroInfo.Add(file, File.ReadAllText(file));
                    }
                    this.LibretroInfo = libretroInfo;
                }
                else if (File.Exists(dir))
                {
                    LastModified = new FileInfo(dir).LastWriteTimeUtc;
                    isFile = true;
                    HmodPath = dir;

                    MetadataCache cache;
                    FileInfo info = new FileInfo(dir);

                    bool skipExtraction = false;
                    if (File.Exists(cacheFile))
                    {
                        try
                        {
                            cache = MetadataCache.Deserialize(cacheFile);
                            if (cache.LastModified == info.LastWriteTimeUtc)
                            {
                                skipExtraction = true;
                                readmeData = cache.getReadmeDictionary();
                                foreach (string[] infoFile in cache.LibretroInfo)
                                {
                                    this.LibretroInfo.Add(infoFile[0], infoFile[1]);
                                }
                            }
                        }
                        catch { }
                    }


                    if (!skipExtraction)
                    {
                        using (var reader = ReaderFactory.Open(File.OpenRead(dir)))
                        {
                            while (reader.MoveToNextEntry())
                            {
                                foreach (var readmeFilename in HmodReadme.readmeFiles)
                                {
                                    if (reader.Entry.Key.ToLower() != readmeFilename && reader.Entry.Key.ToLower() != $"./{readmeFilename}")
                                        continue;

                                    using (var o = new MemoryStream())
                                    using (var e = reader.OpenEntryStream())
                                    {
                                        e.CopyTo(o);
                                        readmeData.Add(readmeFilename, Encoding.UTF8.GetString(o.ToArray()));
                                    }
                                }

                                if (reader.Entry.Key.ToLower().EndsWith("_libretro.info"))
                                {
                                    using (var o = new MemoryStream())
                                    using (var e = reader.OpenEntryStream())
                                    {
                                        e.CopyTo(o);
                                        libretroInfo.Add(reader.Entry.Key, Encoding.UTF8.GetString(o.ToArray()));
                                    }
                                }
                            }
                        }
                        cache = new MetadataCache(readmeData, libretroInfo, "", info.LastWriteTimeUtc);

                        if (!Directory.Exists(cacheDir))
                            Directory.CreateDirectory(cacheDir);

                        this.LibretroInfo = libretroInfo;

                        File.WriteAllText(cacheFile, cache.Serialize());
                    }
                }
                else
                {
                    if (File.Exists(cacheFile))
                    {
                        try
                        {
                            MetadataCache cache;
                            cache = MetadataCache.Deserialize(cacheFile);
                            readmeData = cache.getReadmeDictionary();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception e)
            {
            }

            string readme;
            bool markdown = false;
            if (readmeData.TryGetValue("readme.md", out readme))
            {
                markdown = true;
            }
            else if (readmeData.TryGetValue("readme.txt", out readme)) { }
            else if (readmeData.TryGetValue("readme", out readme)) { }
            else
            {
                readme = "";
            }

            this.Readme = new HmodReadme(readme, markdown);

            if (!this.Readme.frontMatter.TryGetValue("Name", out this.Name))
            {
                this.Name = mod;
            }
            if (!this.Readme.frontMatter.TryGetValue("Category", out this.Category))
            {
                this.Category = Properties.Resources.Unknown;
            }

            if (!this.Readme.frontMatter.TryGetValue("Version", out this.Version))
            {
                this.Version = null;
            }

            if (!this.Readme.frontMatter.TryGetValue("Creator", out this.Creator))
            {
                this.Creator = Properties.Resources.Unknown;
            }

            if (!this.Readme.frontMatter.TryGetValue("Emulated System", out this.EmulatedSystem))
            {
                this.EmulatedSystem = Properties.Resources.Unknown;
            }
        }

        public bool PresentInUserMods()
        {
            return File.Exists(this.HmodPath) || Directory.Exists(this.HmodPath);
        }

        public static List<Hmod> GetMods(bool onlyInstalled = false, string[] installed = null, Form taskerParent = null)
        {
            var usermodsDirectory = UserModsDirectory;
            var installedMods = installed  ?? hakchi.GetPackList() ?? new string[] { };
            var modsList = new List<string>();

            if (onlyInstalled)
            {
                modsList.AddRange(installedMods);
            }
            else
            {
                if (Directory.Exists(usermodsDirectory))
                {
                    modsList.AddRange(from m
                                      in Directory.GetDirectories(usermodsDirectory, "*.hmod", SearchOption.TopDirectoryOnly)
                                      select Path.GetFileNameWithoutExtension(m));
                    modsList.AddRange(from m
                                      in Directory.GetFiles(usermodsDirectory, "*.hmod", SearchOption.TopDirectoryOnly)
                                      select Path.GetFileNameWithoutExtension(m));
                }
            }

            using (Tasker tasker = new Tasker(taskerParent))
            {
                tasker.AttachView(new Tasks.TaskerForm());
                var modObject = new ModTasks.ModObject();
                modObject.HmodsToLoad = modsList;
                modObject.InstalledHmods = installedMods ?? new string[] { };
                tasker.SetTitle(Resources.LoadingHmods);
                tasker.SetStatusImage(Resources.sign_brick);
                tasker.SyncObject = modObject;
                tasker.AddTask(ModTasks.GetHmods);
                tasker.Start();
                return modObject.LoadedHmods;
            }
        }
    }
}
