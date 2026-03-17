using G360.Orders.Application.Helpers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Domain.Models;
using MediatR;

namespace G360.Orders.Application.Handlers;

public class GetInsightsQueryHandler(IInsightsRepository insightsRepository) : IRequestHandler<GetInsightsQuery, Response<InsightsResult>>
{
    public async Task<Response<InsightsResult>> Handle(GetInsightsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await insightsRepository.GetInsightsAsync(
                request.StartDate,
                request.EndDate,
                cancellationToken);
            return new Response<InsightsResult>(true, ["Insights retrieved successfully."], result);
        }
        catch (Exception ex)
        {
            return new Response<InsightsResult>(false, [$"Failed to retrieve insights: {ex.Message}"]);
        }
    }
}
