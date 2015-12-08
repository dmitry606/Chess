using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models;

namespace Chess.Tests
{
	public static class BoardFactory
	{
		public static Board ConstructBoard()
		{
			return new Board
			{
				//CreatedAt = new DateTime(2015, 10, 10, ),
				//LastModifiedAt = new DateTime
				Caption = "Awesome game",

				White = new Player
				{
					Name = "Vasya",
					PieceStrings = new List<string>
					{
						"Qe1", "Nd5", "pe4", "pb2"
					}
				},

				Black = new Player
				{
					Name = "Petya",
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

		public static Board ConstructAnotherGame()
		{
			return new Board
			{
				//CreatedAt = new DateTime(2014, 9, 9),
				Caption = "Not so awesome game",

				White = new Player
				{
					Name = "Jenya",
					PieceStrings = new List<string>
					{
						"pa1", "pa2", "Ke6"
					}
				},

				Black = new Player
				{
					Name = "Vitya",
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
	}
}
