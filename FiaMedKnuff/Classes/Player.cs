﻿using System;
using System.Collections.Generic;
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

namespace FiaMedKnuff
{
    public class Player
    {
        /// <summary>
        /// Gets or sets the player ID which must be a number between 1 and 4
        /// </summary>
        private int playerId;
        private int PlayerId
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
        /// Gets or sets the player color
        /// </summary>
        private string color;
        private string Color
        {
            get { return color; }
            set { color = value; } // TODO: Add logic to change player color and decide the format (HEX?). Maybe this is done somewhere else in the program?
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
            Color = color;

            if (startingPositions.Length != 4)
                throw new ArgumentException("A player needs 4 starting positions for his game pieces");

            for (int i = 0; i < startingPositions.Length; i++)
            {
                //This determines the shape of the game pieces
                Ellipse placeholderPiece01 = new Ellipse
                {
                    //Fill = color,
                    Fill = new SolidColorBrush(Windows.UI.Colors.Black),
                    Stroke = new SolidColorBrush(Windows.UI.Colors.White),
                    StrokeThickness = 3,
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
        public void MoveGamePiece(int id, int diceRoll, Position[] position)
        {
            foreach (GamePiece piece in pieces)
            {
                //Find the correct game piece and check that it does not move more than 45 steps
                if (piece.Id == id && piece.StepsTaken + diceRoll <= 45)
                {
                    piece.StepsTaken += diceRoll;
                    piece.Position = position[piece.StepsTaken - 1];
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
        /// One method disables the pieces from getting clicked and the other one enables them instead
        /// </summary>
        public void enableGamePieces()
        {
            foreach (GamePiece piece in pieces)
            {
                piece.GamePieceShape.IsTapEnabled = true;
            }
        }
		public void disableGamePieces()
		{
			foreach (GamePiece piece in pieces)
			{
				piece.GamePieceShape.IsTapEnabled = false;
			}
		}
	}
}