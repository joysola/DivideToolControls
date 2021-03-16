using DivideToolControls.AnnotationControls;
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
    /// TestAnnoBtnList.xaml 的交互逻辑
    /// </summary>
    public partial class TestAnnoBtnList : UserControl
    {
        public TestAnnoBtnList()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 直线按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ALineBtn_Click(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();

            var _aLine = new AnnoLine(ZoomModel.ALC, ZoomModel.Canvasboard, ZoomModel.MulScaImg, ZoomModel.Objectlist, ZoomModel.SlideZoom, ZoomModel.Calibration);
            _aLine.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void FinishEvent(object sender, MouseEventArgs e)
        {
            
        }

        private void UnLoadHandle()
        {
            //RotateTransform renderTransform = new RotateTransform(0.0);
            //_RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            //_RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            //_RotateViewer.lbl_Angle.Content = 0 + "°";
            //System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            //double x = 39.0;
            //double y = 5.5;
            //System.Windows.Point p = new System.Windows.Point(x, y);
            //System.Windows.Point point = KCommon.PointRotate(center, p, 360.0);
            //Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            //Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            //MsiRotate(0.0);
            IsDw = false;
            UnRegisterMsiEvents();
            if (_myArrowLine != null)
            {
                _myArrowLine.unload();
            }
            if (_myEllipse != null)
            {
                _myEllipse.unload();
            }
            if (_myRectangle != null)
            {
                _myRectangle.unload();
            }
            if (_myPin != null)
            {
                _myPin.unload();
            }
            if (_myPolyline != null)
            {
                _myPolyline.unload();
            }
            if (_myLine != null)
            {
                _myLine.unload();
            }
            if (_myDiyCtcRectangle != null)
            {
                _myDiyCtcRectangle.unload();
            }
            if (_myTmaRectangle != null)
            {
                _myTmaRectangle.unload();
            }
            _myArrowLine = null;
            _myEllipse = null;
            _myRectangle = null;
            _myPin = null;
            _myPolyline = null;
            _myLine = null;
            _myDiyCtcRectangle = null;
            nav.IsHitTestVisible = false;
            foreach (AnnotationBase item in objectlist)
            {
                item.IsActive(Visibility.Collapsed);
            }
        }

        private void UnRegisterMsiEvents()
        {
            throw new NotImplementedException();
        }
    }
}
