using G360.Orders.Domain.Models;

namespace G360.Orders.Domain.Interfaces;

/// <summary>
/// Provides aggregated insights (counts, revenue, top-selling pizza) for reporting.
/// </summary>
public interface IInsightsRepository
{
    /// <summary>
    /// Returns order and catalog insights, excluding soft-deleted data.
    /// </summary>
    /// <param name="startDate">Start of the date-time range for time-based insights (inclusive). When null with endDate null, defaults to 30 days ago.</param>
    /// <param name="endDate">End of the date-time range for time-based insights (inclusive). When null with startDate null, defaults to now.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<InsightsResult> GetInsightsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
