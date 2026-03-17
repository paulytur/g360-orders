namespace G360.Orders.Application.Helpers;

/// <summary>
/// Represents a basic response with success status and messages.
/// </summary>
public class Response(bool success = true, IEnumerable<string>? messages = null)
{
    public bool Success { get; init; } = success;
    public ICollection<string> Messages { get; init; } = messages is not null ? new List<string>(messages) : [];
}

/// <summary>
/// Represents a response with data of type <typeparamref name="T"/>.
/// </summary>
public class Response<T>(bool success = true, IEnumerable<string>? messages = null, T? data = default) : Response(success, messages)
{
    public T? Data { get; init; } = data;
}

/// <summary>
/// Represents a paged response with data of type <typeparamref name="T"/>.
/// </summary>
public class PagedResponse<T>(
    bool success = true,
    IEnumerable<string>? messages = null,
    int? page = null,
    int? pageSize = null,
    int? totalCount = null,
    IEnumerable<T>? data = null) : Response(success, messages)
{
    public int? Page { get; init; } = page;
    public int? PageSize { get; init; } = pageSize;
    public int? TotalCount { get; init; } = totalCount;
    public IEnumerable<T> Data { get; init; } = data ?? [];
}
