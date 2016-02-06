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
			var result1 = (int[])objResult.Value;
			objResult = await ctrl2.Join(game.Id, Color.Black) as ObjectResult;
			var result2 = (int[])objResult.Value;

			Assert.True(result1.SequenceEqual(new[] {2, 1}));
			Assert.True(result2.SequenceEqual(new[] {0, 2}));
			repoMock.Verify(r => r.SaveGameAsync(game), Times.Exactly(2));
		}

		[Fact]
		public async void Join_already_joined_user()
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
			var result1 = (int[])objResult.Value;
			objResult = await ctrl2.Join(game.Id, Color.Black) as ObjectResult;
			var result2 = (int[])objResult.Value;

			Assert.True(result1.SequenceEqual(new[] { 2, 0 }));
			Assert.True(result2.SequenceEqual(new[] { 0, 2 }));
			repoMock.Verify(r => r.SaveGameAsync(game), Times.Never);
		}

		[Fact]
		public async void Join_when_both_sides_are_same_user()
		{
			var game = CreateGame();
			var userInfo = new MockUserInfo { AuthString = "PL1" };
			game.WhiteId = game.BlackId = "PL1";
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var ctrl = new JoinController(repoMock.Object, userInfo);

			var objResult = await ctrl.Join(game.Id, Color.White) as ObjectResult;
			var result1 = (int[])objResult.Value;
			objResult = await ctrl.Join(game.Id, Color.Black) as ObjectResult;
			var result2 = (int[])objResult.Value;

			Assert.True(result1.SequenceEqual(new[] { 2, 2 }));
			Assert.True(result2.SequenceEqual(new[] { 2, 2 }));
			repoMock.Verify(r => r.SaveGameAsync(game), Times.Never);
		}

		[Theory, MemberData(nameof(GetAvailableData))]
		public async void GetAvailableTest(Game game, IUserInfo userInfo, int[] expected)
		{
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var ctrl = new JoinController(repoMock.Object, userInfo);

			var objResult = await ctrl.GetAvailability(game.Id) as ObjectResult;
			var result = (int[])objResult.Value;

			Assert.True(result.SequenceEqual(expected));
		}

		public static IEnumerable<object[]> GetAvailableData()
		{
			var userInfo = new MockUserInfo { AuthString = "PL" };
			var game = CreateGame();
			yield return new object[] { game, userInfo, new[]{1, 1} };

			game = CreateGame();
			game.WhiteId = "RANDOM1";
			yield return new object[] { game, userInfo, new[] { 0, 1 } };

			game = CreateGame();
			game.BlackId = "RANDOM1";
			yield return new object[] { game, userInfo, new[] { 1, 0 } };

			game = CreateGame();
			game.WhiteId = userInfo.AuthString;
			game.BlackId = "RANDOM1";
			yield return new object[] { game, userInfo, new[] { 2, 0 } };

			game = CreateGame();
			game.WhiteId = "RANDOM1";
			game.BlackId = userInfo.AuthString;
			yield return new object[] { game, userInfo, new[] { 0, 2 } };
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
