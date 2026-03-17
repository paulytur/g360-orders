using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Application.Queries;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.PizzaTypes;

public class GetPizzaTypeByIdEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<GetPizzaTypeByIdRequest, Response<PizzaTypeResponse>>
{
    public override void Configure()
    {
        Get("/pizza-types/{id}");
        AllowAnonymous();
        Description(d => d.WithTags("Pizza Types"));
    }

    public override async Task HandleAsync(GetPizzaTypeByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPizzaTypeByIdQuery { Id = req.Id }, ct);
        if (result.Success && result.Data is not null)
        {
            var response = new Response<PizzaTypeResponse>(
                true,
                result.Messages,
                mapper.Map<PizzaTypeResponse>(result.Data));
            await SendOkAsync(response, ct);
            return;
        }
        if (result.Success == false)
        {
            await SendAsync(
                new Response<PizzaTypeResponse>(false, result.Messages),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}

public class GetPizzaTypeByIdRequest
{
    public long Id { get; set; }
}
