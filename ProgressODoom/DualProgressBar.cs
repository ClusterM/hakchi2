using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.DualProgressBar), "Icons.DualProgressBar.ico")]
	public class DualProgressBar : ProgressBarEx {
		private int masterval = 0;
		private int mastermax = 100;
		private IProgressPainter masterpainter;
		private bool masterBottom = false;
		private Rectangle masterbox;
		private int padding = 0;

		protected EventHandler OnMasterValueChanged;
		/// <summary></summary>
		public event EventHandler MasterValueChanged {
			add {
				if (OnMasterValueChanged != null) {
					foreach (Delegate d in OnMasterValueChanged.GetInvocationList()) {
						if (object.ReferenceEquals(d, value)) { return; }
					}
				}
				OnMasterValueChanged = (EventHandler)Delegate.Combine(OnMasterValueChanged, value);
			}
			remove { OnMasterValueChanged = (EventHandler)Delegate.Remove(OnMasterValueChanged, value); }
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the maximum value"), Browsable(true)]
		public override int Maximum {
			get { return base.maximum; }
			set {
				base.Maximum = value;
				mastermax = value;
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the value of the master progress"), Browsable(true)]
		public int MasterValue {
			get { return this.masterval; }
			set {
				this.masterval = value;
				if (OnMasterValueChanged != null) {
					OnMasterValueChanged(this, EventArgs.Empty);
				}
				ResizeMasterProgress();
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the maximum value for the master progress"), Browsable(true)]
		public int MasterMaximum {
			get { return mastermax; }
			set {
				this.mastermax = value;
				ResizeMasterProgress();
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the padding for the master progress"), Browsable(true)]
		public int MasterProgressPadding {
			get { return this.padding; }
			set {
				this.padding = value;
				if (OnValueChanged != null) {
					OnValueChanged(this, EventArgs.Empty);
				}
				ResizeMasterProgress();
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Painters"), Description("Paints this progress bar's master progress"), Browsable(true)]
		public IProgressPainter MasterPainter {
			get { return this.masterpainter; }
			set {
				this.masterpainter = value;
				if (this.masterpainter is AbstractProgressPainter) {
					((AbstractProgressPainter)this.masterpainter).padding = base.ProgressPadding;
				}
				this.masterpainter.PropertiesChanged += new EventHandler(component_PropertiesChanged);
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Determines whether or not the master progress is painted under the main progress"), Browsable(true)]
		public bool PaintMasterFirst {
			get { return this.masterBottom; }
			set {
				this.masterBottom = value;
				this.Invalidate();
			}
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ResizeProgress();
			ResizeMasterProgress();
			if (this.backgroundpainter != null) { this.backgroundpainter.Resize(borderbox); }
			if (masterBottom && this.masterpainter != null) { this.masterpainter.Resize(masterbox); }
			if (this.progresspainter != null) { this.progresspainter.Resize(borderbox); }
			if (!masterBottom && this.masterpainter != null) { this.masterpainter.Resize(masterbox); }
			if (this.borderpainter != null) { this.borderpainter.Resize(borderbox); }
		}

		private void ResizeMasterProgress() {
			Rectangle newprog = base.borderbox;
			newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);
			base.backbox = newprog;

			int val = masterval; if (val > 0) { val++; }
			int progWidth = mastermax > 0 ? (backbox.Width * val / mastermax) : 1;
			if (value >= mastermax && mastermax > 0) {
				progWidth = backbox.Width;
			} /*else if (value > 0) {
				progWidth++;
			}*/
			//newprog = new Rectangle(backbox.X + base.ProgressPadding, backbox.Y + base.ProgressPadding, progWidth - (base.ProgressPadding * 2), backbox.Height - (base.ProgressPadding * 2));
			//newprog = new Rectangle(backbox.X, backbox.Y, progWidth, backbox.Height);
			newprog = new Rectangle(backbox.X + this.padding, backbox.Y + this.padding, progWidth - (this.padding * 2), backbox.Height - (this.padding * 2));
			masterbox = newprog;
		}

		///// <summary></summary>
		//protected override void MarqueeStart() {
		//}
		///// <summary></summary>
		//protected override void MarqueePause() {
		//}
		///// <summary></summary>
		//protected override void MarqueeStop() {
		//}

		/// <summary></summary>
		/// <param name="gr"></param>
		protected override void PaintProgress(Graphics g) {
			if (this.progresspainter != null) {
				if (masterBottom && this.masterpainter != null) {
					this.masterpainter.PaintProgress(masterbox, g);
				}
				this.progresspainter.PaintProgress(progressbox, g);
				if (!masterBottom && this.masterpainter != null) {
					this.masterpainter.PaintProgress(masterbox, g);
				}
			}
		}
	}
}