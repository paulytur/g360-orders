using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL mutations for pizza types. All require X-User-Id request header.</summary>
[ExtendObjectType(typeof(OrderMutation))]
[GraphQLDescription("GraphQL mutations for pizza types. All require X-User-Id request header.")]
public class PizzaTypeMutation
{
    /// <summary>Creates a new pizza type with optional category and ingredients. Sets CreatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Creates a new pizza type with optional category and ingredients. Sets CreatedBy from X-User-Id header.")]
    public async Task<Response<PizzaType>> CreatePizzaType([Service] IMediator mediator, CreatePizzaTypeInput input)
    {
        var command = new CreatePizzaTypeCommand
        {
            Code = input.Code,
            Name = input.Name,
            CategoryId = input.CategoryId,
            IngredientIds = input.IngredientIds ?? []
        };
        return await mediator.Send(command);
    }

    /// <summary>Updates a pizza type by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Updates a pizza type by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response<PizzaType>> UpdatePizzaType([Service] IMediator mediator, long id, UpdatePizzaTypeInput input)
    {
        var command = new UpdatePizzaTypeCommand
        {
            Id = id,
            Code = input.Code,
            Name = input.Name,
            CategoryId = input.CategoryId,
            IngredientIds = input.IngredientIds
        };
        return await mediator.Send(command);
    }

    /// <summary>Soft-deletes a pizza type by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Soft-deletes a pizza type by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response> DeletePizzaType([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new DeletePizzaTypeCommand { Id = id });
    }
}

/// <summary>Input for creating a pizza type.</summary>
[GraphQLDescription("Input for creating a pizza type.")]
public class CreatePizzaTypeInput
{
    [GraphQLDescription("Pizza type code.")]
    public string Code { get; set; } = null!;
    [GraphQLDescription("Pizza type name.")]
    public string Name { get; set; } = null!;
    [GraphQLDescription("Category ID.")]
    public long? CategoryId { get; set; }
    /// <summary>Ingredient IDs to link to this pizza type.</summary>
    [GraphQLDescription("Ingredient IDs to link to this pizza type.")]
    public List<long>? IngredientIds { get; set; }
}

/// <summary>Input for updating a pizza type.</summary>
[GraphQLDescription("Input for updating a pizza type.")]
public class UpdatePizzaTypeInput
{
    [GraphQLDescription("Pizza type code.")]
    public string? Code { get; set; }
    [GraphQLDescription("Pizza type name.")]
    public string? Name { get; set; }
    [GraphQLDescription("Category ID.")]
    public long? CategoryId { get; set; }
    [GraphQLDescription("Ingredient IDs to link to this pizza type.")]
    public List<long>? IngredientIds { get; set; }
}
