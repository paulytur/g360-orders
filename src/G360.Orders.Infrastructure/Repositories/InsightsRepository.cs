using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Models;
using G360.Orders.Domain.Interfaces;
using G360.Orders.Infrastructure.Data;

namespace G360.Orders.Infrastructure.Repositories;

public class InsightsRepository(OrdersDbContext context) : IInsightsRepository
{
    public async Task<InsightsResult> GetInsightsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var orders = context.Orders.Where(o => !o.IsDeleted);
        var orderDetails = context.OrderDetails.Where(d => !d.IsDeleted);
        var pizzas = context.Pizzas.Where(p => !p.IsDeleted);

        var totalOrders = await orders.CountAsync(cancellationToken);

        var revenueQuery = from d in orderDetails
            join o in orders on d.OrderId equals o.Id
            join p in pizzas on d.PizzaId equals p.Id
            select new { d.Quantity, p.Price };
        
        var totalRevenue = await revenueQuery.SumAsync(x => x.Quantity * x.Price, cancellationToken);
        var totalPizzasSold = await revenueQuery.SumAsync(x => x.Quantity, cancellationToken);

        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : decimal.Zero;

        var entityCounts = new EntityCountsResult
        {
            Orders = totalOrders,
            Pizzas = await pizzas.CountAsync(cancellationToken),
            PizzaTypes = await context.PizzaTypes.Where(t => !t.IsDeleted).CountAsync(cancellationToken),
            Ingredients = await context.Ingredients.Where(i => !i.IsDeleted).CountAsync(cancellationToken),
            Categories = await context.Categories.Where(c => !c.IsDeleted).CountAsync(cancellationToken)
        };

        var topSellingPizza = await (from d in orderDetails
                join o in orders on d.OrderId equals o.Id
                join p in pizzas on d.PizzaId equals p.Id
                group new { d.Quantity, p.Code, p.PizzaTypeId, p.Size, p.Price } by new { p.Id, p.Code, p.PizzaTypeId, p.Size, p.Price } into g
                orderby g.Sum(x => x.Quantity) descending
                select new TopPizzaResult
                {
                    PizzaId = g.Key.Id,
                    PizzaCode = g.Key.Code,
                    PizzaTypeId = g.Key.PizzaTypeId,
                    PizzaSize = g.Key.Size,
                    UnitPrice = g.Key.Price,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.Price)
                })
            .FirstOrDefaultAsync(cancellationToken);

        var nowUtc = DateTime.UtcNow;
        var effectiveStart = startDate ?? nowUtc.Date.AddDays(-30);
        var effectiveEnd = endDate ?? nowUtc;
        if (effectiveStart > effectiveEnd)
        {
            (effectiveStart, effectiveEnd) = (effectiveEnd, effectiveStart);
        }

        var ordersWithRevenue = await (from d in orderDetails
                join o in orders on d.OrderId equals o.Id
                join p in pizzas on d.PizzaId equals p.Id
                where o.CreatedDatetime >= effectiveStart && o.CreatedDatetime <= effectiveEnd
                select new { o.Id, o.CreatedDatetime, Revenue = d.Quantity * p.Price })
            .ToListAsync(cancellationToken);

        var dailyTrend = ordersWithRevenue
            .GroupBy(x => x.CreatedDatetime.Date)
            .Select(g => new DailySalesTrendItem
            {
                Date = g.Key,
                OrderCount = g.Select(x => x.Id).Distinct().Count(),
                Revenue = g.Sum(x => x.Revenue)
            })
            .OrderBy(x => x.Date)
            .ToList();

        var peakHoursRaw = await (from d in orderDetails
                join o in orders on d.OrderId equals o.Id
                join p in pizzas on d.PizzaId equals p.Id
                where o.CreatedDatetime >= effectiveStart && o.CreatedDatetime <= effectiveEnd
                select new { o.Id, o.CreatedDatetime, Revenue = d.Quantity * p.Price })
            .ToListAsync(cancellationToken);
        var peakHours = peakHoursRaw
            .GroupBy(x => x.CreatedDatetime.Hour)
            .Select(g => new PeakHourItem
            {
                Hour = g.Key,
                OrderCount = g.Select(x => x.Id).Distinct().Count(),
                Revenue = g.Sum(x => x.Revenue)
            })
            .OrderBy(x => x.Hour)
            .ToList();

        var periodRevenue = ordersWithRevenue.Sum(x => x.Revenue);

        var revenueOverview = new RevenueOverviewResult
        {
            TotalRevenue = totalRevenue,
            PeriodRevenue = periodRevenue,
            StartDate = effectiveStart,
            EndDate = effectiveEnd
        };

        var salesByCategory = await (from d in orderDetails
                join o in orders on d.OrderId equals o.Id
                join p in pizzas on d.PizzaId equals p.Id
                group new { o.Id, d.Quantity, p.Price } by p.PizzaTypeId into g
                select new SalesByCategoryItem
                {
                    CategoryName = Convert.ToString(g.Key),
                    OrderCount = g.Select(x => x.Id).Distinct().Count(),
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.Price)
                })
            .OrderByDescending(x => x.Revenue)
            .ToListAsync(cancellationToken);

        static DateTime GetWeekStart(DateTime date)
        {
            var d = date.Date;
            return d.AddDays(-(int)d.DayOfWeek);
        }

        var weeklyData = ordersWithRevenue;

        var weeklyAnalysis = weeklyData
            .GroupBy(x => GetWeekStart(x.CreatedDatetime))
            .Select(g => new WeeklyAnalysisItem
            {
                WeekStart = g.Key,
                OrderCount = g.Select(x => x.Id).Distinct().Count(),
                Revenue = g.Sum(x => x.Revenue)
            })
            .OrderBy(x => x.WeekStart)
            .ToList();

        return new InsightsResult
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalPizzasSold = totalPizzasSold,
            EntityCounts = entityCounts,
            AverageOrderValue = averageOrderValue,
            DailySalesTrend = dailyTrend,
            PeakHours = peakHours,
            RevenueOverview = revenueOverview,
            SalesByCategory = salesByCategory,
            TopSellingPizza = topSellingPizza,
            WeeklyAnalysis = weeklyAnalysis
        };
    }
}
