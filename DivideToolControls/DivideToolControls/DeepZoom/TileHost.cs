using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DivideToolControls.DeepZoom
{
    public class TileHost : FrameworkElement
    {
        private DrawingVisual _visual;

        private static readonly AnimationTimeline _opacityAnimation = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(500.0))
        {
            EasingFunction = new ExponentialEase()
        };

        public static readonly DependencyProperty xLeftProperty = DependencyProperty.Register("xLeft", typeof(double), typeof(TileHost));

        public static readonly DependencyProperty xTopProperty = DependencyProperty.Register("xTop", typeof(double), typeof(TileHost));

        public static readonly DependencyProperty xIndexProperty = DependencyProperty.Register("xIndex", typeof(int), typeof(TileHost));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(TileHost), new FrameworkPropertyMetadata(null, RefreshTile));

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(TileHost), new FrameworkPropertyMetadata(1.0, RefreshTile));

        public double xLeft
        {
            get
            {
                return (double)GetValue(xLeftProperty);
            }
            set
            {
                SetValue(xLeftProperty, value);
            }
        }

        public double xTop
        {
            get
            {
                return (double)GetValue(xTopProperty);
            }
            set
            {
                SetValue(xTopProperty, value);
            }
        }

        public int xIndex
        {
            get
            {
                return (int)GetValue(xIndexProperty);
            }
            set
            {
                SetValue(xIndexProperty, value);
            }
        }

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
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

        protected override int VisualChildrenCount
        {
            get
            {
                if (_visual != null)
                {
                    return 1;
                }
                return 0;
            }
        }

        public TileHost()
        {
            base.IsHitTestVisible = false;
        }

        public TileHost(ImageSource source, double scale)
            : this()
        {
            Source = source;
            Scale = scale;
        }

        private static void RefreshTile(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TileHost tileHost = d as TileHost;
            if (tileHost != null && tileHost.Source != null && tileHost.Scale > 0.0)
            {
                tileHost.RenderTile();
            }
        }

        public void RenderTile()
        {
            _visual = new DrawingVisual();
            base.Width = Source.Width * Scale;
            base.Height = Source.Height * Scale;
            DrawingContext drawingContext = _visual.RenderOpen();
            drawingContext.DrawImage(Source, new Rect(0.0, 0.0, base.Width, base.Height));
            drawingContext.Close();
            base.CacheMode = new BitmapCache(1.0 / Scale);
            Console.WriteLine("Width:" + base.Width + "Height:" + base.Height);
            base.Opacity = Setting.Opacity;
            BeginAnimation(UIElement.OpacityProperty, _opacityAnimation);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visual;
        }
    }
}
