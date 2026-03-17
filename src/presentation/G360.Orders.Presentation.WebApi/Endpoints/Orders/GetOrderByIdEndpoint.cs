using System.Net;
using AutoMapper;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Application.Queries;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Orders;

public class GetOrderByIdEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<GetOrderByIdRequest, Response<OrderResponse>>
{
    public override void Configure()
    {
        Get("/orders/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Orders"));
    }

    public override async Task HandleAsync(GetOrderByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrderByIdQuery { Id = req.Id }, ct);
        if (result.Success && result.Data is not null)
        {
            var dto = mapper.Map<OrderResponse>(result.Data);
            await SendOkAsync(
                new Response<OrderResponse>(true, result.Messages, dto),
                ct);
            return;
        }
        if (!result.Success)
        {
            await SendAsync(
                new Response<OrderResponse>(false, result.Messages),
                (int)HttpStatusCode.NotFound,
                ct);
        }
        else
        {
            await SendNotFoundAsync(ct);
        }
    }
}

public class GetOrderByIdRequest
{
    public long Id { get; set; }
}
