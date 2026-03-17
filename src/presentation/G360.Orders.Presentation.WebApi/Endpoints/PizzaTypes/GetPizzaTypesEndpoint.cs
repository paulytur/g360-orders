using System.Net;
using FastEndpoints;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Presentation.WebApi.Endpoints.PizzaTypes;

public class GetPizzaTypesEndpoint(IMediator mediator, AutoMapper.IMapper mapper) : Endpoint<GetPizzaTypesRequest, PagedResponse<PizzaTypeResponse>>
{
    public override void Configure()
    {
        Get("/pizza-types");
        AllowAnonymous();
        Description(d => d.WithTags("Pizza Types"));
    }

    public override async Task HandleAsync(GetPizzaTypesRequest req, CancellationToken ct)
    {
        var query = new GetPizzaTypesQuery
        {
            Code = req.Code,
            CategoryId = req.CategoryId,
            PageNumber = req.PageNumber ?? 1,
            PageSize = req.PageSize ?? 10,
            IncludeDeleted = req.IncludeDeleted ?? false
        };
        
        var result = await mediator.Send(query, ct);
        
        if (!result.Success)
        {
            await SendAsync(
                new PagedResponse<PizzaTypeResponse>(false, result.Messages),
                (int)HttpStatusCode.BadRequest,
                ct);
            return;
        }
        var data = mapper.Map<List<PizzaTypeResponse>>(result.Data);
        var response = new PagedResponse<PizzaTypeResponse>(
            result.Success,
            result.Messages,
            result.Page,
            result.PageSize,
            result.TotalCount,
            data);
        await SendOkAsync(response, ct);
    }
}

public class GetPizzaTypesRequest
{
    public string? Code { get; set; }
    public long? CategoryId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeDeleted { get; set; }
}
