using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class UpdateCategoryCommandHandler(IRepository<Category> repository) : IRequestHandler<UpdateCategoryCommand, Response<Category>>
{
    public async Task<Response<Category>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return new Response<Category>(false, ["Category not found."]);
            }

            if (request.Name is not null)
            {
                entity.Name = request.Name;
            }

            await repository.UpdateAsync(entity, cancellationToken);
            var updated = await repository.GetByIdAsync(request.Id, cancellationToken);
            return new Response<Category>(true, ["Category updated successfully."], updated!);
        }
        catch (Exception ex)
        {
            return new Response<Category>(false, [$"Failed to update category: {ex.Message}"]);
        }
    }
}
