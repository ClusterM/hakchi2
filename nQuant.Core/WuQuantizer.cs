using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace nQuant
{
    public class WuQuantizer : WuQuantizerBase, IWuQuantizer
    {
        private static IEnumerable<byte[]> indexedPixels(ImageBuffer image, Pixel[] lookups, int alphaThreshold, int maxColors, PaletteColorHistory[] paletteHistogram)
        {
            int pixelsCount = image.Image.Width * image.Image.Height;
            var lineIndexes = new byte[(int)System.Math.Ceiling(image.Image.Width / System.Math.Log(256.0, maxColors))];
            PaletteLookup lookup = new PaletteLookup(lookups);
            --maxColors;
            foreach (var pixelLine in image.PixelLines)
            {
                for (int pixelIndex = 0; pixelIndex < pixelLine.Length; pixelIndex++)
                {
                    Pixel pixel = pixelLine[pixelIndex];
                    byte bestMatch = (byte)maxColors;
                    if (pixel.Alpha > alphaThreshold)
                    {
                        bestMatch = lookup.GetPaletteIndex(pixel);
                        paletteHistogram[bestMatch].AddPixel(pixel);
                    }
                    switch (maxColors)
                    {
                        case 256 - 1:
                            lineIndexes[pixelIndex] = bestMatch;
                            break;
                        case 16 - 1:
                            if (pixelIndex % 2 == 0) { lineIndexes[pixelIndex / 2] = (byte)(bestMatch << 4); }
                            else { lineIndexes[pixelIndex / 2] |= (byte)(bestMatch & 0x0F); }
                            break;
                        case 2 - 1:
                            if (pixelIndex % 8 == 0) { lineIndexes[pixelIndex / 8] = (byte)(bestMatch << 7); }
                            else { lineIndexes[pixelIndex / 8] |= (byte)((bestMatch & 0x01) << (7 - (pixelIndex % 8))); }
                            break;
                    }
                }
                yield return lineIndexes;
            }
        }

        internal override Image GetQuantizedImage(ImageBuffer image, int colorCount, int maxColors, Pixel[] lookups, int alphaThreshold)
        {
            PixelFormat pf;
            switch (maxColors)
            {
                case 256: pf = PixelFormat.Format8bppIndexed; break;
                case 16: pf = PixelFormat.Format4bppIndexed; break;
                case 2: pf = PixelFormat.Format1bppIndexed; break;
                default: throw new QuantizationException(string.Format("The target amount of colors is not supported. Requested {0} colors.", maxColors));
            }
            var result = new Bitmap(image.Image.Width, image.Image.Height, pf);
            var resultBuffer = new ImageBuffer(result);
            var paletteHistogram = new PaletteColorHistory[colorCount + 1];
            resultBuffer.UpdatePixelIndexes(indexedPixels(image, lookups, alphaThreshold, maxColors, paletteHistogram));
            result.Palette = BuildPalette(result.Palette, paletteHistogram);
            return result;
        }

        private static ColorPalette BuildPalette(ColorPalette palette, PaletteColorHistory[] paletteHistogram)
        {
            for (int paletteColorIndex = 0; paletteColorIndex < paletteHistogram.Length; paletteColorIndex++)
            {
                palette.Entries[paletteColorIndex] = paletteHistogram[paletteColorIndex].ToNormalizedColor();
            }
            return palette;
        }
    }
}
