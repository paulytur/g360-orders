using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

public class UpdatePizzaTypeCommand : IRequest<Response<PizzaType>>
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public long? CategoryId { get; set; }
    /// <summary>When set, replaces pizza details for this type with these ingredient IDs.</summary>
    public List<long>? IngredientIds { get; set; }
}
