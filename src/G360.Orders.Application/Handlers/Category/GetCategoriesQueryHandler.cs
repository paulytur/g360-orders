using MediatR;
using Microsoft.EntityFrameworkCore;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetCategoriesQueryHandler(IRepository<Category> repository) : IRequestHandler<GetCategoriesQuery, PagedResponse<Category>>
{
    public async Task<PagedResponse<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = repository.GetAll(cancellationToken);
            
            if (!request.IncludeDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(c => c.Name != null && c.Name.Contains(request.Name));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<Category>(true, ["Category retrieved successfully."], request.PageNumber, request.PageSize, totalCount, items);
        }
        catch (Exception ex)
        {
            return new PagedResponse<Category>(false, [$"Failed to retrieve categories: {ex.Message}"]);
        }
    }
}
