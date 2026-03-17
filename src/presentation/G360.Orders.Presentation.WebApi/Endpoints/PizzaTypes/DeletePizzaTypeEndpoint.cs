using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.PizzaTypes;

public class DeletePizzaTypeEndpoint(IMediator mediator) : Endpoint<DeletePizzaTypeRequest, Response>
{
    public override void Configure()
    {
        Delete("/pizza-types/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Pizza Types"));
    }

    public override async Task HandleAsync(DeletePizzaTypeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeletePizzaTypeCommand { Id = req.Id }, ct);
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

public class DeletePizzaTypeRequest
{
    public long Id { get; set; }
}
