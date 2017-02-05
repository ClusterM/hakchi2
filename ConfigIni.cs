using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        public static bool RemoveThumbnails = false;
        public static bool EightBitPngCompression = false;
        public static bool FcStart = false;
        public static bool DisableMusic = false;
        public static bool FoldersAZ = false;
        public static byte AntiArmetLevel = 0;
        public static byte ConsoleType = 0;
        public static byte MaxGamesPerFolder = 30;
        public static SelectButtonsForm.NesButtons ResetCombination = SelectButtonsForm.NesButtons.Down | SelectButtonsForm.NesButtons.Select;
        public static Dictionary<string, string> Presets = new Dictionary<string, string>();
        public static string ExtraCommandLineArguments = "";
        public const string ConfigFile = "config.ini";

        public static void Load()
        {
            Debug.WriteLine("Loading config");
            var fileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigFile);
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
                                case "selectedgames":
                                    SelectedGames = value;
                                    break;
                                case "hiddengames":
                                    HiddenGames = value;
                                    break;
                                case "customflashed":
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
                                case "removethumbnails":
                                    RemoveThumbnails = !value.ToLower().Equals("false");
                                    break;
                                case "eightbitpngcompression":
                                    EightBitPngCompression = !value.ToLower().Equals("false");
                                    break;
                                case "resetcombination":
                                    ResetCombination = (SelectButtonsForm.NesButtons) byte.Parse(value);
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
                                case "disablemusic":
                                    DisableMusic = !value.ToLower().Equals("false");
                                    break;
                                case "foldersaz":
                                    FoldersAZ = !value.ToLower().Equals("false");
                                    break;
                                case "maxgamesperfolder":
                                    MaxGamesPerFolder = byte.Parse(value);
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
            configLines.Add(string.Format("SelectedGames={0}", SelectedGames));
            configLines.Add(string.Format("HiddenGames={0}", HiddenGames));
            configLines.Add(string.Format("CustomFlashed={0}", CustomFlashed));
            configLines.Add(string.Format("UseFont={0}", UseFont));
            configLines.Add(string.Format("ResetHack={0}", ResetHack));
            configLines.Add(string.Format("AutofireHack={0}", AutofireHack));
            configLines.Add(string.Format("FirstRun={0}", FirstRun));
            configLines.Add(string.Format("AntiArmetLevel={0}", AntiArmetLevel));
            configLines.Add(string.Format("RemoveThumbnails={0}", RemoveThumbnails));
            configLines.Add(string.Format("EightBitPngCompression={0}", EightBitPngCompression));
            configLines.Add(string.Format("ResetCombination={0}", (byte)ResetCombination));
            configLines.Add(string.Format("ConsoleType={0}", ConsoleType));
            configLines.Add(string.Format("ExtraCommandLineArguments={0}", ExtraCommandLineArguments));
            configLines.Add(string.Format("FcStart={0}", FcStart));
            configLines.Add(string.Format("DisableMusic={0}", DisableMusic));
            configLines.Add(string.Format("FoldersAZ={0}", FoldersAZ));
            configLines.Add(string.Format("MaxGamesPerFolder={0}", MaxGamesPerFolder));

            configLines.Add("");
            configLines.Add("[Presets]");
            foreach (var preset in Presets.Keys)
            {
                configLines.Add(string.Format("{0}={1}", preset, Presets[preset]));
            }
            File.WriteAllLines(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigFile), configLines.ToArray());
        }
    }
}
