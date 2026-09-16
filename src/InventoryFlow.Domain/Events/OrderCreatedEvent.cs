namespace InventoryFlow.Domain.Entities;

public record OrderCreatedEvent(Guid OrderId);
public record OrderReservedEvent(Guid OrderId);
public record OrderShippedEvent(Guid OrderId);
public record OrderCancelledEvent(Guid OrderId, Guid WarehouseId, IReadOnlyList<OrderItem> Items);