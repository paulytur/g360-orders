namespace G360.Orders.Domain.Models;

/// <summary>
/// Aggregated insights for the ordering system (counts, revenue, top pizzas, trends).
/// </summary>
public class InsightsResult
{
    /// <summary>Total number of orders (excluding soft-deleted).</summary>
    public int TotalOrders { get; set; }

    /// <summary>Total revenue from all order details (quantity × pizza price), excluding soft-deleted.</summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>Total number of pizza units sold across all orders.</summary>
    public int TotalPizzasSold { get; set; }

    /// <summary>Entity counts for catalog (excluding soft-deleted).</summary>
    public EntityCountsResult EntityCounts { get; set; } = new();

    /// <summary>Average revenue per order (TotalRevenue / TotalOrders, or 0 if no orders).</summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>Sales per day over the requested date-time range.</summary>
    public List<DailySalesTrendItem> DailySalesTrend { get; set; } = [];

    /// <summary>Orders and revenue grouped by hour of day (0–23) within the requested range.</summary>
    public List<PeakHourItem> PeakHours { get; set; } = [];

    /// <summary>Revenue summary: total (all time) and for the requested date-time range.</summary>
    public RevenueOverviewResult RevenueOverview { get; set; } = new();

    /// <summary>Revenue and quantity sold grouped by pizza type (Pizza.Type).</summary>
    public List<SalesByCategoryItem> SalesByCategory { get; set; } = [];

    /// <summary>The single top-selling pizza by quantity sold.</summary>
    public TopPizzaResult? TopSellingPizza { get; set; }

    /// <summary>Sales per week over the requested date-time range.</summary>
    public List<WeeklyAnalysisItem> WeeklyAnalysis { get; set; } = [];
}

/// <summary>Sales metrics for a single day.</summary>
public class DailySalesTrendItem
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

/// <summary>Orders and revenue for a single hour of the day (0–23).</summary>
public class PeakHourItem
{
    public int Hour { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

/// <summary>Revenue summary: total (all time) and for the requested date-time range.</summary>
public class RevenueOverviewResult
{
    /// <summary>Total revenue (all time).</summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>Revenue in the requested date-time range.</summary>
    public decimal PeriodRevenue { get; set; }

    /// <summary>Start of the range used (inclusive).</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>End of the range used (inclusive).</summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>Sales grouped by category (pizza type name).</summary>
public class SalesByCategoryItem
{
    public string CategoryName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

/// <summary>Sales metrics for a single week.</summary>
public class WeeklyAnalysisItem
{
    public DateTime WeekStart { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

/// <summary>
/// Counts of main entities in the system.
/// </summary>
public class EntityCountsResult
{
    public int Orders { get; set; }
    public int Pizzas { get; set; }
    public int PizzaTypes { get; set; }
    public int Ingredients { get; set; }
    public int Categories { get; set; }
}

/// <summary>
/// A pizza ranked by total quantity sold.
/// </summary>
public class TopPizzaResult
{
    public long PizzaId { get; set; }
    public string PizzaCode { get; set; } = string.Empty;
    public long? PizzaTypeId { get; set; }
    public string? PizzaSize { get; set; }
    public decimal UnitPrice { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal Revenue { get; set; }
}
