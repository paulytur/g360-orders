using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class PizzaDetailRepository(OrdersDbContext context) : IPizzaDetailRepository
{
    public async Task<PizzaDetail> AddAsync(PizzaDetail entity, CancellationToken cancellationToken = default)
    {
        context.PizzaDetails.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<PizzaDetail>> BulkAddAsync(ICollection<PizzaDetail> entities, CancellationToken cancellationToken = default)
    {
        context.PizzaDetails.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(PizzaDetail updatedEntity, CancellationToken cancellationToken = default)
    {
        context.PizzaDetails.Update(updatedEntity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<PizzaDetail?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.PizzaDetails
            .AsNoTracking()
            .Include(p => p.Ingredient)
            .Include(p => p.PizzaType)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public IQueryable<PizzaDetail> GetAll(CancellationToken cancellationToken = default) =>
        context.PizzaDetails
            .AsNoTracking()
            .Include(p => p.Ingredient)
            .Include(p => p.PizzaType);

    public async Task ReplaceForPizzaTypeAsync(long pizzaTypeId, IEnumerable<long> ingredientIds, CancellationToken cancellationToken = default)
    {
        var existing = await context.PizzaDetails
            .Where(d => d.PizzaTypeId == pizzaTypeId && !d.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var d in existing)
        {
            d.IsDeleted = true;
        }
        foreach (var ingredientId in ingredientIds.Distinct())
        {
            context.PizzaDetails.Add(new PizzaDetail
            {
                PizzaTypeId = pizzaTypeId,
                IngredientId = ingredientId
            });
        }
        await context.SaveChangesAsync(cancellationToken);
    }
}
