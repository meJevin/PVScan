using PVScan.Barcodes.API;

public class Program
{
	public static void Main(string[] args)
	{
		CreateHostBuilder(args).Build().Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((context, config) =>
			{
				config
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true)
					.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
					.AddEnvironmentVariables()
					.Build();
			})
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddConsole();
			})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
}