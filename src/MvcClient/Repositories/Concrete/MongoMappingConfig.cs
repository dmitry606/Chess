using Chess.Engine;
using Chess.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.MvcClient.Repositories.Concrete
{
    public static class MongoMappingConfig
    {
		public static void Configure()
		{
			BsonClassMap.RegisterClassMap<Game>(cm =>
			{
				cm.AutoMap();
				cm.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
			});

			BsonClassMap.RegisterClassMap<Player>(cm =>
			{
				cm.AutoMap();
				cm.UnmapProperty(p => p.Board);
			});

		}

	}
}
