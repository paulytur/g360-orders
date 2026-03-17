using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class CreatePizzaTypeCommandHandler(
    IRepository<PizzaType> repository,
    IRepository<PizzaDetail> pizzaDetailRepository) : IRequestHandler<CreatePizzaTypeCommand, Response<PizzaType>>
{
    public async Task<Response<PizzaType>> Handle(CreatePizzaTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new PizzaType
            {
                Code = request.Code,
                Name = request.Name,
                CategoryId = request.CategoryId
            };
            var created = await repository.AddAsync(entity, cancellationToken);

            if (request.IngredientIds is { Count: > 0 })
            {
                var details = request.IngredientIds.Distinct().Select(ingredientId => new PizzaDetail
                {
                    PizzaTypeId = created.Id,
                    IngredientId = ingredientId
                }).ToList();
                await pizzaDetailRepository.BulkAddAsync(details, cancellationToken);
            }

            var withDetails = await repository.GetByIdAsync(created.Id, cancellationToken);
            return new Response<PizzaType>(true, ["Pizza type created successfully."], withDetails ?? created);
        }
        catch (Exception ex)
        {
            return new Response<PizzaType>(false, [$"Failed to create pizza type: {ex.Message}"]);
        }
    }
}
