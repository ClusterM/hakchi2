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
                    targetVersion = new Version("3.6.1.1001"),
                    action = new ActionFunc(() => {
                        mainForm.ResetOriginalGamesForAllSystems(true);
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_USA].FolderImagesSet = "TheWez1981-Genesis";
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_USA].OriginalGames.Clear();
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_USA].OriginalGames.AddRange(new string[] {
                            "CLV-M2-us_us_Alex_Kidd",
                            "CLV-M2-us_us_Alisia_Dragoon",
                            "CLV-M2-us_us_Altered_Beast",
                            "CLV-M2-us_us_Castle_of_Illusion",
                            "CLV-M2-us_us_Castlevania_Bloodlines",
                            "CLV-M2-us_us_Columns",
                            "CLV-M2-us_us_Comix_Zone",
                            "CLV-M2-us_us_Contra_Hard_Corps",
                            "CLV-M2-us_us_Darius",
                            "CLV-M2-us_us_Dr_Robotnik_s_Mean_Bean_Machine",
                            "CLV-M2-us_us_Dynamite_Headdy",
                            "CLV-M2-us_us_Earthworm_Jim",
                            "CLV-M2-us_us_Ecco_the_Dolphin",
                            "CLV-M2-us_us_Eternal_Champions",
                            "CLV-M2-us_us_Ghouls_n_Ghosts",
                            "CLV-M2-us_us_Golden_Axe",
                            "CLV-M2-us_us_Gunstar_Heroes",
                            "CLV-M2-us_us_Kid_Chameleon",
                            "CLV-M2-us_us_Landstalker",
                            "CLV-M2-us_us_Light_Crusader",
                            "CLV-M2-us_us_Mega_Man",
                            "CLV-M2-us_us_Monster_World_IV",
                            "CLV-M2-us_us_Mortal_Kombat_II",
                            "CLV-M2-us_us_Phantasy_Star_IV",
                            "CLV-M2-us_us_Road_Rash_II",
                            "CLV-M2-us_us_STREET_FIGHTER_II",
                            "CLV-M2-us_us_Shining_Force",
                            "CLV-M2-us_us_Shinobi_III",
                            "CLV-M2-us_us_Sonic_Spinball",
                            "CLV-M2-us_us_Sonic_The_Hedgehog",
                            "CLV-M2-us_us_Sonic_The_Hedgehog_2",
                            "CLV-M2-us_us_Space_Harrier_II",
                            "CLV-M2-us_us_Streets_of_Rage_2",
                            "CLV-M2-us_us_Strider",
                            "CLV-M2-us_us_Super_Fantasy_Zone",
                            "CLV-M2-us_us_Sword_of_Vermilion",
                            "CLV-M2-us_us_Tetris",
                            "CLV-M2-us_us_The_Story_of_Thor",
                            "CLV-M2-us_us_Thunder_Force_III",
                            "CLV-M2-us_us_ToeJam_Earl",
                            "CLV-M2-us_us_Vectorman",
                            "CLV-M2-us_us_Virtua_Fighter_2",
                            "CLV-M2-us_us_Wonder_Boy_in_Monster_World",
                            "CLV-M2-us_us_World_of_Illusion"
                        });

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_EUR].FolderImagesSet = "TheWez1981-MegaDrive";
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_EUR].OriginalGames.Clear();
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_EUR].OriginalGames.AddRange(new string[] {
                            "CLV-M2-eu_en_Alex_Kidd",
                            "CLV-M2-eu_en_Alisia_Dragoon",
                            "CLV-M2-eu_en_Altered_Beast",
                            "CLV-M2-eu_en_Castle_of_Illusion",
                            "CLV-M2-eu_en_Castlevania_Bloodlines",
                            "CLV-M2-eu_en_Columns",
                            "CLV-M2-eu_en_Comix_Zone",
                            "CLV-M2-eu_en_Contra_Hard_Corps",
                            "CLV-M2-eu_en_Darius",
                            "CLV-M2-eu_en_Disney_s_Aladdin",
                            "CLV-M2-eu_en_Dr_Robotnik_s_Mean_Bean_Machine",
                            "CLV-M2-eu_en_Dynamite_Headdy",
                            "CLV-M2-eu_en_Earthworm_Jim",
                            "CLV-M2-eu_en_Ecco_the_Dolphin",
                            "CLV-M2-eu_en_Eternal_Champions",
                            "CLV-M2-eu_en_Ghouls_n_Ghosts",
                            "CLV-M2-eu_en_Golden_Axe",
                            "CLV-M2-eu_en_Gunstar_Heroes",
                            "CLV-M2-eu_en_Kid_Chameleon",
                            "CLV-M2-eu_en_Landstalker",
                            "CLV-M2-eu_en_Light_Crusader",
                            "CLV-M2-eu_en_Mega_Man",
                            "CLV-M2-eu_en_Monster_World_IV",
                            "CLV-M2-eu_en_Mortal_Kombat_II",
                            "CLV-M2-eu_en_Phantasy_Star_IV",
                            "CLV-M2-eu_en_Road_Rash_II",
                            "CLV-M2-eu_en_STREET_FIGHTER_II",
                            "CLV-M2-eu_en_Shining_Force",
                            "CLV-M2-eu_en_Shinobi_III",
                            "CLV-M2-eu_en_Sonic_Spinball",
                            "CLV-M2-eu_en_Sonic_The_Hedgehog",
                            "CLV-M2-eu_en_Sonic_The_Hedgehog_2",
                            "CLV-M2-eu_en_Space_Harrier_II",
                            "CLV-M2-eu_en_Streets_of_Rage_2",
                            "CLV-M2-eu_en_Strider",
                            "CLV-M2-eu_en_Super_Fantasy_Zone",
                            "CLV-M2-eu_en_Sword_of_Vermilion",
                            "CLV-M2-eu_en_Tetris",
                            "CLV-M2-eu_en_The_Story_of_Thor",
                            "CLV-M2-eu_en_Thunder_Force_III",
                            "CLV-M2-eu_en_ToeJam_Earl",
                            "CLV-M2-eu_en_Vectorman",
                            "CLV-M2-eu_en_Virtua_Fighter_2",
                            "CLV-M2-eu_en_Wonder_Boy_in_Monster_World",
                            "CLV-M2-eu_en_World_of_Illusion"
                        });

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_JPN].FolderImagesSet = "TheWez1981-MegaDrive";
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_JPN].OriginalGames.Clear();
                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_JPN].OriginalGames.AddRange(new string[] {
                            "CLV-M2-jp_jp_Alisia_Dragoon",
                            "CLV-M2-jp_jp_Castlevania_Bloodlines",
                            "CLV-M2-jp_jp_Columns",
                            "CLV-M2-jp_jp_Comix_Zone",
                            "CLV-M2-jp_jp_Contra_Hard_Corps",
                            "CLV-M2-jp_jp_Darius",
                            "CLV-M2-jp_jp_Disney_s_Aladdin",
                            "CLV-M2-jp_jp_Dyna_Brothers_2",
                            "CLV-M2-jp_jp_Dynamite_Headdy",
                            "CLV-M2-jp_jp_Game_no_Kanzume",
                            "CLV-M2-jp_jp_Gauntlet_IV",
                            "CLV-M2-jp_jp_Ghouls_n_Ghosts",
                            "CLV-M2-jp_jp_Golden_Axe",
                            "CLV-M2-jp_jp_Gunstar_Heroes",
                            "CLV-M2-jp_jp_Landstalker",
                            "CLV-M2-jp_jp_Langrisser_II",
                            "CLV-M2-jp_jp_Lord_Monarch",
                            "CLV-M2-jp_jp_M_U_S_H_A",
                            "CLV-M2-jp_jp_Mado_Monogatari_I",
                            "CLV-M2-jp_jp_Marble_Madness",
                            "CLV-M2-jp_jp_Mega_Man_The_Wily_Wars",
                            "CLV-M2-jp_jp_Monster_World_IV",
                            "CLV-M2-jp_jp_Party_Quiz_MEGA_Q",
                            "CLV-M2-jp_jp_Phantasy_Star_IV",
                            "CLV-M2-jp_jp_Puyo_Puyo",
                            "CLV-M2-jp_jp_Puyo_Puyo_2",
                            "CLV-M2-jp_jp_Rent_A_Hero",
                            "CLV-M2-jp_jp_Road_Rash_II",
                            "CLV-M2-jp_jp_STREET_FIGHTER_II_SPECIAL_CHAMPION_EDITION",
                            "CLV-M2-jp_jp_Shining_Force",
                            "CLV-M2-jp_jp_Shining_Force_II",
                            "CLV-M2-jp_jp_Slap_Fight",
                            "CLV-M2-jp_jp_Snow_Bros",
                            "CLV-M2-jp_jp_Sonic_The_Hedgehog",
                            "CLV-M2-jp_jp_Sonic_The_Hedgehog_2",
                            "CLV-M2-jp_jp_Space_Harrier_II",
                            "CLV-M2-jp_jp_Streets_of_Rage_2",
                            "CLV-M2-jp_jp_Super_Fantasy_Zone",
                            "CLV-M2-jp_jp_Tant_R",
                            "CLV-M2-jp_jp_Target_Earth",
                            "CLV-M2-jp_jp_Tetris",
                            "CLV-M2-jp_jp_The_Hybrid_Front",
                            "CLV-M2-jp_jp_The_Revenge_of_Shinobi",
                            "CLV-M2-jp_jp_The_Story_of_Thor",
                            "CLV-M2-jp_jp_Thunder_Force_III",
                            "CLV-M2-jp_jp_World_of_Illusion_Starring_Mickey_Mouse_and_Donald_Duc",
                            "CLV-M2-jp_jp_Wrestleball",
                            "CLV-M2-jp_jp_Yu_Yu_Hakusho"
                        });

                        ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_ASIA].FolderImagesSet = "TheWez1981-MegaDrive";
                        //ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_ASIA].OriginalGames.Clear();
                        //ConfigIni.Instance.gamesCollectionSettings[hakchi.ConsoleType.MD_ASIA].OriginalGames.AddRange(new string[] {
                        //
                        //});

                        return true;
                    })
                }

            });
        }
    }
}
