using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.MetalProgressPainter), "Icons.MetalProgressPainter.ico")]
	public class MetalProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private Color progColor = Color.FromArgb(201, 202, 201);
		private Color bkColor = Color.FromArgb(240, 240, 240);

		#region Default Colors
		private Color backColor = Color.FromArgb(176, 177, 176);
		private Color borderColor = Color.FromArgb(69, 68, 69);
		private Color backtopColor = Color.FromArgb(160, 157, 160);
		private Color barColor12 = Color.FromArgb(193, 194, 193);
		private Color barColor3 = Color.FromArgb(201, 202, 201);
		private Color barColor8 = Color.FromArgb(226, 226, 226);
		private Color barBorderTopColor = Color.FromArgb(250, 250, 250);
		private Color barBorderBottomColor = Color.FromArgb(176, 173, 176);
		#endregion

		#region Pens & Brushes
		new private Pen border; // fix?
		private Pen backtop;
		private Brush back;

		private Pen bar12;
		private Pen bar8;

		private Pen barBorderTop;
		private Pen barBorderBottom;

		private Brush prog;
		#endregion

		/// <summary></summary>
		public MetalProgressPainter() {
			progColor = barColor3;
			back = new SolidBrush(bkColor); //backColor);

			border = new Pen(new SolidBrush(borderColor), 1f);
			backtop = new Pen(new SolidBrush(backtopColor), 1f);

			bar12 = new Pen(new SolidBrush(barColor12), 1f);
			bar8 = new Pen(new SolidBrush(barColor8), 1f);

			barBorderTop = new Pen(new SolidBrush(barBorderTopColor), 1f);
			barBorderBottom = new Pen(new SolidBrush(barBorderBottomColor), 1f);

			prog = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(0, 20), barColor12, barColor8);
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the base progress color"), Browsable(true)]
		public Color Color {
			get { return progColor; }
			set {
				progColor = value;
				FireChange();
			}
		}

		[Category("Appearance"), Description("Gets or sets the color that the highlights are blended with"), Browsable(true)]
		public Color Highlight {
			get { return backColor; }
			set {
				backColor = value;
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			try {
				box.Width -= 1;
				box.Height -= 1;
			} catch {}
			float x = box.X;
			float y = box.Y;
			float w = box.Right;
			float h = box.Bottom;
			if (w < 2) { return; }

			RebuildBrushes(box.Bottom - 1);

			//g.FillRectangle(prog, x + 1, y + 1, w - 2, h - 1);
			//
			//g.DrawRectangle(barBorderBottom, box.X, box.Y, box.Right, box.Height - 1);
			//g.DrawLine(barBorderTop, box.X, box.Y, box.Width + 2, box.Y);
			//g.DrawLine(barBorderTop, box.X, box.Y, box.X, box.Height + 2);

			g.FillRectangle(prog, box);

			g.DrawLine(barBorderTop, x, y, w, y); // top
			g.DrawLine(barBorderTop, x, y, x, h); // left
			g.DrawLine(barBorderBottom, x, h, w, h); // bottom
			g.DrawLine(barBorderBottom, w, h, w, y); // right

			//g.DrawRectangle(border, x + 2, y + 2, w - 3, h - 4);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		protected override void ResizeThis(Rectangle box) {
			//RebuildBrushes();
		}

		private void RebuildBrushes(int height) {
			Color top = Cross(barColor3, progColor, barColor12);
			Color bottom = Cross(barColor3, progColor, barColor8);
			bar12 = new Pen(new SolidBrush(top), 1f);
			bar8 = new Pen(new SolidBrush(bottom), 1f);
			barBorderTop = new Pen(new SolidBrush(Cross(barColor3, progColor, barBorderTopColor)), 1f);
			barBorderBottom = new Pen(new SolidBrush(Cross(barColor3, progColor, barBorderBottomColor)), 1f);
			int h = height;
			//if (h == 0) { h = 20; }
			prog = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 1), new Point(0, h + 2), top, bottom);

			backtop = new Pen(new SolidBrush(Cross(backColor, bkColor, backtopColor)), 1f);
			back = new SolidBrush(bkColor);
		}

		private Color Cross(Color colorX, Color colorY, Color colorX2) {
			int r = (int)(((float)colorY.R * (float)colorX2.R) / (float)colorX.R);
			int g = (int)(((float)colorY.G * (float)colorX2.G) / (float)colorX.G);
			int b = (int)(((float)colorY.B * (float)colorX2.B) / (float)colorX.B);
			if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
			if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
			if (b > 255) { b = 255; } else if (b < 0) { b = 0; }
			return Color.FromArgb(r, g, b);
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
			if (border != null) { border.Dispose(); }
			if (backtop != null) { backtop.Dispose(); }
			if (back != null) { back.Dispose(); }
			if (bar12 != null) { bar12.Dispose(); }
			if (bar8 != null) { bar8.Dispose(); }
			if (barBorderTop != null) { barBorderTop.Dispose(); }
			if (barBorderBottom != null) { barBorderBottom.Dispose(); }
			if (prog != null) { prog.Dispose(); }
		}
	}
}