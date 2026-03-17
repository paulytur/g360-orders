using MediatR;
using G360.Orders.Application.Commands;
using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Application.Handlers;

public class CreateCategoryCommandHandler(IRepository<Category> repository) : IRequestHandler<CreateCategoryCommand, Response<Category>>
{
    public async Task<Response<Category>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new Category { Name = request.Name };
            var created = await repository.AddAsync(entity, cancellationToken);
            return new Response<Category>(true, ["Category created successfully."], created);
        }
        catch (Exception ex)
        {
            return new Response<Category>(false, [$"Failed to create category: {ex.Message}"]);
        }
    }
}
