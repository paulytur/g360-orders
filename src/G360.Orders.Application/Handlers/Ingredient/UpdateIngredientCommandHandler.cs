using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class UpdateIngredientCommandHandler(IRepository<Ingredient> ingredientRepository) : IRequestHandler<UpdateIngredientCommand, Response<Ingredient>>
{
    public async Task<Response<Ingredient>> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ingredient = await ingredientRepository.GetByIdAsync(request.Id, cancellationToken);
            if (ingredient == null)
            {
                return new Response<Ingredient>(success: false, messages: ["Ingredient not found."]);
            }

            if (request.Description is not null)
            {
                ingredient.Description = request.Description;
            }

            await ingredientRepository.UpdateAsync(ingredient, cancellationToken);
            var updated = await ingredientRepository.GetByIdAsync(request.Id, cancellationToken);
            return new Response<Ingredient>(success: true, messages: ["Ingredient updated successfully."], data: updated!);
        }
        catch (Exception ex)
        {
            return new Response<Ingredient>(success: false, messages: [$"Failed to update ingredient: {ex.Message}"]);
        }
    }
}
