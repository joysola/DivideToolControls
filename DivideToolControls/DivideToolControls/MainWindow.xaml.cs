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
        private DllImageFuc _dllImageFuc = new DllImageFuc();
        private MultiScaleImage msi = new MultiScaleImage();
        private DispatcherTimer _timer;
        private string filePath = @"K:\Test\DivideToolControls\DivideToolControls\DivideToolControls\2020-12-04_15_53_56.kfb";
        private IMAGE_INFO_STRUCT InfoStruct;
        private int nCurLevel;
        private int nTotalLevel;
        private int MinLevel;
        private int MaxLevel;
        private string LevelFilePath;
        private int TileSize;
        private Point lastMousePos;
        //private CtcList CtcWind;
        public MainWindow()
        {
            InitializeComponent();

            InitOnce();
            StartupOpenFiles(filePath);
            
            ClearMemoryThread();
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
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitOnce()
        {
            ZoomModel.Canvasboard = this.canvasboard;
            Bg.Children.Add(msi);
            // MultiScaleImage的 ZoomableCanvas的 Scale或Offset属性变化时，通知回调
            ZoomableCanvas.Refresh += (sender, e) =>
            {
                Refresh();
            };
        }


        private void msi_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = msi;
            e.Mode = ManipulationModes.All;
        }
        public void StartupOpenFiles(string filename)
        {
            //FilePath = filename;
            LoadMsi(filename);
            msi.Ini += Msi_Ini;
            msi.MouseWheel += Msi_MouseWheel;
            msi.IsManipulationEnabled = true;
            msi.ManipulationStarting += msi_ManipulationStarting;
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
                DllImageFuc.GetScanLevelInfoFunc(ref InfoStruct, ref nCurLevel, ref nTotalLevel);
                if (nTotalLevel > 2)
                {
                    if (nTotalLevel % 2 == 0)
                    {
                        MinLevel = -nTotalLevel / 2 + 1;
                        MaxLevel = nTotalLevel / 2;
                    }
                    else
                    {
                        MinLevel = -(nTotalLevel - 1) / 2;
                        MaxLevel = (nTotalLevel - 1) / 2;
                    }
                    x3dSlider.Zvalue.Content = nCurLevel;
                    if (nCurLevel == 0)
                    {
                        LevelFilePath = filename;
                    }
                    else
                    {
                        LevelFilePath = filename.Replace("_" + nCurLevel + ".kfb", ".kfb");
                    }
                    if (CheckAllLevel())
                    {
                        x3dSlider.Visibility = Visibility.Visible;
                    }
                    if (filename == LevelFilePath && nCurLevel != 0)
                    {
                        x3dSlider.Visibility = Visibility.Collapsed;
                    }
                }
            }
            _dllImageFuc.CkGetHeaderInfoFunc(InfoStruct, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref TileSize);
            if (TileSize == 0)
            {
                TileSize = 256;
            }
            //msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, TileSize, 0, InfoStruct, khiScanScale, msi);
            msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, TileSize, 0, InfoStruct, khiScanScale, msi, filename);
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
        public bool CheckAllLevel()
        {
            for (int i = MinLevel; i < MinLevel + nTotalLevel; i++)
            {
                string path = LevelFilePath;
                if (i != 0)
                {
                    path = LevelFilePath.Replace(".kfb", "_" + i + ".kfb");
                }
                if (File.Exists(path) && i != nCurLevel)
                {
                    return true;
                }
            }
            return false;
        }
        public void IniFile(string fileName)
        {
            InfoStruct.DataFilePTR = 0;
            _dllImageFuc.CkInitImageFileFunc(ref InfoStruct, fileName);
        }
        private void Msi_Ini(object sender, RoutedEventArgs e)
        {
            string filePath = this.filePath;
            //nav._Mainpage = this;
            //TempPath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
            //TempFilename = filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1);
            //nav.SetThumbnail(LoadImage(InfoStruct.DataFilePTR, 8, 0, 0));
            //nav.SetMultiScaleImage(msi);
            //Image_lable.Source = GetLable(InfoStruct.DataFilePTR);
            msi.Tag = "1";
            //alc.CB = _AnnoListWind.cbo_mc;
            //alc.Tbk = _AnnoListWind.tbk_info;
            //alc.Tbx = _AnnoListWind.txt_xbz;
            //alc.qsr = _AnnoListWind.txt_qsr;
            //if (File.Exists(TempPath + TempFilename + ".Ano"))
            //{
            //    LoadAnoXml(TempPath + TempFilename + ".Ano");
            //    if (objectlist.Count > 0)
            //    {
            //        Ischange = true;
            //    }
            //}
            //if (File.Exists(TempPath + TempFilename + ".case"))
            //{
            //    LoadCaseXml(TempPath + TempFilename + ".case");
            //}
            //_myRectZoom = new myRectZoom(alc, RectCanvas, msi, objectlist, SlideZoom, Calibration);
            //_myRectZoom.RightZoom += m_RightZoom;
            //ArcMenu();
            ZoomModel.Fitratio = (double)ZoomModel.SlideZoom * msi.ZoomableCanvas.Scale;
            //fitx = msi.ZoomableCanvas.Offset.X;
            //fity = msi.ZoomableCanvas.Offset.Y;
            Refresh();
            string isLabel = Setting.IsLabel;
            string isCase = Setting.IsCase;
            string isNav = Setting.IsNav;
            string isRule = Setting.IsRule;
            string isMagnifier = Setting.IsMagnifier;
            string isRotate = Setting.IsRotate;
            string isOperateball = Setting.IsOperateball;
            //MagnifierScale = Setting.Magnifier;
            //if (isMagnifier == "1")
            //{
            //    Canv_Magnifier.Visibility = Visibility.Visible;
            //    _Magnifiertimer.Start();
            //}
            //else
            //{
            //    Canv_Magnifier.Visibility = Visibility.Collapsed;
            //    _Magnifiertimer.Stop();
            //}
            //if (isRotate == "1")
            //{
            //    _RotateViewer.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    _RotateViewer.Visibility = Visibility.Collapsed;
            //}
            //if (isOperateball == "1")
            //{
            //    Canvas_Operateball.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    Canvas_Operateball.Visibility = Visibility.Collapsed;
            //}
            //if (isLabel == "1")
            //{
            //    Image_lable.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    Image_lable.Visibility = Visibility.Collapsed;
            //}
            //if (isNav == "1")
            //{
            //    nav.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    nav.Visibility = Visibility.Collapsed;
            //}
            //if (isRule == "1")
            //{
            //    RuleCanvas.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    RuleCanvas.Visibility = Visibility.Collapsed;
            //}
            //if (isCase == "1")
            //{
            //    Show_CaseInfoWind(null, null);
            //}
        }

        private void Msi_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int num = e.Timestamp - ZoomModel.PrevTimeStap;
            if (Setting.isCtrl == 0 || x3dSlider.Visibility == Visibility.Collapsed)
            {
                Setting.Opacity = 1;
                ZoomModel.IsDw = false;
                double num2 = 1.0;
                double num3 = Setting.MaxMagValue * ZoomModel.SlideZoom * Setting.MargPara;
                num2 = ZoomHelper.CalcSpeed(num);
                ZoomModel.PrevTimeStap = e.Timestamp;
                double curscale = ZoomModel.Curscale;
                if (curscale == ZoomModel.PrevNewzoom || !(ZoomModel.PrevNewzoom > 1E-08))
                {
                    double magAdjValueByCurMag = ZoomHelper.GetMagAdjValueByCurMag(ZoomModel.Curscale);
                    curscale = e.Delta <= 0 ? (curscale - magAdjValueByCurMag * num2) : (curscale + magAdjValueByCurMag * num2);
                    if (curscale < ZoomModel.Fitratio)
                    {
                        curscale = ZoomModel.Fitratio;
                    }
                    if (curscale > num3)
                    {
                        curscale = num3;
                    }
                    Point position = e.GetPosition(this);
                    if (Setting.IsSynchronous)
                    {
                        foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                        {
                            if ((MainWindow)item.Value != this)
                            {
                                ZoomHelper.ZoomRatio(curscale, position.X, position.Y, LayoutBody, msi, Refresh);
                            }
                        }
                    }
                    ZoomHelper.ZoomRatio(curscale, position.X, position.Y, LayoutBody, msi, Refresh);
                    //Refresh();
                    ZoomModel.PrevNewzoom = curscale;
                }
            }
            //else if (num > 300)
            //{
            //    m_prevTimeStap = e.Timestamp;
            //    if (e.Delta > 0)
            //    {
            //        UpZ_MouseLeftButtonDown(null, null);
            //    }
            //    else
            //    {
            //        DownZ_MouseLeftButtonDown(null, null);
            //    }
            //}
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
            ZoomHelper.ReDraw(); // 画Annotation
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

    }
}
