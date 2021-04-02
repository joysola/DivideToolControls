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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;
using Path = System.Windows.Shapes.Path;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace DivideToolControls.Helper
{
    public class MulScanImgHelper
    {
        public static MulScanImgHelper Instance { get; } = new MulScanImgHelper();

        private MultiScaleImage msi = ZoomModel.MulScaImg;
        private List<int> touch = new List<int>();

        private double fitx;
        private double fity;

        private Point lastMouseDownPos;
        private Point lastMousePos;
        private List<Image> listImg = new List<Image>();


        private string _filePath;
        private string TempPath;
        private string TempFilename;

        private List<Image> ListImg = new List<Image>();
        private AnnoRectZoom _annoRectZoom;



        private Path _annoPath;
        public Path AnnoPath { get => _annoPath; set => _annoPath = value; }



        public void InitAll(string filepath)
        {
            _filePath = filepath;
            ZoomModel.Bg.Children.Add(msi);
            // MultiScaleImage的 ZoomableCanvas的 Scale或Offset属性变化时，通知回调
            LoadMsi(_filePath);
            msi.Ini += Msi_Init;
            RegisterMsiEvents();
            Setting.IsSynchronous = false;
            AnnoWindHelper.Instance.InitAnnoWinRegisterEvents();
            ZoomModel.X3DSlider.UpZ.MouseLeftButtonDown += UpZ_MouseLeftButtonDown;
            ZoomModel.X3DSlider.DownZ.MouseLeftButtonDown += DownZ_MouseLeftButtonDown;
            ZoomModel.Canvasboard.MouseRightButtonDown += msi_MouseRightButtonDown;
            ZoomModel.LayoutBody.MouseMove += LayoutBody_MouseMove;
            ZoomModel.Magfier.MagnifierScale = Setting.Magnifier;
            ZoomableCanvas.Refresh += (sender, e) =>


            {
                ZoomModel.RefreshAction?.Invoke();
            };
        }



        public void RegisterMsiEvents()
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
        public void UnRegisterMsiEvents()
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
            ZoomModel.IsDrag = false;
        }
        private void msi_TouchMove(object sender, TouchEventArgs e)
        {
            if (touch.Count > 1)
            {
                return;
            }
            TouchPoint touchPoint = e.GetTouchPoint(msi);
            lastMousePos = new Point(touchPoint.Position.X, touchPoint.Position.Y);
            if (ZoomModel.IsDrag)
            {
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        //if ((Mainpage)item.Value != this)
                        //{
                        ZoomModel.IsDw = false;
                        TmoveSetOffset(new Point(lastMouseDownPos.X - lastMousePos.X, lastMouseDownPos.Y - lastMousePos.Y));
                        ZoomHelper.ZoomRatio(msi.ZoomableCanvas.Scale * ZoomModel.SlideZoom, msi, ZoomModel.RefreshAction);
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
            if (ZoomModel.AnnoListWind != null)
            {
                int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1 && ZoomModel.ObjList[selectedIndex].isFinish)
                {
                    foreach (AnnoBase item in ZoomModel.ObjList)
                    {
                        item.IsActive(Visibility.Collapsed);
                        ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
                        ZoomModel.AnnoListWind.txt_qsr.Text = "";
                        ZoomModel.AnnoListWind.txt_xbz.Text = "";
                        ZoomModel.AnnoListWind.tbk_info.Text = "";
                        ZoomModel.AnnoListWind.ckb_clinfo.IsChecked = false;
                    }
                }
            }
            TouchPoint touchPoint = e.GetTouchPoint(msi);
            lastMouseDownPos = new Point(touchPoint.Position.X, touchPoint.Position.Y);
            ZoomModel.IsDrag = true;
            touch.Add(e.TouchDevice.Id);
        }
        private void msi_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Keyboard.Focus(ZoomModel.CurCtl);
            Setting.isCtrl = 0;
            Setting.Opacity = 0;
            ZoomModel.IsDw = false;
            IsArcMenu(Visibility.Collapsed);
            if (ZoomModel.AnnoListWind != null)
            {
                int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1 && ZoomModel.ObjList[selectedIndex].isFinish)
                {
                    foreach (AnnoBase item in ZoomModel.ObjList)
                    {
                        item.IsActive(Visibility.Collapsed);
                        ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
                        ZoomModel.AnnoListWind.txt_qsr.Text = "";
                        ZoomModel.AnnoListWind.txt_xbz.Text = "";
                        ZoomModel.AnnoListWind.tbk_info.Text = "";
                        ZoomModel.AnnoListWind.ckb_clinfo.IsChecked = false;
                    }
                }
            }
            lastMouseDownPos = e.GetPosition(msi);
            ZoomModel.IsDrag = true;
            Point position = e.GetPosition(ZoomModel.CurCtl);
            for (int i = 0; i < ZoomModel.ObjList.Count; i++)
            {
                AnnoBase annotationBase = ZoomModel.ObjList[i];
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
                        annotationBase.AnnoControl.CB.SelectedIndex = ZoomModel.ObjList.IndexOf(annotationBase);
                    }
                }
            }
        }

        private void msi_MouseMove(object sender, MouseEventArgs e)
        {
            lastMousePos = e.GetPosition(msi);
            if (ZoomModel.IsDrag)
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
                        ZoomHelper.ZoomRatio(msi.ZoomableCanvas.Scale * ZoomModel.SlideZoom, msi, ZoomModel.RefreshAction);
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
        private void msi_MouseLeftButtonMouseUp(object sender, MouseEventArgs e)
        {
            ZoomModel.IsDrag = false;
        }

        private void msi_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int num = e.Timestamp - ZoomModel.PrevTimeStap;
            if (Setting.isCtrl == 0 || ZoomModel.X3DSlider.Visibility == Visibility.Collapsed)
            {
                Setting.Opacity = 1;
                ZoomModel.IsDw = false;
                double num2 = 1.0;
                // Setting.MargPara = Setting.Calibration40 或 .Calibration20;
                double num3 = Setting.MaxMagValue * ZoomModel.SlideZoom * Setting.MargPara; // 最大极限倍率
                num2 = ZoomHelper.CalcSpeed(num);
                ZoomModel.PrevTimeStap = e.Timestamp;
                double curscale = ZoomModel.Curscale;
                if (curscale == ZoomModel.PrevNewzoom || !(ZoomModel.PrevTimeStap > 1E-08))
                {
                    double magAdjValueByCurMag = ZoomHelper.GetMagAdjValueByCurMag(ZoomModel.Curscale); // 获取缩放步长
                    // 根据滚轮判断是放大还是缩小(Delta>0 倍率增大、放大；Delta<0 倍率减少、缩小)
                    curscale = e.Delta <= 0 ? curscale - magAdjValueByCurMag * num2 : curscale + magAdjValueByCurMag * num2;
                    // ZoomModel.Fitratio = ZoomModel.SlideZoom * msi.ZoomableCanvas.Scale;
                    if (curscale < ZoomModel.Fitratio)
                    {
                        curscale = ZoomModel.Fitratio;
                    }
                    if (curscale > num3)
                    {
                        curscale = num3;
                    }
                    Point position = e.GetPosition(ZoomModel.CurCtl);
                    if (Setting.IsSynchronous)
                    {
                        foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                        {
                            ZoomHelper.ZoomRatio(curscale, position.X, position.Y, ZoomModel.LayoutBody, msi, ZoomModel.RefreshAction);
                        }
                    }
                    ZoomHelper.ZoomRatio(curscale, position.X, position.Y, ZoomModel.LayoutBody, msi, ZoomModel.RefreshAction);
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
            Point position = e.GetPosition(ZoomModel.CurCtl);
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
            position = new Point(position.X - ZoomModel.LayoutBody.ActualWidth / 2.0, position.Y - ZoomModel.LayoutBody.ActualHeight / 2.0);
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
        /// <summary>
        /// 需要修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                        ZoomHelper.ZoomRatio(num2, point.X, point.Y, ZoomModel.LayoutBody, msi, ZoomModel.RefreshAction);

                    }
                }
                ZoomHelper.ZoomRatio(num2, point.X, point.Y, ZoomModel.LayoutBody, msi, ZoomModel.RefreshAction);
                ZoomModel.PrevNewzoom = num2;
            }
        }

        /// <summary>
        /// 需要修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Msi_Init(object sender, RoutedEventArgs e)
        {
            string filePath = _filePath;

            TempPath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
            TempFilename = filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1);
            ZoomModel.Nav.SetThumbnail(ControlHelper.Instance.LoadImage(ZoomModel.InfoStruct.DataFilePTR, 8, 0, 0));
            ZoomModel.ImgLabel.Source = ControlHelper.Instance.GetLable(ZoomModel.InfoStruct.DataFilePTR);
            //ZoomModel.Nav.SetMultiScaleImage(msi);
            msi.Tag = "1";
            ZoomModel.ALC.CB = ZoomModel.AnnoListWind.cbo_mc;
            ZoomModel.ALC.Tbk = ZoomModel.AnnoListWind.tbk_info;
            ZoomModel.ALC.Tbx = ZoomModel.AnnoListWind.txt_xbz;
            ZoomModel.ALC.qsr = ZoomModel.AnnoListWind.txt_qsr;

            if (File.Exists(TempPath + TempFilename + ".Ano"))
            {
                XmlHelper.Instance.LoadAnoXml(TempPath + TempFilename + ".Ano");
                if (ZoomModel.ObjList.Count > 0)
                {
                    XmlHelper.Instance.IsChanged = true;
                }
            }
            //if (File.Exists(TempPath + TempFilename + ".case"))
            //{
            //    LoadCaseXml(TempPath + TempFilename + ".case");
            //}
            _annoRectZoom = new AnnoRectZoom(ZoomModel.ALC, ZoomModel.RectCans, msi, ZoomModel.ObjList, ZoomModel.SlideZoom, ZoomModel.Calibration);
            _annoRectZoom.RightZoom += m_RightZoom;
            ArcMenu();
            ZoomModel.Fitratio = ZoomModel.SlideZoom * msi.ZoomableCanvas.Scale;
            fitx = msi.ZoomableCanvas.Offset.X;
            fity = msi.ZoomableCanvas.Offset.Y;
            ZoomModel.RefreshAction?.Invoke();
            string isLabel = Setting.IsLabel;
            string isCase = Setting.IsCase;
            string isNav = Setting.IsNav;
            string isRule = Setting.IsRule;
            string isMagnifier = Setting.IsMagnifier;
            string isRotate = Setting.IsRotate;
            string isOperateball = Setting.IsOperateball;
            ZoomModel.Magfier.MagnifierScale = Setting.Magnifier;
            if (isMagnifier == "1")
            {
                ZoomModel.Magfier.Visibility = Visibility.Visible;
                ZoomModel.Magfier.Magnifiertimer.Start();
            }
            else
            {
                ZoomModel.Magfier.Visibility = Visibility.Collapsed;
                ZoomModel.Magfier.Magnifiertimer.Stop();
            }
            if (isRotate == "1")
            {
                ZoomModel.RotCtl.Visibility = Visibility.Visible;
            }
            else
            {
                ZoomModel.RotCtl.Visibility = Visibility.Collapsed;
            }
            if (isOperateball == "1")
            {
                ZoomModel.OpBall.Visibility = Visibility.Visible;
            }
            else
            {
                ZoomModel.OpBall.Visibility = Visibility.Collapsed;
            }
            if (isLabel == "1")
            {
                ZoomModel.ImgLabel.Visibility = Visibility.Visible;
            }
            else
            {
                ZoomModel.ImgLabel.Visibility = Visibility.Collapsed;
            }
            if (isNav == "1")
            {
                ZoomModel.Nav.Visibility = Visibility.Visible;
            }
            else
            {
                ZoomModel.Nav.Visibility = Visibility.Collapsed;
            }
            if (isRule == "1")
            {
                ZoomModel.ScRuler.Visibility = Visibility.Visible;
            }
            else
            {
                ZoomModel.ScRuler.Visibility = Visibility.Collapsed;
            }
            //if (isCase == "1")
            //{
            //    Show_CaseInfoWind(null, null);
            //}
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
                    ZoomModel.X3DSlider.Zvalue.Content = ZoomModel.nCurLevel;
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
                        ZoomModel.X3DSlider.Visibility = Visibility.Visible;
                    }
                    if (filename == ZoomModel.LevelFilePath && ZoomModel.nCurLevel != 0)
                    {
                        ZoomModel.X3DSlider.Visibility = Visibility.Collapsed;
                    }
                }
            }
            ZoomModel.DllImgFunc.CkGetHeaderInfoFunc(ZoomModel.InfoStruct, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref ZoomModel.TileSize);
            if (ZoomModel.TileSize == 0)
            {
                ZoomModel.TileSize = 256;
            }
            //msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, TileSize, 0, InfoStruct, khiScanScale, msi);
            //msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, ZoomModel.TileSize, 0, ZoomModel.InfoStruct, khiScanScale, msi, filename); // 关键

            // 测试
            int ww = (int)Math.Ceiling(256.95 * 256);
            int hh = (int)247.48 * 256;
            // 65792、63488
            msi.Source = new MagicZoomTileSource1(ww, hh, ZoomModel.TileSize, 0, ZoomModel.InfoStruct, khiScanScale, msi, filename); // 关键

            if (khiImageCapRes == 0.0)
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
                //ZoomModel.Calibration = khiImageCapRes;
                // 测试
                //ZoomModel.Calibration = 0.1662992;
                ZoomModel.Calibration = 0.24;
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
            //ZoomModel.SlideZoom = khiScanScale;
            //ZoomModel.ImageW = khiImageWidth;
            //ZoomModel.ImageH = khiImageHeight;
            ZoomModel.SlideZoom = 40;
            ZoomModel.ImageW = ww;
            ZoomModel.ImageH = hh;
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
            int num = int.Parse(ZoomModel.X3DSlider.Zvalue.Content.ToString()) + 1;
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
                ZoomModel.X3DSlider.Zvalue.Content = num;
                return text;
            }
            for (int i = 2; i < 99999; i++)
            {
                text = ZoomModel.LevelFilePath;
                int num2 = int.Parse(ZoomModel.X3DSlider.Zvalue.Content.ToString()) + i;
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
                    ZoomModel.X3DSlider.Zvalue.Content = num2;
                    return text;
                }
            }
            return "";
        }
        public string GetLowLevelFileName()
        {
            int num = int.Parse(ZoomModel.X3DSlider.Zvalue.Content.ToString()) - 1;
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
                ZoomModel.X3DSlider.Zvalue.Content = num;
                return text;
            }
            for (int i = 2; i < 99999; i++)
            {
                text = ZoomModel.LevelFilePath;
                int num2 = int.Parse(ZoomModel.X3DSlider.Zvalue.Content.ToString()) - i;
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
                    ZoomModel.X3DSlider.Zvalue.Content = num2;
                    return text;
                }
            }
            return "";
        }
        #endregion


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
            p = new Point(p.X - ZoomModel.LayoutBody.ActualWidth / 2.0, p.Y - ZoomModel.LayoutBody.ActualHeight / 2.0);
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
        /// <summary>
        /// 标记
        /// </summary>
        private void ArcMenu()
        {
            if (AnnoPath != null)
            {
                ZoomModel.ZoomCanvas.Children.Remove(AnnoPath);
                int count = ListImg.Count;
                for (int i = 0; i < count; i++)
                {
                    ZoomModel.ZoomCanvas.Children.Remove(ListImg[i]);
                }
                ListImg.Clear();
            }
            AnnoPath = new Path();
            AnnoPath.Visibility = Visibility.Collapsed;
            ZoomModel.ZoomCanvas.Children.Add(AnnoPath);
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
            ZoomModel.ZoomCanvas.Children.Add(DrawZoomImage(num - 50.0, num2 - 200.0, "1"));
            ZoomModel.ZoomCanvas.Children.Add(DrawZoomImage(num - 100.0, num2 - 210.0, "2"));
            ZoomModel.ZoomCanvas.Children.Add(DrawZoomImage(num - 150.0, num2 - 200.0, "4"));
            ZoomModel.ZoomCanvas.Children.Add(DrawZoomImage(num - 190.0, num2 - 170.0, "10"));
            ZoomModel.ZoomCanvas.Children.Add(DrawZoomImage(num - 210.0, num2 - 120.0, "20"));
            ZoomModel.ZoomCanvas.Children.Add(DrawZoomImage(num - 200.0, num2 - 60.0, "40"));
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

                    ZoomHelper.ZoomRatio(zoom_ratio, msi, ZoomModel.RefreshAction);
                }
            }
            ZoomHelper.ZoomRatio(zoom_ratio, msi, ZoomModel.RefreshAction);
        }

        private void m_RightZoom(object sender, RoutedEventArgs e)
        {
            AnnoRectZoom annoRectZoom = sender as AnnoRectZoom;
            double num = annoRectZoom.Scale;
            double curscale = ZoomModel.Curscale;
            curscale *= num;
            if (curscale > (double)ZoomModel.SlideZoom * Setting.MaxMagValue)
            {
                curscale = (double)ZoomModel.SlideZoom * Setting.MaxMagValue;
                num = curscale / ZoomModel.Curscale;
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    SynMsi(ZoomModel.Curscale, annoRectZoom.pCenter, num);
                }
            }
            Point pCenter = annoRectZoom.pCenter;
            Point center = new Point(0.0, 0.0);
            pCenter = new Point(pCenter.X - ZoomModel.LayoutBody.ActualWidth / 2.0, pCenter.Y - ZoomModel.LayoutBody.ActualHeight / 2.0);
            Point point = KCommon.PointRotate(center, pCenter, ZoomModel.Rotate);
            double x = point.X + msi.ActualWidth / 2.0;
            double y = point.Y + msi.ActualHeight / 2.0;
            pCenter = new Point(x, y);
            double num2 = msi.ZoomableCanvas.Extent.Width * msi.ZoomableCanvas.Scale;
            double num3 = msi.ZoomableCanvas.Extent.Height * msi.ZoomableCanvas.Scale;
            double num4 = msi.ZoomableCanvas.Extent.Width * (curscale / ZoomModel.SlideZoom);
            double num5 = msi.ZoomableCanvas.Extent.Height * (curscale / ZoomModel.SlideZoom);
            Point point2 = new Point(pCenter.X + msi.ZoomableCanvas.Offset.X - num2 / 2.0, pCenter.Y + msi.ZoomableCanvas.Offset.Y - num3 / 2.0);
            Point point3 = new Point(point2.X * num + num4 / 2.0 - pCenter.X - (msi.ActualWidth / 2.0 - pCenter.X), point2.Y * num + num5 / 2.0 - pCenter.Y - (msi.ActualHeight / 2.0 - pCenter.Y));
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
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(curscale / (double)ZoomModel.SlideZoom, timeSpan)
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
                msi.ZoomableCanvas.Offset = new Point(point3.X, point3.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }

        private void LayoutBody_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ZoomModel.Magfier.MagnifierMovePoint = e.GetPosition(ZoomModel.LayoutBody);
        }

    }
}
