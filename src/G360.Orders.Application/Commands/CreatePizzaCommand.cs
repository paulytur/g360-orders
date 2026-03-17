using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to create a new pizza.
/// </summary>
public class CreatePizzaCommand : IRequest<Response<Pizza>>
{
    public required string Code { get; set; }
    public long? PizzaTypeId { get; set; }
    public string? Size { get; set; }
    public decimal Price { get; set; }
}
