using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuFolder : NesMenuElementBase
    {
        static Random rnd = new Random();
        static ResourceManager rm = Resources.ResourceManager;
        public static readonly string FolderImagesDirectory = Path.Combine(Program.BaseDirectoryExternal, "folder_images");

        private int childIndex = 0;
        public int ChildIndex
        {
            get { return childIndex; }
            set
            {
                childIndex = value;
                desktop.Code = string.Format("CLV-S-{0:D5}", childIndex);
            }
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

        public override string SortName
        {
            get { return desktop.Name; }
        }

        public string[] NameParts
        {
            get
            {
                if( string.IsNullOrEmpty(desktop.Name))
                {
                    return new string[0];
                }
                else
                {
                    return new string[] { desktop.Name };
                }
            }
            set
            {
                desktop.Name = value != null ? string.Join(" - ", value) : string.Empty;
            }
        }
        public NesMenuCollection ChildMenuCollection = new NesMenuCollection();
        public string Initial = string.Empty;

        // Workaround for sorting
        public Priority Position
        {
            set
            {
                // Sort to left
                position = value;
                switch (position)
                {
                    case Priority.Leftmost:
                        desktop.Players = 2;
                        desktop.Simultaneous = true;
                        desktop.ReleaseDate = "0000-00-00";
                        desktop.Publisher = new String((char)1, 10);
                        break;
                    case Priority.Left:
                        desktop.Players = 2;
                        desktop.Simultaneous = true;
                        desktop.ReleaseDate = "1111-11-11";
                        desktop.Publisher = new String((char)2, 10);
                        break;
                    case Priority.Right:
                        desktop.Players = 1;
                        desktop.Simultaneous = false;
                        desktop.ReleaseDate = "7777-77-77";
                        desktop.Publisher = new String('Z', 9) + "X";
                        break;
                    case Priority.Rightmost:
                        desktop.Players = 1;
                        desktop.Simultaneous = false;
                        desktop.ReleaseDate = "8888-88-88";
                        desktop.Publisher = new String('Z', 9) + "Y";
                        break;
                    case Priority.Back:
                        desktop.Players = 1;
                        desktop.Simultaneous = false;
                        desktop.ReleaseDate = "9999-99-99";
                        desktop.Publisher = new String('Z', 10);
                        break;
                }
            }
            get
            {
                return position;
            }
        }

        public NesMenuFolder(string name = "Folder", string imageId = "folder")
            : base()
        {
            desktop.Name = name;
            Position = Priority.Right;
            ImageId = imageId;
            desktop.Players = 2;
            desktop.Simultaneous = true;
            desktop.ReleaseDate = "0000-00-00";
            desktop.Publisher = new String('!', 10);
        }

        public override Image Image
        {
            get
            {
                var filePath = Path.Combine(FolderImagesDirectory, imageId + ".png");
                if (File.Exists(filePath))
                {
                    return Image.FromFile(filePath);
                }
                else if (rm.GetObject(imageId) != null)
                {
                    return (Image)rm.GetObject(imageId);
                }
                return null;
            }
        }

        public override Image Thumbnail
        {
            get
            {
                return Image;
            }
        }

        private string imageId;
        public string ImageId
        {
            get { return imageId; }
            set
            {
                var filePath = Path.Combine(FolderImagesDirectory, value + ".png");
                if (File.Exists(filePath) || rm.GetObject(value) != null)
                {
                    imageId = value;
                }
                else
                {
                    throw new FileNotFoundException($"Folder image id \"{imageId}\" is invalid. No corresponding file or resource exists.");
                }
            }
        }

        public void SetOutputPath(string path)
        {
            basePath = path;
            iconPath = Path.Combine(path, desktop.Code + ".png");
            smallIconPath = Path.Combine(path, desktop.Code + "_small.png");
        }

        public override bool Save()
        {
            if (string.IsNullOrEmpty(basePath))
                return false;

            Directory.CreateDirectory(basePath);
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
            desktop.Exec = string.Format("/bin/chmenu {0:D3} {1}", childIndex, NesApplication.GamesHakchiPath);
            desktop.ProfilePath = NesApplication.GamesHakchiProfilePath + "/FOLDER";
            desktop.IconPath = NesApplication.GamesHakchiPath;
            desktop.IconFilename = desktop.Code + ".png";
            desktop.TestId = 777;
            desktop.SortName = prefix + (desktop.Name ?? desktop.Code).ToLower();
            desktop.Save(Path.Combine(basePath, desktop.Code + ".desktop"), false, true);

            var filePath = Path.Combine(FolderImagesDirectory, ImageId + ".png");
            ProcessImageFile(filePath, iconPath, 204, 204, true, false, false);
            ProcessImageFile(filePath, smallIconPath, 40, 40, true, false, false);
            return true;
        }
    }
}
