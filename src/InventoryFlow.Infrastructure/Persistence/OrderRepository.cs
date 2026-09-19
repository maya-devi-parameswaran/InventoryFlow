using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context) => _context = context;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Orders.FindAsync([id], cancellationToken);

    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        => await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

    public async Task AddAsync(Order entity, CancellationToken cancellationToken = default)
        => await _context.Orders.AddAsync(entity, cancellationToken);

    public void Update(Order entity) => _context.Orders.Update(entity);

    public void Remove(Order entity) => _context.Orders.Remove(entity);
}