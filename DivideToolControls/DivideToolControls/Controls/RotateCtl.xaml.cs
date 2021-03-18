using DivideToolControls.Helper;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// RotateCtl.xaml 的交互逻辑
    /// </summary>
    public partial class RotateCtl : UserControl
    {
        public event RoutedEventHandler RotateHandler;

        public RotateCtl()
        {
            InitializeComponent();
            this.RotateHandler += RotateViewer_RotateHandler;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RotateHandler != null)
            {
                this.RotateHandler(sender, null);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            base.Visibility = Visibility.Collapsed;
        }

        private void ThumbAngle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double x = 39.0;
            double y = 5.5;
            double x2 = Canvas.GetLeft(this.ThumbAngle) + 7.5 + e.HorizontalChange;
            double y2 = Canvas.GetTop(ThumbAngle) + 7.5 + e.VerticalChange;
            Point point = new Point(40.0, 40.0);
            Point point2 = new Point(x, y);
            Point p = new Point(x2, y2);
            double num = KCommon.AngleDegree(p, point, point2);
            Point point3 = KCommon.PointRotate(point, point2, num);
            Canvas.SetLeft(ThumbAngle, point3.X - 7.5);
            Canvas.SetTop(ThumbAngle, point3.Y - 7.5);
            RotateTransform renderTransform = new RotateTransform(360.0 - num);
            RenderTransformOrigin = new Point(0.5, 0.5);
            Btn_AngleLine.RenderTransform = renderTransform;
            lbl_Angle.Content = (int)(360.0 - num) + "°";
          RotateHelper.Instance.MsiRotate(360.0 - num);
        }
        private void RotateViewer_RotateHandler(object sender, RoutedEventArgs e)
        {
            double num = double.Parse(((System.Windows.Shapes.Rectangle)sender).Tag.ToString());
            RotateTransform renderTransform = new RotateTransform(num);
            RenderTransformOrigin = new Point(0.5, 0.5);
            Btn_AngleLine.RenderTransform = renderTransform;
            lbl_Angle.Content = (int)num + "°";
            Point center = new Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            Point p = new Point(x, y);
            Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(ThumbAngle, point.X - 7.5);
            Canvas.SetTop(ThumbAngle, point.Y - 7.5);
            RotateHelper.Instance.MsiRotate(num);
        }
        private void Rotate_Anticlockwise_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double num = ZoomModel.Rotate - 5.0;
            if (num >= 360.0)
            {
                num = 360.0 - num;
            }
            if (num < 0.0)
            {
                num = 0.0;
            }
            RotateTransform renderTransform = new RotateTransform(num);
            RenderTransformOrigin = new Point(0.5, 0.5);
            Btn_AngleLine.RenderTransform = renderTransform;
            lbl_Angle.Content = (int)num + "°";
            Point center = new Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            Point p = new Point(x, y);
            Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(ThumbAngle, point.X - 7.5);
            Canvas.SetTop(ThumbAngle, point.Y - 7.5);
            RotateHelper.Instance.MsiRotate(num);
        }
        private void Rotate_Clockwise_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double num = ZoomModel.Rotate + 5.0;
            if (num >= 360.0)
            {
                num = 360.0 - num;
            }
            if (num < 0.0)
            {
                num = 360.0 + num;
            }
            RotateTransform renderTransform = new RotateTransform(num);
            RenderTransformOrigin = new Point(0.5, 0.5);
            Btn_AngleLine.RenderTransform = renderTransform;
            lbl_Angle.Content = (int)num + "°";
            Point center = new Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            Point p = new Point(x, y);
            Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(ThumbAngle, point.X - 7.5);
            Canvas.SetTop(ThumbAngle, point.Y - 7.5);
            RotateHelper.Instance.MsiRotate(num);
        }
        private void centerelp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                RotateTransform renderTransform = new RotateTransform(0.0);
                RenderTransformOrigin = new Point(0.5, 0.5);
                Btn_AngleLine.RenderTransform = renderTransform;
                lbl_Angle.Content = 0 + "°";
                Point center = new Point(40.0, 40.0);
                double x = 39.0;
                double y = 5.5;
                Point p = new Point(x, y);
                Point point = KCommon.PointRotate(center, p, 360.0);
                Canvas.SetLeft(ThumbAngle, point.X - 7.5);
                Canvas.SetTop(ThumbAngle, point.Y - 7.5);
                RotateHelper.Instance.MsiRotate(0.0);
            }
        }
    }
}
