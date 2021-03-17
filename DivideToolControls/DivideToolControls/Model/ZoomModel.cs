using DivideToolControls.AnnotationControls;
using DivideToolControls.Controls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DynamicGeometry;
using DivideToolControls.Helper;
using DivideToolControls.WinCtls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DivideToolControls.Model
{
    public class ZoomModel
    {
        public static int PrevTimeStap { get; set; }

        public static bool IsDw { get; set; }

        public static int SlideZoom { get; set; }

        public static double Curscale { get; set; }

        public static double PrevNewzoom { get; set; }

        public static double Fitratio { get; set; }

        public static double Rotate { get; set; }

        public static List<AnnoBase> ObjList { get; } = new List<AnnoBase>();

        public static double Calibration { get; set; }
        public static double ImageW { get; set; }

        public static double ImageH { get; set; }
        public static IMAGE_INFO_STRUCT InfoStruct;
        public static int nCurLevel;
        public static int nTotalLevel;
        public static int MinLevel { get; set; }
        public static int MaxLevel { get; set; }
        public static string LevelFilePath { get; set; }
        public static int TileSize;

        public static DllImageFuc DllImgFunc { get; } = new DllImageFuc();
        public static MultiScaleImage MulScaImg { get; } = new MultiScaleImage();
        public static AnnoListCtls ALC { get; } = new AnnoListCtls();

        public static bool IsDrag { get; set; }
        public static bool IsDrop { get; set; }





        public static Canvas Canvasboard { get; set; }
        public static FrameworkElement CurCtl { get; set; }
        public static AnnoListWind AnnoListWind { get; set; }
        public static AnnoWind AnnoWind { get; set; }
        public static Navmap Nav { get; set; }
        public static CtcList CtcWind { get; set; }
        public static Slider3D X3DSlider { get; set; }
        public static Grid LayoutBody { get; set; }
        //public static Path AnnoPath { get; set; }
        public static Grid Bg { get; set; }
        public static Canvas ZoomCanvas { get; set; }
        public static Action RefreshAction { get; set; }

    }
}
