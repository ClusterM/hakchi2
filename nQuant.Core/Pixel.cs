namespace nQuant
{
    public struct Pixel
    {
        public Pixel(byte alpha, byte red, byte green, byte blue) : this()
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;

            Argb = alpha << 24 | red << 16 | green << 8 | blue;
        }

        public byte Alpha;
        public byte Red;
        public byte Green;
        public byte Blue;
        public int Argb;
    }
}