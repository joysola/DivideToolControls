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
	public class AnnoDIYCtcRect : AnnoBase
	{
		public Rectangle m_rectangle;

		private Thumb m_ThumbB;

		private Thumb m_ThumbL;

		private Thumb m_ThumbLB;

		private Thumb m_ThumbLT;

		private Thumb m_ThumbR;

		private Thumb m_ThumbRB;

		private Thumb m_ThumbRT;

		private Thumb m_ThumbT;

		private Thumb m_ThumbMove;

		private Point point;

		public Thumb ThumbB
		{
			get
			{
				return m_ThumbB;
			}
			set
			{
				m_ThumbB = value;
			}
		}

		public Thumb ThumbL
		{
			get
			{
				return m_ThumbL;
			}
			set
			{
				m_ThumbL = value;
			}
		}

		public Thumb ThumbLB
		{
			get
			{
				return m_ThumbLB;
			}
			set
			{
				m_ThumbLB = value;
			}
		}

		public Thumb ThumbLT
		{
			get
			{
				return m_ThumbLT;
			}
			set
			{
				m_ThumbLT = value;
			}
		}

		public Thumb ThumbR
		{
			get
			{
				return m_ThumbR;
			}
			set
			{
				m_ThumbR = value;
			}
		}

		public Thumb ThumbRB
		{
			get
			{
				return m_ThumbRB;
			}
			set
			{
				m_ThumbRB = value;
			}
		}

		public Thumb ThumbRT
		{
			get
			{
				return m_ThumbRT;
			}
			set
			{
				m_ThumbRT = value;
			}
		}

		public Thumb ThumbT
		{
			get
			{
				return m_ThumbT;
			}
			set
			{
				m_ThumbT = value;
			}
		}

		public Thumb ThumbMove
		{
			get
			{
				return m_ThumbMove;
			}
			set
			{
				m_ThumbMove = value;
			}
		}

		public Rectangle MRectangle
		{
			get
			{
				return m_rectangle;
			}
			set
			{
				m_rectangle = value;
			}
		}

		public AnnoDIYCtcRect(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, List<AnnoBase> objectlist, int SlideZoom, double Calibration)
		{
			SetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
			msi.MouseLeftButtonDown += MouseDown;
			base.AnnotationType = AnnotationType.DiyCtcRectangle;
			base.ControlName = base.AnnotationType + DateTime.Now.ToString("yyyyMMddHHmmss");
		}

		public void unload()
		{
			base.msi.MouseLeftButtonDown -= MouseDown;
		}

		public AnnoDIYCtcRect(AnnoBase ab)
		{
			base.AnnotationType = AnnotationType.DiyCtcRectangle;
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
			base.FontSize = ab.FontSize;
			m_rectangle = new Rectangle();
			m_rectangle.Name = ab.ControlName;
			Point originStart = MsiToCanvas(base.CurrentStart);
			Point originEnd = MsiToCanvas(base.CurrentEnd);
			base.OriginStart = originStart;
			base.OriginEnd = originEnd;
			m_rectangle.SetValue(Canvas.LeftProperty, originStart.X);
			m_rectangle.SetValue(Canvas.TopProperty, originStart.Y);
			m_rectangle.Width = originEnd.X - originStart.X;
			m_rectangle.Height = originEnd.Y - originStart.Y;
			m_rectangle.StrokeThickness = base.Size;
			m_rectangle.Stroke = base.BorderBrush;
			M_FiguresCanvas.Children.Add(m_rectangle);
			base.objectlist.Insert(0, this);
			CreateMTextBlock();
			CreateThumb();
			UpdateCB();
			base.UpadteTextBlock();
			IsActive(Visibility.Collapsed);
			base.AnnoControl.CB.SelectedIndex = -1;
		}

		public void GotFocus(object sender, EventArgs e)
		{
			base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
		}

		private void MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!base.isdraw)
			{
				m_rectangle = new Rectangle();
				m_rectangle.Name = base.ControlName;
				Point position = e.GetPosition(M_FiguresCanvas);
				m_rectangle.SetValue(Canvas.LeftProperty, position.X);
				m_rectangle.SetValue(Canvas.TopProperty, position.Y);
				m_rectangle.Width = 0.0;
				m_rectangle.Height = 0.0;
				m_rectangle.StrokeThickness = base.Size;
				m_rectangle.Stroke = new SolidColorBrush(Setting.STROCK_COLOR5);
				base.OriginStart = position;
				base.OriginEnd = position;
				point = position;
				base.CurrentStart = CanvasToMsi(position);
				base.CurrentEnd = CanvasToMsi(position);
				M_FiguresCanvas.Children.Add(m_rectangle);
				base.msi.MouseMove += MouseMove;
				Application.Current.MainWindow.MouseUp += MouseUp;
				base.AnnotationName = Setting.CtcRectangle + (base.objectlist.Count + 1);
				base.AnnotationDescription = "";
				base.objectlist.Insert(0, this);
				UpdateCB();
				base.isdraw = true;
			}
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(M_FiguresCanvas);
			Point originEnd = new Point(Math.Max(point.X, position.X), Math.Max(point.Y, position.Y));
			Point originStart = new Point(Math.Min(point.X, position.X), Math.Min(point.Y, position.Y));
			m_rectangle.SetValue(Canvas.LeftProperty, originStart.X);
			m_rectangle.SetValue(Canvas.TopProperty, originStart.Y);
			m_rectangle.Width = originEnd.X - originStart.X;
			m_rectangle.Height = originEnd.Y - originStart.Y;
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
			base.msi.MouseLeftButtonDown -= MouseDown;
			m_rectangle.MouseLeftButtonDown += Select_MouseDown;
			m_rectangle.MouseEnter += GotFocus;
			base.msi.MouseMove -= MouseMove;
			base.msi.MouseUp -= MouseUp;
			M_FiguresCanvas.MouseUp -= MouseUp;
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
			M_FiguresCanvas.Children.Remove(m_rectangle);
			M_FiguresCanvas.Children.Remove(base.MTextBlock);
			M_FiguresCanvas.Children.Remove(m_ThumbB);
			M_FiguresCanvas.Children.Remove(m_ThumbL);
			M_FiguresCanvas.Children.Remove(m_ThumbLB);
			M_FiguresCanvas.Children.Remove(m_ThumbLT);
			M_FiguresCanvas.Children.Remove(m_ThumbR);
			M_FiguresCanvas.Children.Remove(ThumbMove);
			M_FiguresCanvas.Children.Remove(m_ThumbRT);
			M_FiguresCanvas.Children.Remove(m_ThumbT);
			M_FiguresCanvas.Children.Remove(m_ThumbRB);
			base.objectlist.Remove(this);
		}

		public override void UpdateVisual()
		{
			if (base.MTextBlock != null)
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
				m_rectangle.StrokeThickness = base.Size;
				m_rectangle.Stroke = new SolidColorBrush(Setting.STROCK_COLOR5);
				m_rectangle.Width = num;
				m_rectangle.Height = num2;
				Canvas.SetLeft(m_rectangle, x);
				Canvas.SetTop(m_rectangle, y);
				Canvas.SetLeft(base.MTextBlock, x + num + 2.0);
				Canvas.SetTop(base.MTextBlock, y + num2 + 2.0);
				m_rectangle.Visibility = base.isHidden;
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
			if (m_ThumbB == null)
			{
				m_ThumbB = new Thumb();
				m_ThumbL = new Thumb();
				m_ThumbLB = new Thumb();
				m_ThumbLT = new Thumb();
				m_ThumbR = new Thumb();
				m_ThumbRB = new Thumb();
				m_ThumbRT = new Thumb();
				m_ThumbT = new Thumb();
				ThumbMove = new Thumb();
				m_ThumbB.Height = Setting.Thumb_w;
				m_ThumbB.Width = Setting.Thumb_w;
				m_ThumbL.Height = Setting.Thumb_w;
				m_ThumbL.Width = Setting.Thumb_w;
				m_ThumbLB.Height = Setting.Thumb_w;
				m_ThumbLB.Width = Setting.Thumb_w;
				m_ThumbLT.Height = Setting.Thumb_w;
				m_ThumbLT.Width = Setting.Thumb_w;
				m_ThumbR.Height = Setting.Thumb_w;
				m_ThumbR.Width = Setting.Thumb_w;
				m_ThumbRB.Height = Setting.Thumb_w;
				m_ThumbRB.Width = Setting.Thumb_w;
				m_ThumbRT.Height = Setting.Thumb_w;
				m_ThumbRT.Width = Setting.Thumb_w;
				m_ThumbT.Height = Setting.Thumb_w;
				m_ThumbT.Width = Setting.Thumb_w;
				ThumbMove.Height = Setting.Thumb_c;
				ThumbMove.Width = Setting.Thumb_c;
				double num = (double)m_rectangle.GetValue(Canvas.LeftProperty);
				double num2 = (double)m_rectangle.GetValue(Canvas.TopProperty);
				m_ThumbB.SetValue(Canvas.LeftProperty, num + m_rectangle.Width / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbB.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height - Setting.Thumb_w / 2.0);
				m_ThumbL.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
				m_ThumbL.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbLB.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
				m_ThumbLB.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height - Setting.Thumb_w / 2.0);
				m_ThumbLT.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
				m_ThumbLT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
				m_ThumbR.SetValue(Canvas.LeftProperty, num + m_rectangle.Width - Setting.Thumb_w / 2.0);
				m_ThumbR.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbRB.SetValue(Canvas.LeftProperty, num + m_rectangle.Width - Setting.Thumb_w / 2.0);
				m_ThumbRB.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height - Setting.Thumb_w / 2.0);
				m_ThumbRT.SetValue(Canvas.LeftProperty, num + m_rectangle.Width - Setting.Thumb_w / 2.0);
				m_ThumbRT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
				m_ThumbT.SetValue(Canvas.LeftProperty, num + m_rectangle.Width / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
				ThumbMove.SetValue(Canvas.LeftProperty, num + m_rectangle.Width / 2.0 - Setting.Thumb_c / 2.0);
				ThumbMove.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height / 2.0 - Setting.Thumb_c / 2.0);
				M_FiguresCanvas.Children.Add(m_ThumbL);
				M_FiguresCanvas.Children.Add(m_ThumbLB);
				M_FiguresCanvas.Children.Add(m_ThumbLT);
				M_FiguresCanvas.Children.Add(m_ThumbR);
				M_FiguresCanvas.Children.Add(m_ThumbRT);
				M_FiguresCanvas.Children.Add(m_ThumbT);
				M_FiguresCanvas.Children.Add(ThumbMove);
				M_FiguresCanvas.Children.Add(m_ThumbB);
				M_FiguresCanvas.Children.Add(m_ThumbRB);
				m_ThumbB.DragDelta += m_ThumbB_DragDelta;
				m_ThumbL.DragDelta += m_ThumbL_DragDelta;
				m_ThumbLB.DragDelta += m_ThumbLB_DragDelta;
				m_ThumbLT.DragDelta += m_ThumbLT_DragDelta;
				m_ThumbR.DragDelta += m_ThumbR_DragDelta;
				m_ThumbRB.DragDelta += m_ThumbRB_DragDelta;
				m_ThumbRT.DragDelta += m_ThumbRT_DragDelta;
				m_ThumbT.DragDelta += m_ThumbT_DragDelta;
				m_ThumbMove.DragDelta += m_ThumbMove_DragDelta;
				m_ThumbB.DragCompleted += DragCompleted;
				m_ThumbL.DragCompleted += DragCompleted;
				m_ThumbLB.DragCompleted += DragCompleted;
				m_ThumbLT.DragCompleted += DragCompleted;
				m_ThumbR.DragCompleted += DragCompleted;
				m_ThumbRB.DragCompleted += DragCompleted;
				m_ThumbRT.DragCompleted += DragCompleted;
				m_ThumbT.DragCompleted += DragCompleted;
				m_ThumbMove.DragCompleted += DragCompleted;
			}
		}

		private string GetLength()
		{
			return Math.Round(2.0 * m_rectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale + 2.0 * m_rectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2).ToString();
		}

		private string GetArea()
		{
			return Math.Round(m_rectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale * m_rectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2).ToString();
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
			writer.WriteAttributeString("X", Math.Round(base.CurrentStart.X, 2).ToString());
			writer.WriteAttributeString("Y", Math.Round(base.CurrentStart.Y, 2).ToString());
			writer.WriteEndElement();
			writer.WriteStartElement("Vertice");
			writer.WriteAttributeString("X", Math.Round(base.CurrentEnd.X, 2).ToString());
			writer.WriteAttributeString("Y", Math.Round(base.CurrentEnd.Y, 2).ToString());
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
			return stringBuilder.ToString();
		}

		public override string CalcMeasureInfo1()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Setting.Width + Math.Round(m_rectangle.Width * base.Calibration / base.msi.ZoomableCanvas.Scale, 2) + Setting.Unit);
			stringBuilder.AppendLine(Setting.Height + Math.Round(m_rectangle.Height * base.Calibration / base.msi.ZoomableCanvas.Scale, 2) + Setting.Unit);
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
