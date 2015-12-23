using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Tests;
using Xunit;

namespace Chess.Tests.Models
{
	public class PieceTests
	{
		[Fact]
		public void PieceCtorTest()
		{
			var t = new TestPiece(Color.Black, "Tb1");
			Assert.Equal("b1", t.Position);

			Assert.Throws<ArgumentException>(() =>
				new TestPiece(Color.Black, "Qa1"));

			Assert.Throws<ArgumentException>(() =>
				new TestPiece(Color.Black, "Tfff"));

			Assert.Throws<ArgumentException>(() =>
				new TestPiece(Color.Black, "T"));

			Assert.Throws<ArgumentException>(() =>
				new TestPiece(Color.Black, "Tr4"));

			Assert.Throws<ArgumentException>(() =>
				new TestPiece(Color.Black, "Ta9"));
		}

		[Fact]
		public void BishopTest()
		{
			var piece = new Bishop(Color.Black, "Bc8");
			Assert.Equal('B', piece.CharType);

			//blocked
			Assert.Equal(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//across the board
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["h8"] = Color.Black;
			piece = new Bishop(Color.Black, "Bh8");

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(7, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Regular));
			Assert.Equal("a1", moves.Last().Destination);

			//center
			piece = new Bishop(Color.Black, "Be4");
			board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;
			board["b1"] = Color.White; 
			board["g6"] = Color.White; 
			board["g2"] = Color.Black; 

			moves = piece.GetTechnicalMoves(board);

			Assert.Equal(10, moves.Count);
			var capt = moves.Where(m => m.Event == MoveType.Capture).ToList();
			Assert.Equal(2, capt.Count);
			Assert.Equal(1, capt.Count(m => m.Destination == "b1"));
			Assert.Equal(1, capt.Count(m => m.Destination == "g6"));
		}

		[Fact]
		public void RookTest()
		{
			var piece = new Rook(Color.Black, "Ra8");
			Assert.Equal('R', piece.CharType);

			//blocked
			Assert.Equal(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//across the board
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["h1"] = Color.Black;
			piece = new Rook(Color.Black, "Rh1");

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(14, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Regular));
			var last = moves.Last().Destination;
			Assert.True(last == "a1" || last == "h8");

			//center
			piece = new Rook(Color.Black, "Re4");
			board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;
			board["e2"] = Color.White; 
			board["a4"] = Color.White; 
			board["e7"] = Color.Black; 

			moves = piece.GetTechnicalMoves(board);

			Assert.Equal(11, moves.Count);
			var capt = moves.Where(m => m.Event == MoveType.Capture).ToList();
			Assert.Equal(2, capt.Count);
			Assert.Equal(1, capt.Count(m => m.Destination == "e2"));
			Assert.Equal(1, capt.Count(m => m.Destination == "a4"));
		}

		[Fact]
		public void QueenTests()
		{
			var piece = new Queen(Color.Black, "Qa8");
			Assert.Equal('Q', piece.CharType);

			//blocked
			Assert.Equal(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//across the board
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["h1"] = Color.Black;
			piece = new Queen(Color.Black, "Qh1");

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(21, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Regular));
			var last = moves.Last().Destination;
			Assert.True(last == "a1" || last == "h8" || last == "a8");

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

			Assert.Equal(21, moves.Count);
			var capt = moves.Where(m => m.Event == MoveType.Capture).ToList();
			Assert.Equal(4, capt.Count);
			Assert.Equal(1, capt.Count(m => m.Destination == "e2"));
			Assert.Equal(1, capt.Count(m => m.Destination == "a4"));
			Assert.Equal(1, capt.Count(m => m.Destination == "b1"));
			Assert.Equal(1, capt.Count(m => m.Destination == "g6"));
		}

		[Fact]
		public void KingTests()
		{
			var piece = new King(Color.Black, "Ke8");
			Assert.Equal('K', piece.CharType);

			//blocked
			Assert.Equal(0, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//center
			piece = new King(Color.Black, "Ke4");
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;

			board["d4"] = Color.White;
			board["f5"] = Color.Black;

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(7, moves.Count);
			var capt = moves.Where(m => m.Event == MoveType.Capture).ToList();
			Assert.Equal(1, capt.Count);
			Assert.Equal("d4", capt.First().Destination);
		}

		[Fact]
		public void KingCastlingTest()
		{
			var board = BoardMatrixFactory.GetEmptyBoard();
			var moves = SpawnKing(board, "e8", Color.Black);
			Assert.Equal(2, moves.Where(m => m.Event == MoveType.Castling).Count());

			moves = SpawnKing(board, "e7", Color.Black);
			Assert.Equal(0, moves.Where(m => m.Event == MoveType.Castling).Count());

			moves = SpawnKing(board, "e1", Color.White);
			Assert.Equal(2, moves.Where(m => m.Event == MoveType.Castling).Count());

			moves = SpawnKing(board, "e2", Color.White);
			Assert.Equal(0, moves.Where(m => m.Event == MoveType.Castling).Count());

			board = BoardMatrixFactory.GetEmptyBoard();
			board["g8"] = Color.Black;
			moves = SpawnKing(board, "e8", Color.Black);
			Assert.Equal(1, moves.Where(m => m.Event == MoveType.Castling).Count());
			board["b8"] = Color.Black;
			moves = SpawnKing(board, "e8", Color.Black);
			Assert.Equal(0, moves.Where(m => m.Event == MoveType.Castling).Count());

			board = BoardMatrixFactory.GetEmptyBoard();
			board["g1"] = Color.White;
			moves = SpawnKing(board, "e1", Color.White);
			Assert.Equal(1, moves.Where(m => m.Event == MoveType.Castling).Count());
			board["b1"] = Color.White;
			moves = SpawnKing(board, "e1", Color.Black);
			Assert.Equal(0, moves.Where(m => m.Event == MoveType.Castling).Count());
		}

		private List<MoveOption> SpawnKing(BoardMatrix board, string position, Color color)
		{
			board[position] = color;
			var piece = new King(color, 'K' + position);
			return piece.GetTechnicalMoves(board);
		}

		[Fact]
		public void KnightTests()
		{
			var piece = new Knight(Color.Black, "Ng8");
			Assert.Equal('N', piece.CharType);

			//(not)blocked
			Assert.Equal(2, piece.GetTechnicalMoves(BoardMatrixFactory.GetInitialBoard()).Count);

			//center
			piece = new Knight(Color.Black, "Ne4");
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["e4"] = Color.Black;

			board["c3"] = Color.White;
			board["f6"] = Color.Black;
			board["e5"] = Color.Black;
			board["e3"] = Color.White;

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(7, moves.Count);
			var capt = moves.Where(m => m.Event == MoveType.Capture).ToList();
			Assert.Equal(1, capt.Count);
			Assert.Equal("c3", capt.First().Destination);
		}

		[Fact]
		public void PawnTests_black()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
            var piece = new Pawn(Color.Black, "pb7");
			Assert.Equal('p', piece.CharType);

			var moves = piece.GetTechnicalMoves(board);
			Assert.Equal(2, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Regular));
			Assert.True(moves.Any(m => m.Destination == "b6"));
			Assert.True(moves.Any(m => m.Destination == "b5"));
			Assert.True(moves.All(m => m.Secondary == null));

			board["b5"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			Assert.Equal("b6", moves.Single().Destination);

			board["b6"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

			board["c6"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			Assert.Equal(MoveType.Capture, moves.Single().Event);

			board["a6"] = Color.Black;
			var moves2 = piece.GetTechnicalMoves(board);
			Assert.True(moves2.SequenceEqual(moves));

			board["a6"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(2, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Capture));
			Assert.True(moves.All(m => m.Secondary == null));

			board["e4"] = Color.Black;
			piece = new Pawn(Color.Black, "pe4");
			board["f3"] = Color.White;
			board["d3"] = Color.Black; //remove enpassant

			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(2, moves.Count);
			Assert.Equal("f3", moves.Single(m => m.Event == MoveType.Capture).Destination);
			Assert.Equal("e3", moves.Single(m => m.Event == MoveType.Regular).Destination);
			Assert.True(moves.All(m => m.Secondary == null));
		}

		[Fact]
		public void PawnTests_black_borders()
		{
			var board = BoardMatrixFactory.GetInitialBoard();

			board["d1"] = Color.Black;
			var piece = new Pawn(Color.Black, "pd1");
			var moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

			board["a1"] = Color.Black;
			piece = new Pawn(Color.Black, "pa1");
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

			board["h1"] = Color.Black;
			piece = new Pawn(Color.Black, "ph1");
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

		}


		[Fact]
		public void PawnTests_white()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			var piece = new Pawn(Color.White, "pb2");
			Assert.Equal('p', piece.CharType);

			var moves = piece.GetTechnicalMoves(board);
			Assert.Equal(2, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Regular));
			Assert.True(moves.Any(m => m.Destination == "b3"));
			Assert.True(moves.Any(m => m.Destination == "b4"));
			Assert.True(moves.All(m => m.Secondary == null));

			board["b4"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			Assert.Equal("b3", moves.Single().Destination);

			board["b3"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

			board["c3"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			Assert.Equal(MoveType.Capture, moves.Single().Event);

			board["a3"] = Color.White;
			var moves2 = piece.GetTechnicalMoves(board);
			Assert.True(moves2.SequenceEqual(moves));

			board["a3"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(2, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Capture));
			Assert.True(moves.All(m => m.Secondary == null));

			board["e4"] = Color.White;
			piece = new Pawn(Color.White, "pe4");
			board["f5"] = Color.Black;

			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(2, moves.Count);
			Assert.Equal("f5", moves.Single(m => m.Event == MoveType.Capture).Destination);
			Assert.Equal("e5", moves.Single(m => m.Event == MoveType.Regular).Destination);
			Assert.True(moves.All(m => m.Secondary == null));
		}



		[Fact]
		public void PawnTests_white_borders()
		{
			var board = BoardMatrixFactory.GetInitialBoard();

			board["d8"] = Color.White;
			var piece = new Pawn(Color.White, "pd8");
			var moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

			board["a8"] = Color.White;
			piece = new Pawn(Color.White, "pa8");
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);

			board["h8"] = Color.White;
			piece = new Pawn(Color.White, "ph8");
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(0, moves.Count);
		}


		[Fact]
		public void PawnTests_black_promotion()
		{
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["g2"] = Color.Black;
			var piece = new Pawn(Color.Black, "pg2");
			board["h1"] = Color.White;

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(2, moves.Count);
			var m = moves.FirstOrDefault(_ => _.Destination == "g1");
			Assert.NotNull(m);
			Assert.Equal(MoveType.Regular, m.Event);
			Assert.Equal(SecondaryMoveType.Promotion, m.Secondary);

			m = moves.FirstOrDefault(_ => _.Destination == "h1");
			Assert.NotNull(m);
			Assert.Equal(MoveType.Capture, m.Event);
			Assert.Equal(SecondaryMoveType.Promotion, m.Secondary);
		}

		[Fact]
		public void PawnTests_white_promotion()
		{
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["g7"] = Color.White;
			var piece = new Pawn(Color.White, "pg7");
			board["h8"] = Color.Black;

			var moves = piece.GetTechnicalMoves(board);

			Assert.Equal(2, moves.Count);
			var m = moves.FirstOrDefault(_ => _.Destination == "g8");
			Assert.NotNull(m);
			Assert.Equal(MoveType.Regular, m.Event);
			Assert.Equal(SecondaryMoveType.Promotion, m.Secondary);

			m = moves.FirstOrDefault(_ => _.Destination == "h8");
			Assert.NotNull(m);
			Assert.Equal(MoveType.Capture, m.Event);
			Assert.Equal(SecondaryMoveType.Promotion, m.Secondary);
		}

		[Fact]
		public void PawnTests_black_enpassant()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			board["f4"] = Color.Black;
			var piece = new Pawn(Color.Black, "pf4");

			var moves = piece.GetTechnicalMoves(board)
				.Where(m => m.Secondary.HasValue)
				.ToList();
			Assert.Equal(2, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Capture));
			Assert.True(moves.All(m => m.Secondary == SecondaryMoveType.EnPassant));
			Assert.True(moves.All(m => m.SpecialCapturePosition != null));

			board["e3"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.Secondary.HasValue)
				.ToList();
			Assert.Equal(1, moves.Count);
			Assert.Equal("g4", moves.First().SpecialCapturePosition);

			board["e3"] = null;
			board["g3"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.Secondary.HasValue)
				.ToList();
			Assert.Equal(1, moves.Count);
			Assert.Equal("e4", moves.First().SpecialCapturePosition);
		}

		[Fact]
		public void PawnTests_black_enpassant_borders()
		{
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["a4"] = Color.Black;
			var piece = new Pawn(Color.Black, "pa4");
			board["a3"] = Color.Black;
			var moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			var m = moves.First();
			Assert.Equal(SecondaryMoveType.EnPassant, m.Secondary);
			Assert.Equal("b4", m.SpecialCapturePosition);

			board["h4"] = Color.Black;
			piece = new Pawn(Color.Black, "ph4");
			board["h3"] = Color.Black;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			m = moves.First();
			Assert.Equal(SecondaryMoveType.EnPassant, m.Secondary);
			Assert.Equal("g4", m.SpecialCapturePosition);
		}

		[Fact]
		public void PawnTests_white_enpassant()
		{
			var board = BoardMatrixFactory.GetInitialBoard();
			board["c5"] = Color.White;
			var piece = new Pawn(Color.White, "pc5");

			var moves = piece.GetTechnicalMoves(board)
				.Where(m => m.Secondary.HasValue)
				.ToList();
			Assert.Equal(2, moves.Count);
			Assert.True(moves.All(m => m.Event == MoveType.Capture));
			Assert.True(moves.All(m => m.Secondary == SecondaryMoveType.EnPassant));
			Assert.True(moves.All(m => m.SpecialCapturePosition != null));

			board["b6"] = Color.White;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.Secondary.HasValue)
				.ToList();
			Assert.Equal(1, moves.Count);
			Assert.Equal("d5", moves.First().SpecialCapturePosition);

			board["b6"] = null;
			board["d6"] = Color.Black;
			moves = piece.GetTechnicalMoves(board)
				.Where(m => m.Secondary.HasValue)
				.ToList();
			Assert.Equal("b5", moves.First().SpecialCapturePosition);
		}

		[Fact]
		public void PawnTests_white_enpassant_borders()
		{
			var board = BoardMatrixFactory.GetEmptyBoard();
			board["a5"] = Color.White;
			var piece = new Pawn(Color.White, "pa5");
			board["a6"] = Color.White;
			var moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			var m = moves.First();
			Assert.Equal(SecondaryMoveType.EnPassant, m.Secondary);
			Assert.Equal("b5", m.SpecialCapturePosition);

			board["h5"] = Color.White;
			piece = new Pawn(Color.White, "ph5");
			board["h6"] = Color.White;
			moves = piece.GetTechnicalMoves(board);
			Assert.Equal(1, moves.Count);
			m = moves.First();
			Assert.Equal(SecondaryMoveType.EnPassant, m.Secondary);
			Assert.Equal("g5", m.SpecialCapturePosition);
		}


		private class TestPiece : Piece
		{
			public override char CharType => 'T';

			public TestPiece(Color color, string pieceString) : base(color, pieceString) { }

			public override List<MoveOption> GetTechnicalMoves(BoardMatrix boardMatrix)
			{
				throw new NotImplementedException();
			}
		}
	}
}
