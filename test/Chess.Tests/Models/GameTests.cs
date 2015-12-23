using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models;
using Xunit;
using Newtonsoft.Json;

namespace Chess.Tests.Models
{
	public class GameTests
	{
		[Fact]
		public void Equals_and_GetHashCode_test()
		{
			var game1 = new Game();
			var game2 = new Game();
			Assert.Equal(game1, game2);
			Assert.NotEqual(game1, null);
			Assert.Equal(game1.GetHashCode(), game2.GetHashCode());

			game1 = GameFactory.ConstructSomeBoard();
			game2 = GameFactory.ConstructSomeBoard();
			Assert.Equal(game1, game2);
			Assert.Equal(game1.GetHashCode(), game2.GetHashCode());

			game2 = GameFactory.ConstructSomeBoard();
			game2.Caption = "diff";
			Assert.NotEqual(game1, game2);
			Assert.NotEqual(game1.GetHashCode(), game2.GetHashCode());

			game2 = GameFactory.ConstructSomeBoard();
			game2.CreatedAt = game2.CreatedAt.AddDays(1);
			Assert.NotEqual(game1, game2);
			Assert.NotEqual(game1.GetHashCode(), game2.GetHashCode());

			game2 = GameFactory.ConstructSomeBoard();
			game2.CreatedAt = game2.LastModifiedAt.AddHours(1);
			Assert.NotEqual(game1, game2);
			Assert.NotEqual(game1.GetHashCode(), game2.GetHashCode());

			game2 = GameFactory.ConstructSomeBoard();
			game2.BlackName = game2.BlackName + "diff";
			Assert.NotEqual(game1, game2);
			Assert.NotEqual(game1.GetHashCode(), game2.GetHashCode());

			game2 = GameFactory.ConstructSomeBoard();
			game2.Board.Black.PieceStrings.Add("pa1");
			Assert.NotEqual(game1, game2);
			Assert.NotEqual(game1.GetHashCode(), game2.GetHashCode());
		}

		[Fact]
		public void CanBeSerializedByNewtonJson()
		{
			var ser = new JsonSerializer
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				ContractResolver = new WritablePropertiesOnlyResolver()
			};
			var game = GameFactory.ConstructSomeBoard();
			game.CreatedAt = DateTime.Now;
			game.LastModifiedAt = DateTime.Now;

			var writer = new StringWriter();
			ser.Serialize(writer, game);

			var reader = new StringReader(writer.ToString());
			var actual = ser.Deserialize<Game>(new JsonTextReader(reader));

			Assert.Equal(game, actual);
		}
	}
}
