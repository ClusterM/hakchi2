using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.FruityLoopsProgressPainter), "Icons.FruityLoopsProgressPainter.ico")]
	public class FruityLoopsProgressPainter : AbstractProgressPainter, IProgressPainter, IDisposable {
		private FruityLoopsProgressType type;

		private Color OnLit = Color.FromArgb(148, 170, 173);
		private Pen pOnLit; // = new Pen(new SolidBrush(OnLit),1f);
		private Color OnLitTop = Color.FromArgb(206, 227, 231);
		private Pen pOnLitTop; // = new Pen(new SolidBrush(OnLitTop),1f);
		private Color OnLitBot = Color.FromArgb(90, 117, 123);
		private Pen pOnLitBot; // = new Pen(new SolidBrush(OnLitBot),1f);

		private Color OnMid = Color.FromArgb(107, 130, 132);
		private Pen pOnMid; // = new Pen(new SolidBrush(OnMid),1f);
		private Color OnMidTop = Color.FromArgb(140, 154, 156);
		private Pen pOnMidTop; // = new Pen(new SolidBrush(OnMidTop),1f);
		private Color OnMidBot = Color.FromArgb(57, 85, 82);
		private Pen pOnMidBot; // = new Pen(new SolidBrush(OnMidBot),1f);

		private Color OnDrk = Color.FromArgb(57, 85, 82);
		private Pen pOnDrk; // = new Pen(new SolidBrush(OnDrk),1f);
		private Color OnDrkTop = Color.FromArgb(107, 125, 123);
		private Pen pOnDrkTop; // = new Pen(new SolidBrush(OnDrkTop),1f);
		private Color OnDrkBot = Color.FromArgb(33, 60, 66);
		private Pen pOnDrkBot; // = new Pen(new SolidBrush(OnDrkBot),1f);

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the type of FruityLoops progress style"), Browsable(true)]
		public FruityLoopsProgressType FruityType {
			get { return type; }
			set {
				type = value;
				if (type == FruityLoopsProgressType.DoubleLayer) {
					OnLit = Color.FromArgb(148, 170, 173);
					pOnLit = new Pen(new SolidBrush(OnLit), 1f);
					OnLitTop = Color.FromArgb(206, 227, 231);
					pOnLitTop = new Pen(new SolidBrush(OnLitTop), 1f);
					OnLitBot = Color.FromArgb(90, 113, 115);
					pOnLitBot = new Pen(new SolidBrush(OnLitBot), 1f);

					OnDrk = Color.FromArgb(115, 142, 148);
					pOnDrk = new Pen(new SolidBrush(OnDrk), 1f);
					OnDrkTop = Color.FromArgb(181, 199, 198);
					pOnDrkTop = new Pen(new SolidBrush(OnDrkTop), 1f);
					OnDrkBot = Color.FromArgb(66, 89, 90);
					pOnDrkBot = new Pen(new SolidBrush(OnDrkBot), 1f);
				} else if (type == FruityLoopsProgressType.TripleLayer) {
					OnLit = Color.FromArgb(148, 170, 173);
					pOnLit = new Pen(new SolidBrush(OnLit), 1f);
					OnLitTop = Color.FromArgb(206, 227, 231);
					pOnLitTop = new Pen(new SolidBrush(OnLitTop), 1f);
					OnLitBot = Color.FromArgb(90, 117, 123);
					pOnLitBot = new Pen(new SolidBrush(OnLitBot), 1f);

					OnMid = Color.FromArgb(107, 130, 132);
					pOnMid = new Pen(new SolidBrush(OnMid), 1f);
					OnMidTop = Color.FromArgb(140, 154, 156);
					pOnMidTop = new Pen(new SolidBrush(OnMidTop), 1f);
					OnMidBot = Color.FromArgb(57, 85, 82);
					pOnMidBot = new Pen(new SolidBrush(OnMidBot), 1f);

					OnDrk = Color.FromArgb(57, 85, 82);
					pOnDrk = new Pen(new SolidBrush(OnDrk), 1f);
					OnDrkTop = Color.FromArgb(107, 125, 123);
					pOnDrkTop = new Pen(new SolidBrush(OnDrkTop), 1f);
					OnDrkBot = Color.FromArgb(33, 60, 66);
					pOnDrkBot = new Pen(new SolidBrush(OnDrkBot), 1f);
				}
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		protected override void PaintThisProgress(Rectangle box, Graphics g) {
			try {
				box.Height -= 1;
			} catch { }

			if (box.Width <= 1) { return; }
			if (type == FruityLoopsProgressType.DoubleLayer) {
				PaintDouble(box, g);
			} else if (type == FruityLoopsProgressType.TripleLayer) {
				PaintTriple(box, g);
			}

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		private void PaintDouble(Rectangle r, Graphics g) {
			bool lite = true;

			Brush b = new SolidBrush(pOnLit.Color);
			g.FillRectangle(b, r);
			g.DrawLine(pOnLitTop, r.X, r.Y, r.Right - 1, r.Y);
			g.DrawLine(pOnLitBot, r.X, r.Bottom, r.Right - 1, r.Bottom);
			for (int i = r.X; i < r.Right; i++) {
				if (lite) {
					//g.DrawLine(off ? pOffLitTop : pOnLitTop, i, r.Y, i, r.Y + 1);
					//g.DrawLine(off ? pOffLitBot : pOnLitBot, i, r.Height, i, r.Height - 1);
					//g.DrawLine(off ? pOffLit : pOnLit, i, r.Y + 1, i, r.Height - 1);
				} else {
					g.DrawLine(pOnDrkTop, i, r.Y, i, r.Y + 1);
					g.DrawLine(pOnDrkBot, i, r.Bottom, i, r.Bottom - 1);
					g.DrawLine(pOnDrk, i, r.Y + 1, i, r.Bottom - 1);
				}
				lite = !lite;
			}
		}

		private void PaintTriple(Rectangle r, Graphics g) {
			int lite = 1;

			Brush b = new SolidBrush(pOnMid.Color);
			g.FillRectangle(b, r);
			g.DrawLine(pOnMidTop, r.X, r.Y, r.Right - 1, r.Y);
			g.DrawLine(pOnMidBot, r.X, r.Bottom, r.Right - 1, r.Bottom);
			for (int i = r.X; i < r.Right; i++) {
				if (lite == 2) {
					g.DrawLine(pOnLitTop, i, r.Y, i, r.Y + 1);
					g.DrawLine(pOnLitBot, i, r.Bottom, i, r.Bottom - 1);
					g.DrawLine(pOnLit, i, r.Y + 1, i, r.Bottom - 1);
					lite = 0;
				} else if (lite == 1) {
					//g.DrawLine(pOnMidTop, i, r.Y, i, r.Y + 1);
					//g.DrawLine(pOnMidBot, i, r.Height, i, r.Height - 1);
					//g.DrawLine(pOnMid, i, r.Y + 1, i, r.Height - 1);
					lite = 2;
				} else if (lite == 0) {
					g.DrawLine(pOnDrkTop, i, r.Y, i, r.Y + 1);
					g.DrawLine(pOnDrkBot, i, r.Bottom, i, r.Bottom - 1);
					g.DrawLine(pOnDrk, i, r.Y + 1, i, r.Bottom - 1);
					lite = 1;
				}
			}
		}

		/// <summary></summary>
		protected override void DisposeThis(bool disposing) {
			if (pOnLit != null) { pOnLit.Dispose(); }
			if (pOnLitTop != null) { pOnLitTop.Dispose(); }
			if (pOnLitBot != null) { pOnLitBot.Dispose(); }
			if (pOnMid != null) { pOnMid.Dispose(); }
			if (pOnMidTop != null) { pOnMidTop.Dispose(); }
			if (pOnMidBot != null) { pOnMidBot.Dispose(); }
			if (pOnDrk != null) { pOnDrk.Dispose(); }
			if (pOnDrkTop != null) { pOnDrkTop.Dispose(); }
			if (pOnDrkBot != null) { pOnDrkBot.Dispose(); }
		}

		/// <summary></summary>
		public enum FruityLoopsProgressType {
			/// <summary></summary>
			DoubleLayer,
			/// <summary></summary>
			TripleLayer
		}
	}
}