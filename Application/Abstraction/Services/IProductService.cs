using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;

namespace Application.Abstraction.Services;

public interface IProductService
{
    Task<Result<ProductResponse>> GetProductByIdAsync(GetProductByIdRequest request, CancellationToken cancellationToken);
    Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(GetPagedProductsQuery query, CancellationToken cancellationToken);
    Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken);
}
