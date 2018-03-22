using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	[ToolboxBitmapAttribute(typeof(ProgressODoom.RarBackgroundPainter), "Icons.RarBackgroundPainter.ico")]
	public class RarBackgroundPainter : Component, IProgressBackgroundPainter, IDisposable {
		private Brush brush;
		private IGlossPainter gloss;
		private Pen outer, inner, border;

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

		public RarBackgroundPainter() {
			brush = new SolidBrush(Color.FromArgb(148, 110, 110));
			inner = new Pen(new SolidBrush(Color.FromArgb(158, 128, 128)), 1f);
			outer = new Pen(new SolidBrush(Color.FromArgb(180, 148, 148)), 1f);
			border = new Pen(new SolidBrush(Color.FromArgb(096, 096, 096)), 1f);
		}

		/// <summary></summary>
		public IGlossPainter GlossPainter {
			get { return this.gloss; }
			set {
				this.gloss = value;
				if (this.gloss != null) { this.gloss.PropertiesChanged += new EventHandler(component_PropertiesChanged); }
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		public void PaintBackground(Rectangle box, Graphics g) {
			g.FillRectangle(brush, box);
			g.DrawRectangle(inner, 2, 2, box.Width - 3, box.Height - 4);
			g.DrawRectangle(outer, 1, 1, box.Width - 1, box.Height - 2);
			g.DrawLine(border, 1, box.Height, box.Width, box.Height);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		public void Resize(Rectangle box) {
		}

		/// <summary></summary>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			brush.Dispose();
			inner.Dispose();
			outer.Dispose();
			border.Dispose();
		}
	}
}