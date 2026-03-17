using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.PizzaTypes;

public class CreatePizzaTypeEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<CreatePizzaTypeRequest, Response<PizzaTypeResponse>>
{
    public override void Configure()
    {
        Post("/pizza-types");
        AllowAnonymous();
        Description(d => d.WithTags("Pizza Types"));
    }

    public override async Task HandleAsync(CreatePizzaTypeRequest req, CancellationToken ct)
    {
        var command = new CreatePizzaTypeCommand
        {
            Code = req.Code,
            Name = req.Name,
            CategoryId = req.CategoryId,
            IngredientIds = req.IngredientIds ?? []
        };
        var result = await mediator.Send(command, ct);
        if (result.Success && result.Data is not null)
        {
            var response = new Response<PizzaTypeResponse>(
                true,
                result.Messages,
                mapper.Map<PizzaTypeResponse>(result.Data));
            await SendCreatedAtAsync<GetPizzaTypeByIdEndpoint>(
                new
                {
                    id = result.Data.Id
                },
                response,
                cancellation: ct);
        }
        else
        {
            await SendAsync(
                new Response<PizzaTypeResponse>(false, result.Messages),
                (int)HttpStatusCode.BadRequest,
                ct);
        }
    }
}

public class CreatePizzaTypeRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public long? CategoryId { get; set; }
    public List<long> IngredientIds { get; set; } = [];
}
