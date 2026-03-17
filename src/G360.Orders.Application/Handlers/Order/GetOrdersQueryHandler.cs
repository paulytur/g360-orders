using MediatR;
using Microsoft.EntityFrameworkCore;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
namespace G360.Orders.Application.Handlers;

public class GetOrdersQueryHandler(IRepository<Order> orderRepository) : IRequestHandler<GetOrdersQuery, PagedResponse<Order>>
{
    public async Task<PagedResponse<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = orderRepository.GetAll(cancellationToken);
            if (!request.IncludeDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(e => e.CreatedDatetime)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<Order>(
                success: true,
                messages: ["Orders retrieved successfully."],
                page: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount,
                data: items);
        }
        catch (Exception ex)
        {
            return new PagedResponse<Order>(success: false, messages: [$"Failed to retrieve orders: {ex.Message}"]);
        }
    }
}
