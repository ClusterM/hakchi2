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
        public static bool CloverconHack = true;
        public static bool RemoveThumbnails = false;
        public static bool EightBitPngCompression = true;
        public static byte AntiArmetLevel = 0;
        public static Dictionary<string, string> Presets = new Dictionary<string, string>();
        const string ConfigFile = "config.ini";

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
                                case "cloverconhack":
                                    CloverconHack = !value.ToLower().Equals("false");
                                    break;
                                case "removethumbnails":
                                    RemoveThumbnails = !value.ToLower().Equals("false");
                                    break;
                                case "eightbitpngcompression":
                                    EightBitPngCompression = !value.ToLower().Equals("false");
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
            configLines.Add(string.Format("CloverconHack={0}", CloverconHack));
            configLines.Add(string.Format("FirstRun={0}", FirstRun));
            configLines.Add(string.Format("AntiArmetLevel={0}", AntiArmetLevel));
            configLines.Add(string.Format("RemoveThumbnails={0}", RemoveThumbnails));
            configLines.Add(string.Format("EightBitPngCompression={0}", EightBitPngCompression));
            
            configLines.Add("[Presets]");
            configLines.Add("");
            foreach (var preset in Presets.Keys)
            {
                configLines.Add(string.Format("{0}={1}", preset, Presets[preset]));
            }
            File.WriteAllLines(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigFile), configLines.ToArray());
        }
    }
}
