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

        public virtual string CESortName
        {
            get {
                if (desktop.CePrefix != null && desktop.CePrefix.Length > 0)
                    return $"{desktop.CePrefix}: {desktop.SortName}";

                return desktop.SortName;
            }
        }

        public override string ToString()
        {
            if (desktop.CePrefix != null && desktop.CePrefix.Length > 0)
                return $"{desktop.CePrefix}: {(desktop.Name ?? desktop.Code)}";

            return desktop.Name ?? desktop.Code;
        }

        protected string basePath = string.Empty;
        protected string iconPath = string.Empty;
        protected string smallIconPath = string.Empty;
        protected string spinePath = string.Empty;
        protected string logoPath = string.Empty;
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
                logoPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_logo" + Path.GetExtension(desktop.IconFilename));
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
                logoPath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(desktop.IconFilename) + "_logo" + Path.GetExtension(desktop.IconFilename));
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
                    $"{desktop.Code}_spine.png",
                    $"{desktop.Code}_logo.png"
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
                GenerateMdMiniPng();

                return GetMdMiniBitmap().Crop(new Rectangle(1, 1, 28, 214)).Bitmap;
            }
        }
        public virtual Image M2Front
        {
            get
            {
                GenerateMdMiniPng();

                return GetMdMiniBitmap().Crop(new Rectangle(31, 1, 150, 214)).Bitmap;
            }
        }

        public void GenerateMdMiniPng()
        {
            if (!File.Exists(originalArtPath) && File.Exists(iconPath))
            {
                using (var file = File.OpenRead(iconPath))
                using (var bitmap = new SystemDrawingBitmap(new Bitmap(file) as Bitmap))
                using (var fileOut = File.OpenWrite(originalArtPath))
                {
                    bitmap.TrimPixels();
                    bitmap.Bitmap.Save(fileOut, ImageFormat.Png);
                }
            }

            if (!File.Exists(mdMiniIconPath))
            {
                if (File.Exists(originalArtPath))
                    SetMdMini(originalArtPath, GameImageType.MdFront);

                if (File.Exists(spinePath))
                    SetMdMini(spinePath, GameImageType.MdSpine);
            }
        }

        public virtual void SetImage(Image img, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 228;
            int maxY = 204;

            bool mdminiFormatted = false;

            if (File.Exists(originalArtPath))
                File.Delete(originalArtPath);

            if (img.Size.Width == 182 && img.Size.Height == 216)
            {
                using (var bImage = new SystemDrawingBitmap(img.Clone() as Bitmap))
                {
                    if (bImage.EmptyRow(0) && bImage.EmptyRow(215) && bImage.EmptyColumn(0) && bImage.EmptyColumn(181) && bImage.EmptyColumn(29) && bImage.EmptyColumn(30))
                    {
                        mdminiFormatted = true;
                        using (var file = File.OpenWrite(mdMiniIconPath))
                            bImage.Bitmap.Save(file, ImageFormat.Png);

                        img = bImage.Bitmap.Clone(new Rectangle(31, 1, 150, 214), PixelFormat.Format32bppArgb);
                    }
                }
            }

            using (var file = File.OpenWrite(originalArtPath))
            using (var copy = new Bitmap(img))
                copy.Save(file, ImageFormat.Png);

            if (!mdminiFormatted)
                SetMdMini(img as Bitmap, GameImageType.MdFront);

            ProcessImage(img, iconPath, maxX, maxY, false, true, EightBitCompression);

            // thumbnail image ratio
            maxX = 40;
            maxY = 40;
            ProcessImage(img, smallIconPath, maxX, maxY, ConfigIni.Instance.CenterThumbnail, false, EightBitCompression);
        }

        public virtual void SetImageFile(string path, bool EightBitCompression = false)
        {
            using (var file = File.OpenRead(path))
            using (var image = new Bitmap(file))
                SetImage(image, EightBitCompression);

        }

        public virtual void SetThumbnailFile(string path, bool EightBitCompression = false)
        {
            using (var file = File.OpenRead(path))
            using (var image = new Bitmap(file))
                SetThumbnail(image, EightBitCompression);
        }

        public virtual void SetThumbnail(Image image, bool EightBitCompression = false)
        {
            ProcessImage(image, smallIconPath, 40, 40, ConfigIni.Instance.CenterThumbnail, false, EightBitCompression);
        }
        
        private SystemDrawingBitmap GetMdMiniBitmap(MemoryStream bitmapStream = null)
        {
            if (bitmapStream != null && bitmapStream.Length > 0)
            {
                bitmapStream.Seek(0, SeekOrigin.Begin);
                return new SystemDrawingBitmap(new Bitmap(bitmapStream) as Bitmap);
            }
            else if (File.Exists(mdMiniIconPath))
            {
                using (var file = File.OpenRead(mdMiniIconPath))
                    return new SystemDrawingBitmap(new Bitmap(file) as Bitmap);
            }
            else
            {
                var template = new SystemDrawingBitmap(new Bitmap(182, 216, PixelFormat.Format32bppArgb) as Bitmap);

                return template;
            }
        }

        public enum GameImageType { AllFront, CloverFront, CloverThumbnail, MdSpine, MdFront }
        public virtual void ClearMdMini()
        {
            if (File.Exists(mdMiniIconPath))
                File.Delete(mdMiniIconPath);
        }
        public virtual void SetMdMini(string path, GameImageType type, MemoryStream bitmapStream = null)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File Not Found: {path}");

            Bitmap image;

            using (var file = File.OpenRead(path))
                image = new Bitmap(file);

            SetMdMini(image, type, bitmapStream);
        }

        public virtual void SetMdMini(Bitmap image, GameImageType type, MemoryStream bitmapStream = null, bool stretch = false)
        {
            if (type == GameImageType.MdSpine)
            {
                if (bitmapStream == null)
                {
                    if (File.Exists(spinePath))
                    File.Delete(spinePath);

                
                    using (var file = File.OpenWrite(spinePath))
                    using (var copy = new Bitmap(image))
                        copy.Save(file, ImageFormat.Png);
                }
            }

            using (var template = new SpineGen.Spine.Template<Bitmap>()
            {
                Image = GetMdMiniBitmap(bitmapStream),
                LogoArea = new Rectangle(type == GameImageType.MdFront ? 31 : 1, 1, type == GameImageType.MdFront ? 150 : 28, 214),
                LogoRotation = SpineGen.Drawing.Rotation.RotateNone,
                LogoHorizontalAlignment = SpineGen.Drawing.HorizontalAlignment.Middle,
                LogoVerticalAlignment = SpineGen.Drawing.VerticalAlignment.Middle,
                AspectRange = type == GameImageType.MdFront ? 0.04 : 0.006
            }) {
                if (stretch)
                {
                    template.AspectRange = 100;
                }

                template.Image.ClearRegion(template.LogoArea);

                using (var output = template.Process(new SystemDrawingBitmap(new Bitmap(image))))
                {
                    if (bitmapStream == null)
                    {
                        if (File.Exists(mdMiniIconPath))
                            File.Delete(mdMiniIconPath);
                    }

                    var bitmap = output.Bitmap;

                    //Quantize(ref bitmap);
                    template.Dispose();

                    if (bitmapStream == null)
                    {
                        using (var file = File.OpenWrite(mdMiniIconPath))
                            bitmap.Save(file, ImageFormat.Png);
                    }
                    else
                    {
                        bitmapStream.Seek(0, SeekOrigin.Begin);
                        bitmapStream.SetLength(0);
                        bitmap.Save(bitmapStream, ImageFormat.Png);
                        bitmapStream.Seek(0, SeekOrigin.Begin);
                    }
                    
                }
            }
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

