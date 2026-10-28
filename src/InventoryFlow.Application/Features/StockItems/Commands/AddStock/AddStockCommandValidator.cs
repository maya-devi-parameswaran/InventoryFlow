using FluentValidation;

namespace InventoryFlow.Application.Features.StockItems.Commands.AddStock
{
    public class AddStockCommandValidator : AbstractValidator<AddStockCommand>
    {
        public AddStockCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.InitialQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReorderThreshold).GreaterThanOrEqualTo(0);
        }
    }
}
