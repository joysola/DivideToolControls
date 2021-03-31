using DivideToolControls.DeepZoom;
using DivideToolControls.DynamicGeometry.Enum;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace DivideToolControls.AnnotationControls
{
    public class AnnoLine : AnnoBase
    {
        private Line ALine { get; set; }
        private Thumb ThumbEnd { get; set; }
        private Thumb ThumbStart { get; set; }
        private Thumb ThumbMove { get; set; }
        /// <summary>
        /// 画标记构造器
        /// </summary>
        /// <param name="alc"></param>
        /// <param name="canvasboard"></param>
        /// <param name="msi"></param>
        /// <param name="objectlist"></param>
        /// <param name="SlideZoom"></param>
        /// <param name="Calibration"></param>
        public AnnoLine(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, List<AnnoBase> objectlist, int SlideZoom, double Calibration)
        {
            SetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            msi.MouseLeftButtonDown += MouseDown;
            base.AnnotationType = AnnotationType.Line;
            base.ControlName = base.AnnotationType + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// 展示标记构造器
        /// </summary>
        /// <param name="ab"></param>
        public AnnoLine(AnnoBase ab)
        {
            base.AnnotationType = AnnotationType.Line;
            base.ControlName = ab.ControlName;
            base.AnnotationDescription = ab.AnnotationDescription;
            ALine = new Line();
            ALine.Name = ab.ControlName;
            base.Calibration = ab.Calibration;
            base.SlideZoom = ab.SlideZoom;
            base.CurrentStart = ab.CurrentStart;
            base.CurrentEnd = ab.CurrentEnd;
            base.Size = ab.Size;
            base.BorderBrush = ab.BorderBrush;
            base.isVisble = ab.isVisble;
            base.isHidden = ab.isHidden;
            base.isMsVisble = ab.isMsVisble;
            base.isFinish = true;
            base.Zoom = ab.Zoom;
            base.AnnotationName = ab.AnnotationName;
            base.FiguresCanvas = ab.FiguresCanvas;
            base.objectlist = ab.objectlist;
            base.AnnoControl = ab.AnnoControl;
            base.FiguresCanvas = ab.FiguresCanvas;
            base.objectlist = ab.objectlist;
            base.AnnoControl = ab.AnnoControl;
            base.msi = ab.msi;
            ALine.X1 = MsiToCanvas(ab.CurrentStart).X;
            ALine.Y1 = MsiToCanvas(ab.CurrentStart).Y;
            ALine.X2 = MsiToCanvas(ab.CurrentEnd).X;
            ALine.Y2 = MsiToCanvas(ab.CurrentEnd).Y;
            ALine.StrokeThickness = ab.Size;
            ALine.Stroke = ab.BorderBrush;
            M_FiguresCanvas.Children.Add(ALine);
            base.objectlist.Insert(0, this);
            CreateMTextBlock();
            CreateThumb();
            UpdateCB();
            ALine.MouseLeftButtonDown += Select_MouseDown;
            ALine.MouseEnter += GotFocus;
            base.UpadteTextBlock();
            IsActive(Visibility.Collapsed);
            base.AnnoControl.CB.SelectedIndex = -1;
        }

        public void Unload()
        {
            base.msi.MouseLeftButtonDown -= MouseDown;
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (!base.isdraw)
            {
                ALine = new Line();
                ALine.Name = base.ControlName;
                Point position = e.GetPosition(M_FiguresCanvas);
                ALine.X1 = position.X;
                ALine.Y1 = position.Y;
                ALine.X2 = position.X;
                ALine.Y2 = position.Y;
                ALine.StrokeThickness = base.Size;
                ALine.Stroke = base.BorderBrush;
                base.msi.MouseMove += MouseMove;
                Application.Current.MainWindow.MouseLeave += MouseUp;
                Application.Current.MainWindow.MouseUp += MouseUp;
                base.OriginStart = position;
                base.OriginEnd = position;
                base.CurrentStart = CanvasToMsi(position);
                base.CurrentEnd = base.CurrentStart;
                M_FiguresCanvas.Children.Add(ALine);
                base.AnnotationName = Setting.Line + (base.objectlist.Count + 1);
                base.AnnotationDescription = "";
                base.objectlist.Insert(0, this);
                UpdateCB();
                base.isdraw = true;
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(base.msi);
            ALine.X1 = base.OriginStart.X;
            ALine.Y1 = base.OriginStart.Y;
            ALine.X2 = position.X;
            ALine.Y2 = position.Y;
            base.OriginEnd = position;
            base.CurrentEnd = CanvasToMsi(position);
            base.TextBlock_info = CalcMeasureInfo();
            base.AnnoControl.Tbk.Text = null;
            base.AnnoControl.Tbk.Text = CalcMeasureInfo();
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            base.isdraw = false;
            base.isFinish = true;
            ALine.MouseLeftButtonDown += Select_MouseDown;
            new Button();
            ALine.MouseEnter += GotFocus;
            CreateMTextBlock();
            CreateThumb();
            Application.Current.MainWindow.MouseLeave -= MouseUp;
            Application.Current.MainWindow.MouseUp -= MouseUp;
            base.msi.MouseLeftButtonDown -= MouseDown;
            base.msi.MouseMove -= MouseMove;
            base.FinishFunc(this, e);
        }

        public void GotFocus(object sender, EventArgs e)
        {
            if (Setting.isAnnoChange)
            {
                base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
            }
        }

        public void Select_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                Line line = (Line)frameworkElement;
                foreach (AnnoBase item in base.objectlist)
                {
                    item.IsActive(Visibility.Collapsed);
                    if (item.ControlName == line.Name)
                    {
                        item.IsActive(Visibility.Visible);
                        base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
                    }
                }
            }
        }

        private void ThumbMove_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Center, e.HorizontalChange, e.VerticalChange);
        }

        private void ThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.RightBottom, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void ThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.LeftTop, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        public override void DeleteItem()
        {
            M_FiguresCanvas.Children.Remove(ALine);
            M_FiguresCanvas.Children.Remove(base.MTextBlock);
            M_FiguresCanvas.Children.Remove(ThumbStart);
            M_FiguresCanvas.Children.Remove(ThumbMove);
            M_FiguresCanvas.Children.Remove(ThumbEnd);
            base.objectlist.Remove(this);
        }

        public override void CreateMTextBlock()
        {
            base.MTextBlock = new TextBlock();
            base.MTextBlock.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            base.MTextBlock.Opacity = 0.7;
            base.MTextBlock.Padding = new Thickness(2.0, 2.0, 2.0, 2.0);
            base.MTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            base.MTextBlock.FontWeight = FontWeights.Bold;
            base.MTextBlock.Visibility = Visibility.Collapsed;
            base.MTextBlock.MaxWidth = 150.0;
            base.MTextBlock.TextWrapping = TextWrapping.Wrap;
            base.MTextBlock.Text = CalcMeasureInfo(); // 标记信息
            base.MTextBlock.SetValue(Canvas.LeftProperty, ALine.X2);
            base.MTextBlock.SetValue(Canvas.TopProperty, ALine.Y2);
            M_FiguresCanvas.Children.Add(base.MTextBlock);
        }

        public override void CreateThumb()
        {
            if (ThumbEnd == null)
            {
                ThumbEnd = new Thumb();
                ThumbStart = new Thumb();
                ThumbMove = new Thumb();
                ThumbEnd.Height = Setting.Thumb_w;
                ThumbEnd.Width = Setting.Thumb_w;
                ThumbStart.Height = Setting.Thumb_w;
                ThumbStart.Width = Setting.Thumb_w;
                ThumbMove.Height = Setting.Thumb_c;
                ThumbMove.Width = Setting.Thumb_c;
                ThumbStart.SetValue(Canvas.LeftProperty, ALine.X1 - ThumbStart.Width / 2.0);
                ThumbStart.SetValue(Canvas.TopProperty, ALine.Y1 - ThumbStart.Height / 2.0);
                ThumbEnd.SetValue(Canvas.LeftProperty, ALine.X2 - ThumbEnd.Width / 2.0);
                ThumbEnd.SetValue(Canvas.TopProperty, ALine.Y2 - ThumbEnd.Height / 2.0);
                ThumbMove.SetValue(Canvas.LeftProperty, (ALine.X1 + ALine.X2) / 2.0 - ThumbMove.Width / 2.0);
                ThumbMove.SetValue(Canvas.TopProperty, (ALine.Y1 + ALine.Y2) / 2.0 - ThumbMove.Height / 2.0);
                M_FiguresCanvas.Children.Add(ThumbStart);
                M_FiguresCanvas.Children.Add(ThumbMove);
                M_FiguresCanvas.Children.Add(ThumbEnd);
                ThumbEnd.DragDelta += ThumbEnd_DragDelta;
                ThumbStart.DragDelta += ThumbStart_DragDelta;
                ThumbMove.DragDelta += ThumbMove_DragDelta;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string CalcMeasureInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (base.AnnotationDescription.Trim() != "" && base.isMsVisble)
            {
                if (base.isVisble == Visibility.Visible)
                {
                    stringBuilder.AppendLine(Setting.AnnoDesStr + base.AnnotationDescription);
                }
                else
                {
                    stringBuilder.Append(Setting.AnnoDesStr + base.AnnotationDescription);
                }
            }
            if (base.isVisble == Visibility.Collapsed && (!base.isMsVisble || base.AnnotationDescription.Trim() == ""))
            {
                base.MTextBlock.Visibility = Visibility.Collapsed;
            }
            if (base.isVisble == Visibility.Visible)
            {
                stringBuilder.Append(Setting.xLength + GetLength() + Setting.Unit); // 线的实际长度
            }
            return stringBuilder.ToString();
        }

        public override string CalcMeasureInfo1()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Setting.xLength + GetLength() + Setting.Unit);
            return stringBuilder.ToString();
        }

        private string GetLength()
        {
            double num = Math.Abs(ALine.X1 - ALine.X2);
            double num2 = Math.Abs(ALine.Y1 - ALine.Y2);
            double value = Math.Sqrt(num * num + num2 * num2) * base.Calibration / base.msi.ZoomableCanvas.Scale;
            return Math.Round(value, 2).ToString();
        }

        public override void UpdateVisual()
        {
            if (ThumbEnd != null)
            {
                if (base.AnnotationDescription.Trim() != "" || base.isVisble == Visibility.Visible)
                {
                    base.MTextBlock.Visibility = Visibility.Visible;
                }
                if (base.isVisble == Visibility.Collapsed && !base.isMsVisble)
                {
                    base.MTextBlock.Visibility = Visibility.Collapsed;
                }
                if (base.isVisble == Visibility.Collapsed && base.AnnotationDescription.Trim() == "")
                {
                    base.MTextBlock.Visibility = Visibility.Collapsed;
                }
                ALine.StrokeThickness = base.Size;
                ALine.Stroke = base.BorderBrush;
                ALine.X1 = MsiToCanvas(base.CurrentStart).X;
                ALine.Y1 = MsiToCanvas(base.CurrentStart).Y;
                ALine.X2 = MsiToCanvas(base.CurrentEnd).X;
                ALine.Y2 = MsiToCanvas(base.CurrentEnd).Y;
                ThumbStart.SetValue(Canvas.LeftProperty, ALine.X1 - ThumbStart.Width / 2.0);
                ThumbStart.SetValue(Canvas.TopProperty, ALine.Y1 - ThumbStart.Height / 2.0);
                ThumbEnd.SetValue(Canvas.LeftProperty, ALine.X2 - ThumbEnd.Width / 2.0);
                ThumbEnd.SetValue(Canvas.TopProperty, ALine.Y2 - ThumbEnd.Height / 2.0);
                ThumbMove.SetValue(Canvas.LeftProperty, (ALine.X1 + ALine.X2) / 2.0 - ThumbMove.Width / 2.0);
                ThumbMove.SetValue(Canvas.TopProperty, (ALine.Y1 + ALine.Y2) / 2.0 - ThumbMove.Height / 2.0);
                Canvas.SetLeft(base.MTextBlock, (ALine.X1 + ALine.X2) / 2.0);
                Canvas.SetTop(base.MTextBlock, (ALine.Y1 + ALine.Y2) / 2.0);
                ALine.Visibility = base.isHidden;
                base.MTextBlock.Visibility = base.isHidden;
                base.MTextBlock.Text = CalcMeasureInfo();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Region");
            writer.WriteAttributeString("Guid", base.ControlName);
            writer.WriteAttributeString("Name", base.AnnotationName);
            writer.WriteAttributeString("Detail", base.AnnotationDescription);
            writer.WriteAttributeString("FontSize", base.FontSize.ToString());
            writer.WriteAttributeString("FontItalic", base.FontItalic.ToString());
            writer.WriteAttributeString("FontBold", base.FontBold.ToString());
            writer.WriteAttributeString("Size", base.Size.ToString());
            writer.WriteAttributeString("FigureType", base.AnnotationType.ToString());
            writer.WriteAttributeString("Hidden", base.isHidden.ToString());
            writer.WriteAttributeString("Visible", base.isVisble.ToString());
            writer.WriteAttributeString("MsVisble", base.isMsVisble.ToString());
            writer.WriteAttributeString("Color", GetColor().ToString());
            writer.WriteAttributeString("PinType", base.PinType);
            writer.WriteAttributeString("Zoom", base.Zoom.ToString());
            writer.WriteStartElement("Vertices");
            writer.WriteStartElement("Vertice");
            writer.WriteAttributeString("X", base.CurrentStart.X.ToString());
            writer.WriteAttributeString("Y", base.CurrentStart.Y.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Vertice");
            writer.WriteAttributeString("X", base.CurrentEnd.X.ToString());
            writer.WriteAttributeString("Y", base.CurrentEnd.Y.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public override void IsActive(Visibility A)
        {
            if (ThumbEnd != null)
            {
                ThumbEnd.Visibility = A;
                ThumbStart.Visibility = A;
                ThumbMove.Visibility = A;
            }
        }

        public override void MoveUP()
        {
            ResetLocation(Direction.Center, 0.0, -Setting.Move_Step);
        }

        public override void MoveDown()
        {
            ResetLocation(Direction.Center, 0.0, Setting.Move_Step);
        }

        public override void MoveLeft()
        {
            ResetLocation(Direction.Center, -Setting.Move_Step, 0.0);
        }

        public override void MoveRight()
        {
            ResetLocation(Direction.Center, Setting.Move_Step, 0.0);
        }
    }
}
