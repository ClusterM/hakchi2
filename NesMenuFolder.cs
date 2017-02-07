using com.clusterrr.hakchi_gui.Properties;
using nQuant;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuFolder : INesMenuElement
    {
        static Random rnd = new Random();
        private string code = null;
        public enum Priority
        {
            First = 0,
            Left = 1,
            Right = 3,
            Last = 4
        }
        private Priority position;
        //private bool first = true;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        private string name = null;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (!string.IsNullOrEmpty(name))
                    nameParts = new string[] { name };
                else
                    nameParts = new string[0];
            }
        }
        private string[] nameParts;
        public string[] NameParts
        {
            get { return nameParts; }
            set
            {
                nameParts = value;
                if (value != null)
                    name = string.Join(" - ", nameParts);
                else
                    name = null;
            }
        }
        public NesMenuCollection Child = new NesMenuCollection();
        public string Initial = "";
        private Image Icon;
        private Image ThumbnailIcon;

        byte Players = 2;
        byte Simultaneous = 1;
        string ReleaseDate = "0000-00-00";
        string Publisher = new String('!', 10);

        // It's workaround for sorting
        public Priority Position
        {
            set
            {
                // Sort to left
                position = value;
                switch (position)
                {
                    case Priority.First:
                        Players = 2;
                        Simultaneous = 1;
                        ReleaseDate = "0000-00-00";
                        Publisher = new String((char)1, 10);
                        break;
                    case Priority.Left:
                        Players = 2;
                        Simultaneous = 1;
                        ReleaseDate = "1111-11-11";
                        Publisher = new String((char)2, 10);
                        break;
                    case Priority.Right:
                        Players = 1;
                        Simultaneous = 0;
                        ReleaseDate = "7777-77-77";
                        Publisher = new String('Z', 9) + "X";
                        break;
                    case Priority.Last:
                        Players = 1;
                        Simultaneous = 0;
                        ReleaseDate = "9999-99-99";
                        Publisher = new String('Z', 10);
                        break;
                }
            }
            get
            {
                return position;
            }
        }

        public NesMenuFolder()
        {
            Code = GenerateCode((uint)rnd.Next());
            Name = "Folder";
            Position = Priority.Left;
            Image = null;
        }

        public Image Image
        {
            set
            {
                Graphics gr;
                if (value == null)
                {
                    Icon = Resources.folder;
                    ThumbnailIcon = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    gr = Graphics.FromImage(ThumbnailIcon);
                    gr.DrawImage(Icon, new Rectangle(0, 0, ThumbnailIcon.Width, ThumbnailIcon.Height), new Rectangle(0, 0, Icon.Width, Icon.Height), GraphicsUnit.Pixel);
                    gr.Flush();
                    return;
                }
                if (value.Height > value.Width)
                {
                    Icon = new Bitmap(140, 204, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    ThumbnailIcon = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                }
                else
                {
                    Icon = new Bitmap(204, 140, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    ThumbnailIcon = new Bitmap(28, 40, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                }
                gr = Graphics.FromImage(Icon);
                gr.DrawImage(value, new Rectangle(0, 0, Icon.Width, Icon.Height),
                                    new Rectangle(0, 0, value.Width, value.Height), GraphicsUnit.Pixel);
                gr.Flush();

                gr = Graphics.FromImage(ThumbnailIcon);
                gr.DrawImage(Icon, new Rectangle(0, 0, ThumbnailIcon.Width, ThumbnailIcon.Height), new Rectangle(0, 0, Icon.Width, Icon.Height), GraphicsUnit.Pixel);
                gr.Flush();
            }
            get { return Icon; }
        }
        public void Save(string path, int index)
        {
            Directory.CreateDirectory(path);
            var ConfigPath = Path.Combine(path, Code + ".desktop");
            var IconPath = Path.Combine(path, Code + ".png");
            var ThumnnailIconPath = Path.Combine(path, Code + "_small.png");
            char prefix;
            switch (position)
            {
                case Priority.First:
                    prefix = (char)1;
                    break;
                default:
                case Priority.Left:
                    prefix = (char)2;
                    break;
                case Priority.Right:
                    prefix = 'Ю';
                    break;
                case Priority.Last:
                    prefix = 'Я';
                    break;
            }
            File.WriteAllText(ConfigPath, string.Format(
                 "[Desktop Entry]\n" +
                 "Type=Application\n" +
                 "Exec=/bin/chmenu {1:D3} {8}\n" +
                 "Path=/var/lib/clover/profiles/0/FOLDER\n" +
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
                 Code, index, Name ?? Code, Players, ReleaseDate,
                 prefix + (Name ?? Code).ToLower(), (Publisher ?? "").ToUpper(),
                 Simultaneous, Initial)
                 );
            if (Icon == null)
                Image = null;
            Icon.Save(IconPath, ImageFormat.Png);
            ThumbnailIcon.Save(ThumnnailIconPath, ImageFormat.Png);
        }

        private static string GenerateCode(uint crc32)
        {
            return string.Format("CLV-S-{0}{1}{2}{3}{4}",
                (char)('A' + (crc32 % 26)),
                (char)('A' + (crc32 >> 5) % 26),
                (char)('A' + ((crc32 >> 10) % 26)),
                (char)('A' + ((crc32 >> 15) % 26)),
                (char)('A' + ((crc32 >> 20) % 26)));
        }

        public override string ToString()
        {
            return Name ?? Code;
        }
    }
}
