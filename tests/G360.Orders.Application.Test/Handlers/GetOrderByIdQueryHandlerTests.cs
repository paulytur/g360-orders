using G360.Orders.Application.Handlers;
using G360.Orders.Application.Queries;
using G360.Orders.Domain.Entities;
using G360.Orders.Domain.Interfaces;
using Moq;
using Xunit;

namespace G360.Orders.Application.Test.Handlers;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsOrder()
    {
        var order = new Order { Id = 1, CreatedDatetime = DateTime.UtcNow, UpdatedDatetime = DateTime.UtcNow };
        var repo = new Mock<IRepository<Order>>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var handler = new GetOrderByIdQueryHandler(repo.Object);
        var result = await handler.Handle(new GetOrderByIdQuery { Id = 1 }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsFailure()
    {
        var repo = new Mock<IRepository<Order>>();
        repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var handler = new GetOrderByIdQueryHandler(repo.Object);
        var result = await handler.Handle(new GetOrderByIdQuery { Id = 999 }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }
}
