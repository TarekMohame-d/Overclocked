using System.Net;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.Mapping;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Product product = request.ToEntity();

        await productRepository.AddAsync(product, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
