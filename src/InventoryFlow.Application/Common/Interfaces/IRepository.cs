using InventoryFlow.Domain.Common;

namespace InventoryFlow.Application.Common.Interfaces;

// Generic repository — constrained to IAggregateRoot so nobody can
// accidentally create a repository for a child entity like OrderItem
public interface IRepository<T> where T : BaseEntity, IAggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}