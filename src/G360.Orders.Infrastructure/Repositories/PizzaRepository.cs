using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class PizzaRepository(OrdersDbContext context) : IRepository<Pizza>
{
    public async Task<Pizza> AddAsync(Pizza entity, CancellationToken cancellationToken = default)
    {
        context.Pizzas.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<Pizza>> BulkAddAsync(ICollection<Pizza> entities, CancellationToken cancellationToken = default)
    {
        context.Pizzas.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(Pizza updatedEntity, CancellationToken cancellationToken = default)
    {
        context.Pizzas.Update(updatedEntity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Pizza?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public IQueryable<Pizza> GetAll(CancellationToken cancellationToken = default) =>
        context.Pizzas.AsNoTracking();
}
