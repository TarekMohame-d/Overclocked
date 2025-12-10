using System.Net;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Mapping;
using Overclocked.Domain.Common.Results;
using productEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Commands;

public sealed partial class ProductCommands
{
    public async Task<Result> CreateProductCommandHandler(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        productEntity product = command.ToEntity();

        await productRepository.AddAsync(product, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
