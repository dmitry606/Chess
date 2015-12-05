using Chess.Models;
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
		private readonly IBoardRepository _repository;
		private readonly GameState _game;

		public BoardController()
		{
		}

		public BoardController(IBoardRepository repo, GameState game)
		{
			_repository = repo;
			_game = game;
		}
			
		[HttpGet]
		[AllowAnonymous]
		public string SaySomething()
		{
			return "something";
		}

		[HttpPut]
		public async Task<MoveInfo> MakeMove(string from, string to, char? promotionTarget = null)
		{
			var board = await _repository.GetBoardAsync(_game.BoardId);
			var move = board[_game.CurrentPlayerColor].MakeMove(from, to, promotionTarget);
			await _repository.SaveBoardAsync(board);
			return move;
		}

		[HttpGet]
		public async Task<IEnumerable<MoveInfo>> GetLegalMoves(string position)
		{
			var board = await _repository.GetBoardAsync(_game.BoardId);
			return board[_game.CurrentPlayerColor].GetLegalMoves(position);
        }

		[HttpGet]
		public async Task<MoveInfo> GetLastMove()
		{
			var board = await _repository.GetBoardAsync(_game.BoardId);
			return board.PeekHistory();
		}
    }
}
