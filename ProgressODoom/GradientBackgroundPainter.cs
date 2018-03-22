using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.GradientBackgroundPainter), "Icons.GradientBackgroundPainter.ico")]
	public class GradientBackgroundPainter : Component, IProgressBackgroundPainter, IDisposable {
		private Color top;
		private Color bottom;
		private Brush brush;
		private IGlossPainter gloss;

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
		public GradientBackgroundPainter() {
			this.top = Color.FromArgb(240, 240, 240);
			this.bottom = Color.FromArgb(224, 224, 224);
		}

		/// <summary></summary>
		/// <param name="color"></param>
		public GradientBackgroundPainter(Color top, Color bottom) {
			this.top = top;
			this.bottom = bottom;
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
		[Category("Appearance"), Description("Gets or sets the top gradient color"), Browsable(true)]
		public Color TopColor {
			get { return top; }
			set { top = value; FireChange(); }
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the bottom gradient color"), Browsable(true)]
		public Color BottomColor {
			get { return bottom; }
			set { bottom = value; FireChange(); }
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		public void PaintBackground(Rectangle box, Graphics g) {
			Resize(box);
			g.FillRectangle(brush, box);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		public void Resize(Rectangle box) {
			brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(0, box.Height), bottom, top);
		}

		/// <summary></summary>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (brush != null) { brush.Dispose(); }
		}
	}
}