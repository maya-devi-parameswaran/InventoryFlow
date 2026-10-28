using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using MediatR;

namespace InventoryFlow.Application.Features.Warehouses.Commands.CreateWarehouse
{
    public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateWarehouseCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = Warehouse.Create(request.Name, request.Location);
            await _unitOfWork.Warehouses.AddAsync(warehouse, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return warehouse.Id;
        }
    }
}
