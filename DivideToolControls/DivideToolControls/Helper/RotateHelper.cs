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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DivideToolControls.Helper
{
    public class RotateHelper
    {
        public static RotateHelper Instance { get; } = new RotateHelper();
        private MultiScaleImage msi = ZoomModel.MulScaImg;

        public void MsiRotate(double dRotate)
        {
            try
            {
                double rotate = ZoomModel.Rotate;
                Thickness t = new Thickness(0.0, 0.0, 0.0, 0.0);
                if (ZoomModel.Bg.Margin == t)
                {
                    ZoomModel.Bg.Margin = new Thickness(0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(msi.ZoomableCanvas.Offset.X - Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y - Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                }
                _ = msi.ActualWidth;
                double num = dRotate;
                if (num < 360.0)
                {
                    num += 360.0;
                }
                if (num >= 360.0)
                {
                    num -= 360.0;
                }
                RotateTransform renderTransform = new RotateTransform(num);
                ZoomModel.Rotate = num;
                msi.Tag = ZoomModel.Rotate;
                ZoomModel.Bg.RenderTransformOrigin = new Point(0.5, 0.5);
                ZoomModel.Bg.RenderTransform = renderTransform;
                ZoomModel.Magfier.ThumbMag.RenderTransformOrigin = new Point(0.5, 0.5);
                ZoomModel.Magfier.ThumbMag.RenderTransform = renderTransform;
                ZoomModel.Nav.m_Angle = ZoomModel.Rotate;
                int nDataLength = 0;
                int num2 = Math.Max((int)ZoomModel.ImageW, (int)ZoomModel.ImageH);
                int num3 = MathHelper.IsInteger((Math.Log(num2) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num2) / Math.Log(2.0))) : ((int)(Math.Log(num2) / Math.Log(2.0)) + 1);
                float fScale = num3 != 8 ? ZoomModel.SlideZoom / (float)Math.Pow(2.0, num3 - 8) : ZoomModel.SlideZoom;
                ZoomModel.DllImgFunc.CkGetImageStreamFunc(ref ZoomModel.InfoStruct, fScale, 0, 0, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                MemoryStream stream = new MemoryStream(array);
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(stream);
                ZoomModel.Nav.cvThumbnail.Children.Remove(ZoomModel.Nav.m_HLine);
                ZoomModel.Nav.cvThumbnail.Children.Remove(ZoomModel.Nav.m_VLine);
                BitmapSource bitmapSource = ControlHelper.Instance.BitToBS(KCommon.Rotate(b, (int)(360.0 - ZoomModel.Rotate)));
                if (bitmapSource != null)
                {
                    ZoomModel.Nav.SetThumbnail(bitmapSource);
                }
                ZoomModel.Nav.UpdateThumbnailRect();
                if (ZoomModel.Rotate == 0.0)
                {
                    ZoomModel.Bg.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(msi.ZoomableCanvas.Offset.X + Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y + Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                    ZoomModel.Nav.m_Angle = 0.0;
                }
                if (ZoomModel.Nav.ret40.Children.Count > 0)
                {
                    ZoomModel.Nav.ret40.Children.Clear();
                    foreach (mRectangle item in ZoomModel.Nav.listR40)
                    {
                        Rect rotatePoint = ZoomModel.Nav.GetRotatePoint(item.offsetx, item.offsety, item.scale);
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                        rectangle.SetValue(Canvas.LeftProperty, rotatePoint.Left);
                        rectangle.SetValue(Canvas.TopProperty, rotatePoint.Top);
                        rectangle.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item.zindex);
                        rectangle.Width = rotatePoint.Width;
                        rectangle.Height = rotatePoint.Height;
                        rectangle.Fill = new SolidColorBrush(item.color);
                        ZoomModel.Nav.ret40.Children.Add(rectangle);
                    }
                }
                if (ZoomModel.Nav.ret20.Children.Count > 0)
                {
                    ZoomModel.Nav.ret20.Children.Clear();
                    foreach (mRectangle item2 in ZoomModel.Nav.listR20)
                    {
                        Rect rotatePoint2 = ZoomModel.Nav.GetRotatePoint(item2.offsetx, item2.offsety, item2.scale);
                        System.Windows.Shapes.Rectangle rectangle2 = new System.Windows.Shapes.Rectangle();
                        rectangle2.SetValue(Canvas.LeftProperty, rotatePoint2.Left);
                        rectangle2.SetValue(Canvas.TopProperty, rotatePoint2.Top);
                        rectangle2.SetValue(Panel.ZIndexProperty, item2.zindex);
                        rectangle2.Width = rotatePoint2.Width;
                        rectangle2.Height = rotatePoint2.Height;
                        rectangle2.Fill = new SolidColorBrush(item2.color);
                        ZoomModel.Nav.ret20.Children.Add(rectangle2);
                    }
                }
                if (ZoomModel.Nav.ret10.Children.Count > 0)
                {
                    ZoomModel.Nav.ret10.Children.Clear();
                    foreach (mRectangle item3 in ZoomModel.Nav.listR10)
                    {
                        Rect rotatePoint3 = ZoomModel.Nav.GetRotatePoint(item3.offsetx, item3.offsety, item3.scale);
                        System.Windows.Shapes.Rectangle rectangle3 = new System.Windows.Shapes.Rectangle();
                        rectangle3.SetValue(Canvas.LeftProperty, rotatePoint3.Left);
                        rectangle3.SetValue(Canvas.TopProperty, rotatePoint3.Top);
                        rectangle3.SetValue(Panel.ZIndexProperty, item3.zindex);
                        rectangle3.Width = rotatePoint3.Width;
                        rectangle3.Height = rotatePoint3.Height;
                        rectangle3.Fill = new SolidColorBrush(item3.color);
                        ZoomModel.Nav.ret10.Children.Add(rectangle3);
                    }
                }
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item4 in Setting.TabsDic)
                    {
                        double tmpRotate = ZoomModel.Rotate - rotate;
                        SynRotate(tmpRotate);
                    }
                }
            }
            catch
            {
            }
        }

        public void SynRotate(double TmpRotate)
        {
            Thickness t = new Thickness(0.0, 0.0, 0.0, 0.0);
            if (ZoomModel.Bg.Margin == t)
            {
                ZoomModel.Bg.Margin = new Thickness(0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset);
                msi.ZoomableCanvas.Offset = new Point(msi.ZoomableCanvas.Offset.X - Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y - Setting.AngelMsiOffset);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
            ZoomModel.Bg.UpdateLayout();
            _ = msi.ActualWidth;
            double num = ZoomModel.Rotate + TmpRotate;
            if (num < 360.0)
            {
                num += 360.0;
            }
            if (num >= 360.0)
            {
                num -= 360.0;
            }
            RotateTransform renderTransform = new RotateTransform(num);
            ZoomModel.RotCtl.RenderTransformOrigin = new Point(0.5, 0.5);
            ZoomModel.RotCtl.Btn_AngleLine.RenderTransform = renderTransform;
            ZoomModel.RotCtl.lbl_Angle.Content = (int)num + "°";
            Point center = new Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            Point p = new Point(x, y);
            Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(ZoomModel.RotCtl.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(ZoomModel.RotCtl.ThumbAngle, point.Y - 7.5);
            SynMsiRotate(num);
        }

        public void SynMsiRotate(double dRotate)
        {
            try
            {
                double num = dRotate;
                if (num < 360.0)
                {
                    num += 360.0;
                }
                if (num >= 360.0)
                {
                    num -= 360.0;
                }
                RotateTransform renderTransform = new RotateTransform(num);
                ZoomModel.Rotate = num;
                msi.Tag = ZoomModel.Rotate;
                ZoomModel.Bg.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                ZoomModel.Bg.RenderTransform = renderTransform;
                ZoomModel.Magfier.ThumbMag.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                ZoomModel.Magfier.ThumbMag.RenderTransform = renderTransform;
                ZoomModel.Nav.m_Angle = ZoomModel.Rotate;
                if (ZoomModel.Rotate == 0.0)
                {
                    ZoomModel.Bg.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                    msi.ZoomableCanvas.Offset = new Point(msi.ZoomableCanvas.Offset.X + Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y + Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                    ZoomModel.Nav.m_Angle = 0.0;
                    _ = msi.ActualWidth;
                }
                int nDataLength = 0;
                int num2 = Math.Max((int)ZoomModel.ImageW, (int)ZoomModel.ImageH);
                int num3 = MathHelper.IsInteger((Math.Log(num2) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num2) / Math.Log(2.0))) : ((int)(Math.Log(num2) / Math.Log(2.0)) + 1);
                float fScale = (num3 != 8) ? ((float)ZoomModel.SlideZoom / (float)Math.Pow(2.0, num3 - 8)) : ((float)ZoomModel.SlideZoom);
                ZoomModel.DllImgFunc.CkGetImageStreamFunc(ref ZoomModel.InfoStruct, fScale, 0, 0, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                ZoomModel.DllImgFunc.CkDeleteImageDataFunc(datas);
                MemoryStream stream = new MemoryStream(array);
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(stream);
                ZoomModel.Nav.cvThumbnail.Children.Remove(ZoomModel.Nav.m_HLine);
                ZoomModel.Nav.cvThumbnail.Children.Remove(ZoomModel.Nav.m_VLine);
                BitmapSource bitmapSource = ControlHelper.Instance.BitToBS(KCommon.Rotate(b, (int)(360.0 - ZoomModel.Rotate)));
                if (bitmapSource != null)
                {
                    ZoomModel.Nav.SetThumbnail(bitmapSource);
                }
                ZoomModel.Nav.UpdateThumbnailRect();
                if (ZoomModel.Nav.ret40.Children.Count > 0)
                {
                    ZoomModel.Nav.ret40.Children.Clear();
                    foreach (mRectangle item in ZoomModel.Nav.listR40)
                    {
                        Rect rotatePoint = ZoomModel.Nav.GetRotatePoint(item.offsetx, item.offsety, item.scale);
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                        rectangle.SetValue(Canvas.LeftProperty, rotatePoint.Left);
                        rectangle.SetValue(Canvas.TopProperty, rotatePoint.Top);
                        rectangle.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item.zindex);
                        rectangle.Width = rotatePoint.Width;
                        rectangle.Height = rotatePoint.Height;
                        rectangle.Fill = new SolidColorBrush(item.color);
                        ZoomModel.Nav.ret40.Children.Add(rectangle);
                    }
                }
                if (ZoomModel.Nav.ret20.Children.Count > 0)
                {
                    ZoomModel.Nav.ret20.Children.Clear();
                    foreach (mRectangle item2 in ZoomModel.Nav.listR20)
                    {
                        Rect rotatePoint2 = ZoomModel.Nav.GetRotatePoint(item2.offsetx, item2.offsety, item2.scale);
                        System.Windows.Shapes.Rectangle rectangle2 = new System.Windows.Shapes.Rectangle();
                        rectangle2.SetValue(Canvas.LeftProperty, rotatePoint2.Left);
                        rectangle2.SetValue(Canvas.TopProperty, rotatePoint2.Top);
                        rectangle2.SetValue(Panel.ZIndexProperty, item2.zindex);
                        rectangle2.Width = rotatePoint2.Width;
                        rectangle2.Height = rotatePoint2.Height;
                        rectangle2.Fill = new SolidColorBrush(item2.color);
                        ZoomModel.Nav.ret20.Children.Add(rectangle2);
                    }
                }
                if (ZoomModel.Nav.ret10.Children.Count > 0)
                {
                    ZoomModel.Nav.ret10.Children.Clear();
                    foreach (mRectangle item3 in ZoomModel.Nav.listR10)
                    {
                        Rect rotatePoint3 = ZoomModel.Nav.GetRotatePoint(item3.offsetx, item3.offsety, item3.scale);
                        System.Windows.Shapes.Rectangle rectangle3 = new System.Windows.Shapes.Rectangle();
                        rectangle3.SetValue(Canvas.LeftProperty, rotatePoint3.Left);
                        rectangle3.SetValue(Canvas.TopProperty, rotatePoint3.Top);
                        rectangle3.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item3.zindex);
                        rectangle3.Width = rotatePoint3.Width;
                        rectangle3.Height = rotatePoint3.Height;
                        rectangle3.Fill = new SolidColorBrush(item3.color);
                        ZoomModel.Nav.ret10.Children.Add(rectangle3);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
