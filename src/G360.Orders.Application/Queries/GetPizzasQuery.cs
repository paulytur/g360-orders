using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

/// <summary>
/// Query to retrieve pizzas with optional filtering and pagination.
/// </summary>
public class GetPizzasQuery : IRequest<PagedResponse<Pizza>>
{
    public string? Code { get; set; }
    public long? PizzaTypeId { get; set; }
    public string? Size { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; }
}
