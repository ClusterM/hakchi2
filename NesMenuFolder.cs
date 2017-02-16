using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuFolder : INesMenuElement
    {
        static Random rnd = new Random();
        static ResourceManager rm = Resources.ResourceManager;
        public static readonly string FolderImagesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "folder_images");

        private int childIndex = 0;

        public int ChildIndex
        {
            get { return childIndex; }
            set { childIndex = value; }
        }

        public enum Priority
        {
            Leftmost = 0,
            Left = 1,
            Right = 3,
            Rightmost = 4,
            Back = 5
        }
        private Priority position;

        public string Code
        {
            get { return string.Format("CLV-S-{0:D5}", childIndex); }
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
        public NesMenuCollection ChildMenuCollection = new NesMenuCollection();
        public string Initial = "";
        private Image image;
        private string imageId;

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
                    case Priority.Leftmost:
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
                    case Priority.Rightmost:
                        Players = 1;
                        Simultaneous = 0;
                        ReleaseDate = "8888-88-88";
                        Publisher = new String('Z', 9) + "Y";
                        break;
                    case Priority.Back:
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

        public NesMenuFolder(string name = "Folder", string imageId = "folder")
        {
            Name = name;
            Position = Priority.Right;
            ImageId = imageId;
        }

        public Image Image
        {
            set
            {
                if (value == null)
                    ImageId = "folder";
                else
                {
                    image = Image;
                    imageId = null;
                }
            }
            get
            {
                Bitmap outImage;
                Graphics gr;
                if (image == null)
                    ImageId = "folder";
                // Just keep aspect ratio
                const int maxX = 204;
                const int maxY = 204;
                if (image.Width <= maxX && image.Height <= maxY) // Do not upscale
                    return image;
                if (image.Width / image.Height > maxX / maxY)
                    outImage = new Bitmap(maxX, maxY * image.Height / image.Width);
                else
                    outImage = new Bitmap(maxX * image.Width / image.Height, maxY);
                gr = Graphics.FromImage(outImage);
                gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                gr.Flush();
                return outImage;
            }
        }

        public Image ImageThumbnail
        {
            get
            {
                Bitmap outImage;
                Graphics gr;
                if (image == null)
                    ImageId = "folder";
                // Just keep aspect ratio
                const int maxX = 40;
                const int maxY = 40;
                if (image.Width <= maxX && image.Height <= maxY) // Do not upscale
                    return image;
                if (image.Width / image.Height > maxX / maxY)
                    outImage = new Bitmap(maxX, maxY * image.Height / image.Width);
                else
                    outImage = new Bitmap(maxX * image.Width / image.Height, maxY);
                gr = Graphics.FromImage(outImage);
                gr.DrawImage(image, new Rectangle(0, 0, outImage.Width, outImage.Height),
                                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                gr.Flush();
                return outImage;
            }
        }

        public string ImageId
        {
            get { return imageId; }
            set
            {
                var folderImagesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "folder_images");
                var filePath = Path.Combine(folderImagesDirectory, value + ".png");
                if (File.Exists(filePath))
                    image = NesMiniApplication.LoadBitmap(filePath);
                else
                    image = (Image)rm.GetObject(value);
                imageId = value;
            }
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(path);
            var ConfigPath = Path.Combine(path, Code + ".desktop");
            var IconPath = Path.Combine(path, Code + ".png");
            var ThumnnailIconPath = Path.Combine(path, Code + "_small.png");
            char prefix;
            switch (position)
            {
                case Priority.Leftmost:
                    prefix = (char)1;
                    break;
                default:
                case Priority.Left:
                    prefix = (char)2;
                    break;
                case Priority.Right:
                    prefix = 'Э';
                    break;
                case Priority.Rightmost:
                    prefix = 'Ю';
                    break;
                case Priority.Back:
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
                 Code, ChildIndex, Name ?? Code, Players, ReleaseDate,
                 prefix + (Name ?? Code).ToLower(), (Publisher ?? "").ToUpper(),
                 Simultaneous, Initial)
                 );
            Image.Save(IconPath, ImageFormat.Png);
            ImageThumbnail.Save(ThumnnailIconPath, ImageFormat.Png);
        }

        public override string ToString()
        {
            return Name ?? Code;
        }
    }
}
