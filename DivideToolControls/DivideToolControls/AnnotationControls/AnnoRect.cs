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
    public class AnnoRect : AnnoBase
    {

        private Point AnnoPoint { get; set; }

        public Thumb ThumbB { get; set; }

        public Thumb ThumbL { get; set; }

        public Thumb ThumbLB { get; set; }

        public Thumb ThumbLT { get; set; }

        public Thumb ThumbR { get; set; }

        public Thumb ThumbRB { get; set; }

        public Thumb ThumbRT { get; set; }

        public Thumb ThumbT { get; set; }

        public Thumb ThumbMove { get; set; }

        public Rectangle MRectangle { get; set; }

        public AnnoRect(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, List<AnnoBase> objectlist, int SlideZoom, double Calibration)
        {
            SetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            msi.MouseLeftButtonDown += MouseDown;
            base.AnnotationType = AnnotationType.Rectangle;
            base.ControlName = base.AnnotationType + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public void Unload()
        {
            base.msi.MouseLeftButtonDown -= MouseDown;
        }

        public AnnoRect(AnnoBase ab)
        {
            base.AnnotationType = AnnotationType.Rectangle;
            base.Calibration = ab.Calibration;
            base.SlideZoom = ab.SlideZoom;
            base.ControlName = ab.ControlName;
            base.AnnotationDescription = ab.AnnotationDescription;
            base.CurrentStart = ab.CurrentStart;
            base.CurrentEnd = ab.CurrentEnd;
            base.Size = ab.Size;
            base.BorderBrush = ab.BorderBrush;
            base.isVisble = ab.isVisble;
            base.isHidden = ab.isHidden;
            base.isMsVisble = ab.isMsVisble;
            base.AnnotationName = ab.AnnotationName;
            base.isFinish = true;
            base.Zoom = ab.Zoom;
            base.msi = ab.msi;
            base.FiguresCanvas = ab.FiguresCanvas;
            base.objectlist = ab.objectlist;
            base.AnnoControl = ab.AnnoControl;
            MRectangle = new Rectangle();
            MRectangle.Name = ab.ControlName;
            Point point = MsiToCanvas(base.CurrentStart);
            Point point2 = MsiToCanvas(base.CurrentEnd);
            MRectangle.SetValue(Canvas.LeftProperty, point.X);
            MRectangle.SetValue(Canvas.TopProperty, point.Y);
            MRectangle.Width = point2.X - point.X;
            MRectangle.Height = point2.Y - point.Y;
            MRectangle.StrokeThickness = base.Size;
            MRectangle.Stroke = base.BorderBrush;
            M_FiguresCanvas.Children.Add(MRectangle);
            base.objectlist.Insert(0, this);
            CreateMTextBlock();
            CreateThumb();
            UpdateCB();
            MRectangle.MouseLeftButtonDown += Select_MouseDown;
            MRectangle.MouseEnter += GotFocus;
            base.UpadteTextBlock();
            IsActive(Visibility.Collapsed);
            base.AnnoControl.CB.SelectedIndex = -1;
        }

        public void GotFocus(object sender, EventArgs e)
        {
            if (Setting.isAnnoChange)
            {
                base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
            }
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!base.isdraw)
            {
                MRectangle = new Rectangle();
                MRectangle.Name = base.ControlName;
                Point position = e.GetPosition(M_FiguresCanvas);
                MRectangle.SetValue(Canvas.LeftProperty, position.X);
                MRectangle.SetValue(Canvas.TopProperty, position.Y);
                MRectangle.Width = 0.0;
                MRectangle.Height = 0.0;
                MRectangle.StrokeThickness = base.Size;
                MRectangle.Stroke = base.BorderBrush;
                base.OriginStart = position;
                base.OriginEnd = position;
                AnnoPoint = position;
                base.CurrentStart = CanvasToMsi(position);
                base.CurrentEnd = CanvasToMsi(position);
                M_FiguresCanvas.Children.Add(MRectangle);
                base.msi.MouseMove += MouseMove;
                Application.Current.MainWindow.MouseLeave += MouseUp;
                Application.Current.MainWindow.MouseUp += MouseUp;
                base.AnnotationName = Setting.Rectangle + (base.objectlist.Count + 1);
                base.AnnotationDescription = "";
                base.objectlist.Insert(0, this);
                UpdateCB();
                base.isdraw = true;
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(M_FiguresCanvas);
            Point originEnd = new Point(Math.Max(AnnoPoint.X, position.X), Math.Max(AnnoPoint.Y, position.Y));
            Point originStart = new Point(Math.Min(AnnoPoint.X, position.X), Math.Min(AnnoPoint.Y, position.Y));
            MRectangle.SetValue(Canvas.LeftProperty, originStart.X);
            MRectangle.SetValue(Canvas.TopProperty, originStart.Y);
            MRectangle.Width = originEnd.X - originStart.X;
            MRectangle.Height = originEnd.Y - originStart.Y;
            base.OriginStart = originStart;
            base.OriginEnd = originEnd;
            base.CurrentStart = CanvasToMsi(base.OriginStart);
            base.CurrentEnd = CanvasToMsi(base.OriginEnd);
            base.TextBlock_info = CalcMeasureInfo();
            base.AnnoControl.Tbk.Text = null;
            base.AnnoControl.Tbk.Text = CalcMeasureInfo();
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            base.isFinish = true;
            CreateMTextBlock();
            CreateThumb();
            base.msi.MouseLeftButtonDown -= MouseDown;
            MRectangle.MouseLeftButtonDown += Select_MouseDown;
            MRectangle.MouseEnter += GotFocus;
            base.msi.MouseMove -= MouseMove;
            base.msi.MouseUp -= MouseUp;
            M_FiguresCanvas.MouseUp -= MouseUp;
            Application.Current.MainWindow.MouseLeave -= MouseUp;
            Application.Current.MainWindow.MouseUp -= MouseUp;
            base.FinishFunc(this, e);
        }

        private void m_ThumbMove_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Center, e.HorizontalChange, e.VerticalChange);
        }

        private void m_ThumbB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Bottom, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbL_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Left, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbLB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.LeftBottom, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbLT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.LeftTop, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbR_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Right, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbRB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.RightBottom, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbRT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.RightTop, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void m_ThumbT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Top, e.HorizontalChange, e.VerticalChange);
            base.UpadteTextBlock();
        }

        private void Select_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                Rectangle rectangle = (Rectangle)frameworkElement;
                foreach (AnnoBase item in base.objectlist)
                {
                    item.IsActive(Visibility.Collapsed);
                    if (item.ControlName == rectangle.Name)
                    {
                        item.IsActive(Visibility.Visible);
                        base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
                    }
                }
            }
        }

        private void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            double x = Math.Min(base.CurrentStart.X, base.CurrentEnd.X);
            double x2 = Math.Max(base.CurrentStart.X, base.CurrentEnd.X);
            double y = Math.Min(base.CurrentStart.Y, base.CurrentEnd.Y);
            double y2 = Math.Max(base.CurrentStart.Y, base.CurrentEnd.Y);
            base.CurrentStart = new Point(x, y);
            base.CurrentEnd = new Point(x2, y2);
            base.OriginStart = MsiToCanvas(base.CurrentStart);
            base.OriginEnd = MsiToCanvas(base.CurrentEnd);
        }

        public override void DeleteItem()
        {
            M_FiguresCanvas.Children.Remove(MRectangle);
            M_FiguresCanvas.Children.Remove(base.MTextBlock);
            M_FiguresCanvas.Children.Remove(ThumbB);
            M_FiguresCanvas.Children.Remove(ThumbL);
            M_FiguresCanvas.Children.Remove(ThumbLB);
            M_FiguresCanvas.Children.Remove(ThumbLT);
            M_FiguresCanvas.Children.Remove(ThumbR);
            M_FiguresCanvas.Children.Remove(ThumbMove);
            M_FiguresCanvas.Children.Remove(ThumbRT);
            M_FiguresCanvas.Children.Remove(ThumbT);
            M_FiguresCanvas.Children.Remove(ThumbRB);
            base.objectlist.Remove(this);
        }

        public override void UpdateVisual()
        {
            if (ThumbMove != null)
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
                double x = MsiToCanvas(base.CurrentStart).X;
                double y = MsiToCanvas(base.CurrentStart).Y;
                double x2 = MsiToCanvas(base.CurrentEnd).X;
                double y2 = MsiToCanvas(base.CurrentEnd).Y;
                double num = Math.Abs(x - x2);
                double num2 = Math.Abs(y - y2);
                x = Math.Min(x, x2);
                y = Math.Min(y, y2);
                ThumbB.SetValue(Canvas.LeftProperty, x + num / 2.0 - Setting.Thumb_w / 2.0);
                ThumbB.SetValue(Canvas.TopProperty, y + num2 - Setting.Thumb_w / 2.0);
                ThumbL.SetValue(Canvas.LeftProperty, x - Setting.Thumb_w / 2.0);
                ThumbL.SetValue(Canvas.TopProperty, y + num2 / 2.0 - Setting.Thumb_w / 2.0);
                ThumbLB.SetValue(Canvas.LeftProperty, x - Setting.Thumb_w / 2.0);
                ThumbLB.SetValue(Canvas.TopProperty, y + num2 - Setting.Thumb_w / 2.0);
                ThumbLT.SetValue(Canvas.LeftProperty, x - Setting.Thumb_w / 2.0);
                ThumbLT.SetValue(Canvas.TopProperty, y - Setting.Thumb_w / 2.0);
                ThumbR.SetValue(Canvas.LeftProperty, x + num - Setting.Thumb_w / 2.0);
                ThumbR.SetValue(Canvas.TopProperty, y + num2 / 2.0 - Setting.Thumb_w / 2.0);
                ThumbRB.SetValue(Canvas.LeftProperty, x + num - Setting.Thumb_w / 2.0);
                ThumbRB.SetValue(Canvas.TopProperty, y + num2 - Setting.Thumb_w / 2.0);
                ThumbRT.SetValue(Canvas.LeftProperty, x + num - Setting.Thumb_w / 2.0);
                ThumbRT.SetValue(Canvas.TopProperty, y - Setting.Thumb_w / 2.0);
                ThumbT.SetValue(Canvas.LeftProperty, x + num / 2.0 - Setting.Thumb_w / 2.0);
                ThumbT.SetValue(Canvas.TopProperty, y - Setting.Thumb_w / 2.0);
                ThumbMove.SetValue(Canvas.LeftProperty, x + num / 2.0 - Setting.Thumb_c / 2.0);
                ThumbMove.SetValue(Canvas.TopProperty, y + num2 / 2.0 - Setting.Thumb_c / 2.0);
                MRectangle.StrokeThickness = base.Size;
                MRectangle.Stroke = base.BorderBrush;
                MRectangle.Width = num;
                MRectangle.Height = num2;
                Canvas.SetLeft(MRectangle, x);
                Canvas.SetTop(MRectangle, y);
                Canvas.SetLeft(base.MTextBlock, x + num / 2.0);
                Canvas.SetTop(base.MTextBlock, y + num2 / 2.0);
                MRectangle.Visibility = base.isHidden;
                base.MTextBlock.Visibility = base.isHidden;
                base.MTextBlock.Text = CalcMeasureInfo();
            }
        }

        public override void CreateMTextBlock()
        {
            base.MTextBlock = new TextBlock();
            base.MTextBlock.Visibility = Visibility.Collapsed;
            base.MTextBlock.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            base.MTextBlock.Opacity = 0.7;
            base.MTextBlock.MaxWidth = 150.0;
            base.MTextBlock.TextWrapping = TextWrapping.Wrap;
            base.MTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            base.MTextBlock.FontWeight = FontWeights.Bold;
            base.MTextBlock.Padding = new Thickness(2.0, 2.0, 2.0, 2.0);
            base.MTextBlock.Text = CalcMeasureInfo();
            base.MTextBlock.SetValue(Canvas.LeftProperty, (base.OriginStart.X + base.OriginEnd.X) / 2.0);
            base.MTextBlock.SetValue(Canvas.TopProperty, (base.OriginStart.Y + base.OriginEnd.Y) / 2.0);
            M_FiguresCanvas.Children.Add(base.MTextBlock);
        }

        public override void CreateThumb()
        {
            if (ThumbB == null)
            {
                ThumbB = new Thumb();
                ThumbL = new Thumb();
                ThumbLB = new Thumb();
                ThumbLT = new Thumb();
                ThumbR = new Thumb();
                ThumbRB = new Thumb();
                ThumbRT = new Thumb();
                ThumbT = new Thumb();
                ThumbMove = new Thumb();
                ThumbB.Height = Setting.Thumb_w;
                ThumbB.Width = Setting.Thumb_w;
                ThumbL.Height = Setting.Thumb_w;
                ThumbL.Width = Setting.Thumb_w;
                ThumbLB.Height = Setting.Thumb_w;
                ThumbLB.Width = Setting.Thumb_w;
                ThumbLT.Height = Setting.Thumb_w;
                ThumbLT.Width = Setting.Thumb_w;
                ThumbR.Height = Setting.Thumb_w;
                ThumbR.Width = Setting.Thumb_w;
                ThumbRB.Height = Setting.Thumb_w;
                ThumbRB.Width = Setting.Thumb_w;
                ThumbRT.Height = Setting.Thumb_w;
                ThumbRT.Width = Setting.Thumb_w;
                ThumbT.Height = Setting.Thumb_w;
                ThumbT.Width = Setting.Thumb_w;
                ThumbMove.Height = Setting.Thumb_c;
                ThumbMove.Width = Setting.Thumb_c;
                double num = (double)MRectangle.GetValue(Canvas.LeftProperty);
                double num2 = (double)MRectangle.GetValue(Canvas.TopProperty);
                ThumbB.SetValue(Canvas.LeftProperty, num + MRectangle.Width / 2.0 - Setting.Thumb_w / 2.0);
                ThumbB.SetValue(Canvas.TopProperty, num2 + MRectangle.Height - Setting.Thumb_w / 2.0);
                ThumbL.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
                ThumbL.SetValue(Canvas.TopProperty, num2 + MRectangle.Height / 2.0 - Setting.Thumb_w / 2.0);
                ThumbLB.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
                ThumbLB.SetValue(Canvas.TopProperty, num2 + MRectangle.Height - Setting.Thumb_w / 2.0);
                ThumbLT.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
                ThumbLT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
                ThumbR.SetValue(Canvas.LeftProperty, num + MRectangle.Width - Setting.Thumb_w / 2.0);
                ThumbR.SetValue(Canvas.TopProperty, num2 + MRectangle.Height / 2.0 - Setting.Thumb_w / 2.0);
                ThumbRB.SetValue(Canvas.LeftProperty, num + MRectangle.Width - Setting.Thumb_w / 2.0);
                ThumbRB.SetValue(Canvas.TopProperty, num2 + MRectangle.Height - Setting.Thumb_w / 2.0);
                ThumbRT.SetValue(Canvas.LeftProperty, num + MRectangle.Width - Setting.Thumb_w / 2.0);
                ThumbRT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
                ThumbT.SetValue(Canvas.LeftProperty, num + MRectangle.Width / 2.0 - Setting.Thumb_w / 2.0);
                ThumbT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
                ThumbMove.SetValue(Canvas.LeftProperty, num + MRectangle.Width / 2.0 - Setting.Thumb_c / 2.0);
                ThumbMove.SetValue(Canvas.TopProperty, num2 + MRectangle.Height / 2.0 - Setting.Thumb_c / 2.0);
                M_FiguresCanvas.Children.Add(ThumbL);
                M_FiguresCanvas.Children.Add(ThumbLB);
                M_FiguresCanvas.Children.Add(ThumbLT);
                M_FiguresCanvas.Children.Add(ThumbR);
                M_FiguresCanvas.Children.Add(ThumbRT);
                M_FiguresCanvas.Children.Add(ThumbT);
                M_FiguresCanvas.Children.Add(ThumbMove);
                M_FiguresCanvas.Children.Add(ThumbB);
                M_FiguresCanvas.Children.Add(ThumbRB);
                ThumbB.DragDelta += m_ThumbB_DragDelta;
                ThumbL.DragDelta += m_ThumbL_DragDelta;
                ThumbLB.DragDelta += m_ThumbLB_DragDelta;
                ThumbLT.DragDelta += m_ThumbLT_DragDelta;
                ThumbR.DragDelta += m_ThumbR_DragDelta;
                ThumbRB.DragDelta += m_ThumbRB_DragDelta;
                ThumbRT.DragDelta += m_ThumbRT_DragDelta;
                ThumbT.DragDelta += m_ThumbT_DragDelta;
                ThumbMove.DragDelta += m_ThumbMove_DragDelta;
                ThumbB.DragCompleted += DragCompleted;
                ThumbL.DragCompleted += DragCompleted;
                ThumbLB.DragCompleted += DragCompleted;
                ThumbLT.DragCompleted += DragCompleted;
                ThumbR.DragCompleted += DragCompleted;
                ThumbRB.DragCompleted += DragCompleted;
                ThumbRT.DragCompleted += DragCompleted;
                ThumbT.DragCompleted += DragCompleted;
                ThumbMove.DragCompleted += DragCompleted;
            }
        }

        private string GetLength()
        {
            return Math.Round(2.0 * MRectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale + 2.0 * MRectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2).ToString();
        }

        private string GetArea()
        {
            return Math.Round(MRectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale * MRectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2).ToString();
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
            writer.WriteAttributeString("Zoom", base.Zoom.ToString());
            writer.WriteAttributeString("Visible", base.isVisble.ToString());
            writer.WriteAttributeString("MsVisble", base.isMsVisble.ToString());
            writer.WriteAttributeString("Color", GetColor().ToString());
            writer.WriteAttributeString("PinType", base.PinType);
            writer.WriteStartElement("Vertices");
            writer.WriteStartElement("Vertice");
            writer.WriteAttributeString("X", Math.Round(base.CurrentStart.X, 0).ToString());
            writer.WriteAttributeString("Y", Math.Round(base.CurrentStart.Y, 0).ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Vertice");
            writer.WriteAttributeString("X", Math.Round(base.CurrentEnd.X, 0).ToString());
            writer.WriteAttributeString("Y", Math.Round(base.CurrentEnd.Y, 0).ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

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
                stringBuilder.AppendLine(Setting.Width + Math.Round(MRectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale, 2) + Setting.Unit);
                stringBuilder.AppendLine(Setting.Height + Math.Round(MRectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2) + Setting.Unit);
                stringBuilder.AppendLine(Setting.Area + GetArea() + Setting.Area_Unit);
                stringBuilder.Append(Setting.Length + GetLength() + Setting.Unit);
            }
            return stringBuilder.ToString();
        }

        public override string CalcMeasureInfo1()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Setting.Width + Math.Round(MRectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale, 2) + Setting.Unit);
            stringBuilder.AppendLine(Setting.Height + Math.Round(MRectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2) + Setting.Unit);
            stringBuilder.AppendLine(Setting.Area + GetArea() + Setting.Area_Unit);
            stringBuilder.Append(Setting.Length + GetLength() + Setting.Unit);
            return stringBuilder.ToString();
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

        public override void IsActive(Visibility A)
        {
            if (ThumbMove != null)
            {
                ThumbB.Visibility = A;
                ThumbLB.Visibility = A;
                ThumbL.Visibility = A;
                ThumbLT.Visibility = A;
                ThumbR.Visibility = A;
                ThumbRB.Visibility = A;
                ThumbRT.Visibility = A;
                ThumbT.Visibility = A;
                ThumbMove.Visibility = A;
            }
        }
    }
}
