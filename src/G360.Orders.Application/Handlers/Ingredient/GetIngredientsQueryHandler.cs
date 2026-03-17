using MediatR;
using Microsoft.EntityFrameworkCore;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetIngredientsQueryHandler(IRepository<Ingredient> ingredientRepository) : IRequestHandler<GetIngredientsQuery, PagedResponse<Ingredient>>
{
    public async Task<PagedResponse<Ingredient>> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = ingredientRepository.GetAll(cancellationToken);
            if (!request.IncludeDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                query = query.Where(i => i.Description !=  null && i.Description.Contains(request.Description));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(i => i.Description)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<Ingredient>(
                success: true,
                messages: ["Ingredient retrieved successfully."],
                page: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount,
                data: items);
        }
        catch (Exception ex)
        {
            return new PagedResponse<Ingredient>(success: false, messages: [$"Failed to retrieve ingredients: {ex.Message}"]);
        }
    }
}
