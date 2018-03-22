using System;
using System.Drawing;

namespace ProgressODoom {
	/// <summary></summary>
	public interface IProgressBackgroundPainter : IDisposable {
		/// <summary></summary>
		IGlossPainter GlossPainter { get; set; }

		/// <summary></summary>
		/// <param name="box"></param>
		/// <param name="gr"></param>
		void PaintBackground(Rectangle box, Graphics gr);

		/// <summary></summary>
		void Resize(Rectangle box);

		/// <summary></summary>
		event EventHandler PropertiesChanged;
	}
}