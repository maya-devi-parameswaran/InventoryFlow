// Persistence/UnitOfWork.cs
using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Infrastructure.Persistence.Repositories;

namespace InventoryFlow.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IOrderRepository Orders { get; }
    public IStockItemRepository StockItems { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Orders = new OrderRepository(context);
        StockItems = new StockItemRepository(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}