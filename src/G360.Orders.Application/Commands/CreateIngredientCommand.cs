using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to create a new ingredient.
/// </summary>
public class CreateIngredientCommand : IRequest<Response<Ingredient>>
{
    public required string Description { get; set; }
}
