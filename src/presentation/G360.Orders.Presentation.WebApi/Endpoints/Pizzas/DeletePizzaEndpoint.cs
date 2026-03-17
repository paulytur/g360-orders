using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Pizzas;

public class DeletePizzaEndpoint(IMediator mediator) : Endpoint<DeletePizzaRequest, Response>
{
    public override void Configure()
    {
        Delete("/pizzas/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Pizzas"));
    }

    public override async Task HandleAsync(DeletePizzaRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeletePizzaCommand { Id = req.Id }, ct);
        if (result.Success)
        {
            await SendNoContentAsync(ct);
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

public class DeletePizzaRequest
{
    public long Id { get; set; }
}
