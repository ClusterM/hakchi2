using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class ConfigIni
    {
        public static int RunCount = 0;
        public static string SelectedGamesNes = "";
        public static string SelectedGamesFamicom = "";
        public static string SelectedGamesSnes = "";
        public static string SelectedGamesSuperFamicom = "";
        public static MainForm.OriginalGamesPosition OriginalGamesPosition = MainForm.OriginalGamesPosition.AtTop;
        //public static string HiddenGamesNes = "";
        //public static string HiddenGamesFamicom = "";
        //public static string HiddenGamesSnes = "";
        //public static string HiddenGamesSuperFamicom = "";
        public static bool CustomFlashedNes = false;
        public static bool CustomFlashedFamicom = false;
        public static bool CustomFlashedSnes = false;
        public static bool CustomFlashedSuperFamicom = false;
        public static bool UseFontNes = true;
        public static bool UseFontFamicom = true;
        public static bool UseFontSnes = true;
        public static bool UseFontSuperFamicom = true;
        public static byte AntiArmetLevel = 0;
        public static MainForm.ConsoleType ConsoleType = MainForm.ConsoleType.Unknown;
        public static byte MaxGamesPerFolderNes = 30;
        public static byte MaxGamesPerFolderFamicom = 30;
        public static byte MaxGamesPerFolderSnes = 30;
        public static byte MaxGamesPerFolderSuperFamicom = 30;
        public static NesMenuCollection.SplitStyle FoldersModeNes = NesMenuCollection.SplitStyle.Original_Auto;
        public static NesMenuCollection.SplitStyle FoldersModeFamicom = NesMenuCollection.SplitStyle.Original_Auto;
        public static NesMenuCollection.SplitStyle FoldersModeSnes = NesMenuCollection.SplitStyle.Original_Auto;
        public static NesMenuCollection.SplitStyle FoldersModeSuperFamicom = NesMenuCollection.SplitStyle.Original_Auto;
        public static bool AutofireHackNes = false;
        public static bool AutofireHackSnes = false;
        public static bool AutofireXYHack = false;
        public static bool FcStart = false;
        public static bool ResetHackNes = true;
        public static bool ResetHackSnes = true;
        public static uint ResetCombinationNes = (uint)(SelectNesButtonsForm.NesButtons.Down | SelectNesButtonsForm.NesButtons.Select);
        public static uint ResetCombinationSnes = (uint)(SelectNesButtonsForm.NesButtons.Down | SelectNesButtonsForm.NesButtons.Select);
        public static Dictionary<string, string> Presets = new Dictionary<string, string>();
        public static string ExtraCommandLineArgumentsNes = "";
        public static string ExtraCommandLineArgumentsSnes = "";
        public static bool Compress = true;
        public static bool CompressCover = true;
        public static bool DisablePopups = false;
        public const string ConfigDir = "config";
        public const string ConfigFile = "config.ini";
        public static bool FtpServer = false;
        public static bool TelnetServer = false;
        public static string Language = "";

        public static bool CustomFlashed
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return CustomFlashedNes;
                    case MainForm.ConsoleType.Famicom:
                        return CustomFlashedFamicom;
                    case MainForm.ConsoleType.SNES:
                        return CustomFlashedSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return CustomFlashedSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        CustomFlashedNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        CustomFlashedFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        CustomFlashedSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        CustomFlashedSuperFamicom = value;
                        break;
                }
            }
        }

        public static string SelectedGames
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return SelectedGamesNes;
                    case MainForm.ConsoleType.Famicom:
                        return SelectedGamesFamicom;
                    case MainForm.ConsoleType.SNES:
                        return SelectedGamesSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return SelectedGamesSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        SelectedGamesNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        SelectedGamesFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        SelectedGamesSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        SelectedGamesSuperFamicom = value;
                        break;
                }
            }
        }

        /*
        public static string HiddenGames
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return HiddenGamesNes;
                    case MainForm.ConsoleType.Famicom:
                        return HiddenGamesFamicom;
                    case MainForm.ConsoleType.SNES:
                        return HiddenGamesSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return HiddenGamesSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        HiddenGamesNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        HiddenGamesFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        HiddenGamesSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        HiddenGamesSuperFamicom = value;
                        break;
                }
            }
        }
        */

        public static byte MaxGamesPerFolder
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return MaxGamesPerFolderNes;
                    case MainForm.ConsoleType.Famicom:
                        return MaxGamesPerFolderFamicom;
                    case MainForm.ConsoleType.SNES:
                        return MaxGamesPerFolderSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return MaxGamesPerFolderSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        MaxGamesPerFolderNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        MaxGamesPerFolderFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        MaxGamesPerFolderSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        MaxGamesPerFolderSuperFamicom = value;
                        break;
                }
            }
        }

        public static NesMenuCollection.SplitStyle FoldersMode
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return FoldersModeNes;
                    case MainForm.ConsoleType.Famicom:
                        return FoldersModeFamicom;
                    case MainForm.ConsoleType.SNES:
                        return FoldersModeSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return FoldersModeSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        FoldersModeNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        FoldersModeFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        FoldersModeSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        FoldersModeSuperFamicom = value;
                        break;
                }
            }
        }

        public static bool AutofireHack
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return AutofireHackNes;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return AutofireHackSnes;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        AutofireHackNes = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        AutofireHackSnes = value;
                        break;
                }
            }
        }

        public static bool UseFont
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return UseFontNes;
                    case MainForm.ConsoleType.Famicom:
                        return UseFontFamicom;
                    case MainForm.ConsoleType.SNES:
                        return UseFontSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return UseFontSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        UseFontNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        UseFontFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        UseFontSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        UseFontSuperFamicom = value;
                        break;
                }
            }
        }

        public static bool ResetHack
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return ResetHackNes;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return ResetHackSnes;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        ResetHackNes = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        ResetHackSnes = value;
                        break;
                }
            }
        }

        public static uint ResetCombination
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return ResetCombinationNes;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return ResetCombinationSnes;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        ConfigIni.ResetCombinationNes = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        ConfigIni.ResetCombinationSnes = value;
                        break;
                }
            }
        }

        public static string ExtraCommandLineArguments
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return ExtraCommandLineArgumentsNes;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return ExtraCommandLineArgumentsSnes;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        ConfigIni.ExtraCommandLineArgumentsNes = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        ConfigIni.ExtraCommandLineArgumentsSnes = value;
                        break;
                }
            }
        }

        public static void Load()
        {
            Debug.WriteLine("Loading config");
            var fileNameOld = Path.Combine(Program.BaseDirectoryExternal, ConfigFile);
            var configFullDir = Path.Combine(Program.BaseDirectoryExternal, ConfigDir);
            var fileName = Path.Combine(configFullDir, ConfigFile);
            if (File.Exists(fileNameOld)) // Moving old config to new directory
            {
                Directory.CreateDirectory(configFullDir);
                File.Copy(fileNameOld, fileName, true);
                File.Delete(fileNameOld);
            }
            if (File.Exists(fileName))
            {
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
                                case "language":
                                    Language = value;
                                    break;
                                case "selectedgamesnes":
                                    SelectedGamesNes = value;
                                    break;
                                case "selectedgamesfamicom":
                                    SelectedGamesFamicom = value;
                                    break;
                                case "selectedgamessnes":
                                    SelectedGamesSnes = value;
                                    break;
                                case "selectedgamessuperfamicom":
                                    SelectedGamesSuperFamicom = value;
                                    break;
                                case "originalgamesposition":
                                    OriginalGamesPosition = (MainForm.OriginalGamesPosition)byte.Parse(value);
                                    break;
                                //case "hiddengames":
                                //    HiddenGamesNes = value;
                                //    break;
                                //case "hiddengamesfamicom":
                                //    HiddenGamesFamicom = value;
                                //    break;
                                //case "hiddengamessnes":
                                //    HiddenGamesSnes = value;
                                //    break;
                                //case "hiddengamessuperfamicom":
                                //    HiddenGamesSuperFamicom = value;
                                //    break;
                                case "customflashednes":
                                    CustomFlashedNes = !value.ToLower().Equals("false");
                                    break;
                                case "customflashedfamicom":
                                    CustomFlashedFamicom = !value.ToLower().Equals("false");
                                    break;
                                case "customflashedsnes":
                                    CustomFlashedSnes = !value.ToLower().Equals("false");
                                    break;
                                case "customflashedsuperfamicom":
                                    CustomFlashedSuperFamicom = !value.ToLower().Equals("false");
                                    break;
                                case "usefont":
                                    UseFontNes = !value.ToLower().Equals("false");
                                    break;
                                case "usefontfamicom":
                                    UseFontFamicom = !value.ToLower().Equals("false");
                                    break;
                                case "usefontsnes":
                                    UseFontSnes = !value.ToLower().Equals("false");
                                    break;
                                case "usefontsuperfamicom":
                                    UseFontSuperFamicom = !value.ToLower().Equals("false");
                                    break;
                                case "runcount":
                                    RunCount = int.Parse(value);
                                    break;
                                case "antiarmetlevel":
                                    AntiArmetLevel = byte.Parse(value);
                                    break;
                                case "resethack":
                                case "cloverconhack":
                                    ResetHackNes = !value.ToLower().Equals("false");
                                    break;
                                case "resethacksnes":
                                    ResetHackSnes = !value.ToLower().Equals("false");
                                    break;
                                case "autofirehack":
                                    AutofireHackNes = !value.ToLower().Equals("false");
                                    break;
                                case "autofirehacksnes":
                                    AutofireHackSnes = !value.ToLower().Equals("false");
                                    break;
                                case "autofirexyhack":
                                    AutofireXYHack = !value.ToLower().Equals("false");
                                    break;
                                case "resetcombination":
                                    ResetCombinationNes = uint.Parse(value);
                                    break;
                                case "resetcombinationsnes":
                                    ResetCombinationSnes = uint.Parse(value);
                                    break;
                                case "consoletype":
                                    ConsoleType = (MainForm.ConsoleType)byte.Parse(value);
                                    break;
                                case "extracommandlinearguments":
                                    ExtraCommandLineArgumentsNes = value;
                                    break;
                                case "extracommandlineargumentssnes":
                                    ExtraCommandLineArgumentsSnes = value;
                                    break;
                                case "fcstart":
                                    FcStart = !value.ToLower().Equals("false");
                                    break;
                                case "maxgamesperfolder":
                                    MaxGamesPerFolderNes = byte.Parse(value);
                                    break;
                                case "maxgamesperfolderfamicom":
                                    MaxGamesPerFolderFamicom = byte.Parse(value);
                                    break;
                                case "maxgamesperfoldersnes":
                                    MaxGamesPerFolderSnes = byte.Parse(value);
                                    break;
                                case "maxgamesperfoldersuperfamicom":
                                    MaxGamesPerFolderSuperFamicom = byte.Parse(value);
                                    break;
                                case "foldersmode":
                                    FoldersModeNes = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                    break;
                                case "foldersmodefamicom":
                                    FoldersModeSuperFamicom = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                    break;
                                case "foldersmodesnes":
                                    FoldersModeSnes = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                    break;
                                case "foldersmodesuperfamicom":
                                    FoldersModeSuperFamicom = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                    break;
                                case "compress":
                                    Compress = !value.ToLower().Equals("false");
                                    break;
                                case "compresscover":
                                    CompressCover = !value.ToLower().Equals("false");
                                    break;
                                case "disablepopups":
                                    DisablePopups = !value.ToLower().Equals("false");
                                    break;
                                case "ftpserver":
                                    FtpServer = !value.ToLower().Equals("false");
                                    break;
                                case "telnetserver":
                                    TelnetServer = !value.ToLower().Equals("false");
                                    break;
                            }
                            break;
                        case "presets":
                            Presets[param] = value;
                            break;
                    }
                }
            }
        }

        public static void Save()
        {
            Debug.WriteLine("Saving config");
            var configLines = new List<string>();
            configLines.Add("[Config]");
            configLines.Add(string.Format("Language={0}", Language));
            configLines.Add(string.Format("SelectedGamesNes={0}", SelectedGamesNes));
            configLines.Add(string.Format("SelectedGamesFamicom={0}", SelectedGamesFamicom));
            configLines.Add(string.Format("SelectedGamesSnes={0}", SelectedGamesSnes));
            configLines.Add(string.Format("SelectedGamesSuperFamicom={0}", SelectedGamesSuperFamicom));
            configLines.Add(string.Format("OriginalGamesPosition={0}", (byte)OriginalGamesPosition));
            //configLines.Add(string.Format("HiddenGames={0}", HiddenGamesNes));
            //configLines.Add(string.Format("HiddenGamesFamicom={0}", HiddenGamesFamicom));
            //configLines.Add(string.Format("HiddenGamesSnes={0}", HiddenGamesSnes));
            //configLines.Add(string.Format("HiddenGamesSuperFamicom={0}", HiddenGamesSuperFamicom));
            configLines.Add(string.Format("CustomFlashedNes={0}", CustomFlashedNes));
            configLines.Add(string.Format("CustomFlashedFamicom={0}", CustomFlashedFamicom));
            configLines.Add(string.Format("CustomFlashedSnes={0}", CustomFlashedSnes));
            configLines.Add(string.Format("CustomFlashedSuperFamicom={0}", CustomFlashedSuperFamicom));
            configLines.Add(string.Format("UseFont={0}", UseFontNes));
            configLines.Add(string.Format("UseFontFamicom={0}", UseFontFamicom));
            configLines.Add(string.Format("UseFontSuperFamicom={0}", UseFontSuperFamicom));
            configLines.Add(string.Format("UseFontSnes={0}", UseFontSnes));
            configLines.Add(string.Format("ResetHack={0}", ResetHackNes));
            configLines.Add(string.Format("ResetHackSnes={0}", ResetHackSnes));
            configLines.Add(string.Format("ResetCombination={0}", ResetCombinationNes));
            configLines.Add(string.Format("ResetCombinationSnes={0}", ResetCombinationSnes));
            configLines.Add(string.Format("AutofireHack={0}", AutofireHackNes));
            configLines.Add(string.Format("AutofireHackSnes={0}", AutofireHackSnes));
            configLines.Add(string.Format("AutofireXYHack={0}", AutofireXYHack));
            configLines.Add(string.Format("AntiArmetLevel={0}", AntiArmetLevel));
            configLines.Add(string.Format("ConsoleType={0}", (byte)ConsoleType));
            configLines.Add(string.Format("FcStart={0}", FcStart));
            configLines.Add(string.Format("ExtraCommandLineArguments={0}", ExtraCommandLineArgumentsNes));
            configLines.Add(string.Format("ExtraCommandLineArgumentsSnes={0}", ExtraCommandLineArgumentsSnes));
            configLines.Add(string.Format("FoldersMode={0}", (byte)FoldersModeNes));
            configLines.Add(string.Format("FoldersModeFamicom={0}", (byte)FoldersModeFamicom));
            configLines.Add(string.Format("FoldersModeSnes={0}", (byte)FoldersModeSnes));
            configLines.Add(string.Format("FoldersModeSuperFamicom={0}", (byte)FoldersModeSuperFamicom));
            configLines.Add(string.Format("MaxGamesPerFolder={0}", MaxGamesPerFolderNes));
            configLines.Add(string.Format("MaxGamesPerFolderFamicom={0}", MaxGamesPerFolderSuperFamicom));
            configLines.Add(string.Format("MaxGamesPerFolderSnes={0}", MaxGamesPerFolderSnes));
            configLines.Add(string.Format("MaxGamesPerFolderSuperFamicom={0}", MaxGamesPerFolderSuperFamicom));
            configLines.Add(string.Format("Compress={0}", Compress));
            configLines.Add(string.Format("CompressCover={0}", CompressCover));
            configLines.Add(string.Format("DisablePopups={0}", DisablePopups));
            configLines.Add(string.Format("FtpServer={0}", FtpServer));
            configLines.Add(string.Format("TelnetServer={0}", TelnetServer));
            configLines.Add(string.Format("RunCount={0}", RunCount));

            configLines.Add("");
            configLines.Add("[Presets]");
            foreach (var preset in Presets.Keys)
            {
                configLines.Add(string.Format("{0}={1}", preset, Presets[preset]));
            }

            var configFullDir = Path.Combine(Program.BaseDirectoryExternal, ConfigDir);
            var fileName = Path.Combine(configFullDir, ConfigFile);
            Directory.CreateDirectory(configFullDir);
            File.WriteAllLines(fileName, configLines.ToArray());
        }

        public static Dictionary<string, string> GetConfigDictionary()
        {
            var config = new Dictionary<string, string>();
            config["clovercon_home_combination"] = ConfigIni.ResetHack ? string.Format("0x{0:X4}", ConfigIni.ResetCombination) : "0x7FFF";
            config["clovercon_autofire"] = ConfigIni.AutofireHack ? "1" : "0";
            config["clovercon_autofire_xy"] = ConfigIni.AutofireXYHack && (ConfigIni.ConsoleType == MainForm.ConsoleType.NES || ConfigIni.ConsoleType == MainForm.ConsoleType.Famicom) ? "1" : "0";
            config["clovercon_fc_start"] = ConfigIni.FcStart && (ConfigIni.ConsoleType == MainForm.ConsoleType.Famicom) ? "1" : "0";
            config["fontfix_enabled"] = ConfigIni.UseFont ? "y" : "n";
            config["disable_armet"] = (ConfigIni.AntiArmetLevel > 0 && (ConfigIni.ConsoleType == MainForm.ConsoleType.NES || ConfigIni.ConsoleType == MainForm.ConsoleType.Famicom)) ? "y" : "n";
            if ((ConfigIni.ConsoleType == MainForm.ConsoleType.NES || ConfigIni.ConsoleType == MainForm.ConsoleType.Famicom))
                config["nes_extra_args"] = ConfigIni.ExtraCommandLineArguments;
            if ((ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom))
                config["snes_extra_args"] = ConfigIni.ExtraCommandLineArguments;
            return config;
        }
    }
}
