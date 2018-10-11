using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.Tasks;
using com.clusterrr.util;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.ModHub.Repository
{
    public delegate void RepositoryProgressHandler(long current, long max);
    public delegate void RepositoryLoadedHandler(Repository.Item[] items);

    public static class ItemKindMethods
    {
        public static string GetFileExtension(this Repository.ItemKind kind)
        {
            return Repository.ItemKindFileExtensions[(int)kind];
        }
    }

    public class Repository
    {
        public event RepositoryProgressHandler RepositoryProgress;
        public event RepositoryLoadedHandler RepositoryLoaded;

        public string RepositoryURL { get; private set; }
        public string RepositoryPackURL
        {
            get
            {
                return RepositoryURL + "pack.tgz";
            }
        }

        public string RepositoryListURL
        {
            get
            {
                return RepositoryURL + "list";
            }
        }

        public List<Item> Items = new List<Item>();
        public string Readme { get; private set; } = null;

        public static string[] ItemKindFileExtensions = new string[] { null, ".hmod", ".clvg" };
        public enum ItemKind
        {
            Unknown,
            Hmod,
            Game
        }

        public static ItemKind ItemKindFromFilename(string filename)
        {
            string lowerFilename = filename.ToLower();

            foreach (ItemKind kind in Enum.GetValues(typeof(ItemKind)))
            {
                if (kind == ItemKind.Unknown) continue;
                if (lowerFilename.EndsWith(kind.GetFileExtension()))
                    return kind;
            }

            return ItemKind.Unknown;
        }
        
        public class Item
        {
            public string FileName { get; private set; }
            public string RawName
            {
                get
                {
                    if (FileName.EndsWith(".hmod") || FileName.EndsWith(".clvg"))
                        return FileName.Substring(0, FileName.Length - 5);

                    return FileName;
                }
            }
            public string Name { get; private set; }
            public string Category { get; private set; }
            public string Creator { get; private set; }
            public string Version { get; private set; }
            public string EmulatedSystem { get; private set; }
            public string URL { get; private set; }
            public string MD5 { get; private set; }
            public string SHA1 { get; private set; }
            public bool Extract { get; private set; }

            public ItemKind Kind { get; private set; }
            public HmodReadme Readme { get; private set; }

            public Item(string filename, string readme = null, bool markdownReadme = false)
            {
                FileName = filename;
                Kind = ItemKindFromFilename(FileName);
                Name = RawName;
                Category = null;
                Creator = null;
                Version = null;
                EmulatedSystem = null;
                URL = null;
                MD5 = null;
                SHA1 = null;
                Extract = false;
                Readme = new HmodReadme(readme ?? "", markdownReadme);
                setValues();
            }
            public void setURL(string url)
            {
                URL = url;
            }
            public void setMD5(string md5)
            {
                MD5 = md5;
            }
            public void setSHA1(string sha1)
            {
                SHA1 = sha1;
            }
            public void setExtract(bool extract)
            {
                Extract = extract;
            }
            public void setReadme(string readme, bool markdown = false)
            {
                Readme = new HmodReadme(readme, markdown);
                setValues();
            }
            private void setValues()
            {
                Name = Readme.frontMatter.ContainsKey("Name") ? Readme.frontMatter["Name"] : RawName;
                Category = Readme.frontMatter.ContainsKey("Category") ? Readme.frontMatter["Category"] : null;
                Creator = Readme.frontMatter.ContainsKey("Creator") ? Readme.frontMatter["Creator"] : null;
                Version = Readme.frontMatter.ContainsKey("Version") ? Readme.frontMatter["Version"] : null;
                EmulatedSystem = Readme.frontMatter.ContainsKey("Emulated System") ? Readme.frontMatter["Emulated System"] : null;
            }
        }

        public Repository(string repositoryURL)
        {
            this.RepositoryURL = repositoryURL + (repositoryURL.EndsWith("/") ? "" : "/");
            this.RepositoryURL = RepositoryURL + (RepositoryURL.EndsWith("/.repo/") ? "" : ".repo/");
        }

        private string StreamToString(Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        public void Load()
        {
            string[] list = new string[] { };

            var repoResponse = HTTPHelpers.GetHTTPResponseStreamAsync(RepositoryPackURL);
            repoResponse.Wait();

            if (repoResponse.Result.Status == HttpStatusCode.OK)
            {
                // Start pack processing
                var tempDict = new Dictionary<string, Item>();
                var trackableStream = new TrackableStream(repoResponse.Result.Stream);
                trackableStream.OnProgress += (long current, long total) => {
                    RepositoryProgress?.Invoke(current, repoResponse.Result.Length);
                };
                using (var reader = ReaderFactory.Open(trackableStream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (Regex.Match(reader.Entry.Key, @"^(?:\./)?list$", RegexOptions.IgnoreCase).Success)
                        {
                            list = Regex.Replace(StreamToString(reader.OpenEntryStream()), @"[\r\n]+", "\n").Split("\n"[0]);
                        }

                        if (Regex.Match(reader.Entry.Key, @"^(?:\./)?readme.md$", RegexOptions.IgnoreCase).Success)
                        {
                            Readme = StreamToString(reader.OpenEntryStream());
                        }
                        
                        var match = Regex.Match(reader.Entry.Key, @"^(?:\./)?([^/]+)/(extract|link|md5|sha1|readme(?:\.(?:md|txt)?)?)$", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var mod = match.Groups[1].ToString();
                            var fileName = match.Groups[2].ToString();

                            Item item;

                            if (!tempDict.TryGetValue(mod, out item))
                            {
                                item = new Item(mod);
                                tempDict.Add(mod, item);
                            }

                            switch (fileName.ToLower())
                            {
                                case "extract":
                                    item.setExtract(true);
                                    break;

                                case "link":
                                    item.setURL(StreamToString(reader.OpenEntryStream()).Trim());
                                    break;

                                case "md5":
                                    item.setMD5(StreamToString(reader.OpenEntryStream()).Trim());
                                    break;

                                case "sha1":
                                    item.setSHA1(StreamToString(reader.OpenEntryStream()).Trim());
                                    break;

                                case "readme":
                                case "readme.txt":
                                case "readme.md":
                                    item.setReadme(StreamToString(reader.OpenEntryStream()).Trim(), fileName.EndsWith(".md"));
                                    break;
                            }
                        }
                    }
                }

                if (list.Length == 0)
                    list = tempDict.Keys.ToArray();

                foreach (var key in tempDict.Keys.ToArray())
                {
                    var item = tempDict[key];
                    if (list.Contains(key))
                    {
                        Items.Add(item);
                    }
                    tempDict.Remove(key);
                }
                tempDict.Clear();
                tempDict = null;
                Items.Sort((x, y) => x.Name.CompareTo(y.Name));
                RepositoryLoaded?.Invoke(Items.ToArray());
                return;
                // End pack processing
            }
            
            var taskList = HTTPHelpers.GetHTTPResponseStringAsync(RepositoryListURL);

            taskList.Wait();

            list = (taskList.Result ?? "").Split("\n"[0]);
            
            for (int i = 0; i < list.Length; i++)
            {
                var mod = list[i];
                Item item = new Item(mod);
                var taskExtract = HTTPHelpers.GetHTTPStatusCodeAsync($"{RepositoryURL}{mod}/extract");
                var taskURL = HTTPHelpers.GetHTTPResponseStringAsync($"{RepositoryURL}{mod}/link");
                var taskMD5 = HTTPHelpers.GetHTTPResponseStringAsync($"{RepositoryURL}{mod}/md5");
                var taskSHA1 = HTTPHelpers.GetHTTPResponseStringAsync($"{RepositoryURL}{mod}/sha1");

                taskExtract.Wait();
                taskURL.Wait();
                taskMD5.Wait();
                taskSHA1.Wait();

                item.setExtract(taskExtract.Result == HttpStatusCode.OK);
                item.setURL(taskURL.Result);
                item.setMD5(taskMD5.Result);
                item.setSHA1(taskSHA1.Result);

                for (var x = 0; x < HmodReadme.readmeFiles.Length; x++)
                {
                    var taskReadme = HTTPHelpers.GetHTTPResponseStringAsync($"{RepositoryURL}{mod}/{HmodReadme.readmeFiles[x]}");

                    taskReadme.Wait();

                    if (taskReadme.Result != null)
                    {
                        item.setReadme(taskReadme.Result, HmodReadme.readmeFiles[x].EndsWith(".md"));
                        break;
                    }
                }

                Items.Add(item);
                RepositoryProgress?.Invoke(i + 1, list.Length);
            }
            RepositoryLoaded?.Invoke(Items.ToArray());

            return;
        }

        public Item[] LoadTasker(Form hostForm)
        {
            using (var tasker = new Tasks.Tasker(hostForm))
            {
                tasker.AttachViews(new TaskerTaskbar(), new TaskerForm());
                tasker.SetStatusImage(Resources.sign_cogs);
                tasker.SetTitle("Loading Repository");
                tasker.AddTask(LoadTask);
                if (tasker.Start() == Tasker.Conclusion.Success)
                {
                    return Items.ToArray();
                }
                return null;
            }
        }

        private Conclusion LoadTask(Tasker tasker, Object syncObject)
        {
            tasker.SetStatus("Loading...");
            RepositoryProgress += (long current, long max) =>
            {
                tasker.SetProgress(current, max);
            };
            Load();
            return Conclusion.Success;
        }
    }
}
