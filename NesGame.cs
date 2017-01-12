using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
        public string Args;
        public string Name;
        public byte Players;
        public bool Simultaneous;
        public string ReleaseDate;
        public string Publisher;

        const string DefaultReleaseDate = "1983-07-15";
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 3,2 --volume 75 --enable-armet";
        const string DefaultPublisher = "Nintendo";

        private byte[] supportedMappers = new byte[] { 0, 1, 2, 3, 4, 5, 7, 9, 10 };

        public NesGame(string path)
        {
            GamePath = path;
            Code = Path.GetFileNameWithoutExtension(path);
            ConfigPath = Path.Combine(path, Code + ".desktop");
            NesPath = Path.Combine(path, Code + ".nes");
            IconPath = Path.Combine(path, Code + ".png");
            SmallIconPath = Path.Combine(path, Code + "_small.png");
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
        }

        public override string ToString()
        {
            return Name;
        }

        public void SetImage(Image image)
        {
            Bitmap outImage;
            Bitmap outImageSmall;
            if (image.Height > image.Width)
            {
                outImage = new Bitmap(140, 204, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                outImageSmall = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            else
            {
                outImage = new Bitmap(204, 140, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                outImageSmall = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            var gr = Graphics.FromImage(outImage);
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
            outImage.Save(IconPath, ImageFormat.Png);
            gr = Graphics.FromImage(outImageSmall);
            gr.DrawImage(outImage, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height), new Rectangle(0, 0, outImage.Width, outImage.Height), GraphicsUnit.Pixel);
            gr.Flush();
            outImageSmall.Save(SmallIconPath, ImageFormat.Png);
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
                Code, Args ?? DefaultArgs, Name ?? Code, Players, ReleaseDate ?? DefaultReleaseDate, (Name ?? Code).ToLower(), (Publisher ?? DefaultPublisher) .ToUpper(), Simultaneous ? 1 : 0));
        }

        public NesGame(string gamesDirectory, string nesFileName, bool ignoreMapper = false)
        {
            if (!Path.GetExtension(nesFileName).ToLower().Equals(".fds"))
            {
                var nesFile = new NesFile(nesFileName);
                nesFile.CorrectRom();
                if (nesFile.Mapper == 71) nesFile.Mapper = 2; // games by Codemasters/Camerica - this is UNROM clone. One exception - Fire Hawk
                if (!supportedMappers.Contains(nesFile.Mapper) && !ignoreMapper)
                    throw new UnsupportedMapperException(nesFile);
                if (nesFile.Mirroring == NesFile.MirroringType.FourScreenVram && !ignoreMapper)
                    throw new UnsupportedFourScreenException(nesFile);
                var crc32 = nesFile.CRC32;
                Code = string.Format("CLV-H-{0}{1}{2}{3}{4}",
                    (char)('A' + (crc32 % 26)),
                    (char)('A' + (crc32 >> 5) % 26),
                    (char)('A' + ((crc32 >> 10) % 26)),
                    (char)('A' + ((crc32 >> 15) % 26)),
                    (char)('A' + ((crc32 >> 20) % 26)));
                GamePath = Path.Combine(gamesDirectory, Code);
                ConfigPath = Path.Combine(GamePath, Code + ".desktop");
                Directory.CreateDirectory(GamePath);
                NesPath = Path.Combine(GamePath, Code + ".nes");
                nesFile.Save(NesPath);
            }
            else
            {
                var fdsData = File.ReadAllBytes(nesFileName);
                var crc32 = CRC32(fdsData);
                Code = string.Format("CLV-H-{0}{1}{2}{3}{4}",
                    (char)('A' + (crc32 % 26)),
                    (char)('A' + (crc32 >> 5) % 26),
                    (char)('A' + ((crc32 >> 10) % 26)),
                    (char)('A' + ((crc32 >> 15) % 26)),
                    (char)('A' + ((crc32 >> 20) % 26)));
                GamePath = Path.Combine(gamesDirectory, Code);
                ConfigPath = Path.Combine(GamePath, Code + ".desktop");
                Directory.CreateDirectory(GamePath);
                NesPath = Path.Combine(GamePath, Code + ".nes");
                File.WriteAllBytes(NesPath, fdsData);
            }

            Name = Path.GetFileNameWithoutExtension(nesFileName);
            Name = Regex.Replace(Name, @" ?\(.*?\)", string.Empty).Trim();
            Name = Regex.Replace(Name, @" ?\[.*?\]", string.Empty).Trim();
            Name = Name.Replace(", The", "").Replace("_", " ").Replace("  ", " ").Trim();
            Players = 1;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;
            Args = DefaultArgs;
            IconPath = Path.Combine(GamePath, Code + ".png");
            SmallIconPath = Path.Combine(GamePath, Code + "_small.png");
            SetImage(Resources.blank);
            Save();
        }

        private static uint CRC32(byte []data)
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
    }
}
