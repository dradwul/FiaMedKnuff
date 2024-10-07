﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// Gets or sets the color for the game piece
		/// </summary>
		private string color;
		public string Color 
		{
			get { return color; }
			set { color = value; } // TODO: Add logic to change the color of the game piece and decide the format (HEX?). Maybe this is done somewhere else in the program?
		}

		/// <summary>
		/// Gets or sets number of steps taken for the game piece 
		/// Starts at 0 steps, 40 when it is at the entrance of the safe zone (can still lose the piece) and 45 when it has reached its goal
		/// </summary>
		private int stepsTaken;
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
		/// Constructor for a game piece
		/// </summary>
		/// <param name="id"> ID for the game piece (1-4) </param>
		/// <param name="color"> Color for the game piece </param>
		/// <param name="stepsTaken"> Amount of steps taken, starts at 0 </param>
		public GamePiece(int id, string color, int stepsTaken) 
		{ 
			Id = id;
			Color = color;
			StepsTaken = stepsTaken;
		}
	}
}
