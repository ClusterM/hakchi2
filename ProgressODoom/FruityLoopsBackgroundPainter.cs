using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.FruityLoopsBackgroundPainter), "Icons.FruityLoopsBackgroundPainter.ico")]
	public class FruityLoopsBackgroundPainter : Component, IProgressBackgroundPainter, IDisposable {
		private IGlossPainter gloss;
		private FruityLoopsProgressPainter.FruityLoopsProgressType type;
		private Image img;

		private Color OffLit = Color.FromArgb(49, 69, 74);
		private Pen pOffLit; // = new Pen(new SolidBrush(OffLit),1f);
		private Color OffLitTop = Color.FromArgb(66, 85, 90);
		private Pen pOffLitTop; // = new Pen(new SolidBrush(OffLitTop),1f);
		private Color OffLitBot = Color.FromArgb(24, 48, 49);
		private Pen pOffLitBot; // = new Pen(new SolidBrush(OffLitBot),1f);

		private Color OffMid = Color.FromArgb(24, 48, 49);
		private Pen pOffMid; // = new Pen(new SolidBrush(OffMid),1f);
		private Color OffMidTop = Color.FromArgb(24, 48, 49);
		private Pen pOffMidTop; // = new Pen(new SolidBrush(OffMidTop),1f);
		private Color OffMidBot = Color.FromArgb(8, 28, 24);
		private Pen pOffMidBot; // = new Pen(new SolidBrush(OffMidBot),1f);

		private Color OffDrk = Color.FromArgb(0, 24, 24);
		private Pen pOffDrk; // = new Pen(new SolidBrush(OffDrk),1f);
		private Color OffDrkTop = Color.FromArgb(8, 28, 24);
		private Pen pOffDrkTop; // = new Pen(new SolidBrush(OffDrkTop),1f);
		private Color OffDrkBot = Color.FromArgb(0, 16, 16);
		private Pen pOffDrkBot; // = new Pen(new SolidBrush(OffDrkBot),1f);

		private EventHandler onPropertiesChanged;
		/// <summary></summary>
		public event EventHandler PropertiesChanged {
			add {
				if (onPropertiesChanged != null) {
					foreach (Delegate d in onPropertiesChanged.GetInvocationList()) {
						if (object.ReferenceEquals(d, value)) { return; }
					}
				}
				onPropertiesChanged = (EventHandler)Delegate.Combine(onPropertiesChanged, value);
			}
			remove { onPropertiesChanged = (EventHandler)Delegate.Remove(onPropertiesChanged, value); }
		}

		private void FireChange() {
			if (onPropertiesChanged != null) { onPropertiesChanged(this, EventArgs.Empty); }
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void component_PropertiesChanged(object sender, EventArgs e) {
			FireChange();
		}

		/// <summary></summary>
		[Category("Painters"), Description("Gets or sets the chain of gloss painters"), Browsable(true)]
		public IGlossPainter GlossPainter {
			get { return this.gloss; }
			set {
				this.gloss = value;
				if (this.gloss != null) { this.gloss.PropertiesChanged += new EventHandler(component_PropertiesChanged); }
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the type of FruityLoops progress style"), Browsable(true)]
		public FruityLoopsProgressPainter.FruityLoopsProgressType FruityType {
			get { return type; }
			set {
				type = value;
				if (type == FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer) {
					OffLit = Color.FromArgb(49, 69, 74);
					pOffLit = new Pen(new SolidBrush(OffLit), 1f);
					OffLitTop = Color.FromArgb(57, 77, 82);
					pOffLitTop = new Pen(new SolidBrush(OffLitTop), 1f);
					OffLitBot = Color.FromArgb(24, 48, 49);
					pOffLitBot = new Pen(new SolidBrush(OffLitBot), 1f);

					OffDrk = Color.FromArgb(24, 48, 49);
					pOffDrk = new Pen(new SolidBrush(OffDrk), 1f);
					OffDrkTop = Color.FromArgb(16, 40, 41);
					pOffDrkTop = new Pen(new SolidBrush(OffDrkTop), 1f);
					OffDrkBot = Color.FromArgb(8, 18, 24);
					pOffDrkBot = new Pen(new SolidBrush(OffDrkBot), 1f);
				} else if (type == FruityLoopsProgressPainter.FruityLoopsProgressType.TripleLayer) {
					OffLit = Color.FromArgb(49, 69, 74);
					pOffLit = new Pen(new SolidBrush(OffLit), 1f);
					OffLitTop = Color.FromArgb(66, 85, 90);
					pOffLitTop = new Pen(new SolidBrush(OffLitTop), 1f);
					OffLitBot = Color.FromArgb(24, 48, 49);
					pOffLitBot = new Pen(new SolidBrush(OffLitBot), 1f);

					OffMid = Color.FromArgb(24, 48, 49);
					pOffMid = new Pen(new SolidBrush(OffMid), 1f);
					OffMidTop = Color.FromArgb(24, 48, 49);
					pOffMidTop = new Pen(new SolidBrush(OffMidTop), 1f);
					OffMidBot = Color.FromArgb(8, 28, 24);
					pOffMidBot = new Pen(new SolidBrush(OffMidBot), 1f);

					OffDrk = Color.FromArgb(0, 24, 24);
					pOffDrk = new Pen(new SolidBrush(OffDrk), 1f);
					OffDrkTop = Color.FromArgb(8, 28, 24);
					pOffDrkTop = new Pen(new SolidBrush(OffDrkTop), 1f);
					OffDrkBot = Color.FromArgb(0, 16, 16);
					pOffDrkBot = new Pen(new SolidBrush(OffDrkBot), 1f);
				}
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		public void PaintBackground(Rectangle box, Graphics g) {
			if (img == null) {
				if (type == FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer) {
					PaintDouble(box, g);
				} else if (type == FruityLoopsProgressPainter.FruityLoopsProgressType.TripleLayer) {
					PaintTriple(box, g);
				}
			}
			g.DrawImageUnscaled(img, 0, 0);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		protected virtual void PaintDouble(Rectangle r, Graphics g) {
			bool lite = true;
			img = new Bitmap(r.Width + 1, r.Height + 1);
			Graphics gi = Graphics.FromImage(img);

			for (int i = 1; i < r.Width + 1; i++) {
				if (lite) {
					gi.DrawLine(pOffLitTop, i, r.Y, i, r.Y + 1);
					gi.DrawLine(pOffLitBot, i, r.Height, i, r.Height - 1);
					gi.DrawLine(pOffLit, i, r.Y + 1, i, r.Height - 1);
				} else {
					gi.DrawLine(pOffDrkTop, i, r.Y, i, r.Y + 1);
					gi.DrawLine(pOffDrkBot, i, r.Height, i, r.Height - 1);
					gi.DrawLine(pOffDrk, i, r.Y + 1, i, r.Height - 1);
				}
				lite = !lite;
			}
			gi.Dispose();
		}

		/// <summary></summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		protected virtual void PaintTriple(Rectangle r, Graphics g) {
			int lite = 1;
			img = new Bitmap(r.Width + 1, r.Height + 1);
			Graphics gi = Graphics.FromImage(img);

			for (int i = 1; i < r.Width + 1; i++) {
				if (lite == 2) {
					gi.DrawLine(pOffLitTop, i, r.Y, i, r.Y + 1);
					gi.DrawLine(pOffLitBot, i, r.Height, i, r.Height - 1);
					gi.DrawLine(pOffLit, i, r.Y + 1, i, r.Height - 1);
					lite = 0;
				} else if (lite == 1) {
					gi.DrawLine(pOffMidTop, i, r.Y, i, r.Y + 1);
					gi.DrawLine(pOffMidBot, i, r.Height, i, r.Height - 1);
					gi.DrawLine(pOffMid, i, r.Y + 1, i, r.Height - 1);
					lite = 2;
				} else if (lite == 0) {
					gi.DrawLine(pOffDrkTop, i, r.Y, i, r.Y + 1);
					gi.DrawLine(pOffDrkBot, i, r.Height, i, r.Height - 1);
					gi.DrawLine(pOffDrk, i, r.Y + 1, i, r.Height - 1);
					lite = 1;
				}
			}
			gi.Dispose();
		}

		/// <summary></summary>
		public void Resize(Rectangle box) {
			img = null;
		}

		/// <summary></summary>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (img != null) { img.Dispose(); }

			if (pOffLit != null) { pOffLit.Dispose(); }
			if (pOffLitTop != null) { pOffLitTop.Dispose(); }
			if (pOffLitBot != null) { pOffLitBot.Dispose(); }
			if (pOffMid != null) { pOffMid.Dispose(); }
			if (pOffMidTop != null) { pOffMidTop.Dispose(); }
			if (pOffMidBot != null) { pOffMidBot.Dispose(); }
			if (pOffDrk != null) { pOffDrk.Dispose(); }
			if (pOffDrkTop != null) { pOffDrkTop.Dispose(); }
			if (pOffDrkBot != null) { pOffDrkBot.Dispose(); }
		}
	}
}