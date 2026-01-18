using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ProductUseCases.DeleteProduct;

public class DeleteProductRequestHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductRequest>
{
    public async Task<Result> Handle(DeleteProductRequest request, CancellationToken ct)
    {
        Product? product = await productRepository.FindAsync(ProductId.Create(request.Id), ct);

        if (product is null)
            return Result.Failure(ProductErrors.ProductNotFound(request.Id));

        product.DeleteProductImages();

        productRepository.Remove(product);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
