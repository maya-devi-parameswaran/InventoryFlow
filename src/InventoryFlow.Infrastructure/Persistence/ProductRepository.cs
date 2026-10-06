using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products.FindAsync([id], cancellationToken);

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        => await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Products.ToListAsync(cancellationToken);

    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
        => await _context.Products.AddAsync(entity, cancellationToken);

    public void Update(Product entity) => _context.Products.Update(entity);

    public void Remove(Product entity) => _context.Products.Remove(entity);
}