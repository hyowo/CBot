using CBot.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CBot.ViewModels
{
	public partial class ChessMenuViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private ObservableChessPieceCollection board;

		public ObservableChessPieceCollection Board
		{
			get { return board; }
			set
			{
				board = value;
				OnPropertyChanged(nameof(Board));
			}
		}

		public ChessMenuViewModel()
		{
			Board = new ObservableChessPieceCollection();
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Board.Add(new ChessPiece(ChessPieceType.None, ChessPieceColor.None, i, j));
				}
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
