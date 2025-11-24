using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface IBrandService
{
    Task<Result<BrandResponse>> GetBrandByIdAsync(GetBrandByIdRequest request, CancellationToken cancellationToken);
    Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(
        GetAllBrandsRequest request,
        CancellationToken cancellationToken);
    Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteBrandAsync(Guid brandId, CancellationToken cancellationToken);
}
