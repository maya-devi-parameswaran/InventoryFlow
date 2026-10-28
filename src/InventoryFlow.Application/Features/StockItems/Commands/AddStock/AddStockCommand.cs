using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryFlow.Application.Features.StockItems.Commands.AddStock
{
    public record AddStockCommand(
     Guid ProductId,
     Guid WarehouseId,
     int InitialQuantity,
     int ReorderThreshold
 ) : IRequest<Guid>;
}
