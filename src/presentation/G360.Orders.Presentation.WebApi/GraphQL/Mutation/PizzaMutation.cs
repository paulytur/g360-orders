using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL mutations for pizzas. All require X-User-Id request header.</summary>
[ExtendObjectType(typeof(OrderMutation))]
[GraphQLDescription("GraphQL mutations for pizzas. All require X-User-Id request header.")]
public class PizzaMutation
{
    /// <summary>Creates a new pizza. Sets CreatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Creates a new pizza. Sets CreatedBy from X-User-Id header.")]
    public async Task<Response<Pizza>> CreatePizza([Service] IMediator mediator, CreatePizzaInput input)
    {
        var command = new CreatePizzaCommand
        {
            Code = input.Code,
            PizzaTypeId = input.PizzaTypeId,
            Size = input.Size,
            Price = input.Price
        };
        return await mediator.Send(command);
    }

    /// <summary>Updates a pizza by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Updates a pizza by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response<Pizza>> UpdatePizza([Service] IMediator mediator, long id, UpdatePizzaInput input)
    {
        var command = new UpdatePizzaCommand
        {
            Id = id,
            Code = input.Code,
            PizzaTypeId = input.PizzaTypeId,
            Size = input.Size,
            Price = input.Price
        };
        return await mediator.Send(command);
    }

    /// <summary>Soft-deletes a pizza by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Soft-deletes a pizza by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response> DeletePizza([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new DeletePizzaCommand { Id = id });
    }
}

/// <summary>Input for creating a pizza.</summary>
[GraphQLDescription("Input for creating a pizza.")]
public class CreatePizzaInput
{
    [GraphQLDescription("Pizza code.")]
    public string Code { get; set; } = null!;
    [GraphQLDescription("Pizza type id.")]
    public long? PizzaTypeId { get; set; }
    [GraphQLDescription("Pizza size.")]
    public string? Size { get; set; }
    [GraphQLDescription("Pizza price.")]
    public decimal Price { get; set; }
}

/// <summary>Input for updating a pizza.</summary>
[GraphQLDescription("Input for updating a pizza.")]
public class UpdatePizzaInput
{
    [GraphQLDescription("Pizza code.")]
    public string? Code { get; set; }
    [GraphQLDescription("Pizza type.")]
    public long? PizzaTypeId { get; set; }
    [GraphQLDescription("Pizza size.")]
    public string? Size { get; set; }
    [GraphQLDescription("Pizza price.")]
    public decimal? Price { get; set; }
}
