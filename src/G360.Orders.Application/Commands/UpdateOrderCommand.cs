using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to update an existing order.
/// </summary>
public class UpdateOrderCommand : IRequest<Response<Order>>
{
    public long Id { get; set; }
    public List<OrderDetailItem>? Items { get; set; }
}
