using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Categories;

public class GetCategoriesEndpoint(IMediator mediator) : Endpoint<GetCategoriesRequest, PagedResponse<Category>>
{
    public override void Configure()
    {
        Get("/categories");
        AllowAnonymous();
        Description(d => d.WithTags("Category"));
    }

    public override async Task HandleAsync(GetCategoriesRequest req, CancellationToken ct)
    {
        var query = new GetCategoriesQuery
        {
            Name = req.Name,
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

public class GetCategoriesRequest
{
    public string? Name { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeDeleted { get; set; }
}
