using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;

namespace Chess.Engine.Tests
{
	[TestClass]
	public class BoardTests
	{
		[TestMethod]
		public void Clone_test()
		{
			var board = BoardFactory.ConstructSomeBoard();
			var clone = board.Clone();
			Assert.IsFalse(ReferenceEquals(board, clone));
			Assert.AreEqual(board, clone); //bad idea?
		}

		[TestMethod]
		public void IsOpen_test()
		{
			var board = new Board();
			Assert.IsTrue(board.IsOpen);

			board.History.Add(new HistoryEntry("Qd2", "e4", MoveType.Capture));
			Assert.IsTrue(board.IsOpen);

			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Check });
			Assert.IsTrue(board.IsOpen);

			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Checkmate });
			Assert.IsFalse(board.IsOpen);

			board = new Board();
			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Check });
			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Draw });

			Assert.IsFalse(board.IsOpen);
		}

		[TestMethod]
		public void PositionIndexer_test()
		{
			var board = BoardFactory.GetBoard(new[] { "Kb2", "pa4", "Rg5" }, new[] { "Kd6", "Qh2", "Rh4" });
			Assert.AreEqual("pa4", board["a4"].PieceString);
			Assert.AreEqual("Qh2", board["h2"].PieceString);
			var piece = new Rook(Color.White, "Rg5");
			Assert.AreEqual("Rg5", board[piece].PieceString);
			piece = new Rook(Color.Black, "Rg5");
			UnitTestUtil.AssertThrows<ArgumentException>(delegate { var a = board[piece]; });
			UnitTestUtil.AssertThrows<ArgumentException>(delegate { var a = board["Kb1"]; });
		}

		[TestMethod]
		public void CellHandle_move_test()
		{
			var board = BoardFactory.GetBoard(new[] { "Kb2", "pa4", "Rg5" }, new[] { "Kd6", "Qh2", "Rh4", "pe7" });

			var handle = new Board.CellHandle(board, Color.White, "pa4");
			UnitTestUtil.AssertThrows<InvalidOperationException>(() => handle.Move("g5"));
			handle.Move("a5");

			Assert.AreNotEqual(-1, board.White.PieceStrings.IndexOf("pa5"));
			Assert.AreEqual(-1, board.White.PieceStrings.IndexOf("pa4"));
			UnitTestUtil.AssertThrows<InvalidOperationException>(() => handle.Move("a5"));

			handle = new Board.CellHandle(board, Color.Black, "Qh2");
			handle.Move("g5");

			Assert.AreNotEqual(-1, board.Black.PieceStrings.IndexOf("Qg5"));
			Assert.AreEqual(-1, board.Black.PieceStrings.IndexOf("Qh2"));
			Assert.AreEqual(2, board.White.PieceStrings.Count);
			UnitTestUtil.AssertThrows<InvalidOperationException>(() => handle.Move("a5"));

			handle = new Board.CellHandle(board, Color.Black, "pe7");
			handle.Move("e8", 'Q');
			Assert.AreNotEqual(-1, board.Black.PieceStrings.IndexOf("Qe8"));
			Assert.AreEqual(-1, board.Black.PieceStrings.IndexOf("pe7"));
		}

		[TestMethod]
		public void CellHandle_remove_test()
		{
			var board = BoardFactory.GetBoard(new[] { "pa1", "Rb4" }, new[] { "pb7", "Be2" });
			var handle = new Board.CellHandle(board, Color.White, "pa1");

			handle.Remove();
			Assert.AreEqual(1, board.White.PieceStrings.Count);
			Assert.AreEqual("Rb4", board.White.PieceStrings.First());
			UnitTestUtil.AssertThrows<InvalidOperationException>(() => handle.Remove());
		}


		[TestMethod]
		public void GetMatrix_test()
		{
			var boardInitial = BoardFactory.ConstructInitialBoard();
			var matrixInitial = BoardMatrixFactory.GetInitialBoard();
			var actual = boardInitial.GetMatrix();

			for(int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
				{
					Assert.AreEqual(matrixInitial[i, j], actual[i, j]);
				}
        }

		[TestMethod]
		public void PeekHistory_test()
		{
			var board = new Board();

			board.PushHistory(new HistoryEntry("pe2", "e4", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Qd2", "b7", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Ke2", "d6", MoveType.Regular));

			Assert.AreEqual("Ke2", board.PeekHistory().PieceString);
        }

		[TestMethod]
		public void GetLastEntry_test()
		{
			var board = new Board();

			board.PushHistory(new HistoryEntry("pe2", "e4", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Qd2", "b7", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Be4", "d2", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Ke2", "d6", MoveType.Regular));

			Assert.AreEqual("Be4", board.GetLastEntryForCell("d2").PieceString);
		}

		[TestMethod]
		public void GetLastMovePlayerColor_test()
		{
			var board = new Board();

			board.PushHistory(new HistoryEntry("pe2", "e4", MoveType.Regular));
			Assert.AreEqual(Color.White, board.GetLastMovePlayerColor());

			board.PushHistory(new HistoryEntry("Qd2", "b7", MoveType.Regular));
			Assert.AreEqual(Color.Black, board.GetLastMovePlayerColor());

			board.PushHistory(new HistoryEntry("Ke2", "d6", MoveType.Regular));
			Assert.AreEqual(Color.White, board.GetLastMovePlayerColor());
		}

		[TestMethod]
		public void Equals_and_GetHashCode_test()
		{
			var board1 = new Board();
			var board2 = new Board();
			Assert.AreEqual(board1, board2);
			Assert.AreEqual(board1.GetHashCode(), board2.GetHashCode());

			board1 = BoardFactory.ConstructSomeBoard();
			board2 = BoardFactory.ConstructSomeBoard();
			Assert.AreEqual(board1, board2);
			Assert.AreEqual(board1.GetHashCode(), board2.GetHashCode());
			Assert.AreEqual(board2, board1);
			Assert.AreEqual(board2.GetHashCode(), board1.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.Caption = "diff";
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.CreatedAt = board2.CreatedAt.AddDays(1);
            Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.CreatedAt = board2.LastModifiedAt.AddHours(1);
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Checkmate });
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.White.PieceStrings.Add("Qe1");
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.Black.Name = board2.Black.Name + "diff";
            Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());
		}
	}
}