using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL mutations for categories. All require X-User-Id request header.</summary>
[ExtendObjectType(typeof(OrderMutation))]
[GraphQLDescription("GraphQL mutations for categories. All require X-User-Id request header.")]
public class CategoryMutation
{
    /// <summary>Creates a new category. Sets CreatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Creates a new category. Sets CreatedBy from X-User-Id header.")]
    public async Task<Response<Category>> CreateCategory([Service] IMediator mediator, CreateCategoryInput input)
    {
        var command = new CreateCategoryCommand { Name = input.Name };
        return await mediator.Send(command);
    }

    /// <summary>Updates a category by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Updates a category by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response<Category>> UpdateCategory([Service] IMediator mediator, long id, UpdateCategoryInput input)
    {
        var command = new UpdateCategoryCommand { Id = id, Name = input.Name };
        return await mediator.Send(command);
    }

    /// <summary>Soft-deletes a category by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Soft-deletes a category by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response> DeleteCategory([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new DeleteCategoryCommand { Id = id });
    }
}

/// <summary>Input for creating a category.</summary>
[GraphQLDescription("Input for creating a category.")]
public class CreateCategoryInput
{
    [GraphQLDescription("Category name.")]
    public string Name { get; set; } = null!;
}

/// <summary>Input for updating a category.</summary>
[GraphQLDescription("Input for updating a category.")]
public class UpdateCategoryInput
{
    [GraphQLDescription("Category name.")]
    public string? Name { get; set; }
}
