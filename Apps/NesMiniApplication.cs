using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class NesMiniApplication : INesMenuElement
    {
        public readonly static string GamesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "games");
        const string DefaultReleaseDate = "1900-01-01";
        const string DefaultPublisher = "UNKNOWN";

        protected string code;
        public string Code
        {
            get { return code; }
        }
        public const char DefaultPrefix = 'Z';
        public static Image DefaultCover { get { return Resources.blank_app; } }
        internal const string DefaultApp = "/bin/path-to-your-app";
        public virtual string GoogleSuffix
        {
            get { return "game"; }
        }
        public bool isCompressed
        {
            get
            {
                return (System.IO.Directory.GetFiles(this.GamePath, "*.7z").Length > 0);
                
                
                
            }
        }
        public string Console
        {
               get
            {
                return this.GetType().ToString().Replace("com.clusterrr.hakchi_gui.", "").Replace("Game","");
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

        public static NesMiniApplication FromDirectory(string path, bool ignoreEmptyConfig = false)
        {
            string tag = path.Substring(path.LastIndexOf("\\")+1);
            tag = tag.Substring(tag.IndexOf("-") + 1);
            tag = tag.Substring(0,tag.IndexOf( "-")) ;
            var app = AppTypeCollection.GetAppByPrefix(tag[0]);
            if (app != null)
            {
                var constructor = app.Class.GetConstructor(new Type[] { typeof(string), typeof(bool) });
                return (NesMiniApplication)constructor.Invoke(new object[] { path, ignoreEmptyConfig });
            }

            else
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
                        if (command.StartsWith("/usr/bin/clover-kachikachi") || command.StartsWith("/bin/clover-kachikachi-wr"))
                        {
                            if (command.Contains(".nes"))
                                return new NesGame(path, ignoreEmptyConfig);
                            if (command.Contains(".fds"))
                                return new FdsGame(path, ignoreEmptyConfig);
                        }

                        break;
                    }
                }
            }
            return new NesMiniApplication(path, ignoreEmptyConfig);
        }

        public static NesMiniApplication Import(string fileName, string sourceFile = null, byte[] rawRomData = null)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (extension == ".desktop")
                return ImportApp(fileName);
            if (rawRomData == null)
                rawRomData = File.ReadAllBytes(fileName);
            var appinfo = AppTypeCollection.GetAppByExtension(extension);
            if (appinfo != null)
            {
                var import = appinfo.Class.GetMethod("Import", new Type[] { typeof(string), typeof(string), typeof(byte[]) });
                if (import != null)
                    return (NesMiniApplication)import.Invoke(null, new object[] { fileName, sourceFile, rawRomData });
                else
                    return Import(fileName, sourceFile, rawRomData, appinfo.Prefix, appinfo.DefaultApp, appinfo.DefaultCover, ConfigIni.Compress);
            }
            string application = extension.Length > 2 ? ("/bin/" + extension.Substring(1)) : DefaultApp;
            return Import(fileName, sourceFile, rawRomData, DefaultPrefix, application, DefaultCover);
        }
        public string GetNesMiniLocalPath()
        {
            return "/usr/share/games/nes/kachikachi/" + Code + "/" + GetRomFile();
        }
        public string GetRomFile()
        {
            System.Collections.Generic.List<string> exts = new System.Collections.Generic.List<string>(AppTypeCollection.GetAppByType(this.GetType()).Extensions);
            List<string> expectedFiles = new List<string>();
            expectedFiles.Add(Code + ".desktop");
            expectedFiles.Add(Code + ".png");
            expectedFiles.Add(Code + "_small.png");

            List<string> possibleRomFiles = new List<string>();
            List<string> zFiles = new List<string>();

          
            foreach (string f in System.IO.Directory.GetFiles(this.GamePath))
            {
                string fileName = System.IO.Path.GetFileName(f);
                if (!expectedFiles.Contains(fileName))
                {
                    string ext = System.IO.Path.GetExtension(fileName);
                    if (ext == ".7z")
                    {
                        zFiles.Add(fileName);
                    }
                    else
                    {
                        if (!exts.Contains(ext))
                        {
                           
                        }
                        else
                        {
                            possibleRomFiles.Add(fileName);
                        }
                    }
                }
            }
            string tempCommand = Command;
            tempCommand = tempCommand.Replace(".nes .7z", ".nes.7z");
          

                /*Find if of the file is linked in the commande*/
                string foundRom = "";
                foreach (string z in zFiles)
                {
                    if (tempCommand.Contains(z))
                    {
                        foundRom = z;
                        break;
                    }
                }
                if (foundRom == "")
                {
                    /*Rom was not found in 7z*/
                    foreach (string z in possibleRomFiles)
                    {
                        if (tempCommand.Contains(z))
                        {
                            foundRom = z;
                            break;
                        }
                    }
                }
                if(foundRom == "")
            {
                //Cant find rom from command line, now analyse files
                if(zFiles.Count >= 1)
                {
                    //Most likely this one
                    foundRom = zFiles[0];
                }
                else
                {
                    if(possibleRomFiles.Count>=1)
                    {
                        foundRom = possibleRomFiles[0];
                    }

                }

            }
                if(foundRom == "")
            {
                string test = "abc";
            }
            return foundRom;

            
            
        }
        public void Clean()
        {
            System.Collections.Generic.List<string> exts = new System.Collections.Generic.List<string>(AppTypeCollection.GetAppByType(this.GetType()).Extensions);
            List<string> expectedFiles = new List<string>();
            expectedFiles.Add(Code + ".desktop");
            expectedFiles.Add(Code + ".png");
            expectedFiles.Add(Code + "_small.png");

            List<string> possibleRomFiles = new List<string>();
            List<string> zFiles = new List<string>();

            List<string> toDelete = new List<string>();
            foreach (string f in System.IO.Directory.GetFiles(this.GamePath))
            {
                string fileName = System.IO.Path.GetFileName(f);
                if (!expectedFiles.Contains(fileName))
                {
                    string ext = System.IO.Path.GetExtension(fileName);
                    if(ext == ".7z")
                    {
                        zFiles.Add(fileName);
                    }
                    else
                    {
                        if(!exts.Contains(ext))
                        {
                            toDelete.Add(f);
                        }
                        else
                        {
                            possibleRomFiles.Add(fileName);
                        }
                    }
                }
            }
            if(possibleRomFiles.Count + zFiles.Count > 1)
            {
                int x = 0;

                /*Find if of the file is linked in the commande*/
                string foundRom = "";
                foreach(string z in zFiles)
                {
                    if(Command.Contains(z))
                    {
                        foundRom = z;
                        break;
                    }
                }
                if(foundRom == "")
                {
                    /*Rom was not found in 7z*/
                    foreach (string z in possibleRomFiles)
                    {
                        if (Command.Contains(z))
                        {
                            foundRom = z;
                            break;
                        }
                    }
                }
                if(foundRom != "")
                {
                    /*Rom found, delete other files*/
                    foreach(string z in zFiles)
                    {
                        if(z!=foundRom)
                        {
                            string fullFile = System.IO.Path.Combine(this.GamePath, z);
                            System.IO.File.Delete(fullFile);
                        }
                    }
                    foreach (string z in possibleRomFiles)
                    {
                        if (z != foundRom)
                        {
                            string fullFile = System.IO.Path.Combine(this.GamePath, z);
                            System.IO.File.Delete(fullFile);
                        }
                    }
                }
               
            }
            foreach(string f in toDelete)
            {
                System.IO.File.Delete(f);
            }
        }
        public void UniformizeFileName()
        {

            string romName = GetRomFile();

            string noZipRomName = romName;
            if (noZipRomName.EndsWith(".7z"))
            {
                noZipRomName = noZipRomName.Substring(0, noZipRomName.IndexOf(".7z"));
            }

            string ext = System.IO.Path.GetExtension(noZipRomName);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(noZipRomName);

            if(fileName!= Code)
            {
                string properFilename = Code + ext;
                if(isCompressed)
                {
                    properFilename = properFilename + ".7z";
                }
                string fullOldPath = System.IO.Path.Combine(this.GamePath, romName);
                string fullNewPath = System.IO.Path.Combine(this.GamePath, properFilename);

                System.IO.File.Move(fullOldPath, fullNewPath);
                this.Command = this.Command.Replace(romName, properFilename);

            }
        }
        public void Compress()
        {
            System.Collections.Generic.List<string> ext = new System.Collections.Generic.List<string>( AppTypeCollection.GetAppByType(this.GetType()).Extensions);
      
            foreach(string f in System.IO.Directory.GetFiles(this.GamePath))
            {
                if(ext.Contains(System.IO.Path.GetExtension(f)) )
                {
                    string filename = System.IO.Path.GetFileName(f);
                    if(this.Command.Contains(filename))
                    {
                        if(!this.Command.Contains(filename+".7z"))
                        {
                            this.Command = this.Command.Replace(filename, filename + ".7z");
                        }
                    }
                    File.WriteAllBytes(f+".7z",Compress(f));
                    System.IO.File.Delete(f);
                }
            }
            /*  if (compress)
           {
               string temp = null;
               try
               {
                   if (!File.Exists(fileName))
                   {
                       temp = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
                       File.WriteAllBytes(temp, rawRomData);
                       rawRomData = Compress(temp);
                       sevenZipped = true;
                   }
                   else
                   {
                       rawRomData = Compress(fileName);
                       sevenZipped = true;
                   }
               }
               catch (Exception ex)
               {
                   Debug.WriteLine("Compression error: " + ex.Message + ex.Source);
               }
               finally
               {
                   if (!string.IsNullOrEmpty(temp) && File.Exists(temp))
                       File.Delete(temp);
               }
           }*/


        }

        private static NesMiniApplication Import(string fileName, string sourceFile, byte[] rawRomData, char prefixCode, string application, Image defaultCover, bool compress = false)
        {
            var crc32 = CRC32(rawRomData);
            var code = GenerateCode(crc32, prefixCode);
            var gamePath = Path.Combine(GamesDirectory, code);
            bool sevenZipped = false;
            if (compress)
            {
                string temp = null;
                try
                {
                    if (!File.Exists(fileName))
                    {
                        temp = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
                        File.WriteAllBytes(temp, rawRomData);
                        rawRomData = Compress(temp);
                        sevenZipped = true;
                    }
                    else
                    {
                        rawRomData = Compress(fileName);
                        sevenZipped = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Compression error: " + ex.Message + ex.Source);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(temp) && File.Exists(temp))
                        File.Delete(temp);
                }
            }
            var romName = Regex.Replace(Path.GetFileName(fileName), @"[^A-Za-z0-9.-]", "_").Trim() + (sevenZipped ? ".7z" : "");
            var romPath = Path.Combine(gamePath, romName);
            if (Directory.Exists(gamePath))
                Directory.Delete(gamePath, true);
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(romPath, rawRomData);
            var game = new NesMiniApplication(gamePath, true);
            game.Name = Path.GetFileNameWithoutExtension(fileName);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ").Trim();
            game.FindCover(fileName, sourceFile, defaultCover, crc32);
            game.Command = string.Format("{0} /usr/share/games/nes/kachikachi/{1}/{2}", application, code, romName);
            game.Save();
            return NesMiniApplication.FromDirectory(gamePath);
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

        internal bool FindCover(string romFileName, string sourceFileName, Image defaultCover, uint crc32 = 0)
        {
            // Trying to find cover file
            Image cover = null;
            if (!string.IsNullOrEmpty(romFileName))
            {
                if (!string.IsNullOrEmpty(sourceFileName) && sourceFileName != romFileName)
                {
                    var archImagePath = Path.Combine(Path.GetDirectoryName(sourceFileName), Path.GetFileNameWithoutExtension(romFileName) + ".png");
                    if (File.Exists(archImagePath))
                        cover = LoadBitmap(archImagePath);
                    archImagePath = Path.Combine(Path.GetDirectoryName(sourceFileName), Path.GetFileNameWithoutExtension(romFileName) + ".jpg");
                    if (File.Exists(archImagePath))
                        cover = LoadBitmap(archImagePath);
                }
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

        private static byte[] Compress(string filename)
        {
            SevenZipExtractor.SetLibraryPath(Path.Combine(MainForm.BaseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            var arch = new MemoryStream();
            var compressor = new SevenZipCompressor();
            compressor.CompressionLevel = CompressionLevel.High;
            compressor.CompressFiles(arch, filename);
            arch.Seek(0, SeekOrigin.Begin);
            var result = new byte[arch.Length];
            arch.Read(result, 0, result.Length);
            return result;
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

