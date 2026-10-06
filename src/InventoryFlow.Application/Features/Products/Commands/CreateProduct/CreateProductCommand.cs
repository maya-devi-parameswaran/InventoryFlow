using MediatR;

namespace InventoryFlow.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
     string Sku,
     string Name,
     decimal Price,
     string Category,
     string? Description
 ) : IRequest<Guid>; // returns the new Product's Id
}
