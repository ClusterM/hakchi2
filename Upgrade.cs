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
                Trace.WriteLine("[Upgrade] Version has been downgraded from last run, results can be unpredictable");
                return false;
            }
            else if (lastVersion.CompareTo(currentVersion) == 0)
            {
                Trace.WriteLine("[Upgrade] No upgrade action needed");
                return false;
            }

            Trace.WriteLine("[Upgrade] Checking for upgrade actions. Last run version: " + lastVersion.ToString() + ", current version: " + currentVersion.ToString());
            foreach (var action in actions)
            {
                if (lastVersion.CompareTo(action.startingVersion) >= 0 && lastVersion.CompareTo(action.targetVersion) < 0)
                {
                    Trace.WriteLine("[Upgrade] Running upgrade action " + action.startingVersion.ToString() + " -> " + action.targetVersion);
                    try
                    {
                        if (action.action() != true)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("[Upgrade] Error executing action: " + ex.Message + ex.StackTrace);
                        return false;
                    }

                    // bump up currently updated version
                    lastVersion = action.targetVersion;
                    ConfigIni.Instance.LastVersion = lastVersion.ToString();
                }
            }

            // bring the last version up to speed since all actions were successful
            ConfigIni.Instance.LastVersion = currentVersion.ToString();

            Trace.WriteLine("[Upgrade] All actions executed successfully");
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
                            Trace.WriteLine("Converting folders_snes.xml file into eur and usa counterparts.");
                            if (!File.Exists(f1)) File.Copy(f, f1);
                            if (!File.Exists(f2)) File.Copy(f, f2);
                            File.Delete(f);
                        }
                        return true;
                    })
                },

                new Action() {
                    startingVersion = new Version("3.1.0.5"),
                    targetVersion = new Version("3.2.2.0"),
                    action = new ActionFunc(() => {

                        string i = Program.BaseDirectoryInternal;
                        string e = Program.BaseDirectoryExternal;
                        string[] unusedFiles = new string[]
                        {
                            i + "\\data\\fes1.bin",
                            i + "\\data\\splash.gz",
                            i + "\\data\\uboot.bin",
                            i + "\\data\\ubootSD.bin",
                            i + "\\data\\zImage",
                            i + "\\data\\zImageMemboot",
                            i + "\\tools\\cpio.exe",
                            i + "\\tools\\cyggcc_s-1.dll",
                            i + "\\tools\\cygiconv-2.dll",
                            i + "\\tools\\cygintl-8.dll",
                            i + "\\tools\\cygwin1.dll",
                            i + "\\tools\\lzop.exe",
                            i + "\\tools\\mkbootfs.exe",
                            i + "\\tools\\mkbootimg.exe",
                            i + "\\tools\\unpackbootimg.exe",
                            i + "\\tools\\xz.exe",
                            e + "\\user_mods\\hakchi-v1.0.3-110.hmod"
                        };
                        string[] unusedDirectories = new string[]
                        {
                            i + "\\languages\\en-GB",
                            i + "\\mods",
                            e + "\\user_mods\\music_hack.hmod",
                        };

                        foreach (var dir in unusedDirectories)
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                            }
                            catch (DirectoryNotFoundException) { }
                            catch (UnauthorizedAccessException)
                            {
                                Trace.WriteLine($"Could not delete directory \"{dir}\". UAC restrictions. No big deal");
                            }
                            catch { }
                        }

                        foreach (var file in unusedFiles)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (FileNotFoundException) { }
                            catch (UnauthorizedAccessException)
                            {
                                Trace.WriteLine($"Could not delete file \"{file}\". UAC restrictions. No big deal");
                            }
                            catch { }
                        }

                        return true;
                    })
                },

            });
        }
    }
}
