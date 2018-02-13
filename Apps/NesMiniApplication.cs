using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class NesMiniApplication : INesMenuElement
    {
        internal const string DefaultApp = "/bin/path-to-your-app";
        const string DefaultReleaseDate = "1900-01-01";
        const string DefaultPublisher = "UNKNOWN";
        public const char DefaultPrefix = 'Z';
        public static Image DefaultCover = Resources.blank_app;
        public static Form ParentForm;
        public static bool? NeedPatch;
        public static bool? Need3rdPartyEmulator;
        public static bool? NeedAutoDownloadCover;
        const int MaxCompressSize = 10 * 1024;

        public static string GamesDirectory
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return System.IO.Path.Combine(Program.BaseDirectoryExternal, "games");
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return System.IO.Path.Combine(Program.BaseDirectoryExternal, "games_snes");
                }
            }
        }
        public static string GamesCloverPath
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return "/usr/share/games/nes/kachikachi";
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return "/usr/share/games";
                }
            }
        }

        protected string code;
        public string Code
        {
            get { return code; }
        }
        public virtual string GoogleSuffix
        {
            get { return "game"; }
        }


        public const string GameGenieFileName = "gamegenie.txt";
        public string GameGeniePath { private set; get; }
        private string gameGenie = "";
        public string GameGenie
        {
            get { return gameGenie; }
            set
            {
                if (gameGenie != value) hasUnsavedChanges = true;
                gameGenie = value;
            }
        }

        public readonly string GamePath;
        public readonly string ConfigPath;
        public readonly string IconPath;
        public readonly string SmallIconPath;
        protected string command;
        protected bool hasUnsavedChanges = true;

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value) hasUnsavedChanges = true;
                name = value;
            }
        }
        public string Command
        {
            get { return command; }
            set
            {
                if (command != value) hasUnsavedChanges = true;
                command = value;
            }
        }
        private byte players;
        public byte Players
        {
            get { return players; }
            set
            {
                if (players != value) hasUnsavedChanges = true;
                players = value;
            }
        }
        private bool simultaneous;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set
            {
                if (simultaneous != value) hasUnsavedChanges = true;
                simultaneous = value;
            }
        }
        private string releaseDate;
        public string ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                if (releaseDate != value) hasUnsavedChanges = true;
                releaseDate = value;
            }
        }
        private string publisher;
        public string Publisher
        {
            get { return publisher; }
            set
            {
                if (publisher != value) hasUnsavedChanges = true;
                publisher = value;
            }
        }
        private byte saveCount;
        public byte SaveCount
        {
            get { return saveCount; }
            set
            {
                if (saveCount != value) hasUnsavedChanges = true;
                saveCount = value;
            }
        }

        public static NesMiniApplication FromDirectory(string path, bool ignoreEmptyConfig = false)
        {
            var files = Directory.GetFiles(path, "*.desktop", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new FileNotFoundException("Invalid app folder");
            var config = File.ReadAllLines(files[0]);
            foreach (var line in config)
            {
                if (line.StartsWith("Exec="))
                {
                    string command = line.Substring(5);
                    var app = AppTypeCollection.GetAppByExec(command);
                    if (app != null)
                    {
                        var constructor = app.Class.GetConstructor(new Type[] { typeof(string), typeof(bool) });
                        return (NesMiniApplication)constructor.Invoke(new object[] { path, ignoreEmptyConfig });
                    }
                    break;
                }
            }
            return new NesMiniApplication(path, ignoreEmptyConfig);
        }

        public static NesMiniApplication Import(string inputFileName, string originalFileName = null, byte[] rawRomData = null)
        {
            var extension = System.IO.Path.GetExtension(inputFileName).ToLower();
            if (extension == ".desktop")
                return ImportApp(inputFileName);
            if (rawRomData == null) // Maybe it's already extracted data?
                rawRomData = File.ReadAllBytes(inputFileName); // If not, reading file
            if (originalFileName == null) // Original file name from archive
                originalFileName = System.IO.Path.GetFileName(inputFileName);
            char prefix = DefaultPrefix;
            string application = extension.Length > 2 ? ("/bin/" + extension.Substring(1)) : DefaultApp;
            string args = null;
            Image cover = DefaultCover;
            byte saveCount = 0;
            uint crc32 = CRC32(rawRomData);
            string outputFileName = Regex.Replace(System.IO.Path.GetFileName(inputFileName), @" ?\(.*?\)| ?\[.*?\]", "").Trim();
            outputFileName = Regex.Replace(outputFileName, @"[^A-Za-z0-9!\.]+", "_");

            // Trying to determine file type
            var appinfo = AppTypeCollection.GetAppByExtension(extension);
            bool patched = false;
            if (appinfo != null)
            {
                if (appinfo.DefaultApps.Length > 0)
                    application = appinfo.DefaultApps[0];
                prefix = appinfo.Prefix;
                cover = appinfo.DefaultCover;
                var patch = appinfo.Class.GetMethod("Patch");
                if (patch != null)
                {
                    object[] values = new object[] { inputFileName, rawRomData, prefix, application, outputFileName, args, cover, saveCount, crc32 };
                    var result = (bool)patch.Invoke(null, values);
                    if (!result) return null;
                    rawRomData = (byte[])values[1];
                    prefix = (char)values[2];
                    application = (string)values[3];
                    outputFileName = (string)values[4];
                    args = (string)values[5];
                    cover = (Image)values[6];
                    saveCount = (byte)values[7];
                    crc32 = (uint)values[8];
                    patched = true;
                }
            }

            if (!patched)
                FindPatch(ref rawRomData, inputFileName, crc32);

            var code = GenerateCode(crc32, prefix);
            var gamePath = Path.Combine(GamesDirectory, code);
            var romPath = Path.Combine(gamePath, outputFileName);
            if (Directory.Exists(gamePath))
            {
                var files = Directory.GetFiles(gamePath, "*.*", SearchOption.AllDirectories);
                foreach (var f in files)
                try
                {
                        File.Delete(f);
                }
                catch { }
            }
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(romPath, rawRomData);
            var game = new NesMiniApplication(gamePath, true);
            game.Name = System.IO.Path.GetFileNameWithoutExtension(inputFileName);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ").Trim();
            game.Command = $"{application} {GamesCloverPath}/{code}/{outputFileName}";
            if (!string.IsNullOrEmpty(args))
                game.Command += " " + args;
            game.FindCover(inputFileName, cover, crc32);
            game.SaveCount = saveCount;
            game.Save();

            var app = NesMiniApplication.FromDirectory(gamePath);
            if (app is ICloverAutofill)
                (app as ICloverAutofill).TryAutofill(crc32);

            if (ConfigIni.Compress)
            {
                app.Compress();
                app.Save();
            }

            return app;
        }

        private static NesMiniApplication ImportApp(string fileName)
        {
            if (!File.Exists(fileName)) // Archives are not allowed
                throw new FileNotFoundException("Invalid app folder");
            var code = System.IO.Path.GetFileNameWithoutExtension(fileName).ToUpper();
            var targetDir = System.IO.Path.Combine(GamesDirectory, code);
            DirectoryCopy(System.IO.Path.GetDirectoryName(fileName), targetDir, true);
            return FromDirectory(targetDir);
        }

        protected NesMiniApplication()
        {
            GamePath = null;
            ConfigPath = null;
            Players = 1;
            Simultaneous = false;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;
            Command = "";
            SaveCount = 0;
        }

        protected NesMiniApplication(string path, bool ignoreEmptyConfig = false)
        {
            GamePath = path;
            code = System.IO.Path.GetFileName(path);
            Name = Code;
            ConfigPath = System.IO.Path.Combine(path, Code + ".desktop");
            IconPath = System.IO.Path.Combine(path, Code + ".png");
            SmallIconPath = System.IO.Path.Combine(path, Code + "_small.png");
            Players = 1;
            Simultaneous = false;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;
            Command = "";

            if (!File.Exists(ConfigPath))
            {
                if (ignoreEmptyConfig) return;
                throw new FileNotFoundException("Invalid application directory: " + path);
            }
            var configLines = File.ReadAllLines(ConfigPath);
            foreach (var line in configLines)
            {
                int pos = line.IndexOf('=');
                if (pos <= 0) continue;
                var param = line.Substring(0, pos).Trim().ToLower();
                var value = line.Substring(pos + 1).Trim();
                switch (param)
                {
                    case "exec":
                        Command = value;
                        break;
                    case "name":
                        Name = value;
                        break;
                    case "players":
                        Players = byte.Parse(value);
                        break;
                    case "simultaneous":
                        Simultaneous = value != "0";
                        break;
                    case "releasedate":
                        ReleaseDate = value;
                        break;
                    case "sortrawpublisher":
                        Publisher = value;
                        break;
                    case "savecount":
                        SaveCount = byte.Parse(value);
                        break;
                }
            }

            GameGeniePath = Path.Combine(path, GameGenieFileName);
            if (File.Exists(GameGeniePath))
                gameGenie = File.ReadAllText(GameGeniePath);

            hasUnsavedChanges = false;
        }

        public virtual bool Save()
        {
            if (!hasUnsavedChanges) return false;
            Debug.WriteLine(string.Format("Saving application \"{0}\" as {1}", Name, Code));
            Name = Regex.Replace(Name, @"'(\d)", @"`$1"); // Apostrophe + any number in game name crashes whole system. What. The. Fuck?
            var sortRawTitle = Name.ToLower();
            if (sortRawTitle.StartsWith("the "))
                sortRawTitle = sortRawTitle.Substring(4); // Sorting without "THE"
            File.WriteAllText(ConfigPath, 
                $"[Desktop Entry]\n" +
                $"Type=Application\n" +
                $"Exec={command}\n" +
                $"Path=/var/lib/clover/profiles/0/{Code}\n" +
                $"Name={Name ?? Code}\n" +
                $"Icon={GamesCloverPath}/{Code}/{Code}.png\n\n" +
                $"[X-CLOVER Game]\n" +
                $"Code={Code}\n" +
                $"TestID=777\n" +
                $"ID=0\n" +
                $"Players={Players}\n" +
                $"Simultaneous={(Simultaneous ? 1 : 0)}\n" +
                $"ReleaseDate={ReleaseDate ?? DefaultReleaseDate}\n" +
                $"SaveCount={SaveCount}\n" +
                $"SortRawTitle={sortRawTitle}\n" +
                $"SortRawPublisher={(Publisher ?? DefaultPublisher).ToUpper()}\n" +
                $"Copyright=hakchi2 ©2017 Alexey 'Cluster' Avdyukhin\n");

            if (!string.IsNullOrEmpty(gameGenie))
                File.WriteAllText(GameGeniePath, gameGenie);
            else if (File.Exists(GameGeniePath))
                File.Delete(GameGeniePath);

            hasUnsavedChanges = false;
            return true;
        }

        public override string ToString()
        {
            return Name;
        }

        public Image Image
        {
            set
            {
                SetImage(value);
            }
            get
            {
                if (File.Exists(IconPath))
                    return LoadBitmap(IconPath);
                else
                    return null;
            }
        }

        private void SetImage(Image image, bool EightBitCompression = false)
        {
            Bitmap outImage;
            Bitmap outImageSmall;
            Graphics gr;

            // Just keep aspect ratio
            int maxX = 204;
            int maxY = 204;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                maxX = 228;
                maxY = 204;
            }
            if ((double)image.Width / (double)image.Height > (double)maxX / (double)maxY)
            {
                int Y = (int)((double)maxX * (double)image.Height / (double)image.Width);
                if (Y % 2 == 1)
                    ++Y;
                outImage = new Bitmap(maxX, Y);
            }
            else
                outImage = new Bitmap((int)(maxY * (double)image.Width / (double)image.Height), maxY);

            int maxXsmall = 40;
            int maxYsmall = 40;
            if ((double)image.Width / (double)image.Height > (double)maxXsmall / (double)maxYsmall)
            {
                int Y = (int)((double)maxXsmall * (double)image.Height / (double)image.Width);
                if (Y % 2 == 1)
                    ++Y;
                outImageSmall = new Bitmap(maxXsmall, Y);
            }
            else
                outImageSmall = new Bitmap((int)(maxYsmall * (double)image.Width / (double)image.Height), maxYsmall);

            gr = Graphics.FromImage(outImage);
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            gr.Flush();
            outImage.Save(IconPath, ImageFormat.Png);
            gr = Graphics.FromImage(outImageSmall);

            // Better resizing quality (more blur like original files)
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            // Fix first line and column alpha shit
            using (ImageAttributes wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                gr.DrawImage(outImage, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height), 0, 0, outImage.Width, outImage.Height, GraphicsUnit.Pixel, wrapMode);
            }
            gr.Flush();
            outImageSmall.Save(SmallIconPath, ImageFormat.Png);
        }

        protected bool FindCover(string inputFileName, Image defaultCover, uint crc32 = 0)
        {
            // Trying to find cover file
            Image cover = null;
            var artDirectory = System.IO.Path.Combine(Program.BaseDirectoryExternal, "art");
            Directory.CreateDirectory(artDirectory);
            if (!string.IsNullOrEmpty(inputFileName))
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(inputFileName);
                if (crc32 != 0)
                {
                    var covers = Directory.GetFiles(artDirectory, string.Format("{0:X8}*.*", crc32), SearchOption.AllDirectories);
                    if (covers.Length > 0)
                        cover = LoadBitmap(covers[0]);
                }
                if (cover == null)
                {
                    // priority to inputFileName directory
                    var imagePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(inputFileName), name + ".jpg");
                    if (File.Exists(imagePath))
                        cover = LoadBitmap(imagePath);
                    imagePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(inputFileName), name + ".png");
                    if (File.Exists(imagePath))
                        cover = LoadBitmap(imagePath);
                }
                if( cover == null )
                {
                    // do a bidirectional search on sanitized filenames to allow minor variance in filenames, also allows subdirectories
                    Regex rgx = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
                    var sanitizedName = rgx.Replace(name, string.Empty).ToLower();

                    var covers = Directory.GetFiles(artDirectory, "*.*", SearchOption.AllDirectories);
                    foreach(var file in covers)
                    {
                        var sanitized = rgx.Replace(System.IO.Path.GetFileNameWithoutExtension(file), "").ToLower();
                        if (sanitizedName.StartsWith(sanitized) || sanitized.StartsWith(sanitizedName))
                        {
                            cover = LoadBitmap(file);
                            break;
                        }
                    }
                    /*
                    var imagePath = System.IO.Path.Combine(artDirectory, System.IO.Path.GetFileNameWithoutExtension(inputFileName) + ".png");
                    if (File.Exists(imagePath))
                        cover = LoadBitmap(imagePath);
                    imagePath = System.IO.Path.Combine(artDirectory, System.IO.Path.GetFileNameWithoutExtension(inputFileName) + ".jpg");
                    if (File.Exists(imagePath))
                        cover = LoadBitmap(imagePath);
                    */
                }
            }
            if (cover == null)
            {
                Image = defaultCover;
                return false;
            }
            Image = cover;
            return true;
        }

        protected static bool FindPatch(ref byte[] rawRomData, string inputFileName, uint crc32 = 0)
        {
            string patch = null;
            var patchesDirectory = System.IO.Path.Combine(Program.BaseDirectoryExternal, "patches");
            Directory.CreateDirectory(patchesDirectory);
            if (!string.IsNullOrEmpty(inputFileName))
            {
                if (crc32 != 0)
                {
                    var patches = Directory.GetFiles(patchesDirectory, string.Format("{0:X8}*.*", crc32), SearchOption.AllDirectories);
                    if (patches.Length > 0)
                        patch = patches[0];
                }
                var patchesPath = Path.Combine(patchesDirectory, Path.GetFileNameWithoutExtension(inputFileName) + ".ips");
                if (File.Exists(patchesPath))
                    patch = patchesPath;
                patchesPath = Path.Combine(Path.GetDirectoryName(inputFileName), System.IO.Path.GetFileNameWithoutExtension(inputFileName) + ".ips");
                if (File.Exists(patchesPath))
                    patch = patchesPath;
            }

            if (!string.IsNullOrEmpty(patch))
            {
                if (NeedPatch != true)
                {
                    if (NeedPatch != false)
                    {
                        var r = WorkerForm.MessageBoxFromThread(ParentForm,
                            string.Format(Resources.PatchQ, System.IO.Path.GetFileName(inputFileName)),
                            Resources.PatchAvailable,
                            MessageBoxButtons.AbortRetryIgnore,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2, true);
                        if (r == DialogResult.Abort)
                            NeedPatch = true;
                        if (r == DialogResult.Ignore)
                            return false;
                    }
                    else return false;
                }
                IpsPatcher.Patch(patch, ref rawRomData);
                return true;
            }
            return false;
        }

        protected static string GenerateCode(uint crc32, char prefixCode)
        {
            return string.Format("CLV-{5}-{0}{1}{2}{3}{4}",
                (char)('A' + (crc32 % 26)),
                (char)('A' + (crc32 >> 5) % 26),
                (char)('A' + ((crc32 >> 10) % 26)),
                (char)('A' + ((crc32 >> 15) % 26)),
                (char)('A' + ((crc32 >> 20) % 26)),
                prefixCode);
        }

        public NesMiniApplication CopyTo(string path)
        {
            var targetDir = System.IO.Path.Combine(path, code);
            DirectoryCopy(GamePath, targetDir, true);
            return FromDirectory(targetDir);
        }

        internal static long DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string[] skipFiles = null)
        {
            long size = 0;
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
                if (skipFiles != null && skipFiles.Contains(file.Name))
                    continue;
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                size += file.CopyTo(temppath, true).Length;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    size += DirectoryCopy(subdir.FullName, temppath, copySubDirs, skipFiles);
                }
            }
            return size;
        }

        public long Size(string path = null)
        {
            if (path == null)
                path = GamePath;
            long size = 0;
            // Get the subdirectories for the specified directory.
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
                size += Size(subdir.FullName);
            }
            return size;
        }

        protected static uint CRC32(byte[] data)
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

        public static Bitmap LoadBitmap(string path)
        {
            //Open file in read only mode
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //Get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                //make a new Bitmap object the owner of the MemoryStream
                return new Bitmap(memoryStream);
            }
        }

        public string[] CompressPossible()
        {
            if (!Directory.Exists(GamePath)) return new string[0];
            var result = new List<string>();
            var exec = Regex.Replace(Command, "[/\\\"]", " ") + " ";
            var files = Directory.GetFiles(GamePath, "*.*", SearchOption.TopDirectoryOnly);
            var ignoreExtensions = new string[] { ".7z", ".zip", ".hsqs", ".sh" };
            foreach (var file in files)
            {
                var a = from i in ignoreExtensions select i;
                if (ignoreExtensions.Contains(Path.GetExtension(file).ToLower()))
                    continue;
                if (new FileInfo(file).Length > MaxCompressSize)
                    continue;
                if (exec.Contains(" " + Path.GetFileName(file) + " "))
                    result.Add(file);
            }
            return result.ToArray();
        }

        public string[] DecompressPossible()
        {
            if (!Directory.Exists(GamePath)) return new string[0];
            var result = new List<string>();
            var exec = Regex.Replace(Command, "[/\\\"]", " ") + " ";
            var files = Directory.GetFiles(GamePath, "*.7z", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (exec.Contains(" " + System.IO.Path.GetFileName(file) + " "))
                    result.Add(file);
            }
            return result.ToArray();
        }

        public void Compress()
        {
            SevenZipExtractor.SetLibraryPath(System.IO.Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            foreach (var filename in CompressPossible())
            {
                var archName = filename + ".7z";
                var compressor = new SevenZipCompressor();
                compressor.CompressionLevel = CompressionLevel.High;
                Debug.WriteLine("Compressing " + filename);
                compressor.CompressFiles(archName, filename);
                File.Delete(filename);
                Command = Command.Replace(Path.GetFileName(filename), Path.GetFileName(archName));
            }
        }

        public void Decompress()
        {
            SevenZipExtractor.SetLibraryPath(System.IO.Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            foreach (var filename in DecompressPossible())
            {
                using (var szExtractor = new SevenZipExtractor(filename))
                {
                    Debug.WriteLine("Decompressing " + filename);
                    szExtractor.ExtractArchive(GamePath);
                    foreach (var f in szExtractor.ArchiveFileNames)
                        Command = Command.Replace(System.IO.Path.GetFileName(filename), f);
                }
                File.Delete(filename);
            }
        }

        public class NesMiniAppEqualityComparer : IEqualityComparer<NesMiniApplication>
        {
            public bool Equals(NesMiniApplication x, NesMiniApplication y)
            {
                return x.Code == y.Code;
            }

            public int GetHashCode(NesMiniApplication obj)
            {
                return obj.Code.GetHashCode();
            }
        }
    }
}

