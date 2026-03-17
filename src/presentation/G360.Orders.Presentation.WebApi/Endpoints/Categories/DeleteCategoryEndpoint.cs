using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Categories;

public class DeleteCategoryEndpoint(IMediator mediator) : Endpoint<DeleteCategoryRequest, Response>
{
    public override void Configure()
    {
        Delete("/categories/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Category"));
    }

    public override async Task HandleAsync(DeleteCategoryRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteCategoryCommand { Id = req.Id }, ct);
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

public class DeleteCategoryRequest
{
    public long Id { get; set; }
}
