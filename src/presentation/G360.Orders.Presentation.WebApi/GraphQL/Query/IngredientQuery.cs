using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL queries for ingredients.</summary>
[ExtendObjectType(typeof(OrderQuery))]
[GraphQLDescription("GraphQL queries for ingredients.")]
public class IngredientQuery
{
    /// <summary>Returns a paged list of ingredients. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a paged list of ingredients. Requires X-User-Id request header.")]
    public async Task<PagedResponse<Ingredient>> GetIngredients(
        [Service] IMediator mediator,
        string? description = null,
        int? pageNumber = 1,
        int? pageSize = 10,
        bool? includeDeleted = false)
    {
        var query = new GetIngredientsQuery
        {
            Description = description,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
            IncludeDeleted = includeDeleted ?? false
        };
        return await mediator.Send(query);
    }

    /// <summary>Returns a single ingredient by id. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a single ingredient by id. Requires X-User-Id request header.")]
    public async Task<Response<Ingredient>> GetIngredientById([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new GetIngredientByIdQuery { Id = id });
    }
}
