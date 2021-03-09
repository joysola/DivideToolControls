using System.Windows;

namespace DivideToolControls.DeepZoom
{
	public struct DisplayRect
	{
		public Rect Rect
		{
			get;
			set;
		}

		public int MinLevel
		{
			get;
			set;
		}

		public int MaxLevel
		{
			get;
			set;
		}

		public DisplayRect(double x, double y, double width, double height, int minLevel, int maxLevel)
		{
			this = default(DisplayRect);
			Rect = new Rect(x, y, width, height);
			MinLevel = minLevel;
			MaxLevel = maxLevel;
		}
	}
}
