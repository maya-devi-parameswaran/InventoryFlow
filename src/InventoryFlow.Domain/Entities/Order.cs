// Entities/Order.cs
using InventoryFlow.Domain.Common;
using InventoryFlow.Domain.Enums;
using InventoryFlow.Domain.Exceptions;

namespace InventoryFlow.Domain.Entities;

public class Order : BaseEntity, IAggregateRoot
{
    public string OrderNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ShippedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(i => i.LineTotal);

    private Order() { }

    public static Order Create(Guid customerId, Guid warehouseId)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            WarehouseId = warehouseId,
            Status = OrderStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return order;
    }

    public void AddItem(Guid productId, string productSku, int quantity, decimal unitPrice)
    {
        EnsureStatus(OrderStatus.Pending, "add items to");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            throw new DomainException($"Product {productSku} already added. Remove and re-add to change quantity.");

        _items.Add(OrderItem.Create(productId, productSku, quantity, unitPrice));
    }

    public void MarkAsReserved()
    {
        EnsureStatus(OrderStatus.Pending, "reserve stock for");
        if (!_items.Any())
            throw new DomainException("Cannot reserve stock for an order with no items.");

        Status = OrderStatus.Reserved;
        AddDomainEvent(new OrderReservedEvent(Id));
    }

    public void MarkAsShipped()
    {
        EnsureStatus(OrderStatus.Reserved, "ship");

        Status = OrderStatus.Shipped;
        ShippedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new OrderShippedEvent(Id));
    }

    public void MarkAsDelivered()
    {
        EnsureStatus(OrderStatus.Shipped, "mark as delivered");
        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new DomainException($"Cannot cancel an order that is already {Status}.");

        var previousStatus = Status;
        Status = OrderStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;

        // Only orders that had reserved stock need it released
        if (previousStatus == OrderStatus.Reserved)
            AddDomainEvent(new OrderCancelledEvent(Id, WarehouseId, _items.ToList()));
    }

    private void EnsureStatus(OrderStatus required, string action)
    {
        if (Status != required)
            throw new DomainException($"Cannot {action} an order in {Status} status. Order must be {required}.");
    }

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
}