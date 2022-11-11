using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            fillActions(new Version(ConfigIni.Instance.LastVersion), Shared.AppVersion);
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
                }
            }

            // bring the last version up to speed since all actions were successful
            ConfigIni.Instance.LastVersion = currentVersion.ToString();

            Trace.WriteLine("[Upgrade] All actions executed successfully");
            return true;
        }

        private void fillActions(Version lastVersion, Version currentVersion)
        {
            actions.AddRange(new Action[] {

                new Action() {
                    startingVersion = new Version("0.0.0.0"),
                    targetVersion = new Version("3.0.0.0"),
                    action = new ActionFunc(() => {
                        mainForm.ResetOriginalGamesForAllSystems(false);
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
                            Path.Combine(i, "data", "fes1.bin"),
                            Path.Combine(i, "data", "splash.gz"),
                            Path.Combine(i, "data", "uboot.bin"),
                            Path.Combine(i, "data", "ubootSD.bin"),
                            Path.Combine(i, "data", "zImage"),
                            Path.Combine(i, "data", "zImageMemboot"),
                            Path.Combine(i, "tools", "cpio.exe"),
                            Path.Combine(i, "tools", "cyggcc_s-1.dll"),
                            Path.Combine(i, "tools", "cygiconv-2.dll"),
                            Path.Combine(i, "tools", "cygintl-8.dll"),
                            Path.Combine(i, "tools", "cygwin1.dll"),
                            Path.Combine(i, "tools", "lzop.exe"),
                            Path.Combine(i, "tools", "mkbootfs.exe"),
                            Path.Combine(i, "tools", "mkbootimg.exe"),
                            Path.Combine(i, "tools", "unpackbootimg.exe"),
                            Path.Combine(i, "tools", "xz.exe"),
                            Path.Combine(e, "user_mods", "hakchi-v1.0.3-110.hmod")
                        };
                        string[] unusedDirectories = new string[]
                        {
                            Path.Combine(i, "languages", "en-GB"),
                            Path.Combine(i, "mods"),
                            Path.Combine(e, "user_mods", "music_hack.hmod")
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

                new Action()
                {
                    startingVersion = new Version("3.2.2.0"),
                    targetVersion = new Version("3.3.0.0"),
                    action = new ActionFunc(() => {
                        if (ConfigIni.Instance.LastVersion != "0.0.0.0")
                            ConfigIni.Instance.SeparateGameLocalStorage = true;
                        return true;
                    })
                },

                new Action()
                {
                    startingVersion = new Version("3.4.1.0"),
                    targetVersion = new Version("3.4.1.1"),
                    action = new ActionFunc(() => {
                        var tempConfig = ConfigIni.GetCleanInstance();
                        ConfigIni.Instance.TelnetCommand = tempConfig.TelnetCommand;
                        ConfigIni.Instance.TelnetArguments = tempConfig.TelnetArguments;
                        ConfigIni.Instance.FtpCommand = tempConfig.FtpCommand;
                        ConfigIni.Instance.FtpArguments = tempConfig.FtpArguments;
                        return true;
                    })
                },

                new Action()
                {
                    startingVersion = new Version("3.5.0.0"),
                    targetVersion = new Version("3.5.3.0"),
                    action = new ActionFunc(() => {
                        ConfigIni.Instance.repos = ConfigIni.Instance.repos.Where(repo => repo.URL != "modstore://").ToArray();
                        return true;
                    })
                },
                new Action() {
                    startingVersion = new Version("0.0.0.0"),
                    targetVersion = new Version("3.6.1.1008"),
                    action = new ActionFunc(() => {
                        if (lastVersion.CompareTo(new Version(0,0,0,0)) != 0)
                            mainForm.ResetOriginalGamesForAllSystems(true);

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_USA].MaxGamesPerFolder = 42;
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_USA].FolderImagesSet = "TheWez1981-Genesis";

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_EUR].MaxGamesPerFolder = 42;
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_EUR].FolderImagesSet = "TheWez1981-MegaDrive";

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_JPN].MaxGamesPerFolder = 42;
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_JPN].FolderImagesSet = "TheWez1981-MegaDrive-JP";

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_ASIA].MaxGamesPerFolder = 42;
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_ASIA].FolderImagesSet = "TheWez1981-MegaDrive";

                        return true;
                    })
                },
                new Action()
                {
                    startingVersion = new Version("0.0.0.0"),
                    targetVersion = new Version("3.8.0.1"),
                    action = new ActionFunc(() =>
                    {
                        var oldCache = Path.Combine(Program.BaseDirectoryExternal, "user_mods", "readme_cache");
                        
                        if (Directory.Exists(oldCache))
                        {
                            Directory.Delete(oldCache, true);
                        }

                        return true;
                    })
                }

            });
        }
    }
}
