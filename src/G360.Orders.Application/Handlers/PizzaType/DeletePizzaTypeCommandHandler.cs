using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class DeletePizzaTypeCommandHandler(IRepository<PizzaType> repository) : IRequestHandler<DeletePizzaTypeCommand, Response>
{
    public async Task<Response> Handle(DeletePizzaTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return new Response(false, ["Pizza type not found."]);
            }
            entity.IsDeleted = true;
            await repository.UpdateAsync(entity, cancellationToken);
            return new Response(true, ["Pizza type deleted successfully."]);
        }
        catch (Exception ex)
        {
            return new Response(false, [$"Failed to delete pizza type: {ex.Message}"]);
        }
    }
}
