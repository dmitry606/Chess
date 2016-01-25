using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chess.MvcClient.Repositories.Concrete;

namespace Chess.MvcClient.Repositories
{
    public static class ConfigureRepositoryExtensions
    {
		public static void AddRepositories(this IServiceCollection services)
		{
			//MongoMappingConfig.Configure();

			//services.AddTransient<IGameRepository>(s => new MongoGameRepository(
			//	Configuration["Data:MongoDB:ConnectionString"],
			//	Configuration["Data:MongoDB:DatabaseName"]));
			services.AddSingleton<IGameRepository>(s => new InMemoryGameRepository());
		}
	}
}
