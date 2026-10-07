using InventoryFlow.Application.Features.Products.Commands.CreateProduct;
using InventoryFlow.Application.Features.Products.Queries.GetAllProducts;
using InventoryFlow.Domain.Constants;
using InventoryFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace InventoryFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // requires ANY authenticated user (any valid JWT) — applies to every action below by default
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)] // overrides the class-level [Authorize] — only Admin can hit this specific action
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var productId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = productId }, productId);
    }
}