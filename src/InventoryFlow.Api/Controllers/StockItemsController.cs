using InventoryFlow.Application.Features.StockItems.Commands.AddStock;
using InventoryFlow.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StockItemsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<Guid>> Create(AddStockCommand command, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }
    }
}
