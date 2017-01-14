using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace nQuant
{
    public abstract class WuQuantizerBase
    {
        private const int MaxColor = 256;
        protected const int Alpha = 3;
        protected const int Red = 2;
        protected const int Green = 1;
        protected const int Blue = 0;
        private const int SideSize = 33;
        private const int MaxSideIndex = 32;

        public Image QuantizeImage(Bitmap image)
        {
            return QuantizeImage(image, 10, 70);
        }

        public Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader)
        {
            var colorCount = MaxColor;
            var data = BuildHistogram(image, alphaThreshold, alphaFader);
            data = CalculateMoments(data);
            var cubes = SplitData(ref colorCount, data);
            var palette = GetQuantizedPalette(colorCount, data, cubes, alphaThreshold);
            return ProcessImagePixels(image, palette);
        }

        private static Bitmap ProcessImagePixels(Image sourceImage, QuantizedPalette palette)
        {
            var result = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format8bppIndexed);
            var newPalette = result.Palette;
            for (var index = 0; index < palette.Colors.Count; index++)
                newPalette.Entries[index] = palette.Colors[index];
            result.Palette = newPalette;

            BitmapData targetData = null;
            try
            {
                targetData = result.LockBits(Rectangle.FromLTRB(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, result.PixelFormat);
                const byte targetBitDepth = 8;
                var targetByteLength = targetData.Stride < 0 ? -targetData.Stride : targetData.Stride;
                var targetByteCount = Math.Max(1, targetBitDepth >> 3);
                var targetSize = targetByteLength * result.Height;
                var targetOffset = 0;
                var targetBuffer = new byte[targetSize];
                var targetValue = new byte[targetByteCount];
                var pixelIndex = 0;

                for (var y = 0; y < result.Height; y++)
                {
                    var targetIndex = 0;
                    for (var x = 0; x < result.Width; x++)
                    {
                        var targetIndexOffset = targetIndex >> 3;
                        targetValue[0] = (byte)(palette.PixelIndex[pixelIndex] == -1 ? palette.Colors.Count - 1 : palette.PixelIndex[pixelIndex]);
                        pixelIndex++;

                        for (var valueIndex = 0; valueIndex < targetByteCount; valueIndex++)
                            targetBuffer[targetOffset + valueIndex + targetIndexOffset] = targetValue[valueIndex];

                        targetIndex += targetBitDepth;
                    }

                    targetOffset += targetByteLength;
                }

                Marshal.Copy(targetBuffer, 0, targetData.Scan0, targetSize);
            }
            finally
            {
                if(targetData != null)
                    result.UnlockBits(targetData);
            }

            return result;
        }

        private static ColorData BuildHistogram(Bitmap sourceImage, int alphaThreshold, int alphaFader)
        {
            int bitmapWidth = sourceImage.Width;
            int bitmapHeight = sourceImage.Height;

            BitmapData data = sourceImage.LockBits(
                Rectangle.FromLTRB(0, 0, bitmapWidth, bitmapHeight),
                ImageLockMode.ReadOnly, 
                sourceImage.PixelFormat);
            ColorData colorData = new ColorData(MaxSideIndex, bitmapWidth, bitmapHeight);

            try
            {
                var bitDepth = Image.GetPixelFormatSize(sourceImage.PixelFormat);
                if (bitDepth != 32)
                    throw new QuantizationException(string.Format("Thie image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {0} with {1} colors.", bitDepth, sourceImage.Palette.Entries.Length));
                var byteLength = data.Stride < 0 ? -data.Stride : data.Stride;
                var byteCount = Math.Max(1, bitDepth >> 3);
                var offset = 0;
                var buffer = new Byte[byteLength * sourceImage.Height];
                var value = new Byte[byteCount];

                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
               
                for (int y = 0; y < bitmapHeight; y++)
                {
                    var index = 0;
                    for (int x = 0; x < bitmapWidth; x++)
                    {
                        var indexOffset = index >> 3;

                        for (var valueIndex = 0; valueIndex < byteCount; valueIndex++)
                            value[valueIndex] = buffer[offset + valueIndex + indexOffset];

                        var indexAlpha = (byte)((value[Alpha] >> 3) + 1);
                        var indexRed = (byte)((value[Red] >> 3) + 1);
                        var indexGreen = (byte)((value[Green] >> 3) + 1);
                        var indexBlue = (byte)((value[Blue] >> 3) + 1);

                        if (value[Alpha] > alphaThreshold)
                        {
                            if (value[Alpha] < 255)
                            {
                                var alpha = value[Alpha] + (value[Alpha] % alphaFader);
                                value[Alpha] = (byte)(alpha > 255 ? 255 : alpha);
                                indexAlpha = (byte)((value[Alpha] >> 3) + 1);
                            }

                            colorData.Weights[indexAlpha, indexRed, indexGreen, indexBlue]++;
                            colorData.MomentsRed[indexAlpha, indexRed, indexGreen, indexBlue] += value[Red];
                            colorData.MomentsGreen[indexAlpha, indexRed, indexGreen, indexBlue] += value[Green];
                            colorData.MomentsBlue[indexAlpha, indexRed, indexGreen, indexBlue] += value[Blue];
                            colorData.MomentsAlpha[indexAlpha, indexRed, indexGreen, indexBlue] += value[Alpha];
                            colorData.Moments[indexAlpha, indexRed, indexGreen, indexBlue] += (value[Alpha]*value[Alpha]) +
                                                                                              (value[Red]*value[Red]) +
                                                                                              (value[Green]*value[Green]) +
                                                                                              (value[Blue]*value[Blue]);
                        }

                        colorData.AddPixel(
                            new Pixel(value[Alpha], value[Red], value[Green], value[Blue]),
                            BitConverter.ToInt32 (new[] { indexAlpha, indexRed, indexGreen, indexBlue }, 0));
                        index += bitDepth;
                    }

                    offset += byteLength;
                }
            }
            finally
            {
                sourceImage.UnlockBits(data);
            }
            return colorData;
        }

        private static ColorData CalculateMoments(ColorData data)
        {
            for (var alphaIndex = 1; alphaIndex <= MaxSideIndex; ++alphaIndex)
            {
                var xarea = new long[SideSize, SideSize, SideSize];
                var xareaAlpha = new long[SideSize, SideSize, SideSize];
                var xareaRed = new long[SideSize, SideSize, SideSize];
                var xareaGreen = new long[SideSize, SideSize, SideSize];
                var xareaBlue = new long[SideSize, SideSize, SideSize];
                var xarea2 = new float[SideSize, SideSize, SideSize];
                for (var redIndex = 1; redIndex <= MaxSideIndex; ++redIndex)
                {
                    var area = new long[SideSize];
                    var areaAlpha = new long[SideSize];
                    var areaRed = new long[SideSize];
                    var areaGreen = new long[SideSize];
                    var areaBlue = new long[SideSize];
                    var area2 = new float[SideSize];
                    for (var greenIndex = 1; greenIndex <= MaxSideIndex; ++greenIndex)
                    {
                        long line = 0;
                        long lineAlpha = 0;
                        long lineRed = 0;
                        long lineGreen = 0;
                        long lineBlue = 0;
                        var line2 = 0.0f;
                        for (var blueIndex = 1; blueIndex <= MaxSideIndex; ++blueIndex)
                        {
                            line += data.Weights[alphaIndex, redIndex, greenIndex, blueIndex];
                            lineAlpha += data.MomentsAlpha[alphaIndex, redIndex, greenIndex, blueIndex];
                            lineRed += data.MomentsRed[alphaIndex, redIndex, greenIndex, blueIndex];
                            lineGreen += data.MomentsGreen[alphaIndex, redIndex, greenIndex, blueIndex];
                            lineBlue += data.MomentsBlue[alphaIndex, redIndex, greenIndex, blueIndex];
                            line2 += data.Moments[alphaIndex, redIndex, greenIndex, blueIndex];

                            area[blueIndex] += line;
                            areaAlpha[blueIndex] += lineAlpha;
                            areaRed[blueIndex] += lineRed;
                            areaGreen[blueIndex] += lineGreen;
                            areaBlue[blueIndex] += lineBlue;
                            area2[blueIndex] += line2;

                            xarea[redIndex, greenIndex, blueIndex] = xarea[redIndex - 1, greenIndex, blueIndex] + area[blueIndex];
                            xareaAlpha[redIndex, greenIndex, blueIndex] = xareaAlpha[redIndex - 1, greenIndex, blueIndex] + areaAlpha[blueIndex];
                            xareaRed[redIndex, greenIndex, blueIndex] = xareaRed[redIndex - 1, greenIndex, blueIndex] + areaRed[blueIndex];
                            xareaGreen[redIndex, greenIndex, blueIndex] = xareaGreen[redIndex - 1, greenIndex, blueIndex] + areaGreen[blueIndex];
                            xareaBlue[redIndex, greenIndex, blueIndex] = xareaBlue[redIndex - 1, greenIndex, blueIndex] + areaBlue[blueIndex];
                            xarea2[redIndex, greenIndex, blueIndex] = xarea2[redIndex - 1, greenIndex, blueIndex] + area2[blueIndex];

                            data.Weights[alphaIndex, redIndex, greenIndex, blueIndex] = data.Weights[alphaIndex - 1, redIndex, greenIndex, blueIndex] + xarea[redIndex, greenIndex, blueIndex];
                            data.MomentsAlpha[alphaIndex, redIndex, greenIndex, blueIndex] = data.MomentsAlpha[alphaIndex - 1, redIndex, greenIndex, blueIndex] + xareaAlpha[redIndex, greenIndex, blueIndex];
                            data.MomentsRed[alphaIndex, redIndex, greenIndex, blueIndex] = data.MomentsRed[alphaIndex - 1, redIndex, greenIndex, blueIndex] + xareaRed[redIndex, greenIndex, blueIndex];
                            data.MomentsGreen[alphaIndex, redIndex, greenIndex, blueIndex] = data.MomentsGreen[alphaIndex - 1, redIndex, greenIndex, blueIndex] + xareaGreen[redIndex, greenIndex, blueIndex];
                            data.MomentsBlue[alphaIndex, redIndex, greenIndex, blueIndex] = data.MomentsBlue[alphaIndex - 1, redIndex, greenIndex, blueIndex] + xareaBlue[redIndex, greenIndex, blueIndex];
                            data.Moments[alphaIndex, redIndex, greenIndex, blueIndex] = data.Moments[alphaIndex - 1, redIndex, greenIndex, blueIndex] + xarea2[redIndex, greenIndex, blueIndex];
                        }
                    }
                }
            }
            return data;
        }

        private static long Top(Box cube, int direction, int position, long[,,,] moment)
        {
            switch (direction)
            {
                case Alpha:
                    return (moment[position, cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum] -
                            moment[position, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] -
                            moment[position, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] +
                            moment[position, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum]) -
                           (moment[position, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] -
                            moment[position, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] -
                            moment[position, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] +
                            moment[position, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);

                case Red:
                    return (moment[cube.AlphaMaximum, position, cube.GreenMaximum, cube.BlueMaximum] -
                            moment[cube.AlphaMaximum, position, cube.GreenMinimum, cube.BlueMaximum] -
                            moment[cube.AlphaMinimum, position, cube.GreenMaximum, cube.BlueMaximum] +
                            moment[cube.AlphaMinimum, position, cube.GreenMinimum, cube.BlueMaximum]) -
                           (moment[cube.AlphaMaximum, position, cube.GreenMaximum, cube.BlueMinimum] -
                            moment[cube.AlphaMaximum, position, cube.GreenMinimum, cube.BlueMinimum] -
                            moment[cube.AlphaMinimum, position, cube.GreenMaximum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, position, cube.GreenMinimum, cube.BlueMinimum]);

                case Green:
                    return (moment[cube.AlphaMaximum, cube.RedMaximum, position, cube.BlueMaximum] -
                            moment[cube.AlphaMaximum, cube.RedMinimum, position, cube.BlueMaximum] -
                            moment[cube.AlphaMinimum, cube.RedMaximum, position, cube.BlueMaximum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, position, cube.BlueMaximum]) -
                           (moment[cube.AlphaMaximum, cube.RedMaximum, position, cube.BlueMinimum] -
                            moment[cube.AlphaMaximum, cube.RedMinimum, position, cube.BlueMinimum] -
                            moment[cube.AlphaMinimum, cube.RedMaximum, position, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, position, cube.BlueMinimum]);

                case Blue:
                    return (moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMaximum, position] -
                            moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, position] -
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, position] +
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, position]) -
                           (moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, position] -
                            moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, position] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, position] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, position]);

                default:
                    return 0;
            }
        }

        private static long Bottom(Box cube, int direction, long[,,,] moment)
        {
            switch (direction)
            {
                case Alpha:
                    return (-moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum] +
                            moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum]) -
                           (-moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);

                case Red:
                    return (-moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] +
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum]) -
                           (-moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] +
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);

                case Green:
                    return (-moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] +
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum] +
                            moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum]) -
                           (-moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] +
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);

                case Blue:
                    return (-moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] +
                            moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] +
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] -
                            moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]) -
                           (-moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] +
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] -
                            moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);

                default:
                    return 0;
            }
        }

        private static CubeCut Maximize(ColorData data, Box cube, int direction, byte first, byte last, long wholeAlpha, long wholeRed, long wholeGreen, long wholeBlue, long wholeWeight)
        {
            var bottomAlpha = Bottom(cube, direction, data.MomentsAlpha);
            var bottomRed = Bottom(cube, direction, data.MomentsRed);
            var bottomGreen = Bottom(cube, direction, data.MomentsGreen);
            var bottomBlue = Bottom(cube, direction, data.MomentsBlue);
            var bottomWeight = Bottom(cube, direction, data.Weights);

            var result = 0.0f;
            byte? cutPoint = null;

            for (var position = first; position < last; ++position)
            {
                var halfAlpha = bottomAlpha + Top(cube, direction, position, data.MomentsAlpha);
                var halfRed = bottomRed + Top(cube, direction, position, data.MomentsRed);
                var halfGreen = bottomGreen + Top(cube, direction, position, data.MomentsGreen);
                var halfBlue = bottomBlue + Top(cube, direction, position, data.MomentsBlue);
                var halfWeight = bottomWeight + Top(cube, direction, position, data.Weights);

                if (halfWeight == 0) continue;

                var halfDistance = halfAlpha * halfAlpha + halfRed * halfRed + halfGreen * halfGreen + halfBlue * halfBlue;
                var temp = halfDistance / halfWeight;

                halfAlpha = wholeAlpha - halfAlpha;
                halfRed = wholeRed - halfRed;
                halfGreen = wholeGreen - halfGreen;
                halfBlue = wholeBlue - halfBlue;
                halfWeight = wholeWeight - halfWeight;

                if (halfWeight != 0)
                {
                    halfDistance = halfAlpha * halfAlpha + halfRed * halfRed + halfGreen * halfGreen + halfBlue * halfBlue;
                    temp += halfDistance / halfWeight;

                    if (temp > result)
                    {
                        result = temp;
                        cutPoint = position;
                    }
                }
            }

            return new CubeCut(cutPoint, result);
        }

        private bool Cut(ColorData data, ref Box first,ref Box second)
        {
            int direction;
            var wholeAlpha = Volume(first, data.MomentsAlpha);
            var wholeRed = Volume(first, data.MomentsRed);
            var wholeGreen = Volume(first, data.MomentsGreen);
            var wholeBlue = Volume(first, data.MomentsBlue);
            var wholeWeight = Volume(first, data.Weights);

            var maxAlpha = Maximize(data, first, Alpha, (byte) (first.AlphaMinimum + 1), first.AlphaMaximum, wholeAlpha, wholeRed, wholeGreen, wholeBlue, wholeWeight);
            var maxRed = Maximize(data, first, Red, (byte) (first.RedMinimum + 1), first.RedMaximum, wholeAlpha, wholeRed, wholeGreen, wholeBlue, wholeWeight);
            var maxGreen = Maximize(data, first, Green, (byte) (first.GreenMinimum + 1), first.GreenMaximum, wholeAlpha, wholeRed, wholeGreen, wholeBlue, wholeWeight);
            var maxBlue = Maximize(data, first, Blue, (byte) (first.BlueMinimum + 1), first.BlueMaximum, wholeAlpha, wholeRed, wholeGreen, wholeBlue, wholeWeight);

            if ((maxAlpha.Value >= maxRed.Value) && (maxAlpha.Value >= maxGreen.Value) && (maxAlpha.Value >= maxBlue.Value))
            {
                direction = Alpha;
                if (maxAlpha.Position == null) return false;
            }
            else if ((maxRed.Value >= maxAlpha.Value) && (maxRed.Value >= maxGreen.Value) && (maxRed.Value >= maxBlue.Value))
                direction = Red;
            else
            {
                if ((maxGreen.Value >= maxAlpha.Value) && (maxGreen.Value >= maxRed.Value) && (maxGreen.Value >= maxBlue.Value))
                    direction = Green;
                else
                    direction = Blue;
            }

            second.AlphaMaximum = first.AlphaMaximum;
            second.RedMaximum = first.RedMaximum;
            second.GreenMaximum = first.GreenMaximum;
            second.BlueMaximum = first.BlueMaximum;

            switch (direction)
            {
                case Alpha:
                    second.AlphaMinimum = first.AlphaMaximum = (byte) maxAlpha.Position;
                    second.RedMinimum = first.RedMinimum;
                    second.GreenMinimum = first.GreenMinimum;
                    second.BlueMinimum = first.BlueMinimum;
                    break;

                case Red:
                    second.RedMinimum = first.RedMaximum = (byte) maxRed.Position;
                    second.AlphaMinimum = first.AlphaMinimum;
                    second.GreenMinimum = first.GreenMinimum;
                    second.BlueMinimum = first.BlueMinimum;
                    break;

                case Green:
                    second.GreenMinimum = first.GreenMaximum = (byte) maxGreen.Position;
                    second.AlphaMinimum = first.AlphaMinimum;
                    second.RedMinimum = first.RedMinimum;
                    second.BlueMinimum = first.BlueMinimum;
                    break;

                case Blue:
                    second.BlueMinimum = first.BlueMaximum = (byte) maxBlue.Position;
                    second.AlphaMinimum = first.AlphaMinimum;
                    second.RedMinimum = first.RedMinimum;
                    second.GreenMinimum = first.GreenMinimum;
                    break;
            }

            first.Size = (first.AlphaMaximum - first.AlphaMinimum) * (first.RedMaximum - first.RedMinimum) * (first.GreenMaximum - first.GreenMinimum) * (first.BlueMaximum - first.BlueMinimum);
            second.Size = (second.AlphaMaximum - second.AlphaMinimum) * (second.RedMaximum - second.RedMinimum) * (second.GreenMaximum - second.GreenMinimum) * (second.BlueMaximum - second.BlueMinimum);

            return true;
        }

        private static float CalculateVariance(ColorData data, Box cube)
        {
            float volumeAlpha = Volume(cube, data.MomentsAlpha);
            float volumeRed = Volume(cube, data.MomentsRed);
            float volumeGreen = Volume(cube, data.MomentsGreen);
            float volumeBlue = Volume(cube, data.MomentsBlue);
            float volumeMoment = VolumeFloat(cube, data.Moments);
            float volumeWeight = Volume(cube, data.Weights);

            float distance = volumeAlpha * volumeAlpha + volumeRed * volumeRed + volumeGreen * volumeGreen + volumeBlue * volumeBlue;

            var result = volumeMoment - distance / volumeWeight;
            return double.IsNaN(result) ? 0.0f : result;
        }

        private static long Volume(Box cube, long[,,,] moment)
        {
            return (moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum] -
                    moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] -
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] +
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum] -
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum] +
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] +
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] -
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum]) -

                   (moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] -
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] -
                    moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] +
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] -
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] +
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] +
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum] -
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);
        }

        private static float VolumeFloat(Box cube, float[,,,] moment)
        {
            return (moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum] -
                    moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] -
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] +
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum] -
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum] +
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum] +
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum] -
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum]) -

                   (moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] -
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum] -
                    moment[cube.AlphaMaximum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] +
                    moment[cube.AlphaMinimum, cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum] -
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] +
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum] +
                    moment[cube.AlphaMaximum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum] -
                    moment[cube.AlphaMinimum, cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum]);
        }

        private IEnumerable<Box> SplitData(ref int colorCount, ColorData data)
        {
            --colorCount;
            var next = 0;
            var volumeVariance = new float[MaxColor];
            var cubes = new Box[MaxColor];
            cubes[0].AlphaMaximum = MaxSideIndex;
            cubes[0].RedMaximum = MaxSideIndex;
            cubes[0].GreenMaximum = MaxSideIndex;
            cubes[0].BlueMaximum = MaxSideIndex;
            for (var cubeIndex = 1; cubeIndex < colorCount; ++cubeIndex)
            {
                if (Cut(data, ref cubes[next], ref cubes[cubeIndex]))
                {
                    volumeVariance[next] = cubes[next].Size > 1 ? CalculateVariance(data, cubes[next]) : 0.0f;
                    volumeVariance[cubeIndex] = cubes[cubeIndex].Size > 1 ? CalculateVariance(data, cubes[cubeIndex]) : 0.0f;
                }
                else
                {
                    volumeVariance[next] = 0.0f;
                    cubeIndex--;
                }

                next = 0;
                var temp = volumeVariance[0];

                for (var index = 1; index <= cubeIndex; ++index)
                {
                    if (volumeVariance[index] <= temp) continue;
                    temp = volumeVariance[index];
                    next = index;
                }

                if (temp > 0.0) continue;
                colorCount = cubeIndex + 1;
                break;
            }
            return cubes.Take(colorCount).ToList();
        }

        protected LookupData BuildLookups(IEnumerable<Box> cubes, ColorData data)
        {
            LookupData lookups = new LookupData(SideSize);
            int lookupsCount = lookups.Lookups.Count;

            foreach (var cube in cubes)
            {
                for (var alphaIndex = (byte)(cube.AlphaMinimum + 1); alphaIndex <= cube.AlphaMaximum; ++alphaIndex)
                {
                    for (var redIndex = (byte)(cube.RedMinimum + 1); redIndex <= cube.RedMaximum; ++redIndex)
                    {
                        for (var greenIndex = (byte)(cube.GreenMinimum + 1); greenIndex <= cube.GreenMaximum; ++greenIndex)
                        {
                            for (var blueIndex = (byte)(cube.BlueMinimum + 1); blueIndex <= cube.BlueMaximum; ++blueIndex)
                            {
                                lookups.Tags[alphaIndex, redIndex, greenIndex, blueIndex] = lookupsCount;
                            }
                        }
                    }
                }

                var weight = Volume(cube, data.Weights);

                if (weight <= 0) continue;

                var lookup = new Lookup
                    {
                        Alpha = (int) (Volume(cube, data.MomentsAlpha)/weight),
                        Red = (int) (Volume(cube, data.MomentsRed)/weight),
                        Green = (int) (Volume(cube, data.MomentsGreen)/weight),
                        Blue = (int) (Volume(cube, data.MomentsBlue)/weight)
                    };
                lookups.Lookups.Add(lookup);
            }
            return lookups;
        }

        protected abstract QuantizedPalette GetQuantizedPalette(int colorCount, ColorData data, IEnumerable<Box> cubes, int alphaThreshold);
    }
}