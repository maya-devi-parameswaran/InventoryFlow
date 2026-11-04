using InventoryFlow.Application.Common.Exceptions;
using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using InventoryFlow.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryFlow.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        private const int MaxAttempts = 3;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) 
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Could not determine the current user.");

            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    var order = Order.Create(customerId, request.WarehouseId);

                    foreach (var line in request.Items)
                    {
                        var product = await _unitOfWork.Products.GetByIdAsync(line.ProductId, cancellationToken)
                            ?? throw new DomainException($"Product {line.ProductId} not found.");

                        if (!product.IsActive)
                            throw new DomainException($"Product {product.Sku} is not available.");

                        var stock = await _unitOfWork.StockItems.GetByProductAndWarehouseAsync(
                            line.ProductId, request.WarehouseId, cancellationToken)
                            ?? throw new InsufficientStockException(line.ProductId, line.Quantity, 0);

                        stock.Reserve(line.Quantity);

                        // Price is snapshotted now: later price changes must not rewrite past orders
                        order.AddItem(product.Id, product.Sku, line.Quantity, product.Price);
                    }

                    order.MarkAsReserved();

                    await _unitOfWork.Orders.AddAsync(order, cancellationToken);

                    // One SaveChanges = one transaction: order + stock reservations land together or not at all
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return new CreateOrderResult(order.Id, order.OrderNumber, order.TotalAmount, order.Status.ToString());
                }
                catch (ConcurrencyException) when (attempt < MaxAttempts)
                {
                    // Someone else changed the same stock row. Drop stale state and rebuild from fresh data.
                    _unitOfWork.ClearChangeTracker();
                }
            }
        }
    }
}
