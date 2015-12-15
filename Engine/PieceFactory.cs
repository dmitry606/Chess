using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Engine
{
	internal static class PieceFactory
	{
		public static Piece Create(Color color, string pieceString)
		{
			if (string.IsNullOrEmpty(pieceString))
				throw new ArgumentNullException(nameof(pieceString));

			switch (pieceString[0])
			{
				case Piece.King: return new King(color, pieceString);
				case Piece.Queen: return new Queen(color, pieceString);
				case Piece.Bishop: return new Bishop(color, pieceString);
				case Piece.Knight: return new Knight(color, pieceString);
				case Piece.Rook: return new Rook(color, pieceString);
				case Piece.Pawn: return new Pawn(color, pieceString);
			}

			throw new ArgumentException($"Unexpected: {pieceString}");
		}
	}
}