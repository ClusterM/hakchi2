using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.CandyCaneBackgroundPainter), "Icons.CandyCaneBackgroundPainter.ico")]
	public class CandyCaneBackgroundPainter : Component, IProgressBackgroundPainter, IDisposable {
		private Image img;
		private Rectangle box;
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
		/// <param name="box"></param>
		/// <param name="g"></param>
		public void PaintBackground(Rectangle box, Graphics g) {
			if (img == null) {
				Resize(box);
			}
			g.DrawImageUnscaled(img, box.X, box.Y);
			//g.FillRectangle(brush, box);

			if (gloss != null) {
				gloss.PaintGloss(box, g);
			}
		}

		/// <summary></summary>
		public void Resize(Rectangle box) {
			this.box = box;
			RepaintImage(box);
		}

		/// <summary></summary>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (img != null) {
				img.Dispose();
			}
		}

		private void RepaintImage(Rectangle box) {
            if (box.Width <= 0) box.Width = 1;
            if (box.Height <= 0) box.Height = 1;
			Bitmap source = BuildTile(box.Width);
			img = new Bitmap(box.Width, box.Height);
			using (Graphics g = Graphics.FromImage(img)) {
				g.DrawImage(source, 0, 0, box.Width, box.Height + 1); // box
			}
			source.Dispose();

			//Bitmap source = BuildTile();
			//Bitmap tile = new Bitmap((int)(((float)box.Height * (float)source.Width) / (float)source.Height), box.Height);
			//using (Graphics g = Graphics.FromImage(tile)) {
			//    g.DrawImage(source, 0, 0, tile.Width, tile.Height);
			//}
			//source.Dispose();
			
			//img = new Bitmap(box.Width, box.Height);
			//using (Graphics g = Graphics.FromImage(img)) {
			//    int i = 0;
			//    while (i < box.Width) {
			//        g.DrawImageUnscaled(tile, i, 0);
			//        i += tile.Width;
			//    }
			//}
		}

		private Bitmap BuildTile(int width) {
			Bitmap bmp = new Bitmap(width, 9);
			Graphics g = Graphics.FromImage(bmp);

			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(245, 245, 245))), new Point(0, 0), new Point(width - 1, 0));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(236, 236, 236))), new Point(0, 1), new Point(width - 1, 1));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(234, 234, 235))), new Point(0, 2), new Point(width - 1, 2));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(234, 234, 235))), new Point(0, 3), new Point(width - 1, 3));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(222, 222, 222))), new Point(0, 4), new Point(width - 1, 4));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(229, 229, 230))), new Point(0, 5), new Point(width - 1, 5));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(239, 239, 239))), new Point(0, 6), new Point(width - 1, 6));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(247, 247, 247))), new Point(0, 7), new Point(width - 1, 7));
			g.DrawLine(new Pen(new SolidBrush(Color.FromArgb(254, 254, 255))), new Point(0, 8), new Point(width - 1, 8));

			g.Dispose();
			return bmp;
		}
	}
}