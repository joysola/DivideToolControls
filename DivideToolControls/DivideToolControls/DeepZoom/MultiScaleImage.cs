using DivideToolControls.DeepZoomControls;
using DivideToolControls.Extensions;
using DivideToolControls.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DivideToolControls.DeepZoom
{
	[TemplatePart(Name = "PART_ItemsControl", Type = typeof(ItemsControl))]
	public class MultiScaleImage : Control
	{
		private const int ScaleAnimationRelativeDuration = 100;

		private const double MinScaleRelativeToMinSize = 0.2;

		private const int ThrottleIntervalMilliseconds = 10;

		public ItemsControl _itemsControl;

		private ZoomableCanvas _zoomableCanvas;

		public MultiScaleImageSpatialItemsSource _spatialSource;

		private double _originalScale;

		private int _desiredLevel;

		private readonly DispatcherTimer _levelChangeThrottle;

		public static readonly DependencyProperty SourceProperty;

		private static readonly DependencyPropertyKey AspectRatioPropertyKey;

		public static readonly DependencyProperty AspectRatioProperty;

		private double MaxScaleRelativeToMaxSize;

		public ZoomableCanvas ZoomableCanvas
		{
			get
			{
				return _zoomableCanvas;
			}
			set
			{
				_zoomableCanvas = value;
			}
		}

		public MultiScaleTileSource Source
		{
			get
			{
				return (MultiScaleTileSource)GetValue(SourceProperty);
			}
			set
			{
				SetValue(SourceProperty, value);
			}
		}

		public double AspectRatio => (double)GetValue(AspectRatioProperty);

		public event RoutedEventHandler Ini;

		static MultiScaleImage()
		{
			SourceProperty = DependencyProperty.Register("Source", typeof(MultiScaleTileSource), typeof(MultiScaleImage), new FrameworkPropertyMetadata(null, OnSourceChanged));
			AspectRatioPropertyKey = DependencyProperty.RegisterReadOnly("AspectRatio", typeof(double), typeof(MultiScaleImage), new FrameworkPropertyMetadata(1.0));
			AspectRatioProperty = AspectRatioPropertyKey.DependencyProperty;
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiScaleImage), new FrameworkPropertyMetadata(typeof(MultiScaleImage)));
		}

		public MultiScaleImage()
		{
			_levelChangeThrottle = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(10.0),
				IsEnabled = false
			};
			DispatcherTimer levelChangeThrottle = _levelChangeThrottle;
			EventHandler value = delegate
			{
				_spatialSource.CurrentLevel = _desiredLevel;
				_levelChangeThrottle.IsEnabled = false;
			};
			levelChangeThrottle.Tick += value;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			base.IsManipulationEnabled = true;
			_itemsControl = (GetTemplateChild("PART_ItemsControl") as ItemsControl);
			if (_itemsControl != null)
			{
				_itemsControl.ApplyTemplate();
				FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(ZoomableCanvas));
				frameworkElementFactory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(ZoomableCanvasLoaded));
				_itemsControl.ItemsPanel = new ItemsPanelTemplate(frameworkElementFactory);
				if (_spatialSource != null)
				{
					_itemsControl.ItemsSource = _spatialSource;
				}
			}
		}
		/// <summary>
		/// 在应用样式时(OnApplyTemplate)调用此方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ZoomableCanvasLoaded(object sender, RoutedEventArgs e)
		{
			_zoomableCanvas = (sender as ZoomableCanvas);
			if (_zoomableCanvas != null && _zoomableCanvas.Children.Count == 0 && base.Tag != "1")
			{
				_zoomableCanvas.RealizationPriority = DispatcherPriority.Background;
				_zoomableCanvas.RealizationRate = 10;
				InitializeCanvas();
				if (this.Ini != null)
				{
					this.Ini(sender, e);
				}
			}
		}

		public void ZoomAboutLogicalPoint(double zoomIncrementFactor, double zoomCenterLogicalX, double zoomCenterLogicalY)
		{
			Point logicalPoint = new Point(zoomCenterLogicalX, zoomCenterLogicalY);
			ScaleCanvas(zoomIncrementFactor, LogicalToElementPoint(logicalPoint), animate: true);
		}

		public Point ElementToLogicalPoint(Point elementPoint)
		{
			Point canvasPoint = _zoomableCanvas.GetCanvasPoint(elementPoint);
			return new Point(canvasPoint.X / _zoomableCanvas.Extent.Width, canvasPoint.Y / _zoomableCanvas.Extent.Height);
		}

		public Point LogicalToElementPoint(Point logicalPoint)
		{
			Point canvasPoint = new Point(logicalPoint.X * _zoomableCanvas.Extent.Width, logicalPoint.Y * _zoomableCanvas.Extent.Height);
			return _zoomableCanvas.GetVisualPoint(canvasPoint);
		}

		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiScaleImage multiScaleImage = (MultiScaleImage)d;
			MultiScaleTileSource oldSource = (MultiScaleTileSource)e.OldValue;
			MultiScaleTileSource source = multiScaleImage.Source;
			multiScaleImage.OnSourceChanged(oldSource, source);
		}

		protected virtual void OnSourceChanged(MultiScaleTileSource oldSource, MultiScaleTileSource newSource)
		{
			if (newSource == null)
			{
				_spatialSource = null;
				return;
			}
			_spatialSource = new MultiScaleImageSpatialItemsSource(newSource);
			if (_itemsControl != null)
			{
				_itemsControl.ItemsSource = _spatialSource;
			}
			if (_zoomableCanvas != null)
			{
				InitializeCanvas();
			}
		}

		protected void SetAspectRatio(double value)
		{
			SetValue(AspectRatioPropertyKey, value);
		}

		public void InitializeCanvas()
		{
			int level = Source.GetLevel(_zoomableCanvas.ActualWidth, _zoomableCanvas.ActualHeight);
			_spatialSource.CurrentLevel = level;
			Size imageSize = Source.ImageSize;
			double num = _originalScale = Math.Min(_itemsControl.ActualWidth / imageSize.Width, _itemsControl.ActualHeight / imageSize.Height);
			_zoomableCanvas.Scale = _originalScale;
			_zoomableCanvas.Offset = new Point(imageSize.Width * 0.5 * num - _zoomableCanvas.ActualWidth * 0.5, imageSize.Height * 0.5 * num - _zoomableCanvas.ActualHeight * 0.5);
			_zoomableCanvas.Clip = new RectangleGeometry(new Rect(0.0, 0.0, imageSize.Width, imageSize.Height));
			SetAspectRatio(_spatialSource.Extent.Width / _spatialSource.Extent.Height);
			_spatialSource.InvalidateSource();
		}
		/// <summary>
		/// 改变scale和offset
		/// </summary>
		/// <param name="relativeScale"></param>
		/// <param name="center"></param>
		/// <param name="animate"></param>
		public void ScaleCanvas(double relativeScale, Point center, bool animate = false)
		{
			double scale = _zoomableCanvas.Scale;
			if (scale <= 0.0)
			{
				return;
			}
			MaxScaleRelativeToMaxSize = Setting.MaxMagValue * Setting.MargPara;
			relativeScale = relativeScale.Clamp(0.2 * _originalScale / scale, Math.Max(MaxScaleRelativeToMaxSize, MaxScaleRelativeToMaxSize * _originalScale) / scale);
			double num = scale * relativeScale;
			int level = Source.GetLevel(num);
			int currentLevel = _spatialSource.CurrentLevel;
			if (level != currentLevel)
			{
				if (level > currentLevel)
				{
					ThrottleChangeLevel(level);
				}
				else
				{
					_spatialSource.CurrentLevel = level;
				}
			}
			if (num == scale)
			{
				return;
			}
			Vector vector = (Vector)center;
			Point point = (Point)((Vector)(_zoomableCanvas.Offset + vector) * relativeScale - vector);
			if (animate)
			{
				if (relativeScale < 1.0)
				{
					relativeScale = 1.0 / relativeScale;
				}
				if (relativeScale > 4.0)
				{
					relativeScale = 4.0;
				}
				double num2 = relativeScale * 100.0;
				if (num2 < 0.0)
				{
					num2 = 1.0;
				}
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(num2);
				CubicEase easingFunction = new CubicEase();
				// scale属性变动
				_zoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(num, timeSpan)
				{
					EasingFunction = easingFunction
				}, HandoffBehavior.Compose);
				// offset属性变动
				_zoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(point, timeSpan)
				{
					EasingFunction = easingFunction
				}, HandoffBehavior.Compose);
			}
			else
			{
				_zoomableCanvas.Scale = num;
				_zoomableCanvas.Offset = point;
			}
		}

		private void ThrottleChangeLevel(int newLevel)
		{
			_desiredLevel = newLevel;
			if (_levelChangeThrottle.IsEnabled)
			{
				_levelChangeThrottle.Stop();
			}
			_levelChangeThrottle.Start();
		}
	}
}
