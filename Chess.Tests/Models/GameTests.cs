using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models;

namespace Chess.Tests.Models
{
	[TestClass]
	public class GameTests
	{
		[TestMethod]
		public void Equals_and_GetHashCode_test()
		{
			var board1 = new Game();
			var board2 = new Game();
			Assert.AreEqual(board1, board2);
			Assert.AreNotEqual(board1, null);
			Assert.AreEqual(board1.GetHashCode(), board2.GetHashCode());

			board1 = GameFactory.ConstructSomeBoard();
			board2 = GameFactory.ConstructSomeBoard();
			Assert.AreEqual(board1, board2);
			Assert.AreEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = GameFactory.ConstructSomeBoard();
			board2.Caption = "diff";
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = GameFactory.ConstructSomeBoard();
			board2.CreatedAt = board2.CreatedAt.AddDays(1);
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = GameFactory.ConstructSomeBoard();
			board2.CreatedAt = board2.LastModifiedAt.AddHours(1);
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = GameFactory.ConstructSomeBoard();
			board2.BlackName = board2.BlackName + "diff";
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = GameFactory.ConstructSomeBoard();
			board2.Board.Black.PieceStrings.Add("pa1");
			Assert.AreNotEqual(board1, board2);
			Assert.AreNotEqual(board1.GetHashCode(), board2.GetHashCode());

		}
	}
}
