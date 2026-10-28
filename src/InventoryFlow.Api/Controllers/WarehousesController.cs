using InventoryFlow.Application.Features.Warehouses.Commands.CreateWarehouse;
using InventoryFlow.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WarehousesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<Guid>> Create(CreateWarehouseCommand command, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }
    }
}
