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

        // games collection specific settings
        private class GamesCollectionSetting
        {
            public List<string> SelectedGames = new List<string>();
            public List<string> OriginalGames = new List<string>();
            public string FolderImagesSet = string.Empty;
            public byte MaxGamesPerFolder = 30;
            public NesMenuFolder.Priority BackFolderPosition = NesMenuFolder.Priority.Back;
            public bool HomeFolder = false;
            public NesMenuCollection.SplitStyle FoldersMode = NesMenuCollection.SplitStyle.Original_Auto;
            public Dictionary<string, List<string>> Presets = new Dictionary<string, List<string>>();
        };
        [JsonProperty]
        private Dictionary<hakchi.ConsoleType, GamesCollectionSetting> gamesCollectionSettings;
        [JsonProperty]
        private hakchi.ConsoleType consoleType = hakchi.ConsoleType.NES;
        [JsonIgnore]
        public hakchi.ConsoleType ConsoleType
        {
            get
            {
                return consoleType;
            }
            set
            {
                if (value == hakchi.ConsoleType.Unknown) throw new ArgumentException();
                consoleType = value;
            }
        }
        [JsonIgnore]
        public List<string> SelectedGames
        {
            get { return gamesCollectionSettings[consoleType].SelectedGames; }
        }
        [JsonIgnore]
        public List<string> OriginalGames
        {
            get { return gamesCollectionSettings[consoleType].OriginalGames; }
        }
        [JsonIgnore]
        public string FolderImagesSet
        {
            get { return gamesCollectionSettings[consoleType].FolderImagesSet; }
            set { gamesCollectionSettings[consoleType].FolderImagesSet = value; }
        }
        [JsonIgnore]
        public byte MaxGamesPerFolder
        {
            get { return gamesCollectionSettings[consoleType].MaxGamesPerFolder; }
            set { gamesCollectionSettings[consoleType].MaxGamesPerFolder = value; }
        }
        [JsonIgnore]
        public NesMenuFolder.Priority BackFolderPosition
        {
            get { return gamesCollectionSettings[consoleType].BackFolderPosition; }
            set { gamesCollectionSettings[consoleType].BackFolderPosition = value; }
        }
        [JsonIgnore]
        public bool HomeFolder
        {
            get { return gamesCollectionSettings[consoleType].HomeFolder; }
            set { gamesCollectionSettings[consoleType].HomeFolder = value; }
        }
        [JsonIgnore]
        public NesMenuCollection.SplitStyle FoldersMode
        {
            get { return gamesCollectionSettings[consoleType].FoldersMode; }
            set { gamesCollectionSettings[consoleType].FoldersMode = value; }
        }
        [JsonIgnore]
        public Dictionary<string, List<string>> Presets
        {
            get { return gamesCollectionSettings[consoleType].Presets; }
        }

        // special case methods

        public List<string> SelectedGamesForConsole(hakchi.ConsoleType c)
        {
            return (c == hakchi.ConsoleType.Unknown) ? null : gamesCollectionSettings[c].SelectedGames;
        }

        public List<string> SelectedOriginalGamesForConsole(hakchi.ConsoleType c)
        {
            return (c == hakchi.ConsoleType.Unknown) ? null : gamesCollectionSettings[c].OriginalGames;
        }

        public void AddNewSelectedGame(string code)
        {
            hakchi.ConsoleType[] nes = { hakchi.ConsoleType.Famicom, hakchi.ConsoleType.NES, hakchi.ConsoleType.ShonenJump };
            hakchi.ConsoleType[] snes = { hakchi.ConsoleType.SNES_EUR, hakchi.ConsoleType.SNES_USA, hakchi.ConsoleType.SuperFamicom };
            hakchi.ConsoleType[] selected = null;
            if (!SeparateGameLocalStorage)
                selected = nes.Concat(snes).ToArray();
            else if (nes.Contains(consoleType))
                selected = nes;
            else if (snes.Contains(consoleType))
                selected = snes;
            else
                return;
            foreach (var c in selected)
                if (!gamesCollectionSettings[c].SelectedGames.Contains(code))
                    gamesCollectionSettings[c].SelectedGames.Add(code);
        }

        public void SyncGamesCollectionsStructureSettings()
        {
            foreach (var pair in gamesCollectionSettings)
            {
                if (pair.Key == consoleType) continue;
                pair.Value.FolderImagesSet = FolderImagesSet;
                pair.Value.MaxGamesPerFolder = MaxGamesPerFolder;
                pair.Value.BackFolderPosition = BackFolderPosition;
                pair.Value.HomeFolder = HomeFolder;
                pair.Value.FoldersMode = FoldersMode;
            }
        }

        // base console type settings
        public enum ExtraCmdLineTypes { Kachikachi = 0, Canoe = 1, Retroarch = 2 }
        private class ConsoleSetting
        {
            public bool UseFont = true;
            public byte AntiArmetLevel = 0;
            public bool AutofireHack = false;
            public bool AutofireXYHack = false;
            public byte AutofireInterval = 8;
            public bool FcStart = false;
            public bool ResetHack = true;
            public uint ResetCombination = (uint)(SelectNesButtonsForm.NesButtons.Down | SelectNesButtonsForm.NesButtons.Select);
            public Dictionary<ExtraCmdLineTypes, string> ExtraCommandLineArguments = new Dictionary<ExtraCmdLineTypes, string>()
            {
                { ExtraCmdLineTypes.Kachikachi, "" },
                { ExtraCmdLineTypes.Canoe, "" },
                { ExtraCmdLineTypes.Retroarch, "" }
            };
        };

        [JsonProperty]
        private Dictionary<int, ConsoleSetting> consoleSettings;
        public hakchi.ConsoleType LastConnectedConsoleType = hakchi.ConsoleType.Unknown;

        [JsonIgnore]
        public byte AntiArmetLevel
        {
            get { return consoleSettings[0].AntiArmetLevel; }
            set { consoleSettings[0].AntiArmetLevel = value; }
        }
        [JsonIgnore]
        public bool AutofireHack
        {
            get { return consoleSettings[0].AutofireHack; }
            set { consoleSettings[0].AutofireHack = value; }
        }
        [JsonIgnore]
        public bool AutofireXYHack
        {
            get { return consoleSettings[0].AutofireXYHack; }
            set { consoleSettings[0].AutofireXYHack = value; }
        }
        [JsonIgnore]
        public byte AutofireInterval
        {
            get { return consoleSettings[0].AutofireInterval; }
            set { consoleSettings[0].AutofireInterval = value; }
        }
        [JsonIgnore]
        public bool FcStart
        {
            get { return consoleSettings[0].FcStart; }
            set { consoleSettings[0].FcStart = value; }
        }
        [JsonIgnore]
        public bool ResetHack
        {
            get { return consoleSettings[0].ResetHack; }
            set { consoleSettings[0].ResetHack = value; }
        }
        [JsonIgnore]
        public uint ResetCombination
        {
            get { return consoleSettings[0].ResetCombination; }
            set { consoleSettings[0].ResetCombination = value; }
        }
        [JsonIgnore]
        public Dictionary<ExtraCmdLineTypes, string> ExtraCommandLineArguments
        {
            get { return consoleSettings[0].ExtraCommandLineArguments; }
        }

        // other settings
        public DateTime LastMOTD = default(DateTime);
        public int RunCount = 0;
        public string LastVersion = "0.0.0.0";
        public string Language = "";
        public bool UseSFROMTool = false;
        public bool UsePCMPatch = false;
        public bool SeparateGameLocalStorage = false;
        public bool ConvertToSFROM = true;
        public bool Compress = true;
        public bool CompressCover = true;
        public bool CenterThumbnail = false;
        public bool DisablePopups = false;
        public bool SeparateGameStorage = true;
        public bool SyncLinked = true;
        public bool AlwaysCopyOriginalGames = false;
        public string FtpCommand = @"explorer.exe";
        public string FtpArguments = @"ftp://{0}:{1}@{2}:{3}";
        public string TelnetCommand = @"tools\putty\puttytel.exe";
        public string TelnetArguments = @"telnet://{0}:{1}";
        public MainForm.OriginalGamesPosition OriginalGamesPosition = MainForm.OriginalGamesPosition.AtTop;
        public MainForm.GamesSorting GamesSorting = MainForm.GamesSorting.Name;
        public bool ShowGamesWithoutCoverArt = false;
        public bool ForceClovershell = false;
        public bool ForceNetwork = false;
        public bool ForceSSHTransfers = false;
        public bool UploadToTmp = false;
        public bool ExportLinked = true;
        public string ExportDrive = "";
        public FelLib.Fel.UbootType MembootUboot = FelLib.Fel.UbootType.SD;
        public Hmod.ListSort hmodListSort = Hmod.ListSort.Category;

        // constructor
        private ConfigIni()
        {
            gamesCollectionSettings = new Dictionary<hakchi.ConsoleType, GamesCollectionSetting>();
            foreach (hakchi.ConsoleType c in Enum.GetValues(typeof(hakchi.ConsoleType)))
            {
                if (c != hakchi.ConsoleType.Unknown)
                    gamesCollectionSettings[c] = new GamesCollectionSetting();
            };
            consoleSettings = new Dictionary<int, ConsoleSetting>()
            {
                { 0, new ConsoleSetting() }
            };
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

        public static ConfigIni GetCleanInstance()
        {
            return new ConfigIni();
        }

        // load
        public static bool Load()
        {
            string configPath = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, ConfigFile);
            if (File.Exists(configPath))
            {
                Trace.WriteLine("Loading configuration");
                try
                {
                    instance = JsonConvert.DeserializeObject<ConfigIni>(File.ReadAllText(configPath));
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Unexpected error loading config file : " + ex.Message + ex.StackTrace);
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
                Trace.WriteLine("Saving configuration");
                try
                {
                    string configPath = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, ConfigFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(instance, Formatting.Indented));

                    string legacyConfigPath = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, LegacyConfigFile);
                    if (File.Exists(legacyConfigPath))
                    {
                        Trace.WriteLine("Legacy configuration file will be removed");
                        File.Delete(legacyConfigPath);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Unexpected error saving config file : " + ex.Message + ex.StackTrace);
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
                config["clovercon_autofire_interval"] = instance.AutofireInterval.ToString();
                config["clovercon_fc_start"] = instance.FcStart && (instance.LastConnectedConsoleType == hakchi.ConsoleType.Famicom) ? "1" : "0";
                config["disable_armet"] = instance.AntiArmetLevel > 0 ? "y" : "n";
                config["nes_extra_args"] = instance.ExtraCommandLineArguments[ExtraCmdLineTypes.Kachikachi];
                config["snes_extra_args"] = instance.ExtraCommandLineArguments[ExtraCmdLineTypes.Canoe];
                config["retroarch_extra_args"] = instance.ExtraCommandLineArguments[ExtraCmdLineTypes.Retroarch];
            }
            return config;
        }
        public static void SetConfigDictionary(Dictionary<string, string> config)
        {
            foreach (var setting in config)
            {
                switch (setting.Key)
                {
                    case "clovercon_home_combination":
                        instance.ResetHack = setting.Value != "0x7FFF";
                        if (instance.ResetHack)
                        {
                            instance.ResetCombination = Convert.ToUInt32(setting.Value.Substring(2), 16);
                        }
                        break;
                    case "clovercon_autofire":
                        instance.AutofireHack = setting.Value == "1";
                        break;
                    case "clovercon_autofire_xy":
                        instance.AutofireXYHack = setting.Value == "1";
                        break;
                    case "clovercon_autofire_interval":
                        instance.AutofireInterval = byte.Parse(setting.Value);
                        break;
                    case "clovercon_fc_start":
                        instance.FcStart = setting.Value == "1";
                        break;
                    case "disable_armet":
                        instance.AntiArmetLevel = setting.Value == "y" ? (byte)2 : (byte)0;
                        break;
                    case "nes_extra_args":
                        instance.ExtraCommandLineArguments[ExtraCmdLineTypes.Kachikachi] = setting.Value;
                        break;
                    case "snes_extra_args":
                        instance.ExtraCommandLineArguments[ExtraCmdLineTypes.Canoe] = setting.Value;
                        break;
                    case "retroarch_extra_args":
                        instance.ExtraCommandLineArguments[ExtraCmdLineTypes.Retroarch] = setting.Value;
                        break;
                    default:
                        // ignore other settings (for now)
                        break;
                }
            }
        }

        // legacy loading code for transition
        public static bool LoadLegacy()
        {
            var fileName = Shared.PathCombine(Program.BaseDirectoryExternal, ConfigDir, LegacyConfigFile);
            if (File.Exists(fileName))
            {
                try
                {
                    Trace.WriteLine("Loading legacy configuration file");
                    instance = new ConfigIni();

                    var configLines = File.ReadAllLines(fileName);
                    List<string> list;
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
                                    case "selectedgames":
                                    case "selectedgamesnes":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.NES].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        if (instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].SelectedGames.Count == 0)
                                            instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].SelectedGames = instance.gamesCollectionSettings[hakchi.ConsoleType.NES].SelectedGames.ToList();
                                        break;
                                    case "selectedgamesfamicom":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "selectedgamessnes":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_EUR].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_USA].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        if (instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].SelectedGames.Count == 0)
                                            instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].SelectedGames = instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_EUR].SelectedGames.ToList();
                                        break;
                                    case "selectedgamessuperfamicom":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].SelectedGames = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        break;
                                    case "hiddengames":
                                        list = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.NES].OriginalGames = NesApplication.DefaultGames[hakchi.ConsoleType.NES].Where(code => !list.Contains(code)).ToList();
                                        break;
                                    case "hiddengamesfamicom":
                                        list = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].OriginalGames = NesApplication.DefaultGames[hakchi.ConsoleType.Famicom].Where(code => !list.Contains(code)).ToList();
                                        break;
                                    case "hiddengamessnes":
                                        list = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_EUR].OriginalGames = NesApplication.DefaultGames[hakchi.ConsoleType.SNES_EUR].Where(code => !list.Contains(code)).ToList();
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_USA].OriginalGames = NesApplication.DefaultGames[hakchi.ConsoleType.SNES_USA].Where(code => !list.Contains(code)).ToList();
                                        break;
                                    case "hiddengamessuperfamicom":
                                        list = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].OriginalGames = NesApplication.DefaultGames[hakchi.ConsoleType.SuperFamicom].Where(code => !list.Contains(code)).ToList();
                                        break;
                                    case "originalgamesposition":
                                        instance.OriginalGamesPosition = (MainForm.OriginalGamesPosition)byte.Parse(value);
                                        break;
                                    case "groupgamesbyapptype":
                                        instance.GamesSorting = !value.ToLower().Equals("false") ? MainForm.GamesSorting.System : MainForm.GamesSorting.Name;
                                        break;
                                    case "customflashednes":
                                    case "customflashedfamicom":
                                    case "customflashedsnes":
                                    case "customflashedsuperfamicom":
                                    case "usefont":
                                    case "usefontfamicom":
                                    case "usefontsnes":
                                    case "usefontsuperfamicom":
                                    case "antiarmetlevel":
                                    case "resethack":
                                    case "cloverconhack":
                                    case "resethacksnes":
                                    case "autofirehack":
                                    case "autofirehacksnes":
                                    case "autofirexyhack":
                                    case "resetcombination":
                                    case "resetcombinationsnes":
                                    case "usbhostnes":
                                    case "usbhostfamicom":
                                    case "usbhostsnes":
                                    case "usbhostsuperfamicom":
                                        // ignoring these settings, using defaults
                                        break;
                                    case "runcount":
                                        instance.RunCount = int.Parse(value);
                                        break;
                                    case "consoletype":
                                        instance.ConsoleType = (hakchi.ConsoleType)byte.Parse(value);
                                        if (instance.ConsoleType == hakchi.ConsoleType.SNES_USA) // compensate for new 5th console type
                                            instance.ConsoleType = hakchi.ConsoleType.SuperFamicom;
                                        break;
                                    case "extracommandlinearguments":
                                        instance.consoleSettings[0].ExtraCommandLineArguments[ExtraCmdLineTypes.Kachikachi] = value;
                                        break;
                                    case "extracommandlineargumentssnes":
                                        instance.consoleSettings[0].ExtraCommandLineArguments[ExtraCmdLineTypes.Canoe] = value;
                                        break;
                                    case "fcstart":
                                        instance.consoleSettings[0].FcStart = !value.ToLower().Equals("false");
                                        break;
                                    case "maxgamesperfolder":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.NES].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "maxgamesperfolderfamicom":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "maxgamesperfoldersnes":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_EUR].MaxGamesPerFolder =
                                            instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_USA].MaxGamesPerFolder =
                                            byte.Parse(value);
                                        break;
                                    case "maxgamesperfoldersuperfamicom":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].MaxGamesPerFolder = byte.Parse(value);
                                        break;
                                    case "foldersmode":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.NES].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "foldersmodefamicom":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "foldersmodesnes":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_EUR].FoldersMode =
                                            instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_USA].FoldersMode =
                                            (NesMenuCollection.SplitStyle)byte.Parse(value);
                                        break;
                                    case "foldersmodesuperfamicom":
                                        instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
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
                                        //instance.FtpServer = !value.ToLower().Equals("false");
                                        break;
                                    case "ftpcommand":
                                        instance.FtpCommand = value;
                                        break;
                                    case "ftparguments":
                                        instance.FtpArguments = value;
                                        break;
                                    case "telnetserver":
                                        //instance.TelnetServer = !value.ToLower().Equals("false");
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
                                        //instance.ExportRegion = value;
                                        break;
                                    case "membootuboot":
                                        if (value == "uboot.bin")
                                        {
                                            instance.MembootUboot = FelLib.Fel.UbootType.Normal;
                                        }
                                        else
                                        {
                                            instance.MembootUboot = FelLib.Fel.UbootType.SD;
                                        }
                                        break;
                                    case "hmodlistsort":
                                        instance.hmodListSort = (Hmod.ListSort)byte.Parse(value);
                                        break;
                                }
                                break;
                            case "presets":
                                instance.gamesCollectionSettings[hakchi.ConsoleType.NES].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesCollectionSettings[hakchi.ConsoleType.Famicom].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_EUR].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesCollectionSettings[hakchi.ConsoleType.SNES_USA].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                instance.gamesCollectionSettings[hakchi.ConsoleType.SuperFamicom].Presets[param] = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                break;
                        }
                    }

                    // cleanup
                    instance.gamesCollectionSettings.ToList().ForEach(pair => pair.Value.SelectedGames.Remove("default"));

                    // success
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error loading legacy configuration file : " + ex.Message + ex.StackTrace);
                    instance = null;
                }
            }
            // failure
            return false;
        }

    }
}
