using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Pizzas;

public class GetPizzaByIdEndpoint(IMediator mediator) : Endpoint<GetPizzaByIdRequest, Response<Pizza>>
{
    public override void Configure()
    {
        Get("/pizzas/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Pizzas"));
    }

    public override async Task HandleAsync(GetPizzaByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPizzaByIdQuery { Id = req.Id }, ct);
        if (result.Success && result.Data is not null)
        {
            await SendOkAsync(result, ct);
        }
        else if (result.Success == false)
        {
            await SendAsync(
                result,
                (int)HttpStatusCode.NotFound,
                ct);
        }
        else
        {
            await SendNotFoundAsync(ct);
        }
    }
}

public class GetPizzaByIdRequest
{
    public long Id { get; set; }
}
