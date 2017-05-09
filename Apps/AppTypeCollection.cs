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
            public string SystemName;
            public List<Type> Class;
            public string[] Extensions;
            public string[] DefaultApps;
            public char Prefix;
            public Image DefaultCover;
        }

        public static AppInfo[] ApplicationTypes = new AppInfo[]
        {
            new AppInfo
            {
                SystemName = "Famicom Disk System",
                Class = new List<Type>(){typeof(FdsGame) },
                Extensions = new string[] {".fds"},
                DefaultApps = new string[] {},
                Prefix = 'D',
                DefaultCover = Resources.blank_fds
            },
            new AppInfo
            {
                SystemName = "NES",
                Class = new List<Type>(){typeof(NesUGame),typeof(NesGame) },
                Extensions = new string[] {".nes", ".unf", ".unif"},
                DefaultApps = new string[] {"/bin/nes"},
                Prefix = 'I',
                DefaultCover = Resources.blank_jp
            },
           new AppInfo
            {
                SystemName = "NES Default Game",
                Class = new List<Type>(){typeof(NesDefaultGame),typeof(NesGame) },
                Extensions = new string[] {},
                DefaultApps = new string[] {},
                Prefix = 'S',
                DefaultCover = Resources.blank_nes
            },
            new AppInfo
            {
                SystemName = "SNES",
                Class = new List<Type>(){typeof(SnesGame) },
                Extensions = new string[] { ".sfc", ".smc" },
                DefaultApps = new string[] {"/bin/snes"},
                Prefix = 'U',
                DefaultCover = Resources.blank_snes_us
            },
            new AppInfo
            {
                SystemName = "Nintendo 64",
                Class = new List<Type>(){typeof(N64Game) },
                Extensions = new string[] { ".n64", ".z64", ".v64" },
                DefaultApps = new string[] {"/bin/n64"},
                Prefix = '6',
                DefaultCover = Resources.blank_n64
            },
            new AppInfo
            {
                SystemName = "Sega Master System",
                Class = new List<Type>(){typeof(SmsGame) },
                Extensions = new string[] { ".sms" },
                DefaultApps = new string[] {"/bin/sms"},
                Prefix = 'M',
                DefaultCover = Resources.blank_sms
            },
            new AppInfo
            {
                SystemName = "Sega Genesis",
                Class = new List<Type>(){typeof(GenesisGame) },
                Extensions = new string[] { ".gen", ".md", ".smd" },
                DefaultApps = new string[] {"/bin/md"},
                Prefix = 'G',
                DefaultCover = Resources.blank_genesis
            },
            new AppInfo
            {
                SystemName = "Sega 32X",
                Class = new List<Type>(){ typeof(Sega32XGame) },
                Extensions = new string[] { ".32x" },
                DefaultApps = new string[] {"/bin/32x"},
                Prefix = '3',
                DefaultCover = Resources.blank_32x
            },
            new AppInfo
            {
                SystemName = "GameBoy",
                Class = new List<Type>(){typeof(GbGame) },
                Extensions = new string[] { ".gb" },
                DefaultApps = new string[] {"/bin/gb"},
                Prefix = 'B',
                DefaultCover = Resources.blank_gb
            },
            new AppInfo
            {
                SystemName = "GameBoy Color",
                Class = new List<Type>(){typeof(GbcGame) },
                Extensions = new string[] {".gbc"},
                DefaultApps = new string[] {"/bin/gbc"},
                Prefix = 'C',
                DefaultCover = Resources.blank_gbc
            },
            new AppInfo
            {
                SystemName = "GameBoy Advance",
                Class = new List<Type>(){typeof(GbaGame) },
                Extensions = new string[] {".gba"},
                DefaultApps = new string[] {"/bin/gba"},
                Prefix = 'A',
                DefaultCover = Resources.blank_gba
            },
            new AppInfo
            {
                SystemName = "PC Engine",
                Class = new List<Type>(){typeof(PceGame) },
                Extensions = new string[] {".pce"},
                DefaultApps = new string[] {"/bin/pce"},
                Prefix = 'E',
                DefaultCover = Resources.blank_pce
            },
            new AppInfo
            {
                SystemName = "Sega GameGear",
                Class = new List<Type>(){typeof(GameGearGame) },
                Extensions = new string[] {".gg"},
                DefaultApps = new string[] {"/bin/gg"},
                Prefix = 'R',
                DefaultCover = Resources.blank_gg
            },
            new AppInfo
            {
                SystemName="Atari 2600",
                Class = new List<Type>(){typeof(Atari2600Game) },
                Extensions = new string[] {".a26"},
                DefaultApps = new string[] {"/bin/a26"},
                Prefix = 'T',
                DefaultCover = Resources.blank_2600
            },
            new AppInfo
            {
                SystemName = "Arcade",
                Class = new List<Type>(){typeof(GameGearGame) },
                Extensions = new string[] {},
                DefaultApps = new string[] {"/bin/fba", "/bin/mame", "/bin/cps2", "/bin/neogeo" },
                Prefix = 'X',
                DefaultCover = Resources.blank_arcade
            },
        };
        public static AppInfo GetAppByClass(Type theClass)
        {
            foreach (var app in ApplicationTypes)
                if(app.Class.Contains(theClass))
                    return app;
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
            foreach (var app in ApplicationTypes)
                foreach (var cmd in app.DefaultApps)
                    if (exec.StartsWith(cmd + " "))
                        return app;
            return null;
        }
    }
}
