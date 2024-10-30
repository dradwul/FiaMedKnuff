using System;
using System.Linq;
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
        public GamePiece OccupyingPiece { get; set; }

        public Position(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            IsOccupied = false;
            OccupyingPiece = null;
        }

        public Position() { }

        /// <summary>
        /// All outer positions that are neutral and open for every player. 
        /// Got the property IsOccupied to mark it as taken by a game piece. 
        /// The first element in the array is the same as Player1 starting zone. Then it follow 2,3,4,5... clockwise.
        /// </summary>
        private static Position[] allOuterPositions = new Position[]
        {
            new Position(4,1),
            new Position(4,2),
            new Position(4,3),
            new Position(4,4),
            new Position(4,5),
            new Position(3,5),
            new Position(2,5),
            new Position(1,5),
            new Position(0,5),
            new Position(0,6),
            new Position(0,7),
            new Position(1,7),
            new Position(2,7),
            new Position(3,7),
            new Position(4,7),
            new Position(4,8),
            new Position(4,9),
            new Position(4,10),
            new Position(4,11),
            new Position(5,11),
            new Position(6,11),
            new Position(6,10),
            new Position(6,9),
            new Position(6,8),
            new Position(6,7),
            new Position(7,7),
            new Position(8,7),
            new Position(9,7),
            new Position(10,7),
            new Position(10,6),
            new Position(10,5),
            new Position(9,5),
            new Position(8,5),
            new Position(7,5),
            new Position(6,5),
            new Position(6,4),
            new Position(6,3),
            new Position(6,2),
            new Position(6,1),
            new Position(5,1)
        };

        public static Position[] GetAllOuterPositions()
        {
            return allOuterPositions.ToArray();
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