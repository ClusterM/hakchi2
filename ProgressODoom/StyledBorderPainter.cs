using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.StyledBorderPainter), "Icons.StyledBorderPainter.ico")]
	public class StyledBorderPainter : Component, IProgressBorderPainter, IDisposable {
		private Border3DStyle border;

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
		public StyledBorderPainter() {
			border = Border3DStyle.Raised;
		}

		/// <summary></summary>
		[Category("Appearance"), Description("Gets or sets the border style"), Browsable(true)]
		public Border3DStyle Border3D {
			get { return border; }
			set { border = value; FireChange(); }
		}

		/// <summary></summary>
		[Browsable(false)]
		public int BorderWidth {
			get { return 2; }
		}

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		public void PaintBorder(Rectangle box, Graphics g) {
			Rectangle brd = new Rectangle(box.X, box.Y, box.Width, box.Height + 1);
			ControlPaint.DrawBorder3D(g, brd, border);
		}

		/// <summary></summary>
		public void Resize(Rectangle box) {
		}

		/// <summary></summary>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
	}
}