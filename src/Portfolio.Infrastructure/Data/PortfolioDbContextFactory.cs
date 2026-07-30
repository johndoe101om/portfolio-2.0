using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Infrastructure.Data;

public sealed class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    private const string LocalFallbackConnectionString =
        "Host=localhost;Database=portfolio_dev;Username=postgres;Password=postgres;Include Error Detail=true";

    public PortfolioDbContext CreateDbContext(string[] args)
    {
        LoadLocalEnvFile();

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ??
            Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection") ??
            LocalFallbackConnectionString;

        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(PortfolioDbContext).Assembly.GetName().Name);
                npgsql.EnableRetryOnFailure(3);
            })
            .Options;

        return new PortfolioDbContext(options);
    }

    private static void LoadLocalEnvFile()
    {
        foreach (var path in GetLocalEnvFileCandidates().Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(path))
                continue;

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith('#'))
                    continue;

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                var key = line[..separatorIndex].Trim();
                var value = line[(separatorIndex + 1)..].Trim();

                if (value.Length >= 2 &&
                    ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
                {
                    value = value[1..^1];
                }

                if (!string.IsNullOrWhiteSpace(key) && Environment.GetEnvironmentVariable(key) is null)
                    Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    private static IEnumerable<string> GetLocalEnvFileCandidates()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        yield return Path.Combine(currentDirectory, "src", "Portfolio.Api", ".env");
        yield return Path.Combine(currentDirectory, ".env");
        yield return Path.GetFullPath(Path.Combine(currentDirectory, "..", "Portfolio.Api", ".env"));
        yield return Path.Combine(AppContext.BaseDirectory, ".env");
    }
}
