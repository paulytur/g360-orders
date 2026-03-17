using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class CreateIngredientCommandHandler(IRepository<Ingredient> ingredientRepository) : IRequestHandler<CreateIngredientCommand, Response<Ingredient>>
{
    public async Task<Response<Ingredient>> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ingredient = new Ingredient { Description = request.Description };
            var created = await ingredientRepository.AddAsync(ingredient, cancellationToken);
            return new Response<Ingredient>(success: true, messages: ["Ingredient created successfully."], data: created);
        }
        catch (Exception ex)
        {
            return new Response<Ingredient>(success: false, messages: [$"Failed to create ingredient: {ex.Message}"]);
        }
    }
}
