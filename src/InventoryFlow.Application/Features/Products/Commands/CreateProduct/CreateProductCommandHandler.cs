using InventoryFlow.Application.Common.Interfaces;
using InventoryFlow.Domain.Entities;
using MediatR;

namespace InventoryFlow.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = Product.Create(
                request.Sku,
                request.Name,
                request.Price,
                request.Category,
                request.Description);

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }

}
