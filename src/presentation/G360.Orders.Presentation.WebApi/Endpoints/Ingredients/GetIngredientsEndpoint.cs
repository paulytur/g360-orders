using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Ingredients;

public class GetIngredientsEndpoint(IMediator mediator) : Endpoint<GetIngredientsRequest, PagedResponse<Ingredient>>
{
    public override void Configure()
    {
        Get("/ingredients");
        AllowAnonymous();
        Description(d => d.WithTags("Ingredient"));
    }

    public override async Task HandleAsync(GetIngredientsRequest req, CancellationToken ct)
    {
        var query = new GetIngredientsQuery
        {
            Description = req.Description,
            PageNumber = req.PageNumber ?? 1,
            PageSize = req.PageSize ?? 10,
            IncludeDeleted = req.IncludeDeleted ?? false
        };
        var result = await mediator.Send(query, ct);
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

public class GetIngredientsRequest
{
    public string? Description { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeDeleted { get; set; }
}
