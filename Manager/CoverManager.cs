using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace com.clusterrr.hakchi_gui.Manager
{
    public class CoverManager
    {
        public string CoverFolder = System.IO.Path.Combine(Program.BaseDirectoryExternal, "covers");
        private CoverManager()
        {
            LoadLibrary();
        }
        private static CoverManager instance;
        public static CoverManager getInstance()
        {
            if(instance ==null)
            {
                instance = new CoverManager();
            }
            return instance;
        }
        public class Cover
        {
            public override string ToString()
            {
                return System.IO.Path.GetFileNameWithoutExtension(LocalPath);
            }
            public Cover(string path)
            {
                LocalPath = path;
            }
            public string Name { get
                {
                    return System.IO.Path.GetFileNameWithoutExtension(LocalPath);
                } }
           public string LocalPath { get; set; }
            public string SmallPath
            {
                get
                {
                    string fileName = System.IO.Path.GetFileName(LocalPath);
                    string folderName = System.IO.Path.GetDirectoryName(LocalPath);
                    return System.IO.Path.Combine(folderName, "small__" + fileName);
                }
            }
           public double Size { get
                {
                    return Math.Round(((new System.IO.FileInfo(LocalPath)).Length / 1024.0 / 1024.0), 2)+ Math.Round(((new System.IO.FileInfo(SmallPath)).Length / 1024.0 / 1024.0), 2);
                }
            }

        }
        public List<Cover> GetLibrary()
        {
            List<Cover> ret = new List<Cover>();
            ret.AddRange(_CoverLibrary);
            return ret;
        }
        private void LoadLibrary()
        {
            if(!System.IO.Directory.Exists(CoverFolder))
            {
                System.IO.Directory.CreateDirectory(CoverFolder);
            }
            string[] subFiles = System.IO.Directory.GetFiles(CoverFolder, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string s in subFiles)
            {
                if (!System.IO.Path.GetFileName(s).StartsWith("small__"))
                {


                    AddCover(s);
                }
            }
        }
        private bool IsCoverInLibrary(string localPath)
        {
            bool ret = false;
            foreach (Cover r in _CoverLibrary)
            {
                if (localPath.ToLower() == r.LocalPath.ToLower())
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public Cover GetCover(string filePath)
        {
            Cover ret = null;

            foreach (Cover r in _CoverLibrary)
            {
                if (filePath.ToLower() == r.LocalPath.ToLower())
                {
                    ret = r;
                    break;
                }
            }

            return ret;
        }
        public Cover GetCoverByName(string name)
        {
            Cover ret = null;
            foreach(Cover c in _CoverLibrary)
            {
                if(System.IO.Path.GetFileNameWithoutExtension(c.LocalPath).ToLower() == name.ToLower())
                {
                    ret = c;
                    break;
                }
            }
            return ret;
        }
        public Cover AddCover(string filePath)
        {
            Cover ret = null;

            if (System.IO.File.Exists(filePath))
            {
                string destinationPath = System.IO.Path.Combine(CoverFolder, System.IO.Path.GetExtension(filePath).Replace(".", "") + "\\" + System.IO.Path.GetFileName(filePath));
                string smallDestPath = System.IO.Path.Combine(CoverFolder, System.IO.Path.GetExtension(filePath).Replace(".", "") + "\\small__" + System.IO.Path.GetFileName(filePath));
                string ext = System.IO.Path.GetExtension(filePath);
                //  if (EmulatorManager.getInstance().isFileValidRom(ext))
                {
                    if (!IsCoverInLibrary(destinationPath))
                    {
                        if (destinationPath != filePath)
                        {
                            string folder = System.IO.Path.GetDirectoryName(destinationPath);
                            if (!System.IO.Directory.Exists(folder))
                            {
                                System.IO.Directory.CreateDirectory(folder);
                            }
                            if (System.IO.File.Exists(destinationPath))
                            {
                                System.IO.File.Delete(destinationPath);
                            }
                            if (System.IO.File.Exists(smallDestPath))
                            {
                                System.IO.File.Delete(smallDestPath);
                            }


                            Bitmap outImage;
                            Bitmap outImageSmall;
                            Graphics gr;
                            Bitmap image = new Bitmap(filePath);
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
                            outImage.Save(destinationPath, System.Drawing.Imaging.ImageFormat.Png);
                            gr = Graphics.FromImage(outImageSmall);
                            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            gr.DrawImage(outImage, new Rectangle(0, 0, outImageSmall.Width, outImageSmall.Height),
                                new Rectangle(0, 0, outImage.Width, outImage.Height), GraphicsUnit.Pixel);
                            gr.Flush();
                            outImageSmall.Save(smallDestPath, System.Drawing.Imaging.ImageFormat.Png);
     
                        }
                        ret = new Cover(destinationPath);

                        _CoverLibrary.Add(ret);
                       
                    }
                    else
                    {
                        ret = GetCover(destinationPath);
                    }
                }
            }
            return ret;
        }

        private List<Cover> _CoverLibrary = new List<Cover>();
    }
}
