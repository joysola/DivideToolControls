using DivideToolControls.DeepZoom;
using DivideToolControls.DynamicGeometry.Enum;
using DivideToolControls.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace DivideToolControls.AnnotationControls
{
    public class AnnoBase
    {
        private string m_AnnotationName = string.Empty;

        private string m_ControlName = string.Empty;

        public Canvas M_FiguresCanvas;

        public Point m_CurrentStart;

        public Point m_CurrentEnd;

        public ContextMenu AnnoContextMenu;

        public double Rotate;

        public string ControlName
        {
            get => m_ControlName;
            set => m_ControlName = value;
        }

        public string AnnotationName
        {
            get => m_AnnotationName;
            set => m_AnnotationName = value;
        }

        public string AnnotationDescription { get; set; }

        public Visibility isVisble { get; set; }

        public bool isMsVisble { get; set; }
        [JsonIgnore]
        public List<AnnoBase> objectlist { get; set; }

        public ComboBox CB { get; set; }

        public TextBlock Tbk { get; set; }

        public TextBox Tbx { get; set; }

        public Image M_image { get; set; }

        public double Zoom { get; set; }
        [JsonIgnore]
        public MultiScaleImage msi { get; set; }

        public AnnoListCtls AnnoControl { get; set; }

        public Visibility isHidden { get; set; }

        public Brush BorderBrush { get; set; }

        public Brush FontColor { get; set; }

        public AnnotationType AnnotationType { get; set; }

        public PointCollection PointCollection { get; set; }

        public Canvas FiguresCanvas
        {
            get => M_FiguresCanvas;
            set => M_FiguresCanvas = value;
        }

        public TextBlock MTextBlock { get; set; }

        public Point OriginStart { get; set; }

        public Point OriginEnd { get; set; }

        public string PinType { get; set; }
        /// <summary>
        /// 是否正在绘制
        /// </summary>
        public bool isdraw { get; set; }

        public double Size { get; set; }

        public string TextBlock_info { get; set; }

        public int FontSize { get; set; }

        public bool FontItalic { get; set; }

        public bool FontBold { get; set; }

        public double Calibration { get; set; }

        public bool isFinish { get; set; }

        public Point CurrentStart
        {
            get => m_CurrentStart;
            set => m_CurrentStart = value;
        }

        public Point CurrentEnd
        {
            get => m_CurrentEnd;
            set => m_CurrentEnd = value;
        }

        public int SlideZoom { get; set; }

        public event MouseEventHandler FinishEvent;

        public AnnoBase()
        {
            isVisble = Visibility.Visible;
            isHidden = Visibility.Visible;
            Size = Setting.STROCK_SIZE;
            BorderBrush = new SolidColorBrush(Setting.STROCK_COLOR);
            FontColor = new SolidColorBrush(Setting.STROCK_COLOR1);
            ControlName = Guid.NewGuid().ToString();
            isdraw = false;
            FontSize = 12;
            PinType = "images/pin_1.png";
            isMsVisble = true;
            Setting.isAnnoChange = false;
        }

        public void SetPara(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, List<AnnoBase> objectlist, int SlideZoom, double Calibration)
        {
            M_FiguresCanvas = canvasboard;
            this.msi = msi;
            this.objectlist = objectlist;
            AnnoControl = alc;
            this.SlideZoom = SlideZoom;
            Zoom = msi.ZoomableCanvas.Scale * SlideZoom;
            this.Calibration = Calibration;
        }

        public void XmlSetPara(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, List<AnnoBase> objectlist, int SlideZoom, double Calibration)
        {
            M_FiguresCanvas = canvasboard;
            this.msi = msi;
            this.objectlist = objectlist;
            AnnoControl = alc;
            this.SlideZoom = SlideZoom;
            this.Calibration = Calibration;
        }

        public virtual void FinishFunc(object sender, MouseEventArgs e)
        {
            if (this.FinishEvent != null)
            {
                Setting.isAnnoChange = true;
                this.FinishEvent(sender, e);
            }
        }

        protected virtual void ResetLocation(Direction direction, double offsetX, double offsetY)
        {
            double num = msi.ZoomableCanvas.Scale * SlideZoom;
            Point currentStart = CurrentStart;
            Point currentEnd = CurrentEnd;
            switch (direction)
            {
                case Direction.LeftTop:
                    CurrentStart = new Point(CurrentStart.X + offsetX / num, CurrentStart.Y + offsetY / num);
                    break;
                case Direction.Top:
                    CurrentStart = new Point(CurrentStart.X, CurrentStart.Y + offsetY / num);
                    break;
                case Direction.Left:
                    CurrentStart = new Point(CurrentStart.X + offsetX / num, CurrentStart.Y);
                    break;
                case Direction.RightTop:
                    CurrentStart = new Point(CurrentStart.X, CurrentStart.Y + offsetY / num);
                    CurrentEnd = new Point(CurrentEnd.X + offsetX / num, CurrentEnd.Y);
                    break;
                case Direction.Right:
                    CurrentEnd = new Point(CurrentEnd.X + offsetX / num, CurrentEnd.Y);
                    break;
                case Direction.RightBottom:
                    CurrentEnd = new Point(CurrentEnd.X + offsetX / num, CurrentEnd.Y + offsetY / num);
                    break;
                case Direction.Bottom:
                    CurrentEnd = new Point(CurrentEnd.X, CurrentEnd.Y + offsetY / num);
                    break;
                case Direction.LeftBottom:
                    CurrentStart = new Point(CurrentStart.X + offsetX / num, CurrentStart.Y);
                    CurrentEnd = new Point(CurrentEnd.X, CurrentEnd.Y + offsetY / num);
                    break;
                case Direction.Center:
                    CurrentStart = new Point(CurrentStart.X + offsetX / num, CurrentStart.Y + offsetY / num);
                    CurrentEnd = new Point(CurrentEnd.X + offsetX / num, CurrentEnd.Y + offsetY / num);
                    break;
            }
            if (AnnotationType == AnnotationType.Line || AnnotationType == AnnotationType.Arrow)
            {
                UpdateVisual();
                UpadteTextBlock();
                return;
            }
            if (AnnotationType == AnnotationType.Remark)
            {
                UpdateVisual();
                return;
            }
            bool flag = false;
            double num2 = CurrentStart.X - CurrentEnd.X;
            double num3 = currentStart.X - currentEnd.X;
            double num4 = CurrentStart.Y - CurrentEnd.Y;
            double num5 = currentStart.Y - currentEnd.Y;
            if (num2 * num3 > 0.0 && num4 * num5 > 0.0 && num2 * num4 > 50.0 / num)
            {
                flag = false;
            }
            else
            {
                flag = true;
                CurrentStart = currentStart;
                CurrentEnd = currentEnd;
            }
            if (!flag)
            {
                UpdateVisual();
                UpadteTextBlock();
            }
        }

        public virtual void UpdateVisual()
        {
        }

        public virtual void UpadteTextBlock()
        {
            TextBlock_info = CalcMeasureInfo1();
            AnnoControl.Tbk.Text = TextBlock_info;
            if (MTextBlock != null)
            {
                MTextBlock.Text = CalcMeasureInfo();
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
        }

        public virtual string CalcMeasureInfo()
        {
            return null;
        }

        public virtual string CalcMeasureInfo1()
        {
            return null;
        }

        public virtual uint GetColor()
        {
            SolidColorBrush solidColorBrush = BorderBrush as SolidColorBrush;
            return Setting.RgbaToNumber(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
        }

        public virtual uint GetColor1()
        {
            SolidColorBrush solidColorBrush = FontColor as SolidColorBrush;
            return Setting.RgbaToNumber(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
        }

        public virtual void CreateThumb()
        {
        }

        public virtual void CreateMTextBlock()
        {
        }

        public virtual void DeleteItem()
        {
        }

        public virtual void MoveUP()
        {
        }

        public virtual void MoveDown()
        {
        }

        public virtual void MoveLeft()
        {
        }

        public virtual void MoveRight()
        {
        }

        public virtual void IsActive(Visibility A)
        {
        }

        public void UpdateCB()
        {
            AnnoControl.Tbk.Text = CalcMeasureInfo();
            AnnoControl.Tbx.Text = AnnotationName;
            AnnoControl.CB.ItemsSource = null;
            AnnoControl.CB.ItemsSource = objectlist;
            AnnoControl.CB.DisplayMemberPath = "AnnotationName";
            AnnoControl.CB.SelectedIndex = 0;
        }

        public Point CanvasToMsi(Point NewPiont)
        {
            Point offset = msi.ZoomableCanvas.Offset;
            Point result = default(Point);
            result.X = (NewPiont.X + offset.X) / (msi.ZoomableCanvas.Scale * (double)SlideZoom);
            result.Y = (NewPiont.Y + offset.Y) / (msi.ZoomableCanvas.Scale * (double)SlideZoom);
            return result;
        }

        public Point MsiToCanvas(Point NewPiont)
        {
            Point offset = msi.ZoomableCanvas.Offset;
            Point result = default(Point);
            result.X = NewPiont.X * (msi.ZoomableCanvas.Scale * (double)SlideZoom) - offset.X;
            result.Y = NewPiont.Y * (msi.ZoomableCanvas.Scale * (double)SlideZoom) - offset.Y;
            return result;
        }
    }
}
