using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.OrderUseCases.GetPagedOrders;

public record GetPagedOrdersRequest : IRequest<PagedResult<OrderPagedResponse>>, ICachedRequest
{
    public required Guid UserId { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int Year { get; init; }
    public SortDirection SortDirection { get; init; }

    public string CacheKey =>
        CacheKeys.OrderPaged(Page, PageSize, SortDirection.ToString().ToLower(), UserId.ToString(), Year.ToString());
    public string CacheSetKey => CacheKeys.OrderSet(UserId.ToString());
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);

    public static Result<GetPagedOrdersRequest> FromDto(GetPagedOrdersRequestDto dto, int year, Guid userId)
    {
        if (!Enum.TryParse(dto.Direction, ignoreCase: true, out SortDirection parsedDirection))
            return Result.Failure<GetPagedOrdersRequest>(
                Error.Validation(
                    "SortDirection",
                    $"Invalid Sort Direction Type, Allowed values are: {string.Join(", ", Enum.GetNames(typeof(SortDirection)))}"
                )
            );

        var request = new GetPagedOrdersRequest
        {
            UserId = userId,
            Page = dto.Page ?? 1,
            PageSize = dto.PageSize ?? 10,
            Year = year,
            SortDirection = parsedDirection,
        };

        return Result.Success(request);
    }
}
