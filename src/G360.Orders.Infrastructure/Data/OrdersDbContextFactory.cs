using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace G360.Orders.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating OrdersDbContext when running EF Core tools (e.g. migrations).
/// </summary>
public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        // orders.db in the Infrastructure project folder (works when run from solution or project dir)
        var assemblyDir = Path.GetDirectoryName(typeof(OrdersDbContextFactory).Assembly.Location)
            ?? Directory.GetCurrentDirectory();
        var projectDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", ".."));
        var dbPath = Path.Combine(projectDir, "orders.db");
        var connectionString = $"Data Source={dbPath}";

        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
