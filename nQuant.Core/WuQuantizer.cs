using System;
using System.Collections.Generic;
using System.Drawing;

namespace nQuant
{
    public class WuQuantizer : WuQuantizerBase, IWuQuantizer
    {
        protected override QuantizedPalette GetQuantizedPalette(int colorCount, ColorData data, IEnumerable<Box> cubes, int alphaThreshold)
        {
            int imageSize = data.PixelsCount;
            LookupData lookups = BuildLookups(cubes, data);

            IList<int> quantizedPixels = data.QuantizedPixels;
            for (var index = 0; index < imageSize; ++index)
            {
                var indexParts = BitConverter.GetBytes(quantizedPixels[index]);
                quantizedPixels[index] = lookups.Tags[indexParts[Alpha], indexParts[Red], indexParts[Green], indexParts[Blue]];
            }

            var alphas = new int[colorCount + 1];
            var reds = new int[colorCount + 1];
            var greens = new int[colorCount + 1];
            var blues = new int[colorCount + 1];
            var sums = new int[colorCount + 1];
            var palette = new QuantizedPalette(imageSize);

            IList<Pixel> pixels = data.Pixels;
            int pixelsCount = data.PixelsCount;
            IList<Lookup> lookupsList = lookups.Lookups;
            int lookupsCount = lookupsList.Count;

            Dictionary<int, int> cachedMaches = new Dictionary<int, int>();

            for (int pixelIndex = 0; pixelIndex < pixelsCount; pixelIndex++)
            {
                Pixel pixel = pixels[pixelIndex];
                palette.PixelIndex[pixelIndex] = -1;
                if (pixel.Alpha <= alphaThreshold)
                    continue;

                int bestMatch;
                int argb = pixel.Argb;

                if (!cachedMaches.TryGetValue(argb, out bestMatch))
                {
                    int match = quantizedPixels[pixelIndex];
                    bestMatch = match;
                    int bestDistance = int.MaxValue;

                    for (int lookupIndex = 0; lookupIndex < lookupsCount; lookupIndex++)
                    {
                        Lookup lookup = lookupsList[lookupIndex];
                        var deltaAlpha = pixel.Alpha - lookup.Alpha;
                        var deltaRed = pixel.Red - lookup.Red;
                        var deltaGreen = pixel.Green - lookup.Green;
                        var deltaBlue = pixel.Blue - lookup.Blue;

                        int distance = deltaAlpha*deltaAlpha + deltaRed*deltaRed + deltaGreen*deltaGreen + deltaBlue*deltaBlue;

                        if (distance >= bestDistance)
                            continue;

                        bestDistance = distance;
                        bestMatch = lookupIndex;
                    }

                    cachedMaches[argb] = bestMatch;
                }

                alphas[bestMatch] += pixel.Alpha;
                reds[bestMatch] += pixel.Red;
                greens[bestMatch] += pixel.Green;
                blues[bestMatch] += pixel.Blue;
                sums[bestMatch]++;

                palette.PixelIndex[pixelIndex] = bestMatch;
            }

            for (var paletteIndex = 0; paletteIndex < colorCount; paletteIndex++)
            {
                if (sums[paletteIndex] > 0)
                {
                    alphas[paletteIndex] /= sums[paletteIndex];
                    reds[paletteIndex] /= sums[paletteIndex];
                    greens[paletteIndex] /= sums[paletteIndex];
                    blues[paletteIndex] /= sums[paletteIndex];
                }

                var color = Color.FromArgb(alphas[paletteIndex], reds[paletteIndex], greens[paletteIndex], blues[paletteIndex]);
                palette.Colors.Add(color);
            }

            palette.Colors.Add(Color.FromArgb(0, 0, 0, 0));

            return palette;
        }
    }
}
