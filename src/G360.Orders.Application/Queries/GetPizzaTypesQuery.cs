using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

public class GetPizzaTypesQuery : IRequest<PagedResponse<PizzaType>>
{
    public string? Code { get; set; }
    public long? CategoryId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; }
}
