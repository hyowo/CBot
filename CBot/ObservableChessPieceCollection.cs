using CBot.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace CBot
{
	public static class CollectionExtensions
	{
		public static void RemoveAll(this IList list)
		{
			while (list.Count > 0)
			{
				list.RemoveAt(list.Count - 1);
			}
		}
	}

	public class ObservableChessPieceCollection : ObservableCollection<ChessPiece>
	{
		public ObservableChessPieceCollection() : base() { }

		public ObservableChessPieceCollection(IEnumerable<ChessPiece> collection) : base(collection) { }

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					if (item is INotifyPropertyChanged observableItem)
					{
						observableItem.PropertyChanged += OnItemPropertyChanged;
					}
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var item in e.OldItems)
				{
					if (item is INotifyPropertyChanged observableItem)
					{
						observableItem.PropertyChanged -= OnItemPropertyChanged;
					}
				}
			}
		}

		private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			OnCollectionChanged(args);
		}
	}

}
