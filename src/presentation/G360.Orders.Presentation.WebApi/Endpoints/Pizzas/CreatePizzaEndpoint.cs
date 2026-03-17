using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Pizzas;

public class CreatePizzaEndpoint(IMediator mediator) : Endpoint<CreatePizzaRequest, Response<Pizza>>
{
    public override void Configure()
    {
        Post("/pizzas");
        AllowAnonymous();
        Description(d => d.WithTags("Pizzas"));
    }

    public override async Task HandleAsync(CreatePizzaRequest req, CancellationToken ct)
    {
        var command = new CreatePizzaCommand
        {
            Code = req.Code,
            PizzaTypeId = req.PizzaTypeId,
            Size = req.Size,
            Price = req.Price
        };
        var result = await mediator.Send(command, ct);
        if (result.Success && result.Data is not null)
        {
            await SendCreatedAtAsync<GetPizzaByIdEndpoint>(
                new { id = result.Data.Id },
                result,
                cancellation: ct);
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

public class CreatePizzaRequest
{
    public string Code { get; set; } = null!;
    public long? PizzaTypeId { get; set; }
    public string? Size { get; set; }
    public decimal Price { get; set; }
}
