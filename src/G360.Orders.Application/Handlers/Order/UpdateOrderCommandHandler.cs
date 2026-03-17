using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Domain.DTO;
namespace G360.Orders.Application.Handlers;


public class UpdateOrderCommandHandler(
    IRepository<Order> orderRepository,
    IOrderRepository orderRepositoryExt) : IRequestHandler<UpdateOrderCommand, Response<Order>>
{
    public async Task<Response<Order>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (order == null)
            {
                return new Response<Order>(success: false, messages: ["Order not found."]);
            }

            await orderRepository.UpdateAsync(order, cancellationToken);

            if (request.Items is not null)
            {
                var updates = request.Items.Select(i => new OrderDetailUpdate(i.PizzaId, i.Quantity));
                await orderRepositoryExt.ReplaceOrderDetailsAsync(request.Id, updates, cancellationToken);
            }

            var updated = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
            return new Response<Order>(success: true, messages: ["Order updated successfully."], data: updated!);
        }
        catch (Exception ex)
        {
            return new Response<Order>(success: false, messages: [$"Failed to update order: {ex.Message}"]);
        }
    }
}
