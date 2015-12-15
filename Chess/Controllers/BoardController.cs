using Chess.Models;
using Chess.Engine;
using Chess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Chess.Controllers
{
	public class GameState : IDisposable
	{
		public string BoardId { get; set; }
		public Color CurrentPlayerColor { get; }

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}

	public class BoardController : ApiController
    {
		private readonly IGameRepository _repository;
		private readonly GameState _gameStatus;

		public BoardController()
		{
		}

		public BoardController(IGameRepository repo, GameState game)
		{
			_repository = repo;
			_gameStatus = game;
		}
			
		[HttpGet]
		[AllowAnonymous]
		public string SaySomething()
		{
			return "something";
		}

		[HttpPut]
		public async Task<Game> MakeMove(string from, string to, char? promotionTarget = null)
		{
			var game = await _repository.GetGameAsync(_gameStatus.BoardId);
			game.Board[_gameStatus.CurrentPlayerColor].MakeMove(from, to, promotionTarget);
			await _repository.SaveGameAsync(game);
			return game;
		}

		[HttpGet]
		public async Task<IEnumerable<MoveOption>> GetLegalMoves(string position)
		{
			var game = await _repository.GetGameAsync(_gameStatus.BoardId);
			return game.Board[_gameStatus.CurrentPlayerColor].GetLegalMoves(position);
        }

		[HttpGet]
		public async Task<HistoryEntry> GetLastMove()
		{
			var game = await _repository.GetGameAsync(_gameStatus.BoardId);
			return game.Board.PeekHistory();
		}
    }
}
