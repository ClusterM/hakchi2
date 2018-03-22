using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.BarberPoleProgressPainter), "Icons.BarberPoleProgressPainter.ico")]
	public class BarberPoleProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private Color baseColor;
		private Color highlightColor;
		private Color stripeColor;
		private Color baseShadeColor;
		private Color highlightShadeColor;
		private Color stripeShadeColor;
		private int shadeHeight;
		private int stripeWidth;
		private Image img;
		private Rectangle box;

		/// <summary></summary>
		public BarberPoleProgressPainter() {
			baseColor = Color.FromArgb(226, 138, 078);
			highlightColor = Color.FromArgb(225, 132, 068);
			stripeColor = Color.FromArgb(222, 123, 055);
			baseShadeColor = Color.FromArgb(215, 097, 020);
			highlightShadeColor = Color.FromArgb(213, 087, 007);
			stripeShadeColor = Color.FromArgb(210, 078, 000);
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the base progress color"), Browsable(true)]
		public Color Color {
			get { return baseColor; }
			set {
				baseColor = value;
				HSV baseHsv = new HSV(baseColor);

				bool change = false;
				if (baseHsv.Saturation > 166) { baseHsv.Saturation = 166; change = true; }
				if (baseHsv.Value > 239) { baseHsv.Value = 239; change = true; }
				if (change) { baseColor = baseHsv.Color; }

				highlightColor = HSV.FromHsv(baseHsv.Hue, baseHsv.Saturation + 11, baseHsv.Value);
				stripeColor = HSV.FromHsv(baseHsv.Hue, baseHsv.Saturation + 25, baseHsv.Value - 4);

				HSV shade = new HSV(baseHsv.Hue, baseHsv.Saturation + 65, baseHsv.Value - 11);
				baseShadeColor = shade.Color;
				highlightShadeColor = HSV.FromHsv(shade.Hue, shade.Saturation + 15, shade.Value - 2);
				stripeShadeColor = HSV.FromHsv(shade.Hue, shade.Saturation + 24, shade.Value - 5);

				try { if (box != null) { RepaintImage(box); } } catch { }
				FireChange();
			}
		}

		private void RepaintImage(Rectangle box) {
			if (box.Width == 0 || box.Height == 0) { img = null; return; }
			img = new Bitmap(box.Width - (box.X * 2), box.Height - (box.Y * 2));
			Bitmap tile = new Bitmap(img.Height * 2, img.Height);

			shadeHeight = (int)((double)box.Height * 0.4D);
			stripeWidth = box.Height;

			using (Graphics g = Graphics.FromImage(tile)) {
				g.FillRectangle(new SolidBrush(baseColor), 0, 0, tile.Width, tile.Height);
				g.FillRectangle(new SolidBrush(baseShadeColor), 0, tile.Height - shadeHeight, tile.Width, tile.Height);

				Pen highlightPen = new Pen(new SolidBrush(highlightColor), 1f);
				Pen highlightShadePen = new Pen(new SolidBrush(highlightShadeColor), 1f);
				Pen stripePen = new Pen(new SolidBrush(stripeColor), 1f);
				Pen stripeShadePen = new Pen(new SolidBrush(stripeShadeColor), 1f);

				for (int y = 0; y < stripeWidth; y++) {
					if (y < tile.Height - shadeHeight) {
						g.DrawLine(highlightPen, stripeWidth - y - 1, y, (stripeWidth * 2) - y + 1, y);
						g.DrawLine(stripePen, stripeWidth - y, y, (stripeWidth * 2) - y, y);
					} else {
						g.DrawLine(highlightShadePen, stripeWidth - y - 1, y, (stripeWidth * 2) - y + 1, y);
						g.DrawLine(stripeShadePen, stripeWidth - y, y, (stripeWidth * 2) - y, y);
					}
				}
			}

			int x = box.X;
			using (Graphics i = Graphics.FromImage(img)) {
				while (true) {
					if (x > img.Width) { break; }
					i.DrawImageUnscaled(tile, x, box.Y);
					x += tile.Width;
				}
			}

			tile.Dispose();
		}
		private Point Offset(Point p, int x, int y) {
			return new Point(p.X + x, p.Y + y);
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			try {
				box.Width -= 1;
				box.Height -= 1;
			} catch { }
			if (box.Width <= 1) { return; }

			if (img == null) { RepaintImage(box); }
			Rectangle off = new Rectangle(box.Location, box.Size);
			off.Offset(box.Right - img.Width, 0);
			g.DrawImageUnscaled(img, off);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		protected override void ResizeThis(Rectangle box) {
			this.box = box;
			try {
				box.Width -= 1;
				box.Height -= 1;
			} catch {}
			shadeHeight = (int)((double)box.Height * 0.4D);
			stripeWidth = box.Height;
			RepaintImage(box);
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
			if (img != null) { img.Dispose(); }
		}
	}
}