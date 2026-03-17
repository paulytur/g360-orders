using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;
using G360.Orders.Domain.DTO;

namespace G360.Orders.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context) => _context = context;

    public async Task<Order> AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        _context.Orders.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<Order>> BulkAddAsync(ICollection<Order> entities, CancellationToken cancellationToken = default)
    {
        _context.Orders.AddRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> UpdateAsync(Order updatedEntity, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(updatedEntity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderDetails.Where(d => !d.IsDeleted))
            .ThenInclude(d => d.Pizza)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public IQueryable<Order> GetAll(CancellationToken cancellationToken = default) =>
        _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderDetails.Where(d => !d.IsDeleted))
            .ThenInclude(d => d.Pizza);

    public async Task ReplaceOrderDetailsAsync(long orderId, IEnumerable<OrderDetailUpdate> items, CancellationToken cancellationToken = default)
    {
        var existing = await _context.OrderDetails
            .Where(d => d.OrderId == orderId && !d.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var d in existing)
        {
            d.IsDeleted = true;
        }

        foreach (var item in items)
        {
            _context.OrderDetails.Add(new OrderDetail
            {
                OrderId = orderId,
                PizzaId = item.PizzaId,
                Quantity = item.Quantity
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
