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
using Newtonsoft.Json;

namespace com.clusterrr.hakchi_gui
{
    public static class CoreCollection
    {
        private const string WHITELIST_UPDATE_URL = "https://teamshinkansen.github.io/retroarch-whitelist.txt";
        private static readonly string WhiteListFilename = Shared.PathCombine(Program.BaseDirectoryExternal, "config", "retroarch_whitelist.txt");
        private static readonly string CollectionFilename = Shared.PathCombine(Program.BaseDirectoryExternal, "config", "cores.json");

        public enum CoreKind { Unknown, BuiltIn, Libretro };
        public class CoreInfo
        {
            public readonly string Bin;
            public string Name = string.Empty;
            public string DisplayName = string.Empty;
            public string[] SupportedExtensions = null;
            public string[] Systems = null;
            public CoreKind Kind = CoreKind.Unknown;
            public CoreInfo(string bin)
            {
                Bin = bin;
            }
            public string QualifiedBin
            {
                get
                {
                    switch (Kind) {
                        case CoreKind.Libretro:
                            return $"/bin/libretro/{Bin}";
                        case CoreKind.BuiltIn:
                            return $"/bin/{Bin}";
                    }
                    return Bin;
                }
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

        private static CoreInfo Canoe = new CoreInfo("clover-canoe-shvc-wr --rom")
        {
            Name = "Canoe",
            DisplayName = "Nintendo - Super Nintendo Entertainment System (Canoe)",
            SupportedExtensions = new string[] { ".sfrom", ".smc", ".sfc" },
            Systems = new string[] { "Nintendo - Super Nintendo Entertainment System" },
            Kind = CoreKind.BuiltIn
        };
        private static readonly CoreInfo Kachikachi = new CoreInfo("clover-kachikachi-wr")
        {
            Name = "Kachikachi",
            DisplayName = "Nintendo - Nintendo Entertainment System (Kachikachi)",
            SupportedExtensions = new string[] { ".nes", ".fds" },
            Systems = new string[] { "Nintendo - Nintendo Entertainment System", "Nintendo - Family Computer Disk System" },
            Kind = CoreKind.BuiltIn
        };

        private static Dictionary<string, CoreInfo> cores = new Dictionary<string, CoreInfo>();
        private static SortedDictionary<string, List<CoreInfo>> extIndex = new SortedDictionary<string, List<CoreInfo>>();
        private static SortedDictionary<string, List<CoreInfo>> systemIndex = new SortedDictionary<string, List<CoreInfo>>();

        private static void UpdateWhitelist()
        {
            var client = new WebClient();
            try
            {
                Debug.WriteLine("Downloading whitelist file, URL: " + WHITELIST_UPDATE_URL);
                string whitelist = client.DownloadString(WHITELIST_UPDATE_URL);
                if (!string.IsNullOrEmpty(whitelist))
                    File.WriteAllText(WhiteListFilename, whitelist);
            }
            catch { }
        }

        public static void Load()
        {
            // clear and add default cores
            cores.Clear();
            cores.Add(Canoe.Bin, Canoe);
            cores.Add(Kachikachi.Bin, Kachikachi);

            // load info files
            Debug.WriteLine("Loading libretro core info files");
            var whiteList = File.Exists(WhiteListFilename) ? File.ReadAllLines(WhiteListFilename) : Resources.retroarch_whitelist.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
                    string system = null;
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
                                    system = mm.Groups[2].ToString();
                                    break;
                                case "supported_extensions":
                                    core.SupportedExtensions = mm.Groups[2].ToString().Split('|');
                                    for (var i = 0; i < core.SupportedExtensions.Length; ++i)
                                        core.SupportedExtensions[i] = "." + core.SupportedExtensions[i];
                                    break;
                                case "database":
                                    core.Systems = mm.Groups[2].ToString().Split('|');
                                    break;
                            }
                        }
                    }
                    if (core.Systems == null && system != null)
                        core.Systems = new string[] { system };
                    core.Kind = CoreKind.Libretro;
                    cores[bin] = core;
                }
            }
            new Thread(CoreCollection.UpdateWhitelist).Start();

            // cross indexing
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

            DebugWrite();
        }

        public static void DebugWrite()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CollectionFilename));
            File.WriteAllText(CollectionFilename, JsonConvert.SerializeObject(cores, Formatting.Indented));
        }

        public static string[] Cores
        {
            get { return cores.Keys.ToArray(); }
        }

        public static string[] Extensions
        {
            get { return extIndex.Keys.ToArray(); }
        }

        public static string[] Systems
        {
            get { return systemIndex.Keys.ToArray(); }
        }

        public static CoreInfo GetCore(string bin)
        {
            return cores.ContainsKey(bin) ? cores[bin] : null;
        }

        public static IEnumerable<CoreInfo> GetCoresFromExtension(string ext)
        {
            return extIndex.ContainsKey(ext) ? extIndex[ext] : null;
        }

        public static IEnumerable<CoreInfo> GetCoresFromSystem(string name)
        {
            return systemIndex.ContainsKey(name) ? systemIndex[name] : null;
        }

        public static IEnumerable<string> GetSystemsFromExtension(string ext)
        {
            var systems = new List<string>();
            var cores = GetCoresFromExtension(ext);
            if (cores != null)
            {
                foreach(var core in cores)
                {
                    if (core.Systems != null)
                        foreach(var system in core.Systems)
                            systems.Add(system);
                }
            }
            return systems.Distinct();
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
