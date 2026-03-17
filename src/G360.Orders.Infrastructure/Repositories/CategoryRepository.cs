using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class CategoryRepository(OrdersDbContext context) : IRepository<Category>
{
    public async Task<Category> AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        context.Categories.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<Category>> BulkAddAsync(ICollection<Category> entities, CancellationToken cancellationToken = default)
    {
        context.Categories.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(Category updatedEntity, CancellationToken cancellationToken = default)
    {
        context.Categories.Update(updatedEntity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public IQueryable<Category> GetAll(CancellationToken cancellationToken = default) =>
        context.Categories
            .AsNoTracking();
}
