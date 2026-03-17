using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class CreatePizzaCommandHandler(IRepository<Pizza> pizzaRepository) : IRequestHandler<CreatePizzaCommand, Response<Pizza>>
{
    public async Task<Response<Pizza>> Handle(CreatePizzaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pizza = new Pizza
            {
                Code = request.Code,
                PizzaTypeId = request.PizzaTypeId,
                Size = request.Size,
                Price = request.Price
            };
            var created = await pizzaRepository.AddAsync(pizza, cancellationToken);
            return new Response<Pizza>(success: true, messages: ["Pizza created successfully."], data: created);
        }
        catch (Exception ex)
        {
            return new Response<Pizza>(success: false, messages: [$"Failed to create pizza: {ex.Message}"]);
        }
    }
}
