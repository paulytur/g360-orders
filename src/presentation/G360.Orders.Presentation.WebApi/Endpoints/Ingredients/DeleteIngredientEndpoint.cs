using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Ingredients;

public class DeleteIngredientEndpoint(IMediator mediator) : Endpoint<DeleteIngredientRequest, Response>
{
    public override void Configure()
    {
        Delete("/ingredients/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Ingredient"));
    }

    public override async Task HandleAsync(DeleteIngredientRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteIngredientCommand { Id = req.Id }, ct);
        
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

public class DeleteIngredientRequest
{
    public long Id { get; set; }
}
