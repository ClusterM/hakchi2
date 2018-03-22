using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	public abstract class AbstractProgressPainter : Component, IProgressPainter {
		protected IGlossPainter gloss;
		protected IProgressBorderPainter border;
		internal int padding = 0;

		/// <summary></summary>
		[Category("Painters"), Description("Gets or sets the gloss painter chain"), Browsable(true)]
		public IGlossPainter GlossPainter {
			get { return this.gloss; }
			set {
				this.gloss = value;
				if (this.gloss != null) { this.gloss.PropertiesChanged += new EventHandler(component_PropertiesChanged); }
				FireChange();
			}
		}

		/// <summary></summary>
		[Category("Painters"), Description("Gets or sets the border painter for this progress painter"), Browsable(true)]
		public IProgressBorderPainter ProgressBorderPainter {
			get { return this.border; }
			set {
				this.border = value;
				if (this.gloss != null) { this.gloss.PropertiesChanged += new EventHandler(component_PropertiesChanged); }
				FireChange();
			}
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void component_PropertiesChanged(object sender, EventArgs e) {
			FireChange();
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="gr"></param>
		public void PaintProgress(Rectangle box, Graphics gr) {
			PaintThisProgress(box, gr);
			//if (this.gloss != null && box.Width > 1) {
			//    Rectangle b = new Rectangle(box.X, box.Y, box.Width - 1, box.Height - 1);
			//    //gr.DrawRectangle(Pens.Red, b);
			//    this.gloss.PaintGloss(box, gr);
			//}
			if (this.border != null && box.Width > 1) {
				int w = box.Width;
				//if (padding > 0) { w += 3; } else { w += 1; }
				//Rectangle b = new Rectangle(box.X - 1, box.Y - 1, w, box.Height + 3);
				Rectangle b = new Rectangle(box.X, box.Y, box.Width - 1, box.Height - 1);
				b.Inflate(1, 1);
				this.border.PaintBorder(b, gr);
			}
		}
		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="gr"></param>
		protected abstract void PaintThisProgress(Rectangle box, Graphics gr);

		/// <summary></summary>
		/// <param name="box"></param>
		public virtual void Resize(Rectangle box) {
			if (gloss != null) { gloss.Resize(box); }
			if (border != null) { border.Resize(box); }
			ResizeThis(box);
		}
		/// <summary></summary>
		/// <param name="box"></param>
		protected virtual void ResizeThis(Rectangle box) {}

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

		/// <summary></summary>
		protected void FireChange() {
			if (onPropertiesChanged != null) { onPropertiesChanged(this, EventArgs.Empty); }
		}

		/// <summary></summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			DisposeThis(disposing);
		}

		/// <summary></summary>
		/// <param name="disposing"></param>
		protected virtual void DisposeThis(bool disposing) {
		}
	}
}