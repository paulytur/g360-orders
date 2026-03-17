using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

/// <summary>
/// Query to retrieve a pizza by its identifier.
/// </summary>
public class GetPizzaByIdQuery : IRequest<Response<Pizza>>
{
    public long Id { get; set; }
}
