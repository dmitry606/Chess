using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess.Utils;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Chess.Models
{
	public enum Color
	{
		White = 1,
		Black = 2
	}

	public class Board : ICloneable
	{
		private Player _white;
		private Player _black;

		public string Id { get; set; }
		public string Caption { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime LastModifiedAt { get; set; }

		public List<HistoryEntry> History { get; set; }

		public Player White
		{
			get { return _white; }
			set
			{
				value.Board = this;
				_white = value;
			}
		}

		public Player Black
		{
			get { return _black; }
			set
			{
				value.Board = this;
				_black = value;
			}
		}

		public bool IsOpen
		{
			get
			{
				var h = PeekHistory();
				return h == null || h.ResultingEvent == null || h.ResultingEvent == GameEvent.Check;
			}
		}

		public Player this[Color playerColor] => playerColor == Color.White ? White : Black;

		public CellHandle this[string position]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public CellHandle this[Piece piece] => this[piece.Position];

		public Board()
		{
			White = new Player();
			Black = new Player();
			History = new List<HistoryEntry>();
		}

		public BoardMatrix GetMatrix()
		{
			throw new NotImplementedException();
		}

		public void PushHistory(HistoryEntry move)
		{
			History.Add(move);
		}

		public HistoryEntry PeekHistory()
		{
			return History.LastOrDefault();
		}

		public HistoryEntry GetLastEntryForCell(string position)
		{
			throw new NotImplementedException();
		}

		#region Equals
		public bool Equals(Board other)
		{
			if (null == other)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return
				Equals(Id, other.Id) &&
				Equals(Caption, other.Caption) &&
				CreatedAt.ToString() == other.CreatedAt.ToString() &&
				LastModifiedAt.ToString() == other.LastModifiedAt.ToString() &&

				Equals(White, other.White) &&
				Equals(Black, other.Black) &&

				History.SequenceEqual(other.History);
		}
		public override bool Equals(object obj) => Equals(obj as Board);

		public override int GetHashCode()
		{
			int hash = 19;
			hash = hash * 31 + Id?.GetHashCode() ?? 0;
			hash = hash * 31 + CreatedAt.GetHashCode();
			hash = hash * 31 + LastModifiedAt.GetHashCode();
			hash = hash * 31 + Caption?.GetHashCode() ?? 0;
            hash = hash * 31 + White.GetHashCode();
			hash = hash * 31 + Black.GetHashCode();
			hash = hash * 31 + History.SequenceGetHashCode();

			return hash;
		}
		#endregion

		public Board Clone()
		{
			return (Board)((ICloneable)this).Clone();
		}

		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		public class CellHandle
		{
			public CellHandle(Board owner)
			{

			}

			public void Remove()
			{
				throw new NotImplementedException();
			}

			public void Move(string dest, char? promotionPiece = null)
			{
				//board[piece.Color].RemovePieceAt(piece.Position);
				//board[piece.Color].AddPiece(promotionPiece ?? piece.CharType, option.Destination);
				//board[piece.Color.Invert()].RemovePieceAt(option.Destination);


				throw new NotImplementedException();
			}
		}
	}
}

