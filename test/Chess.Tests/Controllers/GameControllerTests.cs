using Chess.MvcClient.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using Microsoft.Extensions.Logging;
using Chess.MvcClient.Repositories.Concrete;
using Chess.MvcClient.Infrastructure;
using Chess.Engine;
using Chess.Models;
using Microsoft.AspNet.Mvc;
using Moq;
using Chess.MvcClient.Repositories;
using Xunit.Extensions;

namespace Chess.Tests.Controllers
{
	public class GameControllerTests
	{
		private static readonly ILogger<GamesController> NullLogger = new NullLogger<GamesController>();

		[Fact]
		public async void NewGame_creates_game()
		{
			var repoMock = new Mock<IGameRepository>();
			Game savedGame = null;
			repoMock.Setup(r => r.SaveGameAsync(It.IsAny<Game>()))
				.Returns(Task.Delay(0))
				.Callback<Game>(g => savedGame = g);

			var ctrl = new GamesController(repoMock.Object, new MockUserInfo(), NullLogger);
			var result = await ctrl.NewGame() as ObjectResult;

			Assert.NotNull(result);
			repoMock.Verify(r => r.SaveGameAsync(It.IsAny<Game>()), Times.Once);
			Assert.Null(savedGame.WhiteId);
			Assert.Null(savedGame.BlackId);
		}

		[Fact]
		public async void GetGame_returns_client_board()
		{
			var game = CreateGame();
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));

			var ctrl = new GamesController(repoMock.Object, new MockUserInfo(), NullLogger);
			var result = await ctrl.GetGame(game.Id) as ObjectResult;

			Assert.NotNull(result);
			Assert.NotNull(result.Value);
			Assert.IsType<Board>(result.Value);
			repoMock.Verify(r => r.GetGameAsync(game.Id), Times.Once);
		}

		[Fact]
		public async void GetGame_returns_HttpNotFound_if_no_game()
		{
			var test = GetWrongPlayerData();

			var repo = new InMemoryGameRepository();

			var ctrl = new GamesController(repo, new MockUserInfo(), NullLogger);
			var actionResult = await ctrl.GetGame("RANDOM_ID_");

			Assert.IsType<HttpNotFoundResult>(actionResult);
		}

		[Fact]
		public void MakeMove_makes_moves()
		{
			var playerMock = new Mock<Player>();
			var game = CreateGame();
			game.Board.White = playerMock.Object;
			var moveParam = new GamesController.MakeMoveParams
			{
				fromPos = "e2",
				toPos = "e4"
			};
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var whiteUserInfo = new MockUserInfo { AuthString = game.WhiteId };

			var ctrl = new GamesController(repoMock.Object, whiteUserInfo, NullLogger);
			ctrl.MakeMove(game.Id, moveParam);

			playerMock.Verify(p => p.MakeMove("e2", "e4", null), Times.Once);
			repoMock.Verify(r => r.SaveGameAsync(It.Is<Game>(g => g.Id == game.Id)), Times.Once);
		}

		[Fact]
		public void MakeMove_makes_moves_when_both_players_are_same_user()
		{
			var playerMock = new Mock<Player>();
			playerMock.Setup(p => p.MakeMove(It.IsAny<string>(), It.IsAny<string>(), null));
			var game = CreateGame();
			var boardMock = new Mock<Board>();
			game.Board = boardMock.Object;
			game.Board.White = playerMock.Object;
			game.Board.Black = playerMock.Object;
			var whiteMoveParams = new GamesController.MakeMoveParams
			{
				fromPos = "e2",
				toPos = "e4"
			};
			var blackMoveParams = new GamesController.MakeMoveParams
			{
				fromPos = "d7",
				toPos = "d6"
			};
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));
			var userInfo = new MockUserInfo { AuthString = "PL1" };
			game.BlackId = userInfo.AuthString;
			game.WhiteId = userInfo.AuthString;

			boardMock.SetupGet(b => b.CurrentTurnColor).Returns(Color.White);
			var ctrl = new GamesController(repoMock.Object, userInfo, NullLogger);
			ctrl.MakeMove(game.Id, whiteMoveParams);
			boardMock.SetupGet(b => b.CurrentTurnColor).Returns(Color.Black);
			ctrl.MakeMove(game.Id, blackMoveParams);

			playerMock.Verify(p => p.MakeMove("e2", "e4", null), Times.Exactly(1));
			playerMock.Verify(p => p.MakeMove("d7", "d6", null), Times.Exactly(1));
			repoMock.Verify(r => r.SaveGameAsync(It.Is<Game>(g => g.Id == game.Id)), Times.Exactly(2));

		}

		[Theory, MemberData(nameof(GetWrongPlayerData))]
		public async void MakeMove_dont_move_when_wrong_player(Game game, IUserInfo userInfo)
		{
			var playerMock = new Mock<Player>();
			game.Board.White = playerMock.Object;
			var param = new GamesController.MakeMoveParams
			{
				fromPos = "e2",
				toPos = "e4"
			};
			var repoMock = new Mock<IGameRepository>();
			repoMock.Setup(r => r.GetGameAsync(game.Id)).Returns(() => Task.FromResult(game));

			var ctrl = new GamesController(repoMock.Object, userInfo, NullLogger);
			var result = await ctrl.MakeMove(game.Id, param);

			Assert.IsType<HttpUnauthorizedResult>(result);
			playerMock.Verify(p => p.MakeMove(It.IsAny<string>(), It.IsAny<string>(), null), Times.Never);
			repoMock.Verify(r => r.SaveGameAsync(It.IsAny<Game>()), Times.Never);
		}

		public static IEnumerable<object[]> GetWrongPlayerData()
		{
			var game = CreateGame();
			yield return new object[] { game, new MockUserInfo { AuthString = null } };

			game = CreateGame();
			yield return new object[] { game, new MockUserInfo { AuthString = game.BlackId } };

			game = CreateGame();
			game.WhiteId = null;
			yield return new object[] { game, new MockUserInfo { AuthString = game.BlackId } };

			game = CreateGame();
			game.WhiteId = null;
			yield return new object[] { game, new MockUserInfo { AuthString = null } };
		}

		private static Game CreateGame()
		{
			return new Game
			{
				Id = "UNIT_TEST_111",
				Caption = $"Test game",
				BlackId = "Black player",
				WhiteId = "White player",
				Board = Board.ConstructInitialBoard()
			};
		}

		private class MockUserInfo : IUserInfo
		{
			public string AuthString { get; set; }
		}
	}
}
