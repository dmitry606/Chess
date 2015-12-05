using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess.Utils;

namespace Chess.Models
{
	public class Player
	{
		public string Name { get; set; }
		public List<string> Pieces { get; set; } = new List<string>();
		public Board Board { get; set; }

		public Color Color => Board.White == this ? Color.White : Color.Black;
		public Player Opponent => Board.White == this ? Board.Black : Board.White;

		public Piece this[string position]
		{
			get
			{
				var index = Pieces.FindIndex(s => s.EndsWith(position));
				if (-1 == index)
					throw new ArgumentException($"No piece at '{position}'", nameof(position));
                return PieceFactory.Create(this, index);
			}
		}

		public List<MoveInfo> GetLegalMoves(string position) => GetLegalMoves(this[position]);

		public MoveInfo MakeMove(string from, string to, char? promotionTarget = null)
		{
			if (string.IsNullOrEmpty(from))
				throw new ArgumentException(nameof(from));
			if (string.IsNullOrEmpty(to))
				throw new ArgumentException(nameof(to));

			var piece = this[from];
			var moves = GetLegalMoves(piece);
			var move = moves.First(m => m.Destination == to);
//			piece = Update(move);
			var graph = new GameGraph(Board);
			graph.Execute(move, promotionTarget);
			Board.PushHistory(move);
			return move;
		}

		private List<MoveInfo> GetLegalMoves(Piece piece)
		{
			var options = piece.GetTechnicalMoves(Board.GetMatrix());
			var graph = new GameGraph(Board);

			return new GameGraph(Board).FilterOptions(piece, options);
		}

		//public Piece Update(MoveInfo move)
		//{
		//	var moved = ChessUtil.ComposePieceStringFromMove(move);
		//	Pieces.Remove(move.PieceString);
		//	Pieces.Add(moved);
		//	Opponent.Pieces.Remove(moved);
		//	return PieceFactory.Create(Color, moved);
		//}

		#region Equals
		public bool Equals(Player other)
		{
			if (null == other)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return Equals(Name, other.Name) && Pieces.SequenceEqual(other.Pieces);
		}
		public override bool Equals(object obj) => Equals(obj as Player);

		public override int GetHashCode()
		{
			int hash = 19;
			hash = hash * 31 + Pieces.SequenceGetHashCode();
			hash = hash * 31 + Name?.GetHashCode() ?? 0;
			return hash;
		}
		#endregion
	}

}