using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models;

namespace Chess.Tests
{
	class GameFactory
	{
		public static Game ConstructInitialBoard()
		{
			return new Game
			{
				Caption = "Initial test board",
				WhiteName = "White player 1",
				BlackName = "Black player 1",
				Board = Engine.Board.ConstructInitialBoard(),
			};
		}

		public static Game ConstructSomeBoard()
		{
			return new Game
			{
				Caption = "Awesome game",
				WhiteName = "White player 2",
				BlackName = "Black player 2",
				Board = BoardFactory.ConstructSomeBoard(),
			};
		}

		public static Game ConstructAnotherBoard()
		{
			return new Game
			{
				Caption = "Not so awesome game",
				WhiteName = "White player 3",
				BlackName = "Black player 3",
				Board = BoardFactory.ConstructAnotherBoard(),
			};
		}
	}
}
