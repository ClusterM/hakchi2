using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class Upgrade
    {
        public delegate bool ActionFunc();
        class Action
        {
            public Version startingVersion;
            public Version targetVersion;
            public ActionFunc action;
        }

        private MainForm mainForm;
        private List<Action> actions;

        public Upgrade(MainForm mainForm)
        {
            this.mainForm = mainForm;
            actions = new List<Action>();
            fillActions();
        }

        public bool Run()
        {
            var lastVersion = new Version(ConfigIni.Instance.LastVersion);
            var currentVersion = Shared.AppVersion;

            if (lastVersion.CompareTo(currentVersion) > 0)
            {
                Debug.WriteLine("[Upgrade] Version has been downgraded from last run, results can be unpredictable");
                return false;
            }
            else if (lastVersion.CompareTo(currentVersion) == 0)
            {
                Debug.WriteLine("[Upgrade] No upgrade action needed");
                return false;
            }

            Debug.WriteLine("[Upgrade] Checking for upgrade actions. Last run version: " + lastVersion.ToString() + ", current version: " + currentVersion.ToString());
            foreach (var action in actions)
            {
                if (lastVersion.CompareTo(action.startingVersion) >= 0 && lastVersion.CompareTo(action.targetVersion) < 0)
                {
                    Debug.WriteLine("[Upgrade] Running upgrade action " + action.startingVersion.ToString() + " -> " + action.targetVersion);
                    try
                    {
                        if (action.action() != true)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[Upgrade] Error executing action: " + ex.Message + ex.StackTrace);
                        return false;
                    }

                    // bump up currently updated version
                    lastVersion = action.targetVersion;
                    ConfigIni.Instance.LastVersion = lastVersion.ToString();
                }
            }

            // bring the last version up to speed since all actions were successful
            ConfigIni.Instance.LastVersion = currentVersion.ToString();

            Debug.WriteLine("[Upgrade] All actions executed successfully");
            return true;
        }

        private void fillActions()
        {
            actions.AddRange(new Action[] {

                new Action() {
                    startingVersion = new Version("0.0.0.0"),
                    targetVersion = new Version("3.0.0.0"),
                    action = new ActionFunc(() => {
                        mainForm.ResetOriginalGamesForAllSystems();
                        return true;
                    })
                },

                new Action() {
                    startingVersion = new Version("3.0.0.0"),
                    targetVersion = new Version("3.1.0.5"),
                    action = new ActionFunc(() => {
                        string f = Path.Combine(Program.BaseDirectoryExternal, ConfigIni.ConfigDir, "folders_snes.xml");
                        string f1 = Path.Combine(Program.BaseDirectoryExternal, ConfigIni.ConfigDir, "folders_snes_eur.xml");
                        string f2 = Path.Combine(Program.BaseDirectoryExternal, ConfigIni.ConfigDir, "folders_snes_usa.xml");
                        if (File.Exists(f))
                        {
                            Debug.WriteLine("Converting folders_snes.xml file into eur and usa counterparts.");
                            if (!File.Exists(f1)) File.Copy(f, f1);
                            if (!File.Exists(f2)) File.Copy(f, f2);
                            File.Delete(f);
                        }
                        return true;
                    })
                }

            });
        }
    }
}
