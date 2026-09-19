// Common/Interfaces/IOrderRepository.cs
using InventoryFlow.Domain.Entities;

namespace InventoryFlow.Application.Common.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}