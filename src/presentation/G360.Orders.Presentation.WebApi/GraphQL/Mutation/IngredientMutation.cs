using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL mutations for ingredients. All require X-User-Id request header.</summary>
[ExtendObjectType(typeof(OrderMutation))]
[GraphQLDescription("GraphQL mutations for ingredients. All require X-User-Id request header.")]
public class IngredientMutation
{
    /// <summary>Creates a new ingredient. Sets CreatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Creates a new ingredient. Sets CreatedBy from X-User-Id header.")]
    public async Task<Response<Ingredient>> CreateIngredient([Service] IMediator mediator, CreateIngredientInput input)
    {
        var command = new CreateIngredientCommand { Description = input.Description };
        return await mediator.Send(command);
    }

    /// <summary>Updates an ingredient by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Updates an ingredient by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response<Ingredient>> UpdateIngredient([Service] IMediator mediator, long id, UpdateIngredientInput input)
    {
        var command = new UpdateIngredientCommand { Id = id, Description = input.Description };
        return await mediator.Send(command);
    }

    /// <summary>Soft-deletes an ingredient by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Soft-deletes an ingredient by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response> DeleteIngredient([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new DeleteIngredientCommand { Id = id });
    }
}

/// <summary>Input for creating an ingredient.</summary>
[GraphQLDescription("Input for creating an ingredient.")]
public class CreateIngredientInput
{
    [GraphQLDescription("Ingredient description.")]
    public string Description { get; set; } = null!;
}

/// <summary>Input for updating an ingredient.</summary>
[GraphQLDescription("Input for updating an ingredient.")]
public class UpdateIngredientInput
{
    [GraphQLDescription("Ingredient description.")]
    public string? Description { get; set; }
}
