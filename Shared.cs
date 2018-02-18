using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    static class Shared
    {
        public const string SquashFsPath = "/var/squashfs";

        public static Bitmap LoadBitmapCopy(string path)
        {
            Bitmap bmp;
            using (var img = Image.FromFile(path))
            {
                bmp = new Bitmap(img);
            }
            return bmp;
        }

        public static uint CRC32(byte[] data)
        {
            uint poly = 0xedb88320;
            uint[] table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                table[i] = temp;
            }
            uint crc = 0xffffffff;
            for (int i = 0; i < data.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ data[i]);
                crc = (uint)((crc >> 8) ^ table[index]);
            }
            return ~crc;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string PathCombine(params string[] pathSegments)
        {
            if (pathSegments.Length == 1) return pathSegments[0];
            if (pathSegments.Length < 1) throw new ArgumentOutOfRangeException("Not enough path segments");

            string output = pathSegments[0];
            for(int i = 1; i < pathSegments.Length; i++)
            {
                output = Path.Combine(output, pathSegments[i]);
            }
            return output;
        }
        
        public static bool isFirstRun()
        {

            if (AppVersion > (new Version(Settings.Default.LastNonPortableVersion)))
            {
                Settings.Default.LastNonPortableVersion = AppVersion.ToString();
                Settings.Default.Save();
                return true;
            }
            return false;
        }

        public static Version AppVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public static string AppDisplayVersion
        {
            get
            {
                Version version = AppVersion;
                string v = $"{version.Major - 2}.{version.Minor}.{version.Build}";
                if (version.Revision > 0)
                    v += $".{version.Revision}";
                return v;
            }
        }

        public static string MinimumHakchiBootVersion
        {
            get
            {
                return "1.0.1";
            }
        }
        
        public static string MinimumHakchiKernelVersion
        {
            get
            {
                return "3.4.112";
            }
        }
        
        public static string MinimumHakchiScriptVersion
        {
            get
            {
                return "1.0.3";
            }
        }
        
        public static string MinimumHakchiScriptRevision
        {
            get
            {
                return "110";
            }
        }

        public static string CurrentHakchiScriptVersion
        {
            get
            {
                return "1.0.3";
            }
        }

        public static string CurrentHakchiScriptRevision
        {
            get
            {
                return "110";
            }
        }

        public static readonly string[] SizeSuffixes =
                   { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            if (mag == 0) { decimalPlaces = 0; }

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool skipExistingFiles = false, bool overwriteExistingFiles = false)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = new FileInfo(Path.Combine(destDirName, file.Name)).FullName;
                if (File.Exists(temppath) && skipExistingFiles)
                {
                    continue;
                }
                else
                {
                    file.CopyTo(temppath, overwriteExistingFiles); // TODO : redundant
                }

            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, skipExistingFiles, overwriteExistingFiles);
                }
            }
        }

        public static void DirectoryDeleteInside(string dirName)
        {
            if (!Directory.Exists(dirName)) // no error, just return
                return;

            // delete files first
            string[] files = Directory.GetFiles(dirName);
            foreach(string file in files)
            {
                File.Delete(file);
            }

            // recurse subdirs, then refresh, pause and delete (seems to prevent explorer bug in most cases)
            string[] dirs = Directory.GetDirectories(dirName);
            foreach(string dir in dirs)
            {
                DirectoryDeleteInside(dir);
                new DirectoryInfo(dir).Refresh();
                System.Threading.Thread.Sleep(0);
                Directory.Delete(dir);
            }
        }

        public static long DirectorySize(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException($"Null path cannot be used with this method.");

            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
                return 0;

            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                size += DirectorySize(subdir.FullName);
            }
            return size;
        }

        // concatenate an arbitrary number of arrays
        public static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }
            return result;
        }

        // workaround to prevent flickering with ListView controls
        public static void DoubleBuffered(this System.Windows.Forms.Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        public static bool IsVersionGreaterOrEqual(string given, string minimum)
        {
            return new Version(given).CompareTo(new Version(minimum)) > -1;
        }

        public static HashSet<ApplicationFileInfo> GetApplicationFileInfoForDirectory(string rootDirectory, bool recursive = true)
        {
            var fileInfoSet = new HashSet<ApplicationFileInfo>();
            var filepaths = Directory.GetFiles(rootDirectory, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (string path in filepaths)
            {
                // follow through on tarstreamref files
                string pathToRead = path;
                bool isRefFile = false;

                if (TarStream.refRegex.IsMatch(path))
                {
                    pathToRead = File.ReadAllText(path);
                    isRefFile = true;
                }

                // make the filepath match what we'd get back from the console
                string canonicalPath = "." + path.Remove(0, rootDirectory.Length).Replace("\\", "/").Replace(".tarstreamref", "");
                FileInfo f = new FileInfo(pathToRead);
                fileInfoSet.Add(new ApplicationFileInfo(canonicalPath, f.Length, f.LastWriteTimeUtc, isRefFile));
            }

            return fileInfoSet;
        }

        public static HashSet<ApplicationFileInfo> GetApplicationFileInfoFromConsoleOutput(string output)
        {
            var fileInfoSet = new HashSet<ApplicationFileInfo>();

            foreach (Match infoMatch in Regex.Matches(output, "^(.*?) (\\d+) (\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}(?:\\.\\d+)?)?$", RegexOptions.Multiline))
            {
                long filesize = long.Parse(infoMatch.Groups[2].Value);
                DateTime lastWriteTime = DateTime.Parse(infoMatch.Groups[3].Value);
                fileInfoSet.Add(new ApplicationFileInfo(infoMatch.Groups[1].Value, filesize, lastWriteTime, false));
            }

            return fileInfoSet;
        }

        public static string GetRemoteGameSyncPath()
        {
            var clovershell = MainForm.Clovershell;
            string gameSyncStorage = clovershell.ExecuteSimple("hakchi findGameSyncStorage", 2000, true).Trim();
            string gameSyncPath = gameSyncStorage;

            if (ConfigIni.SeparateGameStorage)
            {
                string systemCode = clovershell.ExecuteSimple("hakchi eval 'echo \"$sftype-$sfregion\"'", 2000, true).Trim();
                gameSyncPath = $"{gameSyncStorage}/{systemCode}";
            }

            return gameSyncPath;
        }
    }
}
