using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface IProductService
{
    Task<Result<ProductResponse>> GetProductByIdAsync(
        GetProductByIdRequest request,
        CancellationToken cancellationToken);
    Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(
        GetPagedProductsRequest request,
        CancellationToken cancellationToken);
    Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteProductAsync(Guid productId, CancellationToken cancellationToken);
}
