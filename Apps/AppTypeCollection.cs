using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
        }

        public static AppInfo[] ApplicationTypes = new AppInfo[]
        {
            new AppInfo
            {
                Class = typeof(FdsGame),
                Extensions = new string[] {".fds"},
                DefaultApps = new string[] {},
                Prefix = 'D',
                DefaultCover = Resources.blank_fds
            },
            new AppInfo
            {
                Class = typeof(NesUGame),
                Extensions = new string[] {".nes", ".unf", ".unif"},
                DefaultApps = new string[] {"/bin/nes"},
                Prefix = 'I',
                DefaultCover = Resources.blank_jp
            },
            new AppInfo
            {
                Class = typeof(SnesGame),
                Extensions = new string[] { ".sfc", ".smc" },
                DefaultApps = new string[] {"/bin/snes"},
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us
            },
            new AppInfo
            {
                Class = typeof(N64Game),
                Extensions = new string[] { ".n64", ".z64", ".v64" },
                DefaultApps = new string[] {"/bin/n64"},
                Prefix = '6',
                DefaultCover = Resources.blank_n64
            },
            new AppInfo
            {
                Class = typeof(SmsGame),
                Extensions = new string[] { ".sms" },
                DefaultApps = new string[] {"/bin/sms"},
                Prefix = 'M',
                DefaultCover = Resources.blank_sms
            },
            new AppInfo
            {
                Class = typeof(GenesisGame),
                Extensions = new string[] { ".gen", ".md", ".smd" },
                DefaultApps = new string[] {"/bin/md"},
                Prefix = 'G',
                DefaultCover = Resources.blank_genesis
            },
            new AppInfo
            {
                Class = typeof(Sega32XGame),
                Extensions = new string[] { ".32x" },
                DefaultApps = new string[] {"/bin/32x"},
                Prefix = '3',
                DefaultCover = Resources.blank_32x
            },
            new AppInfo
            {
                Class = typeof(GbGame),
                Extensions = new string[] { ".gb" },
                DefaultApps = new string[] {"/bin/gb"},
                Prefix = 'B',
                DefaultCover = Resources.blank_gb
            },
            new AppInfo
            {
                Class = typeof(GbcGame),
                Extensions = new string[] {".gbc"},
                DefaultApps = new string[] {"/bin/gbc"},
                Prefix = 'C',
                DefaultCover = Resources.blank_gbc
            },
            new AppInfo
            {
                Class = typeof(GbaGame),
                Extensions = new string[] {".gba"},
                DefaultApps = new string[] {"/bin/gba"},
                Prefix = 'A',
                DefaultCover = Resources.blank_gba
            },
            new AppInfo
            {
                Class = typeof(PceGame),
                Extensions = new string[] {".pce"},
                DefaultApps = new string[] {"/bin/pce"},
                Prefix = 'E',
                DefaultCover = Resources.blank_pce
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {".gg"},
                DefaultApps = new string[] {"/bin/gg"},
                Prefix = 'R',
                DefaultCover = Resources.blank_gg
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {".a26"},
                DefaultApps = new string[] {"/bin/a26"},
                Prefix = 'T',
                DefaultCover = Resources.blank_2600
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {},
                DefaultApps = new string[] {"/bin/fba", "/bin/mame", "/bin/cps2", "/bin/neogeo" },
                Prefix = 'X',
                DefaultCover = Resources.blank_arcade
            },
        };

        public static AppInfo GetAppByExtension(string extension)
        {
            foreach (var app in ApplicationTypes)
                if (Array.IndexOf(app.Extensions, extension) >= 0)
                    return app;
            return null;
        }

        public static AppInfo GetAppByExec(string exec)
        {
            foreach (var app in ApplicationTypes)
                foreach (var cmd in app.DefaultApps)
                    if (exec.StartsWith(cmd + " "))
                        return app;
            return null;
        }
    }
}
