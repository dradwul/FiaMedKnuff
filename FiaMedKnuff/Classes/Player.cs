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
			Pieces = gamePieces;
		}
	}
}
