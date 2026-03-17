using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Presentation.WebApi.Helpers;

/// <summary>
/// Provides the current user for audit fields. The value is set from the X-User-Id request header
/// by AuditUserMiddleware, then used by AuditSaveChangesInterceptor for audit columns.
/// </summary>
public class AuditUserProvider(IHttpContextAccessor httpContextAccessor) : IAuditUserProvider
{
    private string? _requestUser;

    public string? GetCurrentUser()
    {
        return _requestUser;
    }

    /// <summary>
    /// Set by AuditUserMiddleware from X-User-Id header. Can also be set explicitly for testing.
    /// </summary>
    public void SetCurrentUser(string? user)
    {
        _requestUser = user;
    }
}
