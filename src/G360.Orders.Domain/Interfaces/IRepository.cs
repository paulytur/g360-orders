namespace G360.Orders.Domain.Interfaces;

/// <summary>
/// Defines a generic repository interface for entities.
/// </summary>
/// <typeparam name="T">The entity type, which must implement <see cref="IEntity"/>.</typeparam>
public interface IRepository<T> where T : IEntity
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<ICollection<T>> BulkAddAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(T updatedEntity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    IQueryable<T> GetAll(CancellationToken cancellationToken = default);
}
