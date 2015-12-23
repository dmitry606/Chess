using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Chess.Models;
using Chess.Engine;
using Chess.Repositories;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Chess.MvcClient.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
		private readonly IGameRepository _repository;

		public GamesController(IGameRepository repository)
		{
			_repository = repository;
		}

		// GET api/values/5
		//[HttpGet("{id}")]
		//public async Task<Game> Get(string id)
		//{
		//	return await _repository.GetGameAsync(id);
		//}

		[HttpGet()]
		public async Task<Game> New()
		{
			return new Game
			{
				Id = null,
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
