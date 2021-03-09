using System;
using System.ComponentModel;
using System.Globalization;

namespace DivideToolControls.DeepZoom
{
	public class DeepZoomImageTileSourceConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null)
			{
				try
				{
					return new DeepZoomImageTileSource(new Uri(text, UriKind.RelativeOrAbsolute));
				}
				catch (Exception ex)
				{
					throw new Exception($"Cannot convert '{value}' ({value.GetType()}) - {ex.Message}", ex);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			DeepZoomImageTileSource deepZoomImageTileSource = value as DeepZoomImageTileSource;
			if (deepZoomImageTileSource != null && CanConvertTo(context, destinationType))
			{
				Uri uriSource = deepZoomImageTileSource.UriSource;
				if (!uriSource.IsAbsoluteUri)
				{
					return uriSource.OriginalString;
				}
				return uriSource.AbsoluteUri;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
