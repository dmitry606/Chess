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

			board.History.Add(new MoveInfo {
				Destination = "e4",
				Events = { EventType.Capture, EventType.Check },
				PieceString = "Qd2" });
			Assert.IsTrue(board.IsOpen);

			board.History.Add(new MoveInfo
			{
				Destination = "d7",
				Events = { EventType.Capture },
				PieceString = "Rg1"
			});
			Assert.IsTrue(board.IsOpen);

			board.History.Add(new MoveInfo
			{
				Destination = "b5",
				Events = { EventType.Capture, EventType.Checkmate },
				PieceString = "Ng2"
			});
			Assert.IsFalse(board.IsOpen);

			board = new Board();
			board.History.Add(new MoveInfo
			{
				Destination = "e4",
				Events = { EventType.Capture, EventType.Check },
				PieceString = "Qd2"
			});
			board.History.Add(new MoveInfo
			{
				Destination = "d7",
				Events = { EventType.Draw },
				PieceString = "Rg1"
			});
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
			board2.History.Add(new MoveInfo { PieceString = "Pa1", Destination = "a2", Events = { EventType.Regular } });
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.White.Pieces.Add("Qe1");
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructBoard();
			board2.Black.Name = board2.Black.Name + "diff";
            Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());
		}
	}
}