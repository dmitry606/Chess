using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Chess.Engine;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Conventions;

namespace Chess.App_Start
{
	public class MongoMappingConfig
	{
		public static void MapClasses()
		{
			BsonClassMap.RegisterClassMap<Board>(cm =>
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