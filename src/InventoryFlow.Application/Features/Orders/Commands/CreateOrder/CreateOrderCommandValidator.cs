using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryFlow.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();

            RuleFor(x => x.Items)
            .NotEmpty().WithMessage("An order must contain at least one item.");

            RuleFor(x => x.Items)
           .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count)
           .WithMessage("Each product may appear only once per order.")
           .When(x => x.Items is not null);

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
