using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

public class CreateCategoryCommand : IRequest<Response<Category>>
{
    public required string Name { get; set; }
}
