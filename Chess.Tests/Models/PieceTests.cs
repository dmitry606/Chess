using Chess.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;

namespace Chess.Tests.Models
{
	[TestClass]
	public class PieceTests
	{
		[TestMethod]
		public void PieceCtorTest()
		{
			var t = new TestPiece(Color.Black, "Tb1");
			Assert.AreEqual("b1", t.Position);

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				new TestPiece(Color.Black, "Qa1"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				new TestPiece(Color.Black, "Tfff"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				new TestPiece(Color.Black, "T"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				new TestPiece(Color.Black, "Tr4"));

			UnitTestUtil.AssertThrows<ArgumentException>(() =>
				new TestPiece(Color.Black, "Ta9"));
		}

		[TestMethod]
		public void BishopTest()
		{
			var piece = new Bishop(Color.Black, "Bc8");
			Assert.AreEqual('B', piece.CharType);

			//blocked
			Assert.AreEqual(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//across the board
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["h8"] = Color.Black;
			piece = new Bishop(Color.Black, "Bh8");

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(7, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Regular));
			Assert.AreEqual("a1", moves.Last().Destination);

			//center
			piece = new Bishop(Color.Black, "Be4");
			board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;
			board["b1"] = Color.White; 
			board["g6"] = Color.White; 
			board["g2"] = Color.Black; 

			moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(10, moves.Count);
			var capt = moves.Where(m => m.Event == EventType.Capture).ToList();
			Assert.AreEqual(2, capt.Count);
			Assert.AreEqual(1, capt.Count(m => m.Destination == "b1"));
			Assert.AreEqual(1, capt.Count(m => m.Destination == "g6"));
		}

		[TestMethod]
		public void RookTest()
		{
			var piece = new Rook(Color.Black, "Ra8");
			Assert.AreEqual('R', piece.CharType);

			//blocked
			Assert.AreEqual(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//across the board
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["h1"] = Color.Black;
			piece = new Rook(Color.Black, "Rh1");

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(14, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Regular));
			var last = moves.Last().Destination;
			Assert.IsTrue(last == "a1" || last == "h8");

			//center
			piece = new Rook(Color.Black, "Re4");
			board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;
			board["e2"] = Color.White; 
			board["a4"] = Color.White; 
			board["e7"] = Color.Black; 

			moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(11, moves.Count);
			var capt = moves.Where(m => m.Event == EventType.Capture).ToList();
			Assert.AreEqual(2, capt.Count);
			Assert.AreEqual(1, capt.Count(m => m.Destination == "e2"));
			Assert.AreEqual(1, capt.Count(m => m.Destination == "a4"));
		}

		[TestMethod]
		public void QueenTests()
		{
			var piece = new Queen(Color.Black, "Qa8");
			Assert.AreEqual('Q', piece.CharType);

			//blocked
			Assert.AreEqual(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//across the board
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["h1"] = Color.Black;
			piece = new Queen(Color.Black, "Qh1");

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(21, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Regular));
			var last = moves.Last().Destination;
			Assert.IsTrue(last == "a1" || last == "h8" || last == "a8");

			//center
			piece = new Queen(Color.Black, "Qe4");
			board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;

			board["e2"] = Color.White;
			board["a4"] = Color.White;
			board["e7"] = Color.Black;
			board["b1"] = Color.White; // diag
			board["g6"] = Color.White;
			board["g2"] = Color.Black;

			moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(21, moves.Count);
			var capt = moves.Where(m => m.Event == EventType.Capture).ToList();
			Assert.AreEqual(4, capt.Count);
			Assert.AreEqual(1, capt.Count(m => m.Destination == "e2"));
			Assert.AreEqual(1, capt.Count(m => m.Destination == "a4"));
			Assert.AreEqual(1, capt.Count(m => m.Destination == "b1"));
			Assert.AreEqual(1, capt.Count(m => m.Destination == "g6"));
		}

		[TestMethod]
		public void KingTests()
		{
			var piece = new King(Color.Black, "Ke8");
			Assert.AreEqual('K', piece.CharType);

			//blocked
			Assert.AreEqual(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//center
			piece = new King(Color.Black, "Ke4");
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;

			board["d4"] = Color.White;
			board["f5"] = Color.Black;

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(7, moves.Count);
			var capt = moves.Where(m => m.Event == EventType.Capture).ToList();
			Assert.AreEqual(1, capt.Count);
			Assert.AreEqual("d4", capt.First().Destination);
		}

		[TestMethod]
		public void KingCastlingTest()
		{
			var board = BoardMatrixFactory.GetEmptyBoard();
			var moves = SpawnKing(board, "e8", Color.Black);
			Assert.AreEqual(2, moves.Where(m => m.Event == EventType.Castling).Count());

			moves = SpawnKing(board, "e7", Color.Black);
			Assert.AreEqual(0, moves.Where(m => m.Event == EventType.Castling).Count());

			moves = SpawnKing(board, "e1", Color.White);
			Assert.AreEqual(2, moves.Where(m => m.Event == EventType.Castling).Count());

			moves = SpawnKing(board, "e2", Color.White);
			Assert.AreEqual(0, moves.Where(m => m.Event == EventType.Castling).Count());

			board = BoardMatrixFactory.GetEmptyBoard();
			board["g8"] = Color.Black;
			moves = SpawnKing(board, "e8", Color.Black);
			Assert.AreEqual(1, moves.Where(m => m.Event == EventType.Castling).Count());
			board["b8"] = Color.Black;
			moves = SpawnKing(board, "e8", Color.Black);
			Assert.AreEqual(0, moves.Where(m => m.Event == EventType.Castling).Count());

			board = BoardMatrixFactory.GetEmptyBoard();
			board["g1"] = Color.White;
			moves = SpawnKing(board, "e1", Color.White);
			Assert.AreEqual(1, moves.Where(m => m.Event == EventType.Castling).Count());
			board["b1"] = Color.White;
			moves = SpawnKing(board, "e1", Color.Black);
			Assert.AreEqual(0, moves.Where(m => m.Event == EventType.Castling).Count());
		}

		private List<RawOption> SpawnKing(BoardMatrix board, string position, Color color)
		{
			board[position] = color;
			var piece = new King(color, 'K' + position);
			return piece.GetTechnicalMoves(board);
		}

		[TestMethod]
		public void KnightTests()
		{
			var piece = new Knight(Color.Black, "Ng8");
			Assert.AreEqual('N', piece.CharType);

			//(not)blocked
			Assert.AreEqual(2, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//center
			piece = new Knight(Color.Black, "Ne4");
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;

			board["c3"] = Color.White;
			board["f6"] = Color.Black;
			board["e5"] = Color.Black;
			board["e3"] = Color.White;

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(7, moves.Count);
			var capt = moves.Where(m => m.Event == EventType.Capture).ToList();
			Assert.AreEqual(1, capt.Count);
			Assert.AreEqual("c3", capt.First().Destination);
		}

		[TestMethod]
		public void PawnTests_black()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
            var piece = new Pawn(Color.Black, "pb7");
			Assert.AreEqual('p', piece.CharType);

			var moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(2, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Regular));
			Assert.IsTrue(moves.Any(m => m.Destination == "b6"));
			Assert.IsTrue(moves.Any(m => m.Destination == "b5"));
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == null));

			board["b5"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual("b6", moves.Single().Destination);

			board["b6"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(0, moves.Count);

			board["c6"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual(EventType.Capture, moves.Single().Event);

			board["a6"] = Color.Black;
			var moves2 = piece.GetTechnicalMoves(board);
			Assert.IsTrue(moves2.SequenceEqual(moves));

			board["a6"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(2, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Capture));
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == null));

			board["e4"] = Color.Black;
			piece = new Pawn(Color.Black, "pe4");
			board["f3"] = Color.White;

			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual("f3", moves.Single(m => m.Event == EventType.Capture).Destination);
			Assert.AreEqual("e3", moves.Single(m => m.Event == EventType.Regular).Destination);
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == null));

			board["d1"] = Color.Black;
			piece = new Pawn(Color.Black, "pd1");
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void PawnTests_white()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			var piece = new Pawn(Color.White, "pb2");
			Assert.AreEqual('p', piece.CharType);

			var moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(2, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Regular));
			Assert.IsTrue(moves.Any(m => m.Destination == "b3"));
			Assert.IsTrue(moves.Any(m => m.Destination == "b4"));
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == null));

			board["b4"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual("b3", moves.Single().Destination);

			board["b3"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(0, moves.Count);

			board["c3"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual(EventType.Capture, moves.Single().Event);

			board["a3"] = Color.White;
			var moves2 = piece.GetTechnicalMoves(board);
			Assert.IsTrue(moves2.SequenceEqual(moves));

			board["a3"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(2, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Capture));
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == null));

			board["e4"] = Color.White;
			piece = new Pawn(Color.White, "pe4");
			board["f5"] = Color.Black;

			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual("f5", moves.Single(m => m.Event == EventType.Capture).Destination);
			Assert.AreEqual("e5", moves.Single(m => m.Event == EventType.Regular).Destination);
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == null));

			board["d8"] = Color.White;
			piece = new Pawn(Color.White, "pd8");
			moves = piece.GetTechnicalMoves(board);
			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void PawnTests_black_promotion()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			board["g2"] = Color.Black;
			var piece = new Pawn(Color.Black, "pg2");
			board["h1"] = Color.White;

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(2, moves.Count);
			var m = moves.FirstOrDefault(_ => _.Destination == "g1");
			Assert.IsNotNull(m);
			Assert.AreEqual(EventType.Regular, m.Event);
			Assert.AreEqual(EventType.Promotion, m.SecondaryEvent);

			m = moves.FirstOrDefault(_ => _.Destination == "h1");
			Assert.IsNotNull(m);
			Assert.AreEqual(EventType.Capture, m.Event);
			Assert.AreEqual(EventType.Promotion, m.SecondaryEvent);
		}

		[TestMethod]
		public void PawnTests_white_promotion()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			board["g7"] = Color.White;
			var piece = new Pawn(Color.White, "pg7");
			board["h8"] = Color.Black;

			var moves = piece.GetTechnicalMoves(board);

			Assert.AreEqual(2, moves.Count);
			var m = moves.FirstOrDefault(_ => _.Destination == "g8");
			Assert.IsNotNull(m);
			Assert.AreEqual(EventType.Regular, m.Event);
			Assert.AreEqual(EventType.Promotion, m.SecondaryEvent);

			m = moves.FirstOrDefault(_ => _.Destination == "h8");
			Assert.IsNotNull(m);
			Assert.AreEqual(EventType.Capture, m.Event);
			Assert.AreEqual(EventType.Promotion, m.SecondaryEvent);
		}


		[TestMethod]
		public void PawnTests_black_enpassant()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			board["f4"] = Color.Black;
			var piece = new Pawn(Color.Black, "pf4");

			var moves = piece.GetTechnicalMoves(board)
				.Where(m => m.SecondaryEvent.HasValue)
				.ToList();
			Assert.AreEqual(2, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Capture));
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == EventType.EnPassant));

			board["e3"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.SecondaryEvent.HasValue)
				.ToList();
			Assert.AreEqual(1, moves.Count);

			board["g3"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.SecondaryEvent.HasValue)
				.ToList();
			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void PawnTests_white_enpassant()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			board["c5"] = Color.White;
			var piece = new Pawn(Color.White, "pc5");

			var moves = piece.GetTechnicalMoves(board)
				.Where(m => m.SecondaryEvent.HasValue)
				.ToList();
			Assert.AreEqual(2, moves.Count);
			Assert.IsTrue(moves.All(m => m.Event == EventType.Capture));
			Assert.IsTrue(moves.All(m => m.SecondaryEvent == EventType.EnPassant));

			board["b6"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.SecondaryEvent.HasValue)
				.ToList();
			Assert.AreEqual(1, moves.Count);

			board["d6"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.SecondaryEvent.HasValue)
				.ToList();
			Assert.AreEqual(0, moves.Count);
		}


		private class TestPiece : Piece
		{
			public override char CharType => 'T';

			public TestPiece(Color color, string pieceString) : base(color, pieceString) { }

			public override List<RawOption> GetTechnicalMoves(BoardMatrix boardMatrix)
			{
				throw new NotImplementedException();
			}
		}
	}
}
