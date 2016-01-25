using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models;

namespace Chess.MvcClient.Repositories
{
	public interface IGameRepository
	{
		Task<List<Game>> GetAllGamesAsync();
		Task<Game> GetGameAsync(string id);
		Task SaveGameAsync(Game game);
		Task<List<Game>> GetGamesByUserIdAsync(string userId);
	}
}
