using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;

namespace Chess.Models.Tests
{
	[TestClass()]
	public class GameProcessorTests
	{
		[TestMethod()]
		public void GameProcessorCtorTest()
		{
			new GameProcessor(new Board());
			UnitTestUtil.AssertThrows<ArgumentNullException>(() => new GameProcessor(null));
		}

		[TestMethod]
		public void UsualMovesAllowed()
		{
			var board = BoardFactory.ConstructInitialBoard();
			var gproc = new GameProcessor(board);

			Piece piece = new Pawn(Color.White, "pc2");
			var moves = new List<MoveOption>();
			moves.Add(new MoveOption(MoveType.Regular, "c4"));
			moves.Add(new MoveOption(MoveType.Regular, "c3"));

			var actual = gproc.FilterOptions(piece, moves);
			Assert.AreEqual(2, actual.Count);

			piece = new Knight(Color.Black, "Ng8");
			moves = new List<MoveOption>();
			moves.Add(new MoveOption(MoveType.Regular, "f6"));
			moves.Add(new MoveOption(MoveType.Regular, "h6"));
			Assert.AreEqual(2, actual.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(1, actual.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(0, actual.Count);

			board.History = new List<HistoryEntry>
			{
				new HistoryEntry("pd3", "d4", MoveType.Capture),
			};
			actual = gproc.FilterOptions(piece, moves);
			Assert.AreEqual(0, actual.Count);
		}


		[TestMethod]
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
			Assert.AreEqual(0, actual.Count);

			board.History = new List<HistoryEntry>();
			actual = gproc.FilterOptions(piece, moves);
			Assert.AreEqual(0, actual.Count);
		}

		[TestMethod]
		public void MoveThatPutsKingInCheckIsNotAllowed()
		{
			var board = BoardFactory.GetBoard(new[] { "Rc2", "Ke1" }, new[] { "Bc5", "Kc6" });
			var moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Regular, "d4")
			};
			var gproc = new GameProcessor(board);
			var actual = gproc.FilterOptions(new Bishop(Color.Black, "Bc5"), moves);
			Assert.AreEqual(0, actual.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(0, actual.Count);

			moves = new List<MoveOption>
			{
				new MoveOption(MoveType.Regular, "c5")
			};
			actual = gproc.FilterOptions(piece, moves);
			Assert.AreEqual(1, actual.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(2, actual.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(2, actual.Count);
		}


		[TestMethod]
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
			Assert.AreEqual(0, actual.Count);

			board.History = new List<HistoryEntry>();
			board.PushHistory(new HistoryEntry("Ra1", "a4", MoveType.Regular));

			actual = gproc.FilterOptions(piece, moves);
			Assert.AreEqual(1, actual.Count);
		}


		[TestMethod]
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
			Assert.AreEqual(2, actual.Count);
		}


		[TestMethod]
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
			Assert.AreEqual(0, actual.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(1, actual.Count);

			board.Black.PieceStrings.Add("Ba3");
			actual = gproc.FilterOptions(piece, moves);
			Assert.AreEqual(0, actual.Count);
		}

		[TestMethod]
		public void Execute_UsualMovesExecutedAndHistoryUpdated()
		{
			var board = BoardFactory.ConstructInitialBoard();
			var gproc = new GameProcessor(board);

			var gameEvent = gproc.Execute(
				new Pawn(Color.White, "pe2"),
				new MoveOption(MoveType.Regular, "e4"));
			Assert.IsNull(gameEvent);
			Assert.IsTrue(board.White.PieceStrings.Contains("pe4"));
			Assert.IsFalse(board.White.PieceStrings.Contains("pe2"));

			gameEvent = gproc.Execute(
				new Pawn(Color.Black, "pb7"),
				new MoveOption(MoveType.Regular, "b6"));
			Assert.IsNull(gameEvent);
			Assert.IsTrue(board.Black.PieceStrings.Contains("pb6"));
			Assert.IsFalse(board.Black.PieceStrings.Contains("pb7"));

			Assert.AreEqual(2, board.History.Count);
			Assert.IsTrue(board.History.All(h => h.ResultingEvent == null));
			Assert.AreEqual("pe2", board.History[0].PieceString);
			Assert.AreEqual("pb7", board.History[1].PieceString);
		}

		[TestMethod]
		public void Execute_CantMoveOneColorTwice()
		{
			var board = BoardFactory.ConstructInitialBoard();
			var gproc = new GameProcessor(board);

			gproc.Execute(
				new Pawn(Color.White, "pe2"),
				new MoveOption(MoveType.Regular, "e4"));

			UnitTestUtil.AssertThrows<InvalidOperationException>(() =>
				gproc.Execute(
					new Pawn(Color.White, "pa2"),
					new MoveOption(MoveType.Regular, "a4")));
		}

		[TestMethod]
		public void Execute_ThrowsExcIfMoveNotAllowed()
		{
			var board = BoardFactory.GetBoard(new[] { "Kc3", "pc4", "Nf2" }, new[] { "Be5", "Rb8", "Kc7" });
			var piece = new Pawn(Color.White, "pc4");
			var move = new MoveOption(MoveType.Regular, "c5");
			var gproc = new GameProcessor(board);

			UnitTestUtil.AssertThrows<InvalidOperationException>(() => gproc.Execute(piece, move));
		}

		[TestMethod]
		public void Execute_DrawWorks()
		{
			var board = BoardFactory.GetBoard(
				new[] { "pa6", "Bc8", "Rh5", "Bd4", "Kh8", "Rb1", "pa3" },
                new[] { "Ka5", "pa4", "Nb5" });
			var piece = new King(Color.White, "Kh8");
			var move = new MoveOption(MoveType.Regular, "g8");
			var gproc = new GameProcessor(board);

			var res = gproc.Execute(piece, move);
			Assert.AreEqual(GameEvent.Draw, res);
		}

		[TestMethod]
		public void Execute_CheckWorks()
		{
			var board = BoardFactory.ConstructInitialBoard();
			board.Black.PieceStrings.Remove("pd7");
			board.Black.PieceStrings.Remove("pc2");

			var piece = new Queen(Color.White, "Qd1");
			var move = new MoveOption(MoveType.Regular, "a4");
			var gproc = new GameProcessor(board);

			var res = gproc.Execute(piece, move);
			Assert.AreEqual(GameEvent.Check, res);
		}

		[TestMethod]
		public void Execute_CheckmateWorks()
		{
			var board = BoardFactory.GetBoard(
				new[] { "pe7", "Bf6", "Rd5", "Bc4", "Kh1"},
				new[] {"Ke8", "Bf8", "pf7"});

			var piece = new Bishop(Color.White, "Bc4");
			var move = new MoveOption(MoveType.Regular, "b5");
			var gproc = new GameProcessor(board);

			var res = gproc.Execute(piece, move);
			Assert.AreEqual(GameEvent.Checkmate, res);
		}

	}
}