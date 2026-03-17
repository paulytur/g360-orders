using G360.Orders.Domain.Entities;
using G360.Orders.Domain.DTO;

namespace G360.Orders.Domain.Interfaces;

/// <summary>
/// Extended order repository for operations that require order details.
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Replaces order details for an order (soft-deletes existing and adds new lines).
    /// </summary>
    Task ReplaceOrderDetailsAsync(long orderId, IEnumerable<OrderDetailUpdate> items, CancellationToken cancellationToken = default);
}

