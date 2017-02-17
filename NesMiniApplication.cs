using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace com.clusterrr.hakchi_gui
{
    public class NesMiniApplication : INesMenuElement
    {
        public readonly static string GamesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "games");
        const string DefaultReleaseDate = "1983-07-15"; // Famicom release day
        const string DefaultPublisher = "Nintendo";

        protected string code;
        public string Code
        {
            get { return code; }
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

        public static NesMiniApplication FromDirectory(string path, bool ignoreEmptyConfig = false)
        {
            var files = Directory.GetFiles(path, "*.desktop", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new FileNotFoundException("Invalid app folder");
            var config = File.ReadAllLines(files[0]);
            foreach (var line in config)
                if (line.StartsWith("Exec=/usr/bin/clover-kachikachi") || line.StartsWith("Exec=/bin/clover-kachikachi-wr"))
                {
                    if (line.Contains(".nes"))
                        return new NesGame(path, ignoreEmptyConfig);
                    if (line.Contains(".fds"))
                        return new FdsGame(path, ignoreEmptyConfig);
                }
            return new NesMiniApplication(path, ignoreEmptyConfig);
        }

        public static NesMiniApplication Import(string fileName, byte[] rawRomData = null)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            char prefixCode;
            string application;
            switch (extension)
            {
                // For some unusual NES ROM formats
                case ".fds":
                    return FdsGame.Import(fileName, rawRomData);
                case ".nes":
                case ".unf":
                case ".unif":
                    prefixCode = 'U';
                    application = "/bin/nes";
                    break;
                case ".desktop":
                    return ImportApp(fileName);
                case ".gb":
                case ".gbc":
                    prefixCode = 'C';
                    application = "/bin/gb";
                    break;
                case ".gba":
                    prefixCode = 'A';
                    application = "/bin/gba";
                    break;                
                case ".n64":
                case ".z64":
                case ".v64":
                    prefixCode = 'F';
                    application = "/bin/n64";
                    break;
                case ".smc":
                    prefixCode = 'E';
                    application = "/bin/snes";
                    break;
                case ".gen":
                case ".smd":
                    prefixCode = 'G';
                    application = "/bin/md";
                    break;
                case ".sms":
                    prefixCode = 'M';
                    application = "/bin/sms";
                    break;
                default:
                    prefixCode = 'Z';
                    application = "/bin/path-to-your-app";
                    break;
            }
            if (rawRomData == null)
                rawRomData = File.ReadAllBytes(fileName);
            var crc32 = CRC32(rawRomData);
            var code = GenerateCode(crc32, prefixCode);
            var gamePath = Path.Combine(GamesDirectory, code);
            var romName = Path.GetFileName(fileName).Replace(" ", "_");
            var romPath = Path.Combine(gamePath, romName);
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(romPath, rawRomData);
            var game = new NesMiniApplication(gamePath, true);
            game.Name = Path.GetFileNameWithoutExtension(fileName);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ").Trim();
            game.FindCover(fileName, Resources.blank, crc32);
            game.Command = string.Format("{0} /usr/share/games/nes/kachikachi/{1}/{2}", application, code, romName);
            game.Save();
            return game;
        }

        private static NesMiniApplication ImportApp(string fileName)
        {
            if (!File.Exists(fileName)) // Archives are not allowed
                throw new FileNotFoundException("Invalid app folder");
            var code = Path.GetFileNameWithoutExtension(fileName).ToUpper();
            var targetDir = Path.Combine(GamesDirectory, code);
            DirectoryCopy(Path.GetDirectoryName(fileName), targetDir, true);
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
        }

        protected NesMiniApplication(string path, bool ignoreEmptyConfig = false)
        {
            GamePath = path;
            code = Path.GetFileName(path);
            Name = Code;
            ConfigPath = Path.Combine(path, Code + ".desktop");
            IconPath = Path.Combine(path, Code + ".png");
            SmallIconPath = Path.Combine(path, Code + "_small.png");
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
                }
            }
            hasUnsavedChanges = false;
        }

        public virtual void Save()
        {
            if (!hasUnsavedChanges) return;
            Debug.WriteLine(string.Format("Saving application \"{0}\" as {1}", Name, Code));
            Name = Regex.Replace(Name, @"'(\d)", @"`$1"); // Apostrophe + any number in game name crashes whole system. What. The. Fuck?
            File.WriteAllText(ConfigPath, string.Format(
                "[Desktop Entry]\n" +
                "Type=Application\n" +
                "Exec={1}\n" +
                "Path=/var/lib/clover/profiles/0/{0}\n" +
                "Name={2}\n" +
                "Icon=/usr/share/games/nes/kachikachi/{0}/{0}.png\n\n" +
                "[X-CLOVER Game]\n" +
                "Code={0}\n" +
                "TestID=777\n" +
                "ID=0\n" +
                "Players={3}\n" +
                "Simultaneous={7}\n" +
                "ReleaseDate={4}\n" +
                "SaveCount=0\n" +
                "SortRawTitle={5}\n" +
                "SortRawPublisher={6}\n" +
                "Copyright=hakchi2 ©2017 Alexey 'Cluster' Avdyukhin\n",
                Code, command, Name ?? Code, Players, ReleaseDate ?? DefaultReleaseDate,
                (Name ?? Code).ToLower(), (Publisher ?? DefaultPublisher).ToUpper(),
                Simultaneous ? 1 : 0));
            hasUnsavedChanges = false;
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
            const int maxX = 204;
            const int maxY = 204;
            if ((double)image.Width / (double)image.Height > (double)maxX / (double)maxY)
                outImage = new Bitmap(maxX, (int)((double)maxY * (double)image.Height / (double)image.Width));
            else
                outImage = new Bitmap((int)(maxX * (double)image.Width / (double)image.Height), maxY);

            int maxXsmall = 40;
            int maxYsmall = 40;
            if ((double)image.Width / (double)image.Height > (double)maxXsmall / (double)maxYsmall)
                outImageSmall = new Bitmap(maxXsmall, (int)((double)maxYsmall * (double)image.Height / (double)image.Width));
            else
                outImageSmall = new Bitmap((int)(maxXsmall * (double)image.Width / (double)image.Height), maxYsmall);

            gr = Graphics.FromImage(outImage);
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            gr.Flush();
            outImage.Save(IconPath, ImageFormat.Png);
            gr = Graphics.FromImage(outImageSmall);
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.DrawImage(outImage, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height),
                new Rectangle(0, 0, outImage.Width, outImage.Height), GraphicsUnit.Pixel);
            gr.Flush();
            outImageSmall.Save(SmallIconPath, ImageFormat.Png);
        }

        internal bool FindCover(string romFileName, Image defaultCover, uint crc32 = 0)
        {
            // Trying to find cover file
            Image cover = null;
            if (!string.IsNullOrEmpty(romFileName))
            {
                var imagePath = Path.Combine(Path.GetDirectoryName(romFileName), Path.GetFileNameWithoutExtension(romFileName) + ".png");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                imagePath = Path.Combine(Path.GetDirectoryName(romFileName), Path.GetFileNameWithoutExtension(romFileName) + ".jpg");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                var artDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "art");
                Directory.CreateDirectory(artDirectory);
                imagePath = Path.Combine(artDirectory, Path.GetFileNameWithoutExtension(romFileName) + ".png");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                imagePath = Path.Combine(artDirectory, Path.GetFileNameWithoutExtension(romFileName) + ".jpg");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                if (crc32 != 0)
                {
                    var covers = Directory.GetFiles(artDirectory, string.Format("{0:X8}*.*", crc32), SearchOption.AllDirectories);
                    if (covers.Length > 0)
                        cover = LoadBitmap(covers[0]);
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
            var targetDir = Path.Combine(path, code);
            DirectoryCopy(GamePath, targetDir, true);
            return FromDirectory(targetDir);
        }

        internal static long DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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
                string temppath = Path.Combine(destDirName, file.Name);
                size += file.CopyTo(temppath, true).Length;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    size += DirectoryCopy(subdir.FullName, temppath, copySubDirs);
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
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + path);
            }

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
    }
}

