using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public class GameProcessor
	{
		public Board SourceBoard { get; }

		public GameProcessor(Board board)
		{
			if (null == board)
				throw new ArgumentNullException(nameof(board));

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

			var histEntry = ApplyMove(SourceBoard, piece, move, promotionPiece);
			var gameState = EvaluateGameState(piece.Color);
			histEntry.ResultingEvent = gameState;
			return gameState;
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

		private GameEvent? EvaluateGameState(Color color)
		{
			var king = new King(SourceBoard[color]);
			if (IsUnderAttack(SourceBoard, king.Position, color.Invert()))
			{
				var kingMoves = FilterOptions(king, king.GetTechnicalMoves(SourceBoard.GetMatrix()));
				return kingMoves.Any() ? GameEvent.Check : GameEvent.Checkmate;
			}

			//if the opponent does not have any legal moves left, then it's a draw
			foreach (var piece in SourceBoard[color.Invert()].Pieces)
			{
				var moves = FilterOptions(piece, piece.GetTechnicalMoves(SourceBoard.GetMatrix()));
				if (moves.Any())
					return null;
			}

			return GameEvent.Draw;
        }

		private static HistoryEntry ApplyMove(Board board, Piece piece, MoveOption move, char? promotionPiece = null)
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

			var histEntry = new HistoryEntry
			{
				Move = move,
				PieceString = piece.PieceString,
			};

            board.PushHistory(histEntry);
			return histEntry;
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