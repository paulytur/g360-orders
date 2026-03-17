using G360.Orders.Application.Helpers;
using G360.Orders.Domain.Models;
using MediatR;

namespace G360.Orders.Application.Queries;

/// <summary>
/// Query to retrieve aggregated insights (counts, revenue, top-selling pizza, trends).
/// </summary>
public class GetInsightsQuery : IRequest<Response<InsightsResult>>
{
    /// <summary>Start of the date-time range for time-based insights (inclusive). When null with EndDate null, defaults to 30 days ago.</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>End of the date-time range for time-based insights (inclusive). When null with StartDate null, defaults to now.</summary>
    public DateTime? EndDate { get; set; }
}
