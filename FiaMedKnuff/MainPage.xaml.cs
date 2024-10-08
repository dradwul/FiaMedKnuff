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

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        // Variabler för att hålla reda på varje spelares pjäser
        private Dictionary<int, List<Ellipse>> playerPieces;
        private int currentPlayer = 0;
        private int currentPlayerPieceIndex = 0;

        public MainPage()
        {
            this.InitializeComponent();

            InitializePieces(4);
            InitializeStartTiles();
            //MoveCurrentPiece();
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
        private static readonly string[] DiceImages =
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
            new Position[] { new Position(5,2), new Position(5,3), new Position(5,4), new Position(5,5) },
            new Position[] { new Position(1,6), new Position(2,6), new Position(3,6), new Position(4,6) },
            new Position[] { new Position(5,10), new Position(5,9), new Position(5,8), new Position(5,7) },
            new Position[] { new Position(9,6), new Position(8,6), new Position(7,6), new Position(6,6) }
        };

        /// <summary>
        /// The goal position for everyone
        /// </summary>
        public Position goalPosition = new Position(5, 6);


        /// <summary>
        /// Initializing the pieces in the nests. 
        /// PlaceholderPiece is for development. Will get changed to a gamepiece object.
        /// </summary>
        /// <param name="numberOfPlayers"></param>
        private void InitializePieces(int numberOfPlayers)
        {
            for(int i = 0; i < numberOfPlayers; i++)
            {
                SolidColorBrush color = colors[i];

                for(int j = 0; j < 4; j++)
                {
                    Ellipse placeholderPiece = new Ellipse
                    {
                        //Fill = color,
                        Fill = new SolidColorBrush(Windows.UI.Colors.Black),
                        Stroke = new SolidColorBrush(Windows.UI.Colors.White),
                        StrokeThickness = 3,
                        Width = 40,
                        Height = 40
                    };
                    Grid.SetColumn(placeholderPiece, nestPositions[i][j].ColumnIndex);
                    Grid.SetRow(placeholderPiece, nestPositions[i][j].RowIndex);


                    if (nestPositions[i][j].ColumnIndex == 2 || nestPositions[i][j].ColumnIndex == 9)
                    {
                        placeholderPiece.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    else if (nestPositions[i][j].ColumnIndex == 3 || nestPositions[i][j].ColumnIndex == 10)
                    {
                        placeholderPiece.HorizontalAlignment = HorizontalAlignment.Left;
                    }

                    if (nestPositions[i][j].RowIndex == 1 || nestPositions[i][j].RowIndex == 8)
                    {
                        placeholderPiece.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else if (nestPositions[i][j].RowIndex == 2 || nestPositions[i][j].RowIndex == 9)
                    {
                        placeholderPiece.VerticalAlignment = VerticalAlignment.Top;
                    }

                    GameGrid.Children.Add(placeholderPiece);
                }
            }
        }

        private void InitializeStartTiles()
        {
            for(int i = 0; i < 4; i++)
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

        /*private void MoveCurrentPiece()
        {
            // Hämta den aktuella spelarens pjäs som ska flyttas
            var currentPiece = playerPieces[currentPlayer][currentPlayerPieceIndex];

            // Hämta den nuvarande positionen för den spelaren
            Position currentPosition = allOuterPositions[currentPlayerPieceIndex];

            // Flytta till nästa position på brädet
            currentPlayerPieceIndex = (currentPlayerPieceIndex + 1) % allOuterPositions.Length;
            Position nextPosition = allOuterPositions[currentPlayerPieceIndex];

            // Flytta pjäsen till den nya positionen
            Grid.SetColumn(currentPiece, nextPosition.ColumnIndex);
            Grid.SetRow(currentPiece, nextPosition.RowIndex);

            // Växla till nästa spelare
            currentPlayer = (currentPlayer + 1) % playerPieces.Count;
        }*/

        // Exempel på att flytta pjäserna när sidan klickas
        private void GameGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //MoveCurrentPiece();
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
            string diceImage = DiceImages[diceValue - 1];

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
                double y = Random.Next(0, 40); // Random Y movement, change value to get bigger movements
                TimeSpan keyTime = TimeSpan.FromMilliseconds(i * 30); //Increase value to get slower movement

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
            Storyboard.SetTarget(translateXAnimation, DiceImage);
            Storyboard.SetTargetProperty(translateXAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(translateYAnimation, DiceImage);
            Storyboard.SetTargetProperty(translateYAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            storyboard.Children.Add(translateXAnimation);
            storyboard.Children.Add(translateYAnimation);
            // Update the dice image source once the animation is completed
            storyboard.Completed += (s, a) =>
            {
                DiceImage.Source = new BitmapImage(new Uri(diceImage));
            };

            storyboard.Begin();
            Debug.WriteLine(diceValue);
        }
    }
}