using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.Categories;

public class CreateCategoryEndpoint(IMediator mediator) : Endpoint<CreateCategoryRequest, Response<Category>>
{
    public override void Configure()
    {
        Post("/categories");
        AllowAnonymous();
        Description(d => d.WithTags("Category"));
    }

    public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
    {
        var command = new CreateCategoryCommand { Name = req.Name };
        var result = await mediator.Send(command, ct);
        if (result.Success && result.Data is not null)
        {
            await SendCreatedAtAsync<GetCategoryByIdEndpoint>(
                new
                {
                    id = result.Data.Id
                },
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

public class CreateCategoryRequest
{
    public required string Name { get; set; }
}
