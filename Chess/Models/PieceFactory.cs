using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public static class PieceFactory
	{
		public static Piece Create(Player player, int pieceIndex) => Create(player.Color, player.Pieces[pieceIndex]);

		public static Piece Create(Color color, string pieceString)
		{
			if (string.IsNullOrEmpty(pieceString))
				throw new ArgumentNullException(nameof(pieceString));

			switch (pieceString[0])
			{
				case Piece.King: return new King(color, pieceString);
			}

			throw new ArgumentException($"Unexpected: {pieceString}");
		}
	}
}