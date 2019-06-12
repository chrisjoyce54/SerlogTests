using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SerilogMultipleSinks
{
	public class Program
	{
		public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		public static void Main(string[] args)
		{
			try
			{
				var file = File.CreateText(Path.Combine(Configuration.GetSection("DataFolder").Value, "self2.log"));
				Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(file));
				Log.Information("PSCU Listener service starting ...");
				Log.Verbose("Configuring Logger");

				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(Configuration)
					.CreateLogger();

				Log.Verbose("lOGGER cONFIGURED");

				CreateWebHostBuilder(args)
					.UseSerilog()
					.Build().Run();

				Log.Warning("Service Started");
			}
			catch (Exception e)
			{

			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
