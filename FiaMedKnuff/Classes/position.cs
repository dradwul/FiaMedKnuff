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

        public class GamePiece
        {
            public int Id { get; set; }
            public Position Position { get; set; }
            public int PlayerId { get; set; }

            public GamePiece(int id, Position position, int playerId)
            {
                Id = id;
                Position = position;
                PlayerId = playerId;
            }

            public void KnockOff(Position newPosition)
            {
                Position = newPosition;
            }
        }

        public class Player
        {
            private readonly GamePiece[] pieces;
            public int PlayerId { get; set; }

            public Player(int playerId, Position[] startingPositions)
            {
                PlayerId = playerId;
                pieces = new GamePiece[4];
                for (int i = 0; i < startingPositions.Length; i++)
                {
                    pieces[i] = new GamePiece(i + 1, startingPositions[i], playerId);
                }
            }

            public void MoveGamePiece(int gamePieceId, Position newPosition)
            {
                GamePiece piece = pieces.FirstOrDefault(p => p.Id == gamePieceId);
                if (piece == null)
                    throw new ArgumentException("Invalid game piece ID");

                if (newPosition.IsOccupied)
                {
                    KnockOffOpponent(newPosition);
                }

                piece.Position = newPosition;
                newPosition.IsOccupied = true;
            }

            /// <summary>
            /// Knock the opposing player off the board and place in their nest
            /// </summary>
            private void KnockOffOpponent(Position position)
            {
                foreach (var player in Game.Players)
                {
                    foreach (var piece in player.pieces)
                    {
                        if (piece.Position == position && piece.PlayerId != PlayerId)
                        {
                            piece.KnockOff(player.GetNestPosition());
                            position.IsOccupied = false;
                        }
                    }
                }
            }
            /// <summary>
            /// Find an available position in the player's nest
            /// </summary>
            public Position GetNestPosition()
            {
                return pieces.FirstOrDefault(p => p.Position.IsOccupied == false)?.Position;
            }
        }

        public static class Game
        {
            public static List<Player> Players { get; set; } = new List<Player>();
        }
    }

}
