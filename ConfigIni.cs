using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class ConfigIni
    {
        public static bool FirstRun = true;
        public static string SelectedGames = "default";
        public static string HiddenGames = "";
        public static bool CustomFlashed = false;
        public static bool UseFont = true;
        public static bool ResetHack = true;
        public static bool AutofireHack = false;
        public static bool AutofireXYHack = false;
        public static bool FcStart = false;
        public static byte AntiArmetLevel = 0;
        public static byte ConsoleType = 0;
        public static byte MaxGamesPerFolder = 30;
        public static NesMenuCollection.SplitStyle FoldersMode = NesMenuCollection.SplitStyle.Original_Auto;
        public static SelectButtonsForm.NesButtons ResetCombination = SelectButtonsForm.NesButtons.Down | SelectButtonsForm.NesButtons.Select;
        public static Dictionary<string, string> Presets = new Dictionary<string, string>();
        public static string ExtraCommandLineArguments = "";
        public static bool Compress = true;
        public const string ConfigDir = "config";
        public const string ConfigFile = "config.ini";
        public static bool FtpServer = false;
        public static bool TelnetServer = false;
        public static string Language = "";

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
                                    SelectedGames = value;
                                    break;
                                case "hiddengames":
                                    HiddenGames = value;
                                    break;
                                case "custom2flashed":
                                    CustomFlashed = !value.ToLower().Equals("false");
                                    FirstRun = false;
                                    break;
                                case "usefont":
                                    UseFont = !value.ToLower().Equals("false");
                                    break;
                                case "firstrun":
                                    FirstRun = !value.ToLower().Equals("false");
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
                                    ConsoleType = byte.Parse(value);
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
            configLines.Add(string.Format("SelectedGames={0}", SelectedGames));
            configLines.Add(string.Format("HiddenGames={0}", HiddenGames));
            configLines.Add(string.Format("Custom2Flashed={0}", CustomFlashed));
            configLines.Add(string.Format("UseFont={0}", UseFont));
            configLines.Add(string.Format("ResetHack={0}", ResetHack));
            configLines.Add(string.Format("AutofireHack={0}", AutofireHack));
            configLines.Add(string.Format("AutofireXYHack={0}", AutofireXYHack));
            configLines.Add(string.Format("FirstRun={0}", FirstRun));
            configLines.Add(string.Format("AntiArmetLevel={0}", AntiArmetLevel));
            configLines.Add(string.Format("ResetCombination={0}", (byte)ResetCombination));
            configLines.Add(string.Format("ConsoleType={0}", ConsoleType));
            configLines.Add(string.Format("ExtraCommandLineArguments={0}", ExtraCommandLineArguments));
            configLines.Add(string.Format("FcStart={0}", FcStart));
            configLines.Add(string.Format("FoldersMode={0}", (byte)FoldersMode));
            configLines.Add(string.Format("MaxGamesPerFolder={0}", MaxGamesPerFolder));
            configLines.Add(string.Format("Compress={0}", Compress));
            configLines.Add(string.Format("FtpServer={0}", FtpServer));
            configLines.Add(string.Format("TelnetServer={0}", TelnetServer));

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
            config["clovercon_autofire_xy"] = ConfigIni.AutofireXYHack ? "1" : "0";
            config["clovercon_fc_start"] = ConfigIni.FcStart ? "1" : "0";
            config["fontfix_enabled"] = ConfigIni.UseFont ? "y" : "n";
            config["disable_armet"] = (ConfigIni.AntiArmetLevel > 0) ? "y" : "n";
            config["nes_extra_args"] = ConfigIni.ExtraCommandLineArguments;
            return config;
        }
    }
}
