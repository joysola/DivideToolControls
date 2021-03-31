using DivideToolControls.AnnotationControls;
using DivideToolControls.Controls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DeepZoomControls;
using DivideToolControls.DynamicGeometry;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DivideToolControls.Helper
{
    public class ZoomHelper
    {
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
        /// <summary>
        /// 获取缩放步长
        /// </summary>
        /// <param name="curMagV">当前CurScale</param>
        /// <returns></returns>
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

        private static double CalcFixValueY(double x1, double x2, double y1, double y2, double curMagV)
        {
            double num = x2 - x1;
            double num2 = x2 - curMagV;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoom_ratio">需要缩放成为的倍率</param>
        /// <param name="x">鼠标在控件中的x坐标</param>
        /// <param name="y">鼠标在控件中的y坐标</param>
        /// <param name="LayoutBody"></param>
        /// <param name="msi"></param>
        /// <param name="refreshAction"></param>
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
            Point point3 = msi.ElementToLogicalPoint(elementPoint); // 控件坐标转图像坐标后， 坐标 ÷ 尺寸
            msi.ZoomAboutLogicalPoint(zoom_ratio / ZoomModel.Curscale, point3.X, point3.Y);
            ZoomModel.Curscale = zoom_ratio; // 更新 当前倍率 为 现期倍率
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
    }
}
