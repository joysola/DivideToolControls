using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DivideToolControls.DeepZoom
{
	internal class VisualTile : INotifyPropertyChanged
	{
		private ImageSource _source;

		public int ZIndex
		{
			get;
			private set;
		}

		public double Left
		{
			get;
			private set;
		}

		public double Top
		{
			get;
			private set;
		}

		public double CenterX
		{
			get;
			set;
		}

		public double CenterY
		{
			get;
			set;
		}

		public Point OriginPoint
		{
			get;
			set;
		}

		public double Angle
		{
			get;
			set;
		}

		public double Scale
		{
			get;
			private set;
		}

		public string FName
		{
			get;
			set;
		}

		public string ID
		{
			get;
			set;
		}

		public ImageSource Source
		{
			get
			{
				return _source;
			}
			internal set
			{
				_source = value;
				RaisePropertyChanged("Source");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public VisualTile(Tile tile, MultiScaleTileSource tileSource)
		{
			ZIndex = tile.Level;
			Scale = 1.0 / tileSource.ScaleAtLevel(tile.Level);
			Point tilePosition = tileSource.GetTilePosition(tile.Column, tile.Row);
			Left = tilePosition.X * Scale;
			Top = tilePosition.Y * Scale;
		}

		public VisualTile(Tile tile, MultiScaleTileSource tileSource, ImageSource source)
			: this(tile, tileSource)
		{
			Source = source;
		}

		protected void RaisePropertyChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
