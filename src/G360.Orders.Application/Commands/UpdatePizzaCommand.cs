using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to update an existing pizza.
/// </summary>
public class UpdatePizzaCommand : IRequest<Response<Pizza>>
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public long? PizzaTypeId { get; set; }
    public string? Size { get; set; }
    public decimal? Price { get; set; }
}
