using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
        /// Method to handle knocking pieces off and updating the UI with straight-line animation.
        /// </summary>
        public async Task<bool> KnockOffPiece(Grid gameGrid)
        {
            if (IsOccupied && OccupyingPiece != null)
            {
                GamePiece knockedPiece = OccupyingPiece;

                // Move piece with animation to it's nest position
                Position nestPosition = knockedPiece.StartPosition;

                await AnimateStraightLine(knockedPiece, nestPosition, gameGrid);

                // Update position after animation
                knockedPiece.Position = nestPosition;
                knockedPiece.StepsTaken = 0;

                IsOccupied = false;
                OccupyingPiece = null;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Animates the piece in a straight line from its current position back to its nest.
        /// </summary>
        private async Task AnimateStraightLine(GamePiece piece, Position nestPosition, Grid gameGrid)
        {
            TranslateTransform translateTransform = new TranslateTransform();
            piece.GamePieceShape.RenderTransform = translateTransform;

            // Get size of a grid cell to calculate pixel values
            double cellWidth = gameGrid.ActualWidth / gameGrid.ColumnDefinitions.Count;
            double cellHeight = gameGrid.ActualHeight / gameGrid.RowDefinitions.Count;

            // Calculate start and end pixel positions
            double startX = piece.Position.ColumnIndex * cellWidth;
            double startY = piece.Position.RowIndex * cellHeight;
            double endX = nestPosition.ColumnIndex * cellWidth;
            double endY = nestPosition.RowIndex * cellHeight;

            // Calculate move distance
            double deltaX = endX - startX;
            double deltaY = endY - startY;

            // Duration of animation
            double duration = 400; // 400 ms
            double steps = 10; // Number of steps for a smooth animation
            double stepDuration = duration / steps;

            // Create animation in steps
            for (int step = 0; step < steps; step++)
            {
                // Progression
                double linearProgress = (double)step / steps;
                double easedProgress = EaseInOutQuad(linearProgress); // Använd easing för smidigare animation

                // Update TranslateTransform for each step
                translateTransform.X = deltaX * easedProgress;
                translateTransform.Y = deltaY * easedProgress;

                await Task.Delay((int)stepDuration);
            }

            // Update position to nest and reset transform after animation 
            Grid.SetRow(piece.GamePieceShape, nestPosition.RowIndex);
            Grid.SetColumn(piece.GamePieceShape, nestPosition.ColumnIndex);

            translateTransform.X = 0;
            translateTransform.Y = 0;
        }

        /// <summary>
        /// Easing for a smoother animation
        /// </summary>
        private double EaseInOutQuad(double progress)
        {
            return progress < 0.5 ? 2 * progress * progress : 1 - Math.Pow(-2 * progress + 2, 2) / 2;
        }
    }
}