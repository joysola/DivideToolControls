using DivideToolControls.Helper;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace DivideToolControls.DeepZoom
{
	public static class ImageLoader
	{
		public static BitmapSource LoadImage(Uri uri)
		{
			try
			{
				BitmapImage bitmapImage = new BitmapImage();
				MemoryStream streamSource;
				using (WebClient webClient = new WebClient())
				{
					byte[] buffer = webClient.DownloadData(uri);
					streamSource = new MemoryStream(buffer);
				}
				bitmapImage.BeginInit();
				bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				bitmapImage.CacheOption = BitmapCacheOption.None;
				bitmapImage.StreamSource = streamSource;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				return bitmapImage;
			}
			catch (WebException)
			{
			}
			catch (FileNotFoundException)
			{
			}
			catch (FileFormatException)
			{
			}
			return null;
		}

		public static BitmapSource LoadImage2(string KfbioAddress, string zoom, string posx, string posy)
		{
			try
			{
				MemoryStream memoryStream = null;
				int nDataLength = 0;
				IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
				k.DataFilePTR = int.Parse(KfbioAddress);
				DllImageFuc.GetImageStreamFunc(ref k, float.Parse(zoom), int.Parse(posx), int.Parse(posy), ref nDataLength, out IntPtr datas);
				byte[] array = new byte[nDataLength];
				if (datas != IntPtr.Zero)
				{
					Marshal.Copy(datas, array, 0, nDataLength);
				}
				DllImageFuc.DeleteImageDataFunc(datas);
				BitmapImage bitmapImage = new BitmapImage();
				memoryStream = new MemoryStream(array);
				bitmapImage.BeginInit();
				bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				bitmapImage.CacheOption = BitmapCacheOption.None;
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				return bitmapImage;
			}
			catch (FileNotFoundException)
			{
			}
			catch (FileFormatException)
			{
			}
			return null;
		}
	}
}
