using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuElementBase : INesMenuElement
    {
        protected DesktopFile desktop = new DesktopFile();
        public DesktopFile Desktop
        {
            get { return desktop; }
        }

        public virtual string Code
        {
            get { return desktop.Code; }
        }
        public virtual string Name
        {
            get { return desktop.Name; }
            set { desktop.Name = value; }
        }
        public virtual string SortName
        {
            get { return desktop.SortName; }
        }

        public override string ToString()
        {
            return desktop.Name ?? desktop.Code;
        }

        protected string basePath;
        protected string iconPath;
        protected string smallIconPath;

        public virtual string BasePath
        {
            get
            {
                return basePath;
            }
        }

        protected NesMenuElementBase()
        {
            basePath = string.Empty;
            iconPath = string.Empty;
            smallIconPath = string.Empty;
        }

        protected NesMenuElementBase(string path, bool ignoreEmptyConfig = false)
        {
            basePath = path;
            string code = Path.GetFileName(basePath);
            string configPath = Path.Combine(basePath, code + ".desktop");

            if (File.Exists(configPath))
            {
                desktop.Load(configPath);
                iconPath = Path.Combine(basePath, desktop.IconFilename);
                smallIconPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_small" + Path.GetExtension(desktop.IconFilename));
            }
            else
            {
                if (!ignoreEmptyConfig)
                    throw new FileNotFoundException($"Invalid application directory: {path}");

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                desktop.Code = code;
                desktop.Name = code;
                desktop.IconFilename = code + ".png";
                desktop.Save(configPath);

                iconPath = Path.Combine(basePath, desktop.IconFilename);
                smallIconPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_small" + Path.GetExtension(desktop.IconFilename));
            }
        }

        public virtual bool Save()
        {
            return false;
        }

        public virtual long Size()
        {
            try
            {
                return Shared.DirectorySize(basePath);
            }
            catch { }
            return 0;
        }

        public virtual Image Image
        {
            set
            {
                if (value == null)
                {
                    if (File.Exists(iconPath))
                    {
                        try
                        {
                            File.Delete(iconPath);
                            File.Delete(smallIconPath);
                        }
                        catch { }
                    }
                }
                else
                {
                    SetImage(value, ConfigIni.CompressCover);
                }
            }
            get
            {
                return File.Exists(iconPath) ? Shared.LoadBitmapCopy(iconPath) : null;
            }
        }

        public virtual Image Thumbnail
        {
            get
            {
                return File.Exists(smallIconPath) ? Shared.LoadBitmapCopy(smallIconPath) : null;
            }
        }

        protected static void ProcessImage(Image inImage, string outPath, int targetWidth, int targetHeight, bool enforceHeight, bool upscale, bool quantize)
        {
            int X, Y;
            if (!upscale && inImage.Width <= targetWidth && inImage.Height <= targetHeight)
            {
                X = inImage.Width;
                Y = inImage.Height;
            }
            else if ((double)inImage.Width / (double)inImage.Height > (double)targetWidth / (double)targetHeight)
            {
                X = targetWidth;
                Y = (int)Math.Round((double)targetWidth * (double)inImage.Height / (double)inImage.Width);
                if (Y % 2 == 1) ++Y;
            }
            else
            {
                X = (int)Math.Round((double)targetHeight * (double)inImage.Width / (double)inImage.Height);
                if (X % 2 == 1) ++X;
                Y = targetHeight;
            }

            Bitmap outImage = new Bitmap(X, enforceHeight ? targetHeight : Y);
            Rectangle outRect = (enforceHeight && Y < targetHeight) ?
                new Rectangle(0, (int)((double)(targetHeight - Y) / 2), outImage.Width, Y) :
                new Rectangle(0, 0, outImage.Width, outImage.Height);
            using (Graphics gr = Graphics.FromImage(outImage))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY); // Fix first line and column alpha shit
                    gr.DrawImage(inImage, outRect, 0, 0, inImage.Width, inImage.Height, GraphicsUnit.Pixel, ia);
                }
                gr.Flush();
            }
            if (quantize)
                Quantize(ref outImage);
            outImage.Save(outPath, ImageFormat.Png);
            outImage.Dispose();
        }

        protected static void ProcessImageFile(string inPath, string outPath, int targetWidth, int targetHeight, bool enforceHeight, bool upscale, bool quantize)
        {
            if (String.IsNullOrEmpty(inPath) || !File.Exists(inPath)) // failsafe
                throw new FileNotFoundException($"Image file \"{inPath}\" doesn't exist.");

            // load image
            Bitmap inImage = new Bitmap(Image.FromFile(inPath));

            // only file type candidate for direct copy is ".png"
            if (Path.GetExtension(inPath).ToLower() == ".png")
            {
                // if file is exactly the right aspect ratio, copy it
                if (!quantize && (!enforceHeight || inImage.Height == targetHeight) &&
                    ((inImage.Height == targetHeight && inImage.Width <= targetWidth) ||
                     (inImage.Width == targetWidth && inImage.Height <= targetHeight)))
                {
                    Debug.WriteLine($"NesMenuElementBase.ProcessImageFile: \"{Path.GetFileName(inPath)}\" did not need resizing. No processing done.");
                    File.Copy(inPath, outPath, true);
                    return;
                }
            }

            // any other case, fully process image
            ProcessImage(inImage, outPath, targetWidth, targetHeight, enforceHeight, upscale, quantize);
        }

        protected void SetImage(Image img, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 204;
            int maxY = 204;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                maxX = 228;
                maxY = 204;
            }
            ProcessImage(img, iconPath, maxX, maxY, false, true, EightBitCompression);

            // thumbnail image ratio
            maxX = 40;
            maxY = 40;
            ProcessImage(img, smallIconPath, maxX, maxY, true, false, EightBitCompression);
        }

        public void SetImageFile(string path, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 204;
            int maxY = 204;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                maxX = 228;
                maxY = 204;
            }
            ProcessImageFile(path, iconPath, maxX, maxY, false, true, EightBitCompression);

            // check if a small image file might have accompanied the source image
            string thumbnailPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_small" + Path.GetExtension(path));
            if (File.Exists(thumbnailPath))
                path = thumbnailPath;

            // set thumbnail as well
            SetThumbnailFile(path, EightBitCompression);
        }

        public void SetThumbnailFile(string path, bool EightBitCompression = false)
        {
            // thumbnail image ratio
            ProcessImageFile(path, smallIconPath, 40, 40, true, false, EightBitCompression);
        }

        private static void Quantize(ref Bitmap img)
        {
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
                ConvertTo32bppAndDisposeOriginal(ref img);

            try
            {
                var quantizer = new nQuant.WuQuantizer();
                Bitmap quantized = (Bitmap)quantizer.QuantizeImage(img);
                img.Dispose();
                img = quantized;
            }
            catch (nQuant.QuantizationException q)
            {
                Debug.WriteLine(q.Message);
            }
        }

        private static void ConvertTo32bppAndDisposeOriginal(ref Bitmap img)
        {
            var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            img.Dispose();
            img = bmp;
        }

        public class NesMenuElementBaseComparer : IEqualityComparer<NesMenuElementBase>
        {
            public bool Equals(NesMenuElementBase x, NesMenuElementBase y)
            {
                return x.Code == y.Code;
            }

            public int GetHashCode(NesMenuElementBase obj)
            {
                return obj.Code.GetHashCode();
            }
        }

    }
}

