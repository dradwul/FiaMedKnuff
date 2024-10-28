using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace FiaMedKnuff
{
    public class GamePiece
    {
        public int Id { get; set; }
        public Ellipse GamePieceShape { get; set; }
        public int StepsTaken { get; set; } = 0;
        public Position Position { get; set; }
        public Position StartPosition { get; set; }

        public GamePiece(int id, Ellipse gamePieceShape, Position position)
        {
            Id = id;
            GamePieceShape = gamePieceShape;
            Position = position;
            StartPosition = position; //Set the starting position during initialization
        }

        public GamePiece()
        {
        }

        public Ellipse CreateNew(Windows.UI.Color pieceColor)
        {
            Ellipse newPiece = new Ellipse
            {
                Fill = new SolidColorBrush(pieceColor),
                Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                StrokeThickness = 2,
                Width = 40,
                Height = 40,
                IsTapEnabled = false
            };

            return newPiece;
        }
    }
}