using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Pizzas;

public class UpdatePizzaEndpoint(IMediator mediator) : Endpoint<UpdatePizzaRequest, Response<Pizza>>
{
    public override void Configure()
    {
        Put("/pizzas/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Pizzas"));
    }

    public override async Task HandleAsync(UpdatePizzaRequest req, CancellationToken ct)
    {
        var command = new UpdatePizzaCommand
        {
            Id = req.Id,
            Code = req.Code,
            PizzaTypeId = req.PizzaTypeId,
            Size = req.Size,
            Price = req.Price
        };
        var result = await mediator.Send(command, ct);
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

public class UpdatePizzaRequest
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public long? PizzaTypeId { get; set; }
    public string? Size { get; set; }
    public decimal? Price { get; set; }
}
