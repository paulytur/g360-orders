using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL queries for categories.</summary>
[ExtendObjectType(typeof(OrderQuery))]
[GraphQLDescription("GraphQL queries for categories.")]
public class CategoryQuery
{
    /// <summary>Returns a paged list of categories. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a paged list of categories. Requires X-User-Id request header.")]
    public async Task<PagedResponse<Category>> GetCategories(
        [Service] IMediator mediator,
        string? name = null,
        int? pageNumber = 1,
        int? pageSize = 10,
        bool? includeDeleted = false)
    {
        var query = new GetCategoriesQuery
        {
            Name = name,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
            IncludeDeleted = includeDeleted ?? false
        };
        return await mediator.Send(query);
    }

    /// <summary>Returns a single category by id. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns a single category by id. Requires X-User-Id request header.")]
    public async Task<Response<Category>> GetCategoryById([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new GetCategoryByIdQuery { Id = id });
    }
}
