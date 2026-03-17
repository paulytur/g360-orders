using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class UpdatePizzaTypeCommandHandler(
    IRepository<PizzaType> repository,
    IPizzaDetailRepository pizzaDetailRepository) : IRequestHandler<UpdatePizzaTypeCommand, Response<PizzaType>>
{
    public async Task<Response<PizzaType>> Handle(UpdatePizzaTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return new Response<PizzaType>(false, ["Pizza type not found."]);
            }

            if (request.Code is not null)
            {
                entity.Code = request.Code;
            }
            if (request.Name is not null)
            {
                entity.Name = request.Name;
            }
            if (request.CategoryId.HasValue)
            {
                entity.CategoryId = request.CategoryId.Value;
                entity.Category = null; // clear nav so EF persists the FK; otherwise loaded Category can override it
            }

            await repository.UpdateAsync(entity, cancellationToken);

            if (request.IngredientIds is not null)
            {
                await pizzaDetailRepository.ReplaceForPizzaTypeAsync(request.Id, request.IngredientIds, cancellationToken);
            }

            var updated = await repository.GetByIdAsync(request.Id, cancellationToken);
            return new Response<PizzaType>(true, ["Pizza type updated successfully."], updated!);
        }
        catch (Exception ex)
        {
            return new Response<PizzaType>(false, [$"Failed to update pizza type: {ex.Message}"]);
        }
    }
}
