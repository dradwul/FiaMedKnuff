using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuff
{
    public class Position
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public bool IsOccupied { get; set; }
        /// <summary>
        /// New property to store piece in occupying position
        /// </summary>
        public GamePiece OccupyingPiece { get; set; }
    

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
            OccupyingPiece = null;
        }
        /// <summary>
        /// Method to to handle knocking pieces off and updating the UI
        /// </summary>
        /// <returns></returns>
        public bool KnockOffPiece()
        {
            if (IsOccupied && OccupyingPiece != null)
            {
                GamePiece knockedPiece = OccupyingPiece;

                knockedPiece.Position = knockedPiece.StartPosition;
                knockedPiece.StepsTaken = 0;

                IsOccupied = false;
                OccupyingPiece = null;

                Grid.SetRow(knockedPiece.GamePieceShape, knockedPiece.StartPosition.RowIndex);
                Grid.SetColumn(knockedPiece.GamePieceShape, knockedPiece.StartPosition.ColumnIndex);
            }
            return false;
        }

    }
}