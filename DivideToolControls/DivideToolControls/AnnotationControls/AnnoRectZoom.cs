using DivideToolControls.DeepZoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DivideToolControls.AnnotationControls
{
	public class AnnoRectZoom : AnnoBase
	{
		private Rectangle m_rectangle;

		private Point point;

		public Point pOffset;

		public Point pCenter;

		public double Scale;

		public Point tOffset;

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

		public event RoutedEventHandler RightZoom;

		public AnnoRectZoom(AnnoListCtls alc, Canvas canvasboard, MultiScaleImage msi, List<AnnoBase> objectlist, int SlideZoom, double Calibration)
		{
			SetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
			msi.MouseRightButtonDown += MouseDown;
			base.ControlName = "Myrectzoom" + DateTime.Now.ToString("yyyyMMddHHmmss");
			Rotate = Rotate;
		}

		public void unload()
		{
			base.msi.MouseRightButtonDown -= MouseDown;
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
				m_rectangle.StrokeThickness = 4.0;
				m_rectangle.Stroke = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0));
				base.OriginStart = position;
				base.OriginEnd = position;
				point = position;
				base.CurrentStart = CanvasToMsi(position);
				base.CurrentEnd = CanvasToMsi(position);
				M_FiguresCanvas.Children.Add(m_rectangle);
				base.msi.MouseMove += MouseMove;
				base.msi.MouseRightButtonUp += MouseUp;
				M_FiguresCanvas.MouseRightButtonUp += MouseUp;
				Application.Current.MainWindow.MouseUp += MouseUp;
				base.isdraw = true;
			}
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			if (base.isdraw)
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
			}
		}

		private void MouseUp(object sender, MouseButtonEventArgs e)
		{
			double num = 0.0;
			double num2 = 0.0;
			base.msi.MouseMove -= MouseMove;
			base.msi.MouseRightButtonUp -= MouseUp;
			M_FiguresCanvas.MouseRightButtonUp -= MouseUp;
			Application.Current.MainWindow.MouseUp -= MouseUp;
			if (m_rectangle.Width > 2.0)
			{
				Point elementPoint = default(Point);
				elementPoint.X = base.OriginStart.X + m_rectangle.Width / 2.0;
				elementPoint.Y = base.OriginStart.Y + m_rectangle.Height / 2.0;
				Point point = new Point(base.msi.ActualWidth / 2.0, base.msi.ActualHeight / 2.0);
				Point offset = base.msi.ZoomableCanvas.Offset;
				offset.X += elementPoint.X - point.X;
				offset.Y += elementPoint.Y - point.Y;
				base.msi.ElementToLogicalPoint(elementPoint);
				double num3 = 0.0;
				num3 = ((!(m_rectangle.Height >= m_rectangle.Width)) ? (base.msi.ActualWidth / m_rectangle.Width) : (base.msi.ActualHeight / m_rectangle.Height));
				num = elementPoint.X - point.X;
				num2 = elementPoint.Y - point.Y;
				pOffset = new Point(num, num2);
				pCenter = elementPoint;
				tOffset = offset;
				Scale = num3;
				if (this.RightZoom != null)
				{
					this.RightZoom(this, new RoutedEventArgs());
				}
			}
			DeleteItem();
			base.isFinish = true;
			base.isdraw = false;
		}

		public override void DeleteItem()
		{
			M_FiguresCanvas.Children.Remove(m_rectangle);
		}

		public override void UpdateVisual()
		{
			double x = MsiToCanvas(base.CurrentStart).X;
			double y = MsiToCanvas(base.CurrentStart).Y;
			double x2 = MsiToCanvas(base.CurrentEnd).X;
			double y2 = MsiToCanvas(base.CurrentEnd).Y;
			double width = Math.Abs(x - x2);
			double height = Math.Abs(y - y2);
			x = Math.Min(x, x2);
			y = Math.Min(y, y2);
			m_rectangle.StrokeThickness = base.Size;
			m_rectangle.Stroke = base.BorderBrush;
			m_rectangle.Width = width;
			m_rectangle.Height = height;
			Canvas.SetLeft(m_rectangle, x);
			Canvas.SetTop(m_rectangle, y);
		}
	}
}
