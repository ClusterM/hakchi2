using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public static class AppTypeCollection
    {
        public class AppInfo
        {
            public string Name; // valid matches CoreInfo.Systems
            public string LegacyName;
            public Type Class;
            public string[] Extensions;
            public string[] DefaultApps;
            public char Prefix;
            public Image DefaultCover;
            public string GoogleSuffix;
            public bool Unknown = false;
        }

        public static readonly AppInfo UnknownApplicationType = new AppInfo
        {
            Name = "Unknown System",
            LegacyName = "NesApplication",
            Class = typeof(NesApplication),
            Extensions = new string[] { },
            DefaultApps = new string[] { },
            Prefix = 'Z',
            DefaultCover = Resources.blank_app,
            GoogleSuffix = "game",
            Unknown = true
        };

        public static readonly AppInfo LibretroApplicationType = new AppInfo
        {
            Name = "Libretro System",
            LegacyName = "LibretroGame",
            Class = typeof(LibretroGame),
            Extensions = new string[] { },
            DefaultApps = new string[] { },
            Prefix = 'L',
            DefaultCover = Resources.blank_app,
            GoogleSuffix = "game"
        };

        public static AppInfo[] ApplicationTypes = new AppInfo[]
        {
            new AppInfo
            {
                Name = "Nintendo - Nintendo Entertainment System",
                LegacyName = "NesGame",
                Class = typeof(NesGame),
                Extensions = new string[] {".nes" },
                DefaultApps = new string[] {"/bin/nes", "/bin/clover-kachikachi-wr", "usr/bin/clover-kachikachi" },
                Prefix = 'H',
                DefaultCover = Resources.blank_nes,
                GoogleSuffix = "(nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Nintendo Entertainment System",
                LegacyName = "UNesGame",
                Class = typeof(NesGame),
                Extensions = new string[] {".unf", ".unif" },
                DefaultApps = new string[] {"/bin/nes", "/bin/clover-kachikachi-wr", "usr/bin/clover-kachikachi" },
                Prefix = 'I',
                DefaultCover = Resources.blank_jp,
                GoogleSuffix = "(fds | nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Family Computer Disk System",
                LegacyName = "FdsGame",
                Class = typeof(FdsGame),
                Extensions = new string[] {".fds" },
                DefaultApps = new string[] {"/bin/nes", "/bin/clover-kachikachi-wr", "usr/bin/clover-kachikachi" },
                Prefix = 'D',
                DefaultCover = Resources.blank_fds,
                GoogleSuffix = "(fds | nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Super Nintendo Entertainment System",
                LegacyName = "SnesGame",
                Class = typeof(SnesGame),
                Extensions = new string[] {".sfrom",".smc",".sfc" },
                DefaultApps = new string[] { "/bin/snes", "/bin/clover-canoe-shvc-wr", "usr/bin/clover-canoe-shvc" },
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us,
                GoogleSuffix = "(snes | super nintendo)"
            },
            new AppInfo
            {
                Name = "Nintendo - Nintendo 64",
                LegacyName = "N64Game",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".n64", ".z64", ".v64" },
                DefaultApps = new string[] {"/bin/n64" },
                Prefix = '6',
                DefaultCover = Resources.blank_n64,
                GoogleSuffix = "nintendo 64"
            },
            new AppInfo
            {
                Name = "Sega - Master System - Mark III",
                LegacyName = "SmsGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".sms" },
                DefaultApps = new string[] {"/bin/sms" },
                Prefix = 'M',
                DefaultCover = Resources.blank_sms,
                GoogleSuffix = "(sms | sega master system)"
            },
            new AppInfo
            {
                Name = "Sega - Mega Drive - Genesis",
                LegacyName = "GenesisGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".gen", ".md", ".smd" },
                DefaultApps = new string[] {"/bin/md" },
                Prefix = 'G',
                DefaultCover = Resources.blank_genesis,
                GoogleSuffix = "(genesis | mega drive)"
            },
            new AppInfo
            {
                Name = "Sega - 32X",
                LegacyName = "Sega32XGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".32x" },
                DefaultApps = new string[] {"/bin/32x" },
                Prefix = '3',
                DefaultCover = Resources.blank_32x,
                GoogleSuffix = "sega 32x"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy",
                LegacyName = "GbGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".gb" },
                DefaultApps = new string[] {"/bin/gb" },
                Prefix = 'B',
                DefaultCover = Resources.blank_gb,
                GoogleSuffix = "(gameboy | game boy)"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy Color",
                LegacyName = "GbcGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".gbc" },
                DefaultApps = new string[] {"/bin/gbc" },
                Prefix = 'C',
                DefaultCover = Resources.blank_gbc,
                GoogleSuffix = "(gameboy | game boy)"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy Advance",
                LegacyName = "GbaGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".gba" },
                DefaultApps = new string[] {"/bin/gba" },
                Prefix = 'A',
                DefaultCover = Resources.blank_gba,
                GoogleSuffix = "gba"
            },
            new AppInfo
            {
                Name = "NEC - PC Engine - TurboGrafx 16",
                LegacyName = "PceGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".pce" },
                DefaultApps = new string[] {"/bin/pce" },
                Prefix = 'E',
                DefaultCover = Resources.blank_pce,
                GoogleSuffix = "(pce | pc engine | turbografx 16)"
            },
            new AppInfo
            {
                Name = "Sega - Game Gear",
                LegacyName = "GameGearGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".gg" },
                DefaultApps = new string[] {"/bin/gg" },
                Prefix = 'R',
                DefaultCover = Resources.blank_gg,
                GoogleSuffix = "game gear"
            },
            new AppInfo
            {
                Name = "Atari - 2600",
                LegacyName = "Atari2600Game",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".a26" },
                DefaultApps = new string[] {"/bin/a26" },
                Prefix = 'T',
                DefaultCover = Resources.blank_2600,
                GoogleSuffix = "atari 2600"
            },
            new AppInfo
            {
                Name = "NEC - PC Engine SuperGrafx",
                LegacyName = "PceGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".sgx" },
                DefaultApps = new string[] {},
                Prefix = 'X',
                DefaultCover = Resources.blank_pce,
                GoogleSuffix = "(pce | pc engine | supergrafx 16)"
            },
            new AppInfo
            {
                Name = "DOS",
                LegacyName = "DosGame",
                Class = typeof(LibretroGame),
                Extensions = new string[] {".exe", ".com", ".bat", ".conf" },
                DefaultApps = new string[] {},
                Prefix = 'O',
                DefaultCover = Resources.blank_dos,
                GoogleSuffix = "(dos | dosbox)"
            }
        };

        public static AppInfo GetAppByLegacyName(string name)
        {
            foreach (var app in ApplicationTypes)
                if (name.ToLower() == app.LegacyName.ToLower())
                    return app;
            return UnknownApplicationType;
        }

        public static AppInfo GetAppByExtension(string extension)
        {
            foreach (var app in ApplicationTypes)
                if (Array.IndexOf(app.Extensions, extension) >= 0)
                    return app;
            return UnknownApplicationType;
        }

        public static AppInfo GetAppByExec(string exec)
        {
            exec = Regex.Replace(exec, "['\\\"]|(\\.7z)", " ") + " ";
            foreach (var app in ApplicationTypes)
                foreach (var cmd in app.DefaultApps)
                    if (exec.StartsWith(cmd + " "))
                    {
                        if (app.Extensions.Length == 0)
                            return app;
                        foreach (var ext in app.Extensions)
                        {
                            if (exec.Contains(ext + " "))
                                return app;
                        }
                    }
            return UnknownApplicationType;
        }
    }
}
