using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

public class GetCategoryByIdQuery : IRequest<Response<Category>>
{
    public long Id { get; set; }
    public bool IncludeDeleted { get; set; }
}
