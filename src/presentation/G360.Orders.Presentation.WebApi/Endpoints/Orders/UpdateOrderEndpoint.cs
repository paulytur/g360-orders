using System.Net;
using AutoMapper;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Orders;

public class UpdateOrderEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<UpdateOrderRequest, Response<OrderResponse>>
{
    public override void Configure()
    {
        Put("/orders/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Orders"));
    }

    public override async Task HandleAsync(UpdateOrderRequest req, CancellationToken ct)
    {
        var command = new UpdateOrderCommand
        {
            Id = req.Id,
            Items = req.Items?.Select(i => new OrderDetailItem
            {
                PizzaId = i.PizzaId,
                Quantity = i.Quantity
            }).ToList()
        };
        var result = await mediator.Send(command, ct);
        if (result.Success && result.Data is not null)
        {
            var dto = mapper.Map<OrderResponse>(result.Data);
            await SendOkAsync(
                new Response<OrderResponse>(true, result.Messages, dto),
                ct);
            return;
        }
        await SendAsync(
            new Response<OrderResponse>(false, result.Messages ?? []),
            (int)HttpStatusCode.BadRequest,
            ct);
    }
}

public class UpdateOrderRequest
{
    public long Id { get; set; }
    public List<OrderDetailItemRequest>? Items { get; set; }
}
