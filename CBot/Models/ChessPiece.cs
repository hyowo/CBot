using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBot.Models
{
	public enum ChessPieceType
	{
		King,
		Queen,
		Rook,
		Bishop,
		Knight,
		Pawn,
		None
	}

	public enum ChessPieceColor
	{
		White,
		Black,
		None
	}

	public class ChessPiece : INotifyPropertyChanged
	{
		private ChessPieceType _type;
		public ChessPieceType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged(nameof(Type));
			}
		}

		private ChessPieceColor _color;
		public ChessPieceColor Color
		{
			get { return _color; }
			set
			{
				_color = value;
				OnPropertyChanged(nameof(Color));
			}
		}

		private int _row;
		public int Row
		{
			get { return _row; }
			set
			{
				_row = value;
				OnPropertyChanged(nameof(Row));
			}
		}

		private int _column;
		public int Column
		{
			get { return _column; }
			set
			{
				_column = value;
				OnPropertyChanged(nameof(Column));
			}
		}

		public ChessPiece()
		{
			Type = ChessPieceType.None;
			Color = ChessPieceColor.None;
			Row = 0;
			Column = 0;
		}

		public ChessPiece(ChessPieceType type, ChessPieceColor color, int row, int column)
		{
			Type = type;
			Color = color;
			Row = row;
			Column = column;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
