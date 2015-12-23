using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;
using Xunit;

namespace Chess.Engine.Tests
{
	public class GameProcessorTests
	{
		[Fact]
		public void GameProcessorCtorTest()
		{
			new GameProcessor(new Board());
			Assert.Throws<ArgumentNullException>(() => new GameProcessor(null));
		}

		[Fact]
		public void UsualMovesAllowed()
		{
			var board = Board.ConstructInitialBoard();
			var gproc = new GameProcessor(board);

			Piece piece = new Pawn(Color.White, "pc2");
			var moves = new List<MoveOption>();
			moves.Add(new MoveOption(MoveType.Regular, "c4"));
			moves.Add(new MoveOption(MoveType.Regular, "c3"));

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(2, actual.Count);

			piece = new Knight(Color.Black, "Ng8");
			moves = new List<MoveOption>();
			moves.Add(new MoveOption(MoveType.Regular, "f6"));
			moves.Add(new MoveOption(MoveType.Regular, "h6"));
			Assert.Equal(2, actual.Count);
		}

		[Fact]
		public void EnPassantAllowed()
		{
			var board = BoardFactory.GetBoard(new[] { "pd4", "Ka8" }, new[] { "pc4", "Ka1" });
			board.History = new List<HistoryEntry>
			{
				new HistoryEntry("pd2", "d4", MoveType.Regular),
			};
			var piece = new Pawn(Color.Black, "pc4");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Capture, "d3")
				{
					Secondary = SecondaryMoveType.EnPassant,
					SpecialCapturePosition = "d4"
				}
			};
			var gproc = new GameProcessor(board);

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(1, actual.Count);
		}

		[Fact]
		public void EnPassantNotAllowedIfTheLastMoveWasNot2Square()
		{
			var board = BoardFactory.GetBoard(new[] { "pd4", "Ka8" }, new[] { "pc4", "Ka1" });
			board.History = new List<HistoryEntry>
			{
				new HistoryEntry("pe3", "d4", MoveType.Capture),
			};
			var piece = new Pawn(Color.Black, "pc4");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Capture, "d3")
				{
					Secondary = SecondaryMoveType.EnPassant,
					SpecialCapturePosition = "d4"
				}
			};
			var gproc = new GameProcessor(board);

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);

			board.History = new List<HistoryEntry>
			{
				new HistoryEntry("pd3", "d4", MoveType.Capture),
			};
			actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);
		}


		[Fact]
		public void EnPassantNotAllowedIfPawnMovedNotInPrevTurn()
		{
			var board = BoardFactory.GetBoard(new[] { "pd4", "Ka8" }, new[] { "pc4", "Ka1" });
			board.History = new List<HistoryEntry>
			{
				new HistoryEntry("pd2", "d4", MoveType.Regular),
				new HistoryEntry("Ka5", "a6", MoveType.Regular),
				new HistoryEntry("Qb4", "e2", MoveType.Regular),
			};

			var piece = new Pawn(Color.Black, "pc4");
			var move = new MoveOption(MoveType.Capture, "d3")
			{
				Secondary = SecondaryMoveType.EnPassant,
				SpecialCapturePosition = "d4"
			};
			var moves = new List<MoveOption> { move };
			var gproc = new GameProcessor(board);

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);

			board.History = new List<HistoryEntry>();
			actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);
		}

		[Fact]
		public void MoveThatPutsKingInCheckIsNotAllowed()
		{
			var board = BoardFactory.GetBoard(new[] { "Rc2", "Ke1" }, new[] { "Bc5", "Kc6" });
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Regular, "d4")
			};
			var gproc = new GameProcessor(board);
			var actual = gproc.FilterOptions(new Bishop(Color.Black, "Bc5"), moves);
			Assert.Equal(0, actual.Count);
		}

		[Fact]
		public void InCheckOnlyMovesThatProtectKingAreAllowed()
		{
			var board = BoardFactory.GetBoard(new[] { "Rc2", "Ke1" }, new[] { "Bd6", "Kc6" });
			var piece = new Bishop(Color.Black, "Bd6");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Regular, "c7")
			};
			var gproc = new GameProcessor(board);
			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);

			moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Regular, "c5")
			};
			actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(1, actual.Count);
		}

		[Fact]
		public void CastlingCanBeMade_black()
		{
			var board = BoardFactory.GetBoard(new[] { "Ke1", }, new[] { "Ra8", "Ke8", "Rh8" });
			var gproc = new GameProcessor(board);
			var piece = new King(Color.Black, "Ke8");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Castling, "c8"),
				new MoveOption(MoveType.Castling, "g8"),
			};

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(2, actual.Count);
		}

		[Fact]
		public void CastlingCanBeMade_white()
		{
			var board = BoardFactory.GetBoard(new[] { "Ra1", "Ke1", "Rh1" }, new[] { "Kc8" });
			var gproc = new GameProcessor(board);
			var piece = new King(Color.White, "Ke1");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Castling, "c1"),
				new MoveOption(MoveType.Castling, "g1"),
			};

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(2, actual.Count);
		}


		[Fact]
		public void CastlingNotAllowedIfKingOrRookMoved()
		{
			var board = BoardFactory.GetBoard(new[] { "Ra1", "Ke1", "Rh1" }, new[] { "Kc8" });
			var gproc = new GameProcessor(board);
			var piece = new King(Color.White, "Ke1");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Castling, "c1"),
				new MoveOption(MoveType.Castling, "g1"),
			};
			board.PushHistory(new HistoryEntry("Ke1", "e2", MoveType.Regular));

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);

			board.History = new List<HistoryEntry>();
			board.PushHistory(new HistoryEntry("Ra1", "a4", MoveType.Regular));

			actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(1, actual.Count);
		}


		[Fact]
		public void CastlingAllowedIfRookIsUnderAttack()
		{
			var board = BoardFactory.GetBoard(new[] { "Ra1", "Ke1", "Rh1" }, new[] { "Kc8", "Ra8" });
			var gproc = new GameProcessor(board);
			var piece = new King(Color.White, "Ke1");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Castling, "c1"),
				new MoveOption(MoveType.Castling, "g1"),
			};

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(2, actual.Count);
		}


		[Fact]
		public void CastlingNotAllowedIfKingIsInCheck()
		{
			var board = BoardFactory.GetBoard(new[] { "Ra1", "Ke1", "Rh1" }, new[] { "Kf8", "Re8" });
			var gproc = new GameProcessor(board);
			var piece = new King(Color.White, "Ke1");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Castling, "c1"),
				new MoveOption(MoveType.Castling, "g1"),
			};

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);
		}

		[Fact]
		public void CastlingNotAllowedIfKingRouteIsUnderAttack()
		{
			var board = BoardFactory.GetBoard(new[] { "Ra1", "Ke1", "Rh1" }, new[] { "Kg8", "Rf8" });
			var gproc = new GameProcessor(board);
			var piece = new King(Color.White, "Ke1");
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Castling, "c1"),
				new MoveOption(MoveType.Castling, "g1"),
			};

			var actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(1, actual.Count);

			board.Black.PieceStrings.Add("Ba3");
			actual = gproc.FilterOptions(piece, moves);
			Assert.Equal(0, actual.Count);
		}

		[Fact]
		public void Execute_UsualMovesExecutedAndHistoryUpdated()
		{
			var board = Board.ConstructInitialBoard();
			var gproc = new GameProcessor(board);

			var gameEvent = gproc.Execute(
				new Pawn(Color.White, "pe2"),
				new MoveOption(MoveType.Regular, "e4"));
			Assert.Null(gameEvent);
			Assert.True(board.White.PieceStrings.Contains("pe4"));
			Assert.False(board.White.PieceStrings.Contains("pe2"));

			gameEvent = gproc.Execute(
				new Pawn(Color.Black, "pb7"),
				new MoveOption(MoveType.Regular, "b6"));
			Assert.Null(gameEvent);
			Assert.True(board.Black.PieceStrings.Contains("pb6"));
			Assert.False(board.Black.PieceStrings.Contains("pb7"));

			Assert.Equal(2, board.History.Count);
			Assert.True(board.History.All(h => h.ResultingEvent == null));
			Assert.Equal("pe2", board.History[0].PieceString);
			Assert.Equal("pb7", board.History[1].PieceString);
		}

		[Fact]
		public void Execute_CantMoveOneColorTwice()
		{
			var board = Board.ConstructInitialBoard();
			var gproc = new GameProcessor(board);

			gproc.Execute(
				new Pawn(Color.White, "pe2"),
				new MoveOption(MoveType.Regular, "e4"));

			Assert.Throws<InvalidOperationException>(() =>
				gproc.Execute(
					new Pawn(Color.White, "pa2"),
					new MoveOption(MoveType.Regular, "a4")));
		}

		[Fact]
		public void Execute_ThrowsExcIfMoveNotAllowed()
		{
			var board = BoardFactory.GetBoard(new[] { "Kc3", "pc4", "Nf2" }, new[] { "Be5", "Rb8", "Kc7" });
			var piece = new Pawn(Color.White, "pc4");
			var move = new MoveOption(MoveType.Regular, "c5");
			var gproc = new GameProcessor(board);

			Assert.Throws<InvalidOperationException>(() => gproc.Execute(piece, move));
		}

		[Fact]
		public void Execute_DrawWorks()
		{
			var board = BoardFactory.GetBoard(
				new[] { "pa6", "Bc8", "Rh5", "Bd4", "Kh8", "Rb1", "pa3" },
                new[] { "Ka5", "pa4", "Nb5" });
			var piece = new King(Color.White, "Kh8");
			var move = new MoveOption(MoveType.Regular, "g8");
			var gproc = new GameProcessor(board);

			var res = gproc.Execute(piece, move);
			Assert.Equal(GameEvent.Draw, res);
		}

		[Fact]
		public void Execute_CheckWorks()
		{
			var board = Board.ConstructInitialBoard();
			board.Black.PieceStrings.Remove("pd7");
			board.Black.PieceStrings.Remove("pc2");

			var piece = new Queen(Color.White, "Qd1");
			var move = new MoveOption(MoveType.Regular, "a4");
			var gproc = new GameProcessor(board);

			var res = gproc.Execute(piece, move);
			Assert.Equal(GameEvent.Check, res);
		}

		[Fact]
		public void Execute_CheckmateWorks()
		{
			var board = BoardFactory.GetBoard(
				new[] { "pe7", "Bf6", "Rd5", "Bc4", "Kh1"},
				new[] {"Ke8", "Bf8", "pf7"});

			var piece = new Bishop(Color.White, "Bc4");
			var move = new MoveOption(MoveType.Regular, "b5");
			var gproc = new GameProcessor(board);

			var res = gproc.Execute(piece, move);
			Assert.Equal(GameEvent.Checkmate, res);
		}

	}
}