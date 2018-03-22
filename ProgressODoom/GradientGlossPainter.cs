using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.GradientGlossPainter), "Icons.GradientGlossPainter.ico")]
	public class GradientGlossPainter : ChainedGlossPainter {
		private GlossStyle style = GlossStyle.Bottom;
		private int percent = 50;
		private Color color = Color.White;
		private int highAlpha = 240;
		private int lowAlpha = 0;
		private float angle = 90f;
		private Brush brush;

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
		[Category("Appearance"), Description("Gets or sets the percentage of surface this gloss should cover"), Browsable(true)]
		public int PercentageCovered {
			get { return this.percent; }
			set {
				this.percent = value;
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets color to gloss"), Browsable(true)]
		public Color Color {
			get { return this.color; }
			set {
				this.color = value;
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
		[Category("Blending"), Description("Gets or sets angle for the gradient"), Browsable(true)]
		public float Angle {
			get { return this.angle; }
			set {
				this.angle = value;
				FireChange();
			}
		}

		protected override void PaintThisGloss(Rectangle box, Graphics g) {
			int y = (int)(((float)box.Height * (float)percent) / 100f);
			if (box.Y + y > box.Height) { y = box.Height; }

			Rectangle cover = box;
			switch (style) {
				case GlossStyle.Bottom:
					int start = box.Height + box.Y - y;
					cover = new Rectangle(box.X, start - 1, box.Width /*- 1*/, box.Bottom - start);
					break;
				case GlossStyle.Top:
					cover = new Rectangle(box.X, box.Y - 1, box.Width /*- 1*/, y + 2);
					break;
				case GlossStyle.Both:
					cover = box;
					break;
			}

			Color hcolor = Color.FromArgb(highAlpha, color.R, color.G, color.B);
			Color lcolor = Color.FromArgb(lowAlpha, color.R, color.G, color.B);
			brush = new LinearGradientBrush(cover, hcolor, lcolor, angle, true);
			g.FillRectangle(brush, cover);
			//g.DrawRectangle(Pens.Red, cover);
		}

		protected override void ResizeThis(Rectangle box) {
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (brush != null) { brush.Dispose(); }
		}
	}
}