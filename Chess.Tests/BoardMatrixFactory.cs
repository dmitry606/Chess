using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models;

namespace Chess.Tests
{
	public static class BoardMatrixFactory
	{
		private const Color B = Color.Black;
		private const Color W = Color.White;
		private static readonly Color? _ = null;

		public static BoardMatrix GetInitialBoard() =>
			new BoardMatrix {
				M = new Color?[,]
				{
					{ B, B, B, B, B, B, B, B },
					{ B, B, B, B, B, B, B, B },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ W, W, W, W, W, W, W, W },
					{ W, W, W, W, W, W, W, W },
				}
			};

		public static BoardMatrix GetEmptyBoard() =>
			new BoardMatrix {
				M = new Color?[,]
				{
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
					{ _, _, _, _, _, _, _, _ },
				}
			};
	}
}
