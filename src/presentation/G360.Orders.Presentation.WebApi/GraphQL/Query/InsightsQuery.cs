using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Models;
using HotChocolate.Types;
using MediatR;

namespace G360.Orders.Presentation.WebApi.GraphQL;

/// <summary>GraphQL queries for order and catalog insights (counts, revenue, top-selling pizza).</summary>
[GraphQLDescription("GraphQL queries for order and catalog insights (counts, revenue, top-selling pizza).")]
[ExtendObjectType(typeof(OrderQuery))]
public class InsightsQuery
{
    /// <summary>Returns aggregated insights: total orders, revenue, trends, peak hours, sales by category, top-selling pizza. Requires X-User-Id request header.</summary>
    [GraphQLDescription("Returns aggregated insights: total orders, revenue, trends, peak hours, sales by category, top-selling pizza. Requires X-User-Id request header.")]
    public async Task<Response<InsightsResult>> GetInsights(
        [Service] IMediator mediator,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = new GetInsightsQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };
        return await mediator.Send(query);
    }
}
