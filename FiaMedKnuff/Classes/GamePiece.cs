using System;
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

        public Ellipse ReturnGamePieceShape(int id, GamePiece[] pieces)
        {
            foreach (GamePiece piece in pieces)
            {
                if (piece.Id == id)
                    return piece.GamePieceShape;
            }
            throw new ArgumentException("Game piece shape not found");
        }

        public void EnableGamePieces(int diceRoll, GamePiece[] pieces)
        {
            foreach (GamePiece piece in pieces)
            {
                //Enable piece in nest if dice roll is 1 or 6
                if ((diceRoll == 1 || diceRoll == 6) && piece.StepsTaken == 0)
                {
                    piece.GamePieceShape.IsTapEnabled = true;
                    piece.GamePieceShape.StrokeThickness = 4;
                }
                //Enable piece if it has a legal move
                else if (piece.StepsTaken + diceRoll <= 45 && piece.StepsTaken != 0 && piece.StepsTaken != 45)
                {
                    piece.GamePieceShape.IsTapEnabled = true;
                    piece.GamePieceShape.StrokeThickness = 4;
                }
            }
        }

        public void DisableGamePieces(GamePiece[] pieces)
        {
            foreach (GamePiece piece in pieces)
            {
                piece.GamePieceShape.IsTapEnabled = false;
                piece.GamePieceShape.StrokeThickness = 2;
            }
        }
    }
}