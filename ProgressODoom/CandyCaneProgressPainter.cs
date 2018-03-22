using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.CandyCaneProgressPainter), "Icons.CandyCaneProgressPainter.ico")]
	public class CandyCaneProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private Color baseColor;
		private Image img;
		private Rectangle box;

		/// <summary></summary>
		public CandyCaneProgressPainter() {
			baseColor = Color.FromArgb(049, 129, 222);
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the base progress color"), Browsable(true)]
		public Color Color {
			get { return baseColor; }
			set {
				baseColor = value;
				try { if (box != null) { RepaintImage(box); } } catch { }
				FireChange();
			}
		}

		private void RepaintImage(Rectangle box) {
			this.box = box;
			this.img = new Bitmap(box.Width, box.Height);
			// BuildTile() then resize it to fix the box.Height, then tile it.
			Bitmap source = BuildTile(this.baseColor);
			img = new Bitmap((int)(((float)box.Height * (float)source.Width) / (float)source.Height), box.Height + 1);
			using (Graphics g = Graphics.FromImage(img)) {
				g.DrawImage(source, 0, 0, img.Width, img.Height + 1);
			}

			source.Dispose();
		}
		private Point Offset(Point p, int x, int y) {
			return new Point(p.X + x, p.Y + y);
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			if (img == null) { RepaintImage(box); }
			if (box.Width <= 1) { return; }
			
			int x = box.Width - img.Width;
			while (x > (0 - img.Width)) {
				g.DrawImageUnscaled(img, x, 0);
				x -= img.Width;
			}

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		protected override void ResizeThis(Rectangle box) {
			this.box = box;
			RepaintImage(box);
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
			if (img != null) {
				img.Dispose();
			}
		}

		public Bitmap BuildTile(Color color) {
			HSV clr = new HSV(color);
			Bitmap src = GetSource();
			Bitmap bmp = new Bitmap(src.Width, src.Height);
			for (int x = 0; x < bmp.Width; x++) {
				for (int y = 0; y < bmp.Height; y++) {
					Color original = src.GetPixel(x, y);
					Color altered = Color.FromArgb(0, 255, 255, 255);
					HSV orighsv = new HSV(original);
					Color origrgb = orighsv.Color;
					origrgb = Color.FromArgb(original.A, origrgb.R, origrgb.G, origrgb.B);
					if (!origrgb.Equals(altered)) {
						orighsv.Hue = clr.Hue;
						//orighsv.Saturation = clr.Saturation;
						//orighsv.Value = clr.Value;
						altered = orighsv.Color;
						altered = Color.FromArgb(original.A, altered.R, altered.G, altered.B);
					}
					bmp.SetPixel(x, y, altered);
				}
			}
			src.Dispose();
			return bmp;
		}
		private Bitmap GetSource() {
			Bitmap bmp = new Bitmap(16, 9);
			Graphics g = Graphics.FromImage(bmp);
			g.Clear(Color.FromArgb(0, 255, 255, 255));

			bmp.SetPixel(0, 0, Color.FromArgb(77, 140, 177, 225));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(140, 177, 225))), new Point(1, 0), new Point(7, 0));
			bmp.SetPixel(8, 0, Color.FromArgb(77, 140, 177, 225));

			bmp.SetPixel(0, 1, Color.FromArgb(38, 99, 158, 222));
			bmp.SetPixel(1, 1, Color.FromArgb(128, 99, 158, 222));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(99, 158, 222))), new Point(2, 1), new Point(8, 1));
			bmp.SetPixel(9, 1, Color.FromArgb(64, 99, 158, 222));

			bmp.SetPixel(1, 2, Color.FromArgb(38, 94, 156, 222));
			bmp.SetPixel(2, 2, Color.FromArgb(205, 94, 156, 222));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(94, 156, 222))), new Point(3, 2), new Point(8, 2));
			bmp.SetPixel(9, 2, Color.FromArgb(192, 94, 156, 222));
			bmp.SetPixel(10, 2, Color.FromArgb(38, 94, 156, 222));

			bmp.SetPixel(2, 3, Color.FromArgb(77, 93, 158, 228));
			bmp.SetPixel(3, 3, Color.FromArgb(251, 93, 158, 228));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(93, 158, 228))), new Point(4, 3), new Point(9, 3));
			bmp.SetPixel(10, 3, Color.FromArgb(154, 93, 158, 228));
			bmp.SetPixel(11, 3, Color.FromArgb(26, 93, 158, 228));

			bmp.SetPixel(2, 4, Color.FromArgb(13, 49, 129, 222));
			bmp.SetPixel(3, 4, Color.FromArgb(51, 49, 129, 222));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(49, 129, 222))), new Point(4, 4), new Point(9, 4));
			bmp.SetPixel(10, 4, Color.FromArgb(251, 49, 129, 222));
			bmp.SetPixel(11, 4, Color.FromArgb(90, 49, 129, 222));

			bmp.SetPixel(3, 5, Color.FromArgb(64, 81, 159, 247));
			bmp.SetPixel(4, 5, Color.FromArgb(205, 81, 159, 247));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(81, 159, 247))), new Point(5, 5), new Point(10, 5));
			bmp.SetPixel(11, 5, Color.FromArgb(218, 81, 159, 247));
			bmp.SetPixel(12, 5, Color.FromArgb(77, 81, 159, 247));

			bmp.SetPixel(4, 6, Color.FromArgb(77, 110, 186, 255));
			bmp.SetPixel(5, 6, Color.FromArgb(243, 110, 186, 255));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(110, 186, 255))), new Point(6, 6), new Point(11, 6));
			bmp.SetPixel(12, 6, Color.FromArgb(154, 110, 186, 255));
			bmp.SetPixel(13, 6, Color.FromArgb(38, 110, 186, 255));

			bmp.SetPixel(4, 7, Color.FromArgb(26, 121, 201, 255));
			bmp.SetPixel(5, 7, Color.FromArgb(141, 121, 201, 255));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(121, 201, 255))), new Point(6, 7), new Point(12, 7));
			bmp.SetPixel(13, 7, Color.FromArgb(102, 121, 201, 255));
			bmp.SetPixel(14, 7, Color.FromArgb(26, 121, 201, 255));

			bmp.SetPixel(5, 8, Color.FromArgb(26, 135, 227, 255));
			bmp.SetPixel(6, 8, Color.FromArgb(192, 135, 227, 255));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(135, 227, 255))), new Point(7, 8), new Point(12, 8));
			bmp.SetPixel(13, 8, Color.FromArgb(243, 135, 227, 255));
			bmp.SetPixel(14, 8, Color.FromArgb(64, 135, 227, 255));
			bmp.SetPixel(15, 8, Color.FromArgb(13, 135, 227, 255));

			g.Dispose();
			return bmp;
		}
	}
}