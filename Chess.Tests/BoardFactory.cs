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
					Pieces = new List<string>
					{
						"Qe1", "Nd5", "pe4", "pb2"
					}
				},

				Black = new Player
				{
					Name = "Petya",
					Pieces = new List<string>
					{
						"Ka2", "Rf4", "pd5"
					}
				},

				History = new List<MoveInfo>
				{
					new MoveInfo { PieceString = "Qa2", Destination = "e4", Events = { EventType.Regular } },
					new MoveInfo { PieceString = "Rg1", Destination = "b7", Events = { EventType.Capture, EventType.Check } },
					new MoveInfo { PieceString = "Pb2", Destination = "g4", Events = { EventType.Regular, EventType.Promotion } },
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
					Pieces = new List<string>
					{
						"pa1", "pa2", "Ke6"
					}
				},

				Black = new Player
				{
					Name = "Vitya",
					Pieces = new List<string>
					{
						"Kd7", "Ba4"
					}
				},

				History = new List<MoveInfo>
				{
					new MoveInfo { PieceString = "Kf2", Destination = "e2", Events = { EventType.Castling } },
					new MoveInfo { PieceString = "Bc5", Destination = "c1", Events = { EventType.Regular } },
					new MoveInfo { PieceString = "Ra1", Destination = "a5", Events = { EventType.Regular } },
				}
			};
		}
	}
}
