using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Chess.Repositories.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Chess.Models;
using Chess.Engine;
using Chess.Tests;

namespace Chess.Repositories.Concrete.Tests
{
	[TestClass()]
	public class MongoRepositoryTests
	{
		private const string DATABASE_NAME = "Chess_unit_testing";

		[ClassInitialize()]
		public static void ClassInit(TestContext context)
		{
			var connectionString = ConfigurationManager.ConnectionStrings[MongoGameRepository.CONNECTION_STRING_NAME].ConnectionString;
			var client = new MongoClient(connectionString);
			client.DropDatabaseAsync(DATABASE_NAME).Wait();

			App_Start.MongoMappingConfig.MapClasses();
		}

		[TestInitialize]
		public void TestInit()
		{
			GetDatabase().DropCollectionAsync(MongoGameRepository.GAMES_COLLECTION_NAME).Wait();
		}

		[TestMethod()]
		public void Can_be_created()
		{
			var rep = GetRepository();
			Assert.IsNotNull(rep);
		}

		[TestMethod()]
		public void New_board_can_be_saved_one()
		{
			var rep = GetRepository();
			var board = new Game { Board = new Board() };
			Assert.AreEqual(board.Id, null);

			rep.SaveGameAsync(board).Wait();

			Assert.IsFalse(string.IsNullOrEmpty(board.Id));
			Assert.IsTrue(Exists(board.Id));
		}

		[TestMethod()]
		public void New_board_can_be_saved_multiple()
		{
			var rep = GetRepository();
			var boards = Enumerable.Range(0, 20).Select(i => new Game { Board = new Board() }).ToList();

			var tasks = boards.Select(g => rep.SaveGameAsync(g)).ToArray();
			Task.WaitAll(tasks);

			Assert.IsTrue(boards.All(g => !string.IsNullOrEmpty(g.Id)));
			Assert.IsTrue(boards.Select(g => g.Id).Distinct().Count() == boards.Count);
			Assert.IsTrue(Exist(boards.Select(g => g.Id).ToList()));
		}

		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void Board_with_non_existent_id_is_not_saved()
		{
			var rep = GetRepository();
			var board = GameFactory.ConstructSomeBoard();
			board.Id = ObjectId.GenerateNewId().ToString();

			UnpackAggregateExceptionSingle(
				delegate { rep.SaveGameAsync(board).Wait(); }
			);

			Assert.Fail();
		}

		[TestMethod()]
		public void Board_can_be_saved_and_read_correctly()
		{
			var rep = GetRepository();
			var original = GameFactory.ConstructSomeBoard();

			rep.SaveGameAsync(original).Wait();
			var retrieved = rep.GetGameAsync(original.Id).GetAwaiter().GetResult();

			Assert.IsFalse(ReferenceEquals(original, retrieved));
			Assert.AreEqual(original, retrieved);
		}

		[TestMethod()]
		public void Board_can_be_changed()
		{
			var rep = GetRepository();
			var original = GameFactory.ConstructSomeBoard();

			rep.SaveGameAsync(original).Wait();
			var retrieved = rep.GetGameAsync(original.Id).GetAwaiter().GetResult();

			var addedEntry = new HistoryEntry("Ka1", "b1", MoveType.Regular);
            retrieved.Board.History.Add(addedEntry);
            rep.SaveGameAsync(retrieved).Wait();

			var actual = rep.GetGameAsync(original.Id).GetAwaiter().GetResult();
			var expectedHistory = original.Board.History.ToList();
			expectedHistory.Add(addedEntry);
			Assert.IsTrue(expectedHistory.SequenceEqual(actual.Board.History));
		}

		[TestMethod]
		public void GetAll_works()
		{
			var rep = GetRepository();
			var boards = Enumerable.Range(0, 20).Select(i => GameFactory.ConstructSomeBoard()).ToList();

			var tasks = boards.Select(g => rep.SaveGameAsync(g)).ToArray();
			Task.WaitAll(tasks);
			var actual = rep.GetAllGamesAsync().GetAwaiter().GetResult();

			Comparison<Game> comp = (a, b) => a.Id.CompareTo(b.Id);
			boards.Sort(comp);
			actual.Sort(comp);
			Assert.IsTrue(boards.SequenceEqual(actual));
		}

		private IMongoDatabase GetDatabase()
		{
			var connectionString = ConfigurationManager.ConnectionStrings[MongoGameRepository.CONNECTION_STRING_NAME].ConnectionString;
			var client = new MongoClient(connectionString);
			return client.GetDatabase(DATABASE_NAME);
		}

		private IMongoCollection<BsonDocument> GetboardsCollection()
		{
			return GetDatabase().GetCollection<BsonDocument>(MongoGameRepository.GAMES_COLLECTION_NAME);
        }

		private bool Exists(string boardId)
		{
			if (string.IsNullOrEmpty(boardId))
				throw new ArgumentException();

			var ret = GetboardsCollection()
				.Find(new BsonDocument("_id", new ObjectId(boardId)))
				.Project(new BsonDocument("_id", 1))
				.SingleOrDefaultAsync()
				.GetAwaiter().GetResult();

			return ret != null;
		}

		private bool Exist(List<string> boardIds)
		{
			if (boardIds == null)
				throw new ArgumentNullException();

			var ret = GetboardsCollection()
				.Find(Builders<BsonDocument>.Filter.In(
					"_id", boardIds.Select(id => new ObjectId(id))))
				.Project(new BsonDocument("_id", 1))
				.ToListAsync()
				.GetAwaiter().GetResult();

			return ret.Count == boardIds.Count;
		}


		private IGameRepository GetRepository()
		{
			return new MongoGameRepository(DATABASE_NAME);
		}

		private void UnpackAggregateExceptionSingle(Action action)
		{
			try
			{
				action();
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.Count == 1)
					throw e.InnerExceptions.First();
				throw;
			}
		}
	}
}