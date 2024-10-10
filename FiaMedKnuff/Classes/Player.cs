using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
	public class Player
	{
		/// <summary>
		/// Gets or sets the player ID which must be a number between 1 and 4
		/// </summary>
		private int playerId;
		public int PlayerId 
		{
			get { return playerId; }
			set 
			{ 
				if (1 <= value && value <= 4 )
					playerId = value;
				else
					throw new ArgumentOutOfRangeException("Player ID can only be a number between 1 and 4");
			}
		}

		/// <summary>
		/// Gets or sets the player color
		/// </summary>
		private string color;
		public string Color
		{
			get { return color; }
			set { color = value; } // TODO: Add logic to change player color and decide the format (HEX?). Maybe this is done somewhere else in the program?
		}

		/// <summary>
		/// Gets or sets an array of game pieces for the player
		/// </summary>
		private GamePiece[] pieces = new GamePiece[4];
		public GamePiece[] Pieces 
		{ 
			get { return pieces; }
			set 
			{
				if (value.Length != 4)
					throw new ArgumentException("A player needs to have 4 game pieces");
				else 
					pieces = value;
			}
		}

		/// <summary>
		/// Constructor for player with 3 parameters
		/// </summary>
		/// <param name="playerId"> ID for the player (1-4) </param>
		/// <param name="color"> Color for the player </param>
		/// <param name="gamePieces"> An array with game 4 game pieces for the player </param>
		public Player(int playerId, string color, GamePiece[] gamePieces)
		{
			PlayerId = playerId;
			Color = color;

			if (startingPositions.Length != 4)
				throw new ArgumentException("A player needs 4 starting positions for his game pieces");
			else 
			{
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
						Height = 40
					};

					pieces[i] = new GamePiece(i+1, placeholderPiece01, startingPositions[i]);
					Grid.SetRow(pieces[i].GamePieceShape, pieces[i].Position.RowIndex);
					Grid.SetColumn(pieces[i].GamePieceShape, pieces[i].Position.ColumnIndex);
				}
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
				if(piece.Id == id)
					return piece.GamePieceShape;
			}
			throw new ArgumentException("Game piece shape not found");
		}

		/// <summary>
		/// Moves the specified game pieces
		/// </summary>
		/// <param name="id"> ID for specified gamepiece </param>
		/// <param name="diceRoll"> Dice roll from 1-6 </param>
		/// <param name="position"> Position array of possible "tiles" </param>
		public void MoveGamePiece(int id, int diceRoll, Position[] position)
		{
			foreach(GamePiece piece in pieces)
			{
				if (piece.Id == id)
				{
                    piece.StepsTaken += diceRoll;
					piece.Position = position[piece.StepsTaken-1];
					Grid.SetRow(piece.GamePieceShape, piece.Position.RowIndex);
					Grid.SetColumn(piece.GamePieceShape, piece.Position.ColumnIndex);
                }
			}
		}
	}
}
