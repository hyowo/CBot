using CBot.Models;
using CBot.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				foreach (ChessPiece chp in e.NewItems)
				{
					chp.PropertyChanged += Piece_PositionChanged;
					BitmapImage pieceImage = new();
					pieceImage.BeginInit();
					pieceImage.UriSource = new Uri($"pack://application:,,,/CBot;component/Resources/Images/{((chp.Color == ChessPieceColor.Black) ? 'b':'w')}{GetChessPieceLetter(chp.Type)}.png");
					pieceImage.EndInit();
					RenderOptions.SetBitmapScalingMode(pieceImage, BitmapScalingMode.NearestNeighbor);
					chp.Image = new()
					{
						Source = pieceImage
					};
					foreach (var element in ChessBoard.Children)
					{
						if (element is Border && (element as Border).Name == GetChessSquareFromCoords(chp.Row, chp.Column))
						{
							(element as Border).Child = chp.Image;
							break;
						}
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (ChessPiece chp in e.OldItems)
				{
					chp.PropertyChanged -= Piece_PositionChanged;
					foreach (var element in ChessBoard.Children)
					{
						if (element is Border && (element as Border).Name == GetChessSquareFromCoords(chp.Row, chp.Column))
						{
							(element as Border).Child = null;
							break;
						}
					}
				}
			}
		}

		private void Piece_PositionChanged(object sender, PropertyChangedEventArgs e)
		{
			var _sender = sender as ChessPiece;
			var maybeOver = ChessMenuViewModel.Board.Where(x => x.Column == _sender.Column && x.Row == _sender.Row && x != _sender).First();
			if (maybeOver != null)
				ChessMenuViewModel.Board.Remove(maybeOver);
			foreach (var element in ChessBoard.Children)
			{
				if (element is Border && (element as Border).Name == GetChessSquareFromCoords(_sender.PreviousRow, _sender.PreviousColumn))
				{
					(element as Border).Child = null;
					foreach (var element2 in ChessBoard.Children)
					{
						if (element2 is Border && (element2 as Border).Name == GetChessSquareFromCoords(_sender.Row, _sender.Column))
						{
							(element2 as Border).Child = _sender.Image;
							break;
						}
					}
					break;
				}
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

		public static string GetChessSquareFromCoords(int row, int col)
		{
			return $"{(char)('A' + col-1)}{8-row+1}";
		}

		public static int[] GetCoordsFromChessSquare(string chessSquare)
		{
			// Convert the file character to a column number
			int col = chessSquare[0] - 'A' + 1;

			// Convert the rank number to a row number
			int row = 8 - (chessSquare[1] - '1');

			// Return the row and column as an integer array
			return new int[] { row, col };
		}


		public static char GetChessPieceLetter(ChessPieceType pieceType)
		{
			return pieceType switch
			{
				ChessPieceType.King => 'K',
				ChessPieceType.Queen => 'q',
				ChessPieceType.Rook => 'R',
				ChessPieceType.Bishop => 'B',
				ChessPieceType.Knight => 'N',
				ChessPieceType.Pawn => 'P',
				_ => throw new NotImplementedException(),
			};
		}


		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ChessMenuViewModel.ResetPositions();
			//var children = ChessBoard.Children.Cast<Border>().ToList();
			//children.Reverse();

			//ChessBoard.Children.Clear();
			//foreach (var child in children)
			//{
			//	ChessBoard.Children.Add(child);
			//}
		}

		private Point originalPosition;

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Get the image element that was clicked
			Image image = sender as Image;

			// Store the original position of the image element
			originalPosition = e.GetPosition(null);

			// Set the cursor to the hand cursor to indicate dragging
			image.Cursor = Cursors.Hand;

			// Capture the mouse to the image element to track the dragging operation
			image.CaptureMouse();
		}

		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// Get the image element that was clicked
			Image image = sender as Image;

			image.Visibility = Visibility.Collapsed;

			// Release the mouse capture and reset the cursor
			image.ReleaseMouseCapture();
			image.Cursor = Cursors.Arrow;

			// Reset the position of the image element to the original position
			double left = Math.Max(0, Math.Min(Canvas.GetLeft(image), image.ActualWidth - image.ActualWidth));
			double top = Math.Max(0, Math.Min(Canvas.GetTop(image), image.ActualHeight - image.ActualHeight));
			image.SetValue(Canvas.LeftProperty, left);
			image.SetValue(Canvas.TopProperty, top);

			var pieceToPlace = image.Name;

			image.Visibility = Visibility.Visible;

			if (Mouse.DirectlyOver is UIElement element && element.GetType() == typeof(Border))
			{
				var whereToPlace = element as Border;
				var coords = GetCoordsFromChessSquare(whereToPlace.Name);
				PlaceNewPiece(coords[0], coords[1], pieceToPlace);
			}
		}

		private void PlaceNewPiece(int row, int col, string piece)
		{
			var pieceType = piece[1] switch
			{
				'K' => ChessPieceType.King,
				'Q' => ChessPieceType.Queen,
				'R' => ChessPieceType.Rook,
				'B' => ChessPieceType.Bishop,
				'N' => ChessPieceType.Knight,
				'P' => ChessPieceType.Pawn,
				_ => throw new ArgumentException("Invalid piece type."),
			};
			var pieceColor = piece[0] switch
			{
				'w' => ChessPieceColor.White,
				'b' => ChessPieceColor.Black,
				_ => throw new ArgumentException("Invalid piece color."),
			};
			ChessMenuViewModel.Board.Add(new ChessPiece(pieceType, pieceColor, row, col));
		}

		private void Image_MouseMove(object sender, MouseEventArgs e)
		{
			// Get the image element that is being dragged
			Image image = sender as Image;

			// Check if the image element is being dragged
			if (image.IsMouseCaptured)
			{
				// Get the current position of the mouse
				Point currentPosition = e.GetPosition(null);

				// Calculate the offset from the original position
				Vector offset = currentPosition - originalPosition;

				// Update the position of the image element
				image.SetValue(Canvas.LeftProperty, offset.X);
				image.SetValue(Canvas.TopProperty, offset.Y);
			}
		}
	}
}
