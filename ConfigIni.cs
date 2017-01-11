using System;
using System.Collections.Generic;
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
        const string ConfigFile = "config.ini";

        public static void Load()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigFile);
            if (File.Exists(fileName))
            {
                var configLines = File.ReadAllLines(fileName);
                foreach (var line in configLines)
                {
                    int pos = line.IndexOf('=');
                    if (pos <= 0) continue;
                    var param = line.Substring(0, pos).Trim().ToLower();
                    var value = line.Substring(pos + 1).Trim();
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
                    }
                }

            }
        }

        public static void Save()
        {
            var configLines = new List<string>();
            configLines.Add("[Config]");
            configLines.Add(string.Format("SelectedGames={0}", SelectedGames));
            configLines.Add(string.Format("HiddenGames={0}", HiddenGames));
            configLines.Add(string.Format("CustomFlashed={0}", CustomFlashed));
            configLines.Add(string.Format("UseFont={0}", CustomFlashed));
            configLines.Add(string.Format("FirstRun={0}", CustomFlashed));
            File.WriteAllLines(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigFile), configLines.ToArray());
        }
    }
}
