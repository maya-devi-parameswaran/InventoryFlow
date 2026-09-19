using InventoryFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace InventoryFlow.IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("inventoryflow_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public ApplicationDbContext Context { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        Context = new ApplicationDbContext(options);

        // Applies your real EF Core migrations against this throwaway container —
        // this is the part that actually proves the schema is correct
        await Context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _container.DisposeAsync();
    }
}