using G360.Orders.Application.Helpers;
using G360.Orders.Application.Models;
using G360.Orders.Domain.Entities;
using MediatR;

namespace G360.Orders.Application.Commands;

/// <summary>
/// Command to create a new order with order details.
/// </summary>
public class CreateOrderCommand : IRequest<Response<Order>>
{
    public List<OrderDetailItem> Items { get; set; } = [];
}

