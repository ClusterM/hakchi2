using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.BevelledProgressPainter), "Icons.BevelledProgressPainter.ico")]
	public class BevelledProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private ColorRange bender;

		/// <summary></summary>
		public BevelledProgressPainter() {
			this.Color = Color.FromArgb(151, 151, 234);
		}

		/// <summary></summary>
		/// <param name="color"></param>
		public BevelledProgressPainter(Color color) {
			this.Color = color;
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the base progress color"), Browsable(true)]
		public Color Color {
			get { return bender.BaseColor; }
			set {
				bender = new ColorRange(value);
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			if (box.Width < 2) { return; }
			g.PageUnit = GraphicsUnit.Pixel;

			// fill box
			Rectangle back = new Rectangle(box.X, box.Y, box.Width, box.Height);
			try {
				back.Height -= 1;
				back.Width -= 1;
			} catch { }
			using (SolidBrush b = new SolidBrush(bender.BaseColor)) {
				g.FillRectangle(b, back);
			}

			box = new Rectangle(box.X, box.Y, box.Width - 1, box.Height - 1);

			using (Pen p = new Pen(bender.Light, 1)) {
				// inner top
				g.DrawLine(p, box.X + 1, box.Y + 1, box.Right - 1, box.Y + 1);
				// inner left
				g.DrawLine(p, box.X + 1, box.Y + 1, box.X + 1, box.Bottom - 1);
			}

			using (Pen p = new Pen(bender.Lighter, 1)) {
				// outer top
				g.DrawLine(p, box.X, box.Y, box.Right, box.Y);
				// outer left
				g.DrawLine(p, box.X, box.Y, box.X, box.Bottom);
			}

			// draw border
			using (Pen p = new Pen(bender.Dark, 1)) {
				// inner bottom
				g.DrawLine(p, box.X + 1, box.Bottom - 1, box.Right - 1, box.Bottom - 1);
				// inner right
				g.DrawLine(p, box.Right - 1, box.Y + 1, box.Right - 1, box.Bottom - 1);

				//g.DrawRectangle(p, box.X + 1, box.Y + 1, box.Width - 3, box.Height - 3);
			}

			using (Pen p = new Pen(bender.Darker, 1)) {
				// outer bottom
				g.DrawLine(p, box.X, box.Bottom, box.Right, box.Bottom);
				// outer right
				g.DrawLine(p, box.Right, box.Y, box.Right, box.Bottom);

				//g.DrawRectangle(p, box.X, box.Y, box.Width - 1, box.Height - 1);
			}

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}
	}
}