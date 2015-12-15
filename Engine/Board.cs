using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Engine
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

		public Board()
		{
			White = new Player();
			Black = new Player();
			History = new List<HistoryEntry>();
		}

		internal CellHandle this[string position] => GetCellHandle(position, null);
		internal CellHandle this[Piece piece] => GetCellHandle(piece.PieceString, piece.Color);

		internal BoardMatrix GetMatrix()
		{
			Color?[,] matrix = new Color?[8, 8];
			foreach (var ps in White.PieceStrings)
			{
				var coords = BoardMatrix.ConvertToTupleCoords(ps.Substring(1, 2));
				matrix[coords.Item1, coords.Item2] = Color.White;
			}
			foreach (var ps in Black.PieceStrings)
			{
				var coords = BoardMatrix.ConvertToTupleCoords(ps.Substring(1, 2));
				matrix[coords.Item1, coords.Item2] = Color.Black;
			}

			return new BoardMatrix { M = matrix };
		}

		public void PushHistory(HistoryEntry move)
		{
			History.Add(move);
		}

		public HistoryEntry PeekHistory()
		{
			return History.LastOrDefault();
		}

		public Color GetLastMovePlayerColor()
		{
			return History.Count % 2 == 0 ? Color.Black : Color.White;
		}

		public HistoryEntry GetLastEntryForCell(string position)
		{
			if (null == position || position.Length != 2)
				throw new ArgumentException();

			return History.LastOrDefault(e =>
				e.Move.Destination == position ||
				ChessUtil.ExtractPosition(e.PieceString) == position);
		}

		#region Equals
		public bool Equals(Board other)
		{
			if (null == other)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return
				Equals(White, other.White) &&
				Equals(Black, other.Black) &&

				History.SequenceEqual(other.History);
		}
		public override bool Equals(object obj) => Equals(obj as Board);

		public override int GetHashCode()
		{
			int hash = 19;
            hash = hash * 31 + White.GetHashCode();
			hash = hash * 31 + Black.GetHashCode();
			hash = hash * 31 + History.SequenceGetHashCode();

			return hash;
		}
		#endregion
		#region Cloneable
		public Board Clone()
		{
			return (Board)((ICloneable)this).Clone();
		}

		object ICloneable.Clone()
		{
			var clone = new Board
			{
				History = new List<HistoryEntry>(History),
			};

			clone.White = White.Clone();
			clone.Black = Black.Clone();

			return clone;
		}
		#endregion

		private CellHandle GetCellHandle(string position, Color? color)
		{
			if (string.IsNullOrEmpty(position) || !(position.Length == 2 || position.Length == 3))
				throw new ArgumentException($"Invalid position: '{position}'", nameof(position));

			string pieceString;
			if(null != color)
			{
				pieceString = this[color.Value].PieceStrings.FirstOrDefault(p => p.EndsWith(position));
			}
			else
			{
				pieceString = White.PieceStrings.FirstOrDefault(p => p.EndsWith(position));
				if(null == pieceString)
				{
					pieceString = Black.PieceStrings.FirstOrDefault(p => p.EndsWith(position));
					color = Color.Black;
				}
				else
				{
					color = Color.White;
				}
            }

			if(null == pieceString)
				throw new ArgumentException($"Could not find: '{position}'", nameof(position));

			return new CellHandle(this, color.Value, pieceString);
		}


		internal class CellHandle
		{
			public string PieceString { get; }
			public Color Color { get; }
			public Board Board { get; }

			public Piece Piece => PieceFactory.Create(Color, PieceString);

			private bool _used = false;

			public CellHandle(Board owner, Color color, string pieceString)
			{
				Board = owner;
				Color = color;
				PieceString = pieceString;
			}

			public void Remove()
			{
				if (_used)
					throw new InvalidOperationException("Used");
				Board[Color].PieceStrings.Remove(PieceString);
				_used = true;
			}

			public void Move(string dest, char? promotionPiece = null)
			{
				if (_used)
					throw new InvalidOperationException("Used");
				if(Board[Color].PieceStrings.Any(p => p.EndsWith(dest)))
					throw new InvalidOperationException($"{dest} is already occupied");

				var targetPieceString = ChessUtil.ComposePieceString(promotionPiece ?? PieceString[0], dest);
                Board[Color].PieceStrings.Remove(PieceString);
				Board[Color].PieceStrings.Add(targetPieceString);
				var opponentPiece = Board[Color.Invert()].PieceStrings.FirstOrDefault(p => p.EndsWith(dest));
				if (null != opponentPiece)
					Board[Color.Invert()].PieceStrings.Remove(opponentPiece);
				_used = true;
			}
		}
	}
}

