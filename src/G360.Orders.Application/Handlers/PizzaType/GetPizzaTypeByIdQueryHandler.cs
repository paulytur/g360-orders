using MediatR;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetPizzaTypeByIdQueryHandler(IRepository<PizzaType> repository) : IRequestHandler<GetPizzaTypeByIdQuery, Response<PizzaType>>
{
    public async Task<Response<PizzaType>> Handle(GetPizzaTypeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return new Response<PizzaType>(false, ["Pizza type not found."]);
            }
            return new Response<PizzaType>(true, ["Pizza type retrieved successfully."], entity);
        }
        catch (Exception ex)
        {
            return new Response<PizzaType>(false, [$"Failed to retrieve pizza type: {ex.Message}"]);
        }
    }
}
