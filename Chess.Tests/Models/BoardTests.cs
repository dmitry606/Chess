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
	[TestClass]
	public class BoardTests
	{
		[TestMethod]
		public void Clone_test()
		{
			var board = BoardFactory.ConstructBoard();
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
		public void GetMatrix_test()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void PushHistory_test()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void PeekHistory_test()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void GetLastEntry_test()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void IsInCheck_test()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void Equals_and_GetHashCode_test()
		{
			var board1 = new Board();
			var board2 = new Board();
			Assert.AreEqual(board1, board2);
			Assert.AreEqual(board1.GetHashCode(), board2.GetHashCode());

			board1 = BoardFactory.ConstructBoard();
			board2 = BoardFactory.ConstructBoard();
			Assert.AreEqual(board1, board2);
			Assert.AreEqual(board1.GetHashCode(), board2.GetHashCode());
			Assert.AreEqual(board2, board1);
			Assert.AreEqual(board2.GetHashCode(), board1.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.Caption = "diff";
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.CreatedAt = board2.CreatedAt.AddDays(1);
            Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.CreatedAt = board2.LastModifiedAt.AddHours(1);
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Checkmate });
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.White.PieceStrings.Add("Qe1");
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.Black.Name = board2.Black.Name + "diff";
            Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());
		}
	}
}