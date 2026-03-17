using MediatR;
using Microsoft.EntityFrameworkCore;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetPizzaTypesQueryHandler(IRepository<PizzaType> repository) : IRequestHandler<GetPizzaTypesQuery, PagedResponse<PizzaType>>
{
    public async Task<PagedResponse<PizzaType>> Handle(GetPizzaTypesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = repository.GetAll(cancellationToken);
            if (!request.IncludeDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }
            if (!string.IsNullOrEmpty(request.Code))
            {
                query = query.Where(p => p.Code == request.Code);
            }
            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(p => p.Code)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<PizzaType>(true, ["Pizza types retrieved successfully."], request.PageNumber, request.PageSize, totalCount, items);
        }
        catch (Exception ex)
        {
            return new PagedResponse<PizzaType>(false, [$"Failed to retrieve pizza types: {ex.Message}"]);
        }
    }
}
