using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    public class Position
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public bool IsOccupied { get; set; }

        public Position(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            IsOccupied = false;
        }
    }
}
