using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Orders;

public class DeleteOrderEndpoint(IMediator mediator) : Endpoint<DeleteOrderRequest, Response>
{
    public override void Configure()
    {
        Delete("/orders/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Orders"));
    }

    public override async Task HandleAsync(DeleteOrderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteOrderCommand { Id = req.Id }, ct);
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

public class DeleteOrderRequest
{
    public long Id { get; set; }
}
