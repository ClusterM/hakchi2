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
        public string ReleaseDate;
        public string Publisher;

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
                "Simultaneous=0\n" +
                "ReleaseDate={4}\n" +
                "SaveCount=0\n" +
                "SortRawTitle={5}\n" +
                "SortRawPublisher={6}\n" +
                "Copyright=Copyleft\n",
                Code, Args, Name, Players, ReleaseDate, Name.ToLower(), Publisher.ToUpper()));
        }

        public NesGame(string gamesDirectory, string nesFileName)
        {
            var nesFile = new NesFile(nesFileName);
            nesFile.CorrectRom();
            if (!supportedMappers.Contains(nesFile.Mapper))
                throw new Exception(string.Format(Resources.MapperNotSupported, Path.GetFileName(nesFileName), nesFile.Mapper));
            Code = string.Format("CLV-H-{0}{1}{2}{3}{4}", 
                (char)('A' + (nesFile.CRC32 % 26)), 
                (char)('A' + (nesFile.CRC32 >> 5) % 26), 
                (char)('A' + ((nesFile.CRC32 >> 10) % 26)), 
                (char)('A' + ((nesFile.CRC32 >> 15) % 26)),
                (char)('A' + ((nesFile.CRC32 >> 20) % 26)));
            GamePath = Path.Combine(gamesDirectory, Code);
            ConfigPath = Path.Combine(GamePath, Code + ".desktop");
            Directory.CreateDirectory(GamePath);
            NesPath = Path.Combine(GamePath, Code + ".nes");
            nesFile.Save(NesPath);
            Name = Path.GetFileNameWithoutExtension(nesFileName);
            Name = Regex.Replace(Name, @" ?\(.*?\)", string.Empty).Trim();
            Name = Regex.Replace(Name, @" ?\[.*?\]", string.Empty).Trim();
            Name = Name.Replace(", The", "").Replace("_", " ").Replace("  ", " ").Trim();
            
            Players = 1;
            ReleaseDate = "1983-07-15";
            Publisher = "Nintendo";
            Args = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 3,2 --volume 75 --enable-armet";
            IconPath = Path.Combine(GamePath, Code + ".png");
            SmallIconPath = Path.Combine(GamePath, Code + "_small.png");
            SetImage(Resources.blank);
            Save();
        }
    }
}
