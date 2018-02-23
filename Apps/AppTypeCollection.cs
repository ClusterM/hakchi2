using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Diagnostics;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public static class AppTypeCollection
    {
        public class AppInfo : ICloneable
        {
            public string Name; // valid matches CoreInfo.Systems
            public Type Class;
            public string DefaultCore;
            public string[] LegacyApps;
            public char Prefix;
            public Image DefaultCover;
            public string GoogleSuffix;
            public bool Unknown = false;

            public object Clone()
            {
                var newAppInfo = new AppInfo()
                {
                    Name = string.Copy(Name),
                    Class = Class,
                    DefaultCore = string.Copy(DefaultCore),
                    LegacyApps = new string[LegacyApps.Length],
                    Prefix = Prefix,
                    DefaultCover = DefaultCover,
                    GoogleSuffix = string.Copy(GoogleSuffix),
                    Unknown = Unknown
                };
                LegacyApps.CopyTo(newAppInfo.LegacyApps, 0);

                return newAppInfo;
            }
        }

        public static readonly AppInfo UnknownApp = new AppInfo
        {
            Name = "Unknown System",
            Class = typeof(NesApplication),
            DefaultCore = string.Empty,
            LegacyApps = new string[] { },
            Prefix = 'Z',
            DefaultCover = Resources.blank_app,
            GoogleSuffix = "game",
            Unknown = true
        };

        public static AppInfo[] Apps = new AppInfo[]
        {
            new AppInfo
            {
                Name = "Nintendo - Nintendo Entertainment System",
                Class = typeof(NesGame),
                DefaultCore = "Kachikachi",
                LegacyApps = new string[] {"/bin/nes", "/bin/clover-kachikachi-wr", "/usr/bin/clover-kachikachi" },
                Prefix = 'H',
                DefaultCover = Resources.blank_nes,
                GoogleSuffix = "(fds | nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Family Computer Disk System",
                Class = typeof(FdsGame),
                DefaultCore = "Kachikachi",
                LegacyApps = new string[] {"/bin/fds", "/bin/clover-kachikachi-wr", "/usr/bin/clover-kachikachi" },
                Prefix = 'D',
                DefaultCover = Resources.blank_fds,
                GoogleSuffix = "(fds | nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Super Nintendo Entertainment System",
                Class = typeof(SnesGame),
                DefaultCore = "Canoe",
                LegacyApps = new string[] {"/bin/snes", "/bin/clover-canoe-shvc-wr", "/usr/bin/clover-canoe-shvc" },
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us,
                GoogleSuffix = "(snes | super nintendo)"
            },
            new AppInfo
            {
                Name = "Nintendo - Nintendo 64",
                Class = typeof(LibretroGame),
                DefaultCore = "mupen64plus",
                LegacyApps = new string[] {"/bin/n64" },
                Prefix = '6',
                DefaultCover = Resources.blank_n64,
                GoogleSuffix = "nintendo 64"
            },
            new AppInfo
            {
                Name = "Sega - Master System - Mark III",
                Class = typeof(LibretroGame),
                DefaultCore = "genesis_plus_gx",
                LegacyApps = new string[] {"/bin/sms" },
                Prefix = 'M',
                DefaultCover = Resources.blank_sms,
                GoogleSuffix = "(sms | sega master system)"
            },
            new AppInfo
            {
                Name = "Sega - Mega Drive - Genesis",
                Class = typeof(LibretroGame),
                DefaultCore = "genesis_plus_gx",
                LegacyApps = new string[] {"/bin/md" },
                Prefix = 'G',
                DefaultCover = Resources.blank_genesis,
                GoogleSuffix = "(genesis | mega drive)"
            },
            new AppInfo
            {
                Name = "Sega - 32X",
                Class = typeof(LibretroGame),
                DefaultCore = "picodrive",
                LegacyApps = new string[] {"/bin/32x" },
                Prefix = '3',
                DefaultCover = Resources.blank_32x,
                GoogleSuffix = "sega 32x"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy",
                Class = typeof(LibretroGame),
                DefaultCore = "gambatte",
                LegacyApps = new string[] {"/bin/gb" },
                Prefix = 'B',
                DefaultCover = Resources.blank_gb,
                GoogleSuffix = "(gameboy | game boy)"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy Color",
                Class = typeof(LibretroGame),
                DefaultCore = "gambatte",
                LegacyApps = new string[] {"/bin/gbc" },
                Prefix = 'C',
                DefaultCover = Resources.blank_gbc,
                GoogleSuffix = "(gameboy | game boy)"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy Advance",
                Class = typeof(LibretroGame),
                DefaultCore = "mgba",
                LegacyApps = new string[] {"/bin/gba" },
                Prefix = 'A',
                DefaultCover = Resources.blank_gba,
                GoogleSuffix = "gba"
            },
            new AppInfo
            {
                Name = "NEC - PC Engine - TurboGrafx 16",
                Class = typeof(LibretroGame),
                DefaultCore = "mednafen_pce_fast",
                LegacyApps = new string[] {"/bin/pce" },
                Prefix = 'E',
                DefaultCover = Resources.blank_pce,
                GoogleSuffix = "(pce | pc engine | turbografx 16)"
            },
            new AppInfo
            {
                Name = "Sega - Game Gear",
                Class = typeof(LibretroGame),
                DefaultCore = "genesis_plus_gx",
                LegacyApps = new string[] {"/bin/gg" },
                Prefix = 'R',
                DefaultCover = Resources.blank_gg,
                GoogleSuffix = "game gear"
            },
            new AppInfo
            {
                Name = "Atari - 2600",
                Class = typeof(LibretroGame),
                DefaultCore = "stella",
                LegacyApps = new string[] {"/bin/a26" },
                Prefix = 'T',
                DefaultCover = Resources.blank_2600,
                GoogleSuffix = "atari 2600"
            },
            new AppInfo
            {
                Name = "NEC - PC Engine SuperGrafx",
                Class = typeof(LibretroGame),
                DefaultCore = "mednafen_supergrafx",
                LegacyApps = new string[] {},
                Prefix = 'X',
                DefaultCover = Resources.blank_pce,
                GoogleSuffix = "(pce | pc engine | supergrafx 16)"
            },
            new AppInfo
            {
                Name = "DOS",
                Class = typeof(LibretroGame),
                DefaultCore = "dosbox",
                LegacyApps = new string[] {},
                Prefix = 'O',
                DefaultCover = Resources.blank_dos,
                GoogleSuffix = "(dos | dosbox)"
            }
        };

        public static AppInfo GetAppBySystem(string sys)
        {
            sys = sys.ToLower();
            foreach(var app in Apps)
            {
                if(sys == app.Name.ToLower())
                {
                    return app;
                }
            }
            return UnknownApp;
        }

        public static AppInfo GetAppByClass(Type cls)
        {
            foreach(var app in Apps)
            {
                if (cls == app.Class)
                {
                    return app;
                }
            }
            return UnknownApp;
        }

        public static AppInfo GetAppByExec(string exec)
        {
            exec = exec.ToLower();
            foreach (var app in Apps)
            {
                foreach(var bin in app.LegacyApps)
                {
                    if (exec.StartsWith(bin)) return app;
                }
            }
            return UnknownApp;
        }

    }
}
