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
        /// <summary>
        /// Constructor for Position with 3 parameters to set the row index, column index and if the position is occupied
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        public Position(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            IsOccupied = false;
        }
    }
}
