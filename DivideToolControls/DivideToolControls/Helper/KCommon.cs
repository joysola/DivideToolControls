using DivideToolControls.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DivideToolControls.Helper
{
	internal static class KCommon
	{
		private static PerformanceCounter p1 = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);

		[DllImport("kernel32.dll")]
		public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

		public static void FlushMemory()
		{
			GC.Collect();
			GC.WaitForFullGCComplete();
		}

		public static bool CheckVersion(string filepath)
		{
			try
			{
				int num = 11;
				try
				{
					float f = 0f;
					DllImageFuc.GetDllVersionFunc(ref f);
					num = (int)(Math.Round(f, 1) * 10.0);
				}
				catch
				{
				}
				DllImageFuc dllImageFuc = new DllImageFuc();
				IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
				k.DataFilePTR = 0;
				dllImageFuc.CkInitImageFileFunc(ref k, filepath);
				float fScale = 0f;
				dllImageFuc.CkGetVersionInfoFunc(ref k, ref fScale);
				int num2 = (int)(fScale * 10f);
				if (num2 > num)
				{
					//MessageWind messageWind = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, ((MainWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Filedamage2"], ((MainWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
					//messageWind.ShowDialog();
					return false;
				}
				if (num2 == 0)
				{
					//MessageWind messageWind2 = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, ((MainWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Filedamage"], ((MainWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
					//messageWind2.ShowDialog();
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				System.Windows.MessageBox.Show(e.Message);
				//MessageWind messageWind3 = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, ((MainWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Filedamage2"], ((MainWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
				//messageWind3.ShowDialog();
				return false;
			}
		}

		public static BitmapImage GetKFBThumnail(string path)
		{
			BitmapImage bitmapImage = new BitmapImage();
			try
			{
				DllImageFuc dllImageFuc = new DllImageFuc();
				IntPtr datas = IntPtr.Zero;
				int b = 0;
				int c = 0;
				int a = 0;
				dllImageFuc.CkGetThumnailImagePathFunc(path, out datas, ref a, ref b, ref c);
				byte[] array = new byte[a];
				if (datas != IntPtr.Zero)
				{
					Marshal.Copy(datas, array, 0, a);
				}
				DllImageFuc.DeleteImageDataFunc(datas);
				MemoryStream ss = new MemoryStream(array);
				bitmapImage.BeginInit();
				bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				bitmapImage.CacheOption = BitmapCacheOption.None;
				bitmapImage.StreamSource = ReizeImage(ss, 20.0);
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				return bitmapImage;
			}
			catch
			{
				return bitmapImage;
			}
		}

		public static BitmapImage GetKFBThumnail2(string path)
		{
			DllImageFuc dllImageFuc = new DllImageFuc();
			IntPtr datas = IntPtr.Zero;
			int b = 0;
			int c = 0;
			int a = 0;
			dllImageFuc.CkGetThumnailImagePathFunc(path, out datas, ref a, ref b, ref c);
			byte[] array = new byte[a];
			if (datas != IntPtr.Zero)
			{
				Marshal.Copy(datas, array, 0, a);
			}
			DllImageFuc.DeleteImageDataFunc(datas);
			MemoryStream streamSource = new MemoryStream(array);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			bitmapImage.CacheOption = BitmapCacheOption.None;
			bitmapImage.StreamSource = streamSource;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}

		public static BitmapImage GetKFBLabel(string path)
		{
			IntPtr datas = IntPtr.Zero;
			int b = 0;
			int c = 0;
			int a = 0;
			DllImageFuc.GetLableInfoPathFunc(path, out datas, ref a, ref b, ref c);
			byte[] array = new byte[a];
			if (datas != IntPtr.Zero)
			{
				Marshal.Copy(datas, array, 0, a);
			}
			DllImageFuc.DeleteImageDataFunc(datas);
			MemoryStream streamSource = new MemoryStream(array);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			bitmapImage.CacheOption = BitmapCacheOption.None;
			bitmapImage.StreamSource = streamSource;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}

		public static double AngleDegree2(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = Math.Atan2(y1 - y2, x1 - x2);
			double num2 = (Math.Atan2(y3 - y2, x3 - x2) - num) / Math.PI * 180.0;
			if (num2 < 0.0)
			{
				return 360.0 + num2;
			}
			return num2;
		}

		public static double AngleDegree(System.Windows.Point p1, System.Windows.Point p2, System.Windows.Point p3)
		{
			return AngleDegree2(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
		}

		public static double AngleDegree(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = Math.Atan2(y1 - y2, x1 - x2);
			double num2 = (Math.Atan2(y3 - y2, x3 - x2) - num) / Math.PI * 180.0;
			if (num2 < 0.0)
			{
				return 360.0 + num2;
			}
			return num2;
		}

		public static double Angle2(System.Windows.Point cen, System.Windows.Point first, System.Windows.Point second)
		{
			double num = first.X - cen.X;
			double num2 = first.Y - cen.Y;
			double num3 = second.X - cen.X;
			double num4 = second.Y - cen.Y;
			float num5 = (float)Math.Sqrt(num * num + num2 * num2) * (float)Math.Sqrt(num3 * num3 + num4 * num4);
			if (num5 == 0f)
			{
				return -1.0;
			}
			return (float)Math.Acos((num * num3 + num2 * num4) / (double)num5);
		}

		public static double Angle(System.Windows.Point cen, System.Windows.Point first, System.Windows.Point second)
		{
			double num = first.X - cen.X;
			double num2 = first.Y - cen.Y;
			double num3 = second.X - cen.X;
			double num4 = second.Y - cen.Y;
			double num5 = num * num3 + num2 * num4;
			double num6 = Math.Sqrt(num * num + num2 * num2);
			double num7 = Math.Sqrt(num3 * num3 + num4 * num4);
			double d = num5 / (num6 * num7);
			return Math.Acos(d) * 180.0 / Math.PI;
		}

		public static BitmapImage GetKFBPreView(string path)
		{
			IntPtr datas = IntPtr.Zero;
			int b = 0;
			int c = 0;
			int a = 0;
			DllImageFuc.GetPriviewInfoPathFunc(path, out datas, ref a, ref b, ref c);
			byte[] array = new byte[a];
			if (datas != IntPtr.Zero)
			{
				Marshal.Copy(datas, array, 0, a);
			}
			DllImageFuc.DeleteImageDataFunc(datas);
			MemoryStream streamSource = new MemoryStream(array);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			bitmapImage.CacheOption = BitmapCacheOption.None;
			bitmapImage.StreamSource = streamSource;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}

		public static Stream ReizeImage(Stream ss, double Bl)
		{
			Image image = new Bitmap(ss);
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			if (image.Width > image.Height)
			{
				num2 = (int)Bl;
				num = Bl / (double)image.Width;
				num3 = (int)((double)image.Height * num);
				if (num3 % 4 != 0)
				{
					num3 += 4 - num3 % 4;
				}
			}
			else
			{
				num3 = (int)Bl;
				num = Bl / (double)image.Height;
				num2 = (int)((double)image.Width * num);
				if (num2 % 4 != 0)
				{
					num2 += 4 - num2 % 4;
				}
			}
			Bitmap bitmap = new Bitmap(image, new System.Drawing.Size(num2, num3));
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Bmp);
			return memoryStream;
		}

		public static Bitmap Rotate(Bitmap b, int angle)
		{
			angle %= 360;
			double num = (double)angle * Math.PI / 180.0;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			int width = b.Width;
			int height = b.Height;
			int num4 = (int)Math.Max(Math.Abs((double)width * num2 - (double)height * num3), Math.Abs((double)width * num2 + (double)height * num3));
			int num5 = (int)Math.Max(Math.Abs((double)width * num3 - (double)height * num2), Math.Abs((double)width * num3 + (double)height * num2));
			Bitmap bitmap = new Bitmap(num4, num5);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.InterpolationMode = InterpolationMode.Bilinear;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			System.Drawing.Point point = new System.Drawing.Point((num4 - width) / 2, (num5 - height) / 2);
			Rectangle rect = new Rectangle(point.X, point.Y, width, height);
			System.Drawing.Point point2 = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			graphics.TranslateTransform(point2.X, point2.Y);
			graphics.RotateTransform(360 - angle);
			graphics.TranslateTransform(-point2.X, -point2.Y);
			graphics.DrawImage(b, rect);
			graphics.ResetTransform();
			graphics.Save();
			graphics.Dispose();
			return bitmap;
		}

		public static System.Windows.Point PointRotate(System.Windows.Point center, System.Windows.Point p1, double angle)
		{
			System.Windows.Point result = default(System.Windows.Point);
			double num = angle * Math.PI / 180.0;
			double x = (p1.X - center.X) * Math.Cos(num) + (p1.Y - center.Y) * Math.Sin(num) + center.X;
			double y = (0.0 - (p1.X - center.X)) * Math.Sin(num) + (p1.Y - center.Y) * Math.Cos(num) + center.Y;
			result.X = x;
			result.Y = y;
			return result;
		}

		public static double GetFileSize(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			double num = fileInfo.Length;
			return num / 1024.0 / 1024.0;
		}

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		public static ImageSource ToImageSource(this Icon icon)
		{
			Bitmap bitmap = icon.ToBitmap();
			IntPtr hbitmap = bitmap.GetHbitmap();
			BitmapSource result = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			if (!DeleteObject(hbitmap))
			{
				throw new Win32Exception();
			}
			return result;
		}
	}
}
