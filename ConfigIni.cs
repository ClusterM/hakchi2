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
                        instance = new ConfigIni();
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

    }
}
