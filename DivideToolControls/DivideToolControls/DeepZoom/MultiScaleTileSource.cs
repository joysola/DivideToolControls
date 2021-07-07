using DivideToolControls.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace DivideToolControls.DeepZoom
{
    [TypeConverter(typeof(DeepZoomImageTileSourceConverter))]
    public abstract class MultiScaleTileSource : DependencyObject
    {
        public double CenterX;

        public double CenterY;

        public double Angle;

        public double OffsetX;

        public double OffsetY;
        /// <summary>
        /// 最大层数
        /// </summary>
        private int _maxLevel;
        /// <summary>
        /// 最大层数（移出时可能小一点）
        /// </summary>

        private int _zoomLimitLevel;

        private readonly IList<int> _levelOffsets = new List<int>();

        private readonly IList<int> _rowCounts = new List<int>();

        private readonly IList<int> _columnCounts = new List<int>();

        private readonly IList<double> _levelScales = new List<double>();
        /// <summary>
        /// 总的图像大小
        /// </summary>
        public Size ImageSize
        {
            get;
            set;
        }

        protected internal int TileSize
        {
            get;
            set;
        }

        protected internal int TileOverlap
        {
            get;
            set;
        }

        internal MultiScaleTileSource()
        {
        }

        public MultiScaleTileSource(long imageWidth, long imageHeight, int tileSize, int tileOverlap)
        {
            ImageSize = new Size(imageWidth, imageHeight);
            TileSize = tileSize;
            TileOverlap = tileOverlap;
            InitializeTileSource();
        }
        /// <summary>
        /// 初始化瓦片元数据
        /// </summary>
        internal void InitializeTileSource()
        {
            _maxLevel = GetMaximumLevel(ImageSize.Width, ImageSize.Height);
            _zoomLimitLevel = _maxLevel;
            CalculateLevelOffsets();
        }

        public MultiScaleTileSource(int imageWidth, int imageHeight, int tileSize, int tileOverlap)
            : this((long)imageWidth, (long)imageHeight, tileSize, tileOverlap)
        {
        }

        protected internal abstract object GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY);

        public abstract void GetTileLayersAngle(ref double CenterX, ref double CenterY, ref double Angle, ref double OffsetX, ref double OffsetY);

        internal IEnumerable<Tile> VisibleTilesUntilFill(Rect rectangle, int startingLevel)
        {
            IEnumerable<Tile> enumerable = Enumerable.Empty<Tile>();
            IEnumerable<int> source = Enumerable.Range(0, startingLevel + 1);
            return source.SelectMany(delegate (int level)
            {
                double num = ScaleAtLevel(level);
                Rect rectangle2 = new Rect(rectangle.X * num, rectangle.Y * num, rectangle.Width * num, rectangle.Height * num);
                return VisibleTiles(rectangle2, level);
            });
        }

        internal Point GetTilePosition(int column, int row)
        {
            int num = (column != 0) ? TileOverlap : 0;
            int num2 = (row != 0) ? TileOverlap : 0;
            return new Point(column * TileSize - num, row * TileSize - num2);
        }
        /// <summary>
        /// 根据 缩放倍率 获取瓦片图所在的层数
        /// </summary>
        /// <param name="scaleRatio">缩放倍率</param>
        /// <returns>瓦片图层数</returns>
        public int GetLevel(double scaleRatio)
        {
            int value = _maxLevel + (int)Math.Log(scaleRatio, 2.0);
            return value.Clamp(0, _zoomLimitLevel);
        }
        /// <summary>
        /// 根据控件尺寸确定所在层数
        /// </summary>
        /// <param name="viewportWidth"></param>
        /// <param name="viewportHeight"></param>
        /// <returns></returns>
        internal int GetLevel(double viewportWidth, double viewportHeight)
        {
            double num = ImageSize.Width / ImageSize.Height; // 图像长宽比
            double num2 = viewportWidth / viewportHeight; // 图像所在控件长宽比
            int i = 0;
            if (num2 > num)
            {
                for (; ImageSizeAtLevel(i).Height < viewportHeight && i < _zoomLimitLevel; i++) // 取最接近图像高度的，所在层数高度
                {
                }
            }
            else
            {
                for (; ImageSizeAtLevel(i).Width < viewportWidth && i < _zoomLimitLevel; i++)
                {
                }
            }
            return i;
        }
        /// <summary>
        /// 获取层的缩放倍率
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal double ScaleAtLevel(int level)
        {
            if (_levelScales.Count > level)
            {
                return _levelScales[level];
            }
            return Math.Pow(0.5, _maxLevel - level);
        }
        /// <summary>
        /// 获取指定层下的图像尺寸
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal Size ImageSizeAtLevel(int level)
        {
            double num = ScaleAtLevel(level);
            return new Size(Math.Ceiling(ImageSize.Width * num), Math.Ceiling(ImageSize.Height * num));
        }

        internal int LevelOffset(int level)
        {
            return _levelOffsets[level];
        }

        internal int LevelFromOffset(long tileId)
        {
            int num = _levelOffsets.Count - 1;
            while (num > 0 && _levelOffsets[num] > tileId)
            {
                num--;
            }
            return num;
        }

        internal int ColumnsAtLevel(int level)
        {
            if (_columnCounts.Count > level)
            {
                return _columnCounts[level];
            }
            return (int)Math.Ceiling(ImageSizeAtLevel(level).Width / TileSize); // 指定层 除以瓦片尺寸，得出需要的个数
        }

        internal int RowsAtLevel(int level)
        {
            if (_rowCounts.Count > level)
            {
                return _rowCounts[level];
            }
            return (int)Math.Ceiling(ImageSizeAtLevel(level).Height / TileSize); // 指定层 除以瓦片尺寸，得出需要的个数
        }

        internal int GetTileIndex(Tile tile)
        {
            int num = RowsAtLevel(tile.Level);
            int num2 = ColumnsAtLevel(tile.Level);
            int num3 = LevelOffset(tile.Level);
            if (num2 > num)
            {
                return num3 + num2 * tile.Row + tile.Column;
            }
            return num3 + num * tile.Column + tile.Row;
        }

        internal Tile TileFromIndex(int index)
        {
            int level = LevelFromOffset(index);
            int num = LevelOffset(level);
            int num2 = RowsAtLevel(level);
            int num3 = ColumnsAtLevel(level);
            index -= num;
            int num4 = 0;
            int num5 = 0;
            if (num3 > num2)
            {
                num4 = index / num3;
                num5 = index - num4 * num3;
            }
            else
            {
                num5 = index / num2;
                num4 = index - num5 * num2;
            }
            return new Tile(level, num5, num4);
        }
        /// <summary>
        /// 计算每层的行列、偏移、缩放倍率等
        /// </summary>
        protected void CalculateLevelOffsets()
        {
            int num = 0;
            for (int i = 0; i <= _maxLevel; i++)
            {
                _levelOffsets.Add(num);
                _levelScales.Add(ScaleAtLevel(i));
                _rowCounts.Add(RowsAtLevel(i));
                _columnCounts.Add(ColumnsAtLevel(i));
                try
                {
                    num = checked(num + TilesAtLevel(i));
                }
                catch (OverflowException)
                {
                    _zoomLimitLevel = i - 1;
                    return;
                }
            }
        }

        protected static int GetMaximumLevel(double width, double height)
        {
            return (int)Math.Ceiling(Math.Log(Math.Max(width, height), 2.0)); // log2的结果
        }

        protected int TilesAtLevel(int level)
        {
            return checked(ColumnsAtLevel(level) * RowsAtLevel(level));
        }

        private IEnumerable<Tile> VisibleTiles(Rect rectangle, int level)
        {
            rectangle.Intersect(new Rect(ImageSize));
            double top = Math.Floor(rectangle.Top / (double)TileSize);
            double left = Math.Floor(rectangle.Left / (double)TileSize);
            double right2 = Math.Ceiling(rectangle.Right / (double)TileSize);
            double bottom2 = Math.Ceiling(rectangle.Bottom / (double)TileSize);
            right2 = right2.AtMost(ColumnsAtLevel(level));
            bottom2 = bottom2.AtMost(RowsAtLevel(level));
            double width = (right2 - left).AtLeast(0.0);
            double height = (bottom2 - top).AtLeast(0.0);
            if (top == 0.0 && left == 0.0 && width == 1.0 && height == 1.0)
            {
                yield return new Tile(level, 0, 0);
            }
            else
            {
                foreach (Point pt in Quadivide(new Rect(left, top, width, height)))
                {
                    Point point = pt;
                    int column = (int)point.X;
                    Point point2 = pt;
                    yield return new Tile(level, column, (int)point2.Y);
                }
            }
        }

        private static IEnumerable<Point> Quadivide(Rect area)
        {
            if (!(area.Width > 0.0) || !(area.Height > 0.0))
            {
                yield break;
            }
            Point center = area.GetCenter();
            double x = Math.Floor(center.X);
            double y = Math.Floor(center.Y);
            yield return new Point(x, y);
            Rect quad5 = new Rect(area.TopLeft, new Point(x, y + 1.0));
            Rect quad4 = new Rect(area.TopRight, new Point(x, y));
            Rect quad3 = new Rect(area.BottomLeft, new Point(x + 1.0, y + 1.0));
            Rect quad2 = new Rect(area.BottomRight, new Point(x + 1.0, y));
            Queue<IEnumerator<Point>> quads = new Queue<IEnumerator<Point>>();
            quads.Enqueue(Quadivide(quad5).GetEnumerator());
            quads.Enqueue(Quadivide(quad4).GetEnumerator());
            quads.Enqueue(Quadivide(quad3).GetEnumerator());
            quads.Enqueue(Quadivide(quad2).GetEnumerator());
            while (quads.Count > 0)
            {
                IEnumerator<Point> quad = quads.Dequeue();
                if (quad.MoveNext())
                {
                    yield return quad.Current;
                    quads.Enqueue(quad);
                }
            }
        }
    }
}
