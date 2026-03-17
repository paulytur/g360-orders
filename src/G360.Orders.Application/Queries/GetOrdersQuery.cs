using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

/// <summary>
/// Query to retrieve orders with optional pagination.
/// </summary>
public class GetOrdersQuery : IRequest<PagedResponse<Order>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; }
}
