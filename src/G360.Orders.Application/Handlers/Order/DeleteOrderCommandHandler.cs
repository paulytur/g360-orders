using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
namespace G360.Orders.Application.Handlers;

public class DeleteOrderCommandHandler(IRepository<Order> orderRepository) : IRequestHandler<DeleteOrderCommand, Response>
{
    public async Task<Response> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (order == null)
            {
                return new Response(success: false, messages: ["Order not found."]);
            }

            order.IsDeleted = true;
            await orderRepository.UpdateAsync(order, cancellationToken);

            return new Response(success: true, messages: ["Order deleted successfully."]);
        }
        catch (Exception ex)
        {
            return new Response(success: false, messages: [$"Failed to delete order: {ex.Message}"]);
        }
    }
}
