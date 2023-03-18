using CBot.Models;
using CBot.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CBot.Views
{
	public partial class ChessMenu : Window
	{
		private ChessMenuViewModel ChessMenuViewModel { get; set; }
		public ChessMenu()
		{
			InitializeComponent();
			ChessMenuViewModel = new();
			DataContext = ChessMenuViewModel;

			ChessMenuViewModel.Board.CollectionChanged += Board_CollectionChanged;

			InitializeChessBoard();
		}

		private void Board_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems.Count > 0)
			{
				MessageBox.Show((e.NewItems[0] as ChessPiece).Color.ToString());
			}
		}

		private void ChessMenuViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Check if the changed property is Board
			if (e.PropertyName == "")
			{
				// Update the border images
				InitializeChessBoard();
			}
		}

		public void InitializeChessBoard()
		{
			ChessBoard.Children.Clear();
			for (int i = 0; i < 64; i++)
			{
				Border border = new();
				int row = i / 8;
				if ((row % 2 == 0 && i % 2 == 0) || (row % 2 == 1 && i % 2 == 1))
					border.Background = new SolidColorBrush(Color.FromArgb(200, 96, 43, 138));
				else
					border.Background = new SolidColorBrush(Color.FromArgb(200, 38, 8, 61));
				border.Name = GetChessSquareFromInt(i);
				border.Child = new TextBlock() { Text = border.Name };
				ChessBoard.Children.Add(border);
			}
		}


		public static string GetChessSquareFromInt(int squareIndex)
		{
			int rank = squareIndex / 8;  // calculate the rank (row) number
			int file = squareIndex % 8;  // calculate the file (column) number
			char fileChar = (char)('A' + file);  // convert the file number to a character (A to H)
			int rankNum = 8 - rank;  // convert the rank number to a number (1 to 8)
			return $"{fileChar}{rankNum}";  // return the square in the format of H8 to A1
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ChessMenuViewModel.Board.Add(new ChessPiece(ChessPieceType.Pawn, ChessPieceColor.Black, 5, 6));
			//var children = ChessBoard.Children.Cast<Border>().ToList();
			//children.Reverse();

			//ChessBoard.Children.Clear();
			//foreach (var child in children)
			//{
			//	ChessBoard.Children.Add(child);
			//}
		}
	}
}
