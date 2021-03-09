using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace DivideToolControls.DeepZoom
{
	public class DeepZoomImageTileSource : MultiScaleTileSource
	{
		private string _imageExtension;

		private IList<DisplayRect> _displayRects;

		public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register("UriSource", typeof(Uri), typeof(DeepZoomImageTileSource), new FrameworkPropertyMetadata(null, OnUriSourceChanged));

		public Uri UriSource
		{
			get
			{
				return (Uri)GetValue(UriSourceProperty);
			}
			set
			{
				SetValue(UriSourceProperty, value);
			}
		}

		public event EventHandler UriSourceChanged;

		public DeepZoomImageTileSource(Uri sourceUri)
		{
			UriSource = sourceUri;
		}

		public override void GetTileLayersAngle(ref double CenterX, ref double CenterY, ref double Angle, ref double OffsetX, ref double OffsetY)
		{
		}

		private static void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DeepZoomImageTileSource deepZoomImageTileSource = (DeepZoomImageTileSource)d;
			Uri oldUriSource = (Uri)e.OldValue;
			Uri uriSource = deepZoomImageTileSource.UriSource;
			deepZoomImageTileSource.OnUriSourceChanged(oldUriSource, uriSource);
		}

		protected virtual void OnUriSourceChanged(Uri oldUriSource, Uri newUriSource)
		{
			LoadDeepZoomXml();
			InitializeTileSource();
			if (this.UriSourceChanged != null)
			{
				this.UriSourceChanged(this, EventArgs.Empty);
			}
		}

		protected internal override object GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY)
		{
			if (!TileExists(tileLevel, tilePositionX, tilePositionY))
			{
				return null;
			}
			string originalString = UriSource.OriginalString;
			string uriString = originalString.Substring(0, originalString.Length - 4) + "_files/" + tileLevel + "/" + tilePositionX + "_" + tilePositionY + "." + _imageExtension;
			return new Uri(uriString, UriSource.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
		}

		private void LoadDeepZoomXml()
		{
			XElement xElement = XElement.Load(UriSource.OriginalString);
			if (xElement == null)
			{
				throw new FileFormatException("Invalid XML file.");
			}
			XNamespace xmlns = xElement.GetDefaultNamespace();
			base.TileSize = (int)xElement.Attribute("TileSize");
			base.TileOverlap = (int)xElement.Attribute("Overlap");
			_imageExtension = (string)xElement.Attribute("Format");
			XElement xElement2 = xElement.Element(xmlns + "Size");
			if (xElement2 == null)
			{
				throw new FileFormatException("Invalid XML file.");
			}
			base.ImageSize = new Size((int)xElement2.Attribute("Width"), (int)xElement2.Attribute("Height"));
			XElement xElement3 = xElement.Element(xmlns + "DisplayRects");
			if (xElement3 != null)
			{
				_displayRects = xElement3.Elements(xmlns + "DisplayRect").Select(delegate (XElement el)
				{
					XElement xElement4 = el.Element(xmlns + "Rect");
					double x = (double)xElement4.Attribute("X");
					double y = (double)xElement4.Attribute("Y");
					double width = (double)xElement4.Attribute("Width");
					double height = (double)xElement4.Attribute("Height");
					int minLevel = (int)el.Attribute("MinLevel");
					int maxLevel = (int)el.Attribute("MaxLevel");
					return new DisplayRect(x, y, width, height, minLevel, maxLevel);
				}).ToList();
			}
		}

		private bool TileExists(int level, int column, int row)
		{
			if (_displayRects == null)
			{
				return true;
			}
			double num = ScaleAtLevel(level);
			foreach (DisplayRect item in _displayRects.Where((DisplayRect r) => level >= r.MinLevel && level <= r.MaxLevel))
			{
				double num2 = item.Rect.X * num;
				double num3 = item.Rect.Y * num;
				double num4 = num2 + item.Rect.Width * num;
				double num5 = num3 + item.Rect.Height * num;
				num2 = Math.Floor(num2 / (double)base.TileSize);
				num3 = Math.Floor(num3 / (double)base.TileSize);
				num4 = Math.Ceiling(num4 / (double)base.TileSize);
				num5 = Math.Ceiling(num5 / (double)base.TileSize);
				if (num2 <= (double)column && (double)column < num4 && num3 <= (double)row && (double)row < num5)
				{
					return true;
				}
			}
			return false;
		}
	}
}
