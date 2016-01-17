using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Chess.Models;
using Chess.Engine;
using Chess.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Hosting;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Chess.MvcClient.Controllers
{
    [Route("api/[controller]")]
    public class GameController : Controller
    {
		private readonly IGameRepository _repository;
		private readonly ILogger<GameController> _logger;

		public GameController(IGameRepository repository, ILogger<GameController> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id)
		{
			var game = await _repository.GetGameAsync(id);

			if (null == game)
			{
				return HttpNotFound();
			}

			return new ObjectResult(game);
		}

		[HttpGet("new")]
		public async Task<IActionResult> New()
		{
			var game = CreateNewGame();
			await _repository.SaveGameAsync(game);

			return new ObjectResult(game);
		}

		private static Game CreateNewGame()
		{
			return new Game
			{
				Caption = "New game",
				BlackName = "Black player",
				WhiteName = "White player",
				Board = Board.ConstructInitialBoard(),
			};
		}

		// GET: api/values
		//[HttpGet]
		//public IEnumerable<string> Get()
		//{
		//    return new string[] { "value1", "value2" };
		//}

		// POST api/values
		//[HttpPost]
		//public void Post([FromBody]string value)
		//{
		//}

		// PUT api/values/5
		//[HttpPut("{id}")]
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		// DELETE api/values/5
		//[HttpDelete("{id}")]
		//public void Delete(int id)
		//{
		//}
	}
}
