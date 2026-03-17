using FluentValidation;
using G360.Orders.Application.Commands;

namespace G360.Orders.Application.Validators;

public class CreatePizzaTypeCommandValidator : AbstractValidator<CreatePizzaTypeCommand>
{
    public CreatePizzaTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
