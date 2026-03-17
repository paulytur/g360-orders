using G360.Orders.Domain.Entities;
using G360.Orders.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace G360.Orders.Infrastructure.Test.Data;

public class OrdersDbContextTests
{
    [Fact]
    public async Task Can_Create_And_Save_Order()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        try
        {
            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var context = new OrdersDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var order = new Order
            {
                IsDeleted = false,
                CreatedBy = "test",
                CreatedDatetime = DateTime.UtcNow,
                UpdatedBy = "test",
                UpdatedDatetime = DateTime.UtcNow
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            Assert.True(order.Id > 0);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
