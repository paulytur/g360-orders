using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to update an existing ingredient.
/// </summary>
public class UpdateIngredientCommand : IRequest<Response<Ingredient>>
{
    public long Id { get; set; }
    public string? Description { get; set; }
}
