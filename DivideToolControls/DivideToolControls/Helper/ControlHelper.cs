using DivideToolControls.DeepZoom;
using DivideToolControls.DeepZoomControls;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DivideToolControls.Helper
{
    public class ControlHelper
    {
        public static ControlHelper Instance { get; } = new ControlHelper();

        private MultiScaleImage msi = ZoomModel.MulScaImg;
        public int GetMargin(string V)
        {
            int result = 0;
            switch (V.Length)
            {
                case 1:
                    result = 10;
                    break;
                case 2:
                    result = 15;
                    break;
                case 3:
                    result = 20;
                    break;
                case 4:
                    result = 25;
                    break;
            }
            return result;
        }
        public BitmapSource BitToBS(System.Drawing.Bitmap bmp)
        {
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                return null;
            }
        }
        public double CalcArea(IList<Point> points)
        {
            int count = points.Count;
            if (count < 3)
            {
                return 0.0;
            }
            double num = 0.0;
            int num2 = 0;
            int index = num2 + 1;
            for (int i = num2 + 2; i < count; i++)
            {
                Point p = points[num2];
                Point p2 = points[index];
                Point p3 = points[i];
                num = ((!(RadianOfTwoLine(p, p2, p3) > 0.0)) ? (num - TriangleArea(p, p2, p3)) : (num + TriangleArea(p, p2, p3)));
                index = i;
            }
            return Math.Abs(num);
        }

        public double CalcLengthClosed(IList<Point> points)
        {
            if (points == null)
            {
                return 0.0;
            }
            int count = points.Count;
            if (count < 2)
            {
                return 0.0;
            }
            if (points[0] != points[count - 1])
            {
                List<Point> list = new List<Point>();
                list.AddRange(points);
                list.Add(points[0]);
                return CalcLength(list);
            }
            return CalcLength(points);
        }


        public double CalcLength(IList<Point> points)
        {
            if (points == null)
            {
                return 0.0;
            }
            int count = points.Count;
            if (count < 2)
            {
                return 0.0;
            }
            double num = 0.0;
            int index = 0;
            for (int i = 1; i < count; i++)
            {
                num += LineLength(points[index], points[i]);
                index = i;
            }
            return num;
        }

        public void TmoveSetOffset4(Point p, double ARotate)
        {
            Point offset = msi.ZoomableCanvas.Offset;
            Point p2 = new Point(p.X, p.Y);
            Point center = new Point(0.0, 0.0);
            Point point = KCommon.PointRotate(center, p2, ZoomModel.Rotate - ARotate);
            offset.Y += point.Y;
            offset.X += point.X;
            msi.ZoomableCanvas.Offset = new Point(offset.X, offset.Y);
            msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }
        public BitmapImage LoadImage(int KfbioAddress, int Level, int posx, int posy)
        {
            try
            {
                IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
                k.DataFilePTR = KfbioAddress;
                int nDataLength = 0;
                int num = Math.Max((int)ZoomModel.ImageW, (int)ZoomModel.ImageH);
                int num2 = MathHelper.IsInteger((Math.Log(num) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num) / Math.Log(2.0))) : ((int)(Math.Log(num) / Math.Log(2.0)) + 1);
                float fScale = (num2 != Level) ? ((float)ZoomModel.SlideZoom / (float)Math.Pow(2.0, num2 - Level)) : ((float)ZoomModel.SlideZoom);
                ZoomModel.DllImgFunc.CkGetImageStreamFunc(ref k, fScale, posx * ZoomModel.TileSize, posy * ZoomModel.TileSize, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                BitmapImage bitmapImage = new BitmapImage();
                if (array.Length == 0)
                {
                    return new BitmapImage(new Uri("images/pp.png", UriKind.RelativeOrAbsolute));
                }
                try
                {
                    MemoryStream streamSource = new MemoryStream(array);
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.CacheOption = BitmapCacheOption.None;
                    bitmapImage.StreamSource = streamSource;
                    bitmapImage.EndInit();

                    // 保存图片
                    using (MemoryStream ms = new MemoryStream(array))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        image.Save(@"D:\\1.jpg");
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
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

        public BitmapImage GetLable(int KfbioAddress)
        {
            IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
            k.DataFilePTR = KfbioAddress;
            IntPtr datas = IntPtr.Zero;
            int b = 0;
            int c = 0;
            int a = 0;
            ZoomModel.DllImgFunc.CkGetLableInfoFunc(k, out datas, ref a, ref b, ref c);
            byte[] array = new byte[a];
            if (datas != IntPtr.Zero)
            {
                Marshal.Copy(datas, array, 0, a);
            }
            ZoomModel.DllImgFunc.CkDeleteImageDataFunc(datas);
            BitmapImage bitmapImage = new BitmapImage();
            MemoryStream streamSource = new MemoryStream(array);
            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmapImage.CacheOption = BitmapCacheOption.None;
            bitmapImage.StreamSource = streamSource;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        public BitmapImage GetCap(float _scale, int posx, int posy, int width, int height)
        {
            int nDataLength = 0;
            byte[] array = new byte[0];
            BitmapImage bitmapImage = new BitmapImage();
            try
            {
                DllImageFuc.GetImageDataRoiFunc(ZoomModel.InfoStruct, _scale, posx, posy, width, height, out IntPtr datas, ref nDataLength, true);
                array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                DllImageFuc.DeleteImageDataFunc(datas);
                if (array.Length == 0)
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(200, 200, System.Drawing.Imaging.PixelFormat.Format64bppPArgb);
                    System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
                    graphics.Clear(System.Drawing.Color.White);
                    graphics.Save();
                    array = Bitmap2Byte(bitmap);
                }
                MemoryStream streamSource = new MemoryStream(array);
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.None;
                bitmapImage.StreamSource = streamSource;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return bitmapImage;
            }
        }


        private byte[] Bitmap2Byte(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Seek(0L, SeekOrigin.Begin);
                memoryStream.Read(array, 0, Convert.ToInt32(memoryStream.Length));
                return array;
            }
        }
        private double RadianOfTwoLine(Point p1, Point p2, Point p3)
        {
            double num = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            return Math.Atan2(p3.Y - p1.Y, p3.X - p1.X) - num;
        }

        private double TriangleArea(Point p1, Point p2, Point p3)
        {
            double num = LineLength(p1, p2);
            double num2 = LineLength(p1, p3);
            double num3 = LineLength(p2, p3);
            if (num + num2 <= num3 || num2 + num3 <= num || num3 + num <= num2)
            {
                return 0.0;
            }
            double num4 = (num + num2 + num3) / 2.0;
            return Math.Sqrt(Math.Abs(num4 * (num4 - num) * (num4 - num2) * (num4 - num3)));
        }
        private double LineLength(Point p1, Point p2)
        {
            double num = p2.X - p1.X;
            double num2 = p2.Y - p1.Y;
            return Math.Sqrt(num * num + num2 * num2);
        }
    }
}
