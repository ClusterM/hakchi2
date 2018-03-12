using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace com.clusterrr.hakchi_gui
{
    public class ConfigIni
    {
        // constants
        public const string ConfigDir = "config";
        public const string ConfigFile = "config.json";
        public const string LegacyConfigFile = "config.ini";

        // game collection specific settings
        private class GamesConsoleSetting
        {
            public List<string> SelectedGames = new List<string>();
            public List<string> HiddenGames = new List<string>();
            public byte MaxGamesPerFolder = 30;
            public NesMenuCollection.SplitStyle FoldersMode = NesMenuCollection.SplitStyle.Original_Auto;
            public Dictionary<string, List<string>> Presets = new Dictionary<string, List<string>>();
        };
        [JsonProperty]
        private Dictionary<MainForm.ConsoleType, GamesConsoleSetting> gamesConsoleSettings;
        [JsonProperty]
        private MainForm.ConsoleType gamesConsoleType = MainForm.ConsoleType.NES;
        [JsonIgnore]
        public MainForm.ConsoleType GamesConsoleType
        {
            get
            {
                return gamesConsoleType;
            }
            set
            {
                if (value == MainForm.ConsoleType.Unknown) throw new ArgumentException();
                gamesConsoleType = value;
            }
        }
        [JsonIgnore]
        public ICollection<string> SelectedGames
        {
            get { return gamesConsoleSettings[gamesConsoleType].SelectedGames; }
        }
        [JsonIgnore]
        public ICollection<string> HiddenGames
        {
            get { return gamesConsoleSettings[gamesConsoleType].HiddenGames; }
        }
        [JsonIgnore]
        public byte MaxGamesPerFolder
        {
            get { return gamesConsoleSettings[gamesConsoleType].MaxGamesPerFolder; }
            set { gamesConsoleSettings[gamesConsoleType].MaxGamesPerFolder = value; }
        }
        [JsonIgnore]
        public NesMenuCollection.SplitStyle FoldersMode
        {
            get { return gamesConsoleSettings[gamesConsoleType].FoldersMode; }
            set { gamesConsoleSettings[gamesConsoleType].FoldersMode = value; }
        }
        [JsonIgnore]
        public Dictionary<string, List<string>> Presets
        {
            get { return gamesConsoleSettings[gamesConsoleType].Presets; }
        }

        // special case method
        public ICollection<string> SelectedGamesForConsole(MainForm.ConsoleType c)
        {
            return (c == MainForm.ConsoleType.Unknown) ? null : gamesConsoleSettings[c].SelectedGames;
        }

        // base console type settings
        private class ConsoleSetting
        {
            public bool CustomFlashed = false;
            public bool UsbHost = true;
            public bool UseFont = true;
            public byte AntiArmetLevel = 0;
            public bool AutofireHack = false;
            public bool AutofireXYHack = false;
            public bool ResetHack = true;
            public uint ResetCombination = (uint)(SelectNesButtonsForm.NesButtons.Down | SelectNesButtonsForm.NesButtons.Select);
            public string ExtraCommandLineArguments = "";
        };
        [JsonProperty]
        private Dictionary<MainForm.ConsoleType, ConsoleSetting> consoleSettings;
        [JsonProperty]
        private MainForm.ConsoleType consoleType = MainForm.ConsoleType.Unknown;
        [JsonIgnore]
        public MainForm.ConsoleType ConsoleType
        {
            get { return consoleType; }
            set { consoleType = value; }
        }
        [JsonIgnore]
        public bool CustomFlashed
        {
            get { return consoleSettings[consoleType].CustomFlashed; }
            set { consoleSettings[consoleType].CustomFlashed = value; }
        }
        [JsonIgnore]
        public bool UsbHost
        {
            get { return consoleSettings[consoleType].UsbHost; }
            set { consoleSettings[consoleType].UsbHost = value; }
        }
        [JsonIgnore]
        public bool UseFont
        {
            get { return consoleSettings[consoleType].UseFont; }
            set { consoleSettings[consoleType].UseFont = value; }
        }
        [JsonIgnore]
        public byte AntiArmetLevel
        {
            get { return consoleSettings[consoleType].AntiArmetLevel; }
            set { consoleSettings[consoleType].AntiArmetLevel = value; }
        }
        [JsonIgnore]
        public bool AutofireHack
        {
            get { return consoleSettings[consoleType].AutofireHack; }
            set { consoleSettings[consoleType].AutofireHack = value; }
        }
        [JsonIgnore]
        public bool AutofireXYHack
        {
            get { return consoleSettings[consoleType].AutofireXYHack; }
            set { consoleSettings[consoleType].AutofireXYHack = value; }
        }
        public bool FcStart = false;
        [JsonIgnore]
        public bool ResetHack
        {
            get { return consoleSettings[consoleType].ResetHack; }
            set { consoleSettings[consoleType].ResetHack = value; }
        }
        [JsonIgnore]
        public uint ResetCombination
        {
            get { return consoleSettings[consoleType].ResetCombination; }
            set { consoleSettings[consoleType].ResetCombination = value; }
        }
        [JsonIgnore]
        public string ExtraCommandLineArguments
        {
            get { return consoleSettings[consoleType].ExtraCommandLineArguments; }
            set { consoleSettings[consoleType].ExtraCommandLineArguments = value; }
        }

        // other settings
        public int RunCount = 0;
        public string LastVersion = "0.0.0.0";
        public string Language = "";
        public bool UseSFROMTool = false;
        public bool UsePCMPatch = false;
        public bool Compress = true;
        public bool CompressCover = true;
        public bool CenterThumbnail = false;
        public bool DisablePopups = false;
        public bool SeparateGameStorage = true;
        public bool SyncLinked = false;
        public bool FtpServer = false;
        public bool TelnetServer = false;
        public string FtpCommand = "ftp://{0}:{1}@{2}:{3}";
        public string FtpArguments = "";
        public string TelnetCommand = "telnet://{0}:{1}";
        public string TelnetArguments = "";
        public MainForm.OriginalGamesPosition OriginalGamesPosition = MainForm.OriginalGamesPosition.AtTop;
        public bool GroupGamesByAppType = false;
        public bool ExportLinked = true;
        public string ExportRegion = "";
        public string MembootUboot = "ubootSD.bin";
        public HmodListSort hmodListSort = HmodListSort.Category;

        // constructor
        private ConfigIni()
        {
            gamesConsoleSettings = new Dictionary<MainForm.ConsoleType, GamesConsoleSetting>();
            foreach (MainForm.ConsoleType c in Enum.GetValues(typeof(MainForm.ConsoleType)))
            {
                if (c == MainForm.ConsoleType.Unknown) continue;
                gamesConsoleSettings[c] = new GamesConsoleSetting();
            };

            consoleSettings = new Dictionary<MainForm.ConsoleType, ConsoleSetting>();
            foreach (MainForm.ConsoleType c in Enum.GetValues(typeof(MainForm.ConsoleType)))
            {
                consoleSettings[c] = new ConsoleSetting();
            }
        }

        // instance
        private static ConfigIni instance = null;
        public static ConfigIni Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!Load())
                    {
                        if (!LoadLegacy())
                        {
                            instance = new ConfigIni();
                        }
                    }
                }
                return instance;
            }
        }

        // load
        public static bool Load()
        {
            string configPath = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, ConfigFile);
            if (File.Exists(configPath))
            {
                Debug.WriteLine("Loading configuration");
                try
                {
                    instance = JsonConvert.DeserializeObject<ConfigIni>(File.ReadAllText(configPath));
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unexpected error loading config file : " + ex.Message + ex.StackTrace);
                    instance = null;
                }
            }
            return false;
        }

        // save
        public static void Save()
        {
            if(instance != null)
            {
                Debug.WriteLine("Saving configuration");
                instance.LastVersion = Shared.AppVersion.ToString();
                try
                {
                    string configPath = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, ConfigFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(instance, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unexpected error saving config file : " + ex.Message + ex.StackTrace);
                }
            }
        }

        // config dictionary
        public static Dictionary<string, string> GetConfigDictionary()
        {
            var config = new Dictionary<string, string>();
            if (instance != null)
            {
                config["clovercon_home_combination"] = instance.ResetHack ? string.Format("0x{0:X4}", instance.ResetCombination) : "0x7FFF";
                config["clovercon_autofire"] = instance.AutofireHack ? "1" : "0";
                config["clovercon_autofire_xy"] = instance.AutofireXYHack ? "1" : "0";
                config["clovercon_fc_start"] = instance.FcStart && (instance.ConsoleType == MainForm.ConsoleType.Famicom) ? "1" : "0";
                config["fontfix_enabled"] = instance.UseFont ? "y" : "n";
                config["disable_armet"] = instance.AntiArmetLevel > 0 ? "y" : "n";
                config["nes_extra_args"] = (instance.ConsoleType == MainForm.ConsoleType.Famicom || instance.ConsoleType == MainForm.ConsoleType.NES) ? instance.ExtraCommandLineArguments : "";
                config["snes_extra_args"] = (instance.ConsoleType == MainForm.ConsoleType.SuperFamicom || instance.ConsoleType == MainForm.ConsoleType.SNES) ? instance.ExtraCommandLineArguments : "";
                config["usb_host"] = instance.UsbHost ? "y" : "n";
            }
            return config;
        }

        // legacy loading code for transition
        public static bool LoadLegacy()
        {
            var fileName = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, LegacyConfigFile);
            if (File.Exists(fileName))
            {
                try
                {
                    Debug.WriteLine("Loading legacy configuration file");
                    instance = new ConfigIni();

                    var configLines = File.ReadAllLines(fileName);
                    string section = "";
                    foreach (var line in configLines)
                    {
                        var l = line.Trim();
                        if (l.StartsWith("[") && l.EndsWith("]"))
                            section = l.Substring(1, l.Length - 2).ToLower();
                        int pos = l.IndexOf('=');
                        if (pos <= 0) continue;
                        var param = l.Substring(0, pos).Trim();
                        var value = l.Substring(pos + 1).Trim();
                        switch (section)
                        {
                            case "config":
                                param = param.ToLower();
                                switch (param)
                                {
                                    case "lastversion":
                                        instance.LastVersion = value;
                                        break;
                                    case "language":
                                        instance.Language = value;
                                        break;
                                    case "selectedgamesnes":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.NES].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "selectedgamesfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.Famicom].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "selectedgamessnes":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SNES].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "selectedgamessuperfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SuperFamicom].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "originalgamesposition":
                                        instance.OriginalGamesPosition = (MainForm.OriginalGamesPosition)byte.Parse(value);
                                        break;
                                    case "groupgamesbyapptype":
                                        instance.GroupGamesByAppType = !value.ToLower().Equals("false");
                                        break;
                                    case "hiddengames":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.NES].HiddenGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "hiddengamesfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.Famicom].HiddenGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "hiddengamessnes":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SNES].HiddenGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "hiddengamessuperfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SuperFamicom].HiddenGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "customflashednes":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].CustomFlashed = !value.ToLower().Equals("false");
                                        break;
                                    case "customflashedfamicom":
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].CustomFlashed = !value.ToLower().Equals("false");
                                        break;
                                    case "customflashedsnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].CustomFlashed = !value.ToLower().Equals("false");
                                        break;
                                    case "customflashedsuperfamicom":
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].CustomFlashed = !value.ToLower().Equals("false");
                                        break;
                                    case "usefont":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].UseFont = !value.ToLower().Equals("false");
                                        break;
                                    case "usefontfamicom":
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].UseFont = !value.ToLower().Equals("false");
                                        break;
                                    case "usefontsnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].UseFont = !value.ToLower().Equals("false");
                                        break;
                                    case "usefontsuperfamicom":
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].UseFont = !value.ToLower().Equals("false");
                                        break;
                                    case "runcount":
                                        instance.RunCount = int.Parse(value);
                                        break;
                                    case "antiarmetlevel":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].AntiArmetLevel = byte.Parse(value);
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].AntiArmetLevel = byte.Parse(value);
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].AntiArmetLevel = byte.Parse(value);
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].AntiArmetLevel = byte.Parse(value);
                                        break;
                                    case "resethack":
                                    case "cloverconhack":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].ResetHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].ResetHack = !value.ToLower().Equals("false");
                                        break;
                                    case "resethacksnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].ResetHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].ResetHack = !value.ToLower().Equals("false");
                                        break;
                                    case "autofirehack":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].AutofireHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].AutofireHack = !value.ToLower().Equals("false");
                                        break;
                                    case "autofirehacksnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].AutofireHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].AutofireHack = !value.ToLower().Equals("false");
                                        break;
                                    case "autofirexyhack":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].AutofireXYHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].AutofireXYHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].AutofireXYHack = !value.ToLower().Equals("false");
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].AutofireXYHack = !value.ToLower().Equals("false");
                                        break;
                                    case "resetcombination":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].ResetCombination = uint.Parse(value);
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].ResetCombination = uint.Parse(value);
                                        break;
                                    case "resetcombinationsnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].ResetCombination = uint.Parse(value);
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].ResetCombination = uint.Parse(value);
                                        break;
                                    case "consoletype":
                                        instance.ConsoleType = (MainForm.ConsoleType)byte.Parse(value);
                                        break;
                                    case "extracommandlinearguments":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].ExtraCommandLineArguments = value;
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].ExtraCommandLineArguments = value;
                                        break;
                                    case "extracommandlineargumentssnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].ExtraCommandLineArguments = value;
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].ExtraCommandLineArguments = value;
                                        break;
                                    case "fcstart":
                                        instance.FcStart = !value.ToLower().Equals("false");
                                        break;
                                    case "maxgamesperfolder":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.NES].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "maxgamesperfolderfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.Famicom].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "maxgamesperfoldersnes":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SNES].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "maxgamesperfoldersuperfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SuperFamicom].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "foldersmode":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.NES].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "foldersmodefamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.Famicom].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "foldersmodesnes":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SNES].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "foldersmodesuperfamicom":
                                        instance.gamesConsoleSettings[MainForm.ConsoleType.SuperFamicom].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "usesfromtool":
                                        instance.UseSFROMTool = !value.ToLower().Equals("false");
                                        break;
                                    case "usepcmpatch":
                                        instance.UsePCMPatch = !value.ToLower().Equals("false");
                                        break;
                                    case "compress":
                                        instance.Compress = !value.ToLower().Equals("false");
                                        break;
                                    case "compresscover":
                                        instance.CompressCover = !value.ToLower().Equals("false");
                                        break;
                                    case "centerthumbnail":
                                        instance.CenterThumbnail = !value.ToLower().Equals("false");
                                        break;
                                    case "disablepopups":
                                        instance.DisablePopups = !value.ToLower().Equals("false");
                                        break;
                                    case "ftpserver":
                                        instance.FtpServer = !value.ToLower().Equals("false");
                                        break;
                                    case "usbhostnes":
                                        instance.consoleSettings[MainForm.ConsoleType.NES].UsbHost = !value.ToLower().Equals("false");
                                        break;
                                    case "usbhostfamicom":
                                        instance.consoleSettings[MainForm.ConsoleType.Famicom].UsbHost = !value.ToLower().Equals("false");
                                        break;
                                    case "usbhostsnes":
                                        instance.consoleSettings[MainForm.ConsoleType.SNES].UsbHost = !value.ToLower().Equals("false");
                                        break;
                                    case "usbhostsuperfamicom":
                                        instance.consoleSettings[MainForm.ConsoleType.SuperFamicom].UsbHost = !value.ToLower().Equals("false");
                                        break;
                                    case "ftpcommand":
                                        instance.FtpCommand = value;
                                        break;
                                    case "ftparguments":
                                        instance.FtpArguments = value;
                                        break;
                                    case "telnetserver":
                                        instance.TelnetServer = !value.ToLower().Equals("false");
                                        break;
                                    case "telnetcommand":
                                        instance.TelnetCommand = value;
                                        break;
                                    case "telnetarguments":
                                        instance.TelnetArguments = value;
                                        break;
                                    case "separategamestorage":
                                        instance.SeparateGameStorage = !value.ToLower().Equals("false");
                                        break;
                                    case "exportlinked":
                                        instance.ExportLinked = !value.ToLower().Equals("false");
                                        break;
                                    case "synclinked":
                                        instance.SyncLinked = !value.ToLower().Equals("false");
                                        break;
                                    case "exportregion":
                                        instance.ExportRegion = value;
                                        break;
                                    case "membootuboot":
                                        instance.MembootUboot = value;
                                        break;
                                    case "hmodlistsort":
                                        instance.hmodListSort = (HmodListSort)byte.Parse(value);
                                        break;
                                }
                                break;
                            case "presets":
                                instance.gamesConsoleSettings[MainForm.ConsoleType.NES].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesConsoleSettings[MainForm.ConsoleType.Famicom].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesConsoleSettings[MainForm.ConsoleType.SNES].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesConsoleSettings[MainForm.ConsoleType.SuperFamicom].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                break;
                        }
                    }
                    // success
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error loading legacy configuration file : " + ex.Message + ex.StackTrace);
                    instance = null;
                }
            }
            // failure
            return false;
        }

    }
}
