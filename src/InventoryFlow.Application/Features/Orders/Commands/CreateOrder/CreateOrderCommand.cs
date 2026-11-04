using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryFlow.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(Guid WarehouseId, List<OrderLineRequest> Items) : IRequest<CreateOrderResult>;
    public record OrderLineRequest(Guid ProductId, int Quantity);
    public record CreateOrderResult(Guid OrderId, string OrderNumber, decimal TotalAmount, string Status);
}
