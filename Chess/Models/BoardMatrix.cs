using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
	public class BoardMatrix
	{
		public const string ColumnString = "abcdefgh";

		public Color?[,] M { private get; set; }

		public Color? this[int i, int j]
		{
			get { return M[i, j]; }
			set { M[i, j] = value; }
		}

		public Color? this[Tuple<int, int> t]
		{
			get { return M[t.Item1, t.Item2]; }
			set { M[t.Item1, t.Item2] = value; }
		}

		public Color? this[string position] 
		{
			get
			{
				var t = ConvertToTupleCoords(position);
				return this[t];
			}
			set
			{
				var t = ConvertToTupleCoords(position);
				this[t] = value;
			}
		}

		public bool AreEmpty(params string[] pos) => pos.All(p => this[p] == null);

		#region Coords conversion static helpers
		public static Tuple<int, int> ConvertToTupleCoords(string pos)
		{
			if (!IsValidCoords(pos))
				throw new ArgumentException("Unexpected string: " + pos);

			return new Tuple<int, int>(
				8 - int.Parse(pos.Substring(1, 1)), 
				ColumnString.IndexOf(pos[0]));
		}

		public static string ConvertCoords(int i, int j)
		{
			if (!IsValidCoords(i, j))
				throw new ArgumentOutOfRangeException();

			return ColumnString[j] + (8 - i).ToString();
		}

		public static string ConvertCoords(Tuple<int, int> t)
			=> ConvertCoords(t.Item1, t.Item2);


		public static bool IsValidCoords(Tuple<int, int> t)
		{
			return t.Item1 >= 0 && t.Item1 <= 7 && t.Item2 >= 0 && t.Item2 <= 7;
		}

		public static bool IsValidCoords(int i, int j)
		{
			return i >= 0 && i <= 7 && j >= 0 && j <= 7;
		}

		public static bool IsValidCoords(string position)
		{
			return position.Length == 2 &&
				-1 != ColumnString.IndexOf(position[0]) &&
				position[1] >= '1' && position[1] <= '8';
		}
		#endregion
	}
}