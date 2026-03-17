using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL queries for orders.</summary>
[GraphQLDescription("GraphQL queries for orders.")]
public class OrderQuery
{
    /// <summary>Returns a paged list of orders. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a paged list of orders. Requires X-User-Id request header.")]
    public async Task<PagedResponse<Order>> GetOrders(
        [Service] IMediator mediator,
        int? pageNumber = 1,
        int? pageSize = 10,
        bool? includeDeleted = false)
    {
        var query = new GetOrdersQuery
        {
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
            IncludeDeleted = includeDeleted ?? false
        };
        return await mediator.Send(query);
    }

    /// <summary>Returns a single order by id. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a single order by id. Requires X-User-Id request header.")]
    public async Task<Response<Order>> GetOrderById([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new GetOrderByIdQuery { Id = id });
    }
}
