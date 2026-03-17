using MediatR;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetPizzaByIdQueryHandler(IRepository<Pizza> pizzaRepository) : IRequestHandler<GetPizzaByIdQuery, Response<Pizza>>
{
    public async Task<Response<Pizza>> Handle(GetPizzaByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pizza = await pizzaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (pizza == null)
            {
                return new Response<Pizza>(success: false, messages: ["Pizza not found."]);
            }

            return new Response<Pizza>(success: true, messages: ["Pizza retrieved successfully."], data: pizza);
        }
        catch (Exception ex)
        {
            return new Response<Pizza>(success: false, messages: [$"Failed to retrieve pizza: {ex.Message}"]);
        }
    }
}
