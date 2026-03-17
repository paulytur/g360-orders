using System.Net;
using AutoMapper;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Orders;

public class CreateOrderEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<CreateOrderRequest, Response<OrderResponse>>
{
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
        Description(d => d.WithTags("Orders"));
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        var command = new CreateOrderCommand
        {
            Items = req.Items?.Select(i => new OrderDetailItem
            {
                PizzaId = i.PizzaId,
                Quantity = i.Quantity
            }).ToList() ?? []
        };
        var result = await mediator.Send(command, ct);
        if (result.Success && result.Data is not null)
        {
            var dto = mapper.Map<OrderResponse>(result.Data);
            await SendCreatedAtAsync<GetOrderByIdEndpoint>(
                new { id = result.Data.Id },
                new Response<OrderResponse>(true, result.Messages, dto),
                cancellation: ct);
            return;
        }
        await SendAsync(
            new Response<OrderResponse>(false, result.Messages),
            (int)HttpStatusCode.BadRequest,
            ct);
    }
}

public class CreateOrderRequest
{
    public List<OrderDetailItemRequest>? Items { get; set; }
}

public class OrderDetailItemRequest
{
    public long PizzaId { get; set; }
    public int Quantity { get; set; }
}
