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
	public class BoardTests
	{
		[Fact]
		public void Clone_test()
		{
			var board = BoardFactory.ConstructSomeBoard();
			var clone = board.Clone();
			Assert.False(ReferenceEquals(board, clone));
			Assert.Equal(board, clone); //bad idea?
		}

		[Fact]
		public void IsOpen_test()
		{
			var board = new Board();
			Assert.True(board.IsOpen);

			board.History.Add(new HistoryEntry("Qd2", "e4", MoveType.Capture));
			Assert.True(board.IsOpen);

			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Check });
			Assert.True(board.IsOpen);

			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Checkmate });
			Assert.False(board.IsOpen);

			board = new Board();
			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Check });
			board.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Draw });

			Assert.False(board.IsOpen);
		}

		[Fact]
		public void PositionIndexer_test()
		{
			var board = BoardFactory.GetBoard(new[] { "Kb2", "pa4", "Rg5" }, new[] { "Kd6", "Qh2", "Rh4" });
			Assert.Equal("pa4", board["a4"].PieceString);
			Assert.Equal("Qh2", board["h2"].PieceString);
			var piece = new Rook(Color.White, "Rg5");
			Assert.Equal("Rg5", board[piece].PieceString);
			piece = new Rook(Color.Black, "Rg5");
			Assert.Throws<ArgumentException>(delegate { var a = board[piece]; });
			Assert.Throws<ArgumentException>(delegate { var a = board["Kb1"]; });
		}

		[Fact]
		public void CellHandle_move_test()
		{
			var board = BoardFactory.GetBoard(new[] { "Kb2", "pa4", "Rg5" }, new[] { "Kd6", "Qh2", "Rh4", "pe7" });

			var handle = new Board.CellHandle(board, Color.White, "pa4");
			Assert.Throws<InvalidOperationException>(() => handle.Move("g5"));
			handle.Move("a5");

			Assert.NotEqual(-1, board.White.PieceStrings.IndexOf("pa5"));
			Assert.Equal(-1, board.White.PieceStrings.IndexOf("pa4"));
			Assert.Throws<InvalidOperationException>(() => handle.Move("a5"));

			handle = new Board.CellHandle(board, Color.Black, "Qh2");
			handle.Move("g5");

			Assert.NotEqual(-1, board.Black.PieceStrings.IndexOf("Qg5"));
			Assert.Equal(-1, board.Black.PieceStrings.IndexOf("Qh2"));
			Assert.Equal(2, board.White.PieceStrings.Count);
			Assert.Throws<InvalidOperationException>(() => handle.Move("a5"));

			handle = new Board.CellHandle(board, Color.Black, "pe7");
			handle.Move("e8", 'Q');
			Assert.NotEqual(-1, board.Black.PieceStrings.IndexOf("Qe8"));
			Assert.Equal(-1, board.Black.PieceStrings.IndexOf("pe7"));
		}

		[Fact]
		public void CellHandle_remove_test()
		{
			var board = BoardFactory.GetBoard(new[] { "pa1", "Rb4" }, new[] { "pb7", "Be2" });
			var handle = new Board.CellHandle(board, Color.White, "pa1");

			handle.Remove();
			Assert.Equal(1, board.White.PieceStrings.Count);
			Assert.Equal("Rb4", board.White.PieceStrings.First());
			Assert.Throws<InvalidOperationException>(() => handle.Remove());
		}


		[Fact]
		public void GetMatrix_test()
		{
			var boardInitial = Board.ConstructInitialBoard();
			var matrixInitial = BoardMatrixFactory.GetInitialBoard();
			var actual = boardInitial.GetMatrix();

			for(int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
				{
					Assert.Equal(matrixInitial[i, j], actual[i, j]);
				}
        }

		[Fact]
		public void PeekHistory_test()
		{
			var board = new Board();

			board.PushHistory(new HistoryEntry("pe2", "e4", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Qd2", "b7", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Ke2", "d6", MoveType.Regular));

			Assert.Equal("Ke2", board.PeekHistory().PieceString);
        }

		[Fact]
		public void GetLastEntry_test()
		{
			var board = new Board();

			board.PushHistory(new HistoryEntry("pe2", "e4", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Qd2", "b7", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Be4", "d2", MoveType.Regular));
			board.PushHistory(new HistoryEntry("Ke2", "d6", MoveType.Regular));

			Assert.Equal("Be4", board.GetLastEntryForCell("d2").PieceString);
		}

		[Fact]
		public void GetLastMovePlayerColor_test()
		{
			var board = new Board();

			board.PushHistory(new HistoryEntry("pe2", "e4", MoveType.Regular));
			Assert.Equal(Color.White, board.PrevTurnColor);

			board.PushHistory(new HistoryEntry("Qd2", "b7", MoveType.Regular));
			Assert.Equal(Color.Black, board.PrevTurnColor);

			board.PushHistory(new HistoryEntry("Ke2", "d6", MoveType.Regular));
			Assert.Equal(Color.White, board.PrevTurnColor);
		}

		[Fact]
		public void Equals_and_GetHashCode_test()
		{
			var board1 = new Board();
			var board2 = new Board();
			Assert.Equal(board1, board2);
			Assert.Equal(board1.GetHashCode(), board2.GetHashCode());
			Assert.NotEqual(board1, null);

			board1 = BoardFactory.ConstructSomeBoard();
			board2 = BoardFactory.ConstructSomeBoard();
			Assert.Equal(board1, board2);
			Assert.Equal(board1.GetHashCode(), board2.GetHashCode());

			board2.History.Add(new HistoryEntry("Rg1", "d1", MoveType.Capture) { ResultingEvent = GameEvent.Checkmate });
			Assert.NotEqual(board1, board2);
			Assert.NotEqual(board1.GetHashCode(), board2.GetHashCode());

			board2 = BoardFactory.ConstructSomeBoard();
			board2.White.PieceStrings.Add("Qe1");
			Assert.NotEqual(board1, board2);
			Assert.NotEqual(board1.GetHashCode(), board2.GetHashCode());
		}
	}
}