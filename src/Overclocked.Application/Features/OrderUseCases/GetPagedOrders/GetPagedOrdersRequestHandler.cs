using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.Application.Features.OrderUseCases.Mapping;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.OrderUseCases.GetPagedOrders;

public class GetPagedOrdersRequestHandler(IOrderReadRepository orderRepository)
    : IRequestHandler<GetPagedOrdersRequest, PagedResult<OrderPagedResponse>>
{
    public async Task<Result<PagedResult<OrderPagedResponse>>> Handle(GetPagedOrdersRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);
        var totalCount = await orderRepository.CountAsync(userId, request.Year, ct);

        if (totalCount == 0)
            return Result.Success(PagedResult<OrderPagedResponse>.Empty(request.Page, request.PageSize));

        List<Order> orders = await orderRepository.GetPagedAsync(
            userId,
            request.Page,
            request.PageSize,
            request.Year,
            request.SortDirection,
            ct
        );

        return Result.Success(PagedResult<OrderPagedResponse>.Create(orders.ToDto(), request.Page, request.PageSize, totalCount));
    }
}
