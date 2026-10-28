using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using InventoryFlow.Domain.Exceptions;
using MediatR;

namespace InventoryFlow.Application.Features.StockItems.Commands.AddStock
{
    public class AddStockCommandHandler : IRequestHandler<AddStockCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddStockCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken)
             ?? throw new DomainException($"Product {request.ProductId} not found.");

            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(request.WarehouseId, cancellationToken)
                ?? throw new DomainException($"Warehouse {request.WarehouseId} not found.");

            var existing = await _unitOfWork.StockItems.GetByProductAndWarehouseAsync(
                request.ProductId, request.WarehouseId, cancellationToken);

            if (existing is not null)
                throw new DomainException("Stock already exists for this product at this warehouse. Use an adjust-stock operation instead.");

            var stockItem = StockItem.Create(request.ProductId, request.WarehouseId, request.InitialQuantity, request.ReorderThreshold);

            await _unitOfWork.StockItems.AddAsync(stockItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return stockItem.Id;
        }
    }
}
