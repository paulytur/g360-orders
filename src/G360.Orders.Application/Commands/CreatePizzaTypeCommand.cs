using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

public class CreatePizzaTypeCommand : IRequest<Response<PizzaType>>
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public long? CategoryId { get; set; }
    /// <summary>Ingredient IDs to link to this pizza type (stored in pizza_details table).</summary>
    public List<long> IngredientIds { get; set; } = [];
}
