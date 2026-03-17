using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Categories;

public class GetCategoryByIdEndpoint(IMediator mediator) : Endpoint<GetCategoryByIdRequest, Response<Category>>
{
    public override void Configure()
    {
        Get("/categories/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Category"));
    }

    public override async Task HandleAsync(GetCategoryByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery { Id = req.Id }, ct);
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

public class GetCategoryByIdRequest
{
    public long Id { get; set; }
}
