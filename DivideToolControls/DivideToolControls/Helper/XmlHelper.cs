using DivideToolControls.AnnotationControls;
using DivideToolControls.DeepZoom;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace DivideToolControls.Helper
{
    public class XmlHelper
    {
        public static XmlHelper Instance { get; } = new XmlHelper();
        public bool IsChanged { get; set; }
        private MultiScaleImage msi = ZoomModel.MulScaImg;
        public void LoadAnoXml(string filepath)
        {
            using (StreamReader streamReader = new StreamReader(filepath))
            {
                string text = "";
                while ((text = streamReader.ReadLine()) != null)
                {
                    ShowAnno(Xml2AnnoBaseXml(text));
                }
            }
        }

        /// <summary>
        /// 暂时不管
        /// </summary>
        /// <param name="filepath"></param>
        public void LoadCaseXml(string filepath)
        {
            //using (StreamReader streamReader = new StreamReader(filepath, Encoding.Default))
            //{
            //    string text = "";
            //    while ((text = streamReader.ReadLine()) != null)
            //    {
            //        CaseLine.AppendLine(text);
            //    }
            //    _CaseInfoWind._CaseInfo.Inlines.Add(CaseLine.ToString());
            //}
        }



        private List<AnnoBaseXml> Xml2AnnoBaseXml(string sbXml)
        {
            List<AnnoBaseXml> list = new List<AnnoBaseXml>();
            XmlReader reader = XmlReader.Create(new StringReader(sbXml));
            XDocument xDocument = XDocument.Load(reader);
            foreach (XElement item in xDocument.Descendants("Region"))
            {
                List<string> list2 = new List<string>();
                AnnoBaseXml annoBaseXml = new AnnoBaseXml();
                annoBaseXml.Guid = GetValueFromDocument(item, "Guid", "Line20140528092455");
                annoBaseXml.Name = GetValueFromDocument(item, "Name", "直线");
                annoBaseXml.FigureType = GetValueFromDocument(item, "FigureType", "Line");
                annoBaseXml.Detail = GetValueFromDocument(item, "Detail", "");
                annoBaseXml.Zoom = GetValueFromDocument(item, "Zoom", "1");
                annoBaseXml.Size = GetValueFromDocument(item, "Size", "2");
                annoBaseXml.Color = GetValueFromDocument(item, "Color", "4278190335");
                annoBaseXml.Hidden = GetValueFromDocument(item, "Hidden", "Collapsed");
                annoBaseXml.Visible = GetValueFromDocument(item, "Visible", "Collapsed");
                annoBaseXml.isMsVisble = GetValueFromDocument(item, "MsVisble", "False");
                annoBaseXml.FontSize = int.Parse(GetValueFromDocument(item, "FontSize", "12"));
                annoBaseXml.FontItalic = GetValueFromDocument(item, "FontItalic", "False");
                annoBaseXml.FontBold = GetValueFromDocument(item, "FontBold", "False");
                annoBaseXml.PinType = GetValueFromDocument(item, "PinType", "images/pin_1.png");
                foreach (XElement item2 in item.Element("Vertices").Elements("Vertice"))
                {
                    list2.Add(item2.Attribute("X").Value + "," + item2.Attribute("Y").Value);
                }
                annoBaseXml.ListPoint = list2;
                list.Add(annoBaseXml);
            }
            return list;
        }
        private void ShowAnno(List<AnnoBaseXml> abvos)
        {
            try
            {
                for (int i = 0; i < abvos.Count; i++)
                {
                    AnnoBase annoBase = new AnnoBase();
                    annoBase.ControlName = abvos[i].Guid;
                    annoBase.AnnotationName = abvos[i].Name;
                    annoBase.AnnotationDescription = abvos[i].Detail;
                    annoBase.Size = double.Parse(abvos[i].Size);
                    annoBase.isHidden = abvos[i].Hidden != "Visible" ? Visibility.Collapsed : Visibility.Visible;
                    annoBase.isVisble = abvos[i].Visible != "Visible" ? Visibility.Collapsed : Visibility.Visible;
                    annoBase.BorderBrush = new SolidColorBrush(Setting.NumberToRgba(uint.Parse(abvos[i].Color)));
                    annoBase.FontBold = abvos[i].FontBold == "True" ? true : false;
                    annoBase.FontItalic = abvos[i].FontItalic == "True" ? true : false;
                    annoBase.FontSize = abvos[i].FontSize;
                    annoBase.PinType = abvos[i].PinType;
                    annoBase.isMsVisble = abvos[i].isMsVisble == "True" ? true : false;
                    annoBase.Zoom = double.Parse(abvos[i].Zoom);
                    if (abvos[i].ListPoint.Count() <= 2)
                    {
                        annoBase.CurrentStart = new Point(double.Parse(abvos[i].ListPoint[0].Split(',')[0]), double.Parse(abvos[i].ListPoint[0].Split(',')[1]));
                        if (abvos[i].ListPoint.Count() == 2)
                        {
                            annoBase.CurrentEnd = new Point(double.Parse(abvos[i].ListPoint[1].Split(',')[0]), double.Parse(abvos[i].ListPoint[1].Split(',')[1]));
                        }
                    }
                    else
                    {
                        PointCollection pointCollection = new PointCollection();
                        for (int j = 0; j < abvos[i].ListPoint.Count(); j++)
                        {
                            pointCollection.Add(new Point(double.Parse(abvos[i].ListPoint[j].Split(',')[0]), double.Parse(abvos[i].ListPoint[j].Split(',')[1])));
                        }
                        annoBase.PointCollection = pointCollection;
                    }
                    annoBase.XmlSetPara(ZoomModel.ALC, ZoomModel.Canvasboard, msi, ZoomModel.ObjList, ZoomModel.SlideZoom, ZoomModel.Calibration);
                    switch (abvos[i].FigureType)
                    {
                        case "Line":
                            new AnnoLine(annoBase);
                            break;
                        case "Arrow":
                            new AnnoArrow(annoBase);
                            break;
                        case "Rectangle":
                            new AnnoRect(annoBase);
                            break;
                        case "Ellipse":
                            new AnnoEllipse(annoBase);
                            break;
                        case "Remark":
                            new AnnoPin(annoBase);
                            break;
                        case "Polygon":
                            new AnnoPolyline(annoBase);
                            break;
                        case "DiyCtcRectangle":
                            {
                                AnnoDIYCtcRect value = new AnnoDIYCtcRect(annoBase);
                                if (annoBase.FontSize != 12 || AnnoWindHelper.Instance.AnnoDiyCtcRectDic.Count > 0)
                                {
                                    AnnoWindHelper.Instance.AnnoDiyCtcRectDic.Add(annoBase.FontSize, value);
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private string GetValueFromDocument(XElement f, string name, string defaultV)
        {
            string result = defaultV;
            XAttribute xAttribute = f.Attribute(name);
            if (xAttribute != null)
            {
                result = xAttribute.Value;
            }
            return result;
        }
    }
}
