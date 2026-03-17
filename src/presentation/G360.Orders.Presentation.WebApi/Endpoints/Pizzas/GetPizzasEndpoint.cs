using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Pizzas;

public class GetPizzasEndpoint(IMediator mediator) : Endpoint<GetPizzasRequest, PagedResponse<Pizza>>
{
    public override void Configure()
    {
        Get("/pizzas");
        AllowAnonymous();
        Description(d => d.WithTags("Pizzas"));
    }

    public override async Task HandleAsync(GetPizzasRequest req, CancellationToken ct)
    {
        var query = new GetPizzasQuery
        {
            Code = req.Code,
            PizzaTypeId = req.PizzaTypeId,
            Size = req.Size,
            PageNumber = req.PageNumber ?? 1,
            PageSize = req.PageSize ?? 10,
            IncludeDeleted = req.IncludeDeleted ?? false
        };
        var result = await mediator.Send(query, ct);
        if (result.Success)
        {
            await SendOkAsync(result, ct);
        }
        else
        {
            await SendAsync(
                result,
                (int)HttpStatusCode.BadRequest,
                ct);
        }
    }
}

public class GetPizzasRequest
{
    public string? Code { get; set; }
    public long? PizzaTypeId { get; set; }
    public string? Size { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeDeleted { get; set; }
}
