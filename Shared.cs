using com.clusterrr.clovershell;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.ssh;
using com.clusterrr.util;
using Renci.SshNet;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace com.clusterrr.hakchi_gui
{
    static class Shared
    {
        public static readonly FelLib.Fel.VidPidPair ClassicUSB = new FelLib.Fel.VidPidPair(vid: 0x1F3A, pid: 0xEFE8);
        public static string[] hmodDirectories {
            get
            {
                return new string[]
                {
                    Shared.PathCombine(Program.BaseDirectoryExternal, "user_mods"),
                    Shared.PathCombine(Program.BaseDirectoryInternal, "mods", "hmods")
                };
            }
        }

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

        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
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
                Thread.Sleep(0);
                Directory.Delete(dir);
            }
        }

        public static void DirectoryDeleteEmptyDirectories(string dirName)
        {
            var directories = Directory.GetDirectories(dirName, "*.*", SearchOption.AllDirectories);
            for (int i = directories.Length - 1; i > -1; --i) // backwards to catch deeper directories first
            {
                var dirInfo = new DirectoryInfo(directories[i]);
                if (!dirInfo.EnumerateDirectories().Any() && !dirInfo.EnumerateFiles().Any())
                {
                    Directory.Delete(directories[i]);
                }
            }
        }

        public static long DirectorySize(string path, long blockSize = -1)
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
                size += PadFileSize(file.Length, blockSize);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                size += DirectorySize(subdir.FullName);
            }
            return size;
        }

        public static long PadFileSize(long size, long blockSize = -1)
        {
            return blockSize == -1 ? size : (size % blockSize == 0 ? size : (((long)Math.Floor((double)size / blockSize) + 1) * blockSize));
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

        public static UInt32 CalcKernelSize(byte[] header)
        {
            if (Encoding.ASCII.GetString(header, 0, 8) != "ANDROID!") throw new Exception(Resources.InvalidKernelHeader);
            UInt32 kernel_size = (UInt32)(header[8] | (header[9] * 0x100) | (header[10] * 0x10000) | (header[11] * 0x1000000));
            UInt32 kernel_addr = (UInt32)(header[12] | (header[13] * 0x100) | (header[14] * 0x10000) | (header[15] * 0x1000000));
            UInt32 ramdisk_size = (UInt32)(header[16] | (header[17] * 0x100) | (header[18] * 0x10000) | (header[19] * 0x1000000));
            UInt32 ramdisk_addr = (UInt32)(header[20] | (header[21] * 0x100) | (header[22] * 0x10000) | (header[23] * 0x1000000));
            UInt32 second_size = (UInt32)(header[24] | (header[25] * 0x100) | (header[26] * 0x10000) | (header[27] * 0x1000000));
            UInt32 second_addr = (UInt32)(header[28] | (header[29] * 0x100) | (header[30] * 0x10000) | (header[31] * 0x1000000));
            UInt32 tags_addr = (UInt32)(header[32] | (header[33] * 0x100) | (header[34] * 0x10000) | (header[35] * 0x1000000));
            UInt32 page_size = (UInt32)(header[36] | (header[37] * 0x100) | (header[38] * 0x10000) | (header[39] * 0x1000000));
            UInt32 dt_size = (UInt32)(header[40] | (header[41] * 0x100) | (header[42] * 0x10000) | (header[43] * 0x1000000));
            UInt32 pages = 1;
            pages += (kernel_size + page_size - 1) / page_size;
            pages += (ramdisk_size + page_size - 1) / page_size;
            pages += (second_size + page_size - 1) / page_size;
            pages += (dt_size + page_size - 1) / page_size;
            return pages * page_size;
        }

        public static MemoryStream GetMembootImage()
        {
            var kernelHmodStream = new MemoryStream();

            using (var baseHmods = new MemoryStream(Resources.baseHmods)) {
                using (var szExtractor = new SevenZipExtractor(baseHmods))
                {
                    szExtractor.ExtractFile(".\\hakchi.hmod", kernelHmodStream);
                }

            }
            using (var tar = new MemoryStream())
            {
                using (var szExtractor = new SevenZipExtractor(kernelHmodStream))
                {
                    szExtractor.ExtractFile(0, tar);
                    tar.Seek(0, SeekOrigin.Begin);
                    using (var szExtractorTar = new SevenZipExtractor(tar))
                    {
                        var o = new MemoryStream();
                        szExtractorTar.ExtractFile("boot\\boot.img", o);
                        return o;
                    }
                }
            }
        }

        public static void InPlaceStringEdit(this byte[] buffer, int startOffset, int windowSize, byte fillByte, Func<string, string> Functor)
        {
            string converted = Encoding.ASCII.GetString(buffer, startOffset, windowSize).TrimEnd(new char[] { '\0' });
            converted = Functor(converted);
            byte[] newBuffer = Enumerable.Repeat((byte)fillByte, windowSize).ToArray();
            Array.Copy(
                Encoding.ASCII.GetBytes(converted), 0,
                newBuffer, 0,
                converted.Length < windowSize ? converted.Length : windowSize);
            newBuffer.CopyTo(buffer, startOffset);
        }

        public static Dictionary<hakchi.ConsoleType, string[]> CorrectKeys()
        {
            Dictionary<hakchi.ConsoleType, string[]> correctKeys = new Dictionary<hakchi.ConsoleType, string[]>();
            correctKeys[hakchi.ConsoleType.NES] =
                correctKeys[hakchi.ConsoleType.Famicom] =
                new string[] { "bb8f49e0ae5acc8d5f9b7fa40efbd3e7" };
            correctKeys[hakchi.ConsoleType.SNES_EUR] =
                correctKeys[hakchi.ConsoleType.SNES_USA] =
                correctKeys[hakchi.ConsoleType.SuperFamicom] =
                new string[] { "c5dbb6e29ea57046579cfd50b124c9e1" };
            return correctKeys;
        }

        public static Dictionary<hakchi.ConsoleType, string[]> CorrectKernels()
        {
            Dictionary<hakchi.ConsoleType, string[]> correctKernels = new Dictionary<hakchi.ConsoleType, string[]>();
            correctKernels[hakchi.ConsoleType.NES] = new string[] {
                "5cfdca351484e7025648abc3b20032ff",
                "07bfb800beba6ef619c29990d14b5158",
            };
            correctKernels[hakchi.ConsoleType.Famicom] = new string[] {
                "ac8144c3ea4ab32e017648ee80bdc230",  // Famicom Mini
            };
            correctKernels[hakchi.ConsoleType.SNES_EUR] = new string[] {
                "d76c2a091ebe7b4614589fc6954653a5", // SNES Mini (EUR)
                "c2b57b550f35d64d1c6ce66f9b5180ce", // SNES Mini (EUR)
                "0f890bc78cbd9ede43b83b015ba4c022", // SNES Mini (EUR)
            };
            correctKernels[hakchi.ConsoleType.SNES_USA] = new string[] {
                "449b711238575763c6701f5958323d48", // SNES Mini (USA)
                "5296e64818bf2d1dbdc6b594f3eefd17", // SNES Mini (USA)
                "228967ab1035a347caa9c880419df487", // SNES Mini (USA)
            };
            correctKernels[hakchi.ConsoleType.SuperFamicom] = new string[]
            {
                "632e179db63d9bcd42281f776a030c14", // Super Famicom Mini (JAP)
                "c3378edfc1b96a5268a066d5fbe12d89", // Super Famicom Mini (JAP)
            };
            return correctKernels;
        }

        public static string EscapeShellArgument(string unquotedArgument)
        {
            return Regex.Replace(unquotedArgument, "[^a-zA-Z0-9]", "\\$0");
        }

        public static void SocketTransfer(string server, int port, Stream dataStreamTo, Stream dataStreamFrom)
        {
            if (server is null)
                throw new ArgumentNullException("server");

            if (port < 1)
                throw new ArgumentOutOfRangeException("port");

            if (dataStreamTo is null && dataStreamFrom is null)
                throw new ArgumentNullException("dataStreamIn, dataStreamOut");

            // Create a TcpClient.
            TcpClient client = new TcpClient(server, port);

            // Get a client stream for reading and writing.
            NetworkStream socketStream = client.GetStream();

            // Copy the stream data into dataStream
            Task transferFrom = null;
            if (!(dataStreamFrom is null))
            {
                transferFrom = socketStream.CopyToAsync(dataStreamFrom);
            }

            // Copy dataStream into the stream data
            Task transferTo = null;
            if (!(dataStreamTo is null))
            {
                transferTo = dataStreamTo.CopyToAsync(socketStream);
            }

            // Wait for the streams to finish
            try {
                transferFrom.Wait();
            }
            catch { }

            try {
                transferTo.Wait();
            }
            catch { }

            // Close everything.
            socketStream.Close();
            client.Close();
        }

        
        public static int ShellPipe(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false)
        {
            if (!(hakchi.Shell is INetworkShell))
                return hakchi.Shell.Execute(command, stdin, stdout, stderr, timeout, throwOnNonZero);

            if (!(stderr is null))
                throw new ArgumentException($"stderr is not valid for this connection type: {hakchi.Shell.GetType().ToString()}");

            var stdErr = new MemoryStream();
            SplitterStream splitStream = new SplitterStream(stdErr).AddStreams(Program.debugStreams);
            var transferThread = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(1000);
                    stdErr.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(stdErr))
                    {
                        var line = sr.ReadLine();
                        var match = Regex.Match(line, "^listening on (\\d+\\.\\d+\\.\\d+\\.\\d+):(\\d+)");
                        stdErr.Close();
                        splitStream.RemoveStream(stdErr).AddStreams(stderr);
                        if (match.Success)
                        {
                            SocketTransfer((hakchi.Shell as INetworkShell).IPAddress, int.Parse(match.Groups[2].Value), stdin, stdout);
                        }
                    }
                }
                catch (ThreadAbortException) { }
            });
            transferThread.Start();
            int returnValue = hakchi.Shell.Execute($"nc -lv -w 60 -i 60 -s 0.0.0.0 -e {command}", null, null, splitStream, timeout, throwOnNonZero);
            transferThread.Abort();
            return returnValue;
        }
    }
}
