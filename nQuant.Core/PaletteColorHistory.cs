using System.Drawing;

namespace nQuant
{
    struct PaletteColorHistory
    {
        public int Alpha;
        public int Red;
        public int Green;
        public int Blue;
        public int Sum;

        public Color ToNormalizedColor()
        {
            return (Sum != 0) ? Color.FromArgb((int)(Alpha / Sum), (int)(Red / Sum), (int)(Green / Sum), (int)(Blue / Sum)) : Color.Transparent;
        }

        public void AddPixel(Pixel pixel)
        {
            Alpha += pixel.Alpha;
            Red += pixel.Red;
            Green += pixel.Green;
            Blue += pixel.Blue;
            Sum++;
        }
    }
}
