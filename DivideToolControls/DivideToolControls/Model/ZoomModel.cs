using DivideToolControls.AnnotationControls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DynamicGeometry;
using DivideToolControls.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public static List<AnnoBase> Objectlist { get; set; } = new List<AnnoBase>();

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
        public static Canvas Canvasboard { get; set; }
    }
}
