using MediatR;
using Microsoft.EntityFrameworkCore;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetPizzasQueryHandler(IRepository<Pizza> pizzaRepository) : IRequestHandler<GetPizzasQuery, PagedResponse<Pizza>>
{
    public async Task<PagedResponse<Pizza>> Handle(GetPizzasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = pizzaRepository.GetAll(cancellationToken);
            if (!request.IncludeDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }
            if (!string.IsNullOrEmpty(request.Code))
            {
                query = query.Where(p => p.Code == request.Code);
            }
            if (request.PizzaTypeId is not null)
            {
                query = query.Where(p => p.PizzaTypeId == request.PizzaTypeId);
            }
            if (!string.IsNullOrEmpty(request.Size))
            {
                query = query.Where(p => p.Size == request.Size);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(p => p.Code)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<Pizza>(
                success: true,
                messages: ["Pizzas retrieved successfully."],
                page: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount,
                data: items);
        }
        catch (Exception ex)
        {
            return new PagedResponse<Pizza>(success: false, messages: [$"Failed to retrieve pizzas: {ex.Message}"]);
        }
    }
}
