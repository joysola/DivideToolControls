using DivideToolControls.AnnotationControls;
using DivideToolControls.Controls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DynamicGeometry;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DivideToolControls.Helper
{
    public class ZoomHelper
    {
        //public static ZoomHelper Instance { get; } = new ZoomHelper();
        private static double[] m_magV = new double[12]
        {
        320.0,
        128.0,
        60.0,
        40.0,
        30.0,
        20.0,
        10.0,
        8.0,
        5.0,
        2.0,
        1.5,
        0.0
        };

        private static double[] m_adjV = new double[12]
        {
        10.0,
        5.0,
        4.0,
        3.0,
        2.0,
        1.5,
        1.0,
        0.7,
        0.5,
        0.4,
        0.3,
        0.2
        };

        public static double GetMagAdjValueByCurMag(double curMagV)
        {
            double result = 0.0;
            for (int i = 0; i < 11; i++)
            {
                double num = m_magV[i + 1];
                double num2 = m_magV[i];
                double y = m_adjV[i + 1];
                double y2 = m_adjV[i];
                if (curMagV >= num && curMagV <= num2)
                {
                    result = CalcFixValueY(num, num2, y, y2, curMagV);
                    break;
                }
            }
            return result;
        }

        private static double CalcFixValueY(double x1, double x2, double y1, double y2, double x)
        {
            double num = x2 - x1;
            double num2 = x2 - x;
            double num3 = y2 - y1;
            double num4 = num3 * num2 / (1.0 * num);
            return y2 - num4;
        }

        public static double CalcSpeed(int timeDelta)
        {
            double num = 1.0;
            double num2 = 3.0;
            double num3 = 1.0;
            double num4 = 80.0;
            if (timeDelta <= 0)
            {
                num = num2;
            }
            else if ((double)timeDelta >= num4)
            {
                num = num3;
            }
            else
            {
                num = (num4 - (double)timeDelta) * (num2 - num3) / num4 + num3;
                if (num < num3)
                {
                    num = num3;
                }
                if (num > num2)
                {
                    num = num2;
                }
            }
            return num;
        }

        public static void ZoomRatio(double zoom_ratio, double x, double y, Grid LayoutBody, MultiScaleImage msi, Action refreshAction = null)
        {
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
            Point point = new Point(x, y);
            Point center = new Point(0.0, 0.0);
            point = new Point(point.X - LayoutBody.ActualWidth / 2.0, point.Y - LayoutBody.ActualHeight / 2.0);
            Point point2 = KCommon.PointRotate(center, point, ZoomModel.Rotate);
            double x2 = point2.X + msi.ActualWidth / 2.0;
            double y2 = point2.Y + msi.ActualHeight / 2.0;
            Point elementPoint = new Point(x2, y2);
            Point point3 = msi.ElementToLogicalPoint(elementPoint);
            msi.ZoomAboutLogicalPoint(zoom_ratio / ZoomModel.Curscale, point3.X, point3.Y);
            ZoomModel.Curscale = zoom_ratio;
            refreshAction?.Invoke();
        }
        public static void ZoomRatio(double zoom_ratio, MultiScaleImage msi, Action refreshAction)
        {
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
            Point elementPoint = new Point(msi.ActualWidth / 2.0, msi.ActualHeight / 2.0);
            Point point = msi.ElementToLogicalPoint(elementPoint);
            msi.ZoomAboutLogicalPoint(zoom_ratio * num / ZoomModel.Curscale, point.X, point.Y);
            ZoomModel.Curscale = zoom_ratio;
            refreshAction?.Invoke();
        }
        public static void ReDraw()
        {
            foreach (AnnoBase item in ZoomModel.Objectlist)
            {
                item.UpdateVisual();
            }
        }

        public void StartupOpenFiles(string filename)
        {
            //LoadMsi(filename);
            ZoomModel.MulScaImg.Ini += Msi_Ini;
            ZoomModel.MulScaImg.MouseWheel += Msi_MouseWheel;
            ZoomModel.MulScaImg.IsManipulationEnabled = true;
            ZoomModel.MulScaImg.ManipulationStarting += msi_ManipulationStarting;
        }

        private void msi_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Msi_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Msi_Ini(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //public void LoadMsi(string filename)
        //{
        //    IniFile(filename);
        //    int khiImageHeight = 1;
        //    int khiImageWidth = 2;
        //    int khiScanScale = 3;
        //    float khiSpendTime = 0f;
        //    double khiScanTime = 0.0;
        //    float khiImageCapRes = 0f;
        //    if (filename.IndexOf(".kfb") != -1)
        //    {
        //        DllImageFuc.GetScanLevelInfoFunc(ref ZoomModel.InfoStruct, ref ZoomModel.nCurLevel, ref ZoomModel.nTotalLevel);
        //        if (ZoomModel.nTotalLevel > 2)
        //        {
        //            if (ZoomModel.nTotalLevel % 2 == 0)
        //            {
        //                ZoomModel.MinLevel = -ZoomModel.nTotalLevel / 2 + 1;
        //                ZoomModel.MaxLevel = ZoomModel.nTotalLevel / 2;
        //            }
        //            else
        //            {
        //                ZoomModel.MinLevel = -(ZoomModel.nTotalLevel - 1) / 2;
        //                ZoomModel.MaxLevel = (ZoomModel.nTotalLevel - 1) / 2;
        //            }
        //            x3dSlider.Zvalue.Content = ZoomModel.nCurLevel;
        //            if (ZoomModel.nCurLevel == 0)
        //            {
        //                ZoomModel.LevelFilePath = filename;
        //            }
        //            else
        //            {
        //                ZoomModel.LevelFilePath = filename.Replace("_" + ZoomModel.nCurLevel + ".kfb", ".kfb");
        //            }
        //            if (CheckAllLevel())
        //            {
        //                x3dSlider.Visibility = Visibility.Visible;
        //            }
        //            if (filename == ZoomModel.LevelFilePath && ZoomModel.nCurLevel != 0)
        //            {
        //                x3dSlider.Visibility = Visibility.Collapsed;
        //            }
        //        }
        //    }
        //    ZoomModel.DllImgFunc.CkGetHeaderInfoFunc(ZoomModel.InfoStruct, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref ZoomModel.TileSize);
        //    if (ZoomModel.TileSize == 0)
        //    {
        //        ZoomModel.TileSize = 256;
        //    }
        //    //msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, TileSize, 0, InfoStruct, khiScanScale, msi);
        //    ZoomModel.MulScaImg.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, ZoomModel.TileSize, 0, ZoomModel.InfoStruct, khiScanScale, ZoomModel.MulScaImg, filename);
        //    if ((double)khiImageCapRes == 0.0)
        //    {
        //        switch (khiScanScale)
        //        {
        //            case 20:
        //                ZoomModel.Calibration = 0.5;
        //                break;
        //            case 40:
        //                ZoomModel.Calibration = 0.2439;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        ZoomModel.Calibration = khiImageCapRes;
        //        if (khiScanScale == 40)
        //        {
        //            if (Setting.Calibration40 != 1.0)
        //            {
        //                Setting.MargPara = Setting.Calibration40;
        //            }
        //            if (Setting.CalibrationX40 != 1.0)
        //            {
        //                ZoomModel.Calibration = Setting.CalibrationX40;
        //            }
        //        }
        //        if (khiScanScale == 20)
        //        {
        //            if (Setting.Calibration20 != 1.0)
        //            {
        //                Setting.MargPara = Setting.Calibration20;
        //            }
        //            if (Setting.CalibrationX20 != 1.0)
        //            {
        //                ZoomModel.Calibration = Setting.CalibrationX20;
        //            }
        //        }
        //    }
        //    ZoomModel.SlideZoom = khiScanScale;
        //    ZoomModel.ImageW = khiImageWidth;
        //    ZoomModel.ImageH = khiImageHeight;
        //}
        public void IniFile(string fileName)
        {
            ZoomModel.InfoStruct.DataFilePTR = 0;
            ZoomModel.DllImgFunc.CkInitImageFileFunc(ref ZoomModel.InfoStruct, fileName);
        }


    }
}
