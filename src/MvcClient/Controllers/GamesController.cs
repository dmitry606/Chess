using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Chess.Models;
using Chess.Engine;
using Chess.MvcClient.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Hosting;
using Chess.MvcClient.Infrastructure;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Chess.MvcClient.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
		private readonly IGameRepository _repository;
		private readonly ILogger<GamesController> _logger;
		private readonly IUserInfo _user;

		public GamesController(IGameRepository repository, IUserInfo userInfo, ILogger<GamesController> logger)
		{
			_repository = repository;
			_logger = logger;
			_user = userInfo;
		}

		//GET api/games/
		public async Task<IActionResult> GetAll()
		{
			throw new NotImplementedException();
		}

		//GET api/games/new
		[HttpGet("new")]
		public async Task<IActionResult> NewGame()
		{
			var game = new Game
			{
				Caption = "New game",
				Board = Board.ConstructInitialBoard(),
			};

			await _repository.SaveGameAsync(game);

			return new ObjectResult(game.Id);
		}

		// GET api/games/55555
		[HttpGet("{id}")]
		public async Task<IActionResult> GetGame(string id)
		{
			var game = await _repository.GetGameAsync(id);

			if (null == game)
			{
				return HttpNotFound();
			}

			return new ObjectResult(game.Board);
		}

		// GET api/games/55555/pos=e4
		[HttpGet("{gameId}/pos={position}")]
		public async Task<IActionResult> GetPositions(string gameId, string position)
		{
			var game = await _repository.GetGameAsync(gameId);

			if (null == game)
			{
				return HttpNotFound();
			}

			var moves = game.Board.CurrentTurnPlayer.GetLegalMoves(position);
			return new ObjectResult(moves);
		}

		//PUT api/games/55555/ +[BodyParams]
		[HttpPut("{gameId}")]
		public async Task<IActionResult> MakeMove(string gameId, [FromBody]MakeMoveParams moveParams)
		{
			var game = await _repository.GetGameAsync(gameId);

			if (null == game)
			{
				return HttpNotFound();
			}

			if (!CurrentUserHasColor(game, game.Board.CurrentTurnColor))
			{
				return HttpUnauthorized();
			}

			game.Board.CurrentTurnPlayer.MakeMove(moveParams.fromPos, moveParams.toPos);
			await _repository.SaveGameAsync(game);
			return new NoContentResult();
		}

		public class MakeMoveParams
		{
			public string fromPos { get; set; }
			public string toPos { get; set; }
		}

		private bool CurrentUserHasColor(Game game, Color color)
		{
			if (string.IsNullOrEmpty(_user.AuthString))
				return false;

			if (color == Color.White)
				return _user.AuthString.Equals(game.WhiteId);

			if (color == Color.Black)
				return _user.AuthString.Equals(game.BlackId);

			throw new ArgumentOutOfRangeException();
		}
	}
}
