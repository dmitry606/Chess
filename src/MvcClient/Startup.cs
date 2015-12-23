using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Chess.Repositories.Concrete;
using Chess.Repositories;
using Chess.MvcClient.Repositories.Concrete;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Diagnostics;
using Newtonsoft.Json.Serialization;
using Chess.Tests.Models;

namespace MvcClient
{
    public class Startup
    {
		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				//builder.AddUserSecrets();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddTransient<IGameRepository>(s => new MongoGameRepository(
				Configuration["Data:MongoDB:ConnectionString"],
				Configuration["Data:MongoDB:DatabaseName"]));


			services
				.AddMvc()
				.AddJsonOptions(options => {
					options.SerializerSettings.ReferenceLoopHandling =
						Newtonsoft.Json.ReferenceLoopHandling.Ignore;
					options.SerializerSettings.ContractResolver = new WritablePropertiesOnlyResolver();
				});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();
			//loggerFactory.AddConsole(minLevel: LogLevel.Verbose);
			
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseRuntimeInfoPage(); // default path is /runtimeinfo
			}


			MongoMappingConfig.Configure();

            app.UseIISPlatformHandler();

			app.UseStaticFiles();

			app.UseMvc();

			app.UseExceptionHandler(errorApp =>
			{
				errorApp.Run(context =>
				{
					throw new NotImplementedException();
				});
			});

			app.Run(async (context) =>
			{
				var logger = loggerFactory.CreateLogger("Catchall Endpoint");
				logger.LogInformation("No endpoint found for request {path}", context.Request.Path);
				await context.Response.WriteAsync("No endpoint found!");
			});
		}

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
