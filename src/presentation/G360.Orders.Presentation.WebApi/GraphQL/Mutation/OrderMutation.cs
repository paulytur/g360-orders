using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Domain.Entities;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL mutations for orders. All require X-User-Id request header.</summary>
[GraphQLDescription("GraphQL mutations for orders. All require X-User-Id request header.")]
public class OrderMutation
{
    /// <summary>Creates a new order with the given line items. Sets CreatedBy/UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Creates a new order with the given line items. Sets CreatedBy/UpdatedBy from X-User-Id header.")]
    public async Task<Response<Order>> CreateOrder(
        [Service] IMediator mediator,
        CreateOrderInput input)
    {
        var command = new CreateOrderCommand
        {
            Items = input.Items?.Select(i => new OrderDetailItem
            {
                PizzaId = i.PizzaId,
                Quantity = i.Quantity
            }).ToList() ?? []
        };
        return await mediator.Send(command);
    }

    /// <summary>Updates an existing order's line items. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Updates an existing order's line items. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response<Order>> UpdateOrder(
        [Service] IMediator mediator,
        long id,
        UpdateOrderInput input)
    {
        var command = new UpdateOrderCommand
        {
            Id = id,
            Items = input.Items?.Select(i => new OrderDetailItem
            {
                PizzaId = i.PizzaId,
                Quantity = i.Quantity
            }).ToList()
        };
        return await mediator.Send(command);
    }

    /// <summary>Soft-deletes an order by id. Sets UpdatedBy from X-User-Id header.</summary>
    [GraphQLDescription("Soft-deletes an order by id. Sets UpdatedBy from X-User-Id header.")]
    public async Task<Response> DeleteOrder([Service] IMediator mediator, long id)
    {
        return await mediator.Send(new DeleteOrderCommand { Id = id });
    }
}

/// <summary>Input for creating an order.</summary>
[GraphQLDescription("Input for creating an order.")]
public class CreateOrderInput
{
    /// <summary>Order line items (pizza and quantity).</summary>
    [GraphQLDescription("Order line items (pizza and quantity).")]
    public List<OrderDetailItemInput>? Items { get; set; }
}

/// <summary>Input for updating an order.</summary>
[GraphQLDescription("Input for updating an order.")]
public class UpdateOrderInput
{
    /// <summary>Replacement line items (pizza and quantity).</summary>
    [GraphQLDescription("Replacement line items (pizza and quantity).")]
    public List<OrderDetailItemInput>? Items { get; set; }
}

/// <summary>Order line item (pizza id and quantity).</summary>
[GraphQLDescription("Order line item (pizza id and quantity).")]
public class OrderDetailItemInput
{
    public long PizzaId { get; set; }
    public int Quantity { get; set; }
}
