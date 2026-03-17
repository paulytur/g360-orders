using FluentValidation;
using G360.Orders.Application.Commands;

namespace G360.Orders.Application.Validators;

public class CreateIngredientCommandValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
