using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.Application.Features.OrderUseCases.GetPagedOrders;
using Overclocked.Domain.Common.Shared.ValueObjects.Address;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;
using SortDirection = Overclocked.Application.Common.Enums.SortDirection;

namespace Overclocked.Unit.Tests.OrderTests;

public class GetPagedOrdersRequestHandlerTests
{
    private readonly IOrderReadRepository _orderRepositoryMock;
    private readonly GetPagedOrdersRequestHandler _handler;

    public GetPagedOrdersRequestHandlerTests()
    {
        _orderRepositoryMock = Substitute.For<IOrderReadRepository>();
        _handler = new GetPagedOrdersRequestHandler(_orderRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenCountIsZero()
    {
        // Arrange
        var request = new GetPagedOrdersRequest { UserId = Guid.NewGuid(), Page = 1, PageSize = 10, Year = DateTime.UtcNow.Year };
        _orderRepositoryMock.CountAsync(Arg.Any<UserId>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(0);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalItemCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDtos_WhenOrdersExist()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var request = new GetPagedOrdersRequest { UserId = userId.Value, Page = 1, PageSize = 10, Year = DateTime.UtcNow.Year, SortDirection = SortDirection.Desc };
        var order = CreateOrder(userId);
        order.CalculateTotalPrice();
        
        _orderRepositoryMock.CountAsync(userId, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(1);
        _orderRepositoryMock.GetPagedAsync(userId, 1, 10, Arg.Any<int>(), SortDirection.Desc, Arg.Any<CancellationToken>())
            .Returns(new List<Order> { order });

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count().ShouldBe(1);
        result.Value.Items.First().OrderId.ShouldBe(order.Id.Value);
        result.Value.TotalItemCount.ShouldBe(1);
    }

    private Order CreateOrder(UserId? userId = null)
    {
        var address = Address.Create(1, "B", "S", "C", "1", "D").Value;
        return Order.Create(userId ?? UserId.Create(Guid.NewGuid()), address);
    }
}