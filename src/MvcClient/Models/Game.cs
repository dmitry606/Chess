﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess.Engine;

namespace Chess.Models
{
	public class Game : IEquatable<Game>
	{
		public string Id { get; set; }
		public string Caption { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime LastModifiedAt { get; set; }
		public string WhiteId { get; set; }
		public string BlackId { get; set; }

		public Board Board { get; set; }

		#region Equals
		public bool Equals(Game other)
		{
			if (null == other)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return
				Equals(Id, other.Id) &&
				Equals(Caption, other.Caption) &&
				CreatedAt.ToString() == other.CreatedAt.ToString() &&
				LastModifiedAt.ToString() == other.LastModifiedAt.ToString() &&
				Equals(WhiteId, other.WhiteId) &&
				Equals(BlackId, other.BlackId) &&

				Equals(Board, other.Board);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Game);
		}

		public override int GetHashCode()
		{
			int hash = 19;

			hash = hash * 31 + Id?.GetHashCode() ?? 0;
			hash = hash * 31 + CreatedAt.GetHashCode();
			hash = hash * 31 + LastModifiedAt.GetHashCode();
			hash = hash * 31 + Caption?.GetHashCode() ?? 0;
			hash = hash * 31 + WhiteId?.GetHashCode() ?? 0;
			hash = hash * 31 + BlackId?.GetHashCode() ?? 0;
			hash = hash * 31 + Board?.GetHashCode() ?? 0;

			return hash;
		}
		#endregion
	}
}