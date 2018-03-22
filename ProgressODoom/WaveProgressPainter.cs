using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	[ToolboxBitmapAttribute(typeof(ProgressODoom.WaveProgressPainter), "Icons.WaveProgressPainter.ico")]
	public class WaveProgressPainter : AbstractProgressPainter, IProgressPainter, IAnimatedProgressPainter, IDisposable {
		private Color color1 = Color.FromArgb(110, 195, 248);
		private Color color2 = Color.FromArgb(056, 150, 230);
		private int marqueeSpeed = 10;
		private int marqueeX = 0;
		private int animateX = 0;
		private bool isAnimated = false;

		/// <summary></summary>
		public Color BaseColor {
			get { return color1; }
			set {
				color1 = value;
				FireChange();
			}
		}

		/// <summary></summary>
		public Color WaveColor {
			get { return color2; }
			set {
				color2 = value;
				FireChange();
			}
		}

		/// <summary></summary>
		public int AnimationSpeed {
			get { return marqueeSpeed; }
			set { marqueeSpeed = value; FireChange(); }
		}

		/// <summary></summary>
		public bool Animating {
			get { return isAnimated; }
			set { isAnimated = value; }
		}

		private void AnimateFrame(Rectangle box, Graphics g, ref int marqueeX) {
			if (box == null || g == null || box.Width <= 1) { return; }

			g.SmoothingMode = SmoothingMode.AntiAlias;
			//g.Clip = new Region(box);

			g.FillRectangle(new SolidBrush(color1), box);
			int h = box.Height;
			int hm = (int)((float)h / 2f);

			using (GraphicsPath gp = new GraphicsPath()) {
				Point MidLeft = new Point(0, hm);
				Point MidRight = new Point(h * 2, hm);

				int currentX = box.Right + animateX; // Increment currentX to animate
				int left = currentX - (h * 2);
				if (left < box.Left) { left = box.Left; }
				while (currentX > box.Left) {
					left = currentX - (h * 2);

					MidLeft = new Point(left, hm);
					MidRight = new Point(currentX, hm);

					int crestX = currentX - h;
					gp.AddBezier(MidRight, new Point(crestX, 0), new Point(crestX, h), MidLeft);
					currentX -= h * 2;
				}
				gp.AddLine(MidLeft, new Point(box.Left, box.Bottom)); // left side
				gp.AddLine(new Point(box.Left, box.Bottom), new Point(box.Right, box.Bottom)); // bottom
				gp.AddLine(new Point(box.Right, box.Bottom), new Point(box.Right, hm)); // right side

				g.FillPath(new SolidBrush(color2), gp);
			}
			g.SmoothingMode = SmoothingMode.Default;

			if (isAnimated && ++animateX > (box.Height * 2)) {
				animateX = 1;
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			if (box.Width <= 1) {
				return;
			}

			if (isAnimated) {
				AnimateFrame(box, g, ref marqueeX);
			} else {
				int x = 0;
				AnimateFrame(box, g, ref x);
			}

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
		}
	}
}