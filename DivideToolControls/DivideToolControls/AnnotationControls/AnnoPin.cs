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
using System.Windows.Media.Imaging;
using System.Xml;

namespace DivideToolControls.AnnotationControls
{
    public class AnnoPin : AnnoBase
    {
        private Thumb ThumbMove { get; set; }

        public TextBox TB { get; set; }

        private Image NewImage { get; set; }

        public AnnoPin(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, int SlideZoom, List<AnnoBase> objectlist)
        {
            SetPara(alc, canvasboard, msi, objectlist, SlideZoom, base.Calibration);
            msi.MouseLeftButtonDown += Pinline_handle_MouseDown;
            base.AnnotationType = AnnotationType.Remark;
            base.ControlName = base.AnnotationType + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public void Unload()
        {
            base.msi.MouseLeftButtonDown -= Pinline_handle_MouseDown;
        }

        public AnnoPin(AnnoBase ab)
        {
            base.AnnotationType = AnnotationType.Remark;
            base.ControlName = ab.ControlName;
            base.Calibration = ab.Calibration;
            base.SlideZoom = ab.SlideZoom;
            base.AnnotationDescription = ab.AnnotationDescription;
            base.CurrentStart = ab.CurrentStart;
            base.CurrentEnd = ab.CurrentEnd;
            base.Size = ab.Size;
            base.FontColor = ab.FontColor;
            base.isVisble = ab.isVisble;
            base.isHidden = ab.isHidden;
            base.isMsVisble = ab.isMsVisble;
            base.AnnotationName = ab.AnnotationName;
            base.FontBold = ab.FontBold;
            base.FontItalic = ab.FontItalic;
            base.FontSize = ab.FontSize;
            base.PinType = ab.PinType;
            base.isFinish = true;
            base.Zoom = ab.Zoom;
            base.msi = ab.msi;
            base.FiguresCanvas = ab.FiguresCanvas;
            base.objectlist = ab.objectlist;
            base.AnnoControl = ab.AnnoControl;
            Point point = MsiToCanvas(base.CurrentStart);
            NewImage = new Image();
            NewImage.Name = base.ControlName;
            NewImage.Source = new BitmapImage(new Uri(base.PinType, UriKind.Relative));
            NewImage.Width = 40.0;
            NewImage.SetValue(Canvas.LeftProperty, point.X - 20.0);
            NewImage.SetValue(Canvas.TopProperty, point.Y - 20.0);
            M_FiguresCanvas.Children.Add(NewImage);
            base.OriginStart = point;
            base.CurrentStart = CanvasToMsi(point);
            base.CurrentEnd = base.CurrentStart;
            CreateMTextBlock();
            CreateThumb();
            NewImage.MouseLeftButtonDown += Pin_MouseDown;
            NewImage.MouseEnter += GotFocus;
            base.objectlist.Insert(0, this);
            UpdateCB();
            base.UpadteTextBlock();
            IsActive(Visibility.Collapsed);
            base.AnnoControl.CB.SelectedIndex = -1;
        }

        public override string CalcMeasureInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (base.AnnotationDescription.Trim() != "" && base.isMsVisble)
            {
                stringBuilder.Append(Setting.AnnoDesStr + base.AnnotationDescription);
            }
            else
            {
                base.MTextBlock.Visibility = Visibility.Collapsed;
            }
            return stringBuilder.ToString();
        }

        public void GotFocus(object sender, EventArgs e)
        {
            if (Setting.isAnnoChange)
            {
                base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
            }
        }

        private void Pinline_handle_MouseDown(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(base.msi);
            NewImage = new Image();
            NewImage.Name = base.ControlName;
            NewImage.Source = new BitmapImage(new Uri(base.PinType, UriKind.Relative));
            NewImage.Width = 40.0;
            NewImage.SetValue(Canvas.LeftProperty, position.X - 20.0);
            NewImage.SetValue(Canvas.TopProperty, position.Y - 40.0);
            M_FiguresCanvas.Children.Add(NewImage);
            base.CurrentStart = CanvasToMsi(position);
            base.CurrentEnd = base.CurrentStart;
            base.AnnotationName = Setting.Remark + (base.objectlist.Count + 1);
            base.AnnotationDescription = "";
            base.objectlist.Insert(0, this);
            CreateMTextBlock();
            UpdateCB();
            base.isFinish = true;
            CreateThumb();
            NewImage.MouseLeftButtonDown += Pin_MouseDown;
            NewImage.MouseEnter += GotFocus;
            base.msi.MouseLeftButtonDown -= Pinline_handle_MouseDown;
            base.FinishFunc(this, e);
        }

        public override void CreateMTextBlock()
        {
            base.MTextBlock = new TextBlock();
            base.MTextBlock.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            base.MTextBlock.Opacity = 0.7;
            base.MTextBlock.MaxWidth = 150.0;
            base.MTextBlock.TextWrapping = TextWrapping.Wrap;
            base.MTextBlock.Padding = new Thickness(2.0, 2.0, 2.0, 2.0);
            base.MTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            base.MTextBlock.FontWeight = FontWeights.Bold;
            base.MTextBlock.Visibility = Visibility.Collapsed;
            base.MTextBlock.Text = CalcMeasureInfo();
            base.MTextBlock.SetValue(Canvas.LeftProperty, base.OriginStart.X);
            base.MTextBlock.SetValue(Canvas.TopProperty, base.OriginStart.Y);
            M_FiguresCanvas.Children.Add(base.MTextBlock);
        }

        private void tbTextChanged(object sender, TextChangedEventArgs e)
        {
            base.AnnoControl.qsr.Text = TB.Text;
            base.AnnotationDescription = TB.Text;
            ThumbMove.SetValue(Canvas.LeftProperty, MsiToCanvas(base.CurrentStart).X + TB.ActualWidth / 2.0 - Setting.Thumb_w / 2.0);
            ThumbMove.SetValue(Canvas.TopProperty, MsiToCanvas(base.CurrentStart).Y + TB.ActualHeight / 2.0 - Setting.Thumb_w / 2.0);
        }

        private void reThumb(object sender, RoutedEventArgs e)
        {
            ThumbMove.SetValue(Canvas.LeftProperty, MsiToCanvas(base.CurrentStart).X + TB.ActualWidth / 2.0 - Setting.Thumb_w / 2.0);
            ThumbMove.SetValue(Canvas.TopProperty, MsiToCanvas(base.CurrentStart).Y + TB.ActualHeight / 2.0 - Setting.Thumb_w / 2.0);
        }

        private void Pin_tb_MouseDown(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                TextBox textBox = (TextBox)frameworkElement;
                foreach (AnnoBase item in base.objectlist)
                {
                    if (item.GetType() == typeof(AnnoPin) && ((AnnoPin)item).TB.Name == textBox.Name)
                    {
                        ((AnnoPin)item).IsActive(Visibility.Visible);
                        base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
                    }
                }
            }
        }

        private void Pin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Cursor = Cursors.Hand;
                Image image = (Image)frameworkElement;
                foreach (AnnoBase item in base.objectlist)
                {
                    item.IsActive(Visibility.Collapsed);
                    if (item.ControlName == image.Name)
                    {
                        item.IsActive(Visibility.Visible);
                        base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
                    }
                }
            }
        }

        private void PinLine_ThumbMove(object sender, DragDeltaEventArgs e)
        {
            ResetLocation(Direction.Center, e.HorizontalChange, e.VerticalChange);
        }

        public override void DeleteItem()
        {
            M_FiguresCanvas.Children.Remove(NewImage);
            M_FiguresCanvas.Children.Remove(base.MTextBlock);
            M_FiguresCanvas.Children.Remove(ThumbMove);
            base.objectlist.Remove(this);
        }

        public override void CreateThumb()
        {
            if (ThumbMove == null)
            {
                ThumbMove = new Thumb();
                ThumbMove.Height = Setting.Thumb_w;
                ThumbMove.Width = Setting.Thumb_w;
                ThumbMove.SetValue(Canvas.LeftProperty, base.OriginStart.X - Setting.Thumb_w / 2.0);
                ThumbMove.SetValue(Canvas.TopProperty, base.OriginStart.Y - 20.0 - Setting.Thumb_w / 2.0);
                M_FiguresCanvas.Children.Add(ThumbMove);
                ThumbMove.DragDelta += PinLine_ThumbMove;
            }
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
                ThumbMove.SetValue(Canvas.LeftProperty, MsiToCanvas(base.CurrentStart).X - Setting.Thumb_w / 2.0);
                ThumbMove.SetValue(Canvas.TopProperty, MsiToCanvas(base.CurrentStart).Y - 20.0 - Setting.Thumb_w / 2.0);
                Canvas.SetLeft(NewImage, MsiToCanvas(base.CurrentStart).X - 20.0);
                Canvas.SetTop(NewImage, MsiToCanvas(base.CurrentStart).Y - 40.0);
                NewImage.Source = new BitmapImage(new Uri(base.PinType, UriKind.Relative));
                NewImage.Visibility = base.isHidden;
                base.MTextBlock.Visibility = base.isHidden;
                base.MTextBlock.Text = CalcMeasureInfo();
                Canvas.SetLeft(base.MTextBlock, MsiToCanvas(base.CurrentStart).X);
                Canvas.SetTop(base.MTextBlock, MsiToCanvas(base.CurrentStart).Y);
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
            writer.WriteAttributeString("MsVisble", base.isMsVisble.ToString());
            writer.WriteAttributeString("Zoom", base.Zoom.ToString());
            writer.WriteAttributeString("Visible", base.isVisble.ToString());
            writer.WriteAttributeString("Color", GetColor1().ToString());
            writer.WriteAttributeString("PinType", base.PinType);
            writer.WriteStartElement("Vertices");
            writer.WriteStartElement("Vertice");
            writer.WriteAttributeString("X", Math.Round(base.CurrentStart.X, 0).ToString());
            writer.WriteAttributeString("Y", Math.Round(base.CurrentStart.Y, 0).ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public override void IsActive(Visibility A)
        {
            if (ThumbMove != null)
            {
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
