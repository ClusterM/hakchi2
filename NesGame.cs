using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using nQuant;
using System;
using System.Collections.Generic;
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
    public class NesGame
    {
        public readonly string GamePath;
        public readonly string ConfigPath;
        public readonly string NesPath;
        public readonly string IconPath;
        public readonly string SmallIconPath;
        public readonly string Code;
        public readonly string GameGeniePath;
        public string Args;
        public string Name;
        public byte Players;
        public bool Simultaneous;
        public string ReleaseDate;
        public string Publisher;
        public string GameGenie = "";

        private static Dictionary<uint, CachedGameInfo> gameInfoCache = null;

        const string DefaultReleaseDate = "1983-07-15";
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 3,2 --volume 75 --enable-armet";
        const string DefaultPublisher = "Nintendo";
        public const string GameGenieFileName = "gamegenie.txt";

        private byte[] supportedMappers = new byte[] { 0, 1, 2, 3, 4, 5, 7, 9, 10 };

        public NesGame(string path)
        {
            GamePath = path;
            Code = Path.GetFileNameWithoutExtension(path);
            ConfigPath = Path.Combine(path, Code + ".desktop");
            NesPath = Path.Combine(path, Code + ".nes");
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
        }

        public NesGame(string gamesDirectory, string nesFileName, bool ignoreMapper = false, Form parentForm = null)
        {
            uint crc32;
            if (!Path.GetExtension(nesFileName).ToLower().Equals(".fds"))
            {
                var nesFile = new NesFile(nesFileName);
                nesFile.CorrectRom();
                crc32 = nesFile.CRC32;
                Code = GenerateCode(crc32);
                GamePath = Path.Combine(gamesDirectory, Code);
                Directory.CreateDirectory(GamePath);
                NesPath = Path.Combine(GamePath, Code + ".nes");
                var patchesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "patches");
                Directory.CreateDirectory(patchesDirectory);
                var patches = Directory.GetFiles(patchesDirectory, string.Format("{0:X8}*.ips", crc32), SearchOption.AllDirectories);
                if (patches.Length > 0)
                {
                    if (MessageBox.Show(parentForm, string.Format(Resources.PatchQ, Path.GetFileName(nesFileName)), Resources.PatchAvailable, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var patch = patches[0];
                        IpsPatcher.Patch(patch, nesFileName, NesPath);
                        nesFile = new NesFile(NesPath);
                    }
                }

                if (nesFile.Mapper == 71) nesFile.Mapper = 2; // games by Codemasters/Camerica - this is UNROM clone. One exception - Fire Hawk
                if (!supportedMappers.Contains(nesFile.Mapper) && !ignoreMapper)
                    throw new UnsupportedMapperException(nesFile);
                if (nesFile.Mirroring == NesFile.MirroringType.FourScreenVram && !ignoreMapper)
                    throw new UnsupportedFourScreenException(nesFile);

                ConfigPath = Path.Combine(GamePath, Code + ".desktop");
                nesFile.Save(NesPath);
            }
            else
            {
                var fdsData = File.ReadAllBytes(nesFileName);
                crc32 = CRC32(fdsData);
                Code = GenerateCode(crc32);
                GamePath = Path.Combine(gamesDirectory, Code);
                Directory.CreateDirectory(GamePath);
                ConfigPath = Path.Combine(GamePath, Code + ".desktop");
                NesPath = Path.Combine(GamePath, Code + ".nes");
                File.WriteAllBytes(NesPath, fdsData);
            }

            Name = Path.GetFileNameWithoutExtension(nesFileName);
            Players = 1;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;

            CachedGameInfo gameinfo;
            if (gameInfoCache != null && gameInfoCache.TryGetValue(crc32, out gameinfo))
            {
                Name = gameinfo.Name;
                Players = gameinfo.Players;
                if (Players > 1) Simultaneous = true; // actually unknown...
                ReleaseDate = gameinfo.ReleaseDate;
                if (ReleaseDate.Length == 4) ReleaseDate += "-01";
                if (ReleaseDate.Length == 7) ReleaseDate += "-01";
                Publisher = gameinfo.Publisher;
            }

            int commaPos = Name.IndexOf(",");
            if (commaPos > 0)
                Name = Name.Substring(0, commaPos);
            Name = Regex.Replace(Name, @" ?\(.*?\)", string.Empty).Trim();
            Name = Regex.Replace(Name, @" ?\[.*?\]", string.Empty).Trim();
            Name = Name.Replace("_", " ").Replace("  ", " ").Trim();
            Args = DefaultArgs;
            IconPath = Path.Combine(GamePath, Code + ".png");
            SmallIconPath = Path.Combine(GamePath, Code + "_small.png");
            GameGeniePath = Path.Combine(GamePath, GameGenieFileName);
            SetImage(null);
            Save();
        }

        public void Save()
        {
            File.WriteAllText(ConfigPath, string.Format(
                "[Desktop Entry]\n" +
                "Type=Application\n" +
                "Exec=/usr/bin/clover-kachikachi /usr/share/games/nes/kachikachi/{0}/{0}.nes {1}\n" +
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
                Code, Args ?? DefaultArgs, Name ?? Code, Players, ReleaseDate ?? DefaultReleaseDate, (Name ?? Code).ToLower(), (Publisher ?? DefaultPublisher).ToUpper(), Simultaneous ? 1 : 0));
            if (!string.IsNullOrEmpty(GameGenie.Trim()))
                File.WriteAllText(GameGeniePath, GameGenie.Trim());
            else if (File.Exists(GameGeniePath))
                File.Delete(GameGeniePath);
        }

        public override string ToString()
        {
            return Name;
        }

        public void SetImage(Image image)
        {
            Bitmap outImage;
            Bitmap outImageSmall;
            Graphics gr;

            if (image == null)
            {
                image = Resources.blank;
                image.Save(IconPath, ImageFormat.Png);
                outImageSmall = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                gr = Graphics.FromImage(outImageSmall);
                gr.DrawImage(image, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                gr.Flush();
                outImageSmall.Save(SmallIconPath, ImageFormat.Png);
                return;
            }

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
            gr = Graphics.FromImage(outImage);
            /*
            if (image.Width / image.Height > outImage.Width / outImage.Height)
                gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                    -(image.Width - (image.Height * outImage.Width / outImage.Height)) / 2, 0, image.Width * 2 - (image.Height * outImage.Width / outImage.Height), image.Height, GraphicsUnit.Pixel);
            else
                gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                    0, -(image.Height - (image.Width * outImage.Height / outImage.Width)) / 2, image.Width, image.Height * 2 - (image.Width * outImage.Height / outImage.Width), GraphicsUnit.Pixel);
            */
            gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            gr.Flush();
            var quantizer = new WuQuantizer();
            using (var quantized = quantizer.QuantizeImage(outImage))
            {
                quantized.Save(IconPath, ImageFormat.Png);
            }
            //outImage.Save(IconPath, ImageFormat.Png);
            gr = Graphics.FromImage(outImageSmall);
            gr.DrawImage(outImage, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height), new Rectangle(0, 0, outImage.Width, outImage.Height), GraphicsUnit.Pixel);
            gr.Flush();
            using (var quantized = quantizer.QuantizeImage(outImageSmall))
            {
                quantized.Save(SmallIconPath, ImageFormat.Png);
            }
            //outImageSmall.Save(SmallIconPath, ImageFormat.Png);
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
        }

        public static void LoadCache()
        {
            try
            {
                var xmlDataBasePath = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"), "nescarts.xml");

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
                                    Publisher = game.GetAttribute("publisher", "")
                                };
                            }
                            catch { }
                        };
                    }
                }
            }
            catch { }
        }
    }
}

