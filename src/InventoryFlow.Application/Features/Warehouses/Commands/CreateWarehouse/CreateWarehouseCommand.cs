using MediatR;

namespace InventoryFlow.Application.Features.Warehouses.Commands.CreateWarehouse
{
    public record CreateWarehouseCommand(string Name, string Location) : IRequest<Guid>;
}
