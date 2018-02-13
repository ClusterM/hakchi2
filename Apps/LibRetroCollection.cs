using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace com.clusterrr.hakchi_gui
{
    static class LibRetroCollection
    {
        private const string WHITELIST_UPDATE_URL = "https://teamshinkansen.github.io/retroarch-whitelist.txt";
        private static readonly string WHITELIST_FILENAME = Shared.PathCombine(Program.BaseDirectoryExternal, "config", "retroarch_whitelist.txt");

        public class CoreInfo
        {
            public readonly string Bin;
            public string Name = string.Empty;
            public string DisplayName = string.Empty;
            public string[] SupportedExtensions = null;
            public string[] Systems = null;
            public CoreInfo(string bin)
            {
                Bin = bin;
            }
            public void DebugWrite()
            {
                Debug.WriteLine("Bin: " + Bin);
                Debug.WriteLine("Name: " + Name);
                Debug.WriteLine("DisplayName: " + DisplayName);
                if (SupportedExtensions != null)
                    Debug.WriteLine("SupportedExtensions: " + string.Join(", ", SupportedExtensions));
                if (Systems != null)
                    Debug.WriteLine("Systems: " + string.Join(", ", Systems));
                Debug.WriteLine("");
            }
        }

        private static Dictionary<string, CoreInfo> cores = new Dictionary<string, CoreInfo>();
#if DEBUG
        private static SortedDictionary<string, List<CoreInfo>>
            extIndex = new SortedDictionary<string, List<CoreInfo>>(),
            systemIndex = new SortedDictionary<string, List<CoreInfo>>();
#else
        private static Dictionary<string, List<CoreInfo>>
            extIndex = new Dictionary<string, List<CoreInfo>>(),
            systemIndex = new Dictionary<string, List<CoreInfo>>();
#endif

        private static void UpdateWhitelist()
        {
            var client = new WebClient();
            try
            {
                Debug.WriteLine("Downloading whitelist file, URL: " + WHITELIST_UPDATE_URL);
                string whitelist = client.DownloadString(WHITELIST_UPDATE_URL);
                if (!string.IsNullOrEmpty(whitelist))
                    File.WriteAllText(WHITELIST_FILENAME, whitelist);
            }
            catch { }
        }

        public static void Load()
        {
            Debug.WriteLine("Loading libretro core info files");
            var whiteList = File.Exists(WHITELIST_FILENAME) ? File.ReadAllLines(WHITELIST_FILENAME) : Resources.retroarch_whitelist.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var regex = new Regex("(^[^\\s]+)\\s+=\\s+\"?([^\"\\r\\n]*)\"?", RegexOptions.Multiline | RegexOptions.Compiled);
            var infoFiles = Directory.GetFiles(Shared.PathCombine(Program.BaseDirectoryInternal, "data", "libretro_cores"), "*.info");
            foreach (var file in infoFiles)
            {
                Match m = Regex.Match(Path.GetFileNameWithoutExtension(file), "^(.*)_libretro");
                if (m.Success && !string.IsNullOrEmpty(m.Groups[1].ToString()))
                {
                    var bin = m.Groups[1].ToString();
                    if (!whiteList.Contains(bin))
                        continue;

                    var f = File.ReadAllText(file);
                    var matches = regex.Matches(f);
                    if (matches.Count <= 0)
                        continue;

                    var core = new CoreInfo(bin);
                    var systems = new List<string>();
                    foreach (Match mm in matches)
                    {
                        if (mm.Success)
                        {
                            switch (mm.Groups[1].ToString().ToLower())
                            {
                                case "corename":
                                    core.Name = mm.Groups[2].ToString();
                                    break;
                                case "display_name":
                                    core.DisplayName = mm.Groups[2].ToString();
                                    break;
                                case "systemname":
                                    systems.Add(mm.Groups[2].ToString());
                                    break;
                                case "supported_extensions":
                                    core.SupportedExtensions = mm.Groups[2].ToString().Split('|');
                                    break;
                                case "database":
                                    systems.AddRange(mm.Groups[2].ToString().Split('|'));
                                    break;
                            }
                        }
                    }
                    if (systems.Count > 0)
                        core.Systems = systems.ToArray();
                    cores[bin] = core;
                }
            }
            new Thread(LibRetroCollection.UpdateWhitelist).Start();

            Debug.WriteLine("Building libretro core cross index");
            foreach (var c in cores)
            {
                if (c.Value.SupportedExtensions != null)
                {
                    foreach (var ext in c.Value.SupportedExtensions)
                    {
                        if (!extIndex.ContainsKey(ext))
                            extIndex[ext] = new List<CoreInfo>();
                        if (!extIndex[ext].Contains(c.Value))
                            extIndex[ext].Add(c.Value);
                    }
                }
                if (c.Value.Systems != null)
                {
                    foreach (var sys in c.Value.Systems)
                    {
                        if (!systemIndex.ContainsKey(sys))
                            systemIndex[sys] = new List<CoreInfo>();
                        if (!systemIndex[sys].Contains(c.Value))
                            systemIndex[sys].Add(c.Value);
                    }
                }
            }
        }

        public static void DebugWrite() {
            Debug.WriteLine("Extensions Index:");
            foreach (var i in extIndex)
            {
                Debug.Write(i.Key + ": ");
                var coreList = i.Value;
                foreach (var c in coreList)
                {
                    Debug.Write(c.Bin + ", ");
                }
                Debug.WriteLine("");
            }

            Debug.WriteLine("Systems Index:");
            foreach (var i in systemIndex)
            {
                Debug.Write(i.Key + ": ");
                var coreList = i.Value;
                foreach (var c in coreList)
                {
                    Debug.Write(c.Bin + ", ");
                }
                Debug.WriteLine("");
            }
        }

        public static CoreInfo GetCore(string bin)
        {
            return cores.ContainsKey(bin) ? cores[bin] : null;
        }

        public static IEnumerable<CoreInfo> GetCoresFromExtension(string ext)
        {
            return extIndex.ContainsKey(ext) ? extIndex[ext] : null;
        }

        public static IEnumerable<CoreInfo> GetCoresFromName(string name)
        {
            return systemIndex.ContainsKey(name) ? systemIndex[name] : null;
        }

        public static bool IsCoreValid(string bin, string ext)
        {
            if (!cores.ContainsKey(bin))
                return false;
            if (!cores[bin].SupportedExtensions.Contains(ext))
                return false;
            return true;
        }
    }
}
