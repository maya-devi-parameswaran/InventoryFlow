// Entities/Product.cs
using InventoryFlow.Domain.Common;
using InventoryFlow.Domain.Exceptions;

namespace InventoryFlow.Domain.Entities;

public class Product : BaseEntity
{
    public string Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string Category { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private Product() { } // EF Core

    public static Product Create(string sku, string name, decimal price, string category, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU is required.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");
        if (price <= 0)
            throw new DomainException("Price must be greater than zero.");

        return new Product
        {
            Sku = sku.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Price = price,
            Category = category.Trim(),
            Description = description
        };
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new DomainException("Price must be greater than zero.");
        Price = newPrice;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}