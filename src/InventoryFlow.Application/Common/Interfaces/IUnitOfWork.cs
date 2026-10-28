namespace InventoryFlow.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IOrderRepository Orders { get; }
    IStockItemRepository StockItems { get; }
    IProductRepository Products { get; }
    IWarehouseRepository Warehouses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}