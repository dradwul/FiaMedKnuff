using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

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
            MoveCurrentPiece();
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

        public Position[] allOuterPositions = new Position[]
        {
            // Ytterpositioner som ska användas för att flytta pjäserna.
            new Position(4,1), new Position(4,2), new Position(4,3), new Position(4,4),
            new Position(4,5), new Position(3,5), new Position(2,5), new Position(1,5),
            new Position(0,5), new Position(0,6), new Position(0,7), new Position(1,7),
            new Position(2,7), new Position(3,7), new Position(4,7), new Position(4,8),
            new Position(4,9), new Position(4,10), new Position(4,11), new Position(5,11),
            new Position(6,11), new Position(6,10), new Position(6,9), new Position(6,8),
            new Position(6,7), new Position(7,7), new Position(8,7), new Position(9,7),
            new Position(10,7), new Position(10,6), new Position(10,5), new Position(9,5),
            new Position(8,5), new Position(7,5), new Position(6,5), new Position(6,4),
            new Position(6,3), new Position(6,2), new Position(6,1), new Position(5,1)
        };

        public Position[][] nestPositions = new Position[][]
        {
            new Position[] { new Position(1,2), new Position(1,3), new Position(2,2), new Position(2,3) },
            new Position[] { new Position(1,9), new Position(1,10), new Position(2,9), new Position(2,10) },
            new Position[] { new Position(8,9), new Position(8,10), new Position(9,9), new Position(9,10) },
            new Position[] { new Position(8,2), new Position(8,3), new Position(9,2), new Position(9,3) }
        };

        public Position[] startPositions = new Position[]
        {
            new Position(4,1), new Position(0,7), new Position(6,11), new Position(10,5)
        };

        public Position[][] endPositions = new Position[][]
        {
            new Position[] { new Position(5,2), new Position(5,3), new Position(5,4), new Position(5,5) },
            new Position[] { new Position(1,6), new Position(2,6), new Position(3,6), new Position(4,6) },
            new Position[] { new Position(5,10), new Position(5,9), new Position(5,8), new Position(5,7) },
            new Position[] { new Position(9,6), new Position(8,6), new Position(7,6), new Position(6,6) }
        };

        public Position goalPosition = new Position(5, 6);

        // Ny logik för att hålla reda på spelarpjäser och deras positioner.
        private void InitializePieces(int numberOfPlayers)
        {
            playerPieces = new Dictionary<int, List<Ellipse>>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                SolidColorBrush color = colors[i];
                playerPieces[i] = new List<Ellipse>();

                for (int j = 0; j < 4; j++)
                {
                    Ellipse placeholderPiece = new Ellipse
                    {
                        Fill = color,
                        Stroke = new SolidColorBrush(Windows.UI.Colors.White),
                        StrokeThickness = 3,
                        Width = 40,
                        Height = 40
                    };
                    Grid.SetColumn(placeholderPiece, nestPositions[i][j].ColumnIndex);
                    Grid.SetRow(placeholderPiece, nestPositions[i][j].RowIndex);

                    GameGrid.Children.Add(placeholderPiece);
                    playerPieces[i].Add(placeholderPiece);
                }
            }
        }

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

        private void MoveCurrentPiece()
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
        }

        // Exempel på att flytta pjäserna när sidan klickas
        private void GameGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            MoveCurrentPiece();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustGridSize();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustGridSize();
        }

        private void AdjustGridSize()
        {
            double availableWidth = this.ActualWidth;
            double availableHeight = this.ActualHeight;
            double gridSize = Math.Min(availableWidth, availableHeight);

            GameGrid.Width = gridSize;
            GameGrid.Height = gridSize;
        }
    }
}
