using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.DeepZoom
{
    internal struct Tile
    {
        public int Level
        {
            get;
            set;
        }

        public int Row
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }

        public Tile(int level, int column, int row)
        {
            this = default(Tile);
            Level = level;
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return Level.ToString() + "_" + Row.ToString() + "_" + Column.ToString();
        }
    }
}
