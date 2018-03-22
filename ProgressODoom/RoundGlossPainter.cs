using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	public enum GlossStyle {
		Top, Bottom, Both
	}

	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.RoundGlossPainter), "Icons.RoundGlossPainter.ico")]
	public class RoundGlossPainter : ChainedGlossPainter {
		private GlossStyle style = GlossStyle.Both;
		private int highAlpha = 240;
		private int lowAlpha = 0;
		private int fadewidth = 4;
		private Brush highBrush;
		private Brush lowBrush;
		private Rectangle box;
		private Color color = Color.White;
		private Color topColor;
		private Color botColor;

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the style for this progress gloss"), Browsable(true)]
		public GlossStyle Style {
			get { return this.style; }
			set {
				this.style = value;
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Blending"), Description("Gets or sets the high alpha value"), Browsable(true)]
		public int AlphaHigh {
			get { return this.highAlpha; }
			set {
				if (value < 0 || value > 255) {
					throw new ArgumentException("Alpha values must be between 0 and 255.");
				}
				this.highAlpha = value;
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Blending"), Description("Gets or sets the low alpha value"), Browsable(true)]
		public int AlphaLow {
			get { return this.lowAlpha; }
			set {
				if (value < 0 || value > 255) {
					throw new ArgumentException("Alpha values must be between 0 and 255.");
				}
				this.lowAlpha = value;
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Blending"), Description("Gets or sets the number of pixels to blend over"), Browsable(true)]
		public int TaperHeight {
			get { return this.fadewidth; }
			set {
				this.fadewidth = value;
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets color to gloss"), Browsable(true)]
		public Color Color {
			get { return this.color; }
			set {
				this.color = value;
				this.topColor = Color.FromArgb(highAlpha, this.color.R, this.color.G, this.color.B);
				this.botColor = Color.FromArgb(lowAlpha, this.color.R, this.color.G, this.color.B);
				box = new Rectangle(0, 0, 1, 1);
				FireChange();
			}
		}

		protected override void PaintThisGloss(Rectangle box, Graphics g) {
			if (!this.box.Equals(box)) {
				this.box = box;
				ResetBrushes(box);
			}

			//int midpoint = (int)((float)box.Height / 2f);
			//int toppoint = midpoint > (fadewidth + 2) ? midpoint - (fadewidth / 2) : midpoint;
			//int botpoint = midpoint > (fadewidth + 2) ? midpoint + (fadewidth / 2) : midpoint;

			//Rectangle topBox = new Rectangle(box.X, box.Y, box.Width - 1, box.Y + fadewidth);
			//Rectangle botBox = new Rectangle(box.X, box.Bottom - fadewidth - 2, box.Width - 1, fadewidth + 1);
			Rectangle topBox = new Rectangle(box.X, box.Y, box.Width, fadewidth);
			Rectangle botBox = new Rectangle(box.X, box.Bottom - fadewidth, box.Width, fadewidth);

			//if (midpoint - fadewidth > 2) { midpoint -= fadewidth; }

			switch (style) {
				case GlossStyle.Bottom:
					g.FillRectangle(lowBrush, botBox);
					break;
				case GlossStyle.Top:
					g.FillRectangle(highBrush, topBox);
					break;
				case GlossStyle.Both:
					g.FillRectangle(highBrush, topBox);
					g.FillRectangle(lowBrush, botBox);
					//g.DrawRectangle(Pens.Red, topBox);
					//g.DrawRectangle(Pens.Blue, botBox);
					break;
			}
			//g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64, 255, 0, 0))), topBox);
			//g.FillRectangle(new SolidBrush(Color.FromArgb(32, 255, 0, 0)), topBox);
			//g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64, 0, 0, 255))), botBox);
			//g.FillRectangle(new SolidBrush(Color.FromArgb(32, 0, 0, 255)), botBox);
		}

		protected override void ResizeThis(Rectangle box) {
			if (!this.box.Equals(box)) {
				this.box = box;
				ResetBrushes(box);
			}
		}

		private void ResetBrushes(Rectangle box) {
			//int midpoint = (int)((float)box.Height / 2f);
			//int toppoint = midpoint - (fadewidth / 2);
			//if (toppoint < box.Y + 2) { toppoint = box.Y + 2; }
			//int botpoint = midpoint + (fadewidth / 2);
			//if (botpoint > box.Height - 2) { botpoint = box.Height - 2; }

			//Point top = new Point(box.X, box.Y);
			//Point topmid = new Point(box.X, box.Y + fadewidth + 1);
			//Point botmid = new Point(box.X, box.Height - fadewidth - 1);
			//Point bot = new Point(box.X, box.Bottom);

			Rectangle topBox = new Rectangle(box.X, box.Y, box.Width, fadewidth);
			Rectangle botBox = new Rectangle(box.X, box.Bottom - fadewidth, box.Width, fadewidth);
			Point top = new Point(box.X, topBox.Top);
			Point topmid = new Point(box.X, topBox.Bottom);
			Point botmid = new Point(box.X, botBox.Top - 1);
			Point bot = new Point(box.X, botBox.Bottom);

			Color high = topColor;
			Color low = botColor;
			switch (style) {
				case GlossStyle.Top:
					highBrush = new LinearGradientBrush(top, topmid, high, low);
					break;
				case GlossStyle.Bottom:
					lowBrush = new LinearGradientBrush(botmid, bot, low, high);
					break;
				case GlossStyle.Both:
					highBrush = new LinearGradientBrush(top, topmid, high, low);
					lowBrush = new LinearGradientBrush(botmid, bot, low, high);
					break;
			}
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (highBrush != null) { highBrush.Dispose(); }
			if (lowBrush != null) { lowBrush.Dispose(); }
		}
	}
}