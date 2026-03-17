using System.Net;
using FastEndpoints;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.PizzaTypes;

public class UpdatePizzaTypeEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<UpdatePizzaTypeRequest, Response<PizzaTypeResponse>>
{
    public override void Configure()
    {
        Put("/pizza-types/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Pizza Types"));
    }

    public override async Task HandleAsync(UpdatePizzaTypeRequest req, CancellationToken ct)
    {
        var command = new UpdatePizzaTypeCommand
        {
            Id = req.Id,
            Code = req.Code,
            Name = req.Name,
            CategoryId = req.CategoryId,
            IngredientIds = req.IngredientIds
        };
        var result = await mediator.Send(command, ct);
        if (!result.Success)
        {
            await SendAsync(
                new Response<PizzaTypeResponse>(false, result.Messages),
                (int)HttpStatusCode.BadRequest,
                ct);
            return;
        }
        var response = new Response<PizzaTypeResponse>(
            true,
            result.Messages,
            result.Data is not null
                ? mapper.Map<PizzaTypeResponse>(result.Data)
                : null);
        await SendOkAsync(response, ct);
    }
}

public class UpdatePizzaTypeRequest
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public long? CategoryId { get; set; }
    public List<long>? IngredientIds { get; set; }
}
