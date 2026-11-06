using FluentAssertions;
using InventoryFlow.Application.Common.Exceptions;
using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Application.Features.Orders.Commands.CreateOrder;
using InventoryFlow.Domain.Entities;
using InventoryFlow.Domain.Exceptions;
using InventoryFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace InventoryFlow.IntegrationTests.Orders
{
    public class ConcurrentOrderTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly ITestOutputHelper _output;

        public ConcurrentOrderTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public async Task ConcurrentOrders_ShouldNeverReserveMoreThanAvailableStock()
        {
            const int availableUnits = 5;
            const int concurrentRequests = 12;

            // Arrange: one product, one warehouse, deliberately limited stock
            Guid productId, warehouseId;
            await using (var seedContext = _fixture.CreateContext())
            {
                var product = Product.Create($"SKU-{Guid.NewGuid():N}", "Concurrency Widget", 10m, "Test");
                var warehouse = Warehouse.Create("Test Warehouse", "Test City");

                seedContext.Products.Add(product);
                seedContext.Warehouses.Add(warehouse);
                seedContext.StockItems.Add(StockItem.Create(product.Id, warehouse.Id, availableUnits, reorderThreshold: 1));
                await seedContext.SaveChangesAsync();

                productId = product.Id;
                warehouseId = warehouse.Id;
            }

            var command = new CreateOrderCommand(
           warehouseId,
           new List<OrderLineRequest> { new(productId, 1) });

            // Act: every request gets its own DbContext, and all wait at a starting gate
            // so they hit the database at the same moment
            var startGate = new TaskCompletionSource();

            var tasks = Enumerable.Range(0, concurrentRequests).Select(async _ =>
            {
                await using var context = _fixture.CreateContext();
                var handler = new CreateOrderCommandHandler(
                    new UnitOfWork(context),
                    new FakeCurrentUser(Guid.NewGuid()));

                await startGate.Task;

                try
                {
                    await handler.Handle(command, CancellationToken.None);
                    return (Exception?)null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }).ToList();

            startGate.SetResult();
            var outcomes = await Task.WhenAll(tasks);

            // Assert
            var succeeded = outcomes.Count(e => e is null);
            var outOfStock = outcomes.Count(e => e is InsufficientStockException);
            var retriesExhausted = outcomes.Count(e => e is ConcurrencyException);

            _output.WriteLine($"Succeeded: {succeeded}, out of stock: {outOfStock}, retries exhausted: {retriesExhausted}");

            outcomes.Where(e => e is not null)
                .Should().OnlyContain(
                    e => e is InsufficientStockException || e is ConcurrencyException,
                    "the only acceptable failures are 'out of stock' or 'retries exhausted'");

            await using var verifyContext = _fixture.CreateContext();

            var stock = await verifyContext.StockItems
                .AsNoTracking()
                .SingleAsync(s => s.ProductId == productId);

            var orders = await verifyContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.WarehouseId == warehouseId)
                .ToListAsync();

            var orderedQuantity = orders.Sum(o => o.Items.Sum(i => i.Quantity));

            stock.QuantityReserved.Should().BeLessThanOrEqualTo(availableUnits, "we must never oversell");
            stock.QuantityReserved.Should().Be(succeeded, "every successful order reserved exactly 1 unit");
            orderedQuantity.Should().Be(stock.QuantityReserved, "orders and reservations must agree");
            orders.Should().HaveCount(succeeded);
            stock.QuantityOnHand.Should().Be(availableUnits, "reserving must not remove stock from the shelf");

        }

        private sealed class FakeCurrentUser(Guid userId) : ICurrentUserService
        {
            public Guid? UserId => userId;
        }
    }
}
