using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
namespace G360.Orders.Application.Handlers;

public class CreateOrderCommandHandler(
    IRepository<Order> orderRepository,
    IRepository<OrderDetail> orderDetailRepository) : IRequestHandler<CreateOrderCommand, Response<Order>>
{
    public async Task<Response<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = new Order();
            var createdOrder = await orderRepository.AddAsync(order, cancellationToken);

            if (request.Items?.Count > 0)
            {
                var details = request.Items.Select(item => new OrderDetail
                {
                    OrderId = createdOrder.Id,
                    PizzaId = item.PizzaId,
                    Quantity = item.Quantity
                }).ToList();
                await orderDetailRepository.BulkAddAsync(details, cancellationToken);
            }

            return new Response<Order>(success: true, messages: ["Order created successfully."], data: createdOrder);
        }
        catch (Exception ex)
        {
            return new Response<Order>(success: false, messages: [$"Failed to create order: {ex.Message}"]);
        }
    }
}
