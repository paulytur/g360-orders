using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

/// <summary>
/// Query to retrieve an order by its identifier.
/// </summary>
public class GetOrderByIdQuery : IRequest<Response<Order>>
{
    public long Id { get; set; }
}
