using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.StripedProgressPainter), "Icons.StripedProgressPainter.ico")]
	public class StripedProgressPainter : AbstractProgressPainter, IProgressPainter, IAnimatedProgressPainter, IDisposable {
		private Color color1 = Color.FromArgb(110, 195, 248);
		private Color color2 = Color.FromArgb(056, 150, 230);
		private Color blend = Color.FromArgb(097, 184, 244);
		private int spacing = 6;
		private int marqueeSpeed = 10;

		private int marqueeX = 0;

		private bool isAnimated = false;

		/// <summary></summary>
		public StripedProgressPainter() {
		}

		/// <summary></summary>
		public Color BaseColor {
			get { return color1; }
			set {
				color1 = value;
				blend = ColorRange.Morph(0.25f, color1, color2);
				FireChange();
			}
		}

		/// <summary></summary>
		public Color StripeColor {
			get { return color2; }
			set {
				color2 = value;
				blend = ColorRange.Morph(0.25f, color1, color2);
				FireChange();
			}
		}

		/// <summary></summary>
		public int StripeSpacing {
			get { return spacing; }
			set { spacing = value; FireChange(); }
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

			Pen penb = new Pen(new SolidBrush(blend));
			g.FillRectangle(new SolidBrush(color1), box);
			for (int i = box.Right + marqueeX; i > box.Left; i -= ((box.Height * 2) + StripeSpacing - 1)) {
				Point theoreticalRightTop = new Point(i, box.Top);
				Point theoreticalRightBottom = new Point(i - box.Height, box.Bottom);
				Point theoreticalLeftTop = new Point(i - box.Height, box.Top);
				Point theoreticalLeftBottom = new Point(i - (box.Height * 2), box.Bottom);

				Point leftTop, leftBottom, rightTop, rightBottom;
				using (GraphicsPath gp = new GraphicsPath()) {
					if (theoreticalLeftTop.X <= box.Left) {
						// left triangle
						int diff = i - box.Height;
						rightTop = new Point(i, box.Top);
						rightBottom = new Point(box.Left, box.Bottom + diff);
						leftTop = new Point(box.Left, box.Top);
						leftBottom = leftTop;

						if (rightBottom.Equals(rightTop)) { continue; }

						gp.AddLine(rightTop, rightBottom);
						gp.AddLine(rightBottom, leftTop);
						gp.AddLine(leftTop, new Point(i, box.Top));
					} else if (theoreticalLeftBottom.X <= box.Left) {
						// left pentagon
						int diff = i - (box.Height * 2);
						rightTop = new Point(i, box.Top);
						rightBottom = new Point(i - box.Height, box.Bottom);
						leftTop = new Point(i - box.Height, box.Top);
						leftBottom = new Point(box.Left, box.Bottom + diff);

						gp.AddLine(rightTop, rightBottom);
						gp.AddLine(rightBottom, new Point(box.Left, box.Bottom));
						gp.AddLine(new Point(box.Left, box.Bottom), leftBottom);
						gp.AddLine(leftBottom, leftTop);
						gp.AddLine(leftTop, rightTop);
					} else if (theoreticalRightBottom.X >= box.Right) {
						// right triangle
						int diff = marqueeX - box.Height;
						leftTop = new Point(box.Right, box.Top + diff); //= something funky
						leftBottom = theoreticalLeftBottom;
						rightBottom = new Point(box.Right, box.Bottom);
						rightTop = rightBottom;

						if (leftBottom.Equals(leftTop)) { continue; }

						gp.AddLine(leftTop, rightBottom);
						gp.AddLine(rightBottom, leftBottom);
						gp.AddLine(leftBottom, leftTop);
					} else if (theoreticalRightTop.X >= box.Right) {
						// right pentagon
						int diff = i - box.Right;
						Point topRight = new Point(box.Right, box.Top);
						rightTop = new Point(box.Right, box.Top + diff);
						rightBottom = new Point(i - box.Height, box.Bottom);
						leftTop = new Point(i - box.Height, box.Top);
						leftBottom = new Point(i - (box.Height * 2), box.Bottom);

						gp.AddLine(leftTop, topRight);
						gp.AddLine(topRight, rightTop);
						gp.AddLine(rightTop, rightBottom);
						gp.AddLine(rightBottom, leftBottom);
						gp.AddLine(leftBottom, leftTop);
					} else {
						// mid-range rectangle
						rightTop = new Point(i, box.Top);
						rightBottom = new Point(i - box.Height, box.Bottom);
						leftTop = new Point(i - box.Height, box.Top);
						leftBottom = new Point(i - (box.Height * 2), box.Bottom);

						gp.AddLine(rightTop, rightBottom);
						gp.AddLine(rightBottom, leftBottom);
						gp.AddLine(leftBottom, leftTop);
						gp.AddLine(leftTop, rightTop);
					}
					g.FillPath(new SolidBrush(color2), gp);
				}

				if (!leftTop.Equals(leftBottom)) {
					g.DrawLine(penb, leftTop, leftBottom);
				}
				if (!rightTop.Equals(rightBottom)) {
					g.DrawLine(penb, rightTop, rightBottom);
				}
			}
			g.DrawLine(penb, new Point(box.Left, box.Bottom), new Point(box.Right, box.Bottom));

			if (++marqueeX > (box.Height * 2) + StripeSpacing) {
				marqueeX = 1;
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