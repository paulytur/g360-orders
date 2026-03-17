using MediatR;
using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class GetIngredientByIdQueryHandler(IRepository<Ingredient> ingredientRepository) : IRequestHandler<GetIngredientByIdQuery, Response<Ingredient>>
{
    public async Task<Response<Ingredient>> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var ingredient = await ingredientRepository.GetByIdAsync(request.Id, cancellationToken);
            if (ingredient == null)
            {
                return new Response<Ingredient>(success: false, messages: ["Ingredient not found."]);
            }

            return new Response<Ingredient>(success: true, messages: ["Ingredient retrieved successfully."], data: ingredient);
        }
        catch (Exception ex)
        {
            return new Response<Ingredient>(success: false, messages: [$"Failed to retrieve ingredient: {ex.Message}"]);
        }
    }
}
