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
        public static MainForm.ConsoleType? DetectedMountedConsoleType { get; private set; }
        public static MainForm.ConsoleType? DetectedConsoleType { get; private set; }
        public static string BootVersion { get; private set; }
        public static string KernelVersion { get; private set; }
        public static string ScriptVersion { get; private set; }
        public static bool CanInteract { get; private set; }

        public static string UniqueID { get; private set; }
        public static string ConfigPath { get; private set; }
        public static string RemoteGameSyncPath { get; private set; }
        public static string SystemCode { get; private set; }
        public static string MediaPath { get; private set; }
        public static string GamesPath { get; private set; }
        public static string GamesProfilePath { get; private set; }
        public static string SquashFsPath { get; private set; }

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

        public static string GetRemoteGameSyncPath(bool? separateGames = null, string overrideSystemCode = null)
        {
            if (separateGames == null) separateGames = ConfigIni.Instance.SeparateGameStorage;
            if ((bool)separateGames)
            {
                if (overrideSystemCode != null)
                {
                    return $"{RemoteGameSyncPath}/{overrideSystemCode}";
                }
                else if(SystemCode != null)
                {
                    return $"{RemoteGameSyncPath}/{SystemCode}";
                }
                return null;
            }
            return RemoteGameSyncPath;
        }

        static hakchi()
        {
            DetectedMountedConsoleType = null;
            DetectedConsoleType = null;
            UniqueID = null;
            SystemCode = null;
            RemoteGameSyncPath = "/var/lib/hakchi/games";
            ConfigPath = "/etc/preinit.d/p0000_config";
            MediaPath = "/media";
            GamesPath = "/var/games";
            GamesProfilePath = "/var/saves";
            SquashFsPath = "/var/squashfs";
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

        public static void Clovershell_OnDisconnected()
        {
            DetectedMountedConsoleType = null;
            DetectedConsoleType = null;
            UniqueID = null;
            SystemCode = null;
            CanInteract = false;
            BootVersion = null;
            KernelVersion = null;
            ScriptVersion = null;
        }

        public static void Clovershell_OnConnected()
        {
            // clear up values
            Clovershell_OnDisconnected();
            try
            {
                var Clovershell = MainForm.Clovershell;
                if (!Clovershell.IsOnline)
                {
                    throw new IOException("Clovershell connection unexpectedly offline");
                }

                // detect running/mounted firmware
                string board = Clovershell.ExecuteSimple("cat /etc/clover/boardtype", 3000, true);
                string region = Clovershell.ExecuteSimple("cat /etc/clover/REGION", 3000, true);
                Debug.WriteLine(string.Format("Detected mounted board: {0}", board));
                Debug.WriteLine(string.Format("Detected mounted region: {0}", region));
                DetectedMountedConsoleType = translateConsoleType(board, region);
                if (DetectedMountedConsoleType == MainForm.ConsoleType.Unknown)
                {
                    throw new IOException("Unable to determine mounted firmware");
                }

                // detect running versions
                var versions = MainForm.Clovershell.ExecuteSimple("source /var/version && echo \"$bootVersion $kernelVersion $hakchiVersion\"", 500, true).Split(' ');
                BootVersion = versions[0];
                KernelVersion = versions[1];
                ScriptVersion = versions[2];
                CanInteract = !SystemRequiresReflash() && !SystemRequiresRootfsUpdate();

                // only do more interaction if safe to do so
                if (CanInteract)
                {
                    // detect root firmware
                    var customFirmwareLoaded = Clovershell.ExecuteSimple("hakchi currentFirmware") != "_nand_";
                    if (customFirmwareLoaded)
                    {
                        Clovershell.ExecuteSimple("cryptsetup open /dev/nandb root-crypt --readonly --type plain --cipher aes-xts-plain --key-file /etc/key-file", 3000);
                        Clovershell.ExecuteSimple("mkdir -p /var/squashfs-original", 3000, true);
                        Clovershell.ExecuteSimple("mount /dev/mapper/root-crypt /var/squashfs-original", 3000, true);
                        board = Clovershell.ExecuteSimple("cat /var/squashfs-original/etc/clover/boardtype", 3000, true);
                        region = Clovershell.ExecuteSimple("cat /var/squashfs-original/etc/clover/REGION", 3000, true);
                        Debug.WriteLine(string.Format("Detected system board: {0}", board));
                        Debug.WriteLine(string.Format("Detected system region: {0}", region));
                        Clovershell.ExecuteSimple("umount /var/squashfs-original", 3000, true);
                        Clovershell.ExecuteSimple("rm -rf /var/squashfs-original", 3000, true);
                        Clovershell.ExecuteSimple("cryptsetup close root-crypt", 3000, true);
                    }
                    DetectedConsoleType = translateConsoleType(board, region);

                    // detect unique id
                    UniqueID = Clovershell.ExecuteSimple("echo \"`devmem 0x01C23800``devmem 0x01C23804``devmem 0x01C23808``devmem 0x01C2380C`\"").Trim().Replace("0x", "");
                    Debug.WriteLine($"Detected device unique ID: {UniqueID}");

                    // detect basic paths
                    RemoteGameSyncPath = MainForm.Clovershell.ExecuteSimple("hakchi findGameSyncStorage", 2000, true).Trim();
                    SystemCode = MainForm.Clovershell.ExecuteSimple("hakchi eval 'echo \"$sftype-$sfregion\"'", 2000, true).Trim();

                    // load config
                    ConfigIni.SetConfigDictionary(LoadConfig());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                CanInteract = false;
            }
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

        public static bool SystemRequiresReflash()
        {
            bool requiresReflash = false;
            try
            {
                string kernelVersion = KernelVersion.Substring(0, KernelVersion.LastIndexOf('.'));
                if (!Shared.IsVersionGreaterOrEqual(kernelVersion, hakchi.MinimumHakchiKernelVersion) ||
                    !Shared.IsVersionGreaterOrEqual(BootVersion, hakchi.MinimumHakchiBootVersion))
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
                string scriptVersion = ScriptVersion.Substring(ScriptVersion.IndexOf('v') + 1);
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
                string scriptVersion = ScriptVersion.Substring(ScriptVersion.IndexOf('v') + 1);
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
