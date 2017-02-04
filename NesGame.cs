using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using nQuant;
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
    public class NesGame : INesMenuElement
    {
        public enum GameType { Cartridge, FDS }

        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public readonly string GamePath;
        public readonly string ConfigPath;
        public readonly string NesPath;
        public readonly string IconPath;
        public readonly string SmallIconPath;
        public readonly string GameGeniePath;
        public readonly GameType Type;
        private string args;
        private string region = null;

        private bool hasUnsavedChanges = true;

        public string Args
        {
            get { return args; }
            set
            {
                args = value;
                hasUnsavedChanges = true;
            }
        }
        private byte players;
        public byte Players
        {
            get { return players; }
            set
            {
                players = value;
                hasUnsavedChanges = true;
            }
        }
        private bool simultaneous;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set
            {
                simultaneous = value;
                hasUnsavedChanges = true;
            }
        }
        private string releaseDate;
        public string ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                releaseDate = value;
                hasUnsavedChanges = true;
            }
        }
        private string publisher;
        public string Publisher
        {
            get { return publisher; }
            set
            {
                publisher = value;
                hasUnsavedChanges = true;
            }
        }
        private string gameGenie = "";
        public string GameGenie
        {
            get { return gameGenie; }
            set
            {
                gameGenie = value;
                hasUnsavedChanges = true;
            }
        }
        public string Region
        {
            get { return region; }
            private set { region = value; }
        }

        private static Dictionary<uint, CachedGameInfo> gameInfoCache = null;

        const string DefaultReleaseDate = "1983-07-15"; // Famicom release day
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet";
        const string DefaultPublisher = "Nintendo";
        public const string GameGenieFileName = "gamegenie.txt";

        // TODO: Need more mapper tests.
        private byte[] supportedMappers = new byte[] { 0, 1, 2, 3, 4, 5, 7, 9, 10, 86, 87, 184 };

        public NesGame(string path)
        {
            GamePath = path;
            Code = Path.GetFileNameWithoutExtension(path);
            ConfigPath = Path.Combine(path, Code + ".desktop");
            NesPath = Path.Combine(path, Code + ".nes");
            if (File.Exists(NesPath))
                Type = GameType.Cartridge;
            else
            {
                var fdsPath = Path.Combine(path, Code + ".fds");
                if (File.Exists(fdsPath))
                {
                    NesPath = fdsPath;
                    Type = GameType.FDS;
                }
                else throw new Exception("ROM not found: " + path);
            }
            IconPath = Path.Combine(path, Code + ".png");
            SmallIconPath = Path.Combine(path, Code + "_small.png");
            GameGeniePath = Path.Combine(path, GameGenieFileName);
            if (!File.Exists(ConfigPath)) throw new Exception("Invalid game directory: " + path);

            Name = Code;
            Players = 1;
            Simultaneous = false;
            ReleaseDate = DefaultReleaseDate;
            Args = DefaultArgs;
            Publisher = DefaultPublisher;

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
                        Args = line;
                        if (Args.Contains(".nes"))
                            Args = Args.Substring(Args.IndexOf(".nes") + 4).Trim();
                        else if (Args.Contains(".fds"))
                            Args = Args.Substring(Args.IndexOf(".fds") + 4).Trim();
                        else Args = "";
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
            if (File.Exists(GameGeniePath))
                GameGenie = File.ReadAllText(GameGeniePath);
            hasUnsavedChanges = false;
        }

        public NesGame(string gamesDirectory, string nesFileName, bool ignoreMapper, ref bool? needPatch, Form parentForm = null, byte[] rawRomData = null)
        {
            uint crc32;
            if (!Path.GetExtension(nesFileName).ToLower().Equals(".fds"))
            {
                NesFile nesFile;
                if (rawRomData != null)
                    nesFile = new NesFile(rawRomData);
                else
                    nesFile = new NesFile(nesFileName);
                nesFile.CorrectRom();
                crc32 = nesFile.CRC32;
                Code = GenerateCode(crc32);
                GamePath = Path.Combine(gamesDirectory, Code);
                Args = DefaultArgs;
                Type = GameType.Cartridge;
                NesPath = Path.Combine(GamePath, Code + ".nes");
                var patchesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "patches");
                Directory.CreateDirectory(patchesDirectory);
                Directory.CreateDirectory(GamePath);
                var patches = Directory.GetFiles(patchesDirectory, string.Format("{0:X8}*.ips", crc32), SearchOption.AllDirectories);
                if (patches.Length > 0 && needPatch != false)
                {
                    if (needPatch == true || MessageBox.Show(parentForm, string.Format(Resources.PatchQ, Path.GetFileName(nesFileName)), Resources.PatchAvailable, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        needPatch = true;
                        var patch = patches[0];
                        if (rawRomData == null)
                            rawRomData = File.ReadAllBytes(nesFileName);
                        IpsPatcher.Patch(patch, ref rawRomData);
                        nesFile = new NesFile(rawRomData);
                    }
                    else needPatch = false;
                }

                if (nesFile.Mapper == 71) nesFile.Mapper = 2; // games by Codemasters/Camerica - this is UNROM clone. One exception - Fire Hawk
                if (nesFile.Mapper == 88) nesFile.Mapper = 4; // Compatible with MMC3... sometimes
                if (nesFile.Mapper == 95) nesFile.Mapper = 4; // Compatible with MMC3
                if (nesFile.Mapper == 206) nesFile.Mapper = 4; // Compatible with MMC3
                if (!supportedMappers.Contains(nesFile.Mapper) && !ignoreMapper)
                {
                    Directory.Delete(GamePath, true);
                    throw new UnsupportedMapperException(nesFile);
                }
                if (nesFile.Mirroring == NesFile.MirroringType.FourScreenVram && !ignoreMapper)
                {
                    Directory.Delete(GamePath, true);
                    throw new UnsupportedFourScreenException(nesFile);
                }
                // TODO: Make trainer check. I think that NES Mini doesn't support it.

                ConfigPath = Path.Combine(GamePath, Code + ".desktop");
                nesFile.Save(NesPath);
            }
            else
            {
                byte[] fdsData;
                if (rawRomData != null)
                    fdsData = rawRomData;
                else
                    fdsData = File.ReadAllBytes(nesFileName);
                if (Encoding.ASCII.GetString(fdsData, 0, 3) == "FDS") // header? cut it!
                {
                    var fdsDataNoHeader = new byte[fdsData.Length - 0x10];
                    Array.Copy(fdsData, 0x10, fdsDataNoHeader, 0, fdsDataNoHeader.Length);
                    fdsData = fdsDataNoHeader;
                }
                crc32 = CRC32(fdsData);
                Code = GenerateCode(crc32);
                GamePath = Path.Combine(gamesDirectory, Code);
                Args = DefaultArgs + " --fds-auto-disk-side-switch-on-keypress"; // seems like need to make it default
                Type = GameType.FDS;
                Directory.CreateDirectory(GamePath);
                ConfigPath = Path.Combine(GamePath, Code + ".desktop");
                NesPath = Path.Combine(GamePath, Code + ".fds");
                File.WriteAllBytes(NesPath, fdsData);
            }

            Name = Path.GetFileNameWithoutExtension(nesFileName);
            Players = 1;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;
            if (nesFileName.Contains("(J)"))
                Region = "Japan";

            TryAutofill(crc32);

            Name = Regex.Replace(Name, @" ?\(.*?\)", string.Empty).Trim();
            Name = Regex.Replace(Name, @" ?\[.*?\]", string.Empty).Trim();
            Name = Name.Replace("_", " ").Replace("  ", " ")/*.Replace(", The", "")*/.Trim();
            IconPath = Path.Combine(GamePath, Code + ".png");
            SmallIconPath = Path.Combine(GamePath, Code + "_small.png");
            GameGeniePath = Path.Combine(GamePath, GameGenieFileName);

            // Trying to find cover file
            Image cover = null;
            if (!string.IsNullOrEmpty(nesFileName))
            {
                var imagePath = Path.Combine(Path.GetDirectoryName(nesFileName), Path.GetFileNameWithoutExtension(nesFileName) + ".png");
                if (File.Exists(imagePath))
                    cover = Image.FromFile(imagePath);
                imagePath = Path.Combine(Path.GetDirectoryName(nesFileName), Path.GetFileNameWithoutExtension(nesFileName) + ".jpg");
                if (File.Exists(imagePath))
                    cover = Image.FromFile(imagePath);
            }
            if (cover != null)
                SetImage(cover, ConfigIni.EightBitPngCompression);
            else
                SetImage(null, ConfigIni.EightBitPngCompression);
            Save();
        }

        public bool TryAutofill(uint crc32)
        {
            CachedGameInfo gameinfo;
            if (gameInfoCache != null && gameInfoCache.TryGetValue(crc32, out gameinfo))
            {
                Name = gameinfo.Name;
                Name = Name.Replace("_", " ").Replace("  ", " ")/*.Replace(", The", "")*/.Trim();
                Players = gameinfo.Players;
                if (Players > 1) Simultaneous = true; // actually unknown...
                ReleaseDate = gameinfo.ReleaseDate;
                if (ReleaseDate.Length == 4) ReleaseDate += "-01";
                if (ReleaseDate.Length == 7) ReleaseDate += "-01";
                Publisher = gameinfo.Publisher.ToUpper();
                Region = gameinfo.Region;
                return true;
            }
            return false;
        }

        public void Save()
        {
            if (!hasUnsavedChanges) return;
            Debug.WriteLine("Saving game " + Code);
            Name = Regex.Replace(Name, @"'(\d)", @"`$1"); // Apostrophe + any number in game name crashes whole system. What. The. Fuck?
            File.WriteAllText(ConfigPath, string.Format(
                "[Desktop Entry]\n" +
                "Type=Application\n" +
                "Exec=/usr/bin/clover-kachikachi /usr/share/games/nes/kachikachi/{0}/{8} {1}\n" +
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
                Code, Args ?? DefaultArgs, Name ?? Code, Players, ReleaseDate ?? DefaultReleaseDate,
                (Name ?? Code).ToLower(), (Publisher ?? DefaultPublisher).ToUpper(),
                Simultaneous ? 1 : 0, Path.GetFileName(NesPath)));
            if (!string.IsNullOrEmpty(GameGenie.Trim()))
                File.WriteAllText(GameGeniePath, GameGenie.Trim());
            else if (File.Exists(GameGeniePath))
                File.Delete(GameGeniePath);
            hasUnsavedChanges = false;
        }

        public override string ToString()
        {
            return Name;
        }

        public void SetImage(Image image, bool EightBitCompression)
        {
            Bitmap outImage;
            Bitmap outImageSmall;
            Graphics gr;

            if (image == null)
            {
                if (Type == GameType.Cartridge)
                {
                    if (Region == "Japan")
                        image = Resources.blank_jp;
                    else
                        image = Resources.blank;
                }
                else if (Type == GameType.FDS)
                {
                    image = Resources.blank_fds;
                }
                image.Save(IconPath, ImageFormat.Png);
                outImageSmall = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                gr = Graphics.FromImage(outImageSmall);
                gr.DrawImage(image, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                gr.Flush();
                outImageSmall.Save(SmallIconPath, ImageFormat.Png);
                return;
            }

            if (Type == GameType.Cartridge)
            {
                if (image.Height > image.Width)
                {
                    outImage = new Bitmap(140, 204, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    outImageSmall = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                }
                else
                {
                    outImage = new Bitmap(204, 140, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    outImageSmall = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                }
            }
            else // if (Type == GameType.FDS)
            {
                outImage = new Bitmap(140, 158, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                outImageSmall = new Bitmap(28, 32, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            }
            gr = Graphics.FromImage(outImage);
            gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            gr.Flush();
            if (EightBitCompression)
            {
                var quantizer = new WuQuantizer();
                using (var quantized = quantizer.QuantizeImage(outImage))
                {
                    quantized.Save(IconPath, ImageFormat.Png);
                }
            }
            else outImage.Save(IconPath, ImageFormat.Png);
            gr = Graphics.FromImage(outImageSmall);
            gr.DrawImage(outImage, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height), new Rectangle(0, 0, outImage.Width, outImage.Height), GraphicsUnit.Pixel);
            gr.Flush();
            if (EightBitCompression)
            {
                var quantizer = new WuQuantizer();
                using (var quantized = quantizer.QuantizeImage(outImageSmall))
                {
                    quantized.Save(SmallIconPath, ImageFormat.Png);
                }
            }
            else outImageSmall.Save(SmallIconPath, ImageFormat.Png);
        }

        private static uint CRC32(byte[] data)
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

        private static string GenerateCode(uint crc32)
        {
            return string.Format("CLV-H-{0}{1}{2}{3}{4}",
                (char)('A' + (crc32 % 26)),
                (char)('A' + (crc32 >> 5) % 26),
                (char)('A' + ((crc32 >> 10) % 26)),
                (char)('A' + ((crc32 >> 15) % 26)),
                (char)('A' + ((crc32 >> 20) % 26)));
        }

        private struct CachedGameInfo
        {
            public string Name;
            public byte Players;
            public string ReleaseDate;
            public string Publisher;
            public string Region;
        }

        public static void LoadCache()
        {
            try
            {
                var xmlDataBasePath = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"), "nescarts.xml");
                Debug.WriteLine("Loading " + xmlDataBasePath);

                if (File.Exists(xmlDataBasePath))
                {
                    var xpath = new XPathDocument(xmlDataBasePath);
                    var navigator = xpath.CreateNavigator();
                    var iterator = navigator.Select("/database/game");
                    gameInfoCache = new Dictionary<uint, CachedGameInfo>();
                    while (iterator.MoveNext())
                    {
                        XPathNavigator game = iterator.Current;
                        var cartridges = game.Select("cartridge");
                        while (cartridges.MoveNext())
                        {
                            var cartridge = cartridges.Current;
                            try
                            {
                                var crc = Convert.ToUInt32(cartridge.GetAttribute("crc", ""), 16);
                                gameInfoCache[crc] = new CachedGameInfo
                                {
                                    Name = game.GetAttribute("name", ""),
                                    Players = (byte)((game.GetAttribute("players", "") != "1") ? 2 : 1),
                                    ReleaseDate = game.GetAttribute("date", ""),
                                    Publisher = game.GetAttribute("publisher", ""),
                                    Region = game.GetAttribute("region", "")
                                };
                            }
                            catch { }
                        };
                    }
                }
                Debug.WriteLine(string.Format("XML loading done, {0} roms total", gameInfoCache.Count));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }
}

