using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Infrastructure.Interceptors;

/// <summary>
/// Interceptor that sets audit fields (CreatedBy, UpdatedBy, CreatedDatetime, UpdatedDatetime, IsDeleted) on entities implementing IAuditableEntity.
/// </summary>
public class AuditSaveChangesInterceptor(IAuditUserProvider auditUserProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAudit(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ApplyAudit(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext context)
    {
        var user = auditUserProvider.GetCurrentUser();
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditableEntity auditable)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.CreatedBy = user;
                    auditable.CreatedDatetime = now;
                    auditable.UpdatedBy = user;
                    auditable.UpdatedDatetime = now;
                    auditable.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    auditable.UpdatedBy = user;
                    auditable.UpdatedDatetime = now;
                    entry.Property(nameof(IAuditableEntity.CreatedBy)).IsModified = false;
                    entry.Property(nameof(IAuditableEntity.CreatedDatetime)).IsModified = false;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    auditable.IsDeleted = true;
                    auditable.UpdatedBy = user;
                    auditable.UpdatedDatetime = now;
                    break;
            }
        }
    }
}
