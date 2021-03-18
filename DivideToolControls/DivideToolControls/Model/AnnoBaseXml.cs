using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Model
{
    public class AnnoBaseXml
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public string Detail { get; set; }

        public string Zoom { get; set; }

        public string Length { get; set; }

        public string Area { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }

        public string FigureType { get; set; }

        public string Hidden { get; set; }

        public string Visible { get; set; }

        public int FontSize { get; set; }

        public string FontItalic { get; set; }

        public string FontBold { get; set; }

        public string PinType { get; set; }

        public string isMsVisble { get; set; }

        public List<string> ListPoint { get; set; }
    }
}
