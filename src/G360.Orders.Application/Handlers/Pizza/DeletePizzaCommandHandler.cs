using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class DeletePizzaCommandHandler(IRepository<Pizza> pizzaRepository) : IRequestHandler<DeletePizzaCommand, Response>
{
    public async Task<Response> Handle(DeletePizzaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pizza = await pizzaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (pizza == null)
            {
                return new Response(success: false, messages: ["Pizza not found."]);
            }

            pizza.IsDeleted = true;
            await pizzaRepository.UpdateAsync(pizza, cancellationToken);

            return new Response(success: true, messages: ["Pizza deleted successfully."]);
        }
        catch (Exception ex)
        {
            return new Response(success: false, messages: [$"Failed to delete pizza: {ex.Message}"]);
        }
    }
}
