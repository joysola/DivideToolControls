using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DivideToolControls.Extensions
{
	public static class RectExtension
	{
		public static Point GetCenter(this Rect rect)
		{
			return new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);
		}

		public static bool IsDefined(this Rect rect)
		{
			if (rect.Width >= 0.0 && rect.Height >= 0.0 && rect.Top < double.PositiveInfinity && rect.Left < double.PositiveInfinity && (rect.Top > double.NegativeInfinity || rect.Height == double.PositiveInfinity))
			{
				if (!(rect.Left > double.NegativeInfinity))
				{
					return rect.Width == double.PositiveInfinity;
				}
				return true;
			}
			return false;
		}

		public static bool Intersects(this Rect self, Rect rect)
		{
			if (!self.IsEmpty && !rect.IsEmpty)
			{
				if ((self.Width == double.PositiveInfinity || self.Right >= rect.Left) && (rect.Width == double.PositiveInfinity || rect.Right >= self.Left) && (self.Height == double.PositiveInfinity || self.Bottom >= rect.Top))
				{
					if (rect.Height != double.PositiveInfinity)
					{
						return rect.Bottom >= self.Top;
					}
					return true;
				}
				return false;
			}
			return true;
		}
	}
}
