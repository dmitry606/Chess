using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Chess.MvcClient.Repositories;
using Chess.MvcClient.Infrastructure;
using Microsoft.Extensions.Logging;
using Chess.Engine;
using Microsoft.AspNet.Mvc.ModelBinding;
using Chess.Models;

namespace Chess.MvcClient.Controllers
{
    [Route("api/[controller]")]
    public class JoinController : Controller
    {
		private readonly IGameRepository _repository;
		private readonly IUserInfo _user;

		public JoinController(IGameRepository repository, IUserInfo userInfo)
		{
			_repository = repository;
			_user = userInfo;
		}

		public enum Availability
		{
			Closed = 0,
			Open = 1,
			Joined = 2
		}

		//GET api/join/5555
		[HttpGet("{gameId}")]
		public async Task<IActionResult> GetAvailability(string gameId)
		{
			if (string.IsNullOrEmpty(_user.AuthString))
				return HttpBadRequest();

			if (string.IsNullOrEmpty(gameId))
				return HttpNotFound();

			var game = await _repository.GetGameAsync(gameId);
			if (game == null)
				return HttpNotFound();

			return new ObjectResult(GetAvailability(game));
		}

		// PATCH api/join/5555?color=1
		[HttpPatch("{gameId}")]
        public async Task<IActionResult> Join(string gameId, [FromQuery]Color color)
        {
			if (string.IsNullOrEmpty(_user.AuthString))
				return HttpBadRequest();
			if(!Enum.IsDefined(typeof(Color), color))
				return HttpBadRequest();
			if (string.IsNullOrEmpty(gameId))
				return HttpNotFound();
			
			var game = await _repository.GetGameAsync(gameId);
			if (game == null)
				return HttpNotFound();

			if (game.WhiteId == null && color == Color.White)
			{
				game.WhiteId = _user.AuthString;
				await _repository.SaveGameAsync(game);
			}

			if (game.BlackId == null && color == Color.Black)
			{
				game.BlackId = _user.AuthString;
				await _repository.SaveGameAsync(game);
			}

			return new ObjectResult(GetAvailability(game));
		}

		private int[] GetAvailability(Game game)
		{
			Availability white, black;

			if (Equals(_user.AuthString, game.WhiteId))
				white = Availability.Joined;
			else if (game.WhiteId == null)
				white = Availability.Open;
			else
				white = Availability.Closed;

			if (Equals(_user.AuthString, game.BlackId))
				black = Availability.Joined;
			else if (game.BlackId == null)
				black = Availability.Open;
			else
				black = Availability.Closed;

			return new[] { (int)white, (int)black };
		}
	}
}
