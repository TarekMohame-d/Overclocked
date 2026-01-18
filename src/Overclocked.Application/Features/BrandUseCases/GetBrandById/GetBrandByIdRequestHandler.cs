using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.BrandUseCases.Mapping;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.BrandUseCases.GetBrandById;

public sealed class GetBrandByIdRequestHandler(IBrandReadRepository brandRepository)
    : IRequestHandler<GetBrandByIdRequest, BrandResponse>
{
    public async Task<Result<BrandResponse>> Handle(GetBrandByIdRequest request, CancellationToken ct)
    {
        Brand? brand = await brandRepository.GetByIdAsync(BrandId.Create(request.Id), ct);

        return brand is null
            ? Result.Failure<BrandResponse>(BrandErrors.BrandNotFound(request.Id))
            : Result.Success(brand.ToDto());
    }
}
