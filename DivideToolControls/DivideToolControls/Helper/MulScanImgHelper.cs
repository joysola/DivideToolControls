using DivideToolControls.AnnotationControls;
using DivideToolControls.Controls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DeepZoomControls;
using DivideToolControls.DynamicGeometry.Enum;
using DivideToolControls.Model;
using DivideToolControls.WinCtls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace DivideToolControls.Helper
{
    public class MulScanImgHelper
    {
        public static MulScanImgHelper Instance { get; } = new MulScanImgHelper();

        private MultiScaleImage msi = ZoomModel.MulScaImg;
        private List<int> touch = new List<int>();
        private bool isDrag;
        private bool IsDrop;
        private Point lastMouseDownPos;
        private Point lastMousePos;
        private List<Image> listImg = new List<Image>();
        private List<AnnoBase> objectList = new List<AnnoBase>();
        private List<AnnoTmaRect> tmaObjList = new List<AnnoTmaRect>();
        private Dictionary<int, AnnoDIYCtcRect> annoDiyCtcRectDic = new Dictionary<int, AnnoDIYCtcRect>();
        private string _filePath;
        private string TempPath;
        private string TempFilename;
        public Visibility isAnnoManageWind = Visibility.Collapsed;
        private List<Image> ListImg = new List<Image>();


        private AnnoListWind _annoListWind;
        private AnnoWind _annoWind;
        private Navmap _nav;
        private CtcList _ctcWind;
        private Slider3D _x3dSlider;
        private Grid _layoutBody;
        private Path _annoPath;
        private Canvas _canvasboard;
        private Grid _bg;
        private Canvas _zoomCanvas;
        /// <summary>
        /// 对应mainpage
        /// </summary>
        public FrameworkElement CurCtl { get; set; }
        public AnnoListWind AnnoListWind { get => _annoListWind; set => _annoListWind = value; }
        public AnnoWind AnnoWind { get => _annoWind; set => _annoWind = value; }
        public Navmap Nav { get => _nav; set => _nav = value; }
        public CtcList CtcWind { get => _ctcWind; set => _ctcWind = value; }
        public Slider3D X3DSlider { get => _x3dSlider; set => _x3dSlider = value; }
        public Grid LayoutBody { get => _layoutBody; set => _layoutBody = value; }
        public Path AnnoPath { get => _annoPath; set => _annoPath = value; }
        public Canvas Canvasboard { get => _canvasboard; set => _canvasboard = value; }
        public Grid Bg { get => _bg; set => _bg = value; }
        public Canvas ZoomCanvas { get => _zoomCanvas; set => _zoomCanvas = value; }
        public Action RefreshAction { get; set; }


        public void InitAll(string filepath)
        {
            _filePath = filepath;
            ZoomModel.Canvasboard = _canvasboard;
            Bg.Children.Add(msi);
            // MultiScaleImage的 ZoomableCanvas的 Scale或Offset属性变化时，通知回调
            LoadMsi(_filePath);
            msi.Ini += msi_Ini;
            RegisterMsiEvents();
            Setting.IsSynchronous = false;
            InitAnnoWinRegisterEvents();
            ZoomableCanvas.Refresh += (sender, e) =>
            {
                RefreshAction?.Invoke();
            };
        }

        public void InitAnnoWinRegisterEvents()
        {
            _annoListWind = new AnnoListWind();
            _annoListWind.Owner = Application.Current.MainWindow;
            _annoListWind.txt_xbz.TextChanged += mc_TextChanged;
            _annoListWind.cbo_mc.SelectionChanged += cbo_mcSelectionChanged;
            _annoListWind.txt_qsr.TextChanged += txt_qsrTextChanged;
            _annoListWind.btnyc.MouseLeftButtonDown += btnyc_change;
            _annoListWind.btnqyc.MouseLeftButtonDown += allhidden_change;
            _annoListWind.ckb_clinfo.Click += ckb_clinfo;
            _annoListWind.btnsc.MouseLeftButtonDown += DeleteItem;
            _annoListWind.btnallsc.MouseLeftButtonDown += btnallsc_MouseLeftButtonDown;
            _annoListWind.cbo_mc.DropDownOpened += DropDownOpened;
            _annoListWind.cbo_mc.DropDownClosed += DropDownClosed;
            _annoListWind.LineWidthComboBox.SelectionChanged += LineWidthComboBox_SelectionChanged;
            _annoListWind._colorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;
            _annoListWind.Rad_1.Checked += Rad_Checked;
            _annoListWind.Rad_2.Checked += Rad_Checked;
            _annoListWind.Rad_3.Checked += Rad_Checked;
            _annoListWind.Rad_4.Checked += Rad_Checked;
            _annoListWind.ShowMs.Click += ShowMs_Click;
            _annoListWind.All_ClShow.Click += All_ClShow_Click;
            _annoListWind.All_ClHidden.Click += All_ClHidden_Click;
            _annoListWind.All_MsShow.Click += All_MsShow_Click;
            _annoListWind.All_MsHidden.Click += All_MsHidden_Click;
            _annoListWind.CloseHandler += AnnoListWind_CloseHandler;
            _annoWind = new AnnoWind();
            _annoWind.Owner = Application.Current.MainWindow;
            _annoWind._SaveAnno.Click += SaveAnno_Click;
            _annoWind._CancelAnno.Click += CancelAnno_Click;
        }

        private void RegisterMsiEvents()
        {
            msi.TouchUp += msi_TouchUp;
            msi.TouchMove += msi_TouchMove;
            msi.TouchDown += msi_TouchDown;
            msi.MouseLeftButtonDown += msi_MouseLeftButtonDown;
            msi.MouseMove += msi_MouseMove;
            msi.MouseLeftButtonUp += msi_MouseLeftButtonMouseUp;
            msi.LostMouseCapture += msi_MouseLeftButtonMouseUp;
            msi.MouseLeave += msi_MouseLeftButtonMouseUp;
            msi.MouseEnter += msi_MouseLeftButtonMouseUp;
            msi.MouseWheel += msi_PreviewMouseWheel;
            msi.MouseDoubleClick += msi_MouseDoubleClick;
            msi.MouseRightButtonDown += msi_MouseRightButtonDown;
            msi.IsManipulationEnabled = true;
            msi.ManipulationStarting += msi_ManipulationStarting;
            msi.ManipulationDelta += msi_ManipulationDelta;
        }
        private void UnRegisterMsiEvents()
        {
            msi.MouseLeftButtonDown -= msi_MouseLeftButtonDown;
            msi.MouseMove -= msi_MouseMove;
            msi.MouseLeftButtonUp -= msi_MouseLeftButtonMouseUp;
            msi.LostMouseCapture -= msi_MouseLeftButtonMouseUp;
            msi.MouseLeave -= msi_MouseLeftButtonMouseUp;
            msi.MouseWheel -= msi_PreviewMouseWheel;
            msi.MouseDoubleClick -= msi_MouseDoubleClick;
            msi.MouseRightButtonDown -= msi_MouseRightButtonDown;
            msi.MouseEnter -= msi_MouseLeftButtonMouseUp;
            msi.TouchUp -= msi_TouchUp;
            msi.TouchMove -= msi_TouchMove;
            msi.TouchDown -= msi_TouchDown;
            msi.IsManipulationEnabled = false;
            msi.ManipulationStarting -= msi_ManipulationStarting;
            msi.ManipulationDelta -= msi_ManipulationDelta;
        }
        #region MSI事件
        private void msi_TouchUp(object sender, TouchEventArgs e)
        {
            touch.Remove(e.TouchDevice.Id);
            isDrag = false;
        }
        private void msi_TouchMove(object sender, TouchEventArgs e)
        {
            if (touch.Count > 1)
            {
                return;
            }
            TouchPoint touchPoint = e.GetTouchPoint(msi);
            lastMousePos = new Point(touchPoint.Position.X, touchPoint.Position.Y);
            if (isDrag)
            {
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        //if ((Mainpage)item.Value != this)
                        //{
                        ZoomModel.IsDw = false;
                        TmoveSetOffset(new Point(lastMouseDownPos.X - lastMousePos.X, lastMouseDownPos.Y - lastMousePos.Y));
                        ZoomHelper.ZoomRatio(msi.ZoomableCanvas.Scale * ZoomModel.SlideZoom, msi, RefreshAction);
                        //}
                    }
                }
                double x = msi.ZoomableCanvas.Offset.X + (lastMouseDownPos.X - lastMousePos.X);
                double y = msi.ZoomableCanvas.Offset.Y + (lastMouseDownPos.Y - lastMousePos.Y);
                msi.ZoomableCanvas.Offset = new Point(x, y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                lastMouseDownPos = lastMousePos;
            }
        }
        private void msi_TouchDown(object sender, TouchEventArgs e)
        {
            ZoomModel.IsDw = false;
            IsArcMenu(Visibility.Collapsed);
            if (_annoListWind != null)
            {
                int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1 && objectList[selectedIndex].isFinish)
                {
                    foreach (AnnoBase item in objectList)
                    {
                        item.IsActive(Visibility.Collapsed);
                        _annoListWind.cbo_mc.SelectedIndex = -1;
                        _annoListWind.txt_qsr.Text = "";
                        _annoListWind.txt_xbz.Text = "";
                        _annoListWind.tbk_info.Text = "";
                        _annoListWind.ckb_clinfo.IsChecked = false;
                    }
                }
            }
            TouchPoint touchPoint = e.GetTouchPoint(msi);
            lastMouseDownPos = new Point(touchPoint.Position.X, touchPoint.Position.Y);
            isDrag = true;
            touch.Add(e.TouchDevice.Id);
        }
        private void msi_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Keyboard.Focus(CurCtl);
            Setting.isCtrl = 0;
            Setting.Opacity = 0;
            ZoomModel.IsDw = false;
            IsArcMenu(Visibility.Collapsed);
            if (_annoListWind != null)
            {
                int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1 && objectList[selectedIndex].isFinish)
                {
                    foreach (AnnoBase item in objectList)
                    {
                        item.IsActive(Visibility.Collapsed);
                        _annoListWind.cbo_mc.SelectedIndex = -1;
                        _annoListWind.txt_qsr.Text = "";
                        _annoListWind.txt_xbz.Text = "";
                        _annoListWind.tbk_info.Text = "";
                        _annoListWind.ckb_clinfo.IsChecked = false;
                    }
                }
            }
            lastMouseDownPos = e.GetPosition(msi);
            isDrag = true;
            Point position = e.GetPosition(CurCtl);
            for (int i = 0; i < objectList.Count; i++)
            {
                AnnoBase annotationBase = objectList[i];
                if (annotationBase.AnnotationType == AnnotationType.TmaRectangle)
                {
                    annotationBase.IsActive(Visibility.Collapsed);
                    double x = annotationBase.MsiToCanvas(annotationBase.CurrentStart).X;
                    double y = annotationBase.MsiToCanvas(annotationBase.CurrentStart).Y;
                    double x2 = annotationBase.MsiToCanvas(annotationBase.CurrentEnd).X;
                    double y2 = annotationBase.MsiToCanvas(annotationBase.CurrentEnd).Y;
                    double num = Math.Abs(x - x2);
                    double num2 = Math.Abs(y - y2);
                    x = Math.Min(x, x2);
                    y = Math.Min(y, y2);
                    double num3 = x;
                    double num4 = y;
                    double num5 = num3 + num;
                    double num6 = num4 + num2;
                    double x3 = position.X;
                    double y3 = position.Y;
                    if (x3 <= num5 && y3 <= num6 && x3 >= num3 && y3 >= num4)
                    {
                        annotationBase.IsActive(Visibility.Visible);
                        annotationBase.AnnoControl.CB.SelectedIndex = objectList.IndexOf(annotationBase);
                    }
                }
            }
        }

        private void msi_MouseMove(object sender, MouseEventArgs e)
        {
            lastMousePos = e.GetPosition(msi);
            if (isDrag)
            {
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        //if ((Mainpage)item.Value != this)
                        //{
                        Point p = new Point(lastMouseDownPos.X - lastMousePos.X, lastMouseDownPos.Y - lastMousePos.Y);
                        Point center = new Point(0.0, 0.0);
                        Point point = KCommon.PointRotate(center, p, 360.0 - ZoomModel.Rotate);
                        ZoomModel.IsDw = false;
                        TmoveSetOffset(new Point(point.X, point.Y));
                        ZoomHelper.ZoomRatio(msi.ZoomableCanvas.Scale * ZoomModel.SlideZoom, msi, RefreshAction);
                        //}
                    }
                }
                double x = msi.ZoomableCanvas.Offset.X + (lastMouseDownPos.X - lastMousePos.X);
                double y = msi.ZoomableCanvas.Offset.Y + (lastMouseDownPos.Y - lastMousePos.Y);
                msi.ZoomableCanvas.Offset = new System.Windows.Point(x, y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                lastMouseDownPos = lastMousePos;
            }
        }
        private void msi_MouseLeftButtonMouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
        }

        private void msi_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int num = e.Timestamp - ZoomModel.PrevTimeStap;
            if (Setting.isCtrl == 0 || _x3dSlider.Visibility == Visibility.Collapsed)
            {
                Setting.Opacity = 1;
                ZoomModel.IsDw = false;
                double num2 = 1.0;
                double num3 = Setting.MaxMagValue * ZoomModel.SlideZoom * Setting.MargPara;
                num2 = ZoomHelper.CalcSpeed(num);
                ZoomModel.PrevTimeStap = e.Timestamp;
                double curscale = ZoomModel.Curscale;
                if (curscale == ZoomModel.PrevNewzoom || !(ZoomModel.PrevTimeStap > 1E-08))
                {
                    double magAdjValueByCurMag = ZoomHelper.GetMagAdjValueByCurMag(ZoomModel.Curscale);
                    curscale = ((e.Delta <= 0) ? (curscale - magAdjValueByCurMag * num2) : (curscale + magAdjValueByCurMag * num2));
                    if (curscale < ZoomModel.Fitratio)
                    {
                        curscale = ZoomModel.Fitratio;
                    }
                    if (curscale > num3)
                    {
                        curscale = num3;
                    }
                    Point position = e.GetPosition(CurCtl);
                    if (Setting.IsSynchronous)
                    {
                        foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                        {
                            ZoomHelper.ZoomRatio(curscale, position.X, position.Y, _layoutBody, msi, RefreshAction);
                        }
                    }
                    ZoomHelper.ZoomRatio(curscale, position.X, position.Y, _layoutBody, msi, RefreshAction);
                    ZoomModel.PrevNewzoom = curscale;
                }
            }
            else if (num > 300)
            {
                ZoomModel.PrevTimeStap = e.Timestamp;
                if (e.Delta > 0)
                {
                    UpZ_MouseLeftButtonDown(null, null);
                }
                else
                {
                    DownZ_MouseLeftButtonDown(null, null);
                }
            }
        }
        private void msi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(CurCtl);
            double num = 2.4000000953674316;
            double curscale = ZoomModel.Curscale;
            curscale *= num;
            if (curscale > ZoomModel.SlideZoom * Setting.MaxMagValue)
            {
                curscale = ZoomModel.SlideZoom * Setting.MaxMagValue;
                num = curscale / ZoomModel.Curscale;
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    SynMsi(ZoomModel.Curscale, position, num);
                }
            }
            Point center = new Point(0.0, 0.0);
            position = new Point(position.X - LayoutBody.ActualWidth / 2.0, position.Y - LayoutBody.ActualHeight / 2.0);
            Point point = KCommon.PointRotate(center, position, ZoomModel.Rotate);
            double x = point.X + msi.ActualWidth / 2.0;
            double y = point.Y + msi.ActualHeight / 2.0;
            position = new Point(x, y);
            double num2 = msi.ZoomableCanvas.Extent.Width * msi.ZoomableCanvas.Scale;
            double num3 = msi.ZoomableCanvas.Extent.Height * msi.ZoomableCanvas.Scale;
            double num4 = msi.ZoomableCanvas.Extent.Width * (curscale / ZoomModel.SlideZoom);
            double num5 = msi.ZoomableCanvas.Extent.Height * (curscale / ZoomModel.SlideZoom);
            Point point2 = new Point(position.X + msi.ZoomableCanvas.Offset.X - num2 / 2.0, position.Y + msi.ZoomableCanvas.Offset.Y - num3 / 2.0);
            Point point3 = new Point(point2.X * num + num4 / 2.0 - position.X - (msi.ActualWidth / 2.0 - position.X), point2.Y * num + num5 / 2.0 - position.Y - (msi.ActualHeight / 2.0 - position.Y));
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(curscale / ZoomModel.SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != curscale / ZoomModel.SlideZoom)
            {
                double num6 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num6 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(curscale / ZoomModel.SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point3.X, point3.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point3.X, point3.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }

        private void msi_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 右击菜单

        }

        private void msi_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = msi;
            e.Mode = ManipulationModes.All;
        }

        private void msi_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (touch.Count == 1)
            {
                return;
            }
            FrameworkElement frameworkElement = (FrameworkElement)e.Source;
            ZoomModel.IsDw = false;
            int timeDelta = e.Timestamp - ZoomModel.PrevTimeStap;
            double num = Setting.MaxMagValue * ZoomModel.SlideZoom;
            ZoomHelper.CalcSpeed(timeDelta);
            ZoomModel.PrevTimeStap = e.Timestamp;
            double num2 = ZoomModel.Curscale;
            if (num2 == ZoomModel.PrevNewzoom || !(ZoomModel.PrevNewzoom > 1E-08))
            {
                ZoomHelper.GetMagAdjValueByCurMag(ZoomModel.Curscale);
                if (e.DeltaManipulation.Scale.X > 1.0 || e.DeltaManipulation.Scale.Y > 1.0)
                {
                    double num3 = Math.Max(e.DeltaManipulation.Scale.Y, e.DeltaManipulation.Scale.X);
                    num2 *= num3;
                }
                else if (e.DeltaManipulation.Scale.X != 0.0 && e.DeltaManipulation.Scale.Y != 0.0)
                {
                    double num4 = Math.Max(e.DeltaManipulation.Scale.Y, e.DeltaManipulation.Scale.X);
                    num2 *= num4;
                }
                if (num2 > num)
                {
                    num2 = num;
                }
                Point point = new Point(frameworkElement.ActualWidth / 2.0, frameworkElement.ActualHeight / 2.0);
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {

                        ZoomHelper.ZoomRatio(num2, point.X, point.Y, _layoutBody, msi, RefreshAction);

                    }
                }
                ZoomHelper.ZoomRatio(num2, point.X, point.Y, _layoutBody, msi, RefreshAction);
                ZoomModel.PrevNewzoom = num2;
            }
        }

        /// <summary>
        /// 需要修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msi_Ini(object sender, RoutedEventArgs e)
        {
            string filePath = _filePath;

            TempPath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
            TempFilename = filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1);

            msi.Tag = "1";
            ZoomModel.ALC.CB = AnnoListWind.cbo_mc;
            ZoomModel.ALC.Tbk = AnnoListWind.tbk_info;
            ZoomModel.ALC.Tbx = AnnoListWind.txt_xbz;
            ZoomModel.ALC.qsr = AnnoListWind.txt_qsr;

            ArcMenu();
            ZoomModel.Fitratio = ZoomModel.SlideZoom * msi.ZoomableCanvas.Scale;

            RefreshAction?.Invoke();
        }

        public void LoadMsi(string filename)
        {
            IniFile(filename);
            int khiImageHeight = 1;
            int khiImageWidth = 2;
            int khiScanScale = 3;
            float khiSpendTime = 0f;
            double khiScanTime = 0.0;
            float khiImageCapRes = 0f;
            if (filename.IndexOf(".kfb") != -1)
            {
                DllImageFuc.GetScanLevelInfoFunc(ref ZoomModel.InfoStruct, ref ZoomModel.nCurLevel, ref ZoomModel.nTotalLevel);
                if (ZoomModel.nTotalLevel > 2)
                {
                    if (ZoomModel.nTotalLevel % 2 == 0)
                    {
                        ZoomModel.MinLevel = -ZoomModel.nTotalLevel / 2 + 1;
                        ZoomModel.MaxLevel = ZoomModel.nTotalLevel / 2;
                    }
                    else
                    {
                        ZoomModel.MinLevel = -(ZoomModel.nTotalLevel - 1) / 2;
                        ZoomModel.MaxLevel = (ZoomModel.nTotalLevel - 1) / 2;
                    }
                    X3DSlider.Zvalue.Content = ZoomModel.nCurLevel;
                    if (ZoomModel.nCurLevel == 0)
                    {
                        ZoomModel.LevelFilePath = filename;
                    }
                    else
                    {
                        ZoomModel.LevelFilePath = filename.Replace("_" + ZoomModel.nCurLevel + ".kfb", ".kfb");
                    }
                    if (CheckAllLevel())
                    {
                        X3DSlider.Visibility = Visibility.Visible;
                    }
                    if (filename == ZoomModel.LevelFilePath && ZoomModel.nCurLevel != 0)
                    {
                        X3DSlider.Visibility = Visibility.Collapsed;
                    }
                }
            }
            ZoomModel.DllImgFunc.CkGetHeaderInfoFunc(ZoomModel.InfoStruct, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref ZoomModel.TileSize);
            if (ZoomModel.TileSize == 0)
            {
                ZoomModel.TileSize = 256;
            }
            //msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, TileSize, 0, InfoStruct, khiScanScale, msi);
            msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, ZoomModel.TileSize, 0, ZoomModel.InfoStruct, khiScanScale, msi, filename);
            if ((double)khiImageCapRes == 0.0)
            {
                switch (khiScanScale)
                {
                    case 20:
                        ZoomModel.Calibration = 0.5;
                        break;
                    case 40:
                        ZoomModel.Calibration = 0.2439;
                        break;
                }
            }
            else
            {
                ZoomModel.Calibration = khiImageCapRes;
                if (khiScanScale == 40)
                {
                    if (Setting.Calibration40 != 1.0)
                    {
                        Setting.MargPara = Setting.Calibration40;
                    }
                    if (Setting.CalibrationX40 != 1.0)
                    {
                        ZoomModel.Calibration = Setting.CalibrationX40;
                    }
                }
                if (khiScanScale == 20)
                {
                    if (Setting.Calibration20 != 1.0)
                    {
                        Setting.MargPara = Setting.Calibration20;
                    }
                    if (Setting.CalibrationX20 != 1.0)
                    {
                        ZoomModel.Calibration = Setting.CalibrationX20;
                    }
                }
            }
            ZoomModel.SlideZoom = khiScanScale;
            ZoomModel.ImageW = khiImageWidth;
            ZoomModel.ImageH = khiImageHeight;
        }


        private void TmoveSetOffset(Point p)
        {
            Point offset = msi.ZoomableCanvas.Offset;
            Point p2 = new Point(p.X, p.Y);
            Point center = new Point(0.0, 0.0);
            Point point = KCommon.PointRotate(center, p2, ZoomModel.Rotate);
            offset.Y += point.Y;
            offset.X += point.X;
            msi.ZoomableCanvas.Offset = new Point(offset.X, offset.Y);
            msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }
        private void IsArcMenu(Visibility v)
        {
            _annoPath.Visibility = v;
            int count = listImg.Count;
            for (int i = 0; i < count; i++)
            {
                listImg[i].Visibility = v;
            }
        }
        private void UpZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string upLevelFileName = GetUpLevelFileName();
            if (upLevelFileName != "")
            {
                if (ZoomModel.InfoStruct.DataFilePTR != 0)
                {
                    ZoomModel.DllImgFunc.CkUnInitImageFileFunc(ref ZoomModel.InfoStruct);
                }
                ZoomModel.DllImgFunc.CkInitImageFileFunc(ref ZoomModel.InfoStruct, upLevelFileName);
                ((MagicZoomTileSource1)msi.Source).SetinfoStruct(ZoomModel.InfoStruct);
                LinkedListNode<int> linkedListNode = msi.ZoomableCanvas.RealizedItems.First;
                LinkedListNode<int> last = msi.ZoomableCanvas.RealizedItems.Last;
                Setting.Opacity = 1;
                while (linkedListNode != last)
                {
                    LinkedListNode<int> next = linkedListNode.Next;
                    int value = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value);
                    msi.ZoomableCanvas.RealizeItem(value);
                    linkedListNode = next;
                }
                if (linkedListNode == last)
                {
                    int value2 = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value2);
                    msi.ZoomableCanvas.RealizeItem(value2);
                }
            }
        }
        private void DownZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string lowLevelFileName = GetLowLevelFileName();
            if (lowLevelFileName != "")
            {
                if (ZoomModel.InfoStruct.DataFilePTR != 0)
                {
                    ZoomModel.DllImgFunc.CkUnInitImageFileFunc(ref ZoomModel.InfoStruct);
                }
                ZoomModel.DllImgFunc.CkInitImageFileFunc(ref ZoomModel.InfoStruct, lowLevelFileName);
                ((MagicZoomTileSource1)msi.Source).SetinfoStruct(ZoomModel.InfoStruct);
                LinkedListNode<int> linkedListNode = msi.ZoomableCanvas.RealizedItems.First;
                LinkedListNode<int> last = msi.ZoomableCanvas.RealizedItems.Last;
                Setting.Opacity = 1;
                while (linkedListNode != last)
                {
                    LinkedListNode<int> next = linkedListNode.Next;
                    int value = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value);
                    msi.ZoomableCanvas.RealizeItem(value);
                    linkedListNode = next;
                }
                if (linkedListNode == last)
                {
                    int value2 = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value2);
                    msi.ZoomableCanvas.RealizeItem(value2);
                }
            }
        }
        private string GetUpLevelFileName()
        {
            int num = int.Parse(_x3dSlider.Zvalue.Content.ToString()) + 1;
            if (num > ZoomModel.MaxLevel)
            {
                return "";
            }
            string text = ZoomModel.LevelFilePath;
            if (num != 0)
            {
                text = ZoomModel.LevelFilePath.Replace(".kfb", "_" + num + ".kfb");
            }
            if (File.Exists(text))
            {
                _x3dSlider.Zvalue.Content = num;
                return text;
            }
            for (int i = 2; i < 99999; i++)
            {
                text = ZoomModel.LevelFilePath;
                int num2 = int.Parse(_x3dSlider.Zvalue.Content.ToString()) + i;
                if (num2 > ZoomModel.MaxLevel)
                {
                    return "";
                }
                if (num2 != 0)
                {
                    text = ZoomModel.LevelFilePath.Replace(".kfb", "_" + num2 + ".kfb");
                }
                if (File.Exists(text))
                {
                    _x3dSlider.Zvalue.Content = num2;
                    return text;
                }
            }
            return "";
        }
        public string GetLowLevelFileName()
        {
            int num = int.Parse(_x3dSlider.Zvalue.Content.ToString()) - 1;
            if (num < ZoomModel.MinLevel)
            {
                return "";
            }
            string text = ZoomModel.LevelFilePath;
            if (num != 0)
            {
                text = ZoomModel.LevelFilePath.Replace(".kfb", "_" + num + ".kfb");
            }
            if (File.Exists(text))
            {
                _x3dSlider.Zvalue.Content = num;
                return text;
            }
            for (int i = 2; i < 99999; i++)
            {
                text = ZoomModel.LevelFilePath;
                int num2 = int.Parse(_x3dSlider.Zvalue.Content.ToString()) - i;
                if (num2 < ZoomModel.MinLevel)
                {
                    return "";
                }
                if (num2 != 0)
                {
                    text = ZoomModel.LevelFilePath.Replace(".kfb", "_" + num2 + ".kfb");
                }
                if (File.Exists(text))
                {
                    _x3dSlider.Zvalue.Content = num2;
                    return text;
                }
            }
            return "";
        }
        #endregion

        #region 注册事件
        private void mc_TextChanged(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                objectList[selectedIndex].AnnotationName = _annoListWind.txt_xbz.Text;
                _annoListWind.cbo_mc.ItemsSource = null;
                _annoListWind.cbo_mc.ItemsSource = objectList;
                _annoListWind.cbo_mc.SelectedIndex = selectedIndex;
            }
        }
        private void cbo_mcSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            foreach (AnnoBase item in objectList)
            {
                item.IsActive(Visibility.Collapsed);
            }
            _annoListWind.txt_qsr.Text = objectList[selectedIndex].AnnotationDescription;
            _annoListWind.txt_xbz.Text = objectList[selectedIndex].AnnotationName;
            _annoListWind.tbk_info.Text = objectList[selectedIndex].CalcMeasureInfo1();
            _annoListWind.ckb_clinfo.IsChecked = ((objectList[selectedIndex].isVisble == Visibility.Visible) ? true : false);
            _annoListWind.ShowMs.IsChecked = objectList[selectedIndex].isMsVisble;
            _annoListWind._colorPicker.SelectedColor = ((SolidColorBrush)objectList[selectedIndex].BorderBrush).Color;
            _annoListWind.LineWidthComboBox.SelectedValue = objectList[selectedIndex].Size;
            foreach (ComboBoxItem item2 in (IEnumerable)_annoListWind.LineWidthComboBox.Items)
            {
                if (item2.Content.Equals(objectList[selectedIndex].Size.ToString()))
                {
                    _annoListWind.LineWidthComboBox.SelectedItem = item2;
                    break;
                }
            }
            if (objectList[selectedIndex].isHidden == Visibility.Visible)
            {
                objectList[selectedIndex].IsActive(Visibility.Visible);
            }
            if (objectList[selectedIndex].isHidden == Visibility.Visible)
            {
                _annoListWind.btnyc.Opacity = 1.0;
                _annoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
                objectList[selectedIndex].isHidden = Visibility.Visible;
            }
            else
            {
                _annoListWind.btnyc.Opacity = 0.5;
                _annoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                objectList[selectedIndex].isHidden = Visibility.Collapsed;
            }
            if (objectList[selectedIndex].isFinish && IsDrop)
            {
                dw(sender, new RoutedEventArgs());
            }
            if (objectList[selectedIndex].GetType() == typeof(AnnoPin))
            {
                SetAnnoListRadioButton(true);
                string a = objectList[selectedIndex].PinType.Substring(12, 1);
                if (a == "1")
                {
                    _annoListWind.Rad_1.IsChecked = true;
                }
                else if (a == "2")
                {
                    _annoListWind.Rad_2.IsChecked = true;
                }
                else if (a == "3")
                {
                    _annoListWind.Rad_3.IsChecked = true;
                }
                else if (a == "4")
                {
                    _annoListWind.Rad_4.IsChecked = true;
                }
            }
            else
            {
                SetAnnoListRadioButton(false);
            }
            ReDraw();
        }
        private void txt_qsrTextChanged(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                objectList[selectedIndex].AnnotationDescription = _annoListWind.txt_qsr.Text;
                objectList[selectedIndex].UpadteTextBlock();
            }
        }
        private void btnyc_change(object sender, RoutedEventArgs e)
        {
            int num = 0;
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            if (_annoListWind.btnyc.Opacity == 0.5)
            {
                objectList[selectedIndex].IsActive(Visibility.Visible);
                if (_annoListWind.ckb_clinfo.IsChecked == true)
                {
                    objectList[selectedIndex].isVisble = Visibility.Visible;
                }
                objectList[selectedIndex].isHidden = Visibility.Visible;
                foreach (AnnoBase item in objectList)
                {
                    if (item.isHidden == Visibility.Visible)
                    {
                        num++;
                        if (num == objectList.Count)
                        {
                            _annoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部隐藏");
                            _annoListWind.btnqyc.Opacity = 1.0;
                        }
                    }
                }
                _annoListWind.btnyc.Opacity = 1.0;
                _annoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
            }
            else
            {
                num = 0;
                _annoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                objectList[selectedIndex].IsActive(Visibility.Collapsed);
                objectList[selectedIndex].isVisble = Visibility.Collapsed;
                objectList[selectedIndex].isHidden = Visibility.Collapsed;
                _annoListWind.btnyc.Opacity = 0.5;
                foreach (AnnoBase item2 in objectList)
                {
                    if (item2.isHidden == Visibility.Collapsed)
                    {
                        num++;
                        if (num == objectList.Count)
                        {
                            _annoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部显示");
                            _annoListWind.btnqyc.Opacity = 0.5;
                        }
                    }
                }
            }
            ReDraw();
        }
        private void allhidden_change(object sender, RoutedEventArgs e)
        {
            if (_annoListWind.btnqyc.Opacity == 1.0)
            {
                foreach (AnnoBase item in objectList)
                {
                    item.isHidden = Visibility.Collapsed;
                    item.IsActive(Visibility.Collapsed);
                    item.MTextBlock.Visibility = Visibility.Collapsed;
                    item.UpdateVisual();
                }
                _annoListWind.cbo_mc.SelectedIndex = -1;
                _annoListWind.txt_qsr.Text = "";
                _annoListWind.txt_xbz.Text = "";
                _annoListWind.tbk_info.Text = "";
                _annoListWind.btnyc.Opacity = 0.5;
                _annoListWind.btnqyc.Opacity = 0.5;
                _annoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                _annoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部显示");
            }
            else
            {
                _annoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
                _annoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部隐藏");
                foreach (AnnoBase item2 in objectList)
                {
                    item2.isHidden = Visibility.Visible;
                    item2.isVisble = Visibility.Visible;
                    item2.MTextBlock.Visibility = Visibility.Visible;
                    item2.UpdateVisual();
                }
                _annoListWind.btnyc.Opacity = 1.0;
                _annoListWind.btnqyc.Opacity = 1.0;
            }
        }
        private void ckb_clinfo(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (_annoListWind.ckb_clinfo.IsChecked == true && objectList[selectedIndex].isHidden == Visibility.Visible)
                {
                    objectList[selectedIndex].isVisble = Visibility.Visible;
                }
                else
                {
                    objectList[selectedIndex].isVisble = Visibility.Collapsed;
                }
                ReDraw();
            }
        }
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            Delete();
            AnnoTmaRec_FinishEvent(null, null);
        }
        public void Delete()
        {
            if (_annoListWind.cbo_mc.SelectedIndex != -1)
            {
                object obj = objectList[_annoListWind.cbo_mc.SelectedIndex];
                ((AnnoBase)obj).DeleteItem();
                _annoListWind.cbo_mc.ItemsSource = null;
                _annoListWind.cbo_mc.ItemsSource = objectList;
                if (objectList.Count == 0)
                {
                    _annoListWind.txt_qsr.Text = "";
                    _annoListWind.txt_xbz.Text = "";
                    _annoListWind.tbk_info.Text = "";
                }
                else
                {
                    _annoListWind.cbo_mc.SelectedIndex = -1;
                }
            }
        }
        private void AnnoTmaRec_FinishEvent(object sender, MouseEventArgs e)
        {
            if (tmaObjList.Count == 0)
            {
                return;
            }
            List<AnnoTmaRect> list = new List<AnnoTmaRect>();
            List<AnnoTmaRect> list2 = tmaObjList.OrderBy(s => s.CurrentStart.Y * ZoomModel.Curscale).ToList();
            Dictionary<int, List<AnnoTmaRect>> dictionary = new Dictionary<int, List<AnnoTmaRect>>();
            List<AnnoTmaRect> list3 = new List<AnnoTmaRect>();
            double num = Math.Abs(tmaObjList[0].CurrentEnd.Y - tmaObjList[0].CurrentStart.Y) / 3.0;
            int num2 = 0;
            for (int i = 0; i < list2.Count; i++)
            {
                if (i < list2.Count - 1)
                {
                    double y = list2[i].CurrentEnd.Y;
                    double y2 = list2[i + 1].CurrentEnd.Y;
                    double num3 = Math.Abs(y - y2);
                    if (num3 > num)
                    {
                        list3.Add(list2[i]);
                        dictionary.Add(num2, list3);
                        num2++;
                        list3 = new List<AnnoTmaRect>();
                    }
                    else
                    {
                        list3.Add(list2[i]);
                    }
                }
                else
                {
                    list3.Add(list2[i]);
                    dictionary.Add(num2, list3);
                }
            }
            foreach (KeyValuePair<int, List<AnnoTmaRect>> item in dictionary)
            {
                List<AnnoTmaRect> collection = item.Value.OrderBy(s => s.CurrentStart.X * ZoomModel.Curscale).ToList();
                list.AddRange(collection);
            }
            Mi_TMAClear_Click(null, null);
            tmaObjList.Clear();
            for (int num4 = list.Count - 1; num4 >= 0; num4--)
            {
                AnnoTmaRect myTmaRectangle = list[num4];
                double x = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart).X;
                double y3 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart).Y;
                double x2 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd).X;
                double y4 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd).Y;
                double num5 = Math.Abs(x - x2);
                double num6 = Math.Abs(y3 - y4);
                x = Math.Min(x, x2);
                y3 = Math.Min(y3, y4);
                double left = x;
                double top = y3;
                double width = num5;
                double height = num6;
                AnnoTmaRect myTmaRectangle2 = new AnnoTmaRect(ZoomModel.ALC, ZoomModel.Canvasboard, msi, objectList, ZoomModel.SlideZoom, ZoomModel.Calibration, tmaObjList);
                myTmaRectangle2.DrawRect(left, top, width, height, (num4 + 1).ToString(), myTmaRectangle.AnnotationDescription);
            }
            RegisterMsiEvents();
            _nav.IsHitTestVisible = true;
            _annoListWind.cbo_mc.SelectedIndex = -1;
        }
        private void Mi_TMAClear_Click(object sender, RoutedEventArgs e)
        {
            List<AnnoBase> list = new List<AnnoBase>();
            foreach (AnnoBase item in objectList)
            {
                if (item.AnnotationType == AnnotationType.myTmaLine || item.AnnotationType == AnnotationType.TmaRectangle)
                {
                    list.Add(item);
                }
            }
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                list[i].DeleteItem();
            }
            list.Clear();
        }
        private void btnallsc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int count = objectList.Count;
            for (int i = 0; i < count; i++)
            {
                objectList[0].DeleteItem();
            }
            _annoListWind.cbo_mc.SelectedIndex = -1;
            _annoListWind.cbo_mc.ItemsSource = null;
            _annoListWind.cbo_mc.ItemsSource = objectList;
        }

        public void DropDownOpened(object sender, EventArgs e)
        {
            IsDrop = true;
        }

        public void DropDownClosed(object sender, EventArgs e)
        {
            IsDrop = false;
        }
        private void LineWidthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_annoListWind.LineWidthComboBox.SelectedItem != null)
            {
                int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1)
                {
                    objectList[selectedIndex].Size = int.Parse((_annoListWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
                    objectList[selectedIndex].UpdateVisual();
                }
            }
        }
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                objectList[selectedIndex].BorderBrush = new SolidColorBrush(_annoListWind._colorPicker.SelectedColor);
                if (objectList[selectedIndex].GetType() == typeof(AnnoPin))
                {
                    objectList[selectedIndex].FontColor = new SolidColorBrush(_annoListWind._colorPicker.SelectedColor);
                }
                objectList[selectedIndex].UpdateVisual();
            }
        }
        private void Rad_Checked(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                string a = ((System.Windows.Controls.RadioButton)sender).Name.ToString();
                if (a == "Rad_1")
                {
                    objectList[selectedIndex].PinType = "images/pin_1.png";
                }
                else if (a == "Rad_2")
                {
                    objectList[selectedIndex].PinType = "images/pin_2.png";
                }
                else if (a == "Rad_3")
                {
                    objectList[selectedIndex].PinType = "images/pin_3.png";
                }
                else if (a == "Rad_4")
                {
                    objectList[selectedIndex].PinType = "images/pin_4.png";
                }
                objectList[selectedIndex].UpdateVisual();
            }
        }
        private void ShowMs_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (_annoListWind.ShowMs.IsChecked == true)
                {
                    objectList[selectedIndex].isMsVisble = true;
                }
                else
                {
                    objectList[selectedIndex].isMsVisble = false;
                }
                objectList[selectedIndex].UpdateVisual();
            }
        }

        private void All_ClShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in objectList)
            {
                item.isVisble = Visibility.Visible;
                item.UpdateVisual();
            }
            if (_annoListWind.cbo_mc.SelectedIndex != -1)
            {
                _annoListWind.ckb_clinfo.IsChecked = true;
            }
        }
        private void All_ClHidden_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in objectList)
            {
                item.isVisble = Visibility.Collapsed;
                item.UpdateVisual();
            }
            _annoListWind.ckb_clinfo.IsChecked = false;
        }
        private void All_MsShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in objectList)
            {
                item.isMsVisble = true;
                item.UpdateVisual();
            }
            if (_annoListWind.cbo_mc.SelectedIndex != -1)
            {
                _annoListWind.ShowMs.IsChecked = true;
            }
        }
        private void All_MsHidden_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in objectList)
            {
                item.isMsVisble = false;
                item.UpdateVisual();
            }
            _annoListWind.ShowMs.IsChecked = false;
        }
        private void AnnoListWind_CloseHandler(object sender, RoutedEventArgs e)
        {
            _annoListWind.Visibility = Visibility.Collapsed;
            isAnnoManageWind = Visibility.Collapsed;
        }

        private void SaveAnno_Click(object sender, RoutedEventArgs e)
        {
            _annoWind._AnnotationBase.Size = int.Parse((_annoWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
            _annoWind._AnnotationBase.AnnotationName = _annoWind.AnnoName.Text;
            _annoWind._AnnotationBase.AnnotationDescription = _annoWind.AnnoDes.Text;
            _annoWind._AnnotationBase.BorderBrush = new SolidColorBrush(_annoWind._colorPicker.SelectedColor);
            _annoWind._AnnotationBase.isVisble = ((_annoWind.ShowInfo.IsChecked != true) ? Visibility.Collapsed : Visibility.Visible);
            _annoWind._AnnotationBase.isMsVisble = ((_annoWind.ShowMs.IsChecked == true) ? true : false);
            if (_annoWind._AnnotationBase.GetType() == typeof(AnnoPin))
            {
                if (_annoWind.Rad_1.IsChecked == true)
                {
                    _annoWind._AnnotationBase.PinType = "images/pin_1.png";
                }
                else if (_annoWind.Rad_2.IsChecked == true)
                {
                    _annoWind._AnnotationBase.PinType = "images/pin_2.png";
                }
                else if (_annoWind.Rad_3.IsChecked == true)
                {
                    _annoWind._AnnotationBase.PinType = "images/pin_3.png";
                }
                else if (_annoWind.Rad_4.IsChecked == true)
                {
                    _annoWind._AnnotationBase.PinType = "images/pin_4.png";
                }
                _annoWind._AnnotationBase.FontColor = new SolidColorBrush(_annoWind._colorPicker.SelectedColor);
            }
            if (_annoWind._AnnotationBase.AnnotationType == AnnotationType.DiyCtcRectangle)
            {
                AnnoDIYCtcRect myDiyCtcRectangle = (AnnoDIYCtcRect)_annoWind._AnnotationBase;
                System.Windows.Point originStart = myDiyCtcRectangle.OriginStart;
                double num = msi.ZoomableCanvas.Offset.X + originStart.X;
                double num2 = msi.ZoomableCanvas.Offset.Y + originStart.Y;
                double num3 = num * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num4 = num2 * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num5 = myDiyCtcRectangle.m_rectangle.Width * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num6 = myDiyCtcRectangle.m_rectangle.Height * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double size = Math.Sqrt(num5 * num5 + num6 * num6) * ZoomModel.Calibration;
                int num7 = _ctcWind.AddData((int)num3, (int)num4, (int)num5, (int)num6, size);
                annoDiyCtcRectDic.Add(num7, myDiyCtcRectangle);
                _annoWind._AnnotationBase.FontSize = num7;
                _nav.IsHitTestVisible = true;
            }
            _annoWind._AnnotationBase.UpadteTextBlock();
            _annoWind._AnnotationBase.UpdateVisual();
            _annoWind.Hide();
        }
        private void CancelAnno_Click(object sender, RoutedEventArgs e)
        {
            _annoWind._AnnotationBase.DeleteItem();
            _annoListWind.cbo_mc.SelectedIndex = -1;
            _annoWind.Hide();
        }
        private void dw(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _annoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                AnnoBase annotationBase = objectList[selectedIndex];
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        //if ((Mainpage)item.Value != this)
                        //{
                        SynMsiAnno(annotationBase);
                        //}
                    }
                }
                double num = annotationBase.CurrentEnd.X * annotationBase.Zoom - annotationBase.Zoom * annotationBase.CurrentStart.X;
                double num2 = annotationBase.CurrentEnd.Y * annotationBase.Zoom - annotationBase.Zoom * annotationBase.CurrentStart.Y;
                System.Windows.Point point = new System.Windows.Point(annotationBase.Zoom * annotationBase.CurrentStart.X - msi.ActualWidth / 2.0 + num / 2.0, annotationBase.Zoom * annotationBase.CurrentStart.Y - msi.ActualHeight / 2.0 + num2 / 2.0);
                double scale = msi.ZoomableCanvas.Scale;
                int level = msi.Source.GetLevel(annotationBase.Zoom / ZoomModel.SlideZoom);
                int currentLevel = msi._spatialSource.CurrentLevel;
                if (level != currentLevel)
                {
                    msi._spatialSource.CurrentLevel = level;
                }
                if (scale != annotationBase.Zoom / (double)ZoomModel.SlideZoom)
                {
                    double num3 = 4.0;
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(num3 * 100.0);
                    CubicEase easingFunction = new CubicEase();
                    msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(annotationBase.Zoom / (double)ZoomModel.SlideZoom, timeSpan)
                    {
                        EasingFunction = easingFunction
                    }, HandoffBehavior.Compose);
                    msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point.X, point.Y), timeSpan)
                    {
                        EasingFunction = easingFunction
                    }, HandoffBehavior.Compose);
                }
                else
                {
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                }
            }
        }
        public void SetAnnoRadioButton(bool v)
        {
            _annoWind.Rad_1.IsEnabled = v;
            _annoWind.Rad_2.IsEnabled = v;
            _annoWind.Rad_3.IsEnabled = v;
            _annoWind.Rad_4.IsEnabled = v;
        }

        public void SetAnnoListRadioButton(bool v)
        {
            _annoListWind.Rad_1.IsEnabled = v;
            _annoListWind.Rad_2.IsEnabled = v;
            _annoListWind.Rad_3.IsEnabled = v;
            _annoListWind.Rad_4.IsEnabled = v;
        }
        public void ReDraw()
        {
            foreach (AnnoBase item in objectList)
            {
                item.UpdateVisual();
            }
        }
        public void SynMsiAnno(AnnoBase ab)
        {
            double num = ab.CurrentEnd.X * ab.Zoom - ab.Zoom * ab.CurrentStart.X;
            double num2 = ab.CurrentEnd.Y * ab.Zoom - ab.Zoom * ab.CurrentStart.Y;
            System.Windows.Point point = new Point(ab.Zoom * ab.CurrentStart.X - msi.ActualWidth / 2.0 + num / 2.0, ab.Zoom * ab.CurrentStart.Y - msi.ActualHeight / 2.0 + num2 / 2.0);
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(ab.Zoom / (double)ZoomModel.SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != ab.Zoom / (double)ZoomModel.SlideZoom)
            {
                double num3 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num3 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(ab.Zoom / (double)ZoomModel.SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new Point(point.X, point.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }
        #endregion 注册事件


        private void IniFile(string fileName)
        {
            ZoomModel.InfoStruct.DataFilePTR = 0;
            ZoomModel.DllImgFunc.CkInitImageFileFunc(ref ZoomModel.InfoStruct, fileName);
        }
        private bool CheckAllLevel()
        {
            for (int i = ZoomModel.MinLevel; i < ZoomModel.MinLevel + ZoomModel.nTotalLevel; i++)
            {
                string path = ZoomModel.LevelFilePath;
                if (i != 0)
                {
                    path = ZoomModel.LevelFilePath.Replace(".kfb", "_" + i + ".kfb");
                }
                if (File.Exists(path) && i != ZoomModel.nCurLevel)
                {
                    return true;
                }
            }
            return false;
        }

        private void SynMsi(double newzoom, System.Windows.Point p, double dbscale)
        {
            newzoom *= dbscale;
            if (newzoom > ZoomModel.SlideZoom * Setting.MaxMagValue)
            {
                newzoom = ZoomModel.SlideZoom * Setting.MaxMagValue;
                dbscale = newzoom / ZoomModel.Curscale;
            }
            Point center = new Point(0.0, 0.0);
            p = new Point(p.X - LayoutBody.ActualWidth / 2.0, p.Y - LayoutBody.ActualHeight / 2.0);
            Point point = KCommon.PointRotate(center, p, ZoomModel.Rotate);
            double x = point.X + msi.ActualWidth / 2.0;
            double y = point.Y + msi.ActualHeight / 2.0;
            p = new Point(x, y);
            double num = msi.ZoomableCanvas.Extent.Width * msi.ZoomableCanvas.Scale;
            double num2 = msi.ZoomableCanvas.Extent.Height * msi.ZoomableCanvas.Scale;
            double num3 = msi.ZoomableCanvas.Extent.Width * (newzoom / ZoomModel.SlideZoom);
            double num4 = msi.ZoomableCanvas.Extent.Height * (newzoom / ZoomModel.SlideZoom);
            Point point2 = new System.Windows.Point(p.X + msi.ZoomableCanvas.Offset.X - num / 2.0, p.Y + msi.ZoomableCanvas.Offset.Y - num2 / 2.0);
            Point point3 = new System.Windows.Point(point2.X * dbscale + num3 / 2.0 - p.X - (msi.ActualWidth / 2.0 - p.X), point2.Y * dbscale + num4 / 2.0 - p.Y - (msi.ActualHeight / 2.0 - p.Y));
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(newzoom / ZoomModel.SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != newzoom / ZoomModel.SlideZoom)
            {
                double num5 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num5 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(newzoom / ZoomModel.SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new Point(point3.X, point3.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point3.X, point3.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }
        private void ArcMenu()
        {
            if (AnnoPath != null)
            {
                ZoomCanvas.Children.Remove(AnnoPath);
                int count = ListImg.Count;
                for (int i = 0; i < count; i++)
                {
                    ZoomCanvas.Children.Remove(ListImg[i]);
                }
                ListImg.Clear();
            }
            AnnoPath = new Path();
            AnnoPath.Visibility = Visibility.Collapsed;
            ZoomCanvas.Children.Add(AnnoPath);
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            ArcSegment arcSegment = new ArcSegment();
            double num = msi.ActualWidth;
            double num2 = msi.ActualHeight;
            if (ZoomModel.Rotate > 0.0 && num > Setting.AngelMsiOffset * 2.0 && num2 > Setting.AngelMsiOffset * 2.0)
            {
                num -= Setting.AngelMsiOffset * 2.0;
                num2 -= Setting.AngelMsiOffset * 2.0;
            }
            pathFigure.StartPoint = new Point(num - 150.0, num2);
            arcSegment.Point = new Point(num, num2 - 150.0);
            arcSegment.Size = new Size(1.0, 1.0);
            AnnoPath.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 74, 132));
            AnnoPath.StrokeThickness = 4.0;
            arcSegment.IsLargeArc = true;
            arcSegment.SweepDirection = SweepDirection.Clockwise;
            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);
            AnnoPath.Data = pathGeometry;
            ZoomCanvas.Children.Add(DrawZoomImage(num - 50.0, num2 - 200.0, "1"));
            ZoomCanvas.Children.Add(DrawZoomImage(num - 100.0, num2 - 210.0, "2"));
            ZoomCanvas.Children.Add(DrawZoomImage(num - 150.0, num2 - 200.0, "4"));
            ZoomCanvas.Children.Add(DrawZoomImage(num - 190.0, num2 - 170.0, "10"));
            ZoomCanvas.Children.Add(DrawZoomImage(num - 210.0, num2 - 120.0, "20"));
            ZoomCanvas.Children.Add(DrawZoomImage(num - 200.0, num2 - 60.0, "40"));
        }
        private Image DrawZoomImage(double Left, double Top, string Scale)
        {
            Image image = new Image();
            if (Scale == "40")
            {
                image.Width = 50.0;
                image.Height = 50.0;
            }
            else
            {
                image.Width = 60.0;
                image.Height = 60.0;
            }
            image.Source = new BitmapImage(new Uri("images/bei" + Scale + ".png", UriKind.Relative));
            image.SetValue(Canvas.LeftProperty, Left);
            image.SetValue(Canvas.TopProperty, Top);
            image.Visibility = Visibility.Collapsed;
            image.Name = "buttonR" + Scale;
            image.MouseLeftButtonDown += ZoomClick;
            ListImg.Add(image);
            return image;
        }
        private void ZoomClick(object sender, RoutedEventArgs e)
        {
            ZoomModel.IsDw = false;
            double zoom_ratio = 0.0;
            switch (((Image)sender).Name)
            {
                case "buttonR1":
                    zoom_ratio = 1.0;
                    break;
                case "buttonR2":
                    zoom_ratio = 2.0;
                    break;
                case "buttonR4":
                    zoom_ratio = 4.0;
                    break;
                case "buttonR10":
                    zoom_ratio = 10.0;
                    break;
                case "buttonR20":
                    zoom_ratio = 20.0;
                    break;
                case "buttonR40":
                    zoom_ratio = 40.0;
                    break;
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {

                    ZoomHelper.ZoomRatio(zoom_ratio, msi, RefreshAction);
                }
            }
            ZoomHelper.ZoomRatio(zoom_ratio, msi, RefreshAction);
        }
    }
}
