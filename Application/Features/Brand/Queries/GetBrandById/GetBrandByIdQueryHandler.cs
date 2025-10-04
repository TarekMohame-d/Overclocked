using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Brand.Mapping;

namespace Application.Features.Brand.Queries.GetBrandById;

public class GetBrandByIdQueryHandler : IQueryHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandByIdQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result<BrandDto>.Failure(
                Errors.BrandNotFound,
                HttpStatusCode.NotFound);

        return brand.ToDto();
    }
}
