using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.MiddleGlossPainter), "Icons.MiddleGlossPainter.ico")]
	public class MiddleGlossPainter : ChainedGlossPainter {
		private GlossStyle style = GlossStyle.Both;
		private int highAlpha = 240;
		private int lowAlpha = 0;
		private int fadewidth = 4;
		private Brush highBrush;
		private Brush lowBrush;
		private Brush bothBrush;
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
				this.box = new Rectangle(0, 0, 1, 1);
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
				this.box = new Rectangle(0, 0, 1, 1);
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
				this.box = new Rectangle(0, 0, 1, 1);
				FireChange();
			}
		}
		/// <summary></summary>
		[Category("Blending"), Description("Gets or sets the number of pixels to blend over"), Browsable(true)]
		public int TaperHeight {
			get { return this.fadewidth; }
			set {
				this.fadewidth = value;
				this.box = new Rectangle(0, 0, 1, 1);
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

			int midpoint = box.X + (int)((float)box.Height / 2f);
			Rectangle topBox = new Rectangle(box.X, midpoint - fadewidth, box.Width - 1, fadewidth);
			Rectangle botBox = new Rectangle(box.X, midpoint, box.Width - 1, fadewidth);
			Rectangle fullBox = new Rectangle(box.X, midpoint - fadewidth, box.Width - 1, fadewidth * 2);

			switch (style) {
				case GlossStyle.Bottom:
					g.FillRectangle(lowBrush, botBox);
					//g.DrawRectangle(Pens.Fuchsia, botBox);
					break;
				case GlossStyle.Top:
					g.FillRectangle(highBrush, topBox);
					//g.DrawRectangle(Pens.Fuchsia, topBox);
					break;
				case GlossStyle.Both:
					//g.FillRectangle(highBrush, topBox);
					//g.FillRectangle(lowBrush, botBox);
					g.FillRectangle(bothBrush, fullBox);
					break;
			}
			//g.DrawRectangle(Pens.Purple, fullBox);
			//g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64, 255, 255, 0))), topBox);
			//g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64, 0, 255, 0))), botBox);
		}

		protected override void ResizeThis(Rectangle box) {
			if (!this.box.Equals(box)) {
				this.box = box;
				ResetBrushes(box);
			}
		}

		private void ResetBrushes(Rectangle box) {
			int midpoint = box.X + (int)((float)box.Height / 2f);
			Rectangle topBox = new Rectangle(box.X, midpoint - fadewidth, box.Width - 1, fadewidth);
			Rectangle botBox = new Rectangle(box.X, midpoint, box.Width - 1, fadewidth);
			Rectangle fullBox = new Rectangle(box.X, midpoint - fadewidth, box.Width - 1, fadewidth * 2);

			//int midpoint = box.X + (int)((float)box.Height / 2f);
			Point top = new Point(box.X, fullBox.Top);
			Point topmid = new Point(box.X, topBox.Bottom);
			Point botmid = new Point(box.X, botBox.Top);
			Point bot = new Point(box.X, botBox.Bottom);
			Color high = topColor;
			Color low = botColor;
			//Rectangle fullBox = new Rectangle(box.X, midpoint - fadewidth, box.Width - 1, fadewidth * 2);
			switch (style) {
				case GlossStyle.Top:
					highBrush = new LinearGradientBrush(top, topmid, low, high);
					break;
				case GlossStyle.Bottom:
					lowBrush = new LinearGradientBrush(botmid, bot, high, low);
					break;
				case GlossStyle.Both:
					//highBrush = new LinearGradientBrush(top, topmid, low, high);
					//lowBrush = new LinearGradientBrush(botmid, bot, high, low);
					bothBrush = new LinearGradientBrush(fullBox, low, high, LinearGradientMode.Vertical);
					//((LinearGradientBrush)bothBrush).SetSigmaBellShape(0.5f, 0.5f);
					((LinearGradientBrush)bothBrush).SetBlendTriangularShape(0.5f);
					break;
			}
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (highBrush != null) { highBrush.Dispose(); }
			if (lowBrush != null) { lowBrush.Dispose(); }
			if (bothBrush != null) { bothBrush.Dispose(); }
		}
	}
}