using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.BevelledGradientProgressPainter), "Icons.BevelledGradientProgressPainter.ico")]
	public class BevelledGradientProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private ColorRange min;
		private ColorRange max;

		/// <summary></summary>
		public BevelledGradientProgressPainter() {
			this.MinColor = Color.Cornsilk;
			this.MaxColor = Color.Gold;
		}

		/// <summary></summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public BevelledGradientProgressPainter(Color min, Color max) {
			this.MinColor = min;
			this.MaxColor = max;
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the left progress color"), Browsable(true)]
		public Color MinColor {
			get { return min.BaseColor; }
			set {
				min = new ColorRange(value);
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the right progress color"), Browsable(true)]
		public Color MaxColor {
			get { return max.BaseColor; }
			set {
				max = new ColorRange(value);
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			try {
				box.Height -= 1;
				box.Width -= 1;
			} catch { }

			if (box.Width < 2) { return; }

			Point left = new Point(box.X, box.Y);
			Point right = new Point(box.Right, box.Y);
			Brush bottomOuter = new System.Drawing.Drawing2D.LinearGradientBrush(left, right, min.Darker, max.Darker);
			Brush bottomInner = new System.Drawing.Drawing2D.LinearGradientBrush(left, right, min.Dark, max.Dark);
			Brush topInner = new System.Drawing.Drawing2D.LinearGradientBrush(left, right, min.Light, max.Light);
			Brush topOuter = new System.Drawing.Drawing2D.LinearGradientBrush(left, right, min.Lighter, max.Lighter);
			Brush fill = new System.Drawing.Drawing2D.LinearGradientBrush(left, right, min.BaseColor, max.BaseColor);

			// fill box
			g.FillRectangle(fill, box);

			using (Pen p = new Pen(topInner, 1)) {
				// inner top
				g.DrawLine(p, box.X + 1, box.Y + 1, box.Right - 1, box.Y + 1);
			}
			using (Pen p = new Pen(min.Light, 1)) {
				// inner left
				g.DrawLine(p, box.X + 1, box.Y + 1, box.X + 1, box.Bottom - 1);
			}

			using (Pen p = new Pen(topOuter, 1)) {
				// outer top
				g.DrawLine(p, box.X, box.Y, box.Right, box.Y);
			}
			using (Pen p = new Pen(min.Lighter, 1)) {
				// outer left
				g.DrawLine(p, box.X, box.Y, box.X, box.Bottom);
			}

			// draw border
			using (Pen p = new Pen(bottomInner, 1)) {
				// inner bottom
				g.DrawLine(p, box.X + 1, box.Bottom - 1, box.Right - 1, box.Bottom - 1);
			}
			using (Pen p = new Pen(max.Dark, 1)) {
				// inner right
				g.DrawLine(p, box.Right - 1, box.Y + 1, box.Right - 1, box.Bottom - 1);
			}

			using (Pen p = new Pen(bottomOuter, 1)) {
				// outer bottom
				g.DrawLine(p, box.X, box.Bottom, box.Right, box.Bottom);
			}
			using (Pen p = new Pen(max.Darker, 1)) {
				// outer right
				g.DrawLine(p, box.Right, box.Y, box.Right, box.Bottom);
			}

			bottomOuter.Dispose();
			bottomInner.Dispose();
			topInner.Dispose();
			topOuter.Dispose();
			fill.Dispose();

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}
	}
}