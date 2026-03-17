using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class DeleteCategoryCommandHandler(IRepository<Category> repository) : IRequestHandler<DeleteCategoryCommand, Response>
{
    public async Task<Response> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return new Response(false, ["Category not found."]);
            }

            entity.IsDeleted = true;
            await repository.UpdateAsync(entity, cancellationToken);

            return new Response(true, ["Category deleted successfully."]);
        }
        catch (Exception ex)
        {
            return new Response(false, [$"Failed to delete category: {ex.Message}"]);
        }
    }
}
