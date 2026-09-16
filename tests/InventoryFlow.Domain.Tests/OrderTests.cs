using FluentAssertions;
using InventoryFlow.Domain.Entities;
using InventoryFlow.Domain.Enums;
using InventoryFlow.Domain.Exceptions;
using Xunit;

namespace InventoryFlow.Domain.Tests;

public class OrderTests
{
    [Fact]
    public void Create_ShouldStartInPendingStatus()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_WhenPending_ShouldSucceed()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        order.AddItem(Guid.NewGuid(), "SKU-001", quantity: 2, unitPrice: 25.00m);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(50.00m);
    }

    [Fact]
    public void AddItem_WhenNotPending_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-001", 1, 10m);
        order.MarkAsReserved();

        var act = () => order.AddItem(Guid.NewGuid(), "SKU-002", 1, 10m);

        act.Should().Throw<DomainException>()
           .WithMessage("*Pending*");
    }

    [Fact]
    public void MarkAsReserved_WithNoItems_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        var act = () => order.MarkAsReserved();

        act.Should().Throw<DomainException>()
           .WithMessage("*no items*");
    }

    [Fact]
    public void MarkAsShipped_WithoutBeingReservedFirst_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-001", 1, 10m);

        var act = () => order.MarkAsShipped();

        act.Should().Throw<DomainException>()
           .WithMessage("*must be Reserved*");
    }

    [Fact]
    public void FullHappyPath_PendingToDelivered_ShouldTransitionCorrectly()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-001", 3, 15m);

        order.MarkAsReserved();
        order.Status.Should().Be(OrderStatus.Reserved);

        order.MarkAsShipped();
        order.Status.Should().Be(OrderStatus.Shipped);
        order.ShippedAtUtc.Should().NotBeNull();

        order.MarkAsDelivered();
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void Cancel_AfterDelivered_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-001", 1, 10m);
        order.MarkAsReserved();
        order.MarkAsShipped();
        order.MarkAsDelivered();

        var act = () => order.Cancel();

        act.Should().Throw<DomainException>()
           .WithMessage("*already Delivered*");
    }

    [Fact]
    public void Cancel_WhenReserved_ShouldRaiseCancelledEventWithItems()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SKU-001", 2, 10m);
        order.MarkAsReserved();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().ContainSingle(e => e is OrderCancelledEvent);
    }
}