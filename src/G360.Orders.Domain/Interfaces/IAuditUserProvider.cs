namespace G360.Orders.Domain.Interfaces;

/// <summary>
/// Provides the current user identifier for audit fields.
/// Set via SetCurrentUser (e.g. from request) so AuditSaveChangesInterceptor can use it.
/// </summary>
public interface IAuditUserProvider
{
    string? GetCurrentUser();

    /// <summary>
    /// Sets the audit user for the current request (e.g. from X-User-Id header via middleware).
    /// Used so the interceptor can apply it when saving.
    /// </summary>
    void SetCurrentUser(string? user);
}
