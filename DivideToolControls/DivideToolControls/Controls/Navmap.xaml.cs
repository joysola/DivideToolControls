using DivideToolControls.DeepZoom;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DivideToolControls.Controls
{
    /// <summary>
    /// Navmap.xaml 的交互逻辑
    /// </summary>
    public partial class Navmap : UserControl
    {
        public bool m_IsDragging;
        public double m_Angle;
        public double m_Height;
        public double m_Width;
        public double m_OrgWidth = -99.0;
        public double m_OrgHeight = -99.0;
        public Rectangle m_RectView;
        public bool isOpenBrow;
        public Line m_HLine;
        public Line m_VLine;
        private int zindex = 1;
        public bool NavMove;
        public List<mRectangle> listR10 = new List<mRectangle>();

        public List<mRectangle> listR20 = new List<mRectangle>();

        public List<mRectangle> listR40 = new List<mRectangle>();

        public MultiScaleImage m_MultiScaleImage;
        public Navmap()
        {
            InitializeComponent();
        }
        public void UpdateThumbnailRect()
        {
            if (m_MultiScaleImage != null && !m_IsDragging)
            {
                Rect thumbnailRect = GetThumbnailRect(m_OrgWidth);
                ChangeRectLocation(thumbnailRect.X, thumbnailRect.Y, thumbnailRect.Width, thumbnailRect.Height);
            }
        }
        private Rect GetThumbnailRect(double thumbnailWidth)
        {
            MultiScaleImage multiScaleImage = m_MultiScaleImage;
            double num = multiScaleImage.ActualWidth;
            double num2 = multiScaleImage.ActualHeight;
            double num3 = 0.0;
            double num4 = 0.0;
            if (m_Angle >= 0.0 && num > Setting.AngelMsiOffset * 2.0 && num2 > Setting.AngelMsiOffset * 2.0)
            {
                num -= Setting.AngelMsiOffset * 2.0;
                num2 -= Setting.AngelMsiOffset * 2.0;
            }
            if (m_Angle != 0.0)
            {
                num3 = Setting.AngelMsiOffset;
                num4 = Setting.AngelMsiOffset;
            }
            Point point = default(Point);
            double scale = multiScaleImage.ZoomableCanvas.Scale;
            double num5 = ScaleToViewportWidth(scale, num, multiScaleImage.Source.ImageSize.Width);
            double num6 = thumbnailWidth * num5;
            double num7 = num6 * num2 / num;
            point.X = (multiScaleImage.ZoomableCanvas.Offset.X + num3) * num5 / num;
            point.Y = (multiScaleImage.ZoomableCanvas.Offset.Y + num4) * num5 / num;
            double num8 = point.X * thumbnailWidth;
            double num9 = point.Y * thumbnailWidth;
            Point point2 = new Point(num8, num9);
            if (m_Angle != 0.0)
            {
                Point center = new Point(0.0, 0.0);
                Point point3 = new Point(num8 + num6 / 2.0, num9 + num7 / 2.0);
                Point p = new Point(point3.X - m_OrgWidth / 2.0, point3.Y - m_OrgHeight / 2.0);
                Point point4 = PointRotate(center, p, 360.0 - m_Angle);
                point2 = new Point(point4.X - num6 / 2.0 + m_Width / 2.0, point4.Y - num7 / 2.0 + m_Height / 2.0);
            }
            return new Rect(point2.X, point2.Y, num6, num7);
        }
        private void ChangeRectLocation(double left, double top, double width, double height)
        {
            if (m_RectView != null)
            {
                m_RectView.SetValue(Canvas.LeftProperty, left + 1.0);
                m_RectView.SetValue(Canvas.TopProperty, top + 1.0);
                m_RectView.Width = width;
                m_RectView.Height = height;
                if (m_HLine != null && m_VLine != null)
                {
                    double num3 = m_HLine.Y1 = (m_HLine.Y2 = height / 2.0 + (top + 1.0));
                    double num6 = m_VLine.X1 = (m_VLine.X2 = width / 2.0 + (left + 1.0));
                }
            }
        }
        public void DrawRect(double scale)
        {
            if (m_MultiScaleImage != null && isOpenBrow)
            {
                Rect thumbnailRect = GetThumbnailRect(m_OrgWidth);
                DrawRect(thumbnailRect.X, thumbnailRect.Y, thumbnailRect.Width, thumbnailRect.Height, scale);
            }
        }
        public void DrawRect(double x, double y, double width, double height, double scale)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.SetValue(Canvas.LeftProperty, x);
            rectangle.SetValue(Canvas.TopProperty, y);
            rectangle.SetValue(Panel.ZIndexProperty, zindex);
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.StrokeThickness = 0.0;
            mRectangle mRectangle = new mRectangle();
            mRectangle.offsetx = m_MultiScaleImage.ZoomableCanvas.Offset.X;
            mRectangle.offsety = m_MultiScaleImage.ZoomableCanvas.Offset.Y;
            mRectangle.width = width;
            mRectangle.height = height;
            mRectangle.zindex = zindex;
            mRectangle.scale = m_MultiScaleImage.ZoomableCanvas.Scale;
            rectangle.Stroke = new SolidColorBrush(Setting.STROCK_COLOR);
            if (scale >= 30.0 && scale <= 80.0)
            {
                rectangle.Fill = new SolidColorBrush(Setting.STROCK_COLOR1);
                ret40.Children.Add(rectangle);
                mRectangle.color = Setting.STROCK_COLOR1;
                listR40.Add(mRectangle);
            }
            else if (scale >= 15.0 && scale < 30.0)
            {
                rectangle.Fill = new SolidColorBrush(Setting.STROCK_COLOR2);
                ret20.Children.Add(rectangle);
                mRectangle.color = Setting.STROCK_COLOR2;
                listR20.Add(mRectangle);
            }
            else if (scale >= 10.0 && scale < 15.0)
            {
                rectangle.Fill = new SolidColorBrush(Setting.STROCK_COLOR3);
                ret10.Children.Add(rectangle);
                mRectangle.color = Setting.STROCK_COLOR3;
                listR10.Add(mRectangle);
            }
        }

        public double ScaleToViewportWidth(double Scale, double viewportActualWidth, double slideWidth)
        {
            double num = viewportActualWidth / slideWidth;
            return num / Scale;
        }

        private Point PointRotate(Point center, Point p1, double angle)
        {
            Point result = default(Point);
            double num = angle * Math.PI / 180.0;
            double x = (p1.X - center.X) * Math.Cos(num) + (p1.Y - center.Y) * Math.Sin(num) + center.X;
            double y = (0.0 - (p1.X - center.X)) * Math.Sin(num) + (p1.Y - center.Y) * Math.Cos(num) + center.Y;
            result.X = x;
            result.Y = y;
            return result;
        }

        private void cvThumbnail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //double num = m_MultiScaleImage.ActualWidth;
            //double actualHeight = m_MultiScaleImage.ActualHeight;
            //double num2 = 0.0;
            //if (m_Angle > 0.0 && num > Setting.AngelMsiOffset * 2.0 && actualHeight > Setting.AngelMsiOffset * 2.0)
            //{
            //    num -= Setting.AngelMsiOffset * 2.0;
            //    actualHeight -= Setting.AngelMsiOffset * 2.0;
            //}
            //NavMove = true;
            //if (m_Angle != 0.0)
            //{
            //    num2 = Setting.AngelMsiOffset;
            //    _ = Setting.AngelMsiOffset;
            //}
            //e.Handled = true;
            //double num3 = ScaleToViewportWidth(m_MultiScaleImage.ZoomableCanvas.Scale, num, m_MultiScaleImage.Source.ImageSize.Width);
            //FrameworkElement relativeTo = sender as FrameworkElement;
            //double y = e.GetPosition(relativeTo).Y;
            //double x = e.GetPosition(relativeTo).X;
            //Point center = new Point(0.0, 0.0);
            //_ = (m_Width - m_OrgWidth) / 2.0;
            //_ = (m_Height - m_OrgHeight) / 2.0;
            //Point p = new Point(x - m_Width / 2.0, y - m_Height / 2.0);
            //Point point = PointRotate(center, p, m_Angle);
            //y = point.Y + m_OrgHeight / 2.0;
            //x = point.X + m_OrgWidth / 2.0;
            //double num4 = y - m_RectView.ActualHeight / 2.0;
            //double num5 = x - m_RectView.ActualWidth / 2.0;
            //Point point2 = new Point(0.0, 0.0);
            //point2.X = (num5 - 1.0) / m_RectView.ActualWidth * num3;
            //point2.Y = (num4 - 1.0) / m_RectView.ActualWidth * num3;
            //Point offset = default(Point);
            //offset.X = point2.X * num / num3 - num2;
            //offset.Y = point2.Y * num / num3 - num2;
            //if (Setting.IsSynchronous)
            //{
            //    Point point3 = new Point(m_MultiScaleImage.ZoomableCanvas.Offset.X, m_MultiScaleImage.ZoomableCanvas.Offset.Y);
            //    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            //    {
            //        if ((Mainpage)item.Value != _Mainpage)
            //        {
            //            Point point4 = new Point(offset.X - point3.X, offset.Y - point3.Y);
            //            new Point(0.0, 0.0);
            //            ((Mainpage)item.Value).IsDw = false;
            //            ((Mainpage)item.Value).TmoveSetOffset4((Mainpage)item.Value, new Point(point4.X, point4.Y), m_Angle);
            //        }
            //    }
            //}
            //m_MultiScaleImage.ZoomableCanvas.Offset = offset;
            //m_MultiScaleImage.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            //UpdateThumbnailRect();
            //ClearMemoryThread();
        }
    }
}
