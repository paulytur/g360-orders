using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Categories;

public class UpdateCategoryEndpoint(IMediator mediator) : Endpoint<UpdateCategoryRequest, Response<Category>>
{
    public override void Configure()
    {
        Put("/categories/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Category"));
    }

    public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand { Id = req.Id, Name = req.Name };
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

public class UpdateCategoryRequest
{
    public long Id { get; set; }
    public string? Name { get; set; }
}
