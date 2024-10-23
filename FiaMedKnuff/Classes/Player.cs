using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace FiaMedKnuff
{
    public class Player
    {
        //MediaPlayer instances
        private MediaPlayer moveSoundPlayer;
        private MediaPlayer knockOffSoundPlayer;
        /// <summary>
        /// Gets or sets the player ID which must be a number between 1 and 4
        /// </summary>
        private int playerId;
        // PlayerId changed to public instead of private to get access in startingPlayer method
        public int PlayerId
        {
            get { return playerId; }
            set
            {
                if (1 <= value && value <= 4)
                    playerId = value;
                else
                    throw new ArgumentOutOfRangeException("Player ID can only be a number between 1 and 4");
            }
        }

        /// <summary>
        /// Array for the player pieces, this is created when the constructor is used
        /// </summary>
        private readonly GamePiece[] pieces = new GamePiece[4];


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
                //This determines the shape of the game pieces
                Ellipse placeholderPiece01 = new Ellipse
                {
                    Fill = new SolidColorBrush(pieceColor),
                    Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                    StrokeThickness = 2,
                    Width = 40,
                    Height = 40,
                    IsTapEnabled = false
                };

                pieces[i] = new GamePiece(i + 1, placeholderPiece01, startingPositions[i]);
				Grid.SetRow(pieces[i].GamePieceShape, pieces[i].Position.RowIndex);
                Grid.SetColumn(pieces[i].GamePieceShape, pieces[i].Position.ColumnIndex);

                //Adds the event to the game piece shape (ellipse)
				pieces[i].GamePieceShape.Tapped += gamePieceClicked;
			}
            //Initialize sound effects
            InitializeSounds();
            
        }

        // Method to initialize the sound effects
        private void InitializeSounds()
        {
            moveSoundPlayer = new MediaPlayer();
            moveSoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Move.mp3"));
            moveSoundPlayer.Volume = 0.04; //Volume

            knockOffSoundPlayer = new MediaPlayer();
            knockOffSoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Knock.mp3"));
            knockOffSoundPlayer.Volume = 0.04; //Volume
        }

        // Method to play the move sound effect
        private void PlayMoveSound()
        {
            if (moveSoundPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                moveSoundPlayer.Pause(); // Stop the move sound if it's already playing
                moveSoundPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset position
            }

            moveSoundPlayer.Play();
        }

        // Method to play the knock-off sound effect
        private void PlayKnockOffSound()
        {
            if (knockOffSoundPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                knockOffSoundPlayer.Pause(); // Stop the knock-off sound if it's already playing
                knockOffSoundPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset position
            }

            knockOffSoundPlayer.Play();
        }

        /// <summary>
        /// Toggles the game piece sound when moving
        /// </summary>
        public void ToggleMoveSound()
        {
            if (moveSoundPlayer.Volume == 0)
                moveSoundPlayer.Volume = 0.04;
            else
                moveSoundPlayer.Volume = 0;
        }

        /// <summary>
        /// This function returns the game piece shape that belongs to the specified game piece
        /// It is used for the purpose the initial placing of the game pieces
        /// </summary>
        /// <param name="id"> The ID determines which game piece is being referred to (1-4) </param>
        /// <returns> Returns the specified game piece shape (Ellipse) </returns>
        /// <exception cref="ArgumentException"> Throws an exepction if no game piece has the specified ID </exception>
        public Ellipse ReturnGamePieceShape(int id)
        {
            foreach (GamePiece piece in pieces)
            {
                if (piece.Id == id)
                    return piece.GamePieceShape;
            }
            throw new ArgumentException("Game piece shape not found");
        }

		/// <summary>
		/// Moves the specified game pieces
		/// </summary>
		/// <param name="id"> ID for specified game piece </param>
		/// <param name="diceRoll"> Dice roll from 1-6 </param>
		/// <param name="position"> Position array of possible "tiles" </param>
		/// <param name="gameGrid"> Used for animations and knocking a piece off </param>
		/// <param name="diceImage"> This is the clickable part of the dice. Gets sent to AnimateGamePiece </param>
		public async void MoveGamePiece(int id, int diceRoll, Position[] positions, Grid gameGrid, Image diceImage)
        {
            foreach (GamePiece piece in pieces)
            {
                // Find the correct game piece and check that it does not move more than 45 steps
                if (piece.Id == id && piece.StepsTaken + diceRoll <= 45)
                {
                   
                    // Determine the target position based on dice roll
                    Position targetPosition = positions[piece.StepsTaken + diceRoll - 1];

                    await AnimateGamePiece(piece, diceRoll, positions, piece.StepsTaken, diceImage);

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
                    piece.StepsTaken += diceRoll;
                    piece.Position = targetPosition;

                    //Makes sure the piece doesn't occupy the middle position
                    if(piece.StepsTaken != 45)
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

        /// <summary>
        /// Used after moving a piece to check if it has reached its goal so that it can be moved to the goal zone
        /// </summary>
        /// <param name="id"> ID for the specified game piece</param>
        /// <param name="goalZone"> The zone to move the game piece from </param>
        /// <param name="gameGrid"> Current location of the game piece </param>
        public void CheckGoalReached(int id, StackPanel goalZone, Grid gameGrid) 
        {
            foreach (GamePiece piece in pieces)
            {
				//Find the correct game piece and make sure it is not in the goal zone already
				if (piece.Id == id && piece.StepsTaken == 45 && !goalZone.Children.Contains(piece.GamePieceShape))
                {
					//Remove piece from gameGrid and add to goalZone
					gameGrid.Children.Remove(piece.GamePieceShape);
					goalZone.Children.Add(piece.GamePieceShape);

					piece.GamePieceShape.Width = 30;
					piece.GamePieceShape.Height = 30;
					piece.GamePieceShape.Margin = new Thickness(5);
				}
            }
        }

        /// <summary>
        /// Checks if the player has achieved victory
        /// </summary>
        /// <returns> True if player has won and false if not </returns>
        public bool VictoryCheck()
        {
            int gamePiecesInSafeZone = 0;
            foreach (GamePiece piece in pieces)
            {
                if(piece.StepsTaken == 45)
                    gamePiecesInSafeZone++;
            }

            return gamePiecesInSafeZone == 4;
        }

        /// <summary>
        /// Enables the game pieces that have legal moves and highlights them
        /// </summary>
        public void EnableGamePieces(int diceRoll)
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
                else if(piece.StepsTaken + diceRoll <= 45 && piece.StepsTaken != 0 && piece.StepsTaken != 45)
                {
                    piece.GamePieceShape.IsTapEnabled = true;
					piece.GamePieceShape.StrokeThickness = 4;
				}
			}
        }

        /// <summary>
        /// Disables the game pieces and removes any highlights
        /// </summary>
		public void DisableGamePieces()
		{
			foreach (GamePiece piece in pieces)
			{
				piece.GamePieceShape.IsTapEnabled = false;
				piece.GamePieceShape.StrokeThickness = 2;
			}
		}

        /// <summary>
        /// Checks if the player has any legal moves
        /// </summary>
        /// <param name="diceRoll"> Has value of the current dice roll </param>
        /// <returns> Returns a boolean, true if there are legal moves and false if there are no legal moves </returns>
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
		/// <param name="diceImage">Image of the dice so it can be enabled after animation</param>

		public async Task AnimateGamePiece(GamePiece piece, int diceRoll, Position[] path, int currentStep, Image diceImage)
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