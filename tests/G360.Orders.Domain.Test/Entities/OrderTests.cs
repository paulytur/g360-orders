using G360.Orders.Domain.Entities;
using Xunit;

namespace G360.Orders.Domain.Test.Entities;

public class OrderTests
{
    [Fact]
    public void Order_Can_Be_Created()
    {
        var order = new Order
        {
            Id = 1,
            IsDeleted = false,
            CreatedBy = "user1",
            CreatedDatetime = DateTime.UtcNow,
            UpdatedBy = "user1",
            UpdatedDatetime = DateTime.UtcNow
        };
        Assert.Equal(1, order.Id);
        Assert.False(order.IsDeleted);
        Assert.Equal("user1", order.CreatedBy);
    }
}
