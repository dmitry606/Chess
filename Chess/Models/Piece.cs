using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public abstract class Piece
	{
		public const char King = 'K';
		public const char Queen = 'Q';
		public const char Rook = 'R';
		public const char Bishop = 'B';
		public const char Knight = 'N';
		public const char Pawn = 'p';

		public abstract char CharType { get; }
		public string Position { get; }
		public Color Color { get; }
		public string PieceString => ChessUtil.ComposePieceString(CharType, Position);

		public abstract List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix);

		protected Piece(Color color, string pieceString)
		{
			if (pieceString.Length != 3)
				throw new ArgumentException("Unexpected: " + pieceString, nameof(pieceString));

			if (ChessUtil.ExtractCharType(pieceString) != CharType)
				throw new ArgumentException($"Can't create '{CharType}' from '{pieceString}'", nameof(pieceString));

			Position = ChessUtil.ExtractPosition(pieceString);
			if(!BoardMatrix.IsValidCoords(Position))
				throw new ArgumentException("Invalid position: " + pieceString, nameof(pieceString));
			Color = color;
		}

		protected void GoLongDistance(BoardMatrix boardMatrix, Func<int, Tuple<int, int>> iter, List<RawOption> result)
		{
			for (int i = 1; i < 8; i++)
			{
				var current = iter(i);
				if (!AddIfValid(boardMatrix, current, result))
					break;
			}
		}

		protected bool AddIfValid(BoardMatrix boardMatrix, Tuple<int, int> coords, List<RawOption> result)
		{
			if (!BoardMatrix.IsValidCoords(coords) || boardMatrix[coords] == Color)
				return false;

			var position = BoardMatrix.ConvertCoords(coords);
			var ev = boardMatrix[coords].HasValue ? EventType.Capture : EventType.Regular;
            result.Add(new RawOption(ev, position));

			return ev == EventType.Regular;
		}

		protected static Tuple<int, int> Add(Tuple<int, int> t, int first, int second)
		{
			return new Tuple<int, int>(t.Item1 + first, t.Item2 + second);
		}


		public override string ToString() => PieceString;
	}

	public class Bishop : Piece
	{
		public override char CharType => Bishop;

		public Bishop(Color color, string pieceString) : base(color, pieceString) { }

		public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<RawOption>();

			GoLongDistance(boardMatrix, i => Add(start, i, i), result);
			GoLongDistance(boardMatrix, i => Add(start, -i, -i), result);
			GoLongDistance(boardMatrix, i => Add(start, i, -i), result);
			GoLongDistance(boardMatrix, i => Add(start, -i, i), result);

			return result;
		}
	}

	public class Rook : Piece
	{
		public override char CharType => Rook;

		public Rook(Color color, string pieceString) : base(color, pieceString) { }

		public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<RawOption>();

			GoLongDistance(boardMatrix, i => Add(start, i, 0), result);
			GoLongDistance(boardMatrix, i => Add(start, 0, i), result);
			GoLongDistance(boardMatrix, i => Add(start, -i, 0), result);
			GoLongDistance(boardMatrix, i => Add(start, 0, -i), result);

			return result;
		}
	}

	public class Queen : Piece
	{
		public override char CharType => Queen;

		public Queen(Color color, string pieceString) : base(color, pieceString) { }

		public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<RawOption>();

			GoLongDistance(boardMatrix, i => Add(start, i, 0), result);
			GoLongDistance(boardMatrix, i => Add(start, 0, i), result);
			GoLongDistance(boardMatrix, i => Add(start, -i, 0), result);
			GoLongDistance(boardMatrix, i => Add(start, 0, -i), result);

			GoLongDistance(boardMatrix, i => Add(start, i, i), result);
			GoLongDistance(boardMatrix, i => Add(start, -i, -i), result);
			GoLongDistance(boardMatrix, i => Add(start, i, -i), result);
			GoLongDistance(boardMatrix, i => Add(start, -i, i), result);

			return result;
		}
	}

	public class King : Piece
	{
		public override char CharType => King;

		public King(Color color, string pieceString) : base(color, pieceString) { }

		public King(Player player) 
			:base(player.Color, player.Pieces.SingleOrDefault(s => s[0] == King))
		{
		}

		public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);
			var result = new List<RawOption>();

			Action<int, int> add = (iInc, jInc) => AddIfValid(boardMatrix, Add(start, iInc, jInc), result);

			add(0, 1);
			add(1, 0);
			add(0, -1);
			add(-1, 0);

			add(1, 1);
			add(-1, -1);
			add(1, -1);
			add(-1, 1);

			if(Color == Color.Black && Position == "e8")
			{
				if(boardMatrix.AreEmpty("f8", "g8"))
				{
					result.Add(new RawOption(EventType.Castling, "g8"));
				}

				if (boardMatrix.AreEmpty("b8", "c8", "d8"))
				{
					result.Add(new RawOption(EventType.Castling, "c8"));
				}
            }
			else if (Color == Color.White && Position == "e1")
			{
				if (boardMatrix.AreEmpty("f1", "g1"))
				{
					result.Add(new RawOption(EventType.Castling, "g1"));
				}

				if (boardMatrix.AreEmpty("b1", "c1", "d1"))
				{
					result.Add(new RawOption(EventType.Castling, "c1"));
				}
			}

			return result;
		}
	}

	public class Knight : Piece
	{
		public override char CharType => Knight;

		public Knight(Color color, string pieceString) : base(color, pieceString) { }

		public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<RawOption>();

			AddIfValid(boardMatrix, Add(start, 2, 1), result);
			AddIfValid(boardMatrix, Add(start, 2, -1), result);
			AddIfValid(boardMatrix, Add(start, -2, 1), result);
			AddIfValid(boardMatrix, Add(start, -2, -1), result);

			AddIfValid(boardMatrix, Add(start, 1, 2), result);
			AddIfValid(boardMatrix, Add(start, 1, -2), result);
			AddIfValid(boardMatrix, Add(start, -1, 2), result);
			AddIfValid(boardMatrix, Add(start, -1, -2), result);

			return result;
		}
	}

	public class Pawn : Piece
	{
		public override char CharType => Pawn;

		public Pawn(Color color, string pieceString) : base(color, pieceString) { }

		public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);
			var dir = Color == Color.Black ? +1 : -1;

			var result = new List<RawOption>();

			if ((Color == Color.Black)? (start.Item1 <= 6) : (start.Item1 >= 1))
			{
				var nextRegular = Add(start, dir * 1, 0);
				if (null == boardMatrix[nextRegular])
				{
					result.Add(new RawOption(
						EventType.Regular, BoardMatrix.ConvertCoords(nextRegular)));
				}

				var capture = Add(start, dir * 1, 1);
				var opponentColor = Color == Color.White ? Color.Black : Color.White;
                if (boardMatrix[capture] == opponentColor)
				{
					result.Add(new RawOption(
						EventType.Capture, BoardMatrix.ConvertCoords(capture)));
				}

				capture = Add(start, dir * 1, -1);
				if (boardMatrix[capture] == opponentColor)
				{
					result.Add(new RawOption(
						EventType.Capture, BoardMatrix.ConvertCoords(capture)));
				}
			}

			if (start.Item1 == (Color == Color.Black? 1 : 6))
			{
				var nextRegular = Add(start, dir * 2, 0);
				if (null == boardMatrix[nextRegular])
					result.Add(new RawOption(
						EventType.Regular, BoardMatrix.ConvertCoords(nextRegular)));
			}

			return result;
		}
	}

}