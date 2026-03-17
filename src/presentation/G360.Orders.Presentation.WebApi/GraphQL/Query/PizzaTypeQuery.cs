using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL queries for pizza types.</summary>
[ExtendObjectType(typeof(OrderQuery))]
[GraphQLDescription("GraphQL queries for pizza types.")]
public class PizzaTypeQuery
{
    /// <summary>Returns a paged list of pizza types. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a paged list of pizza types. Requires X-User-Id request header.")]
    public async Task<PagedResponse<PizzaType>> GetPizzaTypes(
        [Service] IMediator mediator,
        string? code = null,
        long? categoryId = null,
        int? pageNumber = 1,
        int? pageSize = 10,
        bool? includeDeleted = false)
    {
        var query = new GetPizzaTypesQuery
        {
            Code = code,
            CategoryId = categoryId,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
            IncludeDeleted = includeDeleted ?? false
        };
        return await mediator.Send(query);
    }

    /// <summary>Returns a single pizza type by id. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a single pizza type by id. Requires X-User-Id request header.")]
    public async Task<Response<PizzaType>> GetPizzaTypeById([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new GetPizzaTypeByIdQuery { Id = id });
    }
}
