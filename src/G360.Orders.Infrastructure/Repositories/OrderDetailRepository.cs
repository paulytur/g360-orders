using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class OrderDetailRepository(OrdersDbContext context) : IRepository<OrderDetail>
{
    public async Task<OrderDetail> AddAsync(OrderDetail entity, CancellationToken cancellationToken = default)
    {
        context.OrderDetails.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<OrderDetail>> BulkAddAsync(ICollection<OrderDetail> entities, CancellationToken cancellationToken = default)
    {
        context.OrderDetails.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(OrderDetail updatedEntity, CancellationToken cancellationToken = default)
    {
        context.OrderDetails.Update(updatedEntity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<OrderDetail?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.OrderDetails.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public IQueryable<OrderDetail> GetAll(CancellationToken cancellationToken = default) =>
        context.OrderDetails.AsNoTracking();
}
