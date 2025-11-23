using System.Data;
using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.Events;
using Application.Services.Product.Mapping;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        using IDbTransaction transaction = await unitOfWork
            .BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

        try
        {
            Domain.Entities.Product? product = await productRepository
                .GetProductForUpdateAsync(request.Id, cancellationToken);

            if(product is null)
                return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

            if(product.Name != request.Name)
            {
                var exist = await productRepository.AnyAsync(
                    x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

                if(exist)
                    return Result.Failure(Errors.ProductNameAlreadyExists, HttpStatusCode.Conflict);
            }

            if(request.Images?.Any() == true)
            {
                IEnumerable<string> oldImages = product.ProductImages.Select(x => x.Image);
                IEnumerable<string> imagesToDelete = oldImages.Except(request.Images);

                if(imagesToDelete.Any())
                {
                    var productUpdatedEvent = new ProductUpdatedEvent(imagesToDelete);
                    await eventDispatcher.DispatchAsync(productUpdatedEvent, cancellationToken);
                }
            }

            product.UpdateFrom(request);

            await unitOfWork.CompleteAsync(cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
