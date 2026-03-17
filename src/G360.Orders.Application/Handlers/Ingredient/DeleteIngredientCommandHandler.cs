using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class DeleteIngredientCommandHandler(IRepository<Ingredient> ingredientRepository) : IRequestHandler<DeleteIngredientCommand, Response>
{
    public async Task<Response> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ingredient = await ingredientRepository.GetByIdAsync(request.Id, cancellationToken);
            if (ingredient == null)
            {
                return new Response(success: false, messages: ["Ingredient not found."]);
            }

            ingredient.IsDeleted = true;
            await ingredientRepository.UpdateAsync(ingredient, cancellationToken);

            return new Response(success: true, messages: ["Ingredient deleted successfully."]);
        }
        catch (Exception ex)
        {
            return new Response(success: false, messages: [$"Failed to delete ingredient: {ex.Message}"]);
        }
    }
}
