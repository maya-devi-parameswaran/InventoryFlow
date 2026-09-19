using InventoryFlow.Domain.Entities;

namespace InventoryFlow.Application.Common.Interfaces;

public interface IStockItemRepository
{
    Task<StockItem?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<List<StockItem>> GetBelowReorderThresholdAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StockItem stockItem, CancellationToken cancellationToken = default);
}