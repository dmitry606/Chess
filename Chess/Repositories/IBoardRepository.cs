using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Engine;

namespace Chess.Repositories
{
	public interface IBoardRepository
	{
		Task<List<Board>> GetAllBoardsAsync();
		Task<Board> GetBoardAsync(string id);
		Task SaveBoardAsync(Board game);
	}
}
