using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Engine;

namespace Chess.Tests
{
	public static class BoardFactory
	{
		public static Board ConstructInitialBoard()
		{
			return new Board
			{
				White = new Player
				{
					PieceStrings = new List<string>
					{
						"pa2", "pb2", "pc2", "pd2", "pe2", "pf2", "pg2", "ph2",
						"Ra1", "Nb1", "Bc1", "Qd1", "Ke1", "Bf1", "Ng1", "Rh1"
					}
				},

				Black = new Player
				{
					PieceStrings = new List<string>
					{
						"pa7", "pb7", "pc7", "pd7", "pe7", "pf7", "pg7", "ph7",
						"Ra8", "Nb8", "Bc8", "Qd8", "Ke8", "Bf8", "Ng8", "Rh8"
					}
				},
			};
		}

		public static Board ConstructSomeBoard()
		{
			return new Board
			{
				White = new Player
				{
					PieceStrings = new List<string>
					{
						"Qe1", "Nd5", "pe4", "pb2"
					}
				},

				Black = new Player
				{
					PieceStrings = new List<string>
					{
						"Ka2", "Rf4", "pd5"
					}
				},

				History = new List<HistoryEntry>
				{
					new HistoryEntry("Qa2", "e4", MoveType.Regular),
					new HistoryEntry ("Rg1", "b7", MoveType.Capture) { ResultingEvent = GameEvent.Check },
					new HistoryEntry ("Pb2", "g4", MoveType.Regular, SecondaryMoveType.Promotion),
				}
			};
		}

		public static Board ConstructAnotherBoard()
		{
			return new Board
			{
				White = new Player
				{
					PieceStrings = new List<string>
					{
						"pa1", "pa2", "Ke6"
					}
				},

				Black = new Player
				{
					PieceStrings = new List<string>
					{
						"Kd7", "Ba4"
					}
				},

				History = new List<HistoryEntry>
				{
					new HistoryEntry ("Kf2", "e2", MoveType.Castling),
					new HistoryEntry ("Bc5", "c1", MoveType.Regular),
					new HistoryEntry ("Ra1", "a5", MoveType.Regular),
				}
			};
		}

		public static Board GetBoard(string[] white, string[] black)
		{
			var board = new Board();
			board.White.PieceStrings = white.ToList();
			board.Black.PieceStrings = black.ToList();

			return board;
		}
	}
}
