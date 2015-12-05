using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public class MoveInfo
	{
		public string PieceString { get; set; }
		public string Destination { get; set; }
		public List<EventType> Events { get; set; }
	}

	public enum EventType
	{
		Regular, //pos || not in check
		Capture, //pos, piece 
		Castling, //pos (or long/short)

		EnPassant, //ep, pos
		Promotion, //=Q/N/R/B 

		Check, //+
		Checkmate, //#
        Draw, //%
	}

	public class RawOption : IEquatable<RawOption>
	{
		public EventType Event { get; }
		public string Destination { get; }
		public EventType? SecondaryEvent { get; } //for the pawn's promotion and en passant

		public RawOption(EventType eventType, string intendedPosition)
		{
			Event = eventType;
			Destination = intendedPosition;
		}

		public MoveInfo ToMoveInfo(Piece piece)
		{
			var result = new MoveInfo
			{
				Destination = Destination,
				PieceString = piece.PieceString,
				Events = new List<EventType> { Event },
			};

			if (SecondaryEvent.HasValue)
				result.Events.Add(SecondaryEvent.Value);

			return result;
		}

		public override string ToString() => Enum.GetName(typeof(EventType), Event) + ' ' + Destination;

		#region Equals
		public bool Equals(RawOption other)
		{
			if (other == null)
				return false;
			return other.Event == Event && Equals(other.Destination, Destination);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as RawOption);
		}

		public override int GetHashCode()
		{
			int hash = 19;
			hash = hash * 31 + (int)Event;
			hash = hash * 31 + Destination?.GetHashCode() ?? 0;
			return hash;
		}
		#endregion
	}


	public class GameGraph
	{
		private readonly Dictionary<EventType, List<EventType>> TheGraph = new Dictionary<EventType, List<EventType>>
		{
			[EventType.Regular] = { EventType.Promotion, EventType.Check, EventType.Draw },
			[EventType.Capture] = { EventType.Promotion, EventType.Check, EventType.Draw },
			[EventType.Castling] = { EventType.Check, EventType.Draw },
			[EventType.EnPassant] = { EventType.Check, EventType.Draw },
			[EventType.Promotion] = { EventType.Check, EventType.Draw },
			[EventType.Check] = { EventType.Checkmate },
			[EventType.Draw] = { },
			[EventType.Checkmate] = { }
		};

		public Board SourceBoard { get; }

		public GameGraph(Board board)
		{
			//Piece = piece;
			SourceBoard = board;
		}

		public List<MoveInfo> FilterOptions(Piece piece, IEnumerable<RawOption> options)
		{
			return options
				.Where(opt => Branch(piece, opt, false))
				.Select(opt => opt.ToMoveInfo(piece))
				.ToList();

			//foreach (var move in options)
			//{
			//	//var clone = SourceBoard.Clone();
			//	//var player = clone[piece.Color];

			//	//var movedPiece = player.Update(move);

			//	//if(!IsInCheck(player))
			//	//{
			//	//	result.Add(move);
			//	//}
			//}
		}

		private bool Branch(Piece piece, RawOption option, bool apply)
		{
			if(option.SecondaryEvent == EventType.EnPassant)
			{
				//1. check history
				//2. simulate
				//3. not in check
				//4. apply
				//5. evalulate
			}

			if(option.Event == EventType.Castling)
			{
				//1. check history
				//2. check the route is not under attack
				//3. apply
				//4. evalulate
			}

			if (option.Event == EventType.Regular || option.Event == EventType.Capture)
			{
				//1. simulate
				//2. not in check
				//3. apply
				//4. evalulate
			}

			throw new NotImplementedException();
		}

		private static void ApplyEvent(Board board, Piece piece, RawOption option, char? promotionPiece = null)
		{
			//if((option.SecondaryEvent != EventType.Promotion && promotionPiece != null) || 
			//	(option.SecondaryEvent == EventType.Promotion && promotionPiece == null))
			//{
			//	throw new ArgumentException("Invalid promotion");
			//}

			switch(option.Event)
			{
                case EventType.Regular:
				case EventType.Capture:
					board[piece].Move(option.Destination, promotionPiece);

					if(option.SecondaryEvent == EventType.EnPassant)
					{
						var captured = option.Destination[0].ToString() + piece.Position[1];
						board[captured].Remove();
					}
					break;

				case EventType.Castling:
					board[piece].Move(option.Destination); //king

					//rook
					var row = piece.Color == Color.White ? "1" : "8";
					if(option.Destination[0] == 'c') //long
					{
						board["a" + row].Move("d" + row);
                    }
					else //short
					{
						board["h" + row].Move("f" + row);
					}

					break;
            }
		}

		public void Execute(MoveInfo move, char? promotionTarget = null)
		{
			//var king = Player
			//var king = Player.Get<King>();
		}

		private static bool NotInCheck(Player player)
		{
			throw new NotImplementedException();
		}

		private static bool IsUnderAttack(string position, Color player)
		{
			throw new NotImplementedException();
		}
	}
}