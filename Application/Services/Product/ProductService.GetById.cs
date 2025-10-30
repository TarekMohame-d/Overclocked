using System.Net;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result<ProductResponse>> GetProductByIdAsync(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var productDto = await _productRepository.GetProductAsync(request.Id, cancellationToken);

        if (productDto is null)
            return Result<ProductResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        return Result<ProductResponse>.Success(productDto);
    }
}
