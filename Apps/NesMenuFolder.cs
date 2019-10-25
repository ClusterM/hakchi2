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
            Back = 5,
            LeftBack = 6
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
                    case Priority.LeftBack:
                        desktop.Players = 2;
                        desktop.Simultaneous = true;
                        desktop.ReleaseDate = "0000-00-00";
                        desktop.Publisher = new String((char)1, 10);
                        break;
                    case Priority.Leftmost:
                        desktop.Players = 2;
                        desktop.Simultaneous = true;
                        desktop.ReleaseDate = "0001-11-11";
                        desktop.Publisher = new String((char)2, 10);
                        break;
                    case Priority.Left:
                        desktop.Players = 2;
                        desktop.Simultaneous = true;
                        desktop.ReleaseDate = "0002-22-22";
                        desktop.Publisher = new String((char)3, 10);
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

        public NesMenuFolder(string name = "Folder", string imageId = "folder", string imageSet = null)
            : base()
        {
            desktop.Name = name;
            Position = Priority.Right;
            ImageSet = imageSet ?? ConfigIni.Instance.FolderImagesSet;
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
                string imagePath = getImagePath(imageId);
                if (imagePath != null)
                {
                    return Image.FromFile(imagePath);
                }
                else if (imageId != null && rm.GetObject(imageId) != null)
                {
                    return (Image)rm.GetObject(imageId);
                }
                return (Image)rm.GetObject("folder");
            }
        }

        public override Image Thumbnail
        {
            get
            {
                return Image;
            }
        }

        private string imageSet;
        public string ImageSet
        {
            get { return imageSet; }
            set
            {
                imageSet = null;
                if (value != null && Directory.Exists(Path.Combine(FolderImagesDirectory, value)))
                {
                    imageSet = value;
                }
            }
        }

        private string imageId;
        public string ImageId
        {
            get { return imageId; }
            set
            {
                if (value == null)
                {
                    imageId = null;
                    return;
                }

                if (getImagePath(value) != null || rm.GetObject(value) != null)
                {
                    imageId = value;
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"Folder image id \"{value??"NULL"}\" is invalid. No corresponding file or resource exists.");
                    imageId = "folder";
                }
            }
        }

        private string getImagePath(string id)
        {
            if (id == null)
                return null;
            string imagePath = Path.Combine(FolderImagesDirectory, id + ".png");
            string overrideImagePath = imageSet != null ? Path.Combine(FolderImagesDirectory, imageSet, id + ".png") : imagePath;
            if (File.Exists(overrideImagePath))
                return overrideImagePath;
            if (File.Exists(imagePath))
                return imagePath;
            return null;
        }

        private DesktopFile getAdjustedDesktopFile()
        {
            var newDesktop = (DesktopFile)desktop.Clone();
            char prefix;
            switch (position)
            {
                case Priority.LeftBack:
                    prefix = (char)1;
                    break;
                case Priority.Leftmost:
                    prefix = (char)2;
                    break;
                default:
                case Priority.Left:
                    prefix = (char)3;
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
            getAdjustedDesktopFile().Save(Path.Combine(basePath, desktop.Code + ".desktop"), false, true);

            var maxX = 228;
            var maxY = 204;
            var sourcePath = getImagePath(ImageId);
            if (sourcePath != null)
            {
                var smallSourcePath = sourcePath.Replace(".png", "_small.png");
                if (!File.Exists(smallSourcePath))
                    smallSourcePath = sourcePath;
                ProcessImageFile(sourcePath, iconPath, maxX, maxY, true, false, false);
                ProcessImageFile(smallSourcePath, smallIconPath, 40, 40, true, false, false);
            }
            else
            {
                ProcessImage(Image, iconPath, maxX, maxY, true, false, false);
                ProcessImage(Image, smallIconPath, 40, 40, true, false, false);
            }
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

            var desktopStream = getAdjustedDesktopFile().SaveTo(new MemoryStream(), false, true);
            localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}.desktop", DateTime.UtcNow, desktopStream));

            var maxX = 228;
            var maxY = 204;
            var sourcePath = getImagePath(ImageId);
            Stream iconStream, smallIconStream;
            if (sourcePath != null)
            {
                var smallSourcePath = sourcePath.Replace(".png", "_small.png");
                if (!File.Exists(smallSourcePath))
                    smallSourcePath = sourcePath;
                iconStream = ProcessImageFileToStream(sourcePath, maxX, maxY, true, false, false);
                smallIconStream = ProcessImageFileToStream(smallSourcePath, 40, 40, true, false, false);

                localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}.png", File.GetLastWriteTimeUtc(sourcePath), iconStream));
                localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}_small.png", File.GetLastWriteTimeUtc(smallSourcePath), smallIconStream));
            }
            else
            {
                iconStream = ProcessImageToStream(Image, maxX, maxY, true, false, false);
                smallIconStream = ProcessImageToStream(Image, 40, 40, true, false, false);

                localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}.png", DateTime.UtcNow, iconStream));
                localGameSet.Add(new ApplicationFileInfo($"./{targetDir}/{desktop.Code}_small.png", DateTime.UtcNow, smallIconStream));
            }

            long calculatedSize =
                Shared.PadFileSize(desktopStream.Length, hakchi.BLOCK_SIZE) +
                Shared.PadFileSize(iconStream.Length, hakchi.BLOCK_SIZE) +
                Shared.PadFileSize(smallIconStream.Length, hakchi.BLOCK_SIZE);

            return calculatedSize;
        }

    }
}
