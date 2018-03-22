using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	public enum ProgressType {
		Smooth, MarqueeWrap, MarqueeBounce, MarqueeBounceDeep, Animated
	}

	/// <summary></summary>
	public abstract class AbstractProgressBar : Control {
		protected int minimum = 0;
		protected int maximum = 100;
		protected int value = 0;
		protected Rectangle borderbox;
		protected Rectangle progressbox;
		protected Rectangle backbox;
		private bool showPercent = false;
		private int padding = 0;
		#region Marquee
		protected ProgressType type = ProgressType.Smooth;
		protected int marqueeSpeed = 30;
		protected int marqueePercentage = 25;
		protected int marqueeStep = 1;
		#endregion

		protected EventHandler OnValueChanged;
		/// <summary></summary>
		public event EventHandler ValueChanged {
			add {
				if (OnValueChanged != null) {
					foreach (Delegate d in OnValueChanged.GetInvocationList()) {
						if (object.ReferenceEquals(d, value)) { return; }
					}
				}
				OnValueChanged = (EventHandler)Delegate.Combine(OnValueChanged, value);
			}
			remove { OnValueChanged = (EventHandler)Delegate.Remove(OnValueChanged, value); }
		}

		public AbstractProgressBar() {
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets whether or not to draw the percentage text"), Browsable(true)]
		public bool ShowPercentage {
			get { return showPercent; }
			set {
				showPercent = value;
				Invalidate();
				if (!showPercent) {
					this.Text = "";
				}
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the minimum value"), Browsable(true)]
		public virtual int Minimum {
			get { return this.minimum; }
			set {
				if (value > maximum) { throw new ArgumentException("Minimum must be smaller than maximum."); }
				this.minimum = value;
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the maximum value"), Browsable(true)]
		public virtual int Maximum {
			get { return this.maximum; }
			set {
				if (value < minimum) { throw new ArgumentException("Maximum must be larger than minimum."); }
				this.maximum = value;
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the current value"), Browsable(true)]
		public int Value {
			get { return this.value; }
			set {
				if (value < minimum) { throw new ArgumentException("Value must be greater than or equal to minimum."); }
				if (value > maximum) { throw new ArgumentException("Value must be less than or equal to maximum."); }
				this.value = value;
				if (showPercent) {
					int percent = (int)(((float)this.value / (float)(this.maximum - 1f)) * 100f);
					if (percent > 0) {
						if (percent > 100) { percent = 100; }
						this.Text = string.Format("{0}%", percent.ToString());
					} else { this.Text = ""; }
				}
				if (OnValueChanged != null) {
					OnValueChanged(this, EventArgs.Empty);
				}
				ResizeProgress();
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the number of pixels to pad between the border and progress"), Browsable(true)]
		public int ProgressPadding {
			get { return this.padding; }
			set {
				this.padding = value;
				if (OnValueChanged != null) {
					OnValueChanged(this, EventArgs.Empty);
				}
				//ResizeProgress();
				OnResize(EventArgs.Empty);
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the type of progress"), Browsable(true)]
		public virtual ProgressType ProgressType {
			get { return this.type; }
			set { this.type = value; }
		}

		#region Marquee
		/// <summary></summary>
		[Category("Marquee"), Description("Gets or sets the number of milliseconds between marquee steps"), Browsable(true)]
		public int MarqueeSpeed {
			get { return this.marqueeSpeed; }
			set {
				this.marqueeSpeed = value;
				if (this.marqueeSpeed < 10) { this.marqueeSpeed = 10; }
			}
		}

		/// <summary></summary>
		[Category("Marquee"), Description("Gets or sets the number of pixels to progress the marquee bar"), Browsable(true)]
		public int MarqueeStep {
			get { return this.marqueeStep; }
			set { this.marqueeStep = value; }
		}

		/// <summary></summary>
		[Category("Marquee"), Description("Gets or sets the percentage of the width that the marquee progress fills"), Browsable(true)]
		public int MarqueePercentage {
			get { return this.marqueePercentage; }
			set {
				if (value < 5 || value > 95) {
					throw new ArgumentException("Marquee percentage width must be between 5% and 95%.");
				}
				this.marqueePercentage = value;
			}
		}
		#endregion

		/// <summary></summary>
		[Browsable(false)]
		public Rectangle BorderBox {
			get { return this.borderbox; }
		}

		/// <summary></summary>
		[Browsable(false)]
		public Rectangle BackBox {
			get { return this.backbox; }
		}

		/// <summary></summary>
		[Browsable(false)]
		public Rectangle ProgressBox {
			get { return this.progressbox; }
		}

		/// <summary></summary>
		/// <param name="gr"></param>
		protected abstract void PaintBackground(Graphics gr);

		/// <summary></summary>
		/// <param name="gr"></param>
		protected abstract void PaintProgress(Graphics gr);

		/// <summary></summary>
		/// <param name="gr"></param>
		protected abstract void PaintText(Graphics gr);

		/// <summary></summary>
		/// <param name="gr"></param>
		protected abstract void PaintBorder(Graphics gr);

		/// <summary></summary>
		protected abstract void ResizeProgress();

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			borderbox = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
			backbox = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
			ResizeProgress();
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			PaintBackground(e.Graphics);
			PaintProgress(e.Graphics);
			e.Graphics.Clip = new Region(new Rectangle(0, 0, this.Width, this.Height));
			PaintText(e.Graphics);
			PaintBorder(e.Graphics);
		}

		/// <summary></summary>
		public abstract void MarqueeStart();
		/// <summary></summary>
		public abstract void MarqueePause();
		/// <summary></summary>
		public abstract void MarqueeStop();
	}
}