using G360.Orders.Application.Helpers;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to soft-delete an order.
/// </summary>
public class DeleteOrderCommand : IRequest<Response>
{
    public long Id { get; set; }
}
