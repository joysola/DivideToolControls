using DivideToolControls.AnnotationControls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DynamicGeometry.Enum;
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
using System.Windows.Shapes;

namespace DivideToolControls.WinCtls
{
    /// <summary>
    /// AnnoWind.xaml 的交互逻辑
    /// </summary>
    public partial class AnnoWind : Window
    {
        public AnnoBase _AnnotationBase;
        private MultiScaleImage msi = ZoomModel.MulScaImg;

        public AnnoWind()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        private void _CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void SaveAnno_Click(object sender, RoutedEventArgs e)
        {
            ZoomModel.AnnoWind._AnnotationBase.Size = int.Parse((ZoomModel.AnnoWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
            ZoomModel.AnnoWind._AnnotationBase.AnnotationName = ZoomModel.AnnoWind.AnnoName.Text;
            ZoomModel.AnnoWind._AnnotationBase.AnnotationDescription = ZoomModel.AnnoWind.AnnoDes.Text;
            ZoomModel.AnnoWind._AnnotationBase.BorderBrush = new SolidColorBrush(ZoomModel.AnnoWind._colorPicker.SelectedColor);
            ZoomModel.AnnoWind._AnnotationBase.isVisble = ((ZoomModel.AnnoWind.ShowInfo.IsChecked != true) ? Visibility.Collapsed : Visibility.Visible);
            ZoomModel.AnnoWind._AnnotationBase.isMsVisble = ((ZoomModel.AnnoWind.ShowMs.IsChecked == true) ? true : false);
            if (ZoomModel.AnnoWind._AnnotationBase.GetType() == typeof(AnnoPin))
            {
                if (ZoomModel.AnnoWind.Rad_1.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_1.png";
                }
                else if (ZoomModel.AnnoWind.Rad_2.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_2.png";
                }
                else if (ZoomModel.AnnoWind.Rad_3.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_3.png";
                }
                else if (ZoomModel.AnnoWind.Rad_4.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_4.png";
                }
                ZoomModel.AnnoWind._AnnotationBase.FontColor = new SolidColorBrush(ZoomModel.AnnoWind._colorPicker.SelectedColor);
            }
            if (ZoomModel.AnnoWind._AnnotationBase.AnnotationType == AnnotationType.DiyCtcRectangle)
            {
                AnnoDIYCtcRect myDiyCtcRectangle = (AnnoDIYCtcRect)ZoomModel.AnnoWind._AnnotationBase;
                Point originStart = myDiyCtcRectangle.OriginStart;
                double num = msi.ZoomableCanvas.Offset.X + originStart.X;
                double num2 = msi.ZoomableCanvas.Offset.Y + originStart.Y;
                double num3 = num * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num4 = num2 * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num5 = myDiyCtcRectangle.m_rectangle.Width * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num6 = myDiyCtcRectangle.m_rectangle.Height * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double size = Math.Sqrt(num5 * num5 + num6 * num6) * ZoomModel.Calibration;
                int num7 = ZoomModel.CtcWind.AddData((int)num3, (int)num4, (int)num5, (int)num6, size);
                AnnoWindHelper.Instance.AnnoDiyCtcRectDic.Add(num7, myDiyCtcRectangle);
                ZoomModel.AnnoWind._AnnotationBase.FontSize = num7;
                ZoomModel.Nav.IsHitTestVisible = true;
            }
            ZoomModel.AnnoWind._AnnotationBase.UpadteTextBlock();
            ZoomModel.AnnoWind._AnnotationBase.UpdateVisual();
            ZoomModel.AnnoWind.Hide();
        }
        private void CancelAnno_Click(object sender, RoutedEventArgs e)
        {
            ZoomModel.AnnoWind._AnnotationBase.DeleteItem();
            ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
            ZoomModel.AnnoWind.Hide();
        }
    }
}
