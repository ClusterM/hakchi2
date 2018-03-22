using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProgressODoom {
	[ToolboxBitmapAttribute(typeof(ProgressODoom.RarProgressBar), "Icons.RarProgressBar.ico")]
	public class RarProgressBar : DualProgressBar {
		public RarProgressBar() {
			this.MasterPainter = new RarProgressPainter(RarProgressPainter.RarProgressType.Silver);
			((RarProgressPainter)this.MasterPainter).ShowEdge = false;
			this.ProgressPainter = new RarProgressPainter(RarProgressPainter.RarProgressType.Gold);
			((RarProgressPainter)this.ProgressPainter).ShowEdge = true;
			this.BorderPainter = new RarBorderPainter();
			this.BackgroundPainter = new RarBackgroundPainter();
			this.PaintMasterFirst = true;
			this.OnValueChanged += new EventHandler(onValueChanged);
			this.OnMasterValueChanged += new EventHandler(onValueChanged);
		}

		private void onValueChanged(object sender, EventArgs e) {
			bool masterAhead = false;
			if (this.MasterValue > this.Value) {
				masterAhead = true;
			}
			((RarProgressPainter)this.MasterPainter).ShowEdge = masterAhead;
			((RarProgressPainter)this.ProgressPainter).ShowEdge = !masterAhead;
		}
	}
}