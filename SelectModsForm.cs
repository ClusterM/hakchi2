using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CommonMark;
using System.Drawing;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;

namespace com.clusterrr.hakchi_gui
{
    public struct ReadmeCache
    {
        public string Checksum;
        public DateTime LastModified;
        public string[] dataKeys;
        public string[] dataValues;
        [XmlIgnore] public Dictionary<string, string> ReadmeData
        {
            get
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                for (int i = 0; i < dataKeys.Length; i++)
                {
                    data.Add(dataKeys[i], dataValues[i]);
                }
                return data;
            }
        }

        public ReadmeCache(Dictionary<string, string> ReadmeData, string Checksum, DateTime LastModified)
        {
            dataKeys = ReadmeData.Keys.ToArray();
            dataValues = ReadmeData.Values.ToArray();
            this.Checksum = Checksum;
            this.LastModified = LastModified;
        }
    }
    public struct HmodReadme
    {
        public readonly Dictionary<string, string> frontMatter;
        public readonly string readme;
        public readonly string rawReadme;
        public readonly bool isMarkdown;
        public HmodReadme(string readme, bool markdown = false)
        {
            this.rawReadme = readme;
            Dictionary<string, string> output = new Dictionary<string, string>();
            Match match = Regex.Match(readme, "^(?:-{3,}[\\r\\n]+(.*?)[\\r\\n]*-{3,})?[\\r\\n\\t\\s]*(.*)[\\r\\n\\t\\s]*$", RegexOptions.Singleline);
            this.readme = match.Groups[2].Value;
            MatchCollection matches = Regex.Matches(match.Groups[1].Value, "^[\\s\\t]*([^:]+)[\\s\\t]*:[\\s\\t]*(.*?)[\\s\\t]*$", RegexOptions.Multiline);
            foreach (Match fmMatch in matches)
            {
                if (!output.ContainsKey(fmMatch.Groups[1].Value))
                {
                    output.Add(fmMatch.Groups[1].Value, fmMatch.Groups[2].Value);
                }
            }
            this.frontMatter = output;
            this.isMarkdown = markdown;
        }
    }
    public struct Hmod
    {
        public readonly string Name;
        public readonly string HmodPath;
        public readonly bool isFile;
        public readonly HmodReadme Readme;
        public readonly string RawName;
        public readonly string Category;
        public readonly string Creator;

        public Hmod(string mod)
        {
            RawName = mod;
            this.HmodPath = null;
            this.isFile = false;

            string[] readmeFiles = new string[] { "readme.txt", "readme.md", "readme" };
            string usermodsDirectory = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
            string cacheDir = Shared.PathCombine(Program.BaseDirectoryExternal, "user_mods", "readme_cache");
            string cacheFile = Path.Combine(cacheDir, $"{mod}.xml");


            Dictionary<string, string> readmeData = new Dictionary<string, string>();

            try
            {
                var dir = Path.Combine(usermodsDirectory, mod + ".hmod");
                if (Directory.Exists(dir))
                {
                    isFile = false;
                    HmodPath = dir;
                    foreach (var f in readmeFiles)
                    {
                        var fn = Path.Combine(dir, f);
                        if (File.Exists(fn))
                        {
                            readmeData.Add(f.ToLower(), File.ReadAllText(fn));
                        }
                    }
                }
                else if (File.Exists(dir))
                {
                    isFile = true;
                    HmodPath = dir;

                    ReadmeCache cache;
                    FileInfo info = new FileInfo(dir);
                    
                    bool skipExtraction = false;
                    if (File.Exists(cacheFile))
                    {
                        try
                        {
                            cache = XMLSerialization.DeserializeXMLFileToObject<ReadmeCache>(cacheFile);
                            if (cache.LastModified == info.LastWriteTimeUtc)
                            {
                                skipExtraction = true;
                                readmeData = cache.ReadmeData;
                            }
                        } catch(Exception ex) { }
                    }


                    if (!skipExtraction)
                    {
                        SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                        using (var szExtractor = new SevenZipExtractor(dir))
                        {
                            var tar = new MemoryStream();
                            szExtractor.ExtractFile(0, tar);
                            tar.Seek(0, SeekOrigin.Begin);
                            using (var szExtractorTar = new SevenZipExtractor(tar))
                            {
                                foreach (var f in szExtractorTar.ArchiveFileNames)
                                {
                                    if (readmeFiles.Contains(f.ToLower()))
                                    {
                                        var o = new MemoryStream();
                                        szExtractorTar.ExtractFile(f, o);
                                        readmeData.Add(f.ToLower(), Encoding.UTF8.GetString(o.ToArray()));
                                    }
                                }
                            }
                        }
                        cache = new ReadmeCache(readmeData, "", info.LastWriteTimeUtc);

                        if (!Directory.Exists(cacheDir))
                            Directory.CreateDirectory(cacheDir);

                        File.WriteAllText(cacheFile, cache.Serialize());
                    }
                }
                else
                {
                    if (File.Exists(cacheFile))
                    {
                        try
                        {
                            ReadmeCache cache;
                            cache = XMLSerialization.DeserializeXMLFileToObject<ReadmeCache>(cacheFile);
                            readmeData = cache.ReadmeData;
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            catch
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

            if (!this.Readme.frontMatter.TryGetValue("Creator", out this.Creator))
            {
                this.Creator = Properties.Resources.Unknown;
            }
        }
    }
    public enum HmodListSort
    {
        Category,
        Creator
    }
    public partial class SelectModsForm : Form
    {
        private readonly string usermodsDirectory;
        private List<Hmod> hmods = new List<Hmod>();

        public SelectModsForm(bool loadInstalledMods, bool allowDropMods, string[] filesToAdd = null)
        {

            InitializeComponent();

            switch (ConfigIni.hmodListSort)
            {
                case HmodListSort.Category:
                    categoryToolStripMenuItem.Checked = true;
                    break;

                case HmodListSort.Creator:
                    creatorToolStripMenuItem.Checked = true;
                    break;
            }

            wbReadme.Document.BackColor = this.BackColor;
            usermodsDirectory = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
            var modsList = new List<string>();
            if (loadInstalledMods && MainForm.Clovershell.IsOnline)
            {
                var modsstr = MainForm.Clovershell.ExecuteSimple("ls /var/lib/hakchi/hmod/uninstall-*", 2000, true);
                var installedMods = modsstr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var mod in installedMods)
                {
                    var modname = mod;
                    int pos;
                    while ((pos = modname.IndexOf("/")) >= 0)
                        modname = modname.Substring(pos + 1);
                    modname = modname.Substring("uninstall-".Length);
                    if (MainForm.InternalMods.Contains(modname))
                        continue;
                    modsList.Add(modname);
                }
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

            using(WorkerForm worker = new WorkerForm())
            {
                worker.Task = WorkerForm.Tasks.GetHmods;
                worker.HmodsToLoad = modsList.ToArray();
                worker.Start();
                hmods = worker.LoadedHmods;
            }

            populateList();

            if (filesToAdd != null) AddMods(filesToAdd);
            this.AllowDrop = allowDropMods;
        }

        private void populateList()
        {
            SortedDictionary<string, ListViewGroup> listGroups = new SortedDictionary<string, ListViewGroup>();
            listViewHmods.BeginUpdate();
            listViewHmods.Groups.Clear();
            listViewHmods.Items.Clear();

            foreach (Hmod hmod in hmods)
            {
                string groupName = Properties.Resources.Unknown;

                switch (ConfigIni.hmodListSort)
                {
                    case HmodListSort.Category:
                        groupName = hmod.Category;
                        break;

                    case HmodListSort.Creator:
                        groupName = hmod.Creator;
                        break;
                }

                ListViewGroup group;
                if (!listGroups.TryGetValue(groupName.ToLower(), out group))
                {
                    group = new ListViewGroup(groupName, HorizontalAlignment.Center);
                    listGroups.Add(groupName.ToLower(), group);
                }
                ListViewItem item = new ListViewItem(new String[] { hmod.Name, hmod.Creator });
                item.SubItems.Add(hmod.Creator);
                item.Tag = hmod;
                item.Group = group;
                listViewHmods.Items.Add(item);
            }

            foreach (ListViewGroup group in listGroups.Values)
            {
                listViewHmods.Groups.Add(group);
            }

            listViewHmods.EndUpdate();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (listViewHmods.CheckedItems.Count > 0)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;
            Close();
        }
        static string getHmodPath(string hmod)
        {
            try
            {
                Dictionary<string, string> readmeData = new Dictionary<string, string>();
                var dir = Shared.PathCombine(Program.BaseDirectoryExternal, "user_mods", hmod + ".hmod");
                if (Directory.Exists(dir))
                {
                    return dir + Path.DirectorySeparatorChar;
                }
                else if (File.Exists(dir))
                {
                    return dir;
                }
            }
            catch
            {
            }

            return null;
        }

        string formatReadme(ref Hmod hmod)
        {
            

            string[] headingFields = { "Creator", "Version" };
            List<string> headingLines = new List<string>();

            foreach (string heading in headingFields)
            {
                string keyValue;
                if (hmod.Readme.frontMatter.TryGetValue(heading, out keyValue))
                {
                    headingLines.Add($"**{heading}:** {keyValue}");
                }
            }

            foreach (string keyName in hmod.Readme.frontMatter.Keys)
            {
                if (!headingFields.Contains(keyName) && keyName != "Name")
                {
                    headingLines.Add($"**{keyName}:** {hmod.Readme.frontMatter[keyName]}");
                }
            }
            
            return CommonMarkConverter.Convert(String.Join("  \n", headingLines.ToArray()) + "\n\n" + (hmod.Readme.isMarkdown || hmod.Readme.readme.Length == 0 ? hmod.Readme.readme : $"```\n{hmod.Readme.readme}\n```"));
        }

        void showReadMe(ref Hmod hmod)
        {
                Color color = this.BackColor;
                string html = String.Format(Properties.Resources.readmeTemplateHTML, Properties.Resources.readmeTemplateCSS, hmod.Name, formatReadme(ref hmod), $"rgb({color.R},{color.G},{color.B})");
                wbReadme.DocumentText = html;
        }

        private void SelectModsForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void SelectModsForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddMods(files);
        }

        private void AddMods(string[] files)
        {
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".hmod")
                {
                    var target = Path.Combine(usermodsDirectory, Path.GetFileName(file));
                    if (file != target)
                        File.Copy(file, target, true);
                    hmods.Add(new Hmod(Path.GetFileNameWithoutExtension(file)));
                }
                else if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var szExtractor = new SevenZipExtractor(file))
                    {
                        foreach (var f in szExtractor.ArchiveFileNames)
                            if (Path.GetExtension(f).ToLower() == ".hmod")
                            {
                                using (var outFile = new FileStream(Path.Combine(usermodsDirectory, Path.GetFileName(f)), FileMode.Create))
                                {
                                    szExtractor.ExtractFile(f, outFile);
                                    hmods.Add(new Hmod(Path.GetFileNameWithoutExtension(file)));
                                }
                            }
                    }
                }
            }

            populateList();
        }

        private void wbReadme_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() == "about:blank") return;

            //cancel the current event
            e.Cancel = true;

            //this opens the URL in the user's default browser
            Process.Start(e.Url.ToString());
        }

        private void listViewHmods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewHmods.SelectedItems.Count > 0)
            {

                Hmod hmod = (Hmod)(listViewHmods.SelectedItems[0].Tag);
                showReadMe(ref hmod);
            }
            else
            {
                wbReadme.Refresh();
            }
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            categoryToolStripMenuItem.Checked = true;
            creatorToolStripMenuItem.Checked = false;
            ConfigIni.hmodListSort = HmodListSort.Category;
            populateList();
        }

        private void creatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            creatorToolStripMenuItem.Checked = true;
            categoryToolStripMenuItem.Checked = false;
            ConfigIni.hmodListSort = HmodListSort.Creator;
            populateList();
        }
    }
}
