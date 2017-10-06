using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class ConfigIni
    {
        public static int RunCount = 0;
        public static string SelectedGamesNes = "default";
        public static string SelectedGamesSnes = "default";
        public static string HiddenGamesNes = "";
        public static string HiddenGamesFamicom = "";
        public static string HiddenGamesSnes = "";
        public static string HiddenGamesSuperFamicom = "";
        public static bool CustomFlashedNes = false;
        public static bool CustomFlashedFamicom = false;
        public static bool CustomFlashedSnes = false;
        public static bool CustomFlashedSuperFamicom = false;
        public static bool UseFont = true;
        public static bool ResetHack = true;
        public static bool AutofireHack = false;
        public static bool AutofireXYHack = false;
        public static bool FcStart = false;
        public static byte AntiArmetLevel = 0;
        public static MainForm.ConsoleType ConsoleType = MainForm.ConsoleType.NES;
        public static byte MaxGamesPerFolder = 30;
        public static NesMenuCollection.SplitStyle FoldersMode = NesMenuCollection.SplitStyle.Original_Auto;
        public static SelectButtonsForm.NesButtons ResetCombination = SelectButtonsForm.NesButtons.Down | SelectButtonsForm.NesButtons.Select;
        public static Dictionary<string, string> Presets = new Dictionary<string, string>();
        public static string ExtraCommandLineArgumentsNes = "";
        public static string ExtraCommandLineArgumentsSnes = "";
        public static bool Compress = true;
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
                    case MainForm.ConsoleType.Famicom:
                        return SelectedGamesNes;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return SelectedGamesSnes;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        ConfigIni.SelectedGamesNes = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        ConfigIni.SelectedGamesSnes = value;
                        break;
                }
            }
        }

        public static string HiddenGames
        {
            get
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return ConfigIni.HiddenGamesNes;
                    case MainForm.ConsoleType.Famicom:
                        return ConfigIni.HiddenGamesFamicom;
                    case MainForm.ConsoleType.SNES:
                        return ConfigIni.HiddenGamesSnes;
                    case MainForm.ConsoleType.SuperFamicom:
                        return ConfigIni.HiddenGamesSuperFamicom;
                }
            }
            set
            {
                switch (ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        ConfigIni.HiddenGamesNes = value;
                        break;
                    case MainForm.ConsoleType.Famicom:
                        ConfigIni.HiddenGamesFamicom = value;
                        break;
                    case MainForm.ConsoleType.SNES:
                        ConfigIni.HiddenGamesSnes = value;
                        break;
                    case MainForm.ConsoleType.SuperFamicom:
                        ConfigIni.HiddenGamesSuperFamicom = value;
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
                                case "selectedgames":
                                    SelectedGamesNes = value;
                                    break;
                                case "selectedgamessnes":
                                    SelectedGamesSnes = value;
                                    break;
                                case "hiddengames":
                                    HiddenGamesNes = value;
                                    break;
                                case "hiddengamesfamicom":
                                    HiddenGamesFamicom = value;
                                    break;
                                case "hiddengamessnes":
                                    HiddenGamesSnes = value;
                                    break;
                                case "hiddengamessuperfamicom":
                                    HiddenGamesSuperFamicom = value;
                                    break;
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
                                    UseFont = !value.ToLower().Equals("false");
                                    break;
                                case "runcount":
                                    RunCount = int.Parse(value);
                                    break;
                                case "antiarmetlevel":
                                    AntiArmetLevel = byte.Parse(value);
                                    break;
                                case "resethack":
                                case "cloverconhack":
                                    ResetHack = !value.ToLower().Equals("false");
                                    break;
                                case "autofirehack":
                                    AutofireHack = !value.ToLower().Equals("false");
                                    break;
                                case "autofirexyhack":
                                    AutofireXYHack = !value.ToLower().Equals("false");
                                    break;
                                case "resetcombination":
                                    ResetCombination = (SelectButtonsForm.NesButtons)byte.Parse(value);
                                    break;
                                case "consoletype":
                                    ConsoleType = (MainForm.ConsoleType)byte.Parse(value);
                                    break;
                                case "extracommandlinearguments":
                                    ExtraCommandLineArguments = value;
                                    break;
                                case "fcstart":
                                    FcStart = !value.ToLower().Equals("false");
                                    break;
                                case "maxgamesperfolder":
                                    MaxGamesPerFolder = byte.Parse(value);
                                    break;
                                case "foldersmode":
                                    FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse(value);
                                    break;
                                case "compress":
                                    Compress = !value.ToLower().Equals("false");
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
            configLines.Add(string.Format("SelectedGames={0}", SelectedGamesNes));
            configLines.Add(string.Format("SelectedGamesSnes={0}", SelectedGamesSnes));
            configLines.Add(string.Format("HiddenGames={0}", HiddenGamesNes));
            configLines.Add(string.Format("HiddenGamesFamicom={0}", HiddenGamesFamicom));
            configLines.Add(string.Format("HiddenGamesSnes={0}", HiddenGamesSnes));
            configLines.Add(string.Format("HiddenGamesSuperFamicom={0}", HiddenGamesSuperFamicom));
            configLines.Add(string.Format("CustomFlashedNes={0}", CustomFlashedNes));
            configLines.Add(string.Format("CustomFlashedFamicom={0}", CustomFlashedFamicom));
            configLines.Add(string.Format("CustomFlashedSnes={0}", CustomFlashedSnes));
            configLines.Add(string.Format("CustomFlashedSuperFamicom={0}", CustomFlashedSuperFamicom));
            configLines.Add(string.Format("UseFont={0}", UseFont));
            configLines.Add(string.Format("ResetHack={0}", ResetHack));
            configLines.Add(string.Format("AutofireHack={0}", AutofireHack));
            configLines.Add(string.Format("AutofireXYHack={0}", AutofireXYHack));
            configLines.Add(string.Format("AntiArmetLevel={0}", AntiArmetLevel));
            configLines.Add(string.Format("ResetCombination={0}", (byte)ResetCombination));
            configLines.Add(string.Format("ConsoleType={0}", (byte)ConsoleType));
            configLines.Add(string.Format("ExtraCommandLineArguments={0}", ExtraCommandLineArguments));
            configLines.Add(string.Format("FcStart={0}", FcStart));
            configLines.Add(string.Format("FoldersMode={0}", (byte)FoldersMode));
            configLines.Add(string.Format("MaxGamesPerFolder={0}", MaxGamesPerFolder));
            configLines.Add(string.Format("Compress={0}", Compress));
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
            config["clovercon_home_combination"] = ConfigIni.ResetHack ? string.Format("0x{0:X2}", (byte)ConfigIni.ResetCombination) : "0xFFFF";
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
