using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryFlow.Application.Features.Products.Queries.GetAllProducts
{
    public record ProductDto(Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string Category,
    bool IsActive);
    
}
