using InventoryFlow.Domain.Entities;
using InventoryFlow.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace InventoryFlow.IntegrationTests.Repositories;

public class OrderRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public OrderRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdWithItems_ShouldPersistOrderAndItemsCorrectly()
    {
        // Arrange
        var repository = new OrderRepository(_fixture.Context);
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-TEST-001", quantity: 3, unitPrice: 19.99m);
        order.AddItem(Guid.NewGuid(), "SKU-TEST-002", quantity: 1, unitPrice: 49.99m);

        // Act
        await repository.AddAsync(order);
        await _fixture.Context.SaveChangesAsync();

        // Clear EF's change tracker so the next fetch hits the database,
        // not just returns the in-memory object we already have
        _fixture.Context.ChangeTracker.Clear();

        var retrieved = await repository.GetByIdWithItemsAsync(order.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Items.Should().HaveCount(2);
        retrieved.TotalAmount.Should().Be(3 * 19.99m + 49.99m);
        retrieved.Status.Should().Be(order.Status);
    }

    [Fact]
    public async Task GetByOrderNumberAsync_WhenOrderExists_ShouldReturnIt()
    {
        var repository = new OrderRepository(_fixture.Context);
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-TEST-003", 1, 10.00m);

        await repository.AddAsync(order);
        await _fixture.Context.SaveChangesAsync();
        _fixture.Context.ChangeTracker.Clear();

        var found = await repository.GetByOrderNumberAsync(order.OrderNumber);

        found.Should().NotBeNull();
        found!.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        var repository = new OrderRepository(_fixture.Context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}