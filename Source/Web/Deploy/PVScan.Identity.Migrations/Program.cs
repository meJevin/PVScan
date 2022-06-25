using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PVScan.Identity.Infrastructure.Data;
using PVScan.Shared.Configurations;

namespace PVSCan.Identity.Migrations
{
    class Program : IDesignTimeDbContextFactory<PVScanIdentityDbContext>
    {
        public PVScanIdentityDbContext CreateDbContext(string[] args)
        {
            var environment = args.Length == 0 ? "Development" : args[0];

            var aspEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (aspEnv != null) environment = aspEnv;

            var jsonFileName = $"appsettings.{environment}.json";

            var configurationBuilder = new ConfigurationBuilder();
            var parentBaseDirectory = Directory.GetParent(AppContext.BaseDirectory)!.FullName;
            configurationBuilder
                .SetBasePath(parentBaseDirectory)
                .AddJsonFile(jsonFileName, true, true);

            _configuration = configurationBuilder.Build();
            var postgresSettings = new PostgresSettings();
            _configuration.GetSection(nameof(PostgresSettings)).Bind(postgresSettings);

            var optionsBuilder = new DbContextOptionsBuilder<PVScanIdentityDbContext>()
                .UseNpgsql(postgresSettings.ConnectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(Program).Assembly.FullName);
                });

            return new PVScanIdentityDbContext(optionsBuilder.Options);
        }

        private static IConfiguration _configuration;

        private static async Task Main(string[] args)
        {
            var p = new Program();

            using (var sc = p.CreateDbContext(args))
            {
                if (sc == null)
                {
                    Console.WriteLine("Could not create context. Exiting");
                    return;
                }

                if (sc.Database.GetPendingMigrations().Any())
                {
                    await sc.Database.MigrateAsync();
                }
            }

            Console.WriteLine("Identity migrations finished");
        }
    }
}