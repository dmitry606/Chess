using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Engine
{
	public class Player : ICloneable
	{
		public string Name { get; set; }
		public List<string> PieceStrings { get; set; } = new List<string>();
		public Board Board { get; set; }

		public Color Color => Board.White == this ? Color.White : Color.Black;
		public Player Opponent => Board.White == this ? Board.Black : Board.White;
		internal List<Piece> Pieces => PieceStrings.Select(s => PieceFactory.Create(Color, s)).ToList();

		public void MakeMove(string from, string to, char? promotionTarget = null)
		{
			if (string.IsNullOrEmpty(from))
				throw new ArgumentException(nameof(from));
			if (string.IsNullOrEmpty(to))
				throw new ArgumentException(nameof(to));

			var piece = FindPiece(from);
			var move = piece
				.GetTechnicalMoves(Board.GetMatrix())
				.FirstOrDefault(m => m.Destination == to);
			if (null == move)
				throw new ArgumentException($"Move to {to} is not possible", nameof(to));

			new GameProcessor(Board).Execute(piece, move, promotionTarget);
		}

		public List<MoveOption> GetLegalMoves(string position)
		{
			var piece = FindPiece(position);
            var options = piece.GetTechnicalMoves(Board.GetMatrix());

			return new GameProcessor(Board).FilterOptions(piece, options);
		}

		private Piece FindPiece(string position)
		{
			var pieceString = PieceStrings.FirstOrDefault(s => s.EndsWith(position));
			if (null == pieceString)
				throw new ArgumentException($"No piece at '{position}'", nameof(position));
			return PieceFactory.Create(Color, pieceString);
		}

		#region Equals
		public bool Equals(Player other)
		{
			if (null == other)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return Equals(Name, other.Name) && PieceStrings.SequenceEqual(other.PieceStrings);
		}
		public override bool Equals(object obj) => Equals(obj as Player);

		public override int GetHashCode()
		{
			int hash = 19;
			hash = hash * 31 + PieceStrings.SequenceGetHashCode();
			hash = hash * 31 + Name?.GetHashCode() ?? 0;
			return hash;
		}

		public Player Clone()
		{
			return (Player)((ICloneable)this).Clone();
		}

		object ICloneable.Clone()
		{
			return new Player
			{
				Name = this.Name,
				PieceStrings = new List<string>(this.PieceStrings)
			};
		}
		#endregion
	}

}