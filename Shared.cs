using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    static class Shared
    {
        public static Bitmap LoadBitmapCopy(string path)
        {
            Bitmap bmp;
            using (var img = Image.FromFile(path))
            {
                bmp = new Bitmap(img);
            }
            return bmp;
        }

        public static Bitmap ResizeImage(Image inImage, PixelFormat? pixelFormat, Color? backgroundColor, int targetWidth, int targetHeight, bool upscale, bool keepProportions, bool expandWidth, bool expandHeight)
        {
            int X, Y;
            if (!upscale && inImage.Width <= targetWidth && inImage.Height <= targetHeight)
            {
                X = inImage.Width;
                Y = inImage.Height;
            }
            else if (!keepProportions)
            {
                X = targetWidth;
                Y = targetHeight;
            }
            else if ((double)inImage.Width / (double)inImage.Height > (double)targetWidth / (double)targetHeight)
            {
                X = targetWidth;
                Y = (int)Math.Round((double)targetWidth * (double)inImage.Height / (double)inImage.Width);
                if (Y % 2 == 1) ++Y;
            }
            else
            {
                X = (int)Math.Round((double)targetHeight * (double)inImage.Width / (double)inImage.Height);
                if (X % 2 == 1) ++X;
                Y = targetHeight;
            }

            Bitmap outImage = pixelFormat == null ?
                new Bitmap(expandWidth ? targetWidth : X, expandHeight ? targetHeight : Y) :
                new Bitmap(expandWidth ? targetWidth : X, expandHeight ? targetHeight : Y, (PixelFormat)pixelFormat);
            var outRect = new Rectangle((int)((double)(outImage.Width - X) / 2), (int)((double)(outImage.Height - Y) / 2), X, Y);
            using (Graphics gr = Graphics.FromImage(outImage))
            {
                if(backgroundColor != null)
                {
                    gr.Clear((Color)backgroundColor);
                }
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY); // Fix first line and column alpha shit
                    gr.DrawImage(inImage, outRect, 0, 0, inImage.Width, inImage.Height, GraphicsUnit.Pixel, ia);
                }
                gr.Flush();
            }
            return outImage;
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

        public static uint CRC32(Stream stream)
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

            long bytesToRead = stream.Length;
            int bytesRead = 0;
            int bufferSize = 1048576;
            byte[] buffer = new byte[bufferSize];

            uint crc = 0xffffffff;
            while (bytesToRead > 0)
            {
                int chunk = bytesToRead < bufferSize ? (int)bytesToRead : bufferSize;
                int n = stream.Read(buffer, 0, chunk);
                for (int i = 0; i < n; ++i)
                {
                    byte index = (byte)(((crc) & 0xff) ^ buffer[i]);
                    crc = (uint)((crc >> 8) ^ table[index]);
                }
                bytesToRead -= n;
                bytesRead += n;
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
                if (version.Revision > 0)
                {
                    return $"{version.Major - 2}.{version.Minor + 1}.0RC{version.Revision}";
                }
                else
                {
                    return $"{version.Major - 2}.{version.Minor}.{version.Build}";
                }
            }
        }

        public static bool IsVersionGreaterOrEqual(string given, string minimum)
        {
            return new Version(given).CompareTo(new Version(minimum)) > -1;
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

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool skipExistingFiles, bool overwriteExistingFiles, bool pseudoLinks, string[] skipFiles = null)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (skipFiles != null && skipFiles.Contains(Path.GetFileName(file.Name)))
                    continue; // same behavior as TarStream

                string tempPath = new FileInfo(Path.Combine(destDirName, file.Name)).FullName;
                if (skipExistingFiles && File.Exists(tempPath))
                    continue;

                file.CopyTo(tempPath, overwriteExistingFiles);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, skipExistingFiles, overwriteExistingFiles, pseudoLinks, skipFiles);
                }
            }
        }

        public static void EnsureEmptyDirectory(string dirName)
        {
            if (Directory.Exists(dirName))
            {
                Shared.DirectoryDeleteInside(dirName);
            }
            Directory.CreateDirectory(dirName);
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

        private static string[][] RomanNumerals = new string[][]
        {
            new string[]{"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
            new string[]{"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
            new string[]{"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
            new string[]{"", "M", "MM", "MMM"} // thousands
        };

        public static string IntegerToRoman(int number)
        {

            // split integer string into array and reverse array
            var intArr = number.ToString().Reverse().ToArray();
            var len = intArr.Length;
            var romanNumeral = "";
            var i = len;

            // starting with the highest place (for 3046, it would be the thousands
            // place, or 3), get the roman numeral representation for that place
            // and add it to the final roman numeral string
            while (i-- > 0)
            {
                romanNumeral += RomanNumerals[i][Int32.Parse(intArr[i].ToString())];
            }

            return romanNumeral;
        }

        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
        {
            {'i', 1},
            {'v', 5},
            {'x', 10},
            {'l', 50},
            {'c', 100},
            {'d', 500},
            {'m', 1000}
        };

        public static int RomanToInteger(string roman)
        {
            int number = 0;
            for (int i = 0; i < roman.Length; i++)
            {
                if (i + 1 < roman.Length && RomanMap[roman[i]] < RomanMap[roman[i + 1]])
                {
                    number -= RomanMap[roman[i]];
                }
                else
                {
                    number += RomanMap[roman[i]];
                }
            }
            return number;
        }
    }
}
