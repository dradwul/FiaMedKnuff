using FiaMedKnuff.Classes;
using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace FiaMedKnuff
{
    public class Player
    {
        private readonly Sound sound;

        public int PlayerId { get; set; }

        /// <summary>
        /// Array for the player pieces, this is created when the constructor is used
        /// </summary>
        public GamePiece[] pieces = new GamePiece[4];

        /// <summary>
        /// Constructor for player with 3 parameters
        /// </summary>
        /// <param name="playerId"> ID for the player (1-4) </param>
        /// <param name="color"> Color for the player </param>
        /// <param name="positions"> An array with the starting positions for the player pieces (nest) </param>
        /// <param name="gamePieceClicked"> This is the event that is linked to each game piece </param>
        public Player(int playerId, string color, Position[] startingPositions, TappedEventHandler gamePieceClicked)
        {
            PlayerId = playerId;
            Windows.UI.Color pieceColor = GeneratePieceColor(color);

            if (startingPositions.Length != 4)
                throw new ArgumentException("A player needs 4 starting positions for his game pieces");

            for (int i = 0; i < startingPositions.Length; i++)
            {
                GamePiece pieceInstance = new GamePiece();
                var newPieceShape = pieceInstance.CreateNew(pieceColor);
                GamePiece newPiece = new GamePiece(i + 1, newPieceShape, startingPositions[i]);

                pieces[i] = newPiece;
                Grid.SetRow(pieces[i].GamePieceShape, pieces[i].Position.RowIndex);
                Grid.SetColumn(pieces[i].GamePieceShape, pieces[i].Position.ColumnIndex);

                //Adds the event to the game piece shape (ellipse)
                pieces[i].GamePieceShape.Tapped += gamePieceClicked;
            }
            //Initialize sound effects
            sound = new Sound();
            InitializeSounds();
        }


        private void InitializeSounds() => sound.InitializeSounds();
        private void PlayMoveSound() => sound.PlayMoveSound();
        private void PlayKnockOffSound() => sound.PlayKnockOffSound();
        public void ToggleMoveSound() => sound.ToggleMoveSound();


        

		public async void MoveGamePiece(int id, int diceRoll, Position[] positions, Grid gameGrid, StackPanel goalZone, Image diceImage)
        {
            foreach (GamePiece piece in pieces)
            {
                int targetSteps = piece.StepsTaken + diceRoll;

                // Find the correct game piece and check that it does not move more than 45 steps
                if (piece.Id == id && targetSteps <= 45)
                {
                    // Determine the target position based on dice roll
                    Position targetPosition = positions[targetSteps - 1];

                    await AnimateGamePiece(piece, diceRoll, positions, piece.StepsTaken, goalZone, diceImage);

                    // Check if the target position is occupied
                    if (targetPosition.IsOccupied)
                    {
                        PlayKnockOffSound();
                        // Knock off the occupying piece
                        await targetPosition.KnockOffPiece(gameGrid);
                    }

                    // Before moving, mark the current position as not occupied
                    piece.Position.IsOccupied = false;
                    piece.Position.OccupyingPiece = null;

                    // Move the current piece to the target position
                    piece.StepsTaken = targetSteps;  // Use the calculated target steps
                    piece.Position = targetPosition;

                    // Make sure the piece doesn't occupy the middle position
                    if (piece.StepsTaken != 45)
                    {
                        targetPosition.IsOccupied = true;
                        targetPosition.OccupyingPiece = piece;
                    }

                    // Update UI to reflect the new position of the game piece
                    Grid.SetRow(piece.GamePieceShape, piece.Position.RowIndex);
                    Grid.SetColumn(piece.GamePieceShape, piece.Position.ColumnIndex);
                }
            }
        }

        public bool CheckIfPlayerHasLegalMoves(int diceRoll)
        {
			foreach (GamePiece piece in pieces)
			{
				//Checks if player can move a piece out of the nest
				if ((diceRoll == 1 || diceRoll == 6) && piece.StepsTaken == 0)
				{
					return true;
				}
				//Checks if there are other legal moves for the player
				else if (piece.StepsTaken + diceRoll <= 45 && piece.StepsTaken != 0 && piece.StepsTaken != 45)
				{
					return true;
				}
			}
            return false;
		}

        /// <summary>
        /// Helper method, generates a window UI color for the game pieces
        /// </summary>
        /// <param name="color"> This is the desired color for the game piece </param>
        /// <returns> The game piece color in window UI format </returns>
        private Windows.UI.Color GeneratePieceColor(string color)
        {
            Windows.UI.Color pieceColor;

			switch (color.ToLower())
			{
				case "red":
                    pieceColor = Windows.UI.ColorHelper.FromArgb(255, 248, 97, 97);
					break;
				case "blue":
                    pieceColor = Windows.UI.ColorHelper.FromArgb(255, 79, 171, 238);
					break;
				case "green":
					pieceColor = Windows.UI.ColorHelper.FromArgb(255, 114, 228, 126);
					break;
				case "yellow":
					pieceColor = Windows.UI.ColorHelper.FromArgb(255, 230, 234, 47);
					break;
				default:
					pieceColor = Windows.UI.Colors.Black;
					break;
			}
            return pieceColor;
		}


        /// <summary>
        /// Method for step animation for pieces
        /// </summary>
        /// <param name="piece">Current piece in use</param>
        /// <param name="diceRoll">Value of the dice</param>
        /// <param name="path">The current piece's path</param>
        /// <param name="currentStep">Current step in piece/player's path</param>
        /// <param name="goalZone">Goal zone StackPanel for the player</param>
        /// <param name="diceImage">Image of the dice so it can be enabled after animation</param>
        /// <returns></returns>
        public async Task AnimateGamePiece(GamePiece piece, int diceRoll, Position[] path, int currentStep, StackPanel goalZone, Image diceImage)
        {
            // Use of TranslateTransform for game movement
            TranslateTransform translateTransform = new TranslateTransform();
            piece.GamePieceShape.RenderTransform = translateTransform;

            // Initial position for the piece
            Grid.SetRow(piece.GamePieceShape, path[currentStep].RowIndex);
            Grid.SetColumn(piece.GamePieceShape, path[currentStep].ColumnIndex);

            // Looping through the positions in the path
            for (int i = currentStep; i < currentStep + diceRoll; i++)
            {
                // Check if the piece is about to enter the goal zone
                if (i + 1 == path.Length) // Assuming the path length is 45 (0-indexed)
                {
                    // Move piece to goal zone and exit the loop
                    PlayMoveSound();

                    // Remove piece from the grid (so it doesn't stay in its old position)
                    var parent = piece.GamePieceShape.Parent as Grid;
                    if (parent != null)
                    {
                        parent.Children.Remove(piece.GamePieceShape);
                    }

                    // Add piece to the player's goal zone (StackPanel)
                    goalZone.Children.Add(piece.GamePieceShape);

                    piece.StepsTaken = 45;

                    diceImage.IsTapEnabled = true;

                    return; // Exit the method since the piece is now in the goal zone
                }

                // Normal movement animation for the piece
                PlayMoveSound();
                Position startPosition = path[i];
                Position endPosition = path[i + 1];

                // Calculate movement for the piece
                double deltaX = endPosition.ColumnIndex - startPosition.ColumnIndex;
                double deltaY = endPosition.RowIndex - startPosition.RowIndex;

                // Duration and speed of piece movement
                double duration = 300;
                double steps = 10; // Increase for a more smooth movement
                double stepDuration = duration / steps;

                // Animation for the piece
                for (int step = 0; step < steps; step++)
                {
                    // Calculate current progress of the piece
                    double linearProgress = (double)step / steps;
                    double easedProgress = EaseInOutQuad(linearProgress); // Använd easing-funktionen

                    // Update TranslateTransform
                    translateTransform.X = deltaX * easedProgress;
                    translateTransform.Y = deltaY * easedProgress;

                    // Delay for piece movement
                    await Task.Delay((int)stepDuration);
                }

                // Update the final position in the grid after the animation step is complete
                Grid.SetRow(piece.GamePieceShape, endPosition.RowIndex);
                Grid.SetColumn(piece.GamePieceShape, endPosition.ColumnIndex);
            }
            //Enables dice when animation is over
			diceImage.IsTapEnabled = true;
		}

		/// <summary>
		/// Method for smooth piece animation
		/// </summary>
		/// <param name="progress"></param>
		/// <returns></returns>
		public double EaseInOutQuad(double progress)
        {
            return progress < 0.5 ? 2 * progress * progress : 1 - Math.Pow(-2 * progress + 2, 2) / 2;
        }
    }
}