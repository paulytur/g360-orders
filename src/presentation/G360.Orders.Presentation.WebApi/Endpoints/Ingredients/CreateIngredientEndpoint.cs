using System.Net;
using MediatR;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;


namespace G360.Orders.Presentation.WebApi.Endpoints.Ingredients;

public class CreateIngredientEndpoint(IMediator mediator) : Endpoint<CreateIngredientRequest, Response<Ingredient>>
{
    public override void Configure()
    {
        Post("/ingredients");
        AllowAnonymous();
        Description(d => d.WithTags("Ingredient"));
    }

    public override async Task HandleAsync(CreateIngredientRequest req, CancellationToken ct)
    {
        var command = new CreateIngredientCommand { Description = req.Description };

        var result = await mediator.Send(command, ct);

        if (result.Success && result.Data is not null)
        {
            await SendCreatedAtAsync<GetIngredientByIdEndpoint>(
                new
                {
                    id = result.Data.Id
                },
                result,
                cancellation: ct
            );
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

public class CreateIngredientRequest
{
    public required string Description { get; set; }
}

