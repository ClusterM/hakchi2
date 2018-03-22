using System;
using System.Drawing;

namespace ProgressODoom {
	public interface IGlossPainter : IDisposable {
		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="g"></param>
		void PaintGloss(Rectangle box, Graphics g);

		/// <summary></summary>
		void Resize(Rectangle box);

		/// <summary></summary>
		event EventHandler PropertiesChanged;
	}
}