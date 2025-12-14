using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        ProductEntity? product = await productRepository
            .FindAsync(ProductId.Create(command.Id), cancellationToken);

        if(product is null)
        {
            return Result.Failure(ProductErrors.ProductNotFound(command.Id));
        }

        product.DeleteProduct();

        productRepository.Delete(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
