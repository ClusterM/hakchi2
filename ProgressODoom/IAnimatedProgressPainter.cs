using System;
using System.Drawing;

namespace ProgressODoom {
	/// <summary></summary>
	public interface IAnimatedProgressPainter : IProgressPainter {
		///// <summary></summary>
		///// <param name="box"></param>
		///// <param name="g"></param>
		///// <param name="marqueeX"></param>
		//void AnimateFrame(Rectangle box, Graphics g, ref int marqueeX);

		/// <summary></summary>
		int AnimationSpeed { get; set; }

		/// <summary></summary>
		bool Animating { get; set; }
	}
}