using DivideToolControls.DeepZoom;
using DivideToolControls.DeepZoomControls;
using DivideToolControls.Helper;
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
using System.Windows.Threading;

namespace DivideToolControls.Controls
{
    /// <summary>
    /// Navmap.xaml 的交互逻辑
    /// </summary>
    public partial class Navmap : UserControl
    {
        private const double BORDERWIDTH = 1.0;

        private const double STROKETHICKNESS = 0.5;

        public Point m_DragAnchor;

        public double m_Height;

        public Line m_HLine;

        public bool m_IsDragging;

        public MultiScaleImage m_MultiScaleImage = ZoomModel.MulScaImg;

        public Rectangle m_RectView;

        public Line m_VLine;

        public double m_Width;

        public double m_OrgWidth = -99.0;

        public double m_OrgHeight = -99.0;

        public double m_CurWidth = -99.0;

        public double m_CurHeight = -99.0;

        public double m_OrgMsiWidth = -99.0;

        public double m_OrgMsiHeight = -99.0;

        public double m_Angle;

        public bool NavMove;

        public bool isOpenBrow;

        public List<mRectangle> listR10 = new List<mRectangle>();

        public List<mRectangle> listR20 = new List<mRectangle>();

        public List<mRectangle> listR40 = new List<mRectangle>();

        private int zindex = 1;

        private DispatcherTimer _timer;

        private int clickcount;

        public Color BlockColor
        {
            set
            {
                m_RectView.Fill = new SolidColorBrush(value);
            }
        }

        public Navmap()
        {
            InitializeComponent();
            CreateRect();
            CreateMenu();
        }

        public void SetImgWH(double w, double h)
        {
            m_CurWidth = w;
            m_CurHeight = h;
        }

        public void CreateMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            MenuItem menuItem2 = new MenuItem();
            MenuItem menuItem3 = new MenuItem();
            Label label = new Label();
            label.Content = "清除浏览记录";
            label.FontSize = 14.0;
            label.Height = 30.0;
            label.HorizontalContentAlignment = HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header = label;
            Label label2 = new Label();
            label2.Content = "关闭浏览记录";
            label2.FontSize = 14.0;
            label2.Height = 30.0;
            label2.HorizontalContentAlignment = HorizontalAlignment.Left;
            label2.VerticalContentAlignment = VerticalAlignment.Center;
            label2.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header2 = label2;
            Label label3 = new Label();
            label3.Content = "开启浏览记录";
            label3.FontSize = 14.0;
            label3.Height = 30.0;
            label3.HorizontalContentAlignment = HorizontalAlignment.Left;
            label3.VerticalContentAlignment = VerticalAlignment.Center;
            label3.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header3 = label3;
            menuItem.Header = header;
            menuItem.Click += menuItem_Click;
            menuItem2.Header = header2;
            menuItem3.Header = header3;
            if (isOpenBrow)
            {
                menuItem2.Click += menuItem1_Click;
                contextMenu.Items.Add(menuItem2);
            }
            else
            {
                menuItem3.Click += menuItem2_Click;
                contextMenu.Items.Add(menuItem3);
            }
            contextMenu.Items.Add(menuItem);
            cvThumbnail.ContextMenu = contextMenu;
        }

        private void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            isOpenBrow = true;
            cvThumbnail.ContextMenu = null;
            CreateMenu();
        }

        private void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            isOpenBrow = false;
            cvThumbnail.ContextMenu = null;
            CreateMenu();
        }

        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            ret40.Children.Clear();
            ret20.Children.Clear();
            ret10.Children.Clear();
            ret4.Children.Clear();
            listR10.Clear();
            listR20.Clear();
            listR40.Clear();
        }

        private void bitmapImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"加载缩略图时，发生错误。\n错误信息：{e.ErrorException.Message}");
        }

        private void bitmapImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = sender as BitmapImage;
            m_Width = bitmapImage.PixelWidth;
            m_Height = bitmapImage.PixelHeight;
            cvRoot.Width = bitmapImage.PixelWidth + 10;
            setClip();
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

        public void RemoveLine()
        {
            cvThumbnail.Children.Remove(m_HLine);
            cvThumbnail.Children.Remove(m_VLine);
        }

        private void CreateLine()
        {
            object obj = cvThumbnail.FindName("hLine");
            if (obj != null)
            {
                cvThumbnail.Children.Remove(obj as Line);
            }
            obj = cvThumbnail.FindName("vLine");
            if (obj != null)
            {
                cvThumbnail.Children.Remove(obj as Line);
            }
            Line line = new Line();
            line.Name = "hLine";
            line.X1 = 0.0;
            line.X2 = cvThumbnail.Width;
            line.Y1 = m_RectView.ActualHeight / 2.0;
            line.Y2 = m_RectView.ActualHeight / 2.0;
            line.Stroke = new SolidColorBrush(Color.FromArgb(155, byte.MaxValue, 0, 0));
            line.StrokeThickness = 0.5;
            Line line2 = m_HLine = line;
            Panel.SetZIndex(m_HLine, 1);
            Line line3 = new Line();
            line3.Name = "vLine";
            line3.X1 = m_RectView.ActualWidth / 2.0;
            line3.X2 = m_RectView.ActualWidth / 2.0;
            line3.Y1 = 0.0;
            line3.Y2 = cvThumbnail.Height;
            line3.Stroke = new SolidColorBrush(Color.FromArgb(155, byte.MaxValue, 0, 0));
            line3.StrokeThickness = 0.5;
            Line line4 = m_VLine = line3;
            Panel.SetZIndex(m_VLine, 1);
            cvThumbnail.Children.Add(m_HLine);
            cvThumbnail.Children.Add(m_VLine);
        }

        private void CreateRect()
        {
            object obj = cvThumbnail.FindName("rectView");
            if (obj != null)
            {
                cvThumbnail.Children.Remove(obj as Rectangle);
            }
            Rectangle rectangle = new Rectangle();
            rectangle.Name = "rectView";
            rectangle.StrokeThickness = 0.5;
            rectangle.Stroke = new SolidColorBrush(Color.FromArgb(155, byte.MaxValue, 0, 0));
            rectangle.Fill = new SolidColorBrush(Color.FromArgb(100, 206, 206, 206));
            rectangle.Cursor = Cursors.Hand;
            Rectangle rectangle2 = m_RectView = rectangle;
            Panel.SetZIndex(m_RectView, 2);
            FrameworkElement frameworkElement = null;
            m_RectView.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
            {
                frameworkElement = (sender as FrameworkElement);
                if (frameworkElement.CaptureMouse())
                {
                    m_IsDragging = true;
                    m_DragAnchor = e.GetPosition(null);
                }
                e.Handled = true;
            };
            m_RectView.MouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e)
            {
                m_IsDragging = false;
                (sender as FrameworkElement).ReleaseMouseCapture();
                e.Handled = true;
            };
            m_RectView.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                frameworkElement = (sender as FrameworkElement);
                if (m_IsDragging)
                {
                    double num = m_MultiScaleImage.ActualWidth;
                    double actualHeight = m_MultiScaleImage.ActualHeight;
                    double num2 = 0.0;
                    double num3 = 0.0;
                    if (m_Angle >= 0.0 && num > Setting.AngelMsiOffset * 2.0 && actualHeight > Setting.AngelMsiOffset * 2.0)
                    {
                        num -= Setting.AngelMsiOffset * 2.0;
                        actualHeight -= Setting.AngelMsiOffset * 2.0;
                    }
                    if (m_Angle != 0.0)
                    {
                        num2 = Setting.AngelMsiOffset;
                        num3 = Setting.AngelMsiOffset;
                    }
                    NavMove = true;
                    Point position = e.GetPosition(null);
                    double num4 = position.Y - m_DragAnchor.Y;
                    double num5 = position.X - m_DragAnchor.X;
                    double num6 = num4 + (double)frameworkElement.GetValue(Canvas.TopProperty);
                    double num7 = num5 + (double)frameworkElement.GetValue(Canvas.LeftProperty);
                    frameworkElement.SetValue(Canvas.TopProperty, Math.Round(num6, 2));
                    frameworkElement.SetValue(Canvas.LeftProperty, Math.Round(num7, 2));
                    m_DragAnchor = position;
                    Point center = new Point(0.0, 0.0);
                    Point p = new Point(num7 - m_Width / 2.0 + m_RectView.ActualWidth / 2.0, num6 - m_Height / 2.0 + m_RectView.ActualHeight / 2.0);
                    Point point = PointRotate(center, p, m_Angle);
                    double num8 = point.Y + m_OrgHeight / 2.0 - m_RectView.ActualHeight / 2.0;
                    double num9 = point.X + m_OrgWidth / 2.0 - m_RectView.ActualWidth / 2.0;
                    double num10 = ScaleToViewportWidth(m_MultiScaleImage.ZoomableCanvas.Scale, num, m_MultiScaleImage.Source.ImageSize.Width);
                    Point point2 = new Point(0.0, 0.0)
                    {
                        X = (num9 - 1.0) / m_RectView.ActualWidth * num10,
                        Y = (num8 - 1.0) / m_RectView.ActualWidth * num10
                    };
                    Point offset = default(Point);
                    offset.X = point2.X * num / num10 - num2;
                    offset.Y = point2.Y * num / num10 - num3;
                    double num13 = m_HLine.Y1 = (m_HLine.Y2 = m_RectView.ActualHeight / 2.0 + num6);
                    double num16 = m_VLine.X1 = (m_VLine.X2 = m_RectView.ActualWidth / 2.0 + num7);
                    if (Setting.IsSynchronous)
                    {
                        Point offset2 = m_MultiScaleImage.ZoomableCanvas.Offset;
                        foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                        {

                            ZoomModel.IsDw = false;
                            Point point3 = new Point(offset.X - offset2.X, offset.Y - offset2.Y);
                            ControlHelper.Instance.TmoveSetOffset4(new Point(point3.X, point3.Y), m_Angle);

                        }
                    }
                    m_MultiScaleImage.ZoomableCanvas.Offset = offset;
                    m_MultiScaleImage.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                }
            };
            base.MouseLeave += delegate (object sender, MouseEventArgs e)
            {
                m_IsDragging = false;
                (sender as FrameworkElement).ReleaseMouseCapture();
            };
            cvThumbnail.Children.Add(m_RectView);
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

        public void ClearMemoryThread()
        {
            clickcount++;
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 2);
                _timer.Tick += _timer_Tick;
                _timer.Start();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (clickcount > 3)
            {
                clickcount = 0;
            }
        }

        private void cvThumbnail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double num = m_MultiScaleImage.ActualWidth;
            double actualHeight = m_MultiScaleImage.ActualHeight;
            double num2 = 0.0;
            if (m_Angle > 0.0 && num > Setting.AngelMsiOffset * 2.0 && actualHeight > Setting.AngelMsiOffset * 2.0)
            {
                num -= Setting.AngelMsiOffset * 2.0;
                actualHeight -= Setting.AngelMsiOffset * 2.0;
            }
            NavMove = true;
            if (m_Angle != 0.0)
            {
                num2 = Setting.AngelMsiOffset;
                _ = Setting.AngelMsiOffset;
            }
            e.Handled = true;
            double num3 = ScaleToViewportWidth(m_MultiScaleImage.ZoomableCanvas.Scale, num, m_MultiScaleImage.Source.ImageSize.Width);
            FrameworkElement relativeTo = sender as FrameworkElement;
            double y = e.GetPosition(relativeTo).Y;
            double x = e.GetPosition(relativeTo).X;
            Point center = new Point(0.0, 0.0);
            _ = (m_Width - m_OrgWidth) / 2.0;
            _ = (m_Height - m_OrgHeight) / 2.0;
            Point p = new Point(x - m_Width / 2.0, y - m_Height / 2.0);
            Point point = PointRotate(center, p, m_Angle);
            y = point.Y + m_OrgHeight / 2.0;
            x = point.X + m_OrgWidth / 2.0;
            double num4 = y - m_RectView.ActualHeight / 2.0;
            double num5 = x - m_RectView.ActualWidth / 2.0;
            Point point2 = new Point(0.0, 0.0);
            point2.X = (num5 - 1.0) / m_RectView.ActualWidth * num3;
            point2.Y = (num4 - 1.0) / m_RectView.ActualWidth * num3;
            Point offset = default(Point);
            offset.X = point2.X * num / num3 - num2;
            offset.Y = point2.Y * num / num3 - num2;
            if (Setting.IsSynchronous)
            {
                Point point3 = new Point(m_MultiScaleImage.ZoomableCanvas.Offset.X, m_MultiScaleImage.ZoomableCanvas.Offset.Y);
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    Point point4 = new Point(offset.X - point3.X, offset.Y - point3.Y);
                    ZoomModel.IsDw = false;
                    ControlHelper.Instance.TmoveSetOffset4(new Point(point4.X, point4.Y), m_Angle);
                }
            }
            m_MultiScaleImage.ZoomableCanvas.Offset = offset;
            m_MultiScaleImage.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            UpdateThumbnailRect();
            ClearMemoryThread();
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

        public double ScaleToViewportWidth(double Scale, double viewportActualWidth, double slideWidth)
        {
            double num = viewportActualWidth / slideWidth;
            return num / Scale;
        }

        private void setClip()
        {
            Geometry geometry = new RectangleGeometry();
            geometry.SetValue(RectangleGeometry.RectProperty, new Rect(0.0, 0.0, m_Width + 2.0, m_Height + 2.0));
            cvThumbnail.Clip = geometry;
            cvThumbnail.Width = m_Width;
            cvThumbnail.Height = m_Height;
            UpdateThumbnailRect();
            CreateLine();
        }

        public void SetMultiScaleImage(MultiScaleImage msi)
        {
            m_MultiScaleImage = msi;
        }

        public void SetThumbnail(string url)
        {
            BitmapImage source = new BitmapImage(new Uri(url));
            imgThumbnail.Source = source;
        }

        /// <summary>
        /// 设置样本图片缩略图
        /// </summary>
        /// <param name="bitmapSource"></param>
        public void SetThumbnail(BitmapSource bitmapSource)
        {
            if (imgThumbnail.Source != null)
            {
                SetImgWH(((BitmapSource)imgThumbnail.Source).PixelWidth, ((BitmapSource)imgThumbnail.Source).PixelHeight);
            }
            imgThumbnail.Source = bitmapSource;
            if (m_OrgWidth == -99.0)
            {
                m_OrgWidth = bitmapSource.PixelWidth;
                m_OrgHeight = bitmapSource.PixelHeight;
                SetImgWH(m_OrgWidth, m_OrgHeight);
            }
            m_Width = bitmapSource.PixelWidth;
            m_Height = bitmapSource.PixelHeight;
            cvRoot.Width = m_Width;
            cvRoot.Height = m_Height;
            ret10.Width = m_Width;
            ret10.Height = m_Height;
            setClip();
        }

        public void UpdateThumbnailRect()
        {
            if (m_MultiScaleImage != null && !m_IsDragging && m_OrgWidth > 0
                )
            {
                Rect thumbnailRect = GetThumbnailRect(m_OrgWidth);
                ChangeRectLocation(thumbnailRect.X, thumbnailRect.Y, thumbnailRect.Width, thumbnailRect.Height);
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

        public Rect GetRotatePoint(double x, double y, double scale)
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
            double num5 = ScaleToViewportWidth(scale, num, multiScaleImage.Source.ImageSize.Width);
            double num6 = m_OrgWidth * num5;
            double num7 = num6 * num2 / num;
            point.X = (x + num3) * num5 / num;
            point.Y = (y + num4) * num5 / num;
            double num8 = point.X * m_OrgWidth;
            double num9 = point.Y * m_OrgWidth;
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
    }
}
