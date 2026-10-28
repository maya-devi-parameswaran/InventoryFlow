using InventoryFlow.Domain.Entities;

namespace InventoryFlow.Application.Common.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
