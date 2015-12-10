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

			if (SourceBoard.GetLastMovePlayerColor() == piece.Color)
				throw new InvalidOperationException($"It's not {piece.Color} turn");

			if (!IsAllowed(piece, move))
				throw new InvalidOperationException($"Move to {move.Destination} is not legal");

			var histEntry = ApplyMove(SourceBoard, piece, move, promotionPiece);
			var gameState = EvaluateGameState();
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

					var lastHistEntry = SourceBoard.PeekHistory();
                    if (lastHistEntry == null ||
						lastHistEntry.Move.Destination != move.SpecialCapturePosition || 
						lastHistEntry.PieceString[0] != Piece.Pawn ||
						Math.Abs(lastHistEntry.PieceString[2] - lastHistEntry.Move.Destination[1]) != 2)
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
			var color = SourceBoard.GetLastMovePlayerColor().Invert();

			var hasMoves = false;
			foreach (var piece in SourceBoard[color].Pieces)
			{
				var moves = FilterOptions(piece, piece.GetTechnicalMoves(SourceBoard.GetMatrix()));
				if (moves.Any())
				{
					hasMoves = true;
					break;
				}
			}

			return IsInCheck(SourceBoard, color) ?
				(hasMoves ? GameEvent.Check : GameEvent.Checkmate) :
				(hasMoves ? (GameEvent?)null : GameEvent.Draw);
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

		private static bool IsInCheck(Board board, Color playerColor)
		{
			return IsUnderAttack(board, new King(board[playerColor]).Position, playerColor.Invert());
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