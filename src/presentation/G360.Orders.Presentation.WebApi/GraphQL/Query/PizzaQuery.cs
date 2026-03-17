using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL queries for pizzas.</summary>
[ExtendObjectType(typeof(OrderQuery))]
[GraphQLDescription("GraphQL queries for pizzas.")]
public class PizzaQuery
{
    /// <summary>Returns a paged list of pizzas. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a paged list of pizzas. Requires X-User-Id request header.")]
    public async Task<PagedResponse<Pizza>> GetPizzas(
        [Service] IMediator mediator,
        string? code = null,
        long? pizzaTypeId = null,
        string? size = null,
        int? pageNumber = 1,
        int? pageSize = 10,
        bool? includeDeleted = false)
    {
        var query = new GetPizzasQuery
        {
            Code = code,
            PizzaTypeId = pizzaTypeId,
            Size = size,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
            IncludeDeleted = includeDeleted ?? false
        };
        return await mediator.Send(query);
    }

    /// <summary>Returns a single pizza by id. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a single pizza by id. Requires X-User-Id request header.")]
    public async Task<Response<Pizza>> GetPizzaById([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new GetPizzaByIdQuery { Id = id });
    }
}
