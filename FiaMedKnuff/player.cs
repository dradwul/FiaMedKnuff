using System.Collections.Generic;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Media;

public class Player
{
    public int PlayerID { get; private set; }
    public string Color { get; private set; }
    public List<Ellipse> Pieces { get; private set; }
    private Position startingPosition;

    public Player(int playerId, string color, Position startingPosition)
    {
        PlayerID = playerId;
        Color = color;
        this.startingPosition = startingPosition;
        Pieces = new List<Ellipse>();

        // Initialize pieces (4 pieces per player)
        for (int i = 0; i < 4; i++)
        {
            Ellipse piece = new Ellipse
            {
                Fill = new SolidColorBrush(Windows.UI.Colors.Black), // Change to player color
                Stroke = new SolidColorBrush(Windows.UI.Colors.White),
                StrokeThickness = 3,
                Width = 40,
                Height = 40
            };
            Pieces.Add(piece);
        }
    }


    public void MovePieceFromNestToStart(Position startPos, Grid gameGrid)
    {
        if (Pieces.Count > 0)
        {
            Ellipse pieceToMove = Pieces[0]; // Get the first piece
            Pieces.RemoveAt(0); // Remove it from the nest

            // Move piece to starting position
            Grid.SetColumn(pieceToMove, startPos.ColumnIndex);
            Grid.SetRow(pieceToMove, startPos.RowIndex);
            gameGrid.Children.Add(pieceToMove);
        }
    }

    public void MoveGamePiece(int pieceId, int steps, Position[] route, List<Position> goalZone, Grid gameGrid)
    {
        // Logic to move a piece based on ID and steps
        Ellipse pieceToMove = Pieces[pieceId - 1]; // Assuming pieceId is 1-indexed
        // Here you would implement your movement logic with the route and handle goal zone
        // This is just a placeholder for actual movement logic
        // For simplicity, you can just change the position directly for now
        // Grid.SetColumn(pieceToMove, newColumn);
        // Grid.SetRow(pieceToMove, newRow);
    }
}
