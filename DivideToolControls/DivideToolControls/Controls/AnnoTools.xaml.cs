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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace DivideToolControls.Controls
{
    /// <summary>
    /// AnnoTools.xaml 的交互逻辑
    /// </summary>
    public partial class AnnoTools : UserControl
    {
        private AnnoLine _annoLine;
        private AnnoArrow _annoArrow;
        private AnnoRect _annoRect;
        private AnnoEllipse _annoEllipse;
        private AnnoPin _annoPin;
        private AnnoPolyline _annoPolyline;
        private AnnoTmaRect _annoTmaRect;
        private AnnoDIYCtcRect _annoDIYCtcRect;
        private MultiScaleImage msi = ZoomModel.MulScaImg;


        public AnnoTools()
        {
            InitializeComponent();
        }



        private void UnLoadHandle()
        {
            RotateTransform renderTransform = new RotateTransform(0.0);
            ZoomModel.RotCtl.RenderTransformOrigin = new Point(0.5, 0.5);
            ZoomModel.RotCtl.Btn_AngleLine.RenderTransform = renderTransform;
            ZoomModel.RotCtl.lbl_Angle.Content = 0 + "°";
            Point center = new Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            Point p = new Point(x, y);
            Point point = KCommon.PointRotate(center, p, 360.0);
            Canvas.SetLeft(ZoomModel.RotCtl.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(ZoomModel.RotCtl.ThumbAngle, point.Y - 7.5);
            RotateHelper.Instance.MsiRotate(0.0);
            ZoomModel.IsDw = false;
            MulScanImgHelper.Instance.UnRegisterMsiEvents(); // 
            if (_annoArrow != null)
            {
                _annoArrow.Unload();
            }
            if (_annoEllipse != null)
            {
                _annoEllipse.Unload();
            }
            if (_annoRect != null)
            {
                _annoRect.Unload();
            }
            if (_annoPin != null)
            {
                _annoPin.Unload();
            }
            if (_annoPolyline != null)
            {
                _annoPolyline.Unload();
            }
            if (_annoLine != null)
            {
                _annoLine.Unload();
            }
            if (_annoDIYCtcRect != null)
            {
                _annoDIYCtcRect.Unload();
            }
            if (_annoTmaRect != null)
            {
                _annoTmaRect.Unload();
            }
            _annoArrow = null;
            _annoEllipse = null;
            _annoRect = null;
            _annoPin = null;
            _annoPolyline = null;
            _annoLine = null;
            _annoDIYCtcRect = null;
            ZoomModel.Nav.IsHitTestVisible = false;
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.IsActive(Visibility.Collapsed);
            }
        }


        private void Btn_Line(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _annoLine = new AnnoLine(ZoomModel.ALC, ZoomModel.Canvasboard, msi, ZoomModel.ObjList, ZoomModel.SlideZoom, ZoomModel.Calibration);
            _annoLine.FinishEvent += AnnoWindHelper.Instance.FinishEvent;
            AnnoWindHelper.Instance.SetAnnoRadioButton(false);
        }

        private void Btn_Rect(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _annoRect = new AnnoRect(ZoomModel.ALC, ZoomModel.Canvasboard, msi, ZoomModel.ObjList, ZoomModel.SlideZoom, ZoomModel.Calibration);
            _annoRect.FinishEvent += AnnoWindHelper.Instance.FinishEvent;
            AnnoWindHelper.Instance.SetAnnoRadioButton(false);
        }
    }
}
