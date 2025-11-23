using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Product.Events;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result> DeleteProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        Domain.Entities.Product? product = await productRepository
            .GetProductWithImagesAsync(productId, cancellationToken);

        if(product is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        if(product.ProductImages.Count != 0)
        {
            IEnumerable<string> images = product.ProductImages.Select(x => x.Image);

            ProductDeletedEvent productDeletedEvent = new(images);
            await eventDispatcher.DispatchAsync(productDeletedEvent, cancellationToken);
        }

        productRepository.Delete(product);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
