using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace nQuant
{
    class ImageBuffer
    {
        public ImageBuffer(Bitmap image)
        {
            this.Image = image;
        }

        public Bitmap Image;

        public IEnumerable<Pixel[]> PixelLines
        {
            get
            {
                var bitDepth = System.Drawing.Image.GetPixelFormatSize(Image.PixelFormat);
                if (bitDepth != 32)
                    throw new QuantizationException(string.Format("The image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {0} with {1} colors.", bitDepth, Image.Palette.Entries.Length));

                int width = this.Image.Width;
                int height = this.Image.Height;
                int[] buffer = new int[width];
                Pixel[] pixels = new Pixel[width];
                for (int rowIndex = 0; rowIndex < height; rowIndex++)
                {
                    BitmapData data = this.Image.LockBits(Rectangle.FromLTRB(0, rowIndex, width, rowIndex + 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        Marshal.Copy(data.Scan0, buffer, 0, width);
                        for(int pixelIndex = 0; pixelIndex < buffer.Length; pixelIndex++)
                        {
                            pixels[pixelIndex] = new Pixel(buffer[pixelIndex]);
                        }
                    }
                    finally
                    {
                        this.Image.UnlockBits(data);
                    }
                    yield return pixels;
                }
            }
        }

        public void UpdatePixelIndexes(IEnumerable<byte[]> lineIndexes)
        {
            int width = this.Image.Width;
            int height = this.Image.Height;
            var indexesIterator = lineIndexes.GetEnumerator();
            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                indexesIterator.MoveNext();
                BitmapData data = this.Image.LockBits(Rectangle.FromLTRB(0, rowIndex, width, rowIndex + 1), ImageLockMode.WriteOnly, this.Image.PixelFormat);
                try
                {
                    Marshal.Copy(indexesIterator.Current, 0, data.Scan0, indexesIterator.Current.Length);
                }
                finally
                {
                    this.Image.UnlockBits(data);
                }
            }
        }
    }
}
 
