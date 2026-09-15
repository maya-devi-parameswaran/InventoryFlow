// Entities/StockItem.cs
using InventoryFlow.Domain.Common;
using InventoryFlow.Domain.Exceptions;

namespace InventoryFlow.Domain.Entities;

public class StockItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }
    public int ReorderThreshold { get; private set; }

    public int QuantityAvailable => QuantityOnHand - QuantityReserved;

    // Concurrency token — critical for preventing overselling under load
    public uint Version { get; private set; }

    private StockItem() { }

    public static StockItem Create(Guid productId, Guid warehouseId, int initialQuantity, int reorderThreshold)
    {
        if (initialQuantity < 0)
            throw new DomainException("Initial quantity cannot be negative.");

        return new StockItem
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            QuantityOnHand = initialQuantity,
            ReorderThreshold = reorderThreshold
        };
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Reserve quantity must be positive.");
        if (quantity > QuantityAvailable)
            throw new InsufficientStockException(ProductId, quantity, QuantityAvailable);

        QuantityReserved += quantity;
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Release quantity must be positive.");
        if (quantity > QuantityReserved)
            throw new DomainException("Cannot release more than reserved.");

        QuantityReserved -= quantity;
    }

    public void FulfillReservation(int quantity)
    {
        if (quantity > QuantityReserved)
            throw new DomainException("Cannot fulfill more than reserved.");

        QuantityReserved -= quantity;
        QuantityOnHand -= quantity;
    }

    public bool IsBelowReorderThreshold() => QuantityOnHand <= ReorderThreshold;
}