using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace FiaMedKnuff
{
    public class GamePiece
    {
        /// <summary>
        /// Gets or sets game piece ID which must be between 1 and 4
        /// </summary>
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                if (1 <= value && value <= 4)
                    id = value;
                else
                    throw new ArgumentOutOfRangeException("A game piece ID can only be a number between 1 and 4");
            }
        }

        /// <summary>
        /// This is the shape for the game piece
        /// </summary>
        public Ellipse GamePieceShape { get; set; }

        /// <summary>
        /// Gets or sets number of steps taken for the game piece 
        /// Starts at 0 steps, 40 when it is at the entrance of the safe zone (can still lose the piece) and 45 when it has reached its goal
        /// </summary>
        private int stepsTaken = 0;
        public int StepsTaken
        {
            get { return stepsTaken; }
            set
            {
                if (0 <= value && value <= 45)
                    stepsTaken = value;
                else
                    throw new ArgumentOutOfRangeException("A game piece can only take between 0 and 45 total steps");
            }
        }

        /// <summary>
        /// This is the current position of the game piece
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Constructor for a game piece
        /// </summary>
        /// <param name="id"> ID for a game piece (1-4) </param>
        /// <param name="ellipse"> This is the shape for the game piece </param>
        /// <param name="position"> Game piece position on the game board </param>
        public GamePiece(int id, Ellipse ellipse, Position position)
        {
            Id = id;
            GamePieceShape = ellipse;
            Position = position;
        }
    }
}