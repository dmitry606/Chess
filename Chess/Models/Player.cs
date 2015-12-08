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
		public List<string> PieceStrings { get; set; } = new List<string>();
		public Board Board { get; set; }

		public Color Color => Board.White == this ? Color.White : Color.Black;
		public Player Opponent => Board.White == this ? Board.Black : Board.White;
		public List<Piece> Pieces => PieceStrings.Select(s => PieceFactory.Create(Color, s)).ToList();

		public List<MoveOption> GetLegalMoves(string position) => GetLegalMoves(FindPiece(position));

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

			var gameState = new GameGraph(Board).Execute(piece, move, promotionTarget);

			Board.PushHistory(new HistoryEntry
			{
				Move = move,
				PieceString = piece.PieceString,
				ResultingEvent = gameState
			});
		}

		private List<MoveOption> GetLegalMoves(Piece piece)
		{
			var options = piece.GetTechnicalMoves(Board.GetMatrix());
			return new GameGraph(Board).FilterOptions(piece, options);
		}

		private Piece FindPiece(string position)
		{
			var index = PieceStrings.FindIndex(s => s.EndsWith(position));
			if (-1 == index)
				throw new ArgumentException($"No piece at '{position}'", nameof(position));
			return PieceFactory.Create(this, index);
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
		#endregion
	}

}