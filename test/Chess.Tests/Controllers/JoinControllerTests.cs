using Chess.Engine;
using Chess.Models;
using Chess.MvcClient.Controllers;
using Chess.MvcClient.Infrastructure;
using Chess.MvcClient.Repositories;
using Microsoft.AspNet.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Chess.Tests.Controllers
{
    public class JoinControllerTests
    {
		[Fact]
		public async void Join_can_join()
		{
			var repoMock = new Mock<IGameRepository>();
			var game = CreateGame();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var userInfo1 = new MockUserInfo { AuthString = "PL1" };
			var userInfo2 = new MockUserInfo { AuthString = "PL2" };
			var ctrl1 = new JoinController(repoMock.Object, userInfo1);
			var ctrl2 = new JoinController(repoMock.Object, userInfo2);

			var objResult = await ctrl1.Join(game.Id, Color.White) as ObjectResult;
			var result1 = (int)objResult.Value;
			objResult = await ctrl2.Join(game.Id, Color.Black) as ObjectResult;
			var result2 = (int)objResult.Value;

			Assert.Equal((int)Color.White, result1);
			Assert.Equal((int)Color.Black, result2);
			repoMock.Verify(r => r.SaveGameAsync(game), Times.Exactly(2));
		}

		[Fact]
		public async void Join_same_user()
		{
			var repoMock = new Mock<IGameRepository>();
			var game = CreateGame();
			game.WhiteId = "PL1";
			game.BlackId = "PL2";
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var userInfo1 = new MockUserInfo { AuthString = "PL1" };
			var userInfo2 = new MockUserInfo { AuthString = "PL2" };
			var ctrl1 = new JoinController(repoMock.Object, userInfo1);
			var ctrl2 = new JoinController(repoMock.Object, userInfo2);

			var objResult = await ctrl1.Join(game.Id, Color.White) as ObjectResult;
			var result1 = (int)objResult.Value;
			objResult = await ctrl2.Join(game.Id, Color.Black) as ObjectResult;
			var result2 = (int)objResult.Value;

			Assert.Equal((int)Color.White, result1);
			Assert.Equal((int)Color.Black, result2);
			repoMock.Verify(r => r.SaveGameAsync(game), Times.Never);
		}

		[Theory, MemberData(nameof(GetAvailableData))]
		public async void GetAvailableTest(Game game, IUserInfo userInfo, int expected)
		{
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var ctrl = new JoinController(repoMock.Object, userInfo);

			var objResult = await ctrl.GetAvailability(game.Id) as ObjectResult;
			var result = (int)objResult.Value;

			Assert.Equal(expected, result);
		}

		public static IEnumerable<object[]> GetAvailableData()
		{
			var userInfo = new MockUserInfo { AuthString = "PL" };
			var game = CreateGame();
			yield return new object[] { game, userInfo,  3};

			game = CreateGame();
			game.BlackId = "TTT";
			yield return new object[] { game, userInfo, 1 };

			game = CreateGame();
			game.WhiteId = "TTT";
			yield return new object[] { game, userInfo, 2 };

			game = CreateGame();
			game.WhiteId = "TTT";
			game.BlackId = "BBB";
			yield return new object[] { game, userInfo, 0 };

			game = CreateGame();
			game.WhiteId = "TTT";
			game.BlackId = userInfo.AuthString;
			yield return new object[] { game, userInfo, 2 };

			game = CreateGame();
			game.WhiteId = userInfo.AuthString;
			game.BlackId = "BBB";
			yield return new object[] { game, userInfo, 1 };

			game = CreateGame();
			game.WhiteId = userInfo.AuthString;
			yield return new object[] { game, userInfo, 3 };

			game = CreateGame();
			game.BlackId = userInfo.AuthString;
			yield return new object[] { game, userInfo, 3 };
		}

		private static Game CreateGame()
		{
			return new Game
			{
				Id = "UNIT_TEST_111",
				Caption = $"Test game",
				Board = Board.ConstructInitialBoard()
			};
		}

		private class MockUserInfo : IUserInfo
		{
			public string AuthString { get; set; }
		}
	}
}
