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
using System.Windows.Shapes;

namespace DivideToolControls.WinCtls
{
    /// <summary>
    /// CtcViewer.xaml 的交互逻辑
    /// </summary>
    public partial class CtcViewer : Window
    {
        public List<BitmapImage> _listbit;

        public int MaxCount = 10;

        public int index;

        public int MaxScale = 40;

        private bool mouseDown;

        private bool mouseMove;

        private Point mouseXY;

        private bool IsCut;

        public CtcViewer()
        {
            InitializeComponent();
            _listbit = new List<BitmapImage>();
            x3dSlider.UpZ.MouseLeftButtonDown += UpZ_MouseLeftButtonDown;
            x3dSlider.DownZ.MouseLeftButtonDown += DownZ_MouseLeftButtonDown;
            TransformGroup transformGroup = ctc_img.FindResource("Imageview") as TransformGroup;
            _ = transformGroup.Children[0];
        }

        private void DownZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            index--;
            if (index < 0)
            {
                index = 0;
            }
            ctc_img.Source = _listbit[index];
            x3dSlider.Zvalue.Content = index.ToString();
        }

        private void UpZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            index++;
            if (index > MaxCount)
            {
                index = MaxCount;
            }
            ctc_img.Source = _listbit[index];
            x3dSlider.Zvalue.Content = index.ToString();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void _CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void IMG1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!mouseDown)
            {
                ContentControl contentControl = sender as ContentControl;
                if (contentControl != null)
                {
                    contentControl.CaptureMouse();
                    mouseDown = true;
                    mouseXY = e.GetPosition(contentControl);
                }
            }
        }

        private void IMG1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentControl contentControl = sender as ContentControl;
            if (contentControl != null)
            {
                contentControl.ReleaseMouseCapture();
                mouseDown = false;
                mouseMove = false;
            }
        }

        private void IMG1_MouseMove(object sender, MouseEventArgs e)
        {
            ContentControl contentControl = sender as ContentControl;
            if (contentControl != null && mouseDown)
            {
                mouseMove = true;
                e.GetPosition(contentControl);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    TransformGroup transformGroup = ctc_img.FindResource("Imageview") as TransformGroup;
                    TranslateTransform translateTransform = transformGroup.Children[1] as TranslateTransform;
                    Point position = e.GetPosition(contentControl);
                    translateTransform.X -= mouseXY.X - position.X;
                    translateTransform.Y -= mouseXY.Y - position.Y;
                    mouseXY = position;
                }
            }
        }

        private void Domousemove(ContentControl img, MouseEventArgs e)
        {
        }

        private void IMG1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ContentControl contentControl = sender as ContentControl;
            if (contentControl != null)
            {
                Point position = e.GetPosition(contentControl);
                TransformGroup group = ctc_img.FindResource("Imageview") as TransformGroup;
                int num = 100;
                num = ((e.Delta <= 0) ? (-200) : 200);
                double delta = (double)num * 0.001;
                DowheelZoom(group, position, delta);
            }
        }

        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            Point point2 = group.Inverse.Transform(point);
            ScaleTransform scaleTransform = group.Children[0] as ScaleTransform;
            if (!(scaleTransform.ScaleX + delta >= 2.0) && !(scaleTransform.ScaleX + delta <= 0.1))
            {
                scaleTransform.ScaleX += delta;
                scaleTransform.ScaleY += delta;
                TranslateTransform translateTransform = group.Children[1] as TranslateTransform;
                translateTransform.X = -1.0 * (point2.X * scaleTransform.ScaleX - point.X);
                translateTransform.Y = -1.0 * (point2.Y * scaleTransform.ScaleY - point.Y);
                lbl_Scale.Content = (double)MaxScale * scaleTransform.ScaleX + "X";
            }
        }
    }
}
