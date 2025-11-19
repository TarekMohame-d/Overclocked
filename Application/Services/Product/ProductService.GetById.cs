using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result<ProductResponse>> GetProductByIdAsync(
        GetProductByIdRequest request,
        CancellationToken cancellationToken
    )
    {
        ProductResponse? productDto = await productRepository.GetProductDetailsAsync(request.Id, cancellationToken);

        return productDto is null
            ? Result<ProductResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound)
            : Result<ProductResponse>.Success(productDto);
    }
}
