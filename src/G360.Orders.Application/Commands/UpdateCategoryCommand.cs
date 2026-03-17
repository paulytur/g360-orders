using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

public class UpdateCategoryCommand : IRequest<Response<Category>>
{
    public long Id { get; set; }
    public string? Name { get; set; }
}
