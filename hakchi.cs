using com.clusterrr.clovershell;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.Tasks;
using com.clusterrr.ssh;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace com.clusterrr.hakchi_gui
{
    public static class hakchi
    {
        public const string SERVICE_NAME = "hakchi";
        public const string SERVICE_TYPE = "_ssh._tcp";
        public const string USERNAME = "root";
        public const string PASSWORD = "";
        public static readonly string latestHmodFile = Path.Combine(Program.BaseDirectoryExternal ?? "", "data", "hakchi-latest.hmod");
        public const string latestHmodUrl = "https://hakchi.net/hakchi/hmods/hakchi-latest.hmod";
        public const long BLOCK_SIZE = 4096;

        

        public struct Hmod
        {
            public enum HmodLocation
            {
                Basehmods,
                HakchiLatest
            }

            public MemoryStream HmodStream;
            public HmodLocation Location;
            public DateTime LastModified;
            public static Hmod Get()
            {
                Hmod hakchiHmod = new Hmod();

                hakchiHmod.HmodStream = new MemoryStream();

                using (var extractor = ArchiveFactory.Open(Path.Combine(Program.BaseDirectoryInternal, "basehmods.tar")))
                {
                    var hakchiEntry = extractor.Entries.Where(e => e.Key == "./hakchi.hmod" || e.Key == "hakchi.hmod").First();
                    if (File.Exists(latestHmodFile) && File.GetLastWriteTime(latestHmodFile) > hakchiEntry.LastModifiedTime)
                    {
                        using (var file = File.OpenRead(latestHmodFile))
                        {
                            hakchiHmod.LastModified = File.GetLastWriteTime(latestHmodFile);
                            hakchiHmod.Location = Hmod.HmodLocation.HakchiLatest;
                            file.CopyTo(hakchiHmod.HmodStream);
                        }
                    }
                    else
                    {
                        hakchiHmod.LastModified = hakchiEntry.LastModifiedTime.Value;
                        hakchiHmod.Location = Hmod.HmodLocation.Basehmods;
                        hakchiEntry.OpenEntryStream().CopyTo(hakchiHmod.HmodStream);
                    }
                }
                hakchiHmod.HmodStream.Seek(0, SeekOrigin.Begin);

                return hakchiHmod;
            }

            public IArchive Archive()
            {
                using (Stream hakchiHmod = HmodStream)
                {
                    MemoryStream tar = new MemoryStream();
                    using (var extractor = ArchiveFactory.Open(hakchiHmod))
                    using (var entryStream = extractor.Entries.First().OpenEntryStream())
                    {
                        entryStream.CopyTo(tar);
                        tar.Position = 0;
                        return SharpCompress.Archives.Tar.TarArchive.Open(tar);
                    }
                }
            }

            private MemoryStream ArchiveFile(string filename)
            {
                var image = new MemoryStream();
                Archive().Entries.Where(e => e.Key == filename).First().OpenEntryStream().CopyTo(image);
                return image;
            }

            public static Dictionary<string, string> GetVersion() => Get().Version();
            public Dictionary<string, string> Version()
            {
                using (var o = ArchiveFile("var/version"))
                {
                    string contents = Encoding.UTF8.GetString(o.ToArray());

                    MatchCollection collection = Regex.Matches(contents, @"^([^=]+)=(""(?:[^""\\]*(?:\\.[^""\\]*)*)""|\'(?:[^\'\\]*(?:\\.[^\'\\]*)*)\')", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    var version = new Dictionary<string, string>();
                    foreach (Match match in collection)
                    {
                        string param = match.Groups[1].Value;
                        string value = match.Groups[2].Value;
                        value = value.Substring(1, value.Length - 2).Replace("\'", "'").Replace("\\\"", "\"");
                        version[param] = value;
                    }
                    return version;
                }
            }

            public static MemoryStream GetHmodStream() => Get().HmodStream;

            public static MemoryStream GetMembootImage() => Get().MembootImage();
            public MemoryStream MembootImage() => ArchiveFile("boot/boot.img");

            public static MemoryStream GetUboot(UbootType type = UbootType.SD) => Get().Uboot(type);
            public MemoryStream Uboot(UbootType type = UbootType.SD)
            {
                var ubootStream = ArchiveFile("boot/uboot.bin");
                if (type == UbootType.SD)
                {
                    ubootStream.Seek(-8, SeekOrigin.End);
                    byte[] sdMarker = new byte[8];
                    ubootStream.Read(sdMarker, 0, sdMarker.Length);
                    ubootStream.Seek(-8, SeekOrigin.End);
                    ubootStream.Write(sdMarker, 4, 4);
                    ubootStream.Write(sdMarker, 0, 4);
                    ubootStream.Seek(0, SeekOrigin.Begin);
                }
                return ubootStream;
            }
        }

        public static Hmod Get() => Hmod.Get();

        public enum ConsoleType
        {
            NES = 0,
            Famicom = 1,
            SNES_EUR = 2,
            SNES_USA = 3,
            SuperFamicom = 4,
            ShonenJump = 5,
            MD_JPN = 6,
            MD_USA = 7,
            MD_EUR = 8,
            MD_ASIA = 9,
            Unknown = 255
        }

        public static readonly Dictionary<ConsoleType, string> ConsoleTypeToSystemCode = new Dictionary<ConsoleType, string>()
        {
            { ConsoleType.NES, "nes-usa" },
            { ConsoleType.Famicom, "nes-jpn" },
            { ConsoleType.SNES_EUR, "snes-eur" },
            { ConsoleType.SNES_USA, "snes-usa" },
            { ConsoleType.SuperFamicom, "snes-jpn" },
            { ConsoleType.ShonenJump, "hvcj-jpn" },
            { ConsoleType.MD_JPN, "md-jpn" },
            { ConsoleType.MD_USA, "md-usa" },
            { ConsoleType.MD_EUR, "md-eur" },
            { ConsoleType.MD_ASIA, "md-asia" },
        };
        public static readonly Dictionary<string, ConsoleType> SystemCodeToConsoleType = new Dictionary<string, ConsoleType>()
        {
            { "nes-usa", ConsoleType.NES },
            { "nes-jpn", ConsoleType.Famicom  },
            { "snes-eur", ConsoleType.SNES_EUR },
            { "snes-usa", ConsoleType.SNES_USA },
            { "snes-jpn", ConsoleType.SuperFamicom },
            { "hvcj-jpn", ConsoleType.ShonenJump },
            { "md-jpn", ConsoleType.MD_JPN },
            { "md-usa", ConsoleType.MD_USA },
            { "md-eur", ConsoleType.MD_EUR },
            { "md-asia", ConsoleType.MD_ASIA }
        };

        public static ISystemShell Shell { get; private set; }
        public static bool Connected { get; private set; }
        public static event OnConnectedEventHandler OnConnected = delegate { };
        public static event OnDisconnectedEventHandler OnDisconnected = delegate { };

        public static ConsoleType? DetectedConsoleType { get; private set; }
        public static bool CustomFirmwareLoaded { get; private set; }
        public static string RawBootVersion { get; private set; }
        public static string RawKernelVersion { get; private set; }
        public static string RawScriptVersion { get; private set; }
        public static string UniqueID { get; private set; }
        public static bool CanInteract { get; private set; }
        public static bool CanSync { get; private set; }
        public static bool MinimalMemboot { get; private set; }
        public static bool UserMinimalMemboot
        {
            get { return hakchi.Shell.IsOnline && MinimalMemboot && (Shell.Execute("ls /user-recovery.flag") == 0); }
        }

        public static string ConfigPath { get; private set; }
        public static string RemoteGameSyncPath { get; private set; }
        public static string SystemCode { get; private set; }
        public static string MediaPath { get; private set; }
        public static string OriginalGamesPath { get; private set; }
        public static string GamesPath { get; private set; }
        public static string RootFsPath { get; private set; }
        public static string GamesProfilePath { get; private set; }
        public static string SquashFsPath { get; private set; }
        public static string GamesSquashFsPath
        {
            get
            {
                switch (ConfigIni.Instance.ConsoleType)
                {
                    default:
                    case ConsoleType.NES:
                    case ConsoleType.Famicom:
                        return "/usr/share/games/nes/kachikachi";
                    case ConsoleType.SNES_USA:
                    case ConsoleType.SNES_EUR:
                    case ConsoleType.SuperFamicom:
                    case ConsoleType.ShonenJump:
                        return "/usr/share/games";
                    case ConsoleType.MD_ASIA:
                    case ConsoleType.MD_EUR:
                    case ConsoleType.MD_JPN:
                    case ConsoleType.MD_USA:
                        return "";
                }
            }
        }

        public static bool IsSnes(ConsoleType consoleType)
        {
            return (new ConsoleType[] { ConsoleType.SNES_EUR, ConsoleType.SNES_USA, ConsoleType.SuperFamicom }).Contains(consoleType);
        }
        public static bool IsSnes() => IsSnes(ConfigIni.Instance.ConsoleType);

        public static bool IsNes(ConsoleType consoleType)
        {
            return (new ConsoleType[] { ConsoleType.Famicom, ConsoleType.NES, ConsoleType.ShonenJump }).Contains(consoleType);
        }
        public static bool IsNes() => IsNes(ConfigIni.Instance.ConsoleType);

        public static bool IsMdPartitioning { get; set; }

        public static bool IsMd(ConsoleType consoleType)
        {
            return (new ConsoleType[] { ConsoleType.MD_JPN, ConsoleType.MD_USA, ConsoleType.MD_EUR, ConsoleType.MD_ASIA }).Contains(consoleType);
        }
        public static bool IsMd() => IsMd(ConfigIni.Instance.ConsoleType);

        public static bool HasPixelArt(ConsoleType consoleType)
        {
            return consoleType == ConsoleType.NES || consoleType == ConsoleType.Famicom;
        }

        public static string GetDetectedRemoteGameSyncPath()
        {
            if (RemoteGameSyncPath == null)
            {
                throw new NullReferenceException("No valid sync path is available");
            }
            if (ConfigIni.Instance.SeparateGameStorage && SystemCode != null)
            {
                return $"{RemoteGameSyncPath}/{SystemCode}";
            }
            return RemoteGameSyncPath;
        }

        public static string GetRemoteGameSyncPath(ConsoleType consoleType)
        {
            if (RemoteGameSyncPath == null)
            {
                throw new NullReferenceException("No valid sync path is available");
            }

            if (ConfigIni.Instance.SeparateGameStorage)
            {
                if (consoleType == ConsoleType.Unknown)
                    throw new ArgumentException("No valid console type was given");
                return RemoteGameSyncPath + "/" + ConsoleTypeToSystemCode[consoleType];
            }
            return RemoteGameSyncPath;
        }

        public static Version MinimumBootVersion
        {
            get { return new Version(1, 0, 2); }
        }

        public static Version MinimumKernelVersion
        {
            get { return new Version(3, 4, 113); }
        }

        public static Version MinimumScriptVersion
        {
            get { return new Version(1, 0, 4, 118); }
        }

        public static string RawLocalBootVersion
        {
            get; private set;
        }

        public static string RawLocalKernelVersion
        {
            get; private set;
        }

        public static string RawLocalScriptVersion
        {
            get; private set;
        }

        public static Version ConvertVersion(string ver)
        {
            Match m = Regex.Match(ver, @"(?:\d+[\.-]){2,3}(?:\d+)+");
            if (m.Success)
            {
                return new Version(m.Value.Replace("-", "."));
            }
            return new Version(0, 0, 0, 0);
        }

        static hakchi()
        {
            Shell = null;
            clearProperties();
        }

        private static void clearProperties()
        {
            IsMdPartitioning = false;
            Connected = false;
            DetectedConsoleType = null;
            CustomFirmwareLoaded = false;
            RawBootVersion = "";
            RawKernelVersion = "";
            RawScriptVersion = "";
            CanInteract = false;
            CanSync = false;
            MinimalMemboot = false;
            UniqueID = null;
            ConfigPath = "/etc/preinit.d/p0000_config";
            RemoteGameSyncPath = "/var/lib/hakchi/games";
            SystemCode = null;
            MediaPath = "/media";
            OriginalGamesPath = "/usr/share/games";
            GamesPath = "/var/games";
            RootFsPath = "/var/lib/hakchi/rootfs";
            GamesProfilePath = "/var/saves";
            SquashFsPath = "/var/squashfs";
        }

        private static List<ISystemShell> shells = new List<ISystemShell>();

        public static void Initialize()
        {
            // load local version info
            try
            {
                var ver = hakchi.Hmod.GetVersion();
                Trace.WriteLine($"Local hakchi.hmod version info: boot {ver["bootVersion"]}, kernel {ver["kernelVersion"]}, script {ver["hakchiVersion"]}");
                RawLocalBootVersion = ver["bootVersion"];
                RawLocalKernelVersion = ver["kernelVersion"];
                RawLocalScriptVersion = ver["hakchiVersion"];
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }

            // just in case Initialize was ever called before
            if (shells.Any())
            {
                if (Connected)
                    Shell_OnDisconnected();
                Shutdown();
                Thread.Sleep(0);
            }

            // placeholder shell
            shells.Add(new UnknownShell());
            Shell = shells.First();

            // clovershell (for legacy compatibility)
            var clovershell = new ClovershellConnection() { AutoReconnect = true };
            clovershell.OnConnected += Shell_OnConnected;
            clovershell.OnDisconnected += Shell_OnDisconnected;
            shells.Add(clovershell);
            clovershell.Enabled = true;

            // new high-tech but slow SSH connection
            var ssh = new SshClientWrapper(SERVICE_NAME, SERVICE_TYPE, null, null, USERNAME, PASSWORD) { AutoReconnect = true };
            ssh.OnConnected += Shell_OnConnected;
            ssh.OnDisconnected += Shell_OnDisconnected;
            shells.Add(ssh);
            ssh.Enabled = true;
        }

        public static void Shutdown()
        {
            shells.ForEach(shell => shell.Dispose());
            shells.Clear();
            Shell = null;
        }

        public static void Shell_OnDisconnected()
        {
            Shell = shells.First();
            MemoryStats.Clear();
            clearProperties();
            OnDisconnected();

            // reenable all shells
            shells.ForEach(shell => shell.Enabled = true);
        }

        public static void Shell_OnConnected(ISystemShell caller)
        {
            // set calling shell as current used shell and disable others
            Shell = caller;
            shells.ForEach(shell => { if (shell != caller) shell.Enabled = false; });
            try
            {
                if (!Shell.IsOnline)
                {
                    throw new IOException("Shell connection should be online!");
                }

                IsMdPartitioning = Shell.Execute("hakchi ismdPartitioning") == 0;

                DetectedConsoleType = ConsoleType.Unknown;

                MinimalMemboot = Shell.Execute("source /hakchi/config; [ \"$cf_memboot\" = \"y\" ]") == 0;
                UniqueID = Shell.ExecuteSimple("hakchi hwid").Replace(" ", "");
                Trace.WriteLine($"Detected device unique ID: {UniqueID}");

                // execution stops here for a minimal memboot
                if (!MinimalMemboot)
                {
                    var versionExists = Shell.ExecuteSimple("[ -f /var/version ] && echo \"yes\"", 2000, false) == "yes";
                    if (versionExists)
                    {
                        var versions = Shell.ExecuteSimple("source /var/version && echo \"$bootVersion $kernelVersion $hakchiVersion\"", 2000, true).Split(' ');
                        RawBootVersion = versions[0];
                        RawKernelVersion = versions[1];
                        RawScriptVersion = versions[2];
                        Trace.WriteLine($"Detected versions: boot {RawBootVersion}, kernel {RawKernelVersion}, script {RawScriptVersion}");

                        CanInteract = !SystemRequiresReflash() && !SystemRequiresRootfsUpdate();
                    }
                    else
                    {
                        RawBootVersion = "1.0.0";
                        RawKernelVersion = "3.4.112-00";
                        RawScriptVersion = "v1.0.0-000";
                        Trace.WriteLine("Detected versions: severely outdated!");

                        CanInteract = false;
                    }

                    if (CanInteract)
                    {
                        // disable sync on legacy clovershell
                        CanSync = !(caller is ClovershellConnection);

                        // detect console firmware/type
                        SystemCode = Shell.ExecuteSimple("hakchi eval 'echo \"$sftype-$sfregion\"'", 2000, true).Trim();
                        if (SystemCodeToConsoleType.ContainsKey(SystemCode))
                            DetectedConsoleType = SystemCodeToConsoleType[SystemCode];
                        CustomFirmwareLoaded = Shell.ExecuteSimple("hakchi currentFirmware", 2000, true) != "_nand_";

                        // detect basic paths
                        RemoteGameSyncPath = Shell.ExecuteSimple("hakchi findGameSyncStorage", 2000, true).Trim();
                        OriginalGamesPath = Shell.ExecuteSimple("hakchi get gamepath", 2000, true).Trim();
                        RootFsPath = Shell.ExecuteSimple("hakchi get rootfs", 2000, true).Trim();
                        SquashFsPath = Shell.ExecuteSimple("hakchi get squashfs", 2000, true).Trim();

                        // load config
                        ConfigIni.SetConfigDictionary(LoadConfig());

                        // calculate stats
                        MemoryStats.Refresh();
                    }
                }

                // chain to other OnConnected events
                Connected = Shell.IsOnline;
                OnConnected(caller);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
                CanInteract = false;
                MinimalMemboot = false;
            }
        }

        public static bool SystemRequiresReflash()
        {
            try
            {
                return ConvertVersion(RawKernelVersion) < MinimumKernelVersion || ConvertVersion(RawBootVersion) < MinimumBootVersion;
            }
            catch { }
            return true;
        }

        public static bool SystemRequiresRootfsUpdate()
        {
            try
            {
                return ConvertVersion(RawScriptVersion) < MinimumScriptVersion;
            }
            catch { }
            return true;
        }

        public static bool SystemEligibleForRootfsUpdate()
        {
            try
            {
                return ConvertVersion(RawBootVersion) < ConvertVersion(RawLocalBootVersion) || ConvertVersion(RawScriptVersion) < ConvertVersion(RawLocalScriptVersion);
            }
            catch { }
            return true;
        }

        public static int ShowSplashScreen()
        {
            if (Shell.IsOnline)
            {
                Shell.ExecuteSimple("uistop");
                return Shell.Execute("gunzip -c - > /dev/fb0", new MemoryStream(Resources.splash), null, null, 3000);
            }
            return 1;
        }

        public static void RunTemporaryScript(Stream script, string fileName, int timeout = 0, bool throwOnNonZero = false)
        {
            try
            {
                hakchi.Shell.Execute($"cat > /tmp/{fileName}", script, null, null, 5000, throwOnNonZero);
                hakchi.Shell.ExecuteSimple($"chmod +x /tmp/{fileName} && /tmp/{fileName}", timeout, throwOnNonZero);
            }
            finally
            {
                hakchi.Shell.ExecuteSimple($"rm /tmp/{fileName}", 2000, throwOnNonZero);
            }

        }

        public static int UploadFile(Stream stream, string remoteFileName, bool makeExec = true)
        {
            return Shell.Execute(
                command: $"cat > \"{remoteFileName}\"" + (makeExec ? $" && chmod +x \"{remoteFileName}\"" : ""),
                stdin: stream,
                throwOnNonZero: true
            );
        }

        public static int UploadFile(string localFileName, string remoteFileName, bool makeExec = true)
        {
            return UploadFile(File.OpenRead(localFileName), remoteFileName, makeExec);
        }

        public static void SyncConfig(Dictionary<string, string> config, bool reboot = false)
        {
            using (var stream = new MemoryStream())
            {
                if (config != null && config.Count > 0)
                {
                    Trace.WriteLine("Saving p00000_config values");
                    foreach (var key in config.Keys)
                    {
                        var data = Encoding.UTF8.GetBytes(string.Format("cfg_{0}='{1}'\n", key, config[key].Replace(@"'", @"\'")));
                        stream.Write(data, 0, data.Length);
                    }
                }
                Shell.Execute($"hakchi eval", stream, null, null, 3000, true);
            }
            if (reboot)
            {
                try
                {
                    Shell.ExecuteSimple("reboot", 100);
                }
                catch { }
            }
        }

        public static Dictionary<string, string> LoadConfig()
        {
            var config = new Dictionary<string, string>();

            try
            {
                Trace.WriteLine("Reading p0000_config file");
                string configFile;
                using (var stream = new MemoryStream())
                {
                    Shell.Execute($"cat {ConfigPath}", null, stream, null, 2000, true);
                    configFile = Encoding.UTF8.GetString(stream.ToArray());
                }

                if (!string.IsNullOrEmpty(configFile))
                {
                    MatchCollection collection = Regex.Matches(configFile, @"cfg_([^=]+)=(""(?:[^""\\]*(?:\\.[^""\\]*)*)""|\'(?:[^\'\\]*(?:\\.[^\'\\]*)*)\')", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    foreach (Match match in collection)
                    {
                        string param = match.Groups[1].Value;
                        string value = match.Groups[2].Value;
                        value = value.Substring(1, value.Length - 2).Replace("\'", "'").Replace("\\\"", "\"");
                        config[param] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error reading p0000_config file : " + ex.Message + ex.StackTrace);
                config.Clear();
            }
            return config;
        }

        public static string[] GetPackList()
        {
            if (hakchi.Shell.IsOnline && (hakchi.MinimalMemboot || hakchi.CanInteract))
            {
                string[] installedMods;
                bool wasMounted = true;
                if (hakchi.MinimalMemboot)
                {
                    if (hakchi.Shell.Execute("hakchi eval 'mountpoint -q \"$mountpoint/var/lib\"'") != 0)
                    {
                        wasMounted = false;
                        hakchi.Shell.ExecuteSimple("hakchi mount_base");
                    }
                }
                installedMods = hakchi.Shell.ExecuteSimple("hakchi pack_list", 0, true).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (!wasMounted)
                    hakchi.Shell.ExecuteSimple("hakchi umount_base");
                return installedMods;
            }

            return null;
        }

    }
}
