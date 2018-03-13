using com.clusterrr.clovershell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public static class hakchi
    {
        public static MainForm.ConsoleType? DetectedMountedConsoleType = null;
        public static MainForm.ConsoleType? DetectedConsoleType = null;
        public static string ConfigPath = "/etc/preinit.d/p0000_config";
        public static string MediaPath = "/media";
        public static string GamesPath = "/var/games";
        public static string GamesProfilePath = "/var/saves";
        public static string SquashFsPath = "/var/squashfs";
        public static string GamesSquashFsPath
        {
            get
            {
                switch (ConfigIni.Instance.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return "/usr/share/games/nes/kachikachi";
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return "/usr/share/games";
                }
            }
        }

        private static string remoteGameSyncPath = null;
        private static string systemCode;
        public static string RemoteGameSyncPath
        {
            get
            {
                if (!MainForm.Clovershell.IsOnline)
                    throw new IOException("Clovershell is offline");

                if (remoteGameSyncPath == null)
                {
                    remoteGameSyncPath = MainForm.Clovershell.ExecuteSimple("hakchi findGameSyncStorage", 2000, true).Trim();
                    systemCode = null;
                }
                if (ConfigIni.Instance.SeparateGameStorage)
                {
                    if (systemCode == null)
                    {
                        systemCode = MainForm.Clovershell.ExecuteSimple("hakchi eval 'echo \"$sftype-$sfregion\"'", 2000, true).Trim();
                    }
                    return $"{remoteGameSyncPath}/{systemCode}";
                }
                return remoteGameSyncPath;
            }
        }

        public static string MinimumHakchiBootVersion
        {
            get { return "1.0.1"; }
        }

        public static string MinimumHakchiKernelVersion
        {
            get { return "3.4.112"; }
        }

        public static string MinimumHakchiScriptVersion
        {
            get { return "1.0.3"; }
        }

        public static string MinimumHakchiScriptRevision
        {
            get { return "110"; }
        }

        public static string CurrentHakchiScriptVersion
        {
            get { return "1.0.3"; }
        }

        public static string CurrentHakchiScriptRevision
        {
            get { return "110"; }
        }

        public static bool SystemRequiresReflash()
        {
            bool requiresReflash = false;

            try
            {
                var bootVersion = MainForm.Clovershell.ExecuteSimple("source /var/version && echo $bootVersion", 500, true);
                var kernelVersion = MainForm.Clovershell.ExecuteSimple("source /var/version && echo $kernelVersion", 500, true);
                kernelVersion = kernelVersion.Substring(0, kernelVersion.LastIndexOf('.'));

                if (!Shared.IsVersionGreaterOrEqual(kernelVersion, hakchi.MinimumHakchiKernelVersion) ||
                    !Shared.IsVersionGreaterOrEqual(bootVersion, hakchi.MinimumHakchiBootVersion))
                {
                    requiresReflash = true;
                }
            }
            catch
            {
                requiresReflash = true;
            }

            return requiresReflash;
        }

        public static bool SystemRequiresRootfsUpdate()
        {
            bool requiresUpdate = false;

            try
            {
                var scriptVersion = MainForm.Clovershell.ExecuteSimple("source /var/version && echo $hakchiVersion", 500, true);
                scriptVersion = scriptVersion.Substring(scriptVersion.IndexOf('v') + 1);
                scriptVersion = scriptVersion.Substring(0, scriptVersion.LastIndexOf('('));

                var scriptElems = scriptVersion.Split(new char[] { '-' });

                if (!Shared.IsVersionGreaterOrEqual(scriptElems[0], hakchi.MinimumHakchiScriptVersion) ||
                    !(int.Parse(scriptElems[1]) >= int.Parse(hakchi.MinimumHakchiScriptRevision)))
                {
                    requiresUpdate = true;
                }
            }
            catch
            {
                requiresUpdate = true;
            }

            return requiresUpdate;
        }

        public static bool SystemEligibleForRootfsUpdate()
        {
            bool eligibleForUpdate = false;

            try
            {
                var scriptVersion = MainForm.Clovershell.ExecuteSimple("source /var/version && echo $hakchiVersion", 500, true);
                scriptVersion = scriptVersion.Substring(scriptVersion.IndexOf('v') + 1);
                scriptVersion = scriptVersion.Substring(0, scriptVersion.LastIndexOf('('));

                var scriptElems = scriptVersion.Split(new char[] { '-' });

                if (!Shared.IsVersionGreaterOrEqual(scriptElems[0], hakchi.CurrentHakchiScriptVersion) ||
                    !(int.Parse(scriptElems[1]) >= int.Parse(hakchi.CurrentHakchiScriptRevision)))
                {
                    eligibleForUpdate = true;
                }
            }
            catch
            {
                eligibleForUpdate = true;
            }

            return eligibleForUpdate;
        }

        private static MainForm.ConsoleType translateConsoleType(string board, string region)
        {
            switch (board)
            {
                default:
                case "dp-nes":
                case "dp-hvc":
                    switch (region)
                    {
                        case "EUR_USA":
                            return MainForm.ConsoleType.NES;
                        case "JPN":
                            return MainForm.ConsoleType.Famicom;
                    }
                    break;
                case "dp-shvc":
                    switch (region)
                    {
                        case "USA":
                        case "EUR":
                            return MainForm.ConsoleType.SNES;
                        case "JPN":
                            return MainForm.ConsoleType.SuperFamicom;
                    }
                    break;
            }
            return MainForm.ConsoleType.Unknown;
        }

        public static void Clovershell_OnConnected()
        {
            DetectedMountedConsoleType = null;
            DetectedConsoleType = null;

            if (MainForm.Clovershell.IsOnline)
            {
                var customFirmwareLoaded = MainForm.Clovershell.ExecuteSimple("hakchi currentFirmware") != "_nand_";
                string board = MainForm.Clovershell.ExecuteSimple("cat /etc/clover/boardtype", 3000, true);
                string region = MainForm.Clovershell.ExecuteSimple("cat /etc/clover/REGION", 3000, true);
                DetectedMountedConsoleType = translateConsoleType(board, region);

                Debug.WriteLine(string.Format("Detected board: {0}", board));
                Debug.WriteLine(string.Format("Detected region: {0}", region));

                if (customFirmwareLoaded)
                {
                    MainForm.Clovershell.ExecuteSimple("cryptsetup open /dev/nandb root-crypt --readonly --type plain --cipher aes-xts-plain --key-file /etc/key-file", 3000);
                    MainForm.Clovershell.ExecuteSimple("mkdir -p /var/squashfs-original", 3000, true);
                    MainForm.Clovershell.ExecuteSimple("mount /dev/mapper/root-crypt /var/squashfs-original", 3000, true);
                    board = MainForm.Clovershell.ExecuteSimple("cat /var/squashfs-original/etc/clover/boardtype", 3000, true);
                    region = MainForm.Clovershell.ExecuteSimple("cat /var/squashfs-original/etc/clover/REGION", 3000, true);
                    MainForm.Clovershell.ExecuteSimple("umount /var/squashfs-original", 3000, true);
                    MainForm.Clovershell.ExecuteSimple("rm -rf /var/squashfs-original", 3000, true);
                    MainForm.Clovershell.ExecuteSimple("cryptsetup close root-crypt", 3000, true);
                }
                DetectedConsoleType = translateConsoleType(board, region);

                ConfigIni.SetConfigDictionary(LoadConfig());
            }
        }

        public static void Clovershell_OnDisconnected()
        {
            DetectedMountedConsoleType = null;
            DetectedConsoleType = null;
            remoteGameSyncPath = null;
        }

        public static void ShowSplashScreen()
        {
            var splashScreenPath = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "data"), "splash.gz");
            MainForm.Clovershell.ExecuteSimple("uistop");
            if (File.Exists(splashScreenPath))
            {
                using (var splash = new FileStream(splashScreenPath, FileMode.Open))
                {
                    MainForm.Clovershell.Execute("gunzip -c - > /dev/fb0", splash, null, null, 3000);
                }
            }
        }

        public static void SyncConfig(Dictionary<string, string> config, bool reboot = false)
        {
            using (var stream = new MemoryStream())
            {
                if (config != null && config.Count > 0)
                {
                    Debug.WriteLine("Saving p00000_config values");
                    foreach (var key in config.Keys)
                    {
                        var data = Encoding.UTF8.GetBytes(string.Format("cfg_{0}='{1}'\n", key, config[key].Replace(@"'", @"\'")));
                        stream.Write(data, 0, data.Length);
                    }
                }
                MainForm.Clovershell.Execute($"hakchi eval", stream, null, null, 3000, true);
            }
            if (reboot)
            {
                try
                {
                    MainForm.Clovershell.ExecuteSimple("reboot", 100);
                }
                catch { }
            }
        }

        public static Dictionary<string, string> LoadConfig()
        {
            var config = new Dictionary<string, string>();

            try
            {
                Debug.WriteLine("Reading p0000_config file");
                string configFile;
                using (var stream = new MemoryStream())
                {
                    MainForm.Clovershell.Execute($"cat {ConfigPath}", null, stream, null, 2000, true);
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
            catch(Exception ex)
            {
                Debug.WriteLine("Error reading p0000_config file : " + ex.Message + ex.StackTrace);
                config.Clear();
            }
            return config;
        }

    }
}
