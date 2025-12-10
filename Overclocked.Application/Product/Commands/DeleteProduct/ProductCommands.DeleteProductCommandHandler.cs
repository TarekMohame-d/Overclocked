using System.Net;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Commands;

public sealed partial class ProductCommands
{
    public async Task<Result> DeleteProductCommandHandler(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        ProductEntity? product = await productRepository.SingleOrDefaultAsync(
            x => x.Id == ProductId.Create(command.Id),
            asNoTracking: false,
            cancellationToken);

        if(product is null)
        {
            return Result.Failure(ProductErrors.ProductNotFound(command.Id), HttpStatusCode.NotFound);
        }

        product.DeleteProduct();

        productRepository.Delete(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
