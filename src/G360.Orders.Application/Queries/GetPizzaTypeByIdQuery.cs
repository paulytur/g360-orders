using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Queries;

public class GetPizzaTypeByIdQuery : IRequest<Response<PizzaType>>
{
    public long Id { get; set; }
}
