using SharpCompress.Writers;
using SpineGen.DrawingBitmaps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        protected string basePath = string.Empty;
        protected string iconPath = string.Empty;
        protected string smallIconPath = string.Empty;
        protected string spinePath = string.Empty;
        protected string originalArtPath = string.Empty;
        protected string mdMiniIconPath = string.Empty;

        public virtual string BasePath
        {
            get
            {
                return basePath;
            }
        }

        protected NesMenuElementBase() { }

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
                spinePath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_spine" + Path.GetExtension(desktop.IconFilename));
                originalArtPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_original" + Path.GetExtension(desktop.IconFilename));
                mdMiniIconPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_mdmini" + Path.GetExtension(desktop.IconFilename));
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
                spinePath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_spine" + Path.GetExtension(desktop.IconFilename));
                originalArtPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_original" + Path.GetExtension(desktop.IconFilename));
                mdMiniIconPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_mdmini" + Path.GetExtension(desktop.IconFilename));
    }
        }

        public virtual bool Save()
        {
            return false;
        }

        public virtual void Archive(string destinationFile)
        {
            using (var file = File.Open(destinationFile, FileMode.Create))
            using (var writer = WriterFactory.Open(file, SharpCompress.Common.ArchiveType.Tar, new WriterOptions(SharpCompress.Common.CompressionType.GZip)))
            {
                writer.WriteAll(basePath, "*", SearchOption.AllDirectories);
            }
        }

        public virtual long Size()
        {
            try
            {
                return Shared.DirectorySize(basePath, hakchi.BLOCK_SIZE, new string[] {
                    $"{desktop.Code}_original.png",
                    $"{desktop.Code}_spine.png"
                });
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
                    try
                    {
                        if (File.Exists(iconPath))
                            File.Delete(iconPath);

                        if (File.Exists(smallIconPath))
                            File.Delete(smallIconPath);

                        if (File.Exists(originalArtPath))
                            File.Delete(originalArtPath);

                        if (File.Exists(mdMiniIconPath))
                            File.Delete(mdMiniIconPath);

                        if (File.Exists(spinePath))
                            File.Delete(spinePath);
                    }
                    catch { }
                }
                else
                {
                    SetImage(value, ConfigIni.Instance.CompressCover);
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
        public virtual Image M2Spine
        {
            get
            {
                return GetM2Bitmap().Crop(new Rectangle(0, 0, 28, 214)).Bitmap;
            }
        }
        public virtual Image M2Front
        {
            get
            {
                return GetM2Bitmap().Crop(new Rectangle(28, 0, 150, 214)).Bitmap;
            }
        }

        protected virtual void SetImage(Image img, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 228;
            int maxY = 204;

            if (File.Exists(originalArtPath))
                File.Delete(originalArtPath);

            using (var file = File.OpenWrite(originalArtPath))
            using (var copy = new Bitmap(img))
                copy.Save(file, ImageFormat.Png);

            ProcessImage(img, iconPath, maxX, maxY, false, true, EightBitCompression);

            // thumbnail image ratio
            maxX = 40;
            maxY = 40;
            ProcessImage(img, smallIconPath, maxX, maxY, ConfigIni.Instance.CenterThumbnail, false, EightBitCompression);
            SetM2Engage(img as Bitmap, M2EngageImageType.Front);
        }

        public virtual void SetImageFile(string path, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 228;
            int maxY = 204;
            Bitmap image;
            using (var file = File.OpenRead(path))
                image = new Bitmap(file);

            if (File.Exists(originalArtPath))
                File.Delete(originalArtPath);

            using (var file = File.OpenWrite(originalArtPath))
            using (var copy = new Bitmap(image))
                copy.Save(file, ImageFormat.Png);

            ProcessImageFile(path, iconPath, maxX, maxY, false, true, EightBitCompression);

            // check if a small image file might have accompanied the source image
            string thumbnailPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_small" + Path.GetExtension(path));
            if (File.Exists(thumbnailPath))
                path = thumbnailPath;

            // set thumbnail as well
            SetThumbnailFile(path, EightBitCompression);
            SetM2Engage(path, M2EngageImageType.Front);


        }

        public virtual void SetThumbnailFile(string path, bool EightBitCompression = false)
        {
            // thumbnail image ratio
            ProcessImageFile(path, smallIconPath, 40, 40, ConfigIni.Instance.CenterThumbnail, false, EightBitCompression);
        }
        
        private SystemDrawingBitmap GetM2Bitmap()
        {
            if (File.Exists(mdMiniIconPath))
            {
                using (var file = File.OpenRead(mdMiniIconPath))
                    return new SystemDrawingBitmap(new Bitmap(file) as Bitmap);
            }
            else
            {
                return new SystemDrawingBitmap(new Bitmap(178, 214, PixelFormat.Format32bppArgb) as Bitmap);
            }
        }

        public enum M2EngageImageType { Spine, Front }
        public virtual void SetM2Engage(string path, M2EngageImageType type)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File Not Found: {path}");

            Bitmap image;

            using (var file = File.OpenRead(path))
                image = new Bitmap(file);

            SetM2Engage(image, type);
        }

        public virtual void SetM2Engage(Bitmap image, M2EngageImageType type)
        {
            if (type == M2EngageImageType.Spine)
            {
                if (File.Exists(spinePath))
                    File.Delete(spinePath);

                using (var file = File.OpenWrite(spinePath))
                using (var copy = new Bitmap(image))
                    copy.Save(file, ImageFormat.Png);
            }

            var template = new SpineGen.Spine.Template<Bitmap>()
            {
                Image = GetM2Bitmap(),
                LogoArea = new Rectangle(type == M2EngageImageType.Front ? 28 : 0, 0, type == M2EngageImageType.Front ? 150 : 28, 214),
                LogoRotation = SpineGen.Drawing.Rotation.RotateNone,
                LogoHorizontalAlignment = SpineGen.Drawing.HorizontalAlignment.Middle,
                LogoVerticalAlignment = SpineGen.Drawing.VerticalAlignment.Middle
            };

            template.Image.ClearRegion(template.LogoArea);

            var output = template.Process(new SystemDrawingBitmap(image));
            template.Dispose();

            if (File.Exists(mdMiniIconPath))
                File.Delete(mdMiniIconPath);

            var bitmap = output.Bitmap;

            Quantize(ref bitmap);

            using (var file = File.OpenWrite(mdMiniIconPath))
                bitmap.Save(file, ImageFormat.Png);

            output.Dispose();
        }

        protected static void ProcessImage(Image inImage, string outPath, int targetWidth, int targetHeight, bool expandHeight, bool upscale, bool quantize)
        {
            var outImage = Shared.ResizeImage(inImage, null, null, targetWidth, targetHeight, upscale, true, false, expandHeight);
            if (quantize)
                Quantize(ref outImage);
            using (var file = File.OpenWrite(outPath))
                outImage.Save(file, ImageFormat.Png);

            outImage.Dispose();
        }

        protected static void ProcessImageFile(string inPath, string outPath, int targetWidth, int targetHeight, bool expandHeight, bool upscale, bool quantize)
        {
            if (String.IsNullOrEmpty(inPath) || !File.Exists(inPath)) // failsafe
                throw new FileNotFoundException($"Image file \"{inPath}\" doesn't exist.");

            // load image
            Bitmap inImage = new Bitmap(Image.FromFile(inPath));

            // only file type candidate for direct copy is ".png"
            if (Path.GetExtension(inPath).ToLower() == ".png")
            {
                // if file is exactly the right aspect ratio, copy it
                if (!quantize && (!expandHeight || inImage.Height == targetHeight) &&
                    ((inImage.Height == targetHeight && inImage.Width <= targetWidth) ||
                     (inImage.Width == targetWidth && inImage.Height <= targetHeight)))
                {
                    Trace.WriteLine($"\"{Path.GetFileName(inPath)}\" did not need resizing, no processing done");
                    File.Copy(inPath, outPath, true);
                    return;
                }
            }

            // any other case, fully process image
            ProcessImage(inImage, outPath, targetWidth, targetHeight, expandHeight, upscale, quantize);
            File.SetLastWriteTimeUtc(outPath, File.GetLastWriteTimeUtc(inPath));
        }

        protected static Stream ProcessImageToStream(Image inImage, int targetWidth, int targetHeight, bool expandHeight, bool upscale, bool quantize)
        {
            var outImage = Shared.ResizeImage(inImage, null, null, targetWidth, targetHeight, upscale, true, false, expandHeight);
            var stream = new MemoryStream();
            if (quantize)
                Quantize(ref outImage);
            outImage.Save(stream, ImageFormat.Png);
            outImage.Dispose();
            stream.Position = 0;
            return stream;
        }

        protected static Stream ProcessImageFileToStream(string inPath, int targetWidth, int targetHeight, bool expandHeight, bool upscale, bool quantize)
        {
            if (String.IsNullOrEmpty(inPath) || !File.Exists(inPath)) // failsafe
                throw new FileNotFoundException($"Image file \"{inPath}\" doesn't exist.");

            // load image
            Bitmap inImage = new Bitmap(Image.FromFile(inPath));

            // only file type candidate for direct copy is ".png"
            if (Path.GetExtension(inPath).ToLower() == ".png")
            {
                // if file is exactly the right aspect ratio, copy it
                if (!quantize && (!expandHeight || inImage.Height == targetHeight) &&
                    ((inImage.Height == targetHeight && inImage.Width <= targetWidth) ||
                     (inImage.Width == targetWidth && inImage.Height <= targetHeight)))
                {
                    Trace.WriteLine($"\"{Path.GetFileName(inPath)}\" did not need resizing, no processing done");
                    var stream = new MemoryStream(File.ReadAllBytes(inPath));
                    stream.Position = 0;
                    return stream;
                }
            }

            // any other case, fully process image
            return ProcessImageToStream(inImage, targetWidth, targetHeight, expandHeight, upscale, quantize);
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
                Trace.WriteLine(q.Message);
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

