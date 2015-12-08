using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public class HistoryEntry
	{
		public string PieceString { get; set; }
		public MoveOption Move { get; set; }
		public GameEvent? ResultingEvent { get; set; }

		public HistoryEntry()
		{
		}

		public HistoryEntry(string pieceString, string dest, MoveType moveType, SecondaryMoveType? sec = null)
		{
			if (pieceString == null || pieceString.Length != 3)
				throw new ArgumentException(pieceString, nameof(pieceString));

			PieceString = pieceString;
			Move = new MoveOption(moveType, dest);
			Move.Secondary = sec;
		}
    }

	public enum MoveType
	{
		Regular, //pos || not in check
		Capture, //pos, piece 
		Castling, //pos
	}

	public enum SecondaryMoveType
	{
		EnPassant, //ep
		Promotion, //=Q/N/R/B 
	}

	public enum GameEvent
	{
		Check, //+
		Checkmate, //#
		Draw, //%
	}

	public class MoveOption : IEquatable<MoveOption>
	{
		public MoveType Event { get; set; }
		public string Destination { get; set; }

		public SecondaryMoveType? Secondary { get; set;  } 
		public string SpecialCapturePosition { get; set; }

		public MoveOption(MoveType moveType, string dest)
		{
			if (dest == null || dest.Length != 2)
				throw new ArgumentException(dest, nameof(dest));

			Event = moveType;
			Destination = dest;
		}

		public override string ToString() => Enum.GetName(typeof(MoveType), Event) + ' ' + Destination;

		#region Equals
		public bool Equals(MoveOption other)
		{
			if (other == null)
				return false;
			return other.Event == Event && Equals(other.Destination, Destination);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as MoveOption);
		}

		public override int GetHashCode()
		{
			int hash = 19;
			hash = hash * 31 + (int)Event;
			hash = hash * 31 + Destination?.GetHashCode() ?? 0;
			return hash;
		}
		#endregion
	}


	public class GameGraph
	{
		public Board SourceBoard { get; }

		public GameGraph(Board board)
		{
			//Piece = piece;
			SourceBoard = board;
		}

		public List<MoveOption> FilterOptions(Piece piece, IEnumerable<MoveOption> options)
		{
			return options.Where(opt => IsAllowed(piece, opt)).ToList();
		}

		public GameEvent? Execute(Piece piece, MoveOption move, char? promotionPiece = null)
		{
			if (piece == null)
				throw new ArgumentNullException(nameof(piece));
			if (move == null)
				throw new ArgumentNullException(nameof(move));


			if ((move.Secondary != SecondaryMoveType.Promotion && promotionPiece != null) ||
				(move.Secondary == SecondaryMoveType.Promotion && promotionPiece == null))
			{
				throw new ArgumentException("Invalid promotion");
			}

			if (!IsAllowed(piece, move))
				throw new ArgumentException($"Move to {move.Destination} is not legal");

			ApplyMove(SourceBoard, piece, move, promotionPiece);
			return EvaluateGameState();
        }

		private bool IsAllowed(Piece piece, MoveOption move)
		{
			if(move.Event == MoveType.Castling)
			{
				var king = piece as King;
				if (null == king)
					throw new ArgumentException($"{move.Event} is not allowed for {piece}");

				//king has not moved
				if (SourceBoard.GetLastEntryForCell(king.Position) != null) 
					return false;

				//rook has not moved
				if (SourceBoard.GetLastEntryForCell(king.GetRookCastlingFromAndToPos(move).Item1) != null)
					return false;

				//king's route is not under attack (incl. its current pos)
				if (king.GetKingCastlingRoute(move).Any(m => IsUnderAttack(SourceBoard, m, piece.Color.Invert())))
					return false;
			}

			if (move.Event == MoveType.Regular || move.Event == MoveType.Capture)
			{
				if (move.Secondary == SecondaryMoveType.EnPassant)
				{
					if (!(piece is Pawn))
						throw new ArgumentException($"{move.Event} is not allowed for {piece}");

					//ensure that the opponent's last move was of a pawn to the enpassant position
					var expected = ChessUtil.ComposePieceString(Piece.Pawn, move.SpecialCapturePosition);
					if (SourceBoard.PeekHistory().PieceString != expected)
						return false;
				}

				var clone = SourceBoard.Clone();
				ApplyMove(clone, piece, move);
				if (IsInCheck(clone, piece.Color))
					return false;
			}

			return true;
		}

		private GameEvent? EvaluateGameState()
		{
			throw new NotImplementedException();
		}

		private static void ApplyMove(Board board, Piece piece, MoveOption move, char? promotionPiece = null)
		{
			switch(move.Event)
			{
                case MoveType.Regular:
				case MoveType.Capture:
					board[piece].Move(move.Destination, promotionPiece);

					if(null != move.SpecialCapturePosition)
					{
						board[move.SpecialCapturePosition].Remove();
					}
					break;

				case MoveType.Castling:
					board[piece].Move(move.Destination); //king
					var rook = ((King)piece).GetRookCastlingFromAndToPos(move);
					board[rook.Item1].Move(rook.Item2);
					break;
            }
		}

		private static bool IsInCheck(Board board, Color color)
		{
			return IsUnderAttack(board, new King(board[color]).Position, color.Invert());
		}

		private static bool IsUnderAttack(Board board, string position, Color fromColor)
		{
			var matrix = board.GetMatrix();
			return board[fromColor].Pieces
				.SelectMany(p => p.GetTechnicalMoves(matrix))
				.Any(m => m.Destination == position || m.SpecialCapturePosition == position);
		}
	}
}