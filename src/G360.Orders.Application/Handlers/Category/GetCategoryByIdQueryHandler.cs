using MediatR;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetCategoryByIdQueryHandler(IRepository<Category> repository) : IRequestHandler<GetCategoryByIdQuery, Response<Category>>
{
    public async Task<Response<Category>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return new Response<Category>(false, ["Category not found."]);
            }
            
            return new Response<Category>(true, ["Category retrieved successfully."], entity);
        }
        catch (Exception ex)
        {
            return new Response<Category>(false, [$"Failed to retrieve category: {ex.Message}"]);
        }
    }
}
