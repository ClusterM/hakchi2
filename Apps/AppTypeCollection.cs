using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

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
            public string[] Extensions;
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
                    Extensions = new string[Extensions.Length],
                    Prefix = Prefix,
                    DefaultCover = DefaultCover,
                    GoogleSuffix = string.Copy(GoogleSuffix),
                    Unknown = Unknown
                };
                LegacyApps.CopyTo(newAppInfo.LegacyApps, 0);
                Extensions.CopyTo(newAppInfo.Extensions, 0);

                return newAppInfo;
            }
        }

        public static readonly AppInfo UnknownApp = new AppInfo
        {
            Name = "Unknown System",
            Class = typeof(UnknownGame),
            DefaultCore = string.Empty,
            LegacyApps = new string[] { },
            Extensions = new string[] { },
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
                DefaultCore = "fceumm",
                LegacyApps = new string[] {"/bin/nes", "/bin/clover-kachikachi-wr", "/usr/bin/clover-kachikachi" },
                Extensions = new string[] {".nes" },
                Prefix = 'H',
                DefaultCover = Resources.blank_nes,
                GoogleSuffix = "(fds | nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Family Computer Disk System",
                Class = typeof(FdsGame),
                DefaultCore = "nestopia",
                LegacyApps = new string[] {"/bin/fds", "/bin/clover-kachikachi-wr", "/usr/bin/clover-kachikachi" },
                Extensions = new string[] {".fds", ".qd" },
                Prefix = 'D',
                DefaultCover = Resources.blank_fds,
                GoogleSuffix = "(fds | nes | famicom)"
            },
            new AppInfo
            {
                Name = "Nintendo - Super Nintendo Entertainment System",
                Class = typeof(SnesGame),
                DefaultCore = "snes9x",
                LegacyApps = new string[] {"/bin/snes", "/bin/clover-canoe-shvc-wr -rom", "/usr/bin/clover-canoe-shvc -rom" },
                Extensions = new string[] {".sfrom", ".smc", ".sfc", ".fig", ".swc" },
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us,
                GoogleSuffix = "(snes | super nintendo)"
            },
            new AppInfo
            {
                Name = "Nintendo - Nintendo 64",
                Class = typeof(LibretroGame),
                DefaultCore = "glupen64",
                LegacyApps = new string[] {"/bin/n64" },
                Extensions = new string[] {".n64", ".z64", ".v64" },
                Prefix = '6',
                DefaultCover = Resources.blank_n64,
                GoogleSuffix = "nintendo 64"
            },
            new AppInfo
            {
                Name = "Nintendo - Game Boy",
                Class = typeof(LibretroGame),
                DefaultCore = "gambatte",
                LegacyApps = new string[] {"/bin/gb" },
                Extensions = new string[] {".gb", ".sgb" },
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
                Extensions = new string[] {".gbc" },
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
                Extensions = new string[] {".gba" },
                Prefix = 'A',
                DefaultCover = Resources.blank_gba,
                GoogleSuffix = "gba"
            },
            new AppInfo
            {
                Name = "Sega - Master System - Mark III",
                Class = typeof(LibretroGame),
                DefaultCore = "genesis_plus_gx",
                LegacyApps = new string[] {"/bin/sms" },
                Extensions = new string[] {".sms" },
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
                Extensions = new string[] {".md", ".mdx", ".smd", ".gen", ".68k" },
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
                Extensions = new string[] {".32x" },
                Prefix = '3',
                DefaultCover = Resources.blank_32x,
                GoogleSuffix = "sega 32x"
            },
            new AppInfo
            {
                Name = "Sega - Game Gear",
                Class = typeof(LibretroGame),
                DefaultCore = "genesis_plus_gx",
                LegacyApps = new string[] {"/bin/gg" },
                Extensions = new string[] {".gg", ".sg" },
                Prefix = 'R',
                DefaultCover = Resources.blank_gg,
                GoogleSuffix = "game gear"
            },
            new AppInfo
            {
                Name = "NEC - PC Engine - TurboGrafx 16",
                Class = typeof(LibretroGame),
                DefaultCore = "mednafen_pce_fast",
                LegacyApps = new string[] {"/bin/pce" },
                Extensions = new string[] {".pce" },
                Prefix = 'E',
                DefaultCover = Resources.blank_pce,
                GoogleSuffix = "(pce | pc engine | turbografx 16)"
            },
            new AppInfo
            {
                Name = "NEC - PC Engine SuperGrafx",
                Class = typeof(LibretroGame),
                DefaultCore = "mednafen_supergrafx",
                LegacyApps = new string[] { },
                Extensions = new string[] {".sgx" },
                Prefix = 'E',
                DefaultCover = Resources.blank_pce,
                GoogleSuffix = "(pce | pc engine | supergrafx 16)"
            },
            new AppInfo
            {
                Name = "Atari - 2600",
                Class = typeof(LibretroGame),
                DefaultCore = "stella",
                LegacyApps = new string[] {"/bin/a26" },
                Extensions = new string[] {".a26" },
                Prefix = 'T',
                DefaultCover = Resources.blank_2600,
                GoogleSuffix = "atari 2600"
            },
            new AppInfo
            {
                Name = "DOS",
                Class = typeof(LibretroGame),
                DefaultCore = "dosbox",
                LegacyApps = new string[] { },
                Extensions = new string[] {".bat", ".com", ".exe", ".conf" },
                Prefix = 'O',
                DefaultCover = Resources.blank_dos,
                GoogleSuffix = "(dos | dosbox)"
            },
            new AppInfo
            {
                Name = "Neo Geo",
                Class = typeof(LibretroGame),
                DefaultCore = "fbalpha2012_neogeo",
                LegacyApps = new string[] { },
                Extensions = new string[] { },
                Prefix = 'N',
                DefaultCover = Resources.blank_neogeo,
                GoogleSuffix = "neo geo"
            },
            new AppInfo
            {
                Name = "SquashFS",
                Class = typeof(UnknownGame),
                DefaultCore = "hsqs",
                LegacyApps = new string[] {"/bin/hsqs" },
                Extensions = new string[] {".hsqs" },
                Prefix = 'L',
                DefaultCover = Resources.blank_dos,
                GoogleSuffix = "hsqs"
            },
            new AppInfo
            {
                Name = "Shell Script",
                Class = typeof(UnknownGame),
                DefaultCore = "sh",
                LegacyApps = new string[] {"/bin/sh" },
                Extensions = new string[] {".sh" },
                Prefix = 'L',
                DefaultCover = Resources.blank_dos,
                GoogleSuffix = "bash script"
            },
            new AppInfo
            {
                Name = "Nintendo - Super Nintendo Entertainment System (MSU-1)",
                Class = typeof(LibretroGame),
                DefaultCore = "msu",
                LegacyApps = new string[] {"/bin/snes9x" },
                Extensions = new string[] {".msu" },
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us,
                GoogleSuffix = "(snes | super nintendo)"
            }
        };

        private static char[] Prefixes = null;
        public static char GetAvailablePrefix(string src)
        {
            src = Regex.Replace(src, @"[^A-Za-z0-9]+", "").ToUpper();
            if (Prefixes == null)
            {
                var pre = new List<char>();
                foreach(var app in Apps)
                    pre.Add(app.Prefix);
                Prefixes = pre.Distinct().ToArray();
                Array.Sort(Prefixes);
            }

            foreach (var l in src.ToCharArray())
            {
                if (!Prefixes.Contains(l))
                    return l;
            }

            char letter = src.Length > 0 ? src[0] : 'Z';
            while (Prefixes.Contains(letter))
            {
                ++letter;
                if (letter > 'Z') letter = 'A';
            }
            return letter;
        }

        public static AppInfo GetAppBySystem(string sys)
        {
            sys = sys.ToLower().Trim();
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
            exec = Regex.Replace(exec.ToLower().Trim(), "['\\\"]|(\\.7z)", " ") + " ";
            foreach (var app in Apps)
            {
                foreach(var bin in app.LegacyApps)
                {
                    if (exec.StartsWith(bin + " "))
                    {
                        if (app.Extensions.Length == 0)
                            return app;
                        foreach(var ext in app.Extensions)
                        {
                            if (exec.Contains(ext + " "))
                                return app;
                        }
                    }
                }
            }
            return UnknownApp;
        }

        public static AppInfo GetAppByExtension(string ext)
        {
            ext = ext.ToLower().Trim();
            foreach (var app in Apps)
            {
                foreach (var e in app.Extensions)
                {
                    if (e == ext)
                    {
                        return app;
                    }
                }
            }
            return UnknownApp;
        }
    }
}
