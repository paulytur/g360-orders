using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class UpdatePizzaCommandHandler(IRepository<Pizza> pizzaRepository) : IRequestHandler<UpdatePizzaCommand, Response<Pizza>>
{
    public async Task<Response<Pizza>> Handle(UpdatePizzaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pizza = await pizzaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (pizza == null)
            {
                return new Response<Pizza>(success: false, messages: ["Pizza not found."]);
            }

            if (request.Code is not null)
            {
                pizza.Code = request.Code;
            }
            if (request.PizzaTypeId is not null)
            {
                pizza.PizzaTypeId = request.PizzaTypeId;
            }
            if (request.Size is not null)
            {
                pizza.Size = request.Size;
            }
            if (request.Price.HasValue)
            {
                pizza.Price = request.Price.Value;
            }

            await pizzaRepository.UpdateAsync(pizza, cancellationToken);
            var updated = await pizzaRepository.GetByIdAsync(request.Id, cancellationToken);
            return new Response<Pizza>(success: true, messages: ["Pizza updated successfully."], data: updated!);
        }
        catch (Exception ex)
        {
            return new Response<Pizza>(success: false, messages: [$"Failed to update pizza: {ex.Message}"]);
        }
    }
}
