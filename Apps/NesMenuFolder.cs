using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
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
                    return new string[2] { string.Empty, string.Empty };
                }
                else
                {
                    var parts = desktop.Name.Split('-');
                    if (parts.Length < 2)
                    {
                        return new string[2] { string.Empty, string.Empty };
                    }
                    for (int i = 0; i < parts.Length; ++i)
                        parts[i] = parts[i].Trim();
                    return parts;
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

        private DesktopFile GetAdjustedDesktopFile()
        {
            var newDesktop = (DesktopFile)desktop.Clone();
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
            newDesktop.Exec = string.Format("/bin/chmenu {0:D3} {1}", childIndex, hakchi.GamesPath);
            newDesktop.ProfilePath = hakchi.GamesProfilePath + "/FOLDER";
            newDesktop.IconPath = hakchi.GamesPath;
            newDesktop.IconFilename = desktop.Code + ".png";
            newDesktop.TestId = 777;
            newDesktop.SortName = prefix + (desktop.Name ?? desktop.Code).ToLower();
            return newDesktop;
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
            GetAdjustedDesktopFile().Save(Path.Combine(basePath, desktop.Code + ".desktop"), false, true);

            var sourcePath = Path.Combine(FolderImagesDirectory, ImageId + ".png");
            var smallSourcePath = Path.Combine(FolderImagesDirectory, ImageId + "_small.png");
            if (!File.Exists(smallSourcePath))
                smallSourcePath = sourcePath;
            ProcessImageFile(sourcePath, iconPath, 204, 204, true, false, false);
            ProcessImageFile(smallSourcePath, smallIconPath, 40, 40, true, false, false);
            return true;
        }

        public NesMenuFolder CopyTo(string path)
        {
            SetOutputPath(path);
            Save();
            return this;
        }

        public long CopyTo(string relativeTargetPath, HashSet<ApplicationFileInfo> localGameSet)
        {
            string targetDir = relativeTargetPath.Trim('/') + "/" + desktop.Code;

            var desktopStream = GetAdjustedDesktopFile().SaveTo(new MemoryStream(), false, true);

            var sourcePath = Path.Combine(FolderImagesDirectory, ImageId + ".png");
            var smallSourcePath = Path.Combine(FolderImagesDirectory, ImageId + "_small.png");
            if (!File.Exists(smallSourcePath))
                smallSourcePath = sourcePath;
            var iconStream = ProcessImageFileToStream(sourcePath, 204, 204, true, false, false);
            var smallIconStream = ProcessImageFileToStream(smallSourcePath, 40, 40, true, false, false);

            localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}.desktop", DateTime.UtcNow, desktopStream));
            localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}.png", File.GetLastWriteTimeUtc(sourcePath), iconStream));
            localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}_small.png", File.GetLastWriteTimeUtc(smallSourcePath), smallIconStream));

            long calculatedSize =
                Shared.PadFileSize(desktopStream.Length, hakchi.BLOCK_SIZE) +
                Shared.PadFileSize(iconStream.Length, hakchi.BLOCK_SIZE) +
                Shared.PadFileSize(smallIconStream.Length, hakchi.BLOCK_SIZE);

            return calculatedSize;
        }

    }
}
