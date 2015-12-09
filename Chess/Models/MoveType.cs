using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public enum MoveType
	{
		Regular, //pos || not in check
		Capture, //pos, piece 
		Castling, //pos
	}

	public enum SecondaryMoveType
	{
		EnPassant, //ep
		Promotion, //=Q/N/R/B 
	}

	public enum GameEvent
	{
		Check, //+
		Checkmate, //#
		Draw, //%
	}

	public class HistoryEntry : IEquatable<HistoryEntry>
	{
		public string PieceString { get; set; }
		public MoveOption Move { get; set; }
		public GameEvent? ResultingEvent { get; set; }

		public HistoryEntry()
		{
		}

		public HistoryEntry(string pieceString, string dest, MoveType moveType, SecondaryMoveType? sec = null)
		{
			if (pieceString == null || pieceString.Length != 3)
				throw new ArgumentException(pieceString, nameof(pieceString));

			PieceString = pieceString;
			Move = new MoveOption(moveType, dest);
			Move.Secondary = sec;
		}

		#region Equals
		public bool Equals(HistoryEntry other)
		{
			if (other == null)
				return false;
			return
				Equals(PieceString, other.PieceString) &&
				Equals(Move, other.Move) &&
				ResultingEvent == other.ResultingEvent;

		}

		public override bool Equals(object obj)
		{
			return Equals(obj as HistoryEntry);
		}

		public override int GetHashCode()
		{
			int hash = 19;
			hash = hash * 31 + PieceString?.GetHashCode() ?? 0;
			hash = hash * 31 + Move?.GetHashCode() ?? 0;
			hash = hash * 31 + ResultingEvent?.GetHashCode() ?? 0;
			return hash;
		}
		#endregion

	}

	public class MoveOption : IEquatable<MoveOption>
	{
		public MoveType Event { get; set; }
		public string Destination { get; set; }

		public SecondaryMoveType? Secondary { get; set; }
		public string SpecialCapturePosition { get; set; }

		public MoveOption(MoveType moveType, string dest)
		{
			if (dest == null || dest.Length != 2)
				throw new ArgumentException(dest, nameof(dest));

			Event = moveType;
			Destination = dest;
		}

		public override string ToString() => Enum.GetName(typeof(MoveType), Event) + ' ' + Destination;

		#region Equals
		public bool Equals(MoveOption other)
		{
			if (other == null)
				return false;
			return other.Event == Event && Equals(other.Destination, Destination);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as MoveOption);
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
}