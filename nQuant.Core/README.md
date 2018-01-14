nQuant.NET
======

nQuant is a .NET color quantizer producing high quality indexed PNG images using an algorithm optimized for the highest quality possible.

nQuant was originally developed as part of a larger effort called [RequestReduce](http://requestreduce.org/) which is an HTTP module that automatically minifies and merges CSS as well as sprites their background images on the fly.

The original repository may be found here: https://nquant.codeplex.com/
This fork was forked from Philippecp's [nQuantLeanerAndFaster](https://nquant.codeplex.com/SourceControl/network/forks/Philippecp/nQuantLeanerAndFaster) fork.

## This Fork
This fork was made to support creating 4-bit images, as the original only supports creating 8-bit images.
The public interface is kept completely compatible with the original, so the same code will do the same thing.

## Usage
Usage is the same as the standard nQuant library, with the only exception being a new optional parameter on `WuQuantizer.QuantizeImage`. It's modified to:
```csharp
Image QuantizeImage(Bitmap image, int alphaThreshold = 10, int alphaFader = 70, int maxColors = 256);
```

* image: An image in 32-bit ARGB format.
* alphaThreshold: All colors with an alpha value equal to or less than this will be considered fully transparent.
* alphaFader: Alpha values will be normalized to the nearest multiple of this value.
* maxColors: The maximum number of colors in the output image format.
  * 256: Return an 8-bit image containing 256 colors.
  * 16: Return a 4-bit image containing 16 colors.
  * 2: Return a 1-bit image containing 2 colors. (Note that the last color is transparent, so each pixel will either be fully transparent or not.)

## Changes
See [CHANGES.md](CHANGES.md) for a list of major changes.

## License
nQuant is issued under the Apache 2.0 license. See [LICENSE.md](LICENSE.md) for details.
