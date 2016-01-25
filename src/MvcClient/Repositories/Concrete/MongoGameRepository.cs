using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Chess.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;

namespace Chess.MvcClient.Repositories.Concrete
{
	public class MongoGameRepository : IGameRepository
	{
		public const string GAMES_COLLECTION_NAME = "games";
		//public const string COUNTERS_COLLECTION_NAME = "counters";

		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<Game> _boardsCollection;

		public MongoGameRepository(string connString, string databaseName) 
		{
			_client = new MongoClient(connString);
			_database = _client.GetDatabase(databaseName);
			_boardsCollection = _database.GetCollection<Game>(GAMES_COLLECTION_NAME);
        }

		public Task<List<Game>> GetAllGamesAsync()
		{
			return _boardsCollection.Find(new BsonDocument()).ToListAsync();
		}

		public Task<Game> GetGameAsync(string id)
		{
			return _boardsCollection.Find(g => g.Id == id).SingleAsync();
		}

		public async Task SaveGameAsync(Game board)
		{
			var isUpsert = false;

			board.LastModifiedAt = DateTime.UtcNow; //TODO: revert on exception?

            if (board.Id == null)
			{
				board.Id = ObjectId.GenerateNewId().ToString();
				board.CreatedAt = board.LastModifiedAt;
				isUpsert = true;
			}

			var ret = await _boardsCollection.ReplaceOneAsync(
				g => g.Id == board.Id, board, new UpdateOptions { IsUpsert = isUpsert });

			if (!isUpsert && ret.ModifiedCount != 1)
				throw new ArgumentException($"Board with id '{board.Id}' not found");

			if(isUpsert && ret.UpsertedId.ToString() != board.Id)
			{
				Debug.Fail("Document was not inserted with an intended id");
				board.Id = ret.UpsertedId.ToString();
			}
		}

		public async Task<List<Game>> GetGamesByUserIdAsync(string userId)
		{
			throw new NotImplementedException();
		}


		////https://docs.mongodb.org/manual/tutorial/create-an-auto-incrementing-field/
		//private int GetNextGameId()
		//{
		//	var name = "games_id";
		//	var col = _database.GetCollection<BsonDocument>(COUNTERS_COLLECTION_NAME);
		//	var ret = col.FindOneAndUpdateAsync(
		//		new BsonDocument("_id", name),
		//		Builders<BsonDocument>.Update.Inc("seq", 1),
		//		new FindOneAndUpdateOptions<BsonDocument>
		//		{
		//			IsUpsert = true,
		//			Projection = new BsonDocument("seq", 1),
		//			ReturnDocument = ReturnDocument.After
		//		}
		//	).GetAwaiter().GetResult();
		//	return ret["seq"].AsInt32;
		//}
	}
}