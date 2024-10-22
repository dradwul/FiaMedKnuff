using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using System.Threading.Tasks;
using System.Threading;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        //Routes for all players
        private Position[][] playerRoutes = new Position[4][];


        //This is where the pieces move when they reach their goal
        private StackPanel[] goalReachedContainer = new StackPanel[4];

        //Fields for deciding starting player
        private Dictionary<Player, int> playersAndRolls = new Dictionary<Player, int>();
        private Dictionary<Player, int> playersWithHighestRolls = new Dictionary<Player, int>();
        private int startingPlayerId;

        //List of players in current game
        public List<Player> playerList = new List<Player>();
        private Player startingPlayer;

        //Variable for the dice value (1-6)
        private int currentDiceValue;

        //Variable to store curreny players turn (1-4)
        private int currentPlayersTurn;

        //Temporary colors for the players
        string[] playerColors = new string[4];

        //MediaPlayer instance for background music
        private MediaPlayer mediaPlayer;
        public MainPage()
        {
            this.InitializeComponent();
            InitializeStartTiles();
            InitializeGame();
		}

        /// <summary>
        /// Function to initilize the game
        /// </summary>
        private void InitializeGame()
        {
            //Initialize MediaPlayer
             mediaPlayer = new MediaPlayer();
            mediaPlayer.Volume = 0.2; // Set volume
            mediaPlayer.AudioCategory = MediaPlayerAudioCategory.GameMedia; // Ensure it's treated as game media
            PlayMenuMusic();
           
            //Creates the routes for all players
            playerRoutes[0] = allOuterPositions.Concat(endPositions[0]).ToArray();
            playerRoutes[1] = ShiftArray(allOuterPositions, 10).Concat(endPositions[1]).ToArray();
            playerRoutes[2] = ShiftArray(allOuterPositions, 20).Concat(endPositions[2]).ToArray();
            playerRoutes[3] = ShiftArray(allOuterPositions, 30).Concat(endPositions[3]).ToArray();

			//Initilizes the zone for each player where the pieces go when they reach the goal
			goalReachedContainer[0] = piecesInGoalZonePlayer1;
			goalReachedContainer[1] = piecesInGoalZonePlayer2;
			goalReachedContainer[2] = piecesInGoalZonePlayer4;
			goalReachedContainer[3] = piecesInGoalZonePlayer3;

			//Temporary colors
			playerColors[0] = "blue";
			playerColors[1] = "yellow";
			playerColors[2] = "green";
			playerColors[3] = "red";
		}
        private async void PlayMenuMusic()
        {
            mediaPlayer.IsLoopingEnabled = true;
            mediaPlayer.Pause(); // Stop current playback
            mediaPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset playback position
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Menu.mp3"));
            await Task.Delay(100); // Short delay to ensure MediaPlayer has time to load the new source
            mediaPlayer.Play();
        }

        private async void PlayGameplayMusic()
        {
            mediaPlayer.IsLoopingEnabled= true;
            mediaPlayer.Pause(); // Stop current playback
            mediaPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset playback position
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Play.mp3"));
            await Task.Delay(100); // Short delay to ensure MediaPlayer has time to load the new source
            mediaPlayer.Play();
        }

        private async void PlayWinMusic()
        {
            mediaPlayer.Pause(); // Stop current playback
            mediaPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset playback position
            mediaPlayer.IsLoopingEnabled = false; // Disable looping for win music
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Win.mp3"));
            await Task.Delay(100); // Short delay to ensure MediaPlayer has time to load the new source
            mediaPlayer.Play();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide start menu and show player select
            startMenuButtons.Visibility = Visibility.Collapsed;
            playerSelectButtons.Visibility = Visibility.Visible;
        }

        private void PlayersSelected(object sender, RoutedEventArgs e)
        {
            // Cast sender to a button to access its content (number between 2-4)
            Button clickedButton = sender as Button;
            int numberOfPlayers = int.Parse(clickedButton.Content.ToString());

            GeneratePlayers(numberOfPlayers);
            startMenu.Visibility = Visibility.Collapsed;
            startMenuButtons.Visibility = Visibility.Collapsed;
            playerSelectButtons.Visibility = Visibility.Collapsed;

            // Start gameplay music after player selection is done and gameboard is visible
            PlayGameplayMusic();
        }

        private void OnPlayerWin()
        {
            // Stop gameplay music and play win music
            PlayWinMusic();
            // Additional win logic goes here
        }


        /// <summary>
        /// Function to clear the game to be able to restart
        /// </summary>
        private void ClearGame()
        {
			foreach (Player player in playerList)
			{
				for (int i = 1; i <= 4; i++)
				{
					GameGrid.Children.Remove(player.ReturnGamePieceShape(i));
				}
			}
            for (int i = 0; i < 4; i++)
            {
                goalReachedContainer[i].Children.Clear();
            }
			playerList.Clear();
		}

        /// <summary>
        /// Shifts an array to the left
        /// Example where we shift an array 1 step to the left: [2,5,3] -> [5,3,2]
        /// </summary>
        /// <param name="array"> Input array to shift </param>
        /// <param name="steps"> Amount of steps to shift </param>
        /// <returns> Returns the array that has been shifted </returns>
		private Position[] ShiftArray(Position[] array, int steps)
		{
			return array.Skip(steps).Concat(array.Take(steps)).ToArray();
		}

		/// <summary>
		/// Available colors
		/// </summary>
		readonly SolidColorBrush[] colors = new SolidColorBrush[]
        {
            new SolidColorBrush(Windows.UI.Colors.Blue),
            new SolidColorBrush(Windows.UI.Colors.Yellow),
            new SolidColorBrush(Windows.UI.Colors.Green),
            new SolidColorBrush(Windows.UI.Colors.Red)
        };

        /// <summary>
        /// Creates a random number generator.
        /// Creates an array of dice images.
        /// </summary>
        private static readonly Random Random = new Random();
        private static readonly string[] diceImages =
        {
            "ms-appx:///Assets/dice1.png",
            "ms-appx:///Assets/dice2.png",
            "ms-appx:///Assets/dice3.png",
            "ms-appx:///Assets/dice4.png",
            "ms-appx:///Assets/dice5.png",
            "ms-appx:///Assets/dice6.png"
        };

        /// <summary>
        /// All outer positions that are neutral and open for every player. 
        /// Got the property IsOccupied to mark it as taken by a game piece. 
        /// The first element in the array is the same as Player1 starting zone. Then it follow 2,3,4,5... clockwise.
        /// </summary>
        public Position[] allOuterPositions = new Position[]
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

        /// <summary>
        /// Array with 4 arrays with the 4 nest squares for each color
        /// </summary>
        public Position[][] nestPositions = new Position[][]
        {
            new Position[] { new Position(1,2), new Position(1,3), new Position(2,2), new Position(2,3) },
            new Position[] { new Position(1,9), new Position(1,10), new Position(2,9), new Position(2,10) },
            new Position[] { new Position(8,9), new Position(8,10), new Position(9,9), new Position(9,10) },
            new Position[] { new Position(8,2), new Position(8,3), new Position(9,2), new Position(9,3) }
        };

        public Position[] startPositions = new Position[]
        {
            new Position(4,1),
            new Position(0,7),
            new Position(6,11),
            new Position(10,5)
        };

        /// <summary>
        /// The final 4 tiles in the endzone of each player.
        /// </summary>
        public Position[][] endPositions = new Position[][]
        {
            new Position[] { new Position(5,2), new Position(5,3), new Position(5,4), new Position(5,5), new Position(5,6) },
            new Position[] { new Position(1,6), new Position(2,6), new Position(3,6), new Position(4,6), new Position(5,6)},
            new Position[] { new Position(5,10), new Position(5,9), new Position(5,8), new Position(5,7) , new Position(5, 6) },
            new Position[] { new Position(9,6), new Position(8,6), new Position(7,6), new Position(6,6) , new Position(5, 6) }
        };

        /// <summary>
        /// This method will initialize a start tiles for each player
        /// It will appear with the same border color as the players nest/pieces
        /// </summary>
        private void InitializeStartTiles()
        {
            for (int i = 0; i < 4; i++)
            {
                SolidColorBrush color = colors[i];

                Ellipse startCircle = new Ellipse
                {
                    Fill = new SolidColorBrush(Windows.UI.Colors.White),
                    Stroke = color,
                    StrokeThickness = 8
                };
                Grid.SetColumn(startCircle, startPositions[i].ColumnIndex);
                Grid.SetRow(startCircle, startPositions[i].RowIndex);

                GameGrid.Children.Add(startCircle);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustGridSize();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustGridSize();
        }


        /// <summary>
        /// This method changes grid sizes when the window size is being adjusted
        /// </summary>
        private void AdjustGridSize()
        {
            double availableWidth = this.ActualWidth;
            double availableHeight = this.ActualHeight;

            double gridSize = Math.Min(availableWidth, availableHeight);

            GameGrid.Width = gridSize;
            GameGrid.Height = gridSize;
        }

        /// <summary>
        /// Handles the Tapped event of the DiceImage control.
        /// Generates a random dice value, animates the correct dice image and creates a random movement animation.
        /// </summary>
        private void DiceImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int diceValue = Random.Next(1, 7);
            string diceImager = diceImages[diceValue - 1];

            // Create a random movement animation for X-axis and Y-axis.
            var storyboard = new Storyboard();
            var translateXAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var translateYAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            // Define random movements with X and Y axis.
            for (int i = 0; i <= 10; i++)
            {
                double x = Random.Next(-20, 20); // Random X movement, change value to get bigger movements
                double y = Random.Next(-20, 10); // Random Y movement, change value to get bigger movements
                TimeSpan keyTime = TimeSpan.FromMilliseconds(i * 40); //Increase value to get slower movement

                translateXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = keyTime,
                    Value = x
                });
                translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = keyTime,
                    Value = y
                });
            }

            // Sets the target for the animations
            Storyboard.SetTarget(translateXAnimation, diceImage);
            Storyboard.SetTargetProperty(translateXAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(translateYAnimation, diceImage);
            Storyboard.SetTargetProperty(translateYAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            storyboard.Children.Add(translateXAnimation);
            storyboard.Children.Add(translateYAnimation);
            // Update the dice image source once the animation is completed
            storyboard.Completed += (s, a) =>
            {
                diceImage.Source = new BitmapImage(new Uri(diceImager));
            };

            storyboard.Begin();
            Debug.WriteLine(diceValue);

            //Stores the value of the dice to a global
            currentDiceValue = diceValue;

            Debug.WriteLine("Current players turn: " + currentPlayersTurn);

            //Checks if the player has a legal move and disables the dice
            if (playerList[currentPlayersTurn - 1].CheckIfPlayerHasLegalMoves(currentDiceValue))
            {
				diceImage.IsTapEnabled = false;
				playerList[currentPlayersTurn - 1].EnableGamePieces(currentDiceValue);
            }
            //If player has no legal moves go to next player
            else
            {
                currentPlayersTurn++;
                if (currentPlayersTurn > playerList.Count)
                    currentPlayersTurn = 1;
            }

            UpdatePlayerNestUI(currentPlayersTurn);
        }

		/// <summary>
		/// Checks which game piece was clicked and moves it according the current value of the dice
		/// </summary>
		/// <param name="sender"> The shape of the game piece that was clicked, an ellipse</param>
		/// <param name="e"></param>
		private void GamePieceClicked(object sender, TappedRoutedEventArgs e)
		{
            //Casts sender as an ellipse to be able to identify which ellipse was clicked
			Ellipse clickedEllipse = sender as Ellipse;

			//Finds the clicked ellipse in the class instance and moves it
			for (int i = 1; i <= 4; i++)
            {
                if (clickedEllipse == playerList[currentPlayersTurn-1].ReturnGamePieceShape(i))
                {
                    //Moves the piece and checks if it has reached its goal
                    playerList[currentPlayersTurn - 1].MoveGamePiece(i, currentDiceValue, playerRoutes[currentPlayersTurn-1]); // TODO: Check if the piece is allowed to move so the player doesn't waste a turn by clicking a piece that can't move
                    playerList[currentPlayersTurn - 1].CheckGoalReached(i, goalReachedContainer[currentPlayersTurn-1], GameGrid);

                    //Check if the player has won and enable victory screen
                    if(playerList[currentPlayersTurn - 1].VictoryCheck())
					{ 
                        victoryScreen.Visibility = Visibility.Visible;
                        winnerTextBlock.Text = "Player " + playerList[currentPlayersTurn - 1].PlayerId + " Wins!";
				    }

				    //Disables the pieces from being clicked and enables the dice
				    playerList[currentPlayersTurn-1].DisableGamePieces();
                    diceImage.IsTapEnabled = true;
                }
			}
            //Checks if player can go again if a 6 was rolled
            if(currentDiceValue != 6)
			    currentPlayersTurn++;

            //Reset back to player 1 when last player has played
			if (currentPlayersTurn > playerList.Count)
				currentPlayersTurn = 1;

            UpdatePlayerNestUI(currentPlayersTurn);
        }

		private void PlayersSelectedHandler(object sender, RoutedEventArgs e)
		{
			//Casts sender to a button to access its content (number between 2-4)
			Button clickedButton = sender as Button;
            int numberOfPlayers = int.Parse(clickedButton.Content.ToString());

            GeneratePlayers(numberOfPlayers);
            startMenu.Visibility = Visibility.Collapsed;
            startMenuButtons.Visibility = Visibility.Collapsed;
            playerSelectButtons.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Generates players depending on input (2-4 players)
        /// </summary>
        /// <param name="amountOfPlayers"></param>
        private void GeneratePlayers(int amountOfPlayers)
        {
            //Populate the player list with players using Player constructor
            for (int i = 0; i < amountOfPlayers; i++)
            {
                playerList.Add(new Player(i+1, playerColors[i], nestPositions[i], GamePieceClicked));
            }

            foreach (Player player in playerList)
            {
				//Placing game pieces on the board (ID 1-4, not 0-3)
				for (int i = 1; i <= 4; i++)
				{
					GameGrid.Children.Add(player.ReturnGamePieceShape(i));
				}
			}

			playerSelectView.Visibility = Visibility.Collapsed;
            GameGrid.Visibility = Visibility.Visible;
            
            decideStartingPlayerView.Visibility = Visibility.Visible;
            
            //Resets the turn order function for when playing a new game
            playersWithHighestRolls.Clear();
            playersAndRolls.Clear();
            sPanelP1.Visibility = Visibility.Visible;
			sPanelP2.Visibility = Visibility.Visible;
			sPanelP3.Visibility = Visibility.Collapsed;
			sPanelP4.Visibility = Visibility.Collapsed;

			if (playerList.Count == 3)
            {
                sPanelP3.Visibility = Visibility.Visible;
            }
            if(playerList.Count == 4)
            {
                sPanelP3.Visibility = Visibility.Visible;
                sPanelP4.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Event method for rolling to decide who will start the game.
        /// Includes logic and ui updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RollForAll_Click(object sender, RoutedEventArgs e)
        {
            // Disables roll button to prevent double clicks
            decideWhoWillStartButton.IsEnabled = false;

            // Will enter this statement if it's the first roll
            if (playersAndRolls.Count == 0)
            {
                foreach (var player in playerList)
                {
                    int roll = Random.Next(1, 7);
                    playersAndRolls[player] = roll;

                    // Gets element name for every player's roll area
                    string playerRollTextName = $"sPanelP{player.PlayerId}Roll";
                    TextBlock playerRollTextBlock = this.FindName(playerRollTextName) as TextBlock;


                    if (playerRollTextBlock != null)
                    {
                        playerRollTextBlock.Text = roll.ToString();
                    }

                    await Task.Delay(700);
                }
            }
            else if(playersAndRolls.Count > 0)
            {
                playersAndRolls.Clear();
                foreach(var player in playersWithHighestRolls)
                {
                    int roll = Random.Next(1, 7);
                    playersAndRolls[player.Key] = roll;

                    string playerRollTextName = $"sPanelP{player.Key.PlayerId}Roll";
                    TextBlock playerRollTextBlock = this.FindName(playerRollTextName) as TextBlock;


                    if (playerRollTextBlock != null)
                    {
                        playerRollTextBlock.Text = roll.ToString();
                    }

                    await Task.Delay(700);
                }
                playersWithHighestRolls.Clear();
            }


            int highestRoll = playersAndRolls.Values.Max();

            // Add players with the highest roll/rolls to list
            foreach (var player in playersAndRolls)
            {
                if (player.Value == highestRoll)
                {
                    playersWithHighestRolls.Add(player.Key, player.Value);
                }
            }

            // Changes button text if more than 1 got highest roll
            if(playersWithHighestRolls.Count != 1)
            {
                decideWhoWillStartButton.Content = "ReRoll!";

                // Iterate through the list with all players and hide player element if the player don't appear in playersWithHighestRoll
                foreach (var player in playersAndRolls)
                {
                    if (playersWithHighestRolls.ContainsKey(player.Key))
                    {
                        continue;
                    }
                    else
                    {
                        string elementName = $"sPanelP{player.Key.PlayerId}";
                        var elementToHide = this.FindName(elementName) as StackPanel;
                        elementToHide.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                playerToStartGrid.Visibility = Visibility.Visible;
                playerToStartTextBox.Text = $"Player {playersWithHighestRolls.First().Key.PlayerId} rolled highest and will start the game!";

                startingPlayer = playersWithHighestRolls.First().Key;

                await Task.Delay(2000);
                playerToStartGrid.Visibility = Visibility.Collapsed;
                decideStartingPlayerView.Visibility = Visibility.Collapsed;

                currentPlayersTurn = startingPlayer.PlayerId;
                Debug.WriteLine($"Player {currentPlayersTurn} is the starting player");

                UpdatePlayerNestUI(currentPlayersTurn);
            }

            decideWhoWillStartButton.IsEnabled = true;
        }

        /// <summary>
        /// This method will update the border for player nests.
        /// Current player gets a border and previous will get it's border removed.
        /// </summary>
        /// <param name="currentPlayer">Gets current player ID</param>
        private async void UpdatePlayerNestUI(int currentPlayer)
        {
            await Task.Delay(400); // This should be synced with AnimateGamePiece in Player class

            string previousPlayerHighlight;
            string currentPlayerHighlight;
            
            // Remove border from previous player's nest
            if (currentPlayer == 1)
            {
                previousPlayerHighlight = $"borderNest{playerList.Count}";
            }
            else
            {
                previousPlayerHighlight = $"borderNest{currentPlayer - 1}";
            }
            var oldPlayerHighlightBorder = this.FindName(previousPlayerHighlight) as Border;
            if(oldPlayerHighlightBorder != null)
            {
                oldPlayerHighlightBorder.BorderThickness = new Thickness(0);
            }
            
            // Enable border for current player's nest
            currentPlayerHighlight = $"borderNest{currentPlayer}";
            var newPlayerHighlightBorder = this.FindName(currentPlayerHighlight) as Border;
            newPlayerHighlightBorder.BorderThickness = new Thickness(10);
        }


        /// <summary>
        /// Hides the start menu when the start button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            //startMenu.Visibility = Visibility.Collapsed;
            startMenuButtons.Visibility = Visibility.Collapsed;
            playerSelectButtons.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Shows the rules, when rules button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            rulesMenu.Visibility = Visibility.Visible;
            startMenuButtons.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Exits the program when the exit button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        /// <summary>
        /// Exits the rules menu when the exit button is clicked. Brings the user back to the start menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitRules_Click(object sender, RoutedEventArgs e)
        {
            rulesMenu.Visibility = Visibility.Collapsed;
            startMenuButtons.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Play again button takes the player through the player select process
        /// and turn order to start a new game. 
        /// It also resets the board (button only available when a player has won)
        /// </summary>
		private void PlayAgain(object sender, RoutedEventArgs e)
		{
            ClearGame();
            InitializeGame();
			playerSelectButtons.Visibility= Visibility.Visible;
			startMenu.Visibility = Visibility.Visible;
			victoryScreen.Visibility = Visibility.Collapsed;
		}

        /// <summary>
        /// Resets the board and returns to menu (button only available when a player has won)
        /// </summary>
		private void ReturnToMenu(object sender, RoutedEventArgs e)
		{
            ClearGame();
            InitializeGame();
			startMenu.Visibility = Visibility.Visible;
            startMenuButtons.Visibility = Visibility.Visible;
            victoryScreen.Visibility = Visibility.Collapsed;
		}
	}
}