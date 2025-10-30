using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Product.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Product.DTOs.Request;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync([request.Id], cancellationToken);

        if (product is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        if (product.Name != request.Name)
        {
            bool exist = await _productRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.ProductNameAlreadyExists, HttpStatusCode.Conflict);
        }

        product.UpdateFrom(request);

        _productRepository.Update(product);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
