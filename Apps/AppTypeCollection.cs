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
            public string DefaultApp;
            public char Prefix;
            public Image DefaultCover;
        }

        public static AppInfo[] ApplicationTypes = new AppInfo[]
        {
            new AppInfo
            {
                Class = typeof(FdsGame),
                Extensions = new string[] {".fds"},
                DefaultApp = null,
                Prefix = 'D',
                DefaultCover = Resources.blank_fds
            },
            new AppInfo
            {
                Class = typeof(NesUGame),
                Extensions = new string[] {".nes", ".unf", ".unif"},
                DefaultApp = "/bin/nes",
                Prefix = 'I',
                DefaultCover = Resources.blank_jp
            },
            new AppInfo
            {
                Class = typeof(SnesGame),
                Extensions = new string[] { ".sfc", ".smc" },
                DefaultApp = "/bin/snes",
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us
            },
            new AppInfo
            {
                Class = typeof(N64Game),
                Extensions = new string[] { ".n64", ".z64", ".v64" },
                DefaultApp = "/bin/n64",
                Prefix = '6',
                DefaultCover = Resources.blank_n64
            },
            new AppInfo
            {
                Class = typeof(SmsGame),
                Extensions = new string[] { ".sms" },
                DefaultApp = "/bin/sms",
                Prefix = 'M',
                DefaultCover = Resources.blank_sms
            },
            new AppInfo
            {
                Class = typeof(SmsGame),
                Extensions = new string[] { ".sms" },
                DefaultApp = "/bin/sms",
                Prefix = 'M',
                DefaultCover = Resources.blank_sms
            },
                        new AppInfo
            {
                Class = typeof(GenesisGame),
                Extensions = new string[] { ".gen", ".md", ".smd" },
                DefaultApp = "/bin/md",
                Prefix = 'G',
                DefaultCover = Resources.blank_genesis
            },
                        new AppInfo
            {
                Class = typeof(GbGame),
                Extensions = new string[] { ".gb" },
                DefaultApp = "/bin/gb",
                Prefix = 'B',
                DefaultCover = Resources.blank_gb
            },
            new AppInfo
            {
                Class = typeof(GbcGame),
                Extensions = new string[] {".gbc"},
                DefaultApp = "/bin/gbc",
                Prefix = 'C',
                DefaultCover = Resources.blank_gbc
            },
            new AppInfo
            {
                Class = typeof(GbaGame),
                Extensions = new string[] {".gba"},
                DefaultApp = "/bin/gba",
                Prefix = 'A',
                DefaultCover = Resources.blank_gba
            },
            new AppInfo
            {
                Class = typeof(PceGame),
                Extensions = new string[] {".pce"},
                DefaultApp = "/bin/pce",
                Prefix = 'E',
                DefaultCover = Resources.blank_pce
            },
            new AppInfo
            {
                Class = typeof(GameGearGame),
                Extensions = new string[] {".gg"},
                DefaultApp = "/bin/gg",
                Prefix = 'R',
                DefaultCover = Resources.blank_app // TODO: icon for GameGear
            }
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
                if (exec.StartsWith(app.DefaultApp + " "))
                    return app;
            return null;
        }
    }
}
