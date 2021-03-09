using DivideToolControls.DynamicGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Model
{
    public class ZoomModel
    {
        public static int m_prevTimeStap;

        public static bool IsDw;

        public static int SlideZoom;

        public static double Curscale;

        public static double m_prevNewzoom;

        public static double fitratio;

        public static double Rotate;

        public static List<AnnotationBase> objectlist = new List<AnnotationBase>();

        public static double Calibration;
        public static double ImageW;

        public static double ImageH;
    }
}
