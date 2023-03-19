using CBot.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CBot.ViewModels
{
	public partial class ChessMenuViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private static readonly List<ChessPiece> StartingPositions = new() {
			new(ChessPieceType.Rook, ChessPieceColor.Black, 1, 1),
			new(ChessPieceType.Knight, ChessPieceColor.Black, 1, 2),
			new(ChessPieceType.Bishop, ChessPieceColor.Black, 1, 3),
			new(ChessPieceType.Queen, ChessPieceColor.Black, 1, 4),
			new(ChessPieceType.King, ChessPieceColor.Black, 1, 5),
			new(ChessPieceType.Bishop, ChessPieceColor.Black, 1, 6),
			new(ChessPieceType.Knight, ChessPieceColor.Black, 1, 7),
			new(ChessPieceType.Rook, ChessPieceColor.Black, 1, 8),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 1),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 2),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 3),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 4),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 5),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 6),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 7),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 8),
			new(ChessPieceType.Pawn, ChessPieceColor.Black, 2, 8),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 1),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 2),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 3),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 4),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 5),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 6),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 7),
			new(ChessPieceType.Pawn, ChessPieceColor.White, 7, 8),
			new(ChessPieceType.Rook, ChessPieceColor.White, 8, 1),
			new(ChessPieceType.Knight, ChessPieceColor.White, 8, 2),
			new(ChessPieceType.Bishop, ChessPieceColor.White, 8, 3),
			new(ChessPieceType.Queen, ChessPieceColor.White, 8, 4),
			new(ChessPieceType.King, ChessPieceColor.White, 8, 5),
			new(ChessPieceType.Bishop, ChessPieceColor.White, 8, 6),
			new(ChessPieceType.Knight, ChessPieceColor.White, 8, 7),
			new(ChessPieceType.Rook, ChessPieceColor.White, 8, 8)
		};

		private ObservableChessPieceCollection? board;

		public ObservableChessPieceCollection Board
		{
			get => board;
			set
			{
				board = value;
				OnPropertyChanged(nameof(Board));
			}
		}

		public ChessMenuViewModel()
		{
			Board = new();
		}

		[RelayCommand]
		public void ResetPositions()
		{
			Board.RemoveAll();
			foreach (ChessPiece piece in StartingPositions)
				Board.Add(piece);
		}

		[RelayCommand]
		public void OnlyKings()
		{
			Board.RemoveAll();
			foreach (ChessPiece piece in StartingPositions)
			{
				if (piece.Type == ChessPieceType.King)
					Board.Add(piece);
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
