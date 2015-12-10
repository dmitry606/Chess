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

		public abstract List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix);

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

		protected void GoLongDistance(BoardMatrix boardMatrix, Func<int, Tuple<int, int>> iter, List<MoveOption> result)
		{
			for (int i = 1; i < 8; i++)
			{
				var current = iter(i);
				if (!AddIfValid(boardMatrix, current, result))
					break;
			}
		}

		protected bool AddIfValid(BoardMatrix boardMatrix, Tuple<int, int> coords, List<MoveOption> result)
		{
			if (!BoardMatrix.IsValidCoords(coords) || boardMatrix[coords] == Color)
				return false;

			var position = BoardMatrix.ConvertCoords(coords);
			var ev = boardMatrix[coords].HasValue ? MoveType.Capture : MoveType.Regular;
            result.Add(new MoveOption(ev, position));

			return ev == MoveType.Regular;
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

		public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<MoveOption>();

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

		public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<MoveOption>();

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

		public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<MoveOption>();

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

		public King(Player player) : base(player.Color, GetKingString(player))
		{
		}

		public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);
			var result = new List<MoveOption>();

			Action<int, int> add = (iInc, jInc) => AddIfValid(boardMatrix, Add(start, iInc, jInc), result);

			add(0, 1);
			add(1, 0);
			add(0, -1);
			add(-1, 0);

			add(1, 1);
			add(-1, -1);
			add(1, -1);
			add(-1, 1);

			var row = Color == Color.White ? "1" : "8";
			if (Position == "e" + row)
			{
				if (boardMatrix.AreEmpty("f" + row, "g" + row))
				{
					result.Add(new MoveOption(MoveType.Castling, "g" + row));
				}

				if (boardMatrix.AreEmpty("b" + row, "c" + row, "d" + row))
				{
					result.Add(new MoveOption(MoveType.Castling, "c" + row));
				}
            }

			return result;
		}

		public Tuple<string, string> GetRookCastlingFromAndToPos(MoveOption option)
		{
			var row = Color == Color.White ? "1" : "8";
			return option.Destination[0] == 'c' ?
				new Tuple<string, string>("a" + row, "d" + row) :
				new Tuple<string, string>("h" + row, "f" + row);
		}

		public List<string> GetKingCastlingRoute(MoveOption move)
		{
			var chars = move.Destination[0] == 'c' ?
				new List<string> { "e", "d", "c" } :
				new List<string> { "e", "f", "g" };

			var row = Color == Color.White ? "1" : "8";
			return chars.Select(c => c + row).ToList();
		}

		private static string GetKingString(Player player)
		{
			if (null == player)
			{
				throw new ArgumentNullException(nameof(player));
			}
			var str = player.PieceStrings.FirstOrDefault(s => s[0] == King);
			if (null == str)
			{
				throw new InvalidOperationException($"King was not found in {player.Color}");
			}
			return str;
		}
	}

	public class Knight : Piece
	{
		public override char CharType => Knight;

		public Knight(Color color, string pieceString) : base(color, pieceString) { }

		public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			var start = BoardMatrix.ConvertToTupleCoords(Position);
			Debug.Assert(boardMatrix[start] == Color);

			var result = new List<MoveOption>();

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

		//private int _enpassantRow;
		//private int _promotionRow;
		private bool _atPromotion;
		private bool _atEnpassant;
		private bool _atInitial;

		private Tuple<int, int> _start;

		public Pawn(Color color, string pieceString) : base(color, pieceString)
		{
			_start = BoardMatrix.ConvertToTupleCoords(Position);
			_atEnpassant = _start.Item1 == (Color == Color.Black ? 4 : 3);
			_atPromotion = _start.Item1 == (Color == Color.Black ? 6 : 1);
			_atInitial = _start.Item1 == (Color == Color.Black ? 1 : 6);
        }

		public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
		{
			Debug.Assert(boardMatrix[_start] == Color);
			var dir = Color == Color.Black ? +1 : -1;
			var result = new List<MoveOption>();

			if ((Color == Color.Black)? (_start.Item1 <= 6) : (_start.Item1 >= 1))
			{
				var nextRegular = Add(_start, dir * 1, 0);
				if (null == boardMatrix[nextRegular])
				{
					var move = new MoveOption(MoveType.Regular, BoardMatrix.ConvertCoords(nextRegular));
					if (_atPromotion)
						move.Secondary = SecondaryMoveType.Promotion;
                    result.Add(move);
				}
			}

			Capture(Add(_start, dir * 1, 1), boardMatrix, result);
			Capture(Add(_start, dir * 1, -1), boardMatrix, result);

			if (_atInitial)
			{
				var doubleMove = Add(_start, dir * 2, 0);
				if (null == boardMatrix[doubleMove])
					result.Add(new MoveOption(
						MoveType.Regular, BoardMatrix.ConvertCoords(doubleMove)));
			}

			return result;
		}

		private void Capture(Tuple<int, int> capture, BoardMatrix boardMatrix, List<MoveOption> result)
		{
			if (!BoardMatrix.IsValidCoords(capture))
				return;

			var opponentColor = Color.Invert();
			var move = new MoveOption(MoveType.Capture, BoardMatrix.ConvertCoords(capture));

			if (boardMatrix[capture] == opponentColor)
			{
				if (_atPromotion)
					move.Secondary = SecondaryMoveType.Promotion;

				result.Add(move);
			}
			else if (_atEnpassant && boardMatrix[capture] == null)
			{
				move.Secondary = SecondaryMoveType.EnPassant;
				move.SpecialCapturePosition = BoardMatrix.ConvertCoords(_start.Item1, capture.Item2);
				result.Add(move);
			}
		}
	}

}