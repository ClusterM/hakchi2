using System.Drawing;

namespace nQuant
{
    public interface IWuQuantizer
    {
        Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader);
        Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader, int maxColors);
    }
}