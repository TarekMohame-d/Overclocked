using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Product.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Product.DTOs.Request;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = request.ToEntity();

        await _productRepository.AddAsync(product);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
