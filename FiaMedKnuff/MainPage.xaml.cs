using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FiaMedKnuff
{

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
    {
		public MainPage()
		{
			this.InitializeComponent();

			//DELETE ME ----------------------------
			GamePiece[] player1Pieces = new GamePiece[]
			{
				new GamePiece(1, "blue", 0),
				new GamePiece(2, "blue", 0),
				new GamePiece(3, "blue", 0),
				new GamePiece(4, "blue", 0)
			};

			Player player1 = new Player(1, "blue", player1Pieces);

            Debug.WriteLine("Player ID: " + player1.PlayerId);
            Debug.WriteLine("Player color " + player1.Color);
			foreach (GamePiece piece in player1.Pieces)
			{
                Debug.WriteLine("Piece ID, color and steps taken: " + piece.Id + ", " + piece.Color + ", " + piece.StepsTaken);
			}
			//DELETE ME ----------------------------
		}
	}
}
