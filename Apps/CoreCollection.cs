using com.clusterrr.hakchi_gui.Properties;
using SharpCompress.Archives;
using Newtonsoft.Json;
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
    public static class CoreCollection
    {
        private static readonly string CollectionFilename = Shared.PathCombine(Program.BaseDirectoryExternal, "config", "cores{0}.json");

        public enum CoreKind { Unknown, BuiltIn, Libretro };
        public class CoreInfo : IEquatable<CoreInfo>
        {
            public readonly string Bin;
            public string DefaultArgs = string.Empty;
            public string Name = string.Empty;
            public string DisplayName = string.Empty;
            public string[] SupportedExtensions = null;
            public string[] Systems = null;
            public CoreKind Kind = CoreKind.Unknown;
            public CoreInfo(string bin)
            {
                Bin = bin;
            }
            [JsonIgnore]
            public string QualifiedBin
            {
                get
                {
                    return $"/bin/" + CoreCommands.GetCommand(Bin);
                }
            }
            public void DebugWrite()
            {
                Trace.WriteLine("Bin: " + Bin);
                Trace.WriteLine("Name: " + Name);
                Trace.WriteLine("DisplayName: " + DisplayName);
                if (SupportedExtensions != null)
                    Trace.WriteLine("SupportedExtensions: " + string.Join(", ", SupportedExtensions));
                if (Systems != null)
                    Trace.WriteLine("Systems: " + string.Join(", ", Systems));
                Trace.WriteLine("");
            }
            public override string ToString()
            {
                return Name;
            }

            // equality methods

            public override bool Equals(object obj)
            {
                var core = obj as CoreInfo;
                return Equals(core);
            }

            public static bool operator ==(CoreInfo a, CoreInfo b)
            {
                if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                    return true;
                if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                    return false;
                return a.Equals(b);
            }

            public static bool operator !=(CoreInfo a, CoreInfo b)
            {
                return !(a == b);
            }

            public bool Equals(CoreInfo core)
            {
                if (ReferenceEquals(core, null))
                    return false;
                return this.Bin.Equals(core.Bin);
            }

            public override int GetHashCode()
            {
                return this.Bin.GetHashCode();
            }
        }

        private static Dictionary<string, CoreInfo> cores = new Dictionary<string, CoreInfo>();
        private static SortedDictionary<string, List<CoreInfo>> extIndex = new SortedDictionary<string, List<CoreInfo>>();
        private static SortedDictionary<string, List<CoreInfo>> systemIndex = new SortedDictionary<string, List<CoreInfo>>();

        public static void Load()
        {
            // try to load from cache
            if (Deserialize())
            {
                return;
            }

            // load info files
            Trace.WriteLine("Loading libretro core info files");
            var regex = new Regex("(^[^\\s]+)\\s+=\\s+\"?([^\"\\r\\n]*)\"?", RegexOptions.Multiline | RegexOptions.Compiled);

            cores = new Dictionary<string, CoreInfo>();
            using (var extractor = ArchiveFactory.Open(Shared.PathCombine(Program.BaseDirectoryInternal, "data", "libretro_cores.7z")))
            {
                using (var reader = extractor.ExtractAllEntries())
                    while (reader.MoveToNextEntry())
                    {
                        string file = reader.Entry.Key;

                        Match m = Regex.Match(Path.GetFileNameWithoutExtension(file), "^(.*)_libretro");
                        if (m.Success && !string.IsNullOrEmpty(m.Groups[1].ToString()))
                        {
                            var bin = m.Groups[1].ToString();
                            var f = new StreamReader(reader.OpenEntryStream()).ReadToEnd();
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
            }

            // add built-in cores
            BuiltInCores.List.ToList().ForEach(c => cores.Add(c.Bin, c));

            // cross indexing
            Trace.WriteLine("Building libretro core cross index");
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

            // save cache
            Serialize();
        }

        public static IEnumerable<CoreInfo> Cores
        {
            get { return cores.Values; }
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

        public static CoreInfo GetCoreFromExec(string exec)
        {
            exec = exec.ToLower().Trim();
            foreach(var core in cores)
            {
                if (exec.StartsWith(core.Value.QualifiedBin))
                {
                    return core.Value;
                }
            }
            return null;
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

        public static IEnumerable<string> GetExtensionsFromSystem(string system)
        {
            var extensions = new List<string>();
            var cores = GetCoresFromSystem(system);
            if (cores != null)
            {
                foreach (var core in cores)
                {
                    if (core.SupportedExtensions != null)
                        foreach (var ext in core.SupportedExtensions)
                            extensions.Add(ext);
                }
            }
            return extensions.Distinct();
        }

        public static bool IsCoreValid(string bin, string ext)
        {
            if (!cores.ContainsKey(bin))
                return false;
            if (!cores[bin].SupportedExtensions.Contains(ext))
                return false;
            return true;
        }

        public static bool Serialize()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CollectionFilename));
                File.WriteAllText(string.Format(CollectionFilename, string.Empty), JsonConvert.SerializeObject(cores, Formatting.Indented));
                StringBuilder builder = new StringBuilder("{\n");
                foreach (var pair in extIndex)
                {
                    string line = "\t\"" + pair.Key + "\": [ \"" + string.Join("\", \"", pair.Value.Select(c => c.Bin).ToArray()) + "\" ],";
                    builder.AppendLine(line);
                }
                builder.Append("}\n");
                File.WriteAllText(string.Format(CollectionFilename, "_ext"), builder.ToString());

                builder = new StringBuilder("{\n");
                foreach (var pair in systemIndex)
                {
                    string line = "\t\"" + pair.Key + "\": [ \"" + string.Join("\", \"", pair.Value.Select(c => c.Bin).ToArray()) + "\" ],";
                    builder.AppendLine(line);
                }
                builder.Append("}\n");
                File.WriteAllText(string.Format(CollectionFilename, "_systems"), builder.ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ": ", ex.StackTrace);
                return false;
            }
            return true;
        }

        public static bool Deserialize()
        {
            try
            {
                if (!File.Exists(string.Format(CollectionFilename, string.Empty)))
                    return false;
                var fileInfo = new FileInfo(string.Format(CollectionFilename, string.Empty));
                if (fileInfo.LastWriteTimeUtc < DateTime.UtcNow.AddDays(-1))
                    return false;

                Trace.Write("CoreCollection loading indexes from cache: ");

                // load cores
                cores = JsonConvert.DeserializeObject<Dictionary<string, CoreInfo>>(File.ReadAllText(string.Format(CollectionFilename, string.Empty)));

                // load extensions index
                var indexStrings = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(File.ReadAllText(string.Format(CollectionFilename, "_ext")));
                extIndex.Clear();
                foreach(var pair in indexStrings)
                {
                    var list = new List<CoreInfo>();
                    foreach(var core in pair.Value)
                    {
                        list.Add(cores[core]);
                    }
                    extIndex[pair.Key] = list;
                }

                // load systems index
                indexStrings = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(File.ReadAllText(string.Format(CollectionFilename, "_systems")));
                systemIndex.Clear();
                foreach (var pair in indexStrings)
                {
                    var list = new List<CoreInfo>();
                    foreach (var core in pair.Value)
                    {
                        list.Add(cores[core]);
                    }
                    systemIndex[pair.Key] = list;
                }

                Trace.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ": ", ex.StackTrace);
                cores.Clear();
                extIndex.Clear();
                systemIndex.Clear();
                return false;
            }
            return true;
        }
    }
}
