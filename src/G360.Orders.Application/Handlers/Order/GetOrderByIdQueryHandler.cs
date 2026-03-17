using MediatR;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
namespace G360.Orders.Application.Handlers;

public class GetOrderByIdQueryHandler(IRepository<Order> orderRepository) : IRequestHandler<GetOrderByIdQuery, Response<Order>>
{
    public async Task<Response<Order>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (order == null)
            {
                return new Response<Order>(success: false, messages: ["Order not found."]);
            }

            return new Response<Order>(success: true, messages: ["Order retrieved successfully."], data: order);
        }
        catch (Exception ex)
        {
            return new Response<Order>(success: false, messages: [$"Failed to retrieve order: {ex.Message}"]);
        }
    }
}
