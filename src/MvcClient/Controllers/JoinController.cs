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

		//GET: api/join?info=5555
		[HttpGet]
		public async Task<IActionResult> GetAvailability(string info)
		{
			if (string.IsNullOrEmpty(_user.AuthString))
				return HttpBadRequest();

			var gameId = info;
			if (string.IsNullOrEmpty(gameId))
				return HttpNotFound();

			var game = await _repository.GetGameAsync(gameId);
			if (game == null)
				return HttpNotFound();

			int result = 0;

			if (Equals(_user.AuthString, game.WhiteId))
				result |= (int)Color.White;

			if (Equals(_user.AuthString, game.BlackId))
				result |= (int)Color.Black;

			if (game.WhiteId == null)
				result |= (int)Color.White;

			if (game.BlackId == null)
				result |= (int)Color.Black;

			return new ObjectResult(result);
		}

		// PUT: api/join/5555?color=1
		[HttpPut("{gameId}")]
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

			if (_user.AuthString.Equals(game.WhiteId) && color == Color.White)
				return new ObjectResult((int)Color.White);
			if (_user.AuthString.Equals(game.BlackId) && color == Color.Black)
				return new ObjectResult((int)Color.Black);

			if (game.WhiteId == null && color == Color.White)
			{
				game.WhiteId = _user.AuthString;
				await _repository.SaveGameAsync(game);
				return new ObjectResult((int)Color.White);
			}

			if (game.BlackId == null && color == Color.Black)
			{
				game.BlackId = _user.AuthString;
				await _repository.SaveGameAsync(game);
				return new ObjectResult((int)Color.Black);
			}

			return new ObjectResult(0);
		}
	}
}
