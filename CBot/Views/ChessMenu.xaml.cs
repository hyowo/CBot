using CBot.Models;
using CBot.ViewModels;
using System;
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
		private readonly ChessMenuViewModel chessMenuViewModel;
		public ChessMenu()
		{
			InitializeComponent();
			chessMenuViewModel = new();
			DataContext = chessMenuViewModel;

			chessMenuViewModel.Board.CollectionChanged += Board_CollectionChanged;

			InitializeChessBoard();
		}

		// shit i broke it :(

		private void Board_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				foreach (ChessPiece chp in e.NewItems)
				{
					var alreadyExist = (sender as ObservableChessPieceCollection).Where(x => x.Column == chp.Column && x.Row == chp.Row);
					if (alreadyExist.Any())
					{
						(sender as ObservableChessPieceCollection).Remove(alreadyExist.First());
					}

					BitmapImage pieceImage = new();
					pieceImage.BeginInit();
					pieceImage.UriSource = new Uri($"pack://application:,,,/CBot;component/Resources/Images/{((chp.Color == ChessPieceColor.Black) ? 'b':'w')}{GetChessPieceLetter(chp.Type)}.png");
					pieceImage.EndInit();
					RenderOptions.SetBitmapScalingMode(pieceImage, BitmapScalingMode.NearestNeighbor);
					chp.Image = new()
					{
						Source = pieceImage
					};
					chp.Image.MouseLeftButtonUp += PieceDrag_MouseUp;
					chp.Image.MouseLeftButtonDown += PieceDrag_MouseDown;
					chp.Image.MouseMove += PieceDrag_MouseMove;
					foreach (var element in ChessBoard.Children)
					{
						if (element is Border && (element as Border).Name == GetChessSquareFromCoords(chp.Row, chp.Column))
						{
							chp.PropertyChanged += Piece_PositionChanged;
							((element as Border).Child as Canvas).Children.Add(chp.Image);
							break;
						}
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (ChessPiece chp in e.OldItems)
				{
					foreach (var element in ChessBoard.Children)
					{
						if (element is Border && (element as Border).Name == GetChessSquareFromCoords(chp.Row, chp.Column))
						{
							chp.PropertyChanged -= Piece_PositionChanged;
							((element as Border).Child as Canvas).Children.Clear();
							break;
						}
					}
				}
			}
		}

		private void Piece_PositionChanged(object sender, PropertyChangedEventArgs e)
		{
			var _sender = sender as ChessPiece;
			var maybeOver = chessMenuViewModel.Board.Where(x => x.Column == _sender.Column && x.Row == _sender.Row && x != _sender).First();
			if (maybeOver != null)
				chessMenuViewModel.Board.Remove(maybeOver);
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
				border.Child = new Canvas();
				ChessBoard.Children.Add(border);
			}
			//chessMenuViewModel.ResetPositions();
		}

		public static string GetChessSquareFromInt(int squareIndex)
		{
			int rank = squareIndex / 8;
			int file = squareIndex % 8;
			char fileChar = (char)('A' + file);
			int rankNum = 8 - rank;
			return $"{fileChar}{rankNum}";
		}

		public static string GetChessSquareFromCoords(int row, int col)
		{
			return $"{(char)('A' + col-1)}{8-row+1}";
		}

		public static int[] GetCoordsFromChessSquare(string chessSquare)
		{
			int col = chessSquare[0] - 'A' + 1;
			int row = 8 - (chessSquare[1] - '1');
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

		private Point originalPosition;

		private void PieceDrag_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Image image = sender as Image;
			originalPosition = e.GetPosition(null);
			image.Cursor = Cursors.Hand;
			image.CaptureMouse();
			image.SetValue(Panel.ZIndexProperty, 5);
		}

		private void PieceDrag_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Image image = sender as Image;

			image.Visibility = Visibility.Collapsed;
			image.ReleaseMouseCapture();
			image.Cursor = Cursors.Arrow;

			double left = Math.Max(0, Math.Min(Canvas.GetLeft(image), 0));
			double top = Math.Max(0, Math.Min(Canvas.GetTop(image), 0));
			image.SetValue(Canvas.LeftProperty, left);
			image.SetValue(Canvas.TopProperty, top);

			Console.WriteLine(VisualTreeHelper.GetParent((UIElement)Mouse.DirectlyOver).ToString());


			if (Mouse.DirectlyOver is Border whereToPlace)
			{
				var coords = GetCoordsFromChessSquare(whereToPlace.Name);
				PlaceNewPiece(coords[0], coords[1], image.Name);
			}
			else if (Mouse.DirectlyOver is Image element2)
			{
				if (VisualTreeHelper.GetParent(element2) is Canvas parent && VisualTreeHelper.GetParent(parent) is Border mainBorder)
				{
					if (mainBorder.Name.Length == 2 && mainBorder.Name[0] >= 'A' && mainBorder.Name[0] <= 'H' && mainBorder.Name[1] >= '1' && mainBorder.Name[1] <= '8')
					{
						if (VisualTreeHelper.GetParent(image) is Canvas dragparent && VisualTreeHelper.GetParent(dragparent) is Border dragMainBorder &&
						dragMainBorder.Name.Length == 2 && dragMainBorder.Name[0] >= 'A' && dragMainBorder.Name[0] <= 'H' &&
						dragMainBorder.Name[1] >= '1' && dragMainBorder.Name[1] <= '8')
						{
							var coords = GetCoordsFromChessSquare(dragMainBorder.Name);
							var coords2 = GetCoordsFromChessSquare(mainBorder.Name);
							Console.WriteLine($"Drag Main Border: {dragMainBorder.Name}\nmainBorder: {mainBorder.Name}");
							Console.WriteLine(chessMenuViewModel.Board.Count);
							//ChessMenuViewModel.Board.ReplacePiece(, coords2[0], coords2[1]);
						}
						else
						{
							var coords = GetCoordsFromChessSquare(mainBorder.Name);
							PlaceNewPiece(coords[0], coords[1], image.Name);
						}
					}
				}
			}

			image.Visibility = Visibility.Visible;
		}

		private void PieceDrag_MouseMove(object sender, MouseEventArgs e)
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

		private void Rotate_Click(object sender, RoutedEventArgs e)
		{
			var children = ChessBoard.Children.Cast<Border>().ToList();
			children.Reverse();

			ChessBoard.Children.Clear();
			foreach (var child in children)
			{
				ChessBoard.Children.Add(child);
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
			var foundPiece = chessMenuViewModel.Board.Where(x => x.Row == row && x.Column == col);
			if (foundPiece != null)
				chessMenuViewModel.Board.Add(new ChessPiece(pieceType, pieceColor, row, col));
			else
			{
				var foundPiece2 = foundPiece as ChessPiece;
				foundPiece2.Type = pieceType;
				foundPiece2.Color = pieceColor;
			}
			Console.WriteLine(chessMenuViewModel.Board.Count);
		}
	}
}
