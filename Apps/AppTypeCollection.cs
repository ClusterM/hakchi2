using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    static class AppTypeCollection
    {
        //public delegate NesMiniApplication 

        public class AppInfo
        {
            public Type Class;
            public string[] Extensions;
            public string[] DefaultApps;
            public char Prefix;
            public Image DefaultCover;
            public string Name;
        }

        public static AppInfo[] ApplicationTypes = new AppInfo[]
        {
            new AppInfo
            {
                Class = typeof(NesGame),
                Extensions = new string[] {".nes"},
                DefaultApps = new string[] { "/bin/nes", "/bin/clover-kachikachi-wr", "/usr/bin/clover-kachikachi" },
                Prefix = 'H',
                DefaultCover = Resources.blank_nes,
                Name = "Nintendo NES"
            },
            new AppInfo
            {
                Class = typeof(NesUGame),
                Extensions = new string[] {".unf", ".unif", ".nes", ".fds" },
                DefaultApps = new string[] {"/bin/nes"},
                Prefix = 'I',
                DefaultCover = Resources.blank_jp,
                Name = "NES/Famicom Disk System"
            },
            new AppInfo
            {
                Class = typeof(FdsGame),
                Extensions = new string[] {".fds"},
                DefaultApps = new string[] { "/bin/nes", "/bin/clover-kachikachi-wr", "/usr/bin/clover-kachikachi" },
                Prefix = 'D',
                DefaultCover = Resources.blank_fds,
                Name = "Famicom Disk System"
            },
            new AppInfo
            {
                Class = typeof(SnesGame),
                Extensions = new string[] { ".sfc", ".smc", ".sfrom" },
                DefaultApps = new string[] { "/bin/snes", "/bin/clover-canoe-shvc-wr", "/usr/bin/clover-canoe-shvc" },
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us,
                Name = "Nintendo Super NES"
            },
            new AppInfo
            {
                Class = typeof(N64Game),
                Extensions = new string[] { ".n64", ".z64", ".v64" },
                DefaultApps = new string[] {"/bin/n64"},
                Prefix = '6',
                DefaultCover = Resources.blank_n64,
                Name = "Nintendo 64"
            },
            new AppInfo
            {
                Class = typeof(SmsGame),
                Extensions = new string[] { ".sms" },
                DefaultApps = new string[] {"/bin/sms"},
                Prefix = 'M',
                DefaultCover = Resources.blank_sms,
                Name = "SEGA Master System"
            },
            new AppInfo
            {
                Class = typeof(GenesisGame),
                Extensions = new string[] { ".gen", ".md", ".smd" },
                DefaultApps = new string[] {"/bin/md"},
                Prefix = 'G',
                DefaultCover = Resources.blank_genesis,
                Name = "SEGA Genesis"
            },
            new AppInfo
            {
                Class = typeof(Sega32XGame),
                Extensions = new string[] { ".32x" },
                DefaultApps = new string[] {"/bin/32x"},
                Prefix = '3',
                DefaultCover = Resources.blank_32x,
                Name = "SEGA 32X"
            },
            new AppInfo
            {
                Class = typeof(GbGame),
                Extensions = new string[] { ".gb" },
                DefaultApps = new string[] {"/bin/gb"},
                Prefix = 'B',
                DefaultCover = Resources.blank_gb,
                Name = "Nintendo Game Boy"
            },
            new AppInfo
            {
                Class = typeof(GbcGame),
                Extensions = new string[] {".gbc"},
                DefaultApps = new string[] {"/bin/gbc"},
                Prefix = 'C',
                DefaultCover = Resources.blank_gbc,
                Name = "Nintendo Game Boy Color"
            },
            new AppInfo
            {
                Class = typeof(GbaGame),
                Extensions = new string[] {".gba"},
                DefaultApps = new string[] {"/bin/gba"},
                Prefix = 'A',
                DefaultCover = Resources.blank_gba,
                Name = "Nintendo Game Boy Advance"
            },
            new AppInfo
            {
                Class = typeof(PceGame),
                Extensions = new string[] {".pce"},
                DefaultApps = new string[] {"/bin/pce"},
                Prefix = 'E',
                DefaultCover = Resources.blank_pce,
                Name = "NEC Turbo Grafx 16"
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {".gg"},
                DefaultApps = new string[] {"/bin/gg"},
                Prefix = 'R',
                DefaultCover = Resources.blank_gg,
                Name = "SEGA Game Gear"
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {".a26"},
                DefaultApps = new string[] {"/bin/a26"},
                Prefix = 'T',
                DefaultCover = Resources.blank_2600,
                Name = "ATARI 2600"
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {},
                DefaultApps = new string[] {"/bin/fba", "/bin/mame", "/bin/cps2", "/bin/neogeo" },
                Prefix = 'X',
                DefaultCover = Resources.blank_arcade,
                Name = "Arcade System"
            },
        };

        public static AppInfo GetAppByClass(Type t)
        {
            foreach(var app in ApplicationTypes)
                if (app.Class == t) return app;
            return null;
        }

        public static AppInfo GetAppByExtension(string extension)
        {
            foreach (var app in ApplicationTypes)
                if (Array.IndexOf(app.Extensions, extension) >= 0)
                    return app;
            return null;
        }

        public static AppInfo GetAppByExec(string exec)
        {
            exec = Regex.Replace(exec, "['\\\"]|(\\.7z)", " ")+" ";
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
            return null;
        }
    }
}
