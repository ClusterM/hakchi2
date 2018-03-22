using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.PlainProgressPainter), "Icons.PlainProgressPainter.ico")]
	public class PlainProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private Color color;
		private Brush brush;
		private Color edge = Color.Transparent;

		/// <summary></summary>
		public PlainProgressPainter() {
			this.Color = Color.FromArgb(151, 151, 234);
		}

		/// <summary></summary>
		/// <param name="color"></param>
		public PlainProgressPainter(Color color) {
			this.Color = color;
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the color to draw the leading edge of the progress with"), Browsable(true)]
		public Color LeadingEdge {
			get { return this.edge; }
			set { this.edge = value; FireChange(); }
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the base progress color"), Browsable(true)]
		public Color Color {
			get { return color; }
			set {
				color = value;
				brush = new SolidBrush(color);
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			if (box.Width <= 1) {
				return;
			}

			g.FillRectangle(brush, box);
			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
			if (!edge.Equals(Color.Transparent)) {
				g.DrawLine(new Pen(new SolidBrush(edge), 1f), box.Right, box.Y, box.Right, box.Bottom - 1);
			}
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
			brush.Dispose();
		}
	}
}