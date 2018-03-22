using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	/// <summary></summary>
	[ToolboxBitmapAttribute(typeof(ProgressODoom.ProgressBarEx), "Icons.ProgressBarEx.ico")]
	public class ProgressBarEx : AbstractProgressBar {
		protected IProgressBackgroundPainter backgroundpainter;
		protected IProgressPainter progresspainter;
		protected IProgressBorderPainter borderpainter;

		public ProgressBarEx() {
			backgroundpainter = new PlainBackgroundPainter();
			progresspainter = new PlainProgressPainter(Color.Gold);
			borderpainter = new PlainBorderPainter();
		}

		/// <summary></summary>
		[Category("Painters"), Description("Paints this progress bar's background"), Browsable(true)]
		public IProgressBackgroundPainter BackgroundPainter {
			get { return this.backgroundpainter; }
			set {
				this.backgroundpainter = value;
				this.backgroundpainter.PropertiesChanged += new EventHandler(component_PropertiesChanged);
				this.Invalidate();
			}
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void component_PropertiesChanged(object sender, EventArgs e) {
			this.Invalidate();
		}

		/// <summary></summary>
		[Category("Painters"), Description("Paints this progress bar's progress"), Browsable(true)]
		public IProgressPainter ProgressPainter {
			get { return this.progresspainter; }
			set {
				if (!(value is IAnimatedProgressPainter) && base.ProgressType == ProgressType.Animated) {
					base.ProgressType = ProgressType.Smooth;
				}
				this.progresspainter = value;
				if (this.progresspainter is AbstractProgressPainter) {
					((AbstractProgressPainter)this.progresspainter).padding = base.ProgressPadding;
				}
				this.progresspainter.PropertiesChanged += new EventHandler(component_PropertiesChanged);
				this.Invalidate();
			}
		}

		/// <summary></summary>
		[Category("Progress"), Description("Gets or sets the type of progress"), Browsable(true)]
		public override ProgressType ProgressType {
			get { return base.type; }
			set {
				if (value == ProgressType.Animated && !(progresspainter is IAnimatedProgressPainter)) {
					throw new ArgumentException("Animated is not available with the current Progress Painter");
				}
				this.type = value;
			}
		}

		/// <summary></summary>
		[Category("Painters"), Description("Paints this progress bar's border"), Browsable(true)]
		public IProgressBorderPainter BorderPainter {
			get { return this.borderpainter; }
			set {
				this.borderpainter = value;
				this.borderpainter.PropertiesChanged += new EventHandler(component_PropertiesChanged);
				ResizeProgress();
				this.Invalidate();
			}
		}

		protected override void ResizeProgress() {
			if (base.ProgressType != ProgressType.Smooth) { return; }
			Rectangle newprog = base.borderbox;
			//newprog.Inflate(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);
			base.backbox = newprog;

			int val = value; if (val > 0) { val++; }
			int progWidth = maximum > 0 ? (backbox.Width * val / maximum) : 1;
			if (value >= maximum && maximum > 0) {
				progWidth = backbox.Width;
			} /*else if (value > 0) {
				progWidth++;
			}*/
			newprog.Inflate(-base.ProgressPadding, -base.ProgressPadding);
			newprog.Width = progWidth - (base.ProgressPadding * 2);
			//newprog.Offset(base.ProgressPadding, base.ProgressPadding);
			//newprog = new Rectangle(backbox.X + base.ProgressPadding, backbox.Y + base.ProgressPadding, progWidth - (base.ProgressPadding * 2), backbox.Height - (base.ProgressPadding * 2));
			base.progressbox = newprog;
		}

		#region Animation
		public void StartAnimation() {
			if (running) { return; }
			IAnimatedProgressPainter iapp = this.progresspainter as IAnimatedProgressPainter;
			if (iapp == null) { return; }
			iapp.Animating = true;
			running = true;
			timerMethod = new EventHandler(DoAnimation);
			timer.Interval = iapp.AnimationSpeed;
			timer.Tick += timerMethod;
			timer.Enabled = true;
		}
		public void StopAnimation() {
			timer.Enabled = false;
			timer.Tick -= timerMethod;
			running = false;
			IAnimatedProgressPainter iapp = this.progresspainter as IAnimatedProgressPainter;
			if (iapp == null) { return; }
			iapp.Animating = false;
		}
		private void DoAnimation(object sender, EventArgs e) {
			IAnimatedProgressPainter iapp = this.progresspainter as IAnimatedProgressPainter;
			if (iapp == null) { return; }

			//Rectangle newprog = base.borderbox;
			//newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			//newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);
			////int progWidth = (int)(((float)marqueePercentage * (float)backbox.Width) / 100f);
			//int progWidth = (int)(((float)marqueePercentage * (float)backbox.Width) / 100f);
			//newprog.Inflate(-base.ProgressPadding, -base.ProgressPadding);
			//newprog.Width = progWidth - (base.ProgressPadding * 2);

			//base.progressbox = newprog;

			////iapp.AnimateFrame(newprog, g, ref marqueeX);

			this.Invalidate();
			this.Refresh();
		}
		#endregion

		#region Marquee
		private bool running = false;
		private Timer timer = new Timer();
		private EventHandler timerMethod;
		/// <summary></summary>
		public override void MarqueeStart() {
			if (running) { return; }
			running = true;
			switch (base.ProgressType) {
				case ProgressType.MarqueeWrap: timerMethod = new EventHandler(DoMarqueeWrap); break;
				case ProgressType.MarqueeBounce: timerMethod = new EventHandler(DoMarqueeBounce); break;
				case ProgressType.MarqueeBounceDeep: timerMethod = new EventHandler(DoMarqueeDeep); break;
			}
			timer.Interval = base.marqueeSpeed;
			timer.Tick += timerMethod;
			timer.Enabled = true;
		}

		private int marqueeX = 0;
		private void DoMarqueeWrap(object sender, EventArgs e) {
			Rectangle newprog = base.borderbox;
			newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);

			int progWidth = (int)(((float)marqueePercentage * (float)backbox.Width) / 100f);

			marqueeX += marqueeStep;
			if (marqueeX > backbox.Width) {
				marqueeX = 0 - progWidth;
			}

			newprog.Inflate(-base.ProgressPadding, -base.ProgressPadding);
			newprog.Width = progWidth - (base.ProgressPadding * 2);
			newprog.X += marqueeX;

			int leftBoundry = backbox.X + this.borderpainter.BorderWidth + base.ProgressPadding;
			int rightBoundry = backbox.X + backbox.Width - (this.borderpainter.BorderWidth + base.ProgressPadding);
			if (marqueeX <= leftBoundry) {
				newprog.Width -= leftBoundry - marqueeX;
				newprog.X = leftBoundry;
			} else if (marqueeX + newprog.Width >= rightBoundry) {
				newprog.Width -= (marqueeX + newprog.Width + base.ProgressPadding) - rightBoundry;
			}

			base.progressbox = newprog;

			this.Invalidate();
			this.Refresh();
		}
		private bool marqueeForward = true;
		private void DoMarqueeBounce(object sender, EventArgs e) {
			Rectangle newprog = base.borderbox;
			newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);

			int progWidth = (int)(((float)marqueePercentage * (float)backbox.Width) / 100f);

			if (marqueeForward) {
				marqueeX += marqueeStep;
			} else {
				marqueeX -= marqueeStep;
			}

			newprog.Inflate(-base.ProgressPadding, -base.ProgressPadding);
			newprog.Width = progWidth - (base.ProgressPadding * 2);
			newprog.X += marqueeX;

			int leftBoundry = backbox.X + this.borderpainter.BorderWidth + base.ProgressPadding;
			int rightBoundry = backbox.X + backbox.Width - (this.borderpainter.BorderWidth + base.ProgressPadding);
			if (marqueeX + progWidth >= rightBoundry) {
				marqueeForward = false;
			} else if (marqueeX <= leftBoundry) {
				marqueeForward = true;
			}

			base.progressbox = newprog;

			this.Invalidate();
			this.Refresh();
		}
		private void DoMarqueeDeep(object sender, EventArgs e) {
			Rectangle newprog = base.borderbox;
			newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);

			int progWidth = (int)(((float)marqueePercentage * (float)backbox.Width) / 100f);

			if (marqueeForward) {
				marqueeX += marqueeStep;
			} else {
				marqueeX -= marqueeStep;
			}
			if (marqueeX > backbox.Width) {
				marqueeForward = false;
			} else if (marqueeX < backbox.X - progWidth) {
				marqueeForward = true;
			}

			newprog.Inflate(-base.ProgressPadding, -base.ProgressPadding);
			newprog.Width = progWidth - (base.ProgressPadding * 2);
			newprog.X += marqueeX;

			int leftBoundry = backbox.X + this.borderpainter.BorderWidth + base.ProgressPadding;
			int rightBoundry = backbox.X + backbox.Width - (this.borderpainter.BorderWidth + base.ProgressPadding);
			if (marqueeX <= leftBoundry) {
				newprog.Width -= leftBoundry - marqueeX;
				newprog.X = leftBoundry;
			} else if (marqueeX + newprog.Width >= rightBoundry) {
				newprog.Width -= (marqueeX + newprog.Width + base.ProgressPadding) - rightBoundry;
			}

			base.progressbox = newprog;

			this.Invalidate();
			this.Refresh();
		}

		/// <summary></summary>
		public override void MarqueePause() {
			running = false;
			timer.Enabled = false;
			timer.Tick -= timerMethod;
		}
		/// <summary></summary>
		public override void MarqueeStop() {
			Rectangle newprog = base.borderbox;
			newprog.Offset(this.borderpainter.BorderWidth, this.borderpainter.BorderWidth);
			newprog.Size = new Size(newprog.Size.Width - this.borderpainter.BorderWidth, newprog.Size.Height - this.borderpainter.BorderWidth);

			newprog.Inflate(-base.ProgressPadding, -base.ProgressPadding);
			newprog.Width = 1;
			base.progressbox = newprog;

			running = false;
			timer.Enabled = false;
			timer.Tick -= timerMethod;

			marqueeX = 0;
			this.Invalidate();
		}
		#endregion

		/// <summary></summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (running) { running = false; }
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ResizeProgress();
			if (this.backgroundpainter != null) { this.backgroundpainter.Resize(borderbox); }
			if (this.progresspainter != null) { this.progresspainter.Resize(borderbox); }
			if (this.borderpainter != null) { this.borderpainter.Resize(borderbox); }
		}

		/// <summary></summary>
		/// <param name="gr"></param>
		protected override void PaintBackground(Graphics g) {
			if (this.backgroundpainter != null) {
				this.backgroundpainter.PaintBackground(backbox, g);
			}
		}

		/// <summary></summary>
		/// <param name="gr"></param>
		protected override void PaintProgress(Graphics g) {
			if (this.progresspainter != null) {
				this.progresspainter.PaintProgress(progressbox, g);
			}
		}

		/// <summary></summary>
		/// <param name="gr"></param>
		protected override void PaintText(Graphics g) {
			if (base.ProgressType != ProgressType.Smooth) { return; }
			Brush b = new SolidBrush(ForeColor);
			SizeF sf = g.MeasureString(Text, Font, Convert.ToInt32(Width), StringFormat.GenericDefault);
			float m = sf.Width;
			float x = (Width / 2) - (m / 2);
			float w = (Width / 2) + (m / 2);
			float h = (float)borderbox.Height - (2f * (float)this.borderpainter.BorderWidth);
			float y = (float)this.borderpainter.BorderWidth + ((h - sf.Height) / 2f);
			g.DrawString(Text, Font, b, RectangleF.FromLTRB(x, y, w, Height - 1), StringFormat.GenericDefault);
		}

		/// <summary></summary>
		/// <param name="gr"></param>
		protected override void PaintBorder(Graphics g) {
			if (this.borderpainter != null) {
				this.borderpainter.PaintBorder(borderbox, g);
			}
		}
	}
}