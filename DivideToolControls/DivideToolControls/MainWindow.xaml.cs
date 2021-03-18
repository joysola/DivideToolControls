using DivideToolControls.Controls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DeepZoomControls;
using DivideToolControls.DynamicGeometry;
using DivideToolControls.Helper;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace DivideToolControls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private MultiScaleImage msi = ZoomModel.MulScaImg;
        private DispatcherTimer _timer;
        private string filePath = @"K:\Test\DivideToolControls\DivideToolControls\DivideToolControls\2020-12-04_15_53_56.kfb";
        
        
        public MainWindow()
        {
            InitializeComponent();
                      
            ClearMemoryThread();
        }

        private void InitTest()
        {
            ZoomModel.CurCtl = this;
            ZoomModel.Bg = this.Bg;
            ZoomModel.Nav = this.nav;
            ZoomModel.X3DSlider = this.x3dSlider;
            ZoomModel.LayoutBody = this.LayoutBody;
            ZoomModel.Canvasboard = this.canvasboard;
            ZoomModel.ZoomCanvas = this.Zoomcanvas;
            ZoomModel.RotCtl = this.RotCtl;
            ZoomModel.ImgLabel = this.Image_lable;
            ZoomModel.RectCans = this.RectCanvas;
            ZoomModel.Magfier = this.Magfier;
            ZoomModel.AnTools = this.AnTools;
            ZoomModel.ScRuler = this.SRuler;
            ZoomModel.OpBall = this.OpBall;
            ZoomModel.RefreshAction = Refresh;
            MulScanImgHelper.Instance.InitAll(filePath);
        }

        public void ClearMemoryThread()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Background);
            _timer.Interval = new TimeSpan(0, 0, 3);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
        private void _timer_Tick(object sender, EventArgs e)
        {
            int privateMemory = MemoryHelper.getPrivateMemory();
            if (privateMemory >= 1048576)
            {
                nav.m_IsDragging = false;
            }
        }

        public void Refresh()
        {
            if (msi.ZoomableCanvas == null)
            {
                return;
            }
            ZoomModel.Curscale = msi.ZoomableCanvas.Scale * (double)ZoomModel.SlideZoom;
            nav.UpdateThumbnailRect();
            nav.DrawRect(msi.ZoomableCanvas.Scale * (double)ZoomModel.SlideZoom);
            double num = 0.0;
            if (ZoomModel.SlideZoom == 40)
            {
                num = Setting.Calibration40;
            }
            else if (ZoomModel.SlideZoom == 20)
            {
                num = Setting.Calibration20;
            }
            if (num == 0.0)
            {
                num = 1.0;
            }
            double num2 = msi.ZoomableCanvas.Scale * (double)ZoomModel.SlideZoom;
            num2 /= num;
            double num3 = Math.Round(num2, 2);
            if (num3 >= (double)(ZoomModel.SlideZoom * (int)Setting.MaxMagValue))
            {
                num3 = ZoomModel.SlideZoom * (int)Setting.MaxMagValue;
            }
            lbl_Scale.Content = Math.Round(num3, 2) + "X";
            ZoomModel.PrevNewzoom = msi.ZoomableCanvas.Scale * (double)ZoomModel.SlideZoom;
            if (num2 > (double)ZoomModel.SlideZoom)
            {
                lbl_Scale.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0));
            }
            else
            {
                lbl_Scale.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            AnnoWindHelper.Instance.ReDraw(); // 画Annotation
            SRuler.UpdateRule(); // 刻度尺
            if (!_timer.IsEnabled)
            {
                _timer.IsEnabled = true;
            }
            ctccanvasboard.Children.Clear();
            if (Math.Round(msi.ZoomableCanvas.Scale * ZoomModel.SlideZoom, 2) != ZoomModel.SlideZoom /*|| CtcWind == null*/)
            {
                return;
            }
            //List<CtcVo> list = CtcWind.listvo.FindAll(delegate (CtcVo user)
            //{
            //    double num6 = user.GlobalPosX;
            //    double num7 = user.GlobalPosY;
            //    _ = user.GlobalPosX;
            //    _ = user.Width;
            //    _ = user.GlobalPosY;
            //    _ = user.Height;
            //    double x2 = msi.ZoomableCanvas.Offset.X;
            //    double y2 = msi.ZoomableCanvas.Offset.Y;
            //    double num8 = x2 + msi.ActualWidth;
            //    double num9 = y2 + msi.ActualHeight;
            //    bool result = false;
            //    if (num6 >= x2 && num7 >= y2 && num6 <= num8 && num7 <= num9)
            //    {
            //        result = true;
            //    }
            //    return result;
            //});

            //for (int i = 0; i < list.Count; i++)
            //{
            //    AnnotationBase annotationBase = new AnnotationBase();
            //    annotationBase.ControlName = annotationBase.AnnotationName;
            //    annotationBase.AnnotationDescription = "";
            //    annotationBase.Size = 2.0;
            //    CtcVo ctcVo = (CtcVo)CtcWind.DgList.SelectedItem;
            //    if (ctcVo != null)
            //    {
            //        if (ctcVo.GlobalPosX == list[i].GlobalPosX && ctcVo.GlobalPosY == list[i].GlobalPosY)
            //        {
            //            annotationBase.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, 0, byte.MaxValue, 0));
            //        }
            //        else
            //        {
            //            annotationBase.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
            //        }
            //    }
            //    else
            //    {
            //        annotationBase.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
            //    }
            //    annotationBase.Zoom = ZoomModel.SlideZoom;
            //    double num4 = (double)list[i].GlobalPosX / (double)ZoomModel.SlideZoom;
            //    double num5 = (double)list[i].GlobalPosY / (double)ZoomModel.SlideZoom;
            //    double x = num4 + (double)list[i].Width / (double)ZoomModel.SlideZoom;
            //    double y = num5 + (double)list[i].Height / (double)ZoomModel.SlideZoom;
            //    annotationBase.CurrentStart = new System.Windows.Point(num4, num5);
            //    annotationBase.CurrentEnd = new System.Windows.Point(x, y);
            //    annotationBase.XmlSetPara(alc, ctccanvasboard, msi, objectlist, ZoomModel.SlideZoom, Calibration);
            //    new myCtcRectangle(annotationBase);
            //}
        }

        private void WindowRendered(object sender, EventArgs e)
        {
            InitTest();
        }
    }
}
