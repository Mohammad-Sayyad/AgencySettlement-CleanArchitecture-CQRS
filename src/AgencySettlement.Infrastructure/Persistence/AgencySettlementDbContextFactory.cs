using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AgencySettlement.Infrastructure.Persistence;

public sealed class AgencySettlementDbContextFactory
    : IDesignTimeDbContextFactory<AgencySettlementDbContext>
{
    public AgencySettlementDbContext CreateDbContext(string[] args)
    {
        // dotnet ef uses the startup project's directory as the working directory.
        // In this project it is:
        // ...\src\AgencySettlement.API
        var apiDirectory = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .AddJsonFile(
                "appsettings.Development.json",
                optional: true,
                reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("AgencySettlementDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AgencySettlementDb' was not found.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<AgencySettlementDbContext>();

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(
                    typeof(AgencySettlementDbContext)
                        .Assembly
                        .GetName()
                        .Name);
            });

        return new AgencySettlementDbContext(
            optionsBuilder.Options);
    }
}