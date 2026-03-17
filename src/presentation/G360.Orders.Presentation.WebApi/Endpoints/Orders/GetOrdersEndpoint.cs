using System.Net;
using AutoMapper;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Application.Queries;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Orders;

public class GetOrdersEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<GetOrdersRequest, PagedResponse<OrderResponse>>
{
    public override void Configure()
    {
        Get("/orders");
        AllowAnonymous();
        Description(d => d.WithTags("Orders"));
    }

    public override async Task HandleAsync(GetOrdersRequest req, CancellationToken ct)
    {
        var query = new GetOrdersQuery
        {
            PageNumber = req.PageNumber ?? 1,
            PageSize = req.PageSize ?? 10,
            IncludeDeleted = req.IncludeDeleted ?? false
        };

        var result = await mediator.Send(query, ct);
        if (!result.Success)
        {
            await SendAsync(
                new PagedResponse<OrderResponse>(false, result.Messages),
                (int)HttpStatusCode.BadRequest,
                ct);
            return;
        }

        var data = mapper.Map<List<OrderResponse>>(result.Data);
        var response = new PagedResponse<OrderResponse>(
            result.Success,
            result.Messages,
            result.Page,
            result.PageSize,
            result.TotalCount,
            data);
        await SendOkAsync(response, ct);
    }
}

public class GetOrdersRequest
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeDeleted { get; set; }
}
