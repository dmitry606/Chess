using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Chess.Engine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;

namespace Chess.Repositories.Concrete
{
	public class MongoBoardRepository : IBoardRepository
	{
		public const string CONNECTION_STRING_NAME = "Chess";
		public const string DEFAULT_DATABASE_NAME = "chess";
		public const string GAMES_COLLECTION_NAME = "boards";
		public const string COUNTERS_COLLECTION_NAME = "counters";

		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<Board> _boardsCollection;

		public MongoBoardRepository() : this(DEFAULT_DATABASE_NAME)
		{ }

		public MongoBoardRepository(string databaseName) 
		{
			var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME].ConnectionString;
			_client = new MongoClient(connectionString);
			_database = _client.GetDatabase(databaseName);
			_boardsCollection = _database.GetCollection<Board>(GAMES_COLLECTION_NAME);
        }

		public Task<List<Board>> GetAllBoardsAsync()
		{
			return _boardsCollection.Find(new BsonDocument()).ToListAsync();
		}

		public Task<Board> GetBoardAsync(string id)
		{
			return _boardsCollection.Find(g => g.Id == id).SingleAsync();
		}

		public async Task SaveBoardAsync(Board board)
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