using DivideToolControls.Extensions;
using DivideToolControls.Extensions.DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DivideToolControls.DeepZoomControls
{
	public class ZoomableCanvas : VirtualPanel, IScrollInfo
	{
		public interface ISpatialItemsSource
		{
			Rect Extent
			{
				get;
			}

			event EventHandler ExtentChanged;

			event EventHandler QueryInvalidated;

			IEnumerable<int> Query(Rect rectangle);
		}

		public class PrivateSpatialIndex : ISpatialItemsSource
		{
			private class SpatialItem
			{
				public int Index
				{
					get;
					set;
				}

				public Rect Bounds
				{
					get;
					set;
				}

				public SpatialItem()
				{
					Index = -1;
					Bounds = Rect.Empty;
				}

				public override string ToString()
				{
					return "Item[" + Index + "].Bounds = " + Bounds;
				}
			}

			private readonly PriorityQuadTree<SpatialItem> _tree = new PriorityQuadTree<SpatialItem>();

			private readonly List<SpatialItem> _items = new List<SpatialItem>();

			private Rect _extent = Rect.Empty;

			private Rect _lastQuery = Rect.Empty;

			public Rect Extent
			{
				get
				{
					if (_extent.IsEmpty)
					{
						foreach (SpatialItem item in _items)
						{
							_extent.Union(item.Bounds);
						}
					}
					return _extent;
				}
			}

			public Rect this[int index]
			{
				get
				{
					return _items[index].Bounds;
				}
				set
				{
					SpatialItem spatialItem = _items[index];
					Rect bounds = spatialItem.Bounds;
					if (bounds != value)
					{
						_extent = Rect.Empty;
						_tree.Remove(spatialItem, bounds);
						_tree.Insert(spatialItem, value, value.IsEmpty ? double.PositiveInfinity : (value.Width + value.Height));
						spatialItem.Bounds = value;
						if (this.ExtentChanged != null)
						{
							this.ExtentChanged(this, EventArgs.Empty);
						}
						if (this.QueryInvalidated != null && (bounds.IntersectsWith(_lastQuery) || value.IntersectsWith(_lastQuery)))
						{
							this.QueryInvalidated(this, EventArgs.Empty);
						}
					}
				}
			}

			public event EventHandler ExtentChanged;

			public event EventHandler QueryInvalidated;

			public IEnumerable<int> Query(Rect bounds)
			{
				_lastQuery = bounds;
				return from i in _tree.GetItemsIntersecting(bounds)
					   select i.Index;
			}

			public void InsertRange(int index, int count)
			{
				SpatialItem[] array = new SpatialItem[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = new SpatialItem();
					array[i].Index = index + i;
					_tree.Insert(array[i], Rect.Empty, double.PositiveInfinity);
				}
				_items.InsertRange(index, array);
				if (this.QueryInvalidated != null)
				{
					this.QueryInvalidated(this, EventArgs.Empty);
				}
			}

			public void RemoveRange(int index, int count)
			{
				for (int i = index; i < _items.Count; i++)
				{
					if (i < index + count)
					{
						_tree.Remove(_items[i], _items[i].Bounds);
					}
					else
					{
						_items[i].Index = i - count;
					}
				}
				_items.RemoveRange(index, count);
				_extent = Rect.Empty;
				if (this.ExtentChanged != null)
				{
					this.ExtentChanged(this, EventArgs.Empty);
				}
				if (this.QueryInvalidated != null)
				{
					this.QueryInvalidated(this, EventArgs.Empty);
				}
			}

			public void Reset(int count)
			{
				_extent = Rect.Empty;
				_items.Clear();
				InsertRange(0, count);
				if (this.ExtentChanged != null)
				{
					this.ExtentChanged(this, EventArgs.Empty);
				}
				if (this.QueryInvalidated != null)
				{
					this.QueryInvalidated(this, EventArgs.Empty);
				}
			}

			public void Optimize()
			{
				Rect extent = _tree.Extent;
				Rect extent2 = Extent;
				if (extent.Top - extent2.Top > extent.Height || extent.Left - extent2.Left > extent.Width || extent2.Right - extent.Right > extent.Width || extent2.Bottom - extent.Bottom > extent.Height)
				{
					_tree.Extent = extent2;
					if (this.QueryInvalidated != null)
					{
						this.QueryInvalidated(this, EventArgs.Empty);
					}
				}
			}
		}

		public static readonly DependencyProperty ApplyTransformProperty;

		public static readonly DependencyProperty ActualViewboxProperty;

		public static readonly DependencyProperty ViewboxProperty;

		public static readonly DependencyProperty StretchProperty;

		public static readonly DependencyProperty StretchDirectionProperty;

		public static readonly DependencyProperty OffsetProperty;

		public static readonly DependencyProperty ScaleProperty;

		public static readonly DependencyProperty RealizationLimitProperty;

		public static readonly DependencyProperty RealizationRateProperty;

		private ISpatialItemsSource SpatialIndex;

		public PrivateSpatialIndex PrivateIndex;

		public LinkedList<int> RealizedItems;

		private Rect ChildrenExtent = Rect.Empty;

		private Rect ComputedExtent = Rect.Empty;

		public bool ApplyTransform
		{
			get
			{
				return (bool)GetValue(ApplyTransformProperty);
			}
			set
			{
				SetValue(ApplyTransformProperty, value);
			}
		}

		public Rect ActualViewbox => (Rect)GetValue(ActualViewboxProperty);

		public Rect Viewbox
		{
			get
			{
				return (Rect)GetValue(ViewboxProperty);
			}
			set
			{
				SetValue(ViewboxProperty, value);
			}
		}

		public Stretch Stretch
		{
			get
			{
				return (Stretch)GetValue(StretchProperty);
			}
			set
			{
				SetValue(StretchProperty, value);
			}
		}

		public StretchDirection StretchDirection
		{
			get
			{
				return (StretchDirection)GetValue(StretchDirectionProperty);
			}
			set
			{
				SetValue(StretchDirectionProperty, value);
			}
		}
		/// <summary>
		/// 控件坐标的偏移
		/// </summary>
		public Point Offset
		{
			get
			{
				return (Point)GetValue(OffsetProperty);
			}
			set
			{
				SetValue(OffsetProperty, value);
			}
		}

		public double Scale
		{
			get
			{
				return (double)GetValue(ScaleProperty);
			}
			set
			{
				SetValue(ScaleProperty, value);
			}
		}

		public int RealizationLimit
		{
			get
			{
				return (int)GetValue(RealizationLimitProperty);
			}
			set
			{
				SetValue(RealizationLimitProperty, value);
			}
		}

		public int RealizationRate
		{
			get
			{
				return (int)GetValue(RealizationRateProperty);
			}
			set
			{
				SetValue(RealizationRateProperty, value);
			}
		}

		private ScaleTransform AppliedScaleTransform
		{
			get
			{
				if (ApplyTransform)
				{
					return (ScaleTransform)((TransformGroup)base.RenderTransform).Children[0];
				}
				return null;
			}
		}

		private TranslateTransform AppliedTranslateTransform
		{
			get
			{
				if (ApplyTransform)
				{
					return (TranslateTransform)((TransformGroup)base.RenderTransform).Children[1];
				}
				return null;
			}
		}

		public virtual Rect Extent
		{
			get
			{
				if (ComputedExtent.IsEmpty)
				{
					ComputedExtent = Rect.Union(ChildrenExtent, (SpatialIndex != null) ? SpatialIndex.Extent : Rect.Empty);
				}
				return ComputedExtent;
			}
		}

		public Point MousePosition
		{
			get
			{
				Point position = Mouse.GetPosition(this);
				if (ApplyTransform)
				{
					return position;
				}
				return GetCanvasPoint(position);
			}
		}

		ScrollViewer IScrollInfo.ScrollOwner
		{
			get;
			set;
		}

		bool IScrollInfo.CanHorizontallyScroll
		{
			get;
			set;
		}

		bool IScrollInfo.CanVerticallyScroll
		{
			get;
			set;
		}

		double IScrollInfo.ViewportHeight => ActualViewbox.Height * Scale;

		double IScrollInfo.ViewportWidth => ActualViewbox.Width * Scale;

		double IScrollInfo.ExtentHeight => Math.Max(Math.Max(ActualViewbox.Bottom, Extent.Bottom) - Math.Min(ActualViewbox.Top, Extent.Top), 0.0) * Scale;

		double IScrollInfo.ExtentWidth => Math.Max(Math.Max(ActualViewbox.Right, Extent.Right) - Math.Min(ActualViewbox.Left, Extent.Left), 0.0) * Scale;

		double IScrollInfo.HorizontalOffset => Math.Max(ActualViewbox.X - Extent.X, 0.0) * Scale;

		double IScrollInfo.VerticalOffset => Math.Max(ActualViewbox.Y - Extent.Y, 0.0) * Scale;

		public static event DependencyPropertyChangedEventHandler Refresh;
		/// <summary>
		/// 加入Transform(ScaleTransform、TranslateTransform)
		/// </summary>
		/// <param name="d"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static object CoerceRenderTransform(DependencyObject d, object value)
		{
			ZoomableCanvas zoomableCanvas = d as ZoomableCanvas;
			if (zoomableCanvas != null && zoomableCanvas.ApplyTransform)
			{
				TransformGroup transformGroup = new TransformGroup();
				transformGroup.Children.Add(new ScaleTransform());
				transformGroup.Children.Add(new TranslateTransform());
				return transformGroup;
			}
			return value;
		}

		private static void OnApplyTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(UIElement.RenderTransformProperty);
		}

		private static object CoerceActualViewbox(DependencyObject d, object value)
		{
			ZoomableCanvas zoomableCanvas = d as ZoomableCanvas;
			if (zoomableCanvas != null)
			{
				Point offset = zoomableCanvas.Offset;
				double scale = zoomableCanvas.Scale;
				Size renderSize = zoomableCanvas.RenderSize;
				value = new Rect(offset.X / scale, offset.Y / scale, renderSize.Width / scale, renderSize.Height / scale);
			}
			return value;
		}

		private static void OnActualViewboxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as ZoomableCanvas)?.InvalidateReality();
			IScrollInfo scrollInfo = d as IScrollInfo;
			if (scrollInfo != null && scrollInfo.ScrollOwner != null)
			{
				scrollInfo.ScrollOwner.InvalidateScrollInfo();
			}
		}

		private static bool IsViewboxValid(object value)
		{
			Rect rect = (Rect)value;
			if (!rect.IsEmpty)
			{
				if (rect.X.IsBetween(double.MinValue, double.MaxValue) && rect.Y.IsBetween(double.MinValue, double.MaxValue) && rect.Width.IsBetween(double.Epsilon, double.MaxValue))
				{
					return rect.Height.IsBetween(double.Epsilon, double.MaxValue);
				}
				return false;
			}
			return true;
		}

		private static void OnViewboxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(ScaleProperty);
			d.CoerceValue(OffsetProperty);
		}

		private static bool IsStretchValid(object value)
		{
			Stretch stretch = (Stretch)value;
			if (stretch != 0 && stretch != Stretch.Uniform)
			{
				return stretch == Stretch.UniformToFill;
			}
			return true;
		}

		private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(ScaleProperty);
			d.CoerceValue(OffsetProperty);
		}

		private static bool IsStretchDirectionValid(object value)
		{
			StretchDirection stretchDirection = (StretchDirection)value;
			if (stretchDirection != StretchDirection.Both && stretchDirection != 0)
			{
				return stretchDirection == StretchDirection.DownOnly;
			}
			return true;
		}

		private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(ScaleProperty);
			d.CoerceValue(OffsetProperty);
		}

		private static bool IsOffsetValid(object value)
		{
			Point point = (Point)value;
			if (point.X.IsBetween(double.MinValue, double.MaxValue))
			{
				return point.Y.IsBetween(double.MinValue, double.MaxValue);
			}
			return false;
		}

		private static object CoerceOffset(DependencyObject d, object value)
		{
			ZoomableCanvas zoomableCanvas = d as ZoomableCanvas;
			if (zoomableCanvas != null)
			{
				Rect viewbox = zoomableCanvas.Viewbox;
				if (!viewbox.IsEmpty)
				{
					double scale = zoomableCanvas.Scale;
					Size renderSize = zoomableCanvas.RenderSize;
					value = new Point((viewbox.X + viewbox.Width / 2.0) * scale - renderSize.Width / 2.0, (viewbox.Y + viewbox.Height / 2.0) * scale - renderSize.Height / 2.0);
				}
			}
			return value;
		}

		private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(ActualViewboxProperty);
			ZoomableCanvas zoomableCanvas = d as ZoomableCanvas;
			if (zoomableCanvas != null)
			{
				zoomableCanvas.OffsetOverride((Point)e.NewValue);
				if (ZoomableCanvas.Refresh != null)
				{
					ZoomableCanvas.Refresh(d, e);
				}
			}
		}

		private static bool IsScaleValid(object value)
		{
			return ((double)value).IsBetween(double.Epsilon, double.MaxValue);
		}

		private static object CoerceScale(DependencyObject d, object value)
		{
			double num = (double)value;
			ZoomableCanvas zoomableCanvas = d as ZoomableCanvas;
			if (zoomableCanvas != null)
			{
				Size renderSize = zoomableCanvas.RenderSize;
				if (renderSize.Width > 0.0 && renderSize.Height > 0.0)
				{
					Rect viewbox = zoomableCanvas.Viewbox;
					if (!viewbox.IsEmpty)
					{
						switch (zoomableCanvas.Stretch)
						{
							case Stretch.Uniform:
								num = Math.Min(renderSize.Width / viewbox.Width, renderSize.Height / viewbox.Height);
								break;
							case Stretch.UniformToFill:
								num = Math.Max(renderSize.Width / viewbox.Width, renderSize.Height / viewbox.Height);
								break;
						}
						switch (zoomableCanvas.StretchDirection)
						{
							case StretchDirection.DownOnly:
								num = num.AtMost((double)value);
								break;
							case StretchDirection.UpOnly:
								num = num.AtLeast((double)value);
								break;
						}
					}
				}
			}
			return num;
		}
		/// <summary>
		/// scale属性变动调用此方法
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(ActualViewboxProperty);
			d.CoerceValue(OffsetProperty);
			ZoomableCanvas zoomableCanvas = d as ZoomableCanvas;
			if (zoomableCanvas != null)
			{
				zoomableCanvas.ScaleOverride((double)e.NewValue);
				if (ZoomableCanvas.Refresh != null)
				{
					ZoomableCanvas.Refresh(d, e);
				}
			}
		}

		private static bool IsRealizationLimitValid(object value)
		{
			return (int)value >= 0;
		}

		private static void OnRealizationLimitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as ZoomableCanvas)?.InvalidateReality();
		}

		private static bool IsRealizationRateValid(object value)
		{
			return (int)value >= 0;
		}

		private static void OnRealizationRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as ZoomableCanvas)?.InvalidateReality();
		}

		static ZoomableCanvas()
		{
			ApplyTransformProperty = DependencyProperty.Register("ApplyTransform", typeof(bool), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange, OnApplyTransformChanged));
			ActualViewboxProperty = DependencyProperty.RegisterReadOnly("ActualViewbox", typeof(Rect), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(Rect.Empty, OnActualViewboxChanged, CoerceActualViewbox)).DependencyProperty;
			ViewboxProperty = DependencyProperty.Register("Viewbox", typeof(Rect), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnViewboxChanged), IsViewboxValid);
			StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(Stretch.Uniform, OnStretchChanged), IsStretchValid);
			StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(StretchDirection.Both, OnStretchDirectionChanged), IsStretchDirectionValid);

			OffsetProperty = DependencyProperty.Register("Offset", typeof(Point), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(new Point(0.0, 0.0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnOffsetChanged, CoerceOffset), IsOffsetValid);
			ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScaleChanged, CoerceScale), IsScaleValid);

			RealizationLimitProperty = DependencyProperty.Register("RealizationLimit", typeof(int), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(int.MaxValue, OnRealizationLimitChanged), IsRealizationLimitValid);
			RealizationRateProperty = DependencyProperty.Register("RealizationRate", typeof(int), typeof(ZoomableCanvas), new FrameworkPropertyMetadata(int.MaxValue, OnRealizationRateChanged), IsRealizationRateValid);
            RenderTransformProperty.OverrideMetadata(typeof(ZoomableCanvas), new FrameworkPropertyMetadata(null, CoerceRenderTransform));
			try
			{
				Canvas.TopProperty.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(OnPositioningChanged));
				Canvas.LeftProperty.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(OnPositioningChanged));
				Canvas.BottomProperty.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(OnPositioningChanged));
				Canvas.RightProperty.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(OnPositioningChanged));
			}
			catch (ArgumentException)
			{
			}
		}

		public ZoomableCanvas()
		{
			CoerceValue(ScaleProperty);
			CoerceValue(OffsetProperty);
			CoerceValue(ActualViewboxProperty);
			CoerceValue(UIElement.RenderTransformProperty);
		}

		protected override void OnIsVirtualizingChanged(bool oldIsVirtualizing, bool newIsVirtualizing)
		{
			OnItemsReset();
			base.OnIsVirtualizingChanged(oldIsVirtualizing, newIsVirtualizing);
		}

		protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
		{
			OnItemsReset();
			base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
		}

		protected override void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				OnItemsAdded(args.NewStartingIndex, args.NewItems);
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				OnItemsRemoved(args.OldStartingIndex, args.OldItems);
			}
			else if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				OnItemsReset();
			}
			InvalidateReality();
			InvalidateExtent();
		}

		private void OnItemsAdded(int index, IList items)
		{
			if (PrivateIndex != null)
			{
				PrivateIndex.InsertRange(index, items.Count);
			}
			if (RealizedItems == null)
			{
				return;
			}
			for (LinkedListNode<int> linkedListNode = RealizedItems.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value >= index)
				{
					linkedListNode.Value += items.Count;
				}
			}
		}

		private void OnItemsRemoved(int index, IList items)
		{
			if (PrivateIndex != null)
			{
				PrivateIndex.RemoveRange(index, items.Count);
			}
			if (RealizedItems == null)
			{
				return;
			}
			LinkedListNode<int> linkedListNode = RealizedItems.First;
			while (linkedListNode != null)
			{
				LinkedListNode<int> next = linkedListNode.Next;
				if (linkedListNode.Value >= index)
				{
					if (linkedListNode.Value < index + items.Count)
					{
						RealizedItems.Remove(linkedListNode);
					}
					else
					{
						linkedListNode.Value -= items.Count;
					}
				}
				linkedListNode = next;
			}
		}

		public void OnItemsReset()
		{
			if (SpatialIndex != null)
			{
				SpatialIndex.ExtentChanged -= OnSpatialExtentChanged;
				SpatialIndex.QueryInvalidated -= OnSpatialQueryInvalidated;
			}
			RealizedItems = null;
			SpatialIndex = null;
			PrivateIndex = null;
			if (base.IsVirtualizing && base.IsItemsHost && base.ItemsOwner != null)
			{
				RealizedItems = new LinkedList<int>();
				SpatialIndex = (base.ItemsOwner.ItemsSource as ISpatialItemsSource);
				if (SpatialIndex == null)
				{
					PrivateIndex = new PrivateSpatialIndex();
					PrivateIndex.Reset((base.ItemsOwner.Items != null) ? base.ItemsOwner.Items.Count : 0);
					SpatialIndex = PrivateIndex;
				}
				SpatialIndex.ExtentChanged += OnSpatialExtentChanged;
				SpatialIndex.QueryInvalidated += OnSpatialQueryInvalidated;
			}
			InvalidateReality();
			InvalidateExtent();
		}

		public void OnItemsReset2()
		{
			if (SpatialIndex != null)
			{
				SpatialIndex.ExtentChanged -= OnSpatialExtentChanged;
				SpatialIndex.QueryInvalidated -= OnSpatialQueryInvalidated;
			}
			RealizedItems = null;
			SpatialIndex = null;
			PrivateIndex = null;
			if (base.IsVirtualizing && base.IsItemsHost && base.ItemsOwner != null)
			{
				RealizedItems = new LinkedList<int>();
				SpatialIndex = (base.ItemsOwner.ItemsSource as ISpatialItemsSource);
				if (SpatialIndex == null)
				{
					PrivateIndex = new PrivateSpatialIndex();
					PrivateIndex.Reset((base.ItemsOwner.Items != null) ? base.ItemsOwner.Items.Count : 0);
					SpatialIndex = PrivateIndex;
				}
				SpatialIndex.ExtentChanged += OnSpatialExtentChanged;
				SpatialIndex.QueryInvalidated += OnSpatialQueryInvalidated;
			}
			InvalidateReality();
			InvalidateExtent();
		}

		private void OnSpatialQueryInvalidated(object sender, EventArgs e)
		{
			InvalidateReality();
		}

		private void OnSpatialExtentChanged(object sender, EventArgs e)
		{
			InvalidateExtent();
		}

		protected override object RealizeOverride(IEnumerable items, object state)
		{
			IEnumerator enumerator = (state as IEnumerator) ?? RealizeOverride();
			int realizationRate = RealizationRate;
			while (realizationRate-- > 0)
			{
				if (!enumerator.MoveNext())
				{
					return null;
				}
			}
			return enumerator;
		}

		public IEnumerator RealizeOverride()
		{
			if (SpatialIndex == null)
			{
				yield break;
			}
			if (PrivateIndex != null)
			{
				PrivateIndex.Optimize();
			}
			IEnumerable<int> query;
			if (base.IsVirtualizing)
			{
				Rect actualViewbox = ActualViewbox;
				int realizationLimit = RealizationLimit;
				actualViewbox.Inflate(actualViewbox.Width / 10.0, actualViewbox.Height / 10.0);
				query = SpatialIndex.Query(actualViewbox).Take(realizationLimit);
			}
			else
			{
				query = SpatialIndex.Query(new Rect(double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity));
			}
			LinkedListNode<int> lastNode = null;
			LinkedListNode<int> nextNode2 = RealizedItems.First;
			foreach (int index2 in query)
			{
				LinkedListNode<int> node2 = RealizedItems.FindNext(lastNode, index2);
				if (node2 == null || node2 != nextNode2)
				{
					if (node2 != null)
					{
						RealizedItems.Remove(node2);
					}
					else
					{
						RealizeItem(index2);
						node2 = new LinkedListNode<int>(index2);
					}
					if (lastNode == null)
					{
						RealizedItems.AddFirst(node2);
					}
					else
					{
						RealizedItems.AddAfter(lastNode, node2);
					}
				}
				lastNode = node2;
				nextNode2 = node2.Next;
				yield return index2;
			}
			nextNode2 = RealizedItems.Last;
			while (nextNode2 != lastNode)
			{
				LinkedListNode<int> node = nextNode2;
				nextNode2 = nextNode2.Previous;
				int index = node.Value;
				UIElement container = ContainerFromIndex(index);
				if (container == null || (!container.IsMouseCaptureWithin && !container.IsKeyboardFocusWithin))
				{
					VirtualizeItem(index);
					RealizedItems.Remove(node);
				}
				yield return index;
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			CoerceValue(ScaleProperty);
			CoerceValue(OffsetProperty);
			CoerceValue(ActualViewboxProperty);
			base.OnRenderSizeChanged(sizeInfo);
		}

		private static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(d);
			ZoomableCanvas zoomableCanvas = parent as ZoomableCanvas;
			if (zoomableCanvas != null)
			{
				zoomableCanvas.InvalidateArrange();
			}
			else
			{
				(parent as Canvas)?.InvalidateArrange();
			}
		}

		protected virtual void ScaleOverride(double scale)
		{
			ScaleTransform appliedScaleTransform = AppliedScaleTransform;
			if (appliedScaleTransform != null)
			{
				appliedScaleTransform.ScaleX = scale;
				appliedScaleTransform.ScaleY = scale;
			}
			else
			{
				InvalidateArrange();
			}
		}

		protected virtual void OffsetOverride(Point offset)
		{
			TranslateTransform appliedTranslateTransform = AppliedTranslateTransform;
			if (appliedTranslateTransform != null)
			{
				appliedTranslateTransform.X = 0.0 - offset.X;
				appliedTranslateTransform.Y = 0.0 - offset.Y;
			}
			else
			{
				InvalidateArrange();
			}
		}
		/// <summary>
		/// 在派生类中重写时，测量子元素在布局中所需的大小，并确定由 FrameworkElement 派生的类的大小。
		/// </summary>
		/// <param name="availableSize">此元素可提供给子元素的可用大小。 可指定无穷大作为一个值，该值指示元素将调整到适应内容的大小。</param>
		/// <returns>此元素基于其对子元素大小的计算确定它在布局期间所需要的大小。</returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			Size availableSize2 = new Size(double.PositiveInfinity, double.PositiveInfinity);
			foreach (UIElement internalChild in base.InternalChildren)
			{
				if (internalChild != null)
				{
					try
					{
						internalChild.Measure(availableSize2);
					}
					catch
					{
					}
				}
			}
			return default(Size);
		}
		/// <summary>
		/// 在派生类中重写时，为 FrameworkElement 派生类定位子元素并确定大小。
		/// </summary>
		/// <param name="finalSize">父级中应使用此元素排列自身及其子元素的最终区域。</param>
		/// <returns>使用的实际大小。</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			bool applyTransform = ApplyTransform;
			Point point = applyTransform ? default(Point) : Offset;
			double num = applyTransform ? 1.0 : Scale;
			ChildrenExtent = Rect.Empty;
			foreach (UIElement internalChild in base.InternalChildren)
			{
				if (internalChild != null)
				{
					Rect rect = new Rect(Canvas.GetLeft(internalChild).GetValueOrDefault(), Canvas.GetTop(internalChild).GetValueOrDefault(), internalChild.DesiredSize.Width / num, internalChild.DesiredSize.Height / num);
					if (PrivateIndex != null)
					{
						int index = IndexFromContainer(internalChild);
						Rect rect2 = PrivateIndex[index];
						if (Math.Abs(rect2.Top - rect.Top) > 0.001 || Math.Abs(rect2.Left - rect.Left) > 0.001 || Math.Abs(rect2.Width - rect.Width) > 0.001 || Math.Abs(rect2.Height - rect.Height) > 0.001)
						{
							PrivateIndex[index] = rect;
						}
					}
					ChildrenExtent.Union(rect);
					rect.X *= num;
					rect.X -= point.X;
					rect.Y *= num;
					rect.Y -= point.Y;
					rect.Width *= num;
					rect.Height *= num;
					rect.X = rect.X.AtLeast(-1.7014117331926443E+38);
					rect.Y = rect.Y.AtLeast(-1.7014117331926443E+38);
					rect.Width = rect.Width.AtMost(3.4028234663852886E+38);
					rect.Height = rect.Height.AtMost(3.4028234663852886E+38);
					internalChild.Arrange(rect);
				}
			}
			InvalidateExtent();
			return finalSize;
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			if (!base.ClipToBounds)
			{
				return null;
			}
			return new RectangleGeometry(new Rect(base.RenderSize));
		}

		protected void InvalidateExtent()
		{
			ComputedExtent = Rect.Empty;
			((IScrollInfo)this).ScrollOwner?.InvalidateScrollInfo();
		}
		/// <summary>
		/// 图像坐标 转 控件坐标
		/// </summary>
		/// <param name="canvasPoint">图像坐标</param>
		/// <returns>控件坐标</returns>
		public Point GetVisualPoint(Point canvasPoint)
		{
			return (Point)((Vector)canvasPoint * Scale - (Vector)Offset);
		}
		/// <summary>
		/// 控件坐标 转 图像坐标
		/// </summary>
		/// <param name="screenPoint">控件坐标</param>
		/// <returns>图像坐标</returns>
		public Point GetCanvasPoint(Point screenPoint)
		{
			return (Point)(((Vector)Offset + (Vector)screenPoint) / Scale);
		}

		void IScrollInfo.LineDown()
		{
			((IScrollInfo)this).SetVerticalOffset(((IScrollInfo)this).VerticalOffset + 16.0);
		}

		void IScrollInfo.LineLeft()
		{
			((IScrollInfo)this).SetHorizontalOffset(((IScrollInfo)this).HorizontalOffset - 16.0);
		}

		void IScrollInfo.LineRight()
		{
			((IScrollInfo)this).SetHorizontalOffset(((IScrollInfo)this).HorizontalOffset + 16.0);
		}

		void IScrollInfo.LineUp()
		{
			((IScrollInfo)this).SetVerticalOffset(((IScrollInfo)this).VerticalOffset - 16.0);
		}

		void IScrollInfo.MouseWheelDown()
		{
			((IScrollInfo)this).SetVerticalOffset(((IScrollInfo)this).VerticalOffset + 48.0);
		}

		void IScrollInfo.MouseWheelLeft()
		{
			((IScrollInfo)this).SetHorizontalOffset(((IScrollInfo)this).HorizontalOffset - 48.0);
		}

		void IScrollInfo.MouseWheelRight()
		{
			((IScrollInfo)this).SetHorizontalOffset(((IScrollInfo)this).HorizontalOffset + 48.0);
		}

		void IScrollInfo.MouseWheelUp()
		{
			((IScrollInfo)this).SetVerticalOffset(((IScrollInfo)this).VerticalOffset - 48.0);
		}

		void IScrollInfo.PageDown()
		{
			((IScrollInfo)this).SetVerticalOffset(((IScrollInfo)this).VerticalOffset + ((IScrollInfo)this).ViewportHeight);
		}

		void IScrollInfo.PageLeft()
		{
			((IScrollInfo)this).SetHorizontalOffset(((IScrollInfo)this).HorizontalOffset - ((IScrollInfo)this).ViewportWidth);
		}

		void IScrollInfo.PageRight()
		{
			((IScrollInfo)this).SetHorizontalOffset(((IScrollInfo)this).HorizontalOffset + ((IScrollInfo)this).ViewportWidth);
		}

		void IScrollInfo.PageUp()
		{
			((IScrollInfo)this).SetVerticalOffset(((IScrollInfo)this).VerticalOffset - ((IScrollInfo)this).ViewportHeight);
		}

		void IScrollInfo.SetHorizontalOffset(double offset)
		{
			offset = Math.Max(Math.Min(offset, ((IScrollInfo)this).ExtentWidth - ((IScrollInfo)this).ViewportWidth), 0.0);
			Rect viewbox = Viewbox;
			if (viewbox.IsEmpty)
			{
				Offset = new Point(Offset.X + offset - ((IScrollInfo)this).HorizontalOffset, Offset.Y);
				return;
			}
			viewbox.X += (offset - ((IScrollInfo)this).HorizontalOffset) / Scale;
			Viewbox = viewbox;
		}

		void IScrollInfo.SetVerticalOffset(double offset)
		{
			offset = Math.Max(Math.Min(offset, ((IScrollInfo)this).ExtentHeight - ((IScrollInfo)this).ViewportHeight), 0.0);
			Rect viewbox = Viewbox;
			if (viewbox.IsEmpty)
			{
				Offset = new Point(Offset.X, Offset.Y + offset - ((IScrollInfo)this).VerticalOffset);
				return;
			}
			viewbox.Y += (offset - ((IScrollInfo)this).VerticalOffset) / Scale;
			Viewbox = viewbox;
		}

		Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
		{
			if (rectangle.IsEmpty || visual == null || !IsAncestorOf(visual))
			{
				return Rect.Empty;
			}
			rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
			rectangle = base.RenderTransform.TransformBounds(rectangle);
			double viewportWidth = ((IScrollInfo)this).ViewportWidth;
			double viewportHeight = ((IScrollInfo)this).ViewportHeight;
			double num = 0.0 - rectangle.X;
			double num2 = num + viewportWidth - rectangle.Width;
			double num3 = 0.0 - rectangle.Y;
			double num4 = num3 + viewportHeight - rectangle.Height;
			double num5 = (num > 0.0 && num2 > 0.0) ? Math.Min(num, num2) : ((num < 0.0 && num2 < 0.0) ? Math.Max(num, num2) : 0.0);
			double num6 = (num3 > 0.0 && num4 > 0.0) ? Math.Min(num3, num4) : ((num3 < 0.0 && num4 < 0.0) ? Math.Max(num3, num4) : 0.0);
			Point offset = Offset;
			offset.X -= num5;
			offset.Y -= num6;
			Offset = offset;
			rectangle.X += num5;
			rectangle.Y += num6;
			rectangle.Intersect(new Rect(0.0, 0.0, viewportWidth, viewportHeight));
			return rectangle;
		}
	}
}
