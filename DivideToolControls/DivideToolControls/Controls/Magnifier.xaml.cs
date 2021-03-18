using DivideToolControls.DeepZoom;
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
    /// Magnifier.xaml 的交互逻辑
    /// </summary>
    public partial class Magnifier : UserControl
    {
        public DispatcherTimer Magnifiertimer { get; set; } = new DispatcherTimer(DispatcherPriority.Background);
        public Point MagnifierMovePoint { get; set; } = new Point(0.0, 0.0);

        private MultiScaleImage msi = ZoomModel.MulScaImg;
        public int MagnifierScale;
        public Magnifier()
        {
            InitializeComponent();
            Magnifiertimer.Interval = new TimeSpan(0, 0, 0, 0, 80);
            Magnifiertimer.Tick += _Magnifiertimer_Tick;
            this.CreateMagnifierMenu();
        }
        private void _Magnifiertimer_Tick(object sender, EventArgs e)
        {
            if (msi.ZoomableCanvas != null)
            {
                Point magnifierMovePoint = MagnifierMovePoint;
                Point center = new Point(0.0, 0.0);
                magnifierMovePoint = new Point(magnifierMovePoint.X - ZoomModel.LayoutBody.ActualWidth / 2.0, magnifierMovePoint.Y - ZoomModel.LayoutBody.ActualHeight / 2.0);
                Point point = KCommon.PointRotate(center, magnifierMovePoint, ZoomModel.Rotate);
                double num = msi.ZoomableCanvas.Offset.X + point.X + msi.ActualWidth / 2.0;
                double num2 = msi.ZoomableCanvas.Offset.Y + point.Y + msi.ActualHeight / 2.0;
                float num3 = 0f;
                if (MagnifierScale == 2)
                {
                    num3 = (float)ZoomModel.Curscale * 2f;
                }
                if (MagnifierScale == 4)
                {
                    num3 = (float)ZoomModel.Curscale * 4f;
                }
                if (MagnifierScale == ZoomModel.SlideZoom || MagnifierScale == 0)
                {
                    num3 = ZoomModel.SlideZoom;
                }
                int posx = (int)(num * num3 / ZoomModel.Curscale) - 100;
                int posy = (int)(num2 * num3 / ZoomModel.Curscale) - 100;
                ThumbMag.Background = new ImageBrush(ControlHelper.Instance.GetCap(num3, posx, posy, 200, 200));
            }
        }

        private void CreateMagnifierMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            MenuItem menuItem2 = new MenuItem();
            MenuItem menuItem3 = new MenuItem();
            MenuItem menuItem4 = new MenuItem();
            
           Label label = new Label();
            label.Content = "2X";
            label.FontSize = 14.0;
            label.Height = 30.0;
            label.HorizontalContentAlignment = HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header = label;
            Label label2 = new Label();
            label2.Content = "4X";
            label2.FontSize = 14.0;
            label2.Height = 30.0;
            label2.HorizontalContentAlignment = HorizontalAlignment.Left;
            label2.VerticalContentAlignment = VerticalAlignment.Center;
            label2.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header2 = label2;
            Label label3 = new Label();
            label3.Content = "Max";
            label3.FontSize = 14.0;
            label3.Height = 30.0;
            label3.HorizontalContentAlignment = HorizontalAlignment.Left;
            label3.VerticalContentAlignment = VerticalAlignment.Center;
            label3.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header3 = label3;
            Label label4 = new Label();
            label4.Content = "关闭";
            label4.FontSize = 14.0;
            label4.Height = 30.0;
            label4.HorizontalContentAlignment = HorizontalAlignment.Left;
            label4.VerticalContentAlignment = VerticalAlignment.Center;
            label4.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Label header4 = label4;
            menuItem.Header = header;
            menuItem2.Header = header2;
            menuItem3.Header = header3;
            menuItem4.Header = header4;
            if (MagnifierScale == 2)
            {
                menuItem.Icon = new Image
                {
                    Source = new BitmapImage(new Uri("images/Ok1.png", UriKind.Relative)),
                    Width = 30.0,
                    Height = 30.0
                };
            }
            if (MagnifierScale == 4)
            {
                menuItem2.Icon = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri("images/Ok1.png", UriKind.Relative)),
                    Width = 30.0,
                    Height = 30.0
                };
            }
            else if (MagnifierScale ==ZoomModel.SlideZoom || MagnifierScale == 0)
            {
                menuItem3.Icon = new Image
                {
                    Source = new BitmapImage(new Uri("images/Ok1.png", UriKind.Relative)),
                    Width = 30.0,
                    Height = 30.0
                };
            }
            contextMenu.Items.Add(menuItem);
            contextMenu.Items.Add(menuItem2);
            contextMenu.Items.Add(menuItem3);
            contextMenu.Items.Add(menuItem4);
            menuItem.Click += menuItem_Click;
            menuItem2.Click += menuItem1_Click;
            menuItem3.Click += menuItem2_Click;
            menuItem4.Click += menuItem3_Click;
            Can_Magnifier.ContextMenu = contextMenu;
        }
        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            MagnifierScale = 2;
            Can_Magnifier.ContextMenu = null;
            CreateMagnifierMenu();
        }

        private void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            MagnifierScale = 4;
            Can_Magnifier.ContextMenu = null;
            CreateMagnifierMenu();
        }

        private void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            MagnifierScale = ZoomModel.SlideZoom;
            Can_Magnifier.ContextMenu = null;
            CreateMagnifierMenu();
        }

        private void menuItem3_Click(object sender, RoutedEventArgs e)
        {
            Can_Magnifier.Visibility = Visibility.Collapsed;
            Magnifiertimer.Stop();
        }
    }
}
