// Persistence/UnitOfWork.cs
using InventoryFlow.Application.Common.Exceptions;
using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IOrderRepository Orders { get; }
    public IStockItemRepository StockItems { get; }

    public IProductRepository Products { get; }

    public IWarehouseRepository Warehouses { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Orders = new OrderRepository(context);
        StockItems = new StockItemRepository(context);
        Products = new ProductRepository(context);
        Warehouses = new WarehouseRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            throw new ConcurrencyException("The data was modified by another request.", ex);
        }       
    }

    public void ClearChangeTracker()
    {
        _context.ChangeTracker.Clear();
    }
}