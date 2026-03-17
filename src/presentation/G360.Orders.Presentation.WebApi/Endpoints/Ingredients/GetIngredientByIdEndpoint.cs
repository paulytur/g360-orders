using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Ingredients;

public class GetIngredientByIdEndpoint(IMediator mediator) : Endpoint<GetIngredientByIdRequest, Response<Ingredient>>
{
    public override void Configure()
    {
        Get("/ingredients/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Ingredient"));
    }

    public override async Task HandleAsync(GetIngredientByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetIngredientByIdQuery { Id = req.Id }, ct);
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

public class GetIngredientByIdRequest
{
    public long Id { get; set; }
}
