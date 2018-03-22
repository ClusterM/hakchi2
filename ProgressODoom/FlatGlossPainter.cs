using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.FlatGlossPainter), "Icons.FlatGlossPainter.ico")]
	public class FlatGlossPainter : ChainedGlossPainter {
		private GlossStyle style = GlossStyle.Bottom;
		private int percent = 50;
		private Rectangle box;
		private Color color = Color.White;
		private int alpha = 128;
		private Brush brush;

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
		[Category("Appearance"), Description("Gets or sets the percentage of surface this gloss should cover"), Browsable(true)]
		public int PercentageCovered {
			get { return this.percent; }
			set {
				if (value < 0 || value > 100) {
					throw new ArgumentException("Percentage value must be between 0 and 100.");
				}
				this.percent = value;
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
				box = new Rectangle(0, 0, 1, 1);
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Blending"), Description("Gets or sets the alpha value"), Browsable(true)]
		public int Alpha {
			get { return this.alpha; }
			set {
				if (value < 0 || value > 255) {
					throw new ArgumentException("Alpha values must be between 0 and 255.");
				}
				this.alpha = value;
				box = new Rectangle(0, 0, 1, 1);
				FireChange();
			}
		}

		protected override void PaintThisGloss(Rectangle box, Graphics g) {
			if (!this.box.Equals(box)) {
				this.box = box;
			}

			int y = (int)(((float)box.Height * (float)percent) / 100f);
			if (box.Y + y > box.Height) { y = box.Height; }

			Rectangle cover = box;
			switch (style) {
				case GlossStyle.Bottom:
					int start = box.Height + box.Y - y;
					cover = new Rectangle(box.X, start, box.Width, box.Bottom - start);
					break;
				case GlossStyle.Top:
					cover = new Rectangle(box.X, box.Y, box.Width, y + 1);
					break;
				case GlossStyle.Both:
					cover = box;
					break;
			}

			Color ccv = Color.FromArgb(alpha, color.R, color.G, color.B);
			brush = new SolidBrush(ccv);
			g.FillRectangle(brush, cover);
		}

		protected override void ResizeThis(Rectangle box) {
			if (!this.box.Equals(box)) {
				this.box = box;
			}
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (brush != null) { brush.Dispose(); }
		}
	}
}