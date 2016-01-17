﻿using Chess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chess.Models;
using MongoDB.Bson;
using Chess.Engine;

namespace Chess.MvcClient.Repositories.Concrete
{
	public class InMemoryRepository : IGameRepository
	{
		private Dictionary<string, Game> _data = new Dictionary<string, Game>
		{
			["606"] = new Game
			{
				Caption = "Classic",
				BlackName = "Black player",
				WhiteName = "White player",
				Board = Board.ConstructInitialBoard(),
			}
		};

		public async Task<List<Game>> GetAllGamesAsync()
		{
			return _data.Values.ToList();
		}

		public async Task<Game> GetGameAsync(string id)
		{
			Game game;
			_data.TryGetValue(id, out game);
			return game;
		}

		public async Task SaveGameAsync(Game game)
		{
			var isUpsert = false;

			game.LastModifiedAt = DateTime.UtcNow; //TODO: revert on exception?

			if (game.Id == null)
			{
				game.Id = ObjectId.GenerateNewId().ToString();
				game.CreatedAt = game.LastModifiedAt;
				isUpsert = true;
			}

			if(!isUpsert && !_data.ContainsKey(game.Id))
				throw new ArgumentException($"Board with id '{game.Id}' not found");

			_data[game.Id] = game;
		}
	}
}
