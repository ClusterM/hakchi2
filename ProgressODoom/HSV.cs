using System;
using System.Drawing;

#pragma warning disable 0660, 0661
namespace ProgressODoom {
    public struct HSV {
		private int hue;
		private int sat;
		private int val;

		public HSV(int h, int s, int v) {
			hue = h;
			sat = s;
			val = v;
		}

		public HSV(Color color) {
			hue = 0;
			sat = 0;
			val = 0;
			FromRGB(color);
		}

		public static Color FromHsv(int h, int s, int v) {
			HSV hsv = new HSV(h, s, v);
			return hsv.Color;
		}

		public int Hue {
			get { return hue; }
			set { hue = value; }
		}

		public int Saturation {
			get { return sat; }
			set { sat = value; }
		}

		public int Value {
			get { return val; }
			set { val = value; }
		}

		public Color Color {
			get { return ToRGB(); }
			set { FromRGB(value); }
		}

		private void FromRGB(Color color) {
			/*
			if (max = min)
				h = 0
			if (max = r)
				h = (60deg * (g-b)/(max-min) + 0deg) % 360deg
			if (max = g)
				h = (60deg * (b-r)/(max-min) + 120deg)
			if (max = b)
				h = (60deg * (r-g)/(max-min) + 240deg)
			
			if (max = 0)
				s = 0
			else
				s = 1 - min/max
			
			v = max
			*/

			double min;
			double max;
			double delta;

			double r = (double)color.R / 255D;
			double g = (double)color.G / 255D;
			double b = (double)color.B / 255D;

			double h;
			double s;
			double v;

			min = Math.Min(Math.Min(r, g), b);
			max = Math.Max(Math.Max(r, g), b);
			v = max;
			delta = max - min;
			if (max == 0 || delta == 0) {
				s = 0;
				h = 0;
			} else {
				s = delta / max;
				if (r == max) {
					h = (60D * ((g - b) / delta)) % 360D;
				} else if (g == max) {
					h = 60D * ((b - r) / delta) + 120D;
				} else {
					h = 60D * ((r - g) / delta) + 240D;
				}
			}
			if (h < 0) {
				h += 360D;
			}

			Hue = (int)(h / 360D * 255D);
			Saturation = (int)(s * 255D);
			Value = (int)(v * 255D);
		}
		private Color ToRGB() {
			double h;
			double s;
			double v;

			double r = 0;
			double g = 0;
			double b = 0;

			// Scale Hue to be between 0 and 360. Saturation
			// and value scale to be between 0 and 1.
			h = ((double)Hue / 255D * 360D) % 360D;
			s = (double)Saturation / 255D;
			v = (double)Value / 255D;

			if (s == 0) {
				r = v;
				g = v;
				b = v;
			} else {
				double p;
				double q;
				double t;

				double fractionalSector;
				int sectorNumber;
				double sectorPos;

				sectorPos = h / 60D;
				sectorNumber = (int)(Math.Floor(sectorPos));

				fractionalSector = sectorPos - sectorNumber;

				p = v * (1D - s);
				q = v * (1D - (s * fractionalSector));
				t = v * (1D - (s * (1D - fractionalSector)));

				switch (sectorNumber) {
					case 0:
						r = v;
						g = t;
						b = p;
						break;
					case 1:
						r = q;
						g = v;
						b = p;
						break;
					case 2:
						r = p;
						g = v;
						b = t;
						break;
					case 3:
						r = p;
						g = q;
						b = v;
						break;
					case 4:
						r = t;
						g = p;
						b = v;
						break;
					case 5:
						r = v;
						g = p;
						b = q;
						break;
				}
			}
			return Color.FromArgb((int)(r * 255D), (int)(g * 255D), (int)(b * 255D));
		}

		public static bool operator !=(HSV left, HSV right) {
			return !(left == right);
		}
		public static bool operator ==(HSV left, HSV right) {
			return (left.Hue == right.Hue && left.Value == right.Value && left.Saturation == right.Saturation);
		}
		public override string ToString() {
			string s = string.Format("HSV({0:f2}, {1:f2}, {2:f2})", Hue, Saturation, Value);
			return s;
		}
	}
}
#pragma warning restore
