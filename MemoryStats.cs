using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public static class MemoryStats
    {
        public static long StorageTotal { get; private set; }
        public static long StorageUsed { get; private set; }
        public static long StorageFree { get; private set; }
        public static bool ExternalSaves { get; private set; }
        public static long SaveStatesSize { get; private set; }
        public static long AllGamesSize { get; private set; }
        public static long NonMultibootGamesSize { get; private set; }
        public static long ExtraFilesSize { get; private set; }
        public static Dictionary<hakchi.ConsoleType, long> Collections { get; private set; }
        public static long ReservedMemory
        {
            get
            {
                if (ExternalSaves)
                    return 5 * 1024 * 1024;
                if (hakchi.IsNes(hakchi.DetectedConsoleType ?? hakchi.ConsoleType.Unknown))
                    return 10 * 1024 * 1024;
                if (hakchi.IsSnes(hakchi.DetectedConsoleType ?? hakchi.ConsoleType.Unknown))
                    return 30 * 1024 * 1024;
                return 0;
            }
        }
        public static long DefaultMaxGamesSize
        {
            get
            {
                if (hakchi.IsNes(hakchi.DetectedConsoleType ?? hakchi.ConsoleType.Unknown))
                    return 300;
                if (hakchi.IsSnes(hakchi.DetectedConsoleType ?? hakchi.ConsoleType.Unknown))
                    return 200;
                return 200;
            }
        }

        static MemoryStats()
        {
            Clear();
        }

        public static void Clear()
        {
            StorageTotal = -1;
            StorageUsed = -1;
            StorageFree = -1;
            ExternalSaves = false;
            SaveStatesSize = -1;
            AllGamesSize = -1;
            NonMultibootGamesSize = -1;
            ExtraFilesSize = -1;
            Collections = new Dictionary<hakchi.ConsoleType, long>();
        }

        public static void DebugDisplay()
        {
            if (StorageTotal != -1 || StorageUsed != -1 || StorageFree != -1)
            {
                Trace.WriteLine(string.Format("Storage size: {0:F1}MB, used: {1:F1}MB, free: {2:F1}MB", StorageTotal / 1024.0 / 1024.0, StorageUsed / 1024.0 / 1024.0, StorageFree / 1024.0 / 1024.0));
                Trace.Indent();
                Trace.WriteLine(string.Format("Used by all games: {0:F1}MB", AllGamesSize / 1024.0 / 1024.0));
                Trace.WriteLine(string.Format("Used by non multi-boot games: {0:F1}MB", NonMultibootGamesSize / 1024.0 / 1024.0));
                Trace.WriteLine(string.Format("Used by current games collection: {0:F1}MB", CurrentCollectionSize() / 1024.0 / 1024.0));
                Trace.WriteLine(string.Format("Used by save-states: {0:F1}MB", SaveStatesSize / 1024.0 / 1024.0));
                Trace.WriteLine(string.Format("Used by other files (mods, configs, etc.): {0:F1}MB", (StorageUsed - AllGamesSize - SaveStatesSize) / 1024.0 / 1024.0));
                Trace.WriteLine(string.Format("Reserved memory: {0:F1}MB", ReservedMemory / 1024.0 / 1024.0));
                Trace.WriteLine(string.Format("Available for games: {0:F1}MB", AvailableForGames() / 1024.0 / 1024.0));
                Trace.Unindent();
            }
        }

        public static void Refresh()
        {
            try
            {
                var shell = hakchi.Shell;
                if (!shell.IsOnline)
                {
                    Clear();
                    return;
                }

                var storage = shell.ExecuteSimple("df \"$(hakchi findGameSyncStorage)\" | tail -n 1 | awk '{ print $2 \" | \" $3 \" | \" $4 }'", 0, true).Split('|');
                StorageTotal = long.Parse(storage[0]) * 1024;
                StorageUsed = long.Parse(storage[1]) * 1024;
                StorageFree = long.Parse(storage[2]) * 1024;
                ExternalSaves = shell.ExecuteSimple("mount | grep /var/lib/clover/profiles").Trim().Length > 0;
                SaveStatesSize = long.Parse(shell.ExecuteSimple("du -s \"$(readlink /var/saves)\" | awk '{ print $1 }'", 0, true)) * 1024;
                getCollectionsSize();

                DebugDisplay();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
                Clear();
            }
        }

        public static long CurrentCollectionSize()
        {
            if (!hakchi.Shell.IsOnline)
                return -1;
            if (ConfigIni.Instance.SeparateGameStorage)
            {
                if (Collections.ContainsKey(ConfigIni.Instance.ConsoleType))
                {
                    return Collections[ConfigIni.Instance.ConsoleType];
                }
                return 0;
            }
            return NonMultibootGamesSize;
        }

        public static long AvailableForGames()
        {
            if (!hakchi.Shell.IsOnline)
                return -1;
            if (ConfigIni.Instance.SeparateGameStorage)
            {
                return StorageFree + CurrentCollectionSize() + NonMultibootGamesSize + ExtraFilesSize - ReservedMemory;
            }
            else
            {
                return StorageFree + AllGamesSize - ReservedMemory;
            }
        }

        private static void getCollectionsSize()
        {
            AllGamesSize = 0;
            NonMultibootGamesSize = 0;
            ExtraFilesSize = 0;
            Collections = new Dictionary<hakchi.ConsoleType, long>();

            string rawData = hakchi.Shell.ExecuteSimple("du -d 1 \"" + hakchi.RemoteGameSyncPath + "\" | head -n -1 | awk '{ print $2 \" | \" $1 }'", 0, true);
            if (!string.IsNullOrEmpty(rawData))
            {
                foreach (string line in rawData.Split(new char[] { '\n', '\r', '\f' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] col = line.Split(new string[] { " | " }, StringSplitOptions.RemoveEmptyEntries);
                    string dir = Path.GetFileName(col[0]);
                    long size = long.Parse(col[1]) * 1024;
                    if (dir == ".storage" || Regex.IsMatch(dir, @"^[0-9]{3}$"))
                    {
                        NonMultibootGamesSize += size;
                    }
                    else if (hakchi.SystemCodeToConsoleType.ContainsKey(dir))
                    {
                        Collections[hakchi.SystemCodeToConsoleType[dir]] = size;
                    }
                    else
                    {
                        ExtraFilesSize += size;
                    }
                    AllGamesSize += size;
                }
            }
        }

    }
}
