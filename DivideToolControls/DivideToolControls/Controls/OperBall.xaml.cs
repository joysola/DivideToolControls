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
using System.Windows.Threading;

namespace DivideToolControls.Controls
{
    /// <summary>
    /// OperBall.xaml 的交互逻辑
    /// </summary>
    public partial class OperBall : UserControl
    {
        private double HorizontalChangeX;

        private double VerticalChangeY;
        private DispatcherTimer _Operationtimer = new DispatcherTimer();
        public OperBall()
        {
            InitializeComponent();
        }
        private void Canvas_OperateballMenu_Click(object sender, RoutedEventArgs e)
        {
            Canvas_Operateball.Visibility = Visibility.Collapsed;
        }
        private void myThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int num = 40;
            double num2 = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            double num3 = Canvas.GetTop(myThumb) + e.VerticalChange;
            Point point = new Point(num2, num3);
            Point point2 = new Point(20.0, 20.0);
            double num4 = Math.Sqrt(Math.Abs(point.X - point2.X) * Math.Abs(point.X - point2.X) + Math.Abs(point.Y - point2.Y) * Math.Abs(point.Y - point2.Y));
            if (num4 > (double)num)
            {
                double num5 = 0.0;
                double num6 = 0.0;
                if (num2 > (double)num && num3 < (double)num && num3 > -10.0)
                {
                    num6 = Canvas.GetTop(myThumb) + e.VerticalChange;
                    num5 = Math.Sqrt((double)(num * num) - Math.Abs(num6 - point2.Y) * Math.Abs(num6 - point2.Y)) + point2.X;
                }
                if (num2 < -10.0 && num3 < (double)num && num3 > -10.0)
                {
                    num6 = Canvas.GetTop(myThumb) + e.VerticalChange;
                    num5 = (Math.Sqrt((double)(num * num) - Math.Abs(num6 - point2.Y) * Math.Abs(num6 - point2.Y)) - point2.X) * -1.0;
                }
                if (num3 < -10.0 && num2 < (double)num && num2 > -10.0)
                {
                    num5 = Canvas.GetLeft(myThumb) + e.HorizontalChange;
                    num6 = (Math.Sqrt((double)(num * num) - Math.Abs(num5 - point2.X) * Math.Abs(num5 - point2.X)) - point2.Y) * -1.0;
                }
                if (num3 > (double)num && num2 < (double)num && num2 > -10.0)
                {
                    num5 = Canvas.GetLeft(myThumb) + e.HorizontalChange;
                    num6 = Math.Sqrt((double)(num * num) - Math.Abs(num5 - point2.X) * Math.Abs(num5 - point2.X)) + point2.Y;
                }
                Console.WriteLine(num2 + "," + num3);
                Console.WriteLine(num5 + "," + num6);
                if (num5 != 0.0 && num6 != 0.0)
                {
                    Canvas.SetTop(myThumb, num6);
                    Canvas.SetLeft(myThumb, num5);
                }
            }
            else
            {
                Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) + e.HorizontalChange);
                Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) + e.VerticalChange);
            }
            HorizontalChangeX = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            VerticalChangeY = Canvas.GetTop(myThumb) + e.VerticalChange;
            _Operationtimer.Start();
        }
        private void myThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Canvas.SetLeft(myThumb, 20.0);
            Canvas.SetTop(myThumb, 20.0);
            HorizontalChangeX = 20.0;
            VerticalChangeY = 20.0;
            _Operationtimer.Stop();
        }
    }
}
