using System.Text.Json;
using eShop.NTier.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eShop.NTier.DataAccess;

public class CatalogDbContextFactory
    : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var configPath = Path.Combine(
            currentDirectory,
            "eShop.NTier.API",
            "appsettings.json");

        if (!File.Exists(configPath))
        {
            configPath = Path.Combine(
                currentDirectory,
                "appsettings.json");
        }

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException(
                "appsettings.json tidak ditemukan.");
        }

        using var document = JsonDocument.Parse(
            File.ReadAllText(configPath));

        var connectionString = document.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("CatalogDatabase")
            .GetString();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "CatalogDatabase connection string kosong.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<CatalogDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new CatalogDbContext(optionsBuilder.Options);
    }
}