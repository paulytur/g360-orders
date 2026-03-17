namespace G360.Orders.Domain.Interfaces;

/// <summary>
/// Repository for pizza details with replace-by-pizza-type support (used when saving/updating pizza type with ingredients).
/// </summary>
public interface IPizzaDetailRepository : IRepository<G360.Orders.Domain.Entities.PizzaDetail>
{
    /// <summary>
    /// Soft-deletes existing pizza details for the type and adds new rows for each ingredient id.
    /// </summary>
    Task ReplaceForPizzaTypeAsync(long pizzaTypeId, IEnumerable<long> ingredientIds, CancellationToken cancellationToken = default);
}
