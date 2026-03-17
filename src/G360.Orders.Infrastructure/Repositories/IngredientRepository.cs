using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class IngredientRepository(OrdersDbContext context) : IRepository<Ingredient>
{
    public async Task<Ingredient> AddAsync(Ingredient entity, CancellationToken cancellationToken = default)
    {
        context.Ingredients.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<Ingredient>> BulkAddAsync(ICollection<Ingredient> entities, CancellationToken cancellationToken = default)
    {
        context.Ingredients.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(Ingredient updatedEntity, CancellationToken cancellationToken = default)
    {
        context.Ingredients.Update(updatedEntity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Ingredient?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.Ingredients.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public IQueryable<Ingredient> GetAll(CancellationToken cancellationToken = default) =>
        context.Ingredients.AsNoTracking();
}
