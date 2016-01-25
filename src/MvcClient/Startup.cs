using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Chess.MvcClient.Repositories.Concrete;
using Chess.MvcClient.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Diagnostics;
using Newtonsoft.Json.Serialization;
using Chess.Tests.Models;
using Microsoft.Extensions.Logging.Console.Internal;
using Chess.MvcClient.Infrastructure;

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

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddRepositories();

			services.AddScoped<IUserInfo>(s => new UserInfo());

			services
				.AddMvc()
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling =
						Newtonsoft.Json.ReferenceLoopHandling.Ignore;
					options.SerializerSettings.ContractResolver = new WritablePropertiesOnlyResolver();
				});
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			var logSection = Configuration.GetSection("Logging");
			loggerFactory.AddConsole(logSection);
			loggerFactory.AddFileDestination(logSection);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseRuntimeInfoPage(); // default path is /runtimeinfo
			}
			
            app.UseIISPlatformHandler();
			app.UseStaticFiles();
			app.UseMiddleware<AuthCookiesMiddleware>();

			app.UseMvc(routes =>
			{
				var endpoint = new { controller = "Home", action = "Index" };
				routes.MapRoute("home", "", endpoint);
				routes.MapRoute("game", "game/{id}", endpoint);
			});

			app.Run(async (context) =>
			{
				var logger = loggerFactory.CreateLogger("Catchall Endpoint");
				var message = $"No endpoint found for request '{context.Request.Path}'"; 
				logger.LogInformation(message);
				await context.Response.WriteAsync(message);
			});
		}

		// Entry point for the application.
		public static void Main(string[] args)
		{
			WebApplication.Run<Startup>(args);
		}
    }
}
