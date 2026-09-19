using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Infrastructure.Persistence.Repositories;

public class StockItemRepository : IStockItemRepository
{
    private readonly ApplicationDbContext _context;

    public StockItemRepository(ApplicationDbContext context) => _context = context;

    public async Task<StockItem?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default)
        => await _context.StockItems
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId, cancellationToken);

    public async Task<List<StockItem>> GetBelowReorderThresholdAsync(CancellationToken cancellationToken = default)
        => await _context.StockItems
            .Where(s => s.QuantityOnHand <= s.ReorderThreshold)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(StockItem stockItem, CancellationToken cancellationToken = default)
        => await _context.StockItems.AddAsync(stockItem, cancellationToken);
}