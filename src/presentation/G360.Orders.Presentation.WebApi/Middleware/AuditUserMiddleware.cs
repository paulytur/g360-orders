using System.Net;
using System.Text.Json;
using G360.Orders.Domain.Interfaces;

namespace G360.Orders.Presentation.WebApi.Middleware;

/// <summary>
/// Sets the current audit user from the X-User-Id request header so AuditSaveChangesInterceptor uses it for audit columns.
/// Rejects requests when X-User-Id is missing or empty, unless the path is in the bypass list (/swagger, /scalar, /health);
/// bypass paths use a default audit user so GetCurrentUser() is never null.
/// </summary>
public class AuditUserMiddleware(RequestDelegate next)
{
    public const string XUserIdHeaderName = "X-User-Id";

    /// <summary>
    /// Path prefixes that do not require X-User-Id. Requests to these paths use <see cref="BypassPathAuditUser"/> for audit instead of null.
    /// </summary>
    private static readonly string[] BypassPathPrefixes = ["/swagger", "/scalar", "/health", "/graphql"];

    /// <summary>
    /// Audit user value used for bypass paths (/swagger, /scalar, /health) so GetCurrentUser() is never null.
    /// </summary>
    public const string BypassPathAuditUser = "system";

    public async Task InvokeAsync(HttpContext context, IAuditUserProvider auditUserProvider)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var bypass = BypassPathPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        if (bypass)
        {
            auditUserProvider.SetCurrentUser(BypassPathAuditUser);
            await next(context);
            return;
        }

        var userId = context.Request.Headers[XUserIdHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new
            {
                success = false,
                messages = new[] { "The X-User-Id request header is required and must not be empty." }
            });
            await context.Response.WriteAsync(body);
            return;
        }
        auditUserProvider.SetCurrentUser(userId.Trim());
        await next(context);
    }
}
