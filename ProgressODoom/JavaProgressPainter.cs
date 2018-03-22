using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.JavaProgressPainter), "Icons.JavaProgressPainter.ico")]
	public class JavaProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private Color color;
		private ColorSet colors;

		/// <summary></summary>
		public JavaProgressPainter() {
			color = Color.SkyBlue;
			colors = new ColorSet(false, color, 0.95f, 8);
		}

		/// <summary></summary>
		/// <param name="color"></param>
		public JavaProgressPainter(Color color) {
			this.color = color;
			colors = new ColorSet(false, color, 0.95f, 8);
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the base progress color."), Browsable(true)]
		public Color Color {
			get { return color; }
			set {
				color = value;
				colors = new ColorSet(false, color, 0.95f, 8);
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
			if (box.Width <= 1) { return; }

			float x = (float)box.X;
			float y = (float)box.Y;
			float w = (float)box.Right;
			float h = (float)box.Bottom;

			//Color corner
			Pen p;
			x += 2f; //x += 3f; //x += 2f; //x = x + 3f;
			y += 4f; //y += 5f; //y += 4f; //y = y + 5f;
			w -= 2f; //w -= 3f; //w -= 1f; //w = w - 2f;
			h -= 4f; //h -= 6f; //h -= 4f; //h = h - 6f;

			// Progress
			colors = new ColorSet(true, this.color, 0.95f, (int)h);
			float z = 2;
			float ni;
			float th = box.Height - 4;
			for (int i = -2; i < th; i++) {
			//for (int i = -2; i < th; i++) {
				z = (i < 0 ? i * -1 : i);
				Color c = colors.Colors[colors.Colors.Length - 1];
				try {
					c = colors.Colors[(int)z];
				} catch {}
				p = new Pen(c);
				ni = y + i;
				g.DrawLine(p, x, ni, w, ni);
			}

			Color progborder = ColorRange.Morph(0.2f, Color.FromArgb(98, 98, 89), this.color);

			Color fade = Color.FromArgb(64, progborder.R, progborder.G, progborder.B);
			p = new Pen(new SolidBrush(fade), 1);
			//g.DrawRectangle(p, x - 1f, y - 3f, w - 1f, h + 1f);
			Rectangle bbox = box;
			bbox.Inflate(-1, -1);
			g.DrawRectangle(p, bbox);

			// Border
			p = new Pen(progborder, 1);
			////g.DrawRectangle(p, x - 1f, y - 3f, w - 1f, h + 1f);
			//g.DrawLine(p, x, y - 3f, w, y - 3f);
			//g.DrawLine(p, x, h + 3f, w, h + 3f);
			//g.DrawLine(p, x - 1f, y - 2f, x - 1f, h + 2f);
			//g.DrawLine(p, w + 1f, y - 2f, w + 1f, h + 2f);
			g.DrawLine(p, x + 1f, y - 3f, w - 1f, y - 3f);
			g.DrawLine(p, x + 1f, h + 3f, w - 1f, h + 3f);
			g.DrawLine(p, x - 1f, y - 1f, x - 1f, h + 1f);
			g.DrawLine(p, w + 1f, y - 1f, w + 1f, h + 1f);

			// Border corner skirt
			//Color skirt = ColorRange.Morph(0.8f, this.color, progborder);
			Color skirt = Color.FromArgb(210, progborder.R, progborder.G, progborder.B);
			p = new Pen(skirt, 1);
			//// Upper Left
			//g.DrawLine(p, x - 1f, y - 3f, x, y - 3f);
			//g.DrawLine(p, x - 1f, y - 3f, x - 1f, y - 2f);
			g.DrawLine(p, x, y - 3f, x - 1f, y - 2f);
			//// Lower Left
			//g.DrawLine(p, x - 1f, h + 3f, x - 1f, h + 2f);
			//g.DrawLine(p, x - 1f, h + 3f, x, h + 3f);
			g.DrawLine(p, x, h + 3f, x - 1f, h + 2f);
			//// Upper Right
			//g.DrawLine(p, w + 1f, y - 3f, w + 1f, y - 2f);
			//g.DrawLine(p, w + 1f, y - 3f, w, y - 3f);
			g.DrawLine(p, w, y - 3f, w + 1f, y - 2f);
			//// Lower Right
			//g.DrawLine(p, w + 1f, h + 3f, w, h + 3f);
			//g.DrawLine(p, w + 1f, h + 3f, w + 1f, h + 2f);
			g.DrawLine(p, w, h + 3f, w + 1f, h + 2f);

			//// Border corner  x-1f, y-3f, w-2f, h+1f
			//Color corners = ColorRange.Morph(0.5f, this.color, progborder); //Color.FromArgb(229, 229, 222)
			//p = new Pen(corners, 1);
			//g.DrawLine(p, x - 1f, y - 3f, x, y - 2f);  // ul
			//g.DrawLine(p, x - 1f, h + 3f, x, h + 2f);  // ll
			//g.DrawLine(p, w + 1f, y - 3f, w, y - 2f);  // ur
			//g.DrawLine(p, w + 1f, h + 3f, w, h + 2f);  // lr

			////// Outer corner (Left side only)
			////Color outcorner = ColorRange.Morph(0.5f, progborder, Color.FromArgb(229, 229, 222));
			////p = new Pen(outcorner, 1);
			////g.DrawLine(p, x - 2f, y - 4f, x - 1f, y - 3f);
			////g.DrawLine(p, x - 2f, h + 4f, x - 1f, h + 3f);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}
	}
}