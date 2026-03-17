using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class PizzaTypeRepository(OrdersDbContext context) : IRepository<PizzaType>
{
    public async Task<PizzaType> AddAsync(PizzaType entity, CancellationToken cancellationToken = default)
    {
        context.PizzaTypes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<PizzaType>> BulkAddAsync(ICollection<PizzaType> entities, CancellationToken cancellationToken = default)
    {
        context.PizzaTypes.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(PizzaType updatedEntity, CancellationToken cancellationToken = default)
    {
        context.PizzaTypes.Update(updatedEntity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<PizzaType?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.PizzaTypes
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.PizzaDetails.Where(d => !d.IsDeleted))
            .ThenInclude(d => d.Ingredient)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public IQueryable<PizzaType> GetAll(CancellationToken cancellationToken = default) =>
        context.PizzaTypes
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.PizzaDetails.Where(d => !d.IsDeleted))
            .ThenInclude(d => d.Ingredient);
}
