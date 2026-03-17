using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Ingredients;

public class UpdateIngredientEndpoint(IMediator mediator) : Endpoint<UpdateIngredientRequest, Response<Ingredient>>
{
    public override void Configure()
    {
        Put("/ingredients/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Ingredient"));
    }

    public override async Task HandleAsync(UpdateIngredientRequest req, CancellationToken ct)
    {
        var command = new UpdateIngredientCommand
        {
            Id = req.Id,
            Description = req.Description
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

public class UpdateIngredientRequest
{
    public long Id { get; set; }
    public string? Description { get; set; }
}
