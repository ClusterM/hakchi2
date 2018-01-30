using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    static class Shared
    {
        public static string AppDisplayVersion
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major - 2}.{version.Minor}.{version.Build}";
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

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
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
    }
}
