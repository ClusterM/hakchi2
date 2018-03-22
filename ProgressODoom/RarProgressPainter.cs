using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.RarProgressPainter), "Icons.RarProgressPainter.ico")]
	public class RarProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private Brush brush;
		private Pen inner, outer, edge;
		private bool showEdge = false;
		private RarProgressType type;

		/// <summary></summary>
		public RarProgressPainter() {
			this.ProgressType = RarProgressType.Silver;
		}

		/// <summary></summary>
		/// <param name="type"></param>
		public RarProgressPainter(RarProgressType type) {
			this.ProgressType = type;
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the type of rar progress color"), Browsable(true)]
		public RarProgressType ProgressType {
			get { return type; }
			set {
				this.type = value;
				switch (type) {
					case RarProgressType.Silver:
						brush = new SolidBrush(Color.FromArgb(214, 214, 220));
						inner = new Pen(new SolidBrush(Color.FromArgb(232, 232, 238)), 1f);
						outer = new Pen(new SolidBrush(Color.FromArgb(255, 255, 255)), 1f);
						edge = new Pen(new SolidBrush(Color.FromArgb(096, 096, 096)), 1f);
						break;
					case RarProgressType.Gold:
						brush = new SolidBrush(Color.FromArgb(208, 192, 160));
						inner = new Pen(new SolidBrush(Color.FromArgb(228, 212, 180)), 1f);
						outer = new Pen(new SolidBrush(Color.FromArgb(255, 255, 192)), 1f);
						edge = new Pen(new SolidBrush(Color.FromArgb(096, 096, 096)), 1f);
						break;
				}
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets whether or not this progress has a leading edge"), Browsable(true)]
		public bool ShowEdge {
			get { return showEdge; }
			set {
				showEdge = value;
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
			if (box.Width <= 1) {
				return;
			}

			g.FillRectangle(brush, box);
			Rectangle innerBox = box;
			innerBox.Inflate(-1, -1);
			g.DrawRectangle(inner, innerBox);
			g.DrawLine(outer, box.X, box.Y, box.Right, box.Y);
			g.DrawLine(outer, box.X, box.Y, box.X, box.Bottom);
			g.DrawLine(edge, box.X, box.Bottom, box.Right, box.Bottom);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}

			if (showEdge) {
				g.DrawLine(edge, box.Right, box.Y, box.Right, box.Bottom);
			}
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
			brush.Dispose();
			inner.Dispose();
			outer.Dispose();
			edge.Dispose();
		}

		/// <summary></summary>
		public enum RarProgressType {
			Gold, Silver
		}
	}
}