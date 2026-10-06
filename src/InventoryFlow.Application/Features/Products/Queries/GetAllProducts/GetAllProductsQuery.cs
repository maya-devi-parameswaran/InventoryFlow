using MediatR;

namespace InventoryFlow.Application.Features.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery : IRequest<List<ProductDto>>;
}
