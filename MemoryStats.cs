using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.hakchi_gui
{
    public static class MemoryStats
    {
        public static bool ExternalSaves { get; private set; }
        public static long WrittenGamesSize { get; private set; }
        public static long SaveStatesSize { get; private set; }
        public static long StorageTotal { get; private set; }
        public static long StorageUsed { get; private set; }
        public static long StorageFree { get; private set; }
        public static long ReservedMemory
        {
            get
            {
                if (ExternalSaves)
                    return 5;
                switch (ConfigIni.Instance.ConsoleType)
                {
                    default:
                    case hakchi.ConsoleType.NES:
                    case hakchi.ConsoleType.Famicom:
                        return 10;
                    case hakchi.ConsoleType.SNES_EUR:
                    case hakchi.ConsoleType.SNES_USA:
                    case hakchi.ConsoleType.SuperFamicom:
                        return 30;
                }
            }
        }
        public static long DefaultMaxGamesSize
        {
            get
            {
                switch (ConfigIni.Instance.ConsoleType)
                {
                    default:
                    case hakchi.ConsoleType.NES:
                    case hakchi.ConsoleType.Famicom:
                        return 300;
                    case hakchi.ConsoleType.SNES_EUR:
                    case hakchi.ConsoleType.SNES_USA:
                    case hakchi.ConsoleType.SuperFamicom:
                        return 200;
                }
            }
        }

        static MemoryStats()
        {
            Clear();
        }

        public static void Clear()
        {
            ExternalSaves = false;
            WrittenGamesSize = -1;
            SaveStatesSize = -1;
            StorageTotal = -1;
            StorageUsed = -1;
            StorageFree = -1;
        }

        public static void Refresh()
        {
            try
            {
                if (!hakchi.Shell.IsOnline)
                {
                    Clear();
                    return;
                }

                var shell = hakchi.Shell;
                var storage = shell.ExecuteSimple("df \"" + hakchi.RemoteGameSyncPath + "\" | tail -n 1 | awk '{ print $2 \" | \" $3 \" | \" $4 }'", 2000, true).Split('|');
                ExternalSaves = shell.ExecuteSimple("mount | grep /var/lib/clover").Trim().Length > 0;
                WrittenGamesSize = long.Parse(shell.ExecuteSimple("du -s \"" + hakchi.RemoteGameSyncPath + "\" | awk '{ print $1 }'", 2000, true)) * 1024;
                SaveStatesSize = long.Parse(shell.ExecuteSimple("du -s \"$(readlink /var/saves)\" | awk '{ print $1 }'", 2000, true)) * 1024;
                StorageTotal = long.Parse(storage[0]) * 1024;
                StorageUsed = long.Parse(storage[1]) * 1024;
                StorageFree = long.Parse(storage[2]) * 1024;

                Debug.WriteLine(string.Format("Storage size: {0:F1}MB, used: {1:F1}MB, free: {2:F1}MB", StorageTotal / 1024.0 / 1024.0, StorageUsed / 1024.0 / 1024.0, StorageFree / 1024.0 / 1024.0));
                Debug.WriteLine(string.Format("Used by games: {0:F1}MB", WrittenGamesSize / 1024.0 / 1024.0));
                Debug.WriteLine(string.Format("Used by save-states: {0:F1}MB", SaveStatesSize / 1024.0 / 1024.0));
                Debug.WriteLine(string.Format("Used by other files (mods, configs, etc.): {0:F1}MB", (StorageUsed - WrittenGamesSize - SaveStatesSize) / 1024.0 / 1024.0));
                Debug.WriteLine(string.Format("Available for games: {0:F1}MB", (StorageFree + WrittenGamesSize) / 1024.0 / 1024.0));
            }
            catch
            {
                Clear();
            }
        }

    }
}
