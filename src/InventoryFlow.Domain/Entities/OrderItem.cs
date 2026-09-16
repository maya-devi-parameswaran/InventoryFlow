// Entities/OrderItem.cs
using InventoryFlow.Domain.Common;
using InventoryFlow.Domain.Exceptions;

namespace InventoryFlow.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductSku { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal => Quantity * UnitPrice;

    private OrderItem() { }

    internal static OrderItem Create(Guid productId, string productSku, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Order item quantity must be positive.");
        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        return new OrderItem
        {
            ProductId = productId,
            ProductSku = productSku,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}