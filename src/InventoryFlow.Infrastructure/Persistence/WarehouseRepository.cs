using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Infrastructure.Persistence
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseRepository(ApplicationDbContext context) => _context = context;

        public async Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Warehouses.FindAsync([id], cancellationToken);

        public async Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Warehouses.ToListAsync(cancellationToken);

        public async Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default)
            => await _context.Warehouses.AddAsync(entity, cancellationToken);

        public void Update(Warehouse entity) => _context.Warehouses.Update(entity);
        public void Remove(Warehouse entity) => _context.Warehouses.Remove(entity);
    }
}
